#region GPL statement
/*Epic Edit is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.*/
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.ItemProba;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.UI.Gfx;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace EpicEdit.Rom
{
	public enum Regions
	{
		Jap,
		US,
		Euro
	};

	public enum ItemType
	{
		Mushroom,
		Feather,
		Star,
		Banana,
		GreenShell,
		RedShell,
		Ghost,
		Coin,
		Lightning,
	}

	/// <summary>
	/// The Super Mario Kart Game class, which contains all of the game data.
	/// </summary>
	public sealed class Game : IDisposable
	{
		/// <summary>
		/// Number of GP tracks per Group (Cup).
		/// </summary>
		public const int GPTracksPerGroupCount = 5;

		/// <summary>
		/// Number of GP Groups (Cups).
		/// </summary>
		public const int GPGroupCount = 4;

		/// <summary>
		/// Number of GP tracks.
		/// </summary>
		public const int GPTrackCount = GPGroupCount * GPTracksPerGroupCount;

		/// <summary>
		/// Number of battle tracks.
		/// </summary>
		public const int BattleTrackCount = 4;

		/// <summary>
		/// Number of battle tracks.
		/// </summary>
		public const int BattleTrackGroupCount = 1;

		/// <summary>
		/// Total number of tracks (GP tracks + battle tracks).
		/// </summary>
		public const int TrackCount = GPTrackCount + BattleTrackCount;

		/// <summary>
		/// Total number of track groups (GP and Battle).
		/// </summary>
		public const int TotalTrackGroupCount = GPGroupCount + BattleTrackGroupCount;

		/// <summary>
		/// Number of themes.
		/// </summary>
		public const int ThemeCount = 8;

		private static class RomSize
		{
			internal const int Size256 = 256 * 1024; // The smallest SNES ROM size possible: 256 KiB (2 megabits), and the step value between each possible ROM sizes.
			internal const int Size512 = 512 * 1024; // The size of the original Super Mario Kart ROM: 512 KiB (4 megabits).
			internal const int Size768 = 768 * 1024; // 768 KiB (6 megabits)
			internal const int Size1024 = 1024 * 1024; // The limit up to which Epic Edit can write data: 1024 KiB (8 megabits).
			// Epic Edit doesn't save data beyond 1024 KiB because offsets don't go beyond 0xFFFFF.
			internal const int Size2048 = 2048 * 1024; // 16 megabits
			internal const int Size4096 = 4096 * 1024; // 32 megabits
			//internal const int Size6144 = 6144 * 1024; // 48 megabits
			internal const int Size8192 = 8192 * 1024; // 64 megabits
		}

		#region Private members

		/// <summary>
		/// The path to the loaded ROM file.
		/// </summary>
		private string filePath;

		/// <summary>
		/// Some ROMs have a 512-byte header.
		/// </summary>
		private byte[] romHeader;

		/// <summary>
		/// Buffer that contains all of the ROM data.
		/// </summary>
		private byte[] romBuffer;

		/// <summary>
		/// The offsets to find the needed data in the ROM.
		/// </summary>
		private Offsets offsets;

		/// <summary>
		/// The track groups, each of which contains several tracks.
		/// </summary>
		private TrackGroup[] trackGroups;

		/// <summary>
		/// All the available track themes.
		/// </summary>
		private Themes themes;

		/// <summary>
		/// The item probabilities for all the tracks and race types.
		/// </summary>
		private ItemProbabilities itemProbabilities;

		/// <summary>
		/// The different overlay tile sizes.
		/// </summary>
		private OverlayTileSizes overlayTileSizes;

		/// <summary>
		/// The different overlay tile patterns.
		/// </summary>
		private OverlayTilePatterns overlayTilePatterns;

		/// <summary>
		/// The item icons.
		/// </summary>
		private Bitmap[] itemIcons;

		private bool modified;

		#endregion Private members

		/// <param name="filePath">The path to the ROM file.</param>
		public Game(string filePath)
		{
			this.filePath = filePath;
			this.LoadRom();
			this.ValidateRom();
			this.LoadData();
		}

		#region Read ROM & Validate

		/// <summary>
		/// Loads all of the ROM content in a buffer.
		/// </summary>
		private void LoadRom()
		{
			if (!this.filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
			{
				this.LoadUnzippedRom();
			}
			else
			{
				this.LoadZippedRom();
			}
		}

		private void LoadUnzippedRom()
		{
			using (FileStream fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
			using (BinaryReader br = new BinaryReader(fs))
			{
				int size = (int)br.BaseStream.Length;
				this.romBuffer = br.ReadBytes(size);
			}
		}

		private void LoadZippedRom()
		{
			try
			{
				using (FileStream fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
				using (ZipInputStream zipInStream = new ZipInputStream(fs))
				{
					ZipEntry entry;

					while ((entry = zipInStream.GetNextEntry()) != null)
					{
						if (!entry.IsFile || !entry.CanDecompress || entry.IsCrypted)
						{
							continue;
						}

						String fileExt = Path.GetExtension(entry.Name);
						if (".bin".Equals(fileExt, StringComparison.OrdinalIgnoreCase) ||
							".fig".Equals(fileExt, StringComparison.OrdinalIgnoreCase) ||
							".sfc".Equals(fileExt, StringComparison.OrdinalIgnoreCase) ||
							".smc".Equals(fileExt, StringComparison.OrdinalIgnoreCase) ||
							".swc".Equals(fileExt, StringComparison.OrdinalIgnoreCase))
						{
							using (ZipFile zf = new ZipFile(this.filePath))
							using (InflaterInputStream iis = (InflaterInputStream)zf.GetInputStream(entry))
							{
								this.romBuffer = new byte[entry.Size];
								iis.Read(this.romBuffer, 0, romBuffer.Length);

								// Update the file path so that it includes the name of the ROM file
								// inside the zip, rather than the name of the zip archive itself
								this.filePath =
									this.filePath.Substring(0, this.filePath.LastIndexOf(Path.DirectorySeparatorChar) + 1) + entry.Name;

								return;
							}
						}
					}
				}

				throw new ZipException("No ROM found in the zip archive.");
			}
			catch (ZipException ex)
			{
				// NOTE: we don't throw a ZipException,
				// so that the presence of the SharpZipLib assembly
				// is not required to load uncompressed ROMs.
				throw new InvalidDataException(ex.Message, ex);
			}
		}

		/// <summary>
		/// Function that checks if the ROM is valid. If it is, the ROM header value is initialized.
		/// Either there is no ROM header (value 0), or if there is one, it's usually a 512-byte one.
		/// The ROM header is used by adding its value to all ROM offsets, to find the relevent data.
		/// </summary>
		private void ValidateRom()
		{
			if (!this.IsSnesRom())
			{
				throw new InvalidDataException("\"" + this.FileName + "\" is not an SNES ROM.");
			}

			if (!this.IsSuperMarioKart())
			{
				throw new InvalidDataException("\"" + this.FileName + "\" is not a Super Mario Kart ROM.");
			}
		}

		/// <summary>
		/// Checks whether the file loaded is a Super Nintendo ROM.
		/// </summary>
		private bool IsSnesRom()
		{
			int romHeaderSize = this.romBuffer.Length % RomSize.Size256;

			if (romHeaderSize == 512)
			{
				this.romHeader = new byte[512];
				Array.Copy(this.romBuffer, this.romHeader, this.romHeader.Length);

				byte[] romBufferWithoutHeader = new byte[this.romBuffer.Length - this.romHeader.Length];
				Array.Copy(this.romBuffer, this.romHeader.Length, romBufferWithoutHeader, 0, romBufferWithoutHeader.Length);
				this.romBuffer = romBufferWithoutHeader;
			}
			else if (romHeaderSize != 0)
			{
				// Wrong header size
				return false;
			}

			if (this.romBuffer.Length < RomSize.Size256 || // ROM size < 256 KiB
				this.romBuffer.Length > RomSize.Size8192) // ROM size > 8 MiB
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Checks whether the file loaded is Super Mario Kart.
		/// </summary>
		private bool IsSuperMarioKart()
		{
			if (this.romBuffer.Length < RomSize.Size512)
			{
				return false;
			}

			byte cartType = this.romBuffer[0xFFD6]; // Cartridge type. SMK has 05 here, if this byte in any SNES ROM is not 05 then it is not a battery backed DSP-1 game
			byte cartRamSize = this.romBuffer[0xFFD8]; // Cart RAM size. SMK has 01 here, to say that there's 2 KiB of oncart RAM
			byte cartRomType = this.romBuffer[0xFFD5]; // SMK has 31 here, to indicate a HiROM FastROM game

			if (cartType != 0x05 || cartRamSize != 0x01 || cartRomType != 0x31)
			{
				return false;
			}

			return true;
		}

		#endregion Read ROM & Validate

		#region Load data

		/// <summary>
		/// Retrieves all the needed data from the game, such as tracks and themes.
		/// </summary>
		private void LoadData()
		{
			this.offsets = new Offsets(this.romBuffer, this.Region);

			this.trackGroups = new TrackGroup[Game.TotalTrackGroupCount];

			string[] names = this.LoadTrackNames();

			this.themes = new Themes(this.romBuffer, this.offsets, names);

			this.overlayTileSizes = new OverlayTileSizes(this.romBuffer, this.offsets);

			this.overlayTilePatterns = new OverlayTilePatterns(this.romBuffer, this.offsets, this.overlayTileSizes);

			byte[] trackThemes = Utilities.ReadBlock(this.romBuffer, this.offsets[Address.TrackThemes], Game.TrackCount);
			byte[] trackOrder = this.LoadTrackOrder();
			byte[][] trackNameIndex = this.LoadTrackNameIndexes();

			Offset[] mapAddresses = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Address.TrackMaps], Game.TrackCount);

			for (int i = 0; i < this.trackGroups.Length; i++)
			{
				int trackCountInGroup;
				string trackGroupName;
				if (i != this.trackGroups.Length - 1) // GP track group
				{
					trackCountInGroup = Game.GPTracksPerGroupCount;
					trackGroupName = names[i];
				}
				else // Battle track group
				{
					trackCountInGroup = Game.BattleTrackCount;
					trackGroupName = names[trackNameIndex[Game.GPTrackCount][1]];
				}

				int aiOffsetBase = (this.romBuffer[this.offsets[Address.TrackAIDataFirstAddressByte]] & 0xF) << 16;
				byte[] aiZoneOffsets = Utilities.ReadBlock(this.romBuffer, this.offsets[Address.TrackAIZones], Game.TrackCount * 2); // 2 offset bytes per track
				byte[] aiTargetOffsets = Utilities.ReadBlock(this.romBuffer, this.offsets[Address.TrackAITargets], Game.TrackCount * 2); // 2 offset bytes per track

				Track[] tracks = new Track[trackCountInGroup];

				for (int j = 0; j < trackCountInGroup; j++)
				{
					int iterator = i * 5 + j;
					int trackIndex = trackOrder[iterator];

					#region trackName parameter
					string trackName = names[trackNameIndex[iterator][1]];
					if (trackNameIndex[iterator].Length > 2) // We check if there is a track number (eg: Rainbow Road doesn't have one)
					{
						trackName += trackNameIndex[iterator][2];
					}
					#endregion trackName parameter

					#region trackTheme parameter
					int themeId = trackThemes[trackIndex] >> 1;
					Theme trackTheme = this.themes[themeId];
					#endregion trackTheme parameter

					#region trackMap parameter
					byte[] trackMap = Codec.Decompress(Codec.Decompress(this.romBuffer, mapAddresses[trackIndex]), 0, 16384);
					#endregion trackMap parameter

					#region overlayTileData parameter
					byte[] overlayTileData = this.LoadOverlayTileData(trackIndex);
					#endregion overlayTileData parameter

					#region aiZoneData & aiTargetData parameters
					int aiOffset = trackIndex * 2;

					int aiZoneDataOffset = aiOffsetBase + (aiZoneOffsets[aiOffset + 1] << 8) + aiZoneOffsets[aiOffset];
					byte[] aiZoneData = Utilities.ReadBlockUntil(this.romBuffer, aiZoneDataOffset, 0xFF);

					int aiTargetDataOffset = aiOffsetBase + (aiTargetOffsets[aiOffset + 1] << 8) + aiTargetOffsets[aiOffset];
					int aiTargetDataLength = TrackAI.ComputeTargetDataLength(aiZoneData);
					byte[] aiTargetData = Utilities.ReadBlock(romBuffer, aiTargetDataOffset, aiTargetDataLength);
					#endregion aiZoneData & aiTargetData parameters

					if (trackIndex < Game.GPTrackCount)
					{
						#region startPositionData parameter
						byte[] startPositionData = this.LoadStartPositionData(trackIndex);
						#endregion startPositionData parameter

						#region lapLineData parameter
						byte[] lapLineData = new byte[6];
						int lapLineDataOffset = this.offsets[Address.TrackLapLines] + trackIndex * lapLineData.Length;
						Array.Copy(this.romBuffer, lapLineDataOffset, lapLineData, 0, lapLineData.Length);
						#endregion lapLineData parameter

						#region objectData & objectZoneData parameters
						int objectOffset = this.offsets[Address.TrackObjects] + (trackIndex * 64);
						byte[] objectData = Utilities.ReadBlock(this.romBuffer, objectOffset, 44);
						// 16 objects * 2 coordinate bytes = 32 bytes
						// + 6 Match Race objects (Chain Chomps) * 2 coordinate bytes = 12 bytes
						// Total = 44 bytes

						int objectZoneOffset = this.GetObjectZonesOffset(trackIndex);

						byte[] objectZoneData;
						
						if (objectZoneOffset < 0)
						{
							objectZoneData = objectZoneOffset == -1 ?
								null : new byte[0];
						}
						else
						{
							objectZoneData = Utilities.ReadBlock(this.romBuffer, objectZoneOffset, 10);
						}

						#endregion objectData & objectZoneData parameters

						tracks[j] = new GPTrack(trackName, trackTheme,
												trackMap, overlayTileData,
												aiZoneData, aiTargetData,
												startPositionData, lapLineData,
												objectData, objectZoneData,
												this.overlayTileSizes,
												this.overlayTilePatterns);
					}
					else
					{
						// TODO: Load battle track start position data.
						byte[] startPositionData = null;

						tracks[j] = new BattleTrack(trackName, trackTheme,
													trackMap, overlayTileData,
													aiZoneData, aiTargetData,
													startPositionData,
													this.overlayTileSizes,
													this.overlayTilePatterns);
					}
				}

				this.trackGroups[i] = new TrackGroup(trackGroupName, tracks);
			}

			this.LoadItemProbabilities();
			this.LoadItemIcons();
		}

		private string[] LoadTrackNames()
		{
			int nameCount = this.trackGroups.Length + Game.ThemeCount;
			string[] names = new string[nameCount];
			byte[][] nameIndex = Utilities.ReadBlockGroup(this.romBuffer, this.offsets[Address.NameStrings], 2, names.Length);
			int offset;

			for (int i = 0; i < names.Length; i++)
			{
				offset = 0x10000 + (nameIndex[i][1] << 8) + new Offset(nameIndex[i][0]); // Recreates offsets from the index table loaded above
				names[i] = Utilities.DecryptRomText(Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF), this.Region);
			}

			return names;
		}

		/// <summary>
		/// Load the order of the tracks.
		/// </summary>
		/// <returns></returns>
		private byte[] LoadTrackOrder()
		{
			byte[] gpTrackOrder = Utilities.ReadBlock(this.romBuffer, this.offsets[Address.GPTrackOrder], Game.GPTrackCount);
			byte[] battleTrackOrder = Utilities.ReadBlock(this.romBuffer, this.offsets[Address.BattleTrackOrder], Game.BattleTrackCount);

			byte[] trackOrder = new byte[Game.TrackCount];

			Array.Copy(gpTrackOrder, 0, trackOrder, 0, Game.GPTrackCount);
			Array.Copy(battleTrackOrder, 0, trackOrder, Game.GPTrackCount, Game.BattleTrackCount);
			return trackOrder;
		}

		private byte[][] LoadTrackNameIndexes()
		{
			int gpTrackNamesOffset = this.offsets[Address.GPTrackNames];

			byte[][] gpTrackPointers = new byte[Game.GPTrackCount][];

			for (int i = 0; i < Game.GPGroupCount; i++)
			{
				Utilities.ReadBlockGroup(this.romBuffer, gpTrackNamesOffset, 4, Game.GPTracksPerGroupCount).CopyTo(gpTrackPointers, Game.GPTracksPerGroupCount * i);
				gpTrackNamesOffset += Game.GPTracksPerGroupCount * Game.GPGroupCount + 2; // 2 separating bytes
			}

			byte[][] battleTrackPointers = Utilities.ReadBlockGroup(this.romBuffer, this.offsets[Address.BattleTrackNames], 4, Game.BattleTrackCount);
			byte[][] trackNameIndex = new byte[Game.TrackCount][];

			for (int i = 0; i < gpTrackPointers.Length; i++)
			{
				int address = 0x10000 + (gpTrackPointers[i][1] << 8) + gpTrackPointers[i][0];
				trackNameIndex[i] = Utilities.ReadBlockUntil(this.romBuffer, address, 0xFF);
			}

			for (int i = 0; i < battleTrackPointers.Length; i++)
			{
				int address = 0x10000 + (battleTrackPointers[i][1] << 8) + battleTrackPointers[i][0];
				trackNameIndex[gpTrackPointers.Length + i] = Utilities.ReadBlockUntil(this.romBuffer, address, 0xFF);
			}

			for (int i = 0; i < trackNameIndex.Length; i++)
			{
				trackNameIndex[i][1] = (byte)(trackNameIndex[i][1] & 0xF);
			}

			return trackNameIndex;
		}

		#region Object Zones
		private int GetObjectZonesOffset(int trackIndex)
		{
			int[] reorder =
			{
				2, -1 /* Ghost Valley x */, 12, 8, 15,
				10, 17, 0, -1 /* Ghost Valley x */, 9,
				5, 13, 14, -2 /* Koopa Beach 1 */, 3,
				1, -1 /* Ghost Valley x */, 7, 4, 11
			};
			// TODO: Retrieve order dynamically from the ROM

			if (reorder[trackIndex] < 0)
			{
				return reorder[trackIndex];
			}

			int objectZonesOffset = this.offsets[Address.TrackObjectZones];
			int index = objectZonesOffset + reorder[trackIndex] * 2;
			return 0x40000 + (this.romBuffer[index + 1] << 8) + this.romBuffer[index];
		}
		#endregion Object Zones

		#region Track Overlay Tiles

		private byte[] LoadOverlayTileData(int trackIndex)
		{
			int offset = this.GetOverlayTileDataOffset(trackIndex);
			byte[] data = new byte[128];
			Array.Copy(this.romBuffer, offset, data, 0, 128);
			return data;
		}

		private void SaveOverlayTileData(int trackIndex, byte[] data)
		{
			int offset = this.GetOverlayTileDataOffset(trackIndex);
			Array.Copy(data, 0, this.romBuffer, offset, 128);
		}

		private int GetOverlayTileDataOffset(int trackIndex)
		{
			return this.offsets[Address.TrackOverlayItems] + trackIndex * 128;
		}

		#endregion Track Overlay Tiles

		#region Start Position

		private byte[] LoadStartPositionData(int trackIndex)
		{
			if (trackIndex >= Game.GPTrackCount) // If the track is a battle track
			{
				return null;
			}

			int offset = this.GetStartPositionDataOffset(trackIndex);
			byte[] data = new byte[6];
			Array.Copy(this.romBuffer, offset, data, 0, data.Length);
			return data;
		}

		private void SaveStartPositionData(int trackIndex, byte[] data)
		{
			if (data == null)
			{
				return;
			}

			int offset = this.GetStartPositionDataOffset(trackIndex);
			Array.Copy(data, 0, this.romBuffer, offset, 6);
		}

		private int GetStartPositionDataOffset(int trackIndex)
		{
			int[] reorder = { 14, 10, 7, 8, 15, 19, 16, 4, 17, 13, 6, 12, 11, 5, 18, 9, 2, 3, 1, 0 };
			// TODO: Retrieve order dynamically from the ROM
			return this.offsets[Address.TrackStartPositions] + reorder[trackIndex] * 8;
		}

		#endregion Start Position

		#region Item Probabilities

		private void LoadItemProbabilities()
		{
			this.itemProbabilities = new ItemProbabilities(this.romBuffer, this.offsets[Address.ItemProbabilities]);
		}

		#endregion Item Probabilities

		#region Item Icons

		private void LoadItemIcons()
		{
			byte[] itemIconData = Codec.Decompress(this.romBuffer, this.offsets[Address.ItemIcons]);
			int itemCount = Enum.GetValues(typeof(ItemType)).Length;
			this.itemIcons = new Bitmap[itemCount];

			for (int i = 0; i < this.itemIcons.Length; i++)
			{
				this.LoadItemIcon(itemIconData, i);
			}
		}

		private void LoadItemIcon(byte[] itemIconData, int index)
		{
			int iconPaletteOffset = this.offsets[Address.ItemIconTilesPalettes] + index * 2;

			int tileIndex = this.romBuffer[iconPaletteOffset] & 0x7F;
			byte globalPalIndex = this.romBuffer[iconPaletteOffset + 1];
			int palIndex = globalPalIndex / 16;
			int subPalIndex = globalPalIndex % 16;
			Palette palette = this.themes[0].Palettes[palIndex];
			Color[] mushPal =
			{
				palette[subPalIndex],
				palette[subPalIndex + 1],
				palette[subPalIndex + 2],
				palette[subPalIndex + 3]
			};

			this.itemIcons[index] = GraphicsConverter.GetBitmapFrom2bppPlanar(itemIconData, tileIndex * 16, mushPal, 16, 16);
		}

		#endregion Item Icons

		#endregion Load data

		/// <summary>
		/// Returns the main track groups.
		/// </summary>
		/// <returns>An array composed of the main track groups.</returns>
		public TrackGroup[] GetTrackGroups()
		{
			return this.trackGroups;
		}

		/// <summary>
		/// Returns a specific track.
		/// </summary>
		/// <param name="trackGroupId">The group id of the track.</param>
		/// <param name="trackId">The id of the track.</param>
		/// <returns>A track.</returns>
		public Track GetTrack(int trackGroupId, int trackId)
		{
			return this.trackGroups[trackGroupId][trackId];
		}

		/// <summary>
		/// Returns the track themes.
		/// </summary>
		/// <returns>The track theme collection.</returns>
		public Themes GetThemes()
		{
			return this.themes;
		}

		/// <summary>
		/// Returns the track overlay sizes.
		/// </summary>
		/// <returns>The track overlay tile size collection.</returns>
		public OverlayTileSizes GetOverlayTileSizes()
		{
			return this.overlayTileSizes;
		}

		/// <summary>
		/// Returns the track overlay sizes.
		/// </summary>
		/// <returns>The track overlay tile pattern collection.</returns>
		public OverlayTilePatterns GetOverlayTilePatterns()
		{
			return this.overlayTilePatterns;
		}

		/// <summary>
		/// Returns the file path of the loaded ROM.
		/// </summary>
		public string FilePath
		{
			get { return this.filePath; }
		}

		/// <summary>
		/// Returns the file name of the loaded ROM.
		/// </summary>
		public string FileName
		{
			get { return Path.GetFileName(this.filePath); }
		}

		private Regions Region
		{
			get
			{
				int regionAddress = 0xFFD9;
				int region = this.romBuffer[regionAddress];

				if (!Enum.IsDefined(typeof(Regions), region))
				{
					if (this.romHeader != null)
					{
						regionAddress += this.romHeader.Length;
					}

					throw new InvalidDataException(string.Format("\"{0}\" has an invalid region. Value at {1:X} must be 0, 1 or 2, was: {2:X}.",
																 this.FileName, regionAddress, region));
				}

				return (Regions)region;
			}
		}

		public string[] GetModeNames()
		{
			int offset = this.offsets[Address.ModeStrings];

			string[] modeNames = new string[3];

			// TODO: Retrieve name length dynamically from the ROM.
			// Don't know where that data is.
			int[] lengths;

			if (this.Region == Regions.Jap)
			{
				lengths = new int[] { 16, 16, 16 };
			}
			else
			{
				lengths = new int[] { 24, 20, 22 };
			}

			for (int i = 0; i < modeNames.Length; i++)
			{
				int length = lengths[i];
				byte[] hexText = new byte[length];
				Array.Copy(this.romBuffer, offset, hexText, 0, length);
				modeNames[i] = Utilities.DecryptRomTextOdd(hexText, this.Region);
				offset += length;
			}

			return modeNames;
		}

		public ItemProbabilities GetItemProbabilities()
		{
			return this.itemProbabilities;
		}

		public Bitmap GetItemIcon(ItemType type)
		{
			return this.itemIcons[(int)type];
		}

		#region Track reodering
		public void ReorderTracks(int sourceTrackGroupId, int sourceTrackId, int destinationTrackGroupId, int destinationTrackId)
		{
			if (sourceTrackGroupId == destinationTrackGroupId &&
				sourceTrackId == destinationTrackId)
			{
				return;
			}

			if (sourceTrackGroupId < 4) // GP track reordering
			{
				#region Global track array creation
				// To make the treatment easier, we simply create an array with all the GP tracks
				Track[] tracks = new Track[Game.GPTrackCount];
				for (int i = 0; i < this.trackGroups.Length - 1; i++)
				{
					Track[] groupTracks = this.trackGroups[i].GetTracks();

					for (int j = 0; j < groupTracks.Length; j++)
					{
						tracks[i * groupTracks.Length + j] = groupTracks[j];
					}
				}

				sourceTrackId = sourceTrackGroupId * 5 + sourceTrackId;
				destinationTrackId = destinationTrackGroupId * 5 + destinationTrackId;
				#endregion Global track array creation

				int trackOrderOffset = this.offsets[Address.GPTrackOrder];
				int trackNameOffset = this.offsets[Address.GPTrackNames];

				this.ReorderTracksSub(tracks, sourceTrackId, destinationTrackId, trackOrderOffset, trackNameOffset);

				#region GP track specific data update
				// Update Time Trial lap line positions
				int startingLineOffset = this.offsets[Address.TimeTrialPreviewTrackLapLines];
				byte[] sourceTrackStartingLine =
				{ this.romBuffer[startingLineOffset + sourceTrackId * 2],
					this.romBuffer[startingLineOffset + sourceTrackId * 2 + 1] };

				if (sourceTrackId < destinationTrackId)
				{
					Array.Copy(
						this.romBuffer,
						startingLineOffset + (sourceTrackId + 1) * 2,
						this.romBuffer,
						startingLineOffset + sourceTrackId * 2,
						(destinationTrackId - sourceTrackId) * 2
					);
				}
				else
				{
					Array.Copy(
						this.romBuffer,
						startingLineOffset + destinationTrackId * 2,
						this.romBuffer,
						startingLineOffset + (destinationTrackId + 1) * 2,
						(sourceTrackId - destinationTrackId) * 2
					);
				}

				this.romBuffer[startingLineOffset + destinationTrackId * 2] = sourceTrackStartingLine[0];
				this.romBuffer[startingLineOffset + destinationTrackId * 2 + 1] = sourceTrackStartingLine[1];
				#endregion GP track specific data update

				#region Update track pointers in track groups
				for (int i = 0; i < this.trackGroups.Length - 1; i++)
				{
					Track[] groupTracks = this.trackGroups[i].GetTracks();

					for (int j = 0; j < groupTracks.Length; j++)
					{
						groupTracks[j] = tracks[i * groupTracks.Length + j];
					}
				}
				#endregion Update track pointers in track groups
			}
			else // Battle track reordering
			{
				Track[] tracks = this.trackGroups[sourceTrackGroupId].GetTracks();

				int trackOrderOffset = this.offsets[Address.BattleTrackOrder];
				int trackNameOffset = this.offsets[Address.BattleTrackNames];

				this.ReorderTracksSub(tracks, sourceTrackId, destinationTrackId, trackOrderOffset, trackNameOffset);

				#region Battle track specific data update
				// Update the track shown by default when entering the battle track selection
				this.romBuffer[this.offsets[Address.FirstBattleTrack]] = this.romBuffer[trackOrderOffset];

				// Update the selection cursor positions of the battle track selection
				for (byte i = 0; i < Game.BattleTrackCount; i++)
				{
					byte value = (byte)(this.romBuffer[trackOrderOffset + i] - 0x14);
					this.romBuffer[trackOrderOffset + Game.BattleTrackCount + value] = i;
				}
				#endregion Battle track specific data update
			}

			this.modified = true;
		}

		private void ReorderTracksSub(Track[] tracks, int sourceTrackId, int destinationTrackId, int trackOrderOffset, int trackNameOffset)
		{
			Track sourceTrack = tracks[sourceTrackId];
			byte sourceTrackOrder = this.romBuffer[trackOrderOffset + sourceTrackId];

			int sourceTrackNameOffset = Game.GetTrackNameOffset(trackNameOffset, sourceTrackId);
			byte[] sourceTrackName =
			{
				this.romBuffer[sourceTrackNameOffset],
				this.romBuffer[sourceTrackNameOffset + 1]
			};

			if (sourceTrackId < destinationTrackId)
			{
				for (int i = sourceTrackId; i < destinationTrackId; i++)
				{
					this.RemapTrack(tracks, i + 1, i, trackOrderOffset, trackNameOffset);
				}
			}
			else
			{
				for (int i = sourceTrackId; i > destinationTrackId; i--)
				{
					this.RemapTrack(tracks, i - 1, i, trackOrderOffset, trackNameOffset);
				}
			}

			tracks[destinationTrackId] = sourceTrack;
			this.romBuffer[trackOrderOffset + destinationTrackId] = sourceTrackOrder;

			int destinationTrackNameOffset = Game.GetTrackNameOffset(trackNameOffset, destinationTrackId);
			this.romBuffer[destinationTrackNameOffset] = sourceTrackName[0];
			this.romBuffer[destinationTrackNameOffset + 1] = sourceTrackName[1];
		}

		private void RemapTrack(Track[] tracks, int sourceTrackId, int destinationTrackId, int trackOrderOffset, int trackNameOffset)
		{
			tracks[destinationTrackId] = tracks[sourceTrackId];
			this.romBuffer[trackOrderOffset + destinationTrackId] = this.romBuffer[trackOrderOffset + sourceTrackId];

			int destinationTrackNameOffset = Game.GetTrackNameOffset(trackNameOffset, destinationTrackId);
			int sourceTrackNameOffset = Game.GetTrackNameOffset(trackNameOffset, sourceTrackId);
			this.romBuffer[destinationTrackNameOffset] = this.romBuffer[sourceTrackNameOffset];
			this.romBuffer[destinationTrackNameOffset + 1] = this.romBuffer[sourceTrackNameOffset + 1];
		}

		private static int GetTrackNameOffset(int trackNameOffset, int trackId)
		{
			int shiftValue = (int)Math.Ceiling((double)(trackId / 5)) * 2;
			return trackNameOffset + trackId * 4 + shiftValue;
		}
		#endregion Track reodering

		#region ROM saving
		public void SaveRom(string filePath)
		{
			this.filePath = filePath;

			this.SaveDataToBuffer();
			this.SaveItemProbabilities();
			this.SetChecksum();
			this.SaveFile();
			this.ResetModifiedFlags();
		}

		private void SaveDataToBuffer()
		{
			int zoneStart = RomSize.Size512;
			int zoneEnd = Math.Min(this.romBuffer.Length, RomSize.Size1024);
			Range epicZone = new Range(zoneStart, zoneEnd);

			int epicZoneIterator = epicZone.Start;
			List<byte[]> savedData = new List<byte[]>();

			this.SaveAIs(ref epicZoneIterator, savedData);
			this.SaveTracks(epicZone, ref epicZoneIterator, savedData);

			// Compute total size of all the saved data to make sure it fits
			int savedDataSize = 0;
			foreach (byte[] dataBlock in savedData)
			{
				savedDataSize += dataBlock.Length;
			}

			// Check if all the saved data fits in the zone
			if (savedDataSize > epicZone.Length)
			{
				if (savedDataSize <= RomSize.Size512)
				{
					if (epicZone.Length == 0 && // If the ROM is 512 KiB (ie: the original SMK ROM size)
						savedDataSize > RomSize.Size256) // And if the data that needs to be saved is over 256 Kib
					{
						this.ExpandRomBuffer(RomSize.Size512);
					}
					else
					{
						// The ROM size is 512 or 768 KiB
						// and can be expanded by 256 KiB to make all the data fit
						this.ExpandRomBuffer(RomSize.Size256);
					}

					epicZone.End = this.romBuffer.Length;
				}
				else
				{
					// The data doesn't fit and we can't expand the ROM for more free space
					throw new InvalidOperationException("It's not possible to fit more data in this ROM.");
				}
			}

			// Save data to buffer
			epicZoneIterator = epicZone.Start;
			foreach (byte[] dataBlock in savedData)
			{
				Array.Copy(dataBlock, 0, this.romBuffer, epicZoneIterator, dataBlock.Length);
				epicZoneIterator += dataBlock.Length;
			}

			// Wipe out the rest of the zone
			for (int i = epicZoneIterator; i < epicZone.End; i++)
			{
				this.romBuffer[i] = 0xFF;
			}
		}

		private void SetChecksum()
		{
			int[] romSizes = new int[] { RomSize.Size256, RomSize.Size512, RomSize.Size1024, RomSize.Size2048, RomSize.Size4096, RomSize.Size8192 };

			bool isExactSize = false;
			int sizeIndex = 0;
			for (; sizeIndex < romSizes.Length; sizeIndex++)
			{
				if (this.romBuffer.Length == romSizes[sizeIndex])
				{
					isExactSize = true;
					break;
				}
				else if (this.romBuffer.Length < romSizes[sizeIndex])
				{
					break;
				}
			}
			// Set the ROM size
			//  8 =  2 Mb
			//  9 =  4 Mb
			// 10 =  8 Mb
			// 11 = 16 Mb
			// 12 = 32 Mb
			// 13 = 64 Mb
			this.romBuffer[0xFFD7] = (byte)(sizeIndex + 8);

			// Reset the checksum in case it is corrupted
			this.romBuffer[0xFFDC] = 0xFF;
			this.romBuffer[0xFFDD] = 0xFF;
			this.romBuffer[0xFFDE] = 0x00;
			this.romBuffer[0xFFDF] = 0x00;

			int end = isExactSize ? romSizes[sizeIndex] : romSizes[sizeIndex - 1];

			int total = 0;
			for (int index = 0; index < end; index++)
			{
				total += this.romBuffer[index];
			}

			if (!isExactSize)
			{
				int sizeLeft = this.romBuffer.Length - end;
				int multiplier = (romSizes[sizeIndex] - romSizes[sizeIndex - 1]) / sizeLeft;
				int lastPartTotal = 0;
				for (int index = end; index < this.romBuffer.Length; index++)
				{
					lastPartTotal += this.romBuffer[index];
				}
				total += lastPartTotal * multiplier;
			}

			this.romBuffer[0xFFDE] = (byte)(total & 0xFF);
			this.romBuffer[0xFFDF] = (byte)((total & 0xFF00) >> 8);
			this.romBuffer[0xFFDC] = (byte)(0xFF - this.romBuffer[0xFFDE]);
			this.romBuffer[0xFFDD] = (byte)(0xFF - this.romBuffer[0xFFDF]);
		}

		/// <summary>
		/// Expands the ROM buffer by the given value.
		/// </summary>
		/// <param name="expandValue">Number of bytes added to the buffer.</param>
		private void ExpandRomBuffer(int expandValue)
		{
			this.ResizeRomBuffer(this.romBuffer.Length + expandValue);
		}

		/// <summary>
		/// Resize the ROM buffer to the given size.
		/// </summary>
		/// <param name="newSize">New ROM buffer length.</param>
		private void ResizeRomBuffer(int newSize)
		{
			if (newSize > RomSize.Size8192)
			{
				throw new ArgumentOutOfRangeException("newSize", "The ROM can't be expanded because the maximum size has been reached.");
			}

			byte[] resizedRomBuffer = new byte[newSize];
			Array.Copy(this.romBuffer, resizedRomBuffer, this.romBuffer.Length);

			this.romBuffer = resizedRomBuffer;
		}

		private void SaveAIs(ref int epicZoneIterator, List<byte[]> savedData)
		{
			int aiFirstAddressByteOffset = this.offsets[Address.TrackAIDataFirstAddressByte];

			if (this.romBuffer[aiFirstAddressByteOffset] != 0xC8)
			{
				// This is either the original SMK ROM (if == 0xC6)
				// or possibly an altered ROM (if != 0xC6)
				this.romBuffer[aiFirstAddressByteOffset] = 0xC8;
			}

			byte[] trackOrder = this.LoadTrackOrder();

			for (int i = 0; i < this.trackGroups.Length; i++)
			{
				Track[] tracks = this.trackGroups[i].GetTracks();
				int trackGroupSize = tracks.Length;

				for (int j = 0; j < trackGroupSize; j++)
				{
					int trackIndex = trackOrder[i * 5 + j];
					this.SaveAI(tracks[j], trackIndex, ref epicZoneIterator, savedData);
				}
			}
		}

		private void SaveAI(Track track, int trackIndex, ref int epicZoneIterator, List<byte[]> savedData)
		{
			byte[] trackAIData = track.AI.GetBytes();
			savedData.Add(trackAIData);

			// Update AI offsets
			Offset aiZoneOffset = new Offset(epicZoneIterator);
			Offset aiTargetOffset = new Offset(epicZoneIterator + trackAIData.Length - track.AI.ElementCount * 3);

			int trackAIZoneIndex = this.offsets[Address.TrackAIZones] + trackIndex * 2;
			int trackAITargetIndex = this.offsets[Address.TrackAITargets] + trackIndex * 2;

			byte[] aiZoneOffsetValue = aiZoneOffset.GetBytes();
			byte[] aiTargetOffsetValue = aiTargetOffset.GetBytes();

			this.romBuffer[trackAIZoneIndex] = aiZoneOffsetValue[0];
			this.romBuffer[trackAIZoneIndex + 1] = aiZoneOffsetValue[1];
			this.romBuffer[trackAITargetIndex] = aiTargetOffsetValue[0];
			this.romBuffer[trackAITargetIndex + 1] = aiTargetOffsetValue[1];

			epicZoneIterator += trackAIData.Length;
		}

		private void SaveTracks(Range epicZone, ref int epicZoneIterator, List<byte[]> savedData)
		{
			byte[] trackOrder = this.LoadTrackOrder();
			Offset[] mapAddresses = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Address.TrackMaps], Game.TrackCount);

			for (int i = 0; i < this.trackGroups.Length; i++)
			{
				Track[] tracks = this.trackGroups[i].GetTracks();
				int trackGroupSize = tracks.Length;

				for (int j = 0; j < trackGroupSize; j++)
				{
					int iterator = i * 5 + j;
					int trackIndex = trackOrder[iterator];

					if (tracks[j].Modified)
					{
						this.SaveTrack(tracks[j], iterator, trackIndex, ref epicZoneIterator, savedData);
					}
					else
					{
						Offset trackOffset = mapAddresses[trackIndex];
						bool isInEpicZone = epicZone.Includes(trackOffset);

						if (isInEpicZone)
						{
							this.MoveTrackMap(trackIndex, trackOffset, ref epicZoneIterator, savedData);
						}
					}
				}
			}
		}

		private void SaveTrack(Track track, int iterator, int trackIndex, ref int epicZoneIterator, List<byte[]> savedData)
		{
			bool quirksMode = this.Region != Regions.US;
			byte[] compressedTrack = Codec.Compress(Codec.Compress(track.Map.GetBytes(), quirksMode), quirksMode);

			// Update track theme id
			byte themeId = this.themes.GetThemeId(track.Theme);
			int themeIdOffset = this.offsets[Address.TrackThemes] + trackIndex;
			this.romBuffer[themeIdOffset] = themeId;

			// Update overlay tiles
			byte[] overlayTileData = track.OverlayTiles.GetBytes(this.overlayTileSizes, this.overlayTilePatterns);
			this.SaveOverlayTileData(trackIndex, overlayTileData);

			if (track is GPTrack)
			{
				GPTrack gpTrack = track as GPTrack;
				byte[] data;

				// Update driver starting position
				this.SaveStartPositionData(trackIndex, gpTrack.StartPosition.GetBytes());

				// Update lap line position and length
				data = gpTrack.LapLine.GetBytes();
				Array.Copy(data, 0, this.romBuffer, this.offsets[Address.TrackLapLines] + trackIndex * data.Length, data.Length);

				// Update lap line position on Time Trial track preview
				int timeTrialLapLineOffset = offsets[Address.TimeTrialPreviewTrackLapLines] + iterator * 2;
				Point timeTrialLineLocation = Game.GetTimeTrialLapLineLocation(gpTrack);
				this.romBuffer[timeTrialLapLineOffset] = (byte)timeTrialLineLocation.X;
				this.romBuffer[timeTrialLapLineOffset + 1] = (byte)timeTrialLineLocation.Y;

				if (gpTrack.ObjectZones != null)
				{
					// Update object zones
					if (!gpTrack.ObjectZones.ReadOnly)
					{
						data = gpTrack.ObjectZones.GetBytes();
						Array.Copy(data, 0, this.romBuffer, this.GetObjectZonesOffset(trackIndex), data.Length);
					}

					// Update object coordinates
					data = gpTrack.Objects.GetBytes();
					Array.Copy(data, 0, this.romBuffer, this.offsets[Address.TrackObjects] + trackIndex * 64, data.Length);
				}
			}

			this.SaveTrackSub(trackIndex, ref epicZoneIterator, savedData, compressedTrack);
		}

		private static Point GetTimeTrialLapLineLocation(GPTrack track)
		{
			// Track coordinates:
			int xTopLeft = 40; // Top-left X value
			int xBottomLeft = 6; // Bottom-left X value
			int xBottomRight = 235; // Bottom-right X value
			int yTop = 16; // Top value
			int yBottom = 104; // Bottom value

			float yRelative = (1023 - track.LapLine.Y) * (xBottomRight - xBottomLeft) / 1023;
			int y = (int)(yBottom - (yRelative * Math.Sin(0.389)) - 7);

			float xPercent = (float)(track.StartPosition.X + track.StartPosition.SecondRowOffset / 2) / 1023;
			float yPercent = (float)(y - yTop) / (yBottom - yTop);
			int xStart = (int)(xTopLeft - (xTopLeft - xBottomLeft) * yPercent);
			int mapWidth = xBottomRight - (xStart - xBottomLeft) * 2;
			int x = (int)(xStart + mapWidth * xPercent);
			if (x < (xBottomRight - xBottomLeft) / 2)
			{
				// If the lap line is on the left side, shift its position a bit
				x -= 5;
			}

			return new Point(x, y);
		}

		private void MoveTrackMap(int trackIndex, Offset trackOffset, ref int epicZoneIterator, List<byte[]> savedData)
		{
			int compressedTrackLength = Codec.GetLength(this.romBuffer, trackOffset);
			byte[] compressedTrack = new byte[compressedTrackLength];
			Array.Copy(this.romBuffer, trackOffset, compressedTrack, 0, compressedTrackLength);

			this.SaveTrackSub(trackIndex, ref epicZoneIterator, savedData, compressedTrack);
		}

		private void SaveTrackSub(int trackIndex, ref int epicZoneIterator, List<byte[]> savedData, byte[] compressedTrack)
		{
			savedData.Add(compressedTrack);

			// Update track offset
			Offset offset = new Offset(epicZoneIterator);
			int trackAddressIndex = this.offsets[Address.TrackMaps] + trackIndex * 3;
			Array.Copy(offset.GetBytes(), 0, this.romBuffer, trackAddressIndex, 3);

			epicZoneIterator += compressedTrack.Length;
		}

		private void SaveItemProbabilities()
		{
			this.itemProbabilities.Save(this.romBuffer, offsets[Address.ItemProbabilities]);
		}

		private void SaveFile()
		{
			using (FileStream fs = new FileStream(this.filePath, FileMode.Create, FileAccess.Write))
			using (BinaryWriter bw = new BinaryWriter(fs))
			{
				if (this.romHeader != null)
				{
					bw.Write(this.romHeader);
				}
				bw.Write(this.romBuffer);
				bw.Close();
			}
		}

		private void ResetModifiedFlags()
		{
			this.modified = false;

			foreach (TrackGroup trackGroup in this.trackGroups)
			{
				foreach (Track track in trackGroup)
				{
					if (track.Modified)
					{
						track.Modified = false;
					}
				}
			}
		}

		public bool HasPendingChanges()
		{
			if (this.modified)
			{
				return true;
			}

			if (this.itemProbabilities.Modified)
			{
				return true;
			}

			if (this.overlayTileSizes.Modified)
			{
				return true;
			}

			if (this.overlayTilePatterns.Modified)
			{
				return true;
			}

			foreach (TrackGroup trackGroup in this.trackGroups)
			{
				foreach (Track track in trackGroup)
				{
					if (track.Modified)
					{
						return true;
					}
				}
			}

			return false;
		}
		#endregion ROM saving

		public void Dispose()
		{
			this.themes.Dispose();

			foreach (Bitmap icon in this.itemIcons)
			{
				icon.Dispose();
			}

			GC.SuppressFinalize(this);
		}
	}
}
