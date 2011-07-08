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
using System.Globalization;
using System.IO;

using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.ItemProba;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.UI.Gfx;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace EpicEdit.Rom
{
    public enum Region
    {
        Jap,
        US,
        Euro
    }

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
        #region Public members and methods

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
        /// Gets the track themes.
        /// </summary>
        public Themes Themes
        {
            get { return this.themes; }
        }

        /// <summary>
        /// Gets the overlay tile sizes.
        /// </summary>
        public OverlayTileSizes OverlayTileSizes
        {
            get { return this.overlayTileSizes; }
        }

        /// <summary>
        /// Gets the overlay tile patterns.
        /// </summary>
        public OverlayTilePatterns OverlayTilePatterns
        {
            get { return this.overlayTilePatterns; }
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

        public string[] GetModeNames()
        {
            string[] modeNames = new string[3];

            int offset = this.offsets[Offset.ModeStrings];
            int nameOffset = Utilities.BytesToOffset(this.romBuffer[offset], this.romBuffer[offset + 1], 5);
            int lengthOffset = offset + 6;

            for (int i = 0; i < modeNames.Length; i++)
            {
                int length = this.romBuffer[lengthOffset] * 2;
                byte[] hexText = new byte[length];
                Buffer.BlockCopy(this.romBuffer, nameOffset, hexText, 0, length);
                modeNames[i] = Utilities.DecryptRomTextOdd(hexText, this.region);
                nameOffset += length;
                lengthOffset += 2;
            }

            return modeNames;
        }

        public ItemProbabilities ItemProbabilities
        {
            get { return this.itemProbabilities; }
        }

        public Bitmap GetItemIcon(ItemType type)
        {
            return this.itemIcons[(int)type];
        }

        #endregion Public members and methods

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
        /// The region of the ROM (Jap, US or Euro).
        /// </summary>
        private Region region;

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
            this.romBuffer = File.ReadAllBytes(this.filePath);
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

                        string ext = Path.GetExtension(entry.Name);
                        if (".bin".Equals(ext, StringComparison.OrdinalIgnoreCase) ||
                            ".fig".Equals(ext, StringComparison.OrdinalIgnoreCase) ||
                            ".sfc".Equals(ext, StringComparison.OrdinalIgnoreCase) ||
                            ".smc".Equals(ext, StringComparison.OrdinalIgnoreCase) ||
                            ".swc".Equals(ext, StringComparison.OrdinalIgnoreCase))
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
                // NOTE: We don't throw a ZipException,
                // so that the presence of the SharpZipLib assembly
                // is not required to load uncompressed ROMs.
                throw new InvalidDataException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Checks if the ROM is valid. If it is, the ROM header value is initialized.
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
                Buffer.BlockCopy(this.romBuffer, 0, this.romHeader, 0, this.romHeader.Length);

                byte[] romBufferWithoutHeader = new byte[this.romBuffer.Length - this.romHeader.Length];
                Buffer.BlockCopy(this.romBuffer, this.romHeader.Length, romBufferWithoutHeader, 0, romBufferWithoutHeader.Length);
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
            this.SetRegion();
            this.offsets = new Offsets(this.romBuffer, this.region);

            this.trackGroups = new TrackGroup[Track.GroupCount];
            string[] names = this.GetCupAndThemeNames();

            this.themes = new Themes(this.romBuffer, this.offsets, names);
            this.overlayTileSizes = new OverlayTileSizes(this.romBuffer, this.offsets);
            this.overlayTilePatterns = new OverlayTilePatterns(this.romBuffer, this.offsets, this.overlayTileSizes);

            byte[] trackThemes = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackThemes], Track.Count);
            byte[] trackOrder = this.GetTrackOrder();
            byte[][] trackNameIndex = this.GetTrackNameIndexes();

            int[] mapAddresses = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Offset.TrackMaps], Track.Count);

            byte aiOffsetBase = this.romBuffer[this.offsets[Offset.TrackAIDataFirstAddressByte]];
            byte[] aiZoneOffsets = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackAIZones], Track.Count * 2); // 2 offset bytes per track
            byte[] aiTargetOffsets = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackAITargets], Track.Count * 2); // 2 offset bytes per track

            for (int i = 0; i < this.trackGroups.Length; i++)
            {
                int trackCountInGroup;
                string trackGroupName;
                if (i != this.trackGroups.Length - 1) // GP track group
                {
                    trackCountInGroup = GPTrack.CountPerGroup;
                    trackGroupName = names[i];
                }
                else // Battle track group
                {
                    trackCountInGroup = BattleTrack.Count;
                    trackGroupName = names[trackNameIndex[GPTrack.Count][1]];
                }

                Track[] tracks = new Track[trackCountInGroup];

                for (int j = 0; j < trackCountInGroup; j++)
                {
                    int iterator = i * GPTrack.CountPerGroup + j;
                    int trackIndex = trackOrder[iterator];

                    string trackName = names[trackNameIndex[iterator][1]];
                    if (trackNameIndex[iterator].Length > 2) // We check if there is a track number (eg: Rainbow Road doesn't have one)
                    {
                        trackName += trackNameIndex[iterator][2];
                    }

                    int themeId = trackThemes[trackIndex] >> 1;
                    Theme trackTheme = this.themes[themeId];

                    byte[] trackMap = Codec.Decompress(Codec.Decompress(this.romBuffer, mapAddresses[trackIndex]), 0, TrackMap.SquareSize);

                    byte[] overlayTileData = this.GetOverlayTileData(trackIndex);

                    byte[] aiZoneData, aiTargetData;
                    this.LoadAIData(trackIndex, aiOffsetBase, aiZoneOffsets, aiTargetOffsets, out aiZoneData, out aiTargetData);

                    if (trackIndex < GPTrack.Count) // GP track
                    {
                        byte[] startPositionData = this.GetGPStartPositionData(trackIndex);
                        byte[] lapLineData = this.GetLapLineData(trackIndex);
                        byte[] objectData = this.GetObjectData(trackIndex);
                        byte[] objectZoneData = this.GetObjectZoneData(trackIndex);

                        GPTrack gpTrack = new GPTrack(trackName, trackTheme,
                                                      trackMap, overlayTileData,
                                                      aiZoneData, aiTargetData,
                                                      startPositionData, lapLineData,
                                                      objectData, objectZoneData,
                                                      this.overlayTileSizes,
                                                      this.overlayTilePatterns);

                        this.SetObjectProperties(gpTrack, trackIndex, themeId);

                        tracks[j] = gpTrack;
                    }
                    else // Battle track
                    {
                        byte[] startPositionData = this.GetBattleStartPositionData(trackIndex);

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

        private void SetRegion()
        {
            int regionAddress = 0xFFD9;
            int region = this.romBuffer[regionAddress];

            if (!Enum.IsDefined(typeof(Region), region))
            {
                if (this.romHeader != null)
                {
                    regionAddress += this.romHeader.Length;
                }

                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                                                             "\"{0}\" has an invalid region. Value at {1:X} must be 0, 1 or 2, was: {2:X}.",
                                                             this.FileName, regionAddress, region));
            }

            this.region = (Region)region;
        }

        /// <summary>
        /// Gets the names of the cups and track themes.
        /// </summary>
        /// <returns>The names of the cups and track themes.</returns>
        private string[] GetCupAndThemeNames()
        {
            int nameCount = this.trackGroups.Length + Theme.Count;
            string[] names = new string[nameCount];
            byte[][] nameIndex = Utilities.ReadBlockGroup(this.romBuffer, this.offsets[Offset.NameStrings], 2, names.Length);
            int offset;

            for (int i = 0; i < names.Length; i++)
            {
                offset = Utilities.BytesToOffset(nameIndex[i][0], nameIndex[i][1], 1); // Recreates offsets from the index table loaded above
                names[i] = Utilities.DecryptRomText(Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF), this.region);
            }

            return names;
        }

        /// <summary>
        /// Gets the order of the tracks.
        /// </summary>
        /// <returns></returns>
        private byte[] GetTrackOrder()
        {
            byte[] gpTrackOrder = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.GPTrackOrder], GPTrack.Count);
            byte[] battleTrackOrder = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.BattleTrackOrder], BattleTrack.Count);

            byte[] trackOrder = new byte[Track.Count];

            Buffer.BlockCopy(gpTrackOrder, 0, trackOrder, 0, GPTrack.Count);
            Buffer.BlockCopy(battleTrackOrder, 0, trackOrder, GPTrack.Count, BattleTrack.Count);
            return trackOrder;
        }

        private byte[][] GetTrackNameIndexes()
        {
            int gpTrackNamesOffset = this.offsets[Offset.GPTrackNames];

            byte[][] gpTrackPointers = new byte[GPTrack.Count][];

            for (int i = 0; i < GPTrack.GroupCount; i++)
            {
                Utilities.ReadBlockGroup(this.romBuffer, gpTrackNamesOffset, 4, GPTrack.CountPerGroup).CopyTo(gpTrackPointers, GPTrack.CountPerGroup * i);
                gpTrackNamesOffset += GPTrack.CountPerGroup * GPTrack.GroupCount + 2; // 2 separating bytes
            }

            byte[][] battleTrackPointers = Utilities.ReadBlockGroup(this.romBuffer, this.offsets[Offset.BattleTrackNames], 4, BattleTrack.Count);
            byte[][] trackNameIndex = new byte[Track.Count][];

            for (int i = 0; i < gpTrackPointers.Length; i++)
            {
                int offset = Utilities.BytesToOffset(gpTrackPointers[i][0], gpTrackPointers[i][1], 1);
                trackNameIndex[i] = Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF);
            }

            for (int i = 0; i < battleTrackPointers.Length; i++)
            {
                int offset = Utilities.BytesToOffset(battleTrackPointers[i][0], battleTrackPointers[i][1], 1);
                trackNameIndex[gpTrackPointers.Length + i] = Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF);
            }

            for (int i = 0; i < trackNameIndex.Length; i++)
            {
                trackNameIndex[i][1] = (byte)(trackNameIndex[i][1] & 0xF);
            }

            return trackNameIndex;
        }

        #endregion Load data

        #region Get / Set, Load / Save specific data

        #region Track Overlay Tiles

        private byte[] GetOverlayTileData(int trackIndex)
        {
            int offset = this.GetOverlayTileDataOffset(trackIndex);
            byte[] data = new byte[OverlayTiles.Size];
            Buffer.BlockCopy(this.romBuffer, offset, data, 0, data.Length);
            return data;
        }

        private void SaveOverlayTileData(int trackIndex, byte[] data)
        {
            int offset = this.GetOverlayTileDataOffset(trackIndex);
            Buffer.BlockCopy(data, 0, this.romBuffer, offset, OverlayTiles.Size);
        }

        private int GetOverlayTileDataOffset(int trackIndex)
        {
            return this.offsets[Offset.TrackOverlayItems] + trackIndex * OverlayTiles.Size;
        }

        #endregion Track Overlay Tiles

        #region GP Start Positions

        private byte[] GetGPStartPositionData(int trackIndex)
        {
            int offset = this.GetGPStartPositionDataOffset(trackIndex);
            byte[] data = new byte[6];
            Buffer.BlockCopy(this.romBuffer, offset, data, 0, data.Length);
            return data;
        }

        private void SaveGPStartPositionData(GPTrack track, int trackIndex)
        {
            byte[] data = track.StartPosition.GetBytes();
            int offset = this.GetGPStartPositionDataOffset(trackIndex);
            Buffer.BlockCopy(data, 0, this.romBuffer, offset, 6);
        }

        private int GetGPStartPositionDataOffset(int trackIndex)
        {
            int[] reorder = { 14, 10, 7, 8, 15, 19, 16, 4, 17, 13, 6, 12, 11, 5, 18, 9, 2, 3, 1, 0 };
            // TODO: Retrieve order dynamically from the ROM
            return this.offsets[Offset.GPTrackStartPositions] + reorder[trackIndex] * 8;
        }

        #endregion GP Start Positions

        #region Lap Line

        private byte[] GetLapLineData(int trackIndex)
        {
            byte[] data = new byte[6];
            int lapLineDataOffset = this.offsets[Offset.TrackLapLines] + trackIndex * data.Length;
            Buffer.BlockCopy(this.romBuffer, lapLineDataOffset, data, 0, data.Length);
            return data;
        }

        #endregion Lap Line

        #region Battle Start Positions

        private byte[] GetBattleStartPositionData(int trackIndex)
        {
            int startPositionOffset = this.GetBattleStartPositionDataOffset(trackIndex);

            byte[] data = new byte[8];

            if (this.BattleStartPositionsRelocated)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = this.romBuffer[startPositionOffset + i];
                }
            }
            else
            {
                // In the original game, it's 2P data first, then 1P.
                int index = 0;
                for (int i = 4; i < data.Length; i++)
                {
                    data[index++] = this.romBuffer[startPositionOffset + i];
                }

                for (int i = 0; i < 4; i++)
                {
                    data[index++] = this.romBuffer[startPositionOffset + i];
                }
            }

            return data;
        }

        private int GetBattleStartPositionDataOffset(int trackIndex)
        {
            int startPositionOffset;
            int bTrackIndex = trackIndex - GPTrack.Count;

            if (this.BattleStartPositionsRelocated)
            {
                startPositionOffset = 0x80000 + bTrackIndex * 8;
            }
            else
            {
                // The battle starting positions haven't been relocated yet.
                // Ie: This ROM has not been resaved with Epic Edit yet.
                int startPositionOffsetIndex = this.offsets[Offset.BattleTrackStartPositions] + bTrackIndex * 8;
                startPositionOffset = Utilities.BytesToOffset(this.romBuffer[startPositionOffsetIndex], this.romBuffer[startPositionOffsetIndex + 1], 1);
                startPositionOffset += 2; // Skip 2 leading bytes
            }

            return startPositionOffset;
        }

        private bool BattleStartPositionsRelocated
        {
            get
            {
                int offset = this.offsets[Offset.BattleTrackStartPositionsIndex];

                if (this.romBuffer[offset] == 0x5C &&
                    this.romBuffer[offset + 1] == 0x20 &&
                    this.romBuffer[offset + 2] == 0x00 &&
                    this.romBuffer[offset + 3] == 0xC8)
                {
                    return true;
                }

                if (this.romBuffer[offset] == 0xAD &&
                    this.romBuffer[offset + 1] == 0x24 &&
                    this.romBuffer[offset + 2] == 0x01 &&
                    this.romBuffer[offset + 3] == 0x0A)
                {
                    return false;
                }

                throw new InvalidDataException("Error when loading battle track starting positions.");
            }
        }

        #endregion Battle Start Positions

        #region Objects

        private byte[] GetObjectData(int trackIndex)
        {
            int objectOffset = this.offsets[Offset.TrackObjects] + (trackIndex * 64);
            byte[] data = Utilities.ReadBlock(this.romBuffer, objectOffset, 44);
            // 16 objects * 2 coordinate bytes = 32 bytes
            // + 6 Match Race objects (Chain Chomps) * 2 coordinate bytes = 12 bytes
            // Total = 44 bytes
            return data;
        }

        private void SetObjectProperties(GPTrack track, int trackIndex, int themeId)
        {
            if (this.ObjectZonesRelocated)
            {
                int offset = 0x80062 + trackIndex; // TODO: Define in Offsets.cs
                track.ObjectTileset = (ObjectType)this.romBuffer[offset];
                track.ObjectInteraction = (ObjectType)this.romBuffer[offset + Track.Count];
                track.ObjectRoutine = (ObjectType)this.romBuffer[offset + Track.Count * 2];
            }
            else
            {
                ObjectType objectType;

                switch (themeId)
                {
                    case 0: // Ghost Valley
                        objectType = ObjectType.Pillar;
                        break;

                    case 1: // Mario Circuit
                    case 4: // Vanilla Lake
                        objectType = ObjectType.Pipe;
                        break;

                    case 2: // Donut Plains
                        objectType = trackIndex == 19 ? ObjectType.Pipe : ObjectType.Mole;
                        break;

                    case 3: // Choco Island
                        objectType = ObjectType.Plant;
                        break;

                    case 5: // Koopa Beach
                        objectType = ObjectType.Fish;
                        break;

                    case 6: // Bowser Castle
                        objectType = ObjectType.Thwomp;
                        break;

                    case 7: // Rainbow Road
                        objectType = ObjectType.RThwomp;
                        break;

                    default: throw new InvalidDataException();
                }

                track.ObjectTileset = objectType;
                track.ObjectInteraction = objectType;
                track.ObjectRoutine = objectType;
            }
        }

        #endregion Objects

        #region Object Zones

        private byte[] GetObjectZoneData(int trackIndex)
        {
            int objectZoneOffset = this.GetObjectZoneOffset(trackIndex);

            byte[] data;

            if (objectZoneOffset == -1)
            {
                data = new byte[10]; // Ghost Valley track, has pillars
            }
            else
            {
                data = Utilities.ReadBlock(this.romBuffer, objectZoneOffset, 10);
            }

            return data;
        }

        private int GetObjectZoneOffset(int trackIndex)
        {
            if (this.ObjectZonesRelocated)
            {
                return 0x80216 + trackIndex * 10; // TODO: Define in offsets
            }

            int[] reorder =
            {
                2, -1 /* Ghost Valley x */, 12, 8, 15,
                10, 17, 0, -1 /* Ghost Valley x */, 9,
                5, 13, 14, 17, 3,
                1, -1 /* Ghost Valley x */, 7, 4, 11
            };
            // TODO: Retrieve order dynamically from the ROM

            // NOTE: The 2 bytes at 4DB85 (93DB) are an address (4DB93)
            // to 2 other bytes (CFDA), which are an address (4DACF)
            // to the object zones of Mario Circuit 1. The other tracks follow.
            // But I don't know where the track order is defined.

            if (reorder[trackIndex] == -1)
            {
                return reorder[trackIndex];
            }

            int objectZonesOffset = this.offsets[Offset.TrackObjectZones];
            int index = objectZonesOffset + reorder[trackIndex] * 2;
            return Utilities.BytesToOffset(this.romBuffer[index], this.romBuffer[index + 1], 4);
        }

        private bool ObjectZonesRelocated
        {
            get
            {
                return this.romBuffer[0x4DCA9] == 0xB7; // TODO: Define in Offsets.cs
            }
        }

        #endregion Object Zones

        #region AI

        private void LoadAIData(int trackIndex, byte aiOffsetBase, byte[] aiZoneOffsets, byte[] aiTargetOffsets, out byte[] aiZoneData, out byte[] aiTargetData)
        {
            int aiOffset = trackIndex * 2;

            int aiZoneDataOffset = Utilities.BytesToOffset(aiZoneOffsets[aiOffset], aiZoneOffsets[aiOffset + 1], aiOffsetBase);
            aiZoneData = Utilities.ReadBlockUntil(this.romBuffer, aiZoneDataOffset, 0xFF);

            int aiTargetDataOffset = Utilities.BytesToOffset(aiTargetOffsets[aiOffset], aiTargetOffsets[aiOffset + 1], aiOffsetBase);
            int aiTargetDataLength = TrackAI.ComputeTargetDataLength(aiZoneData);
            aiTargetData = Utilities.ReadBlock(romBuffer, aiTargetDataOffset, aiTargetDataLength);
        }

        #endregion AI

        #region Item Probabilities

        private void LoadItemProbabilities()
        {
            this.itemProbabilities = new ItemProbabilities(this.romBuffer, this.offsets[Offset.ItemProbabilities]);
        }

        #endregion Item Probabilities

        #region Item Icons

        private void LoadItemIcons()
        {
            byte[] itemGfx = Codec.Decompress(this.romBuffer, this.offsets[Offset.ItemIcons]);
            int itemCount = Enum.GetValues(typeof(ItemType)).Length;
            this.itemIcons = new Bitmap[itemCount];

            for (int i = 0; i < this.itemIcons.Length; i++)
            {
                this.LoadItemIcon(itemGfx, i);
            }
        }

        private void LoadItemIcon(byte[] itemGfx, int index)
        {
            int iconPaletteOffset = this.offsets[Offset.ItemIconTilesPalettes] + index * 2;

            int tileIndex = this.romBuffer[iconPaletteOffset] & 0x7F;
            byte globalPalIndex = this.romBuffer[iconPaletteOffset + 1];
            int palIndex = globalPalIndex / 16;
            Palette palette = this.themes[0].Palettes[palIndex];
            int subPalIndex = globalPalIndex % 16;

            this.itemIcons[index] = GraphicsConverter.GetBitmapFrom2bppPlanar(itemGfx, tileIndex * 16, palette, subPalIndex, 16, 16);
        }

        #endregion Item Icons

        #endregion Get / Set, Load / Save specific data

        #region Track reodering
        public void ReorderTracks(int sourceTrackGroupId, int sourceTrackId, int destinationTrackGroupId, int destinationTrackId)
        {
            if (sourceTrackGroupId == destinationTrackGroupId &&
                sourceTrackId == destinationTrackId)
            {
                return;
            }

            if (sourceTrackGroupId < GPTrack.GroupCount) // GP track reordering
            {
                #region Global track array creation
                // To make the treatment easier, we simply create an array with all the GP tracks
                Track[] tracks = new Track[GPTrack.Count];
                for (int i = 0; i < this.trackGroups.Length - 1; i++)
                {
                    Track[] groupTracks = this.trackGroups[i].GetTracks();

                    for (int j = 0; j < groupTracks.Length; j++)
                    {
                        tracks[i * groupTracks.Length + j] = groupTracks[j];
                    }
                }

                sourceTrackId = sourceTrackGroupId * GPTrack.CountPerGroup + sourceTrackId;
                destinationTrackId = destinationTrackGroupId * GPTrack.CountPerGroup + destinationTrackId;
                #endregion Global track array creation

                int trackOrderOffset = this.offsets[Offset.GPTrackOrder];
                int trackNameOffset = this.offsets[Offset.GPTrackNames];

                this.ReorderTracksSub(tracks, sourceTrackId, destinationTrackId, trackOrderOffset, trackNameOffset);

                #region GP track specific data update
                // Update Time Trial lap line positions
                int startingLineOffset = this.offsets[Offset.TrackPreviewLapLines];
                byte[] sourceTrackStartingLine =
                { this.romBuffer[startingLineOffset + sourceTrackId * 2],
                    this.romBuffer[startingLineOffset + sourceTrackId * 2 + 1] };

                if (sourceTrackId < destinationTrackId)
                {
                    Buffer.BlockCopy(
                        this.romBuffer,
                        startingLineOffset + (sourceTrackId + 1) * 2,
                        this.romBuffer,
                        startingLineOffset + sourceTrackId * 2,
                        (destinationTrackId - sourceTrackId) * 2
                    );
                }
                else
                {
                    Buffer.BlockCopy(
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

                int trackOrderOffset = this.offsets[Offset.BattleTrackOrder];
                int trackNameOffset = this.offsets[Offset.BattleTrackNames];

                this.ReorderTracksSub(tracks, sourceTrackId, destinationTrackId, trackOrderOffset, trackNameOffset);

                #region Battle track specific data update
                // Update the track shown by default when entering the battle track selection
                this.romBuffer[this.offsets[Offset.FirstBattleTrack]] = this.romBuffer[trackOrderOffset];

                // Update the selection cursor positions of the battle track selection
                for (byte i = 0; i < BattleTrack.Count; i++)
                {
                    byte value = (byte)(this.romBuffer[trackOrderOffset + i] - 0x14);
                    this.romBuffer[trackOrderOffset + BattleTrack.Count + value] = i;
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

        #region Save data
        public void SaveRom(string filePath)
        {
            this.filePath = filePath;

            this.SaveDataToBuffer();
            this.SetChecksum();
            this.SaveFile();
            this.ResetModifiedFlags();
        }

        private void SaveDataToBuffer()
        {
            SaveBuffer saveBuffer = new SaveBuffer(this.romBuffer);
            this.SaveBattleStartPositions(saveBuffer);

            if (this.region == Region.US) // TODO: Add support for other regions
            {
                this.SaveObjectData(saveBuffer);
            }

            this.SaveAIs(saveBuffer);
            this.SaveTracks(saveBuffer);
            this.romBuffer = saveBuffer.GetRomBuffer();

            this.SaveItemProbabilities();
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

        private void SaveBattleStartPositions(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();

            Track[] tracks = this.trackGroups[GPTrack.GroupCount].GetTracks();
            int trackGroupSize = tracks.Length;

            for (int i = 0; i < trackGroupSize; i++)
            {
                int iterator = GPTrack.Count + i;
                int trackIndex = trackOrder[iterator];
                int bTrackIndex = trackIndex - GPTrack.Count;

                this.SaveBattleStartPositions(tracks[bTrackIndex] as BattleTrack, saveBuffer);
            }

            this.RelocateBattleStartPositions(saveBuffer);
        }

        private void SaveBattleStartPositions(BattleTrack track, SaveBuffer saveBuffer)
        {
            byte[] startPositionP1Data = track.StartPositionP1.GetBytes();
            byte[] startPositionP2Data = track.StartPositionP2.GetBytes();
            saveBuffer.Add(startPositionP1Data);
            saveBuffer.Add(startPositionP2Data);
        }

        private void RelocateBattleStartPositions(SaveBuffer saveBuffer)
        {
            // Relocate the battle track starting positions (to 0x80000).
            int offset = this.offsets[Offset.BattleTrackStartPositionsIndex];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x20;
            this.romBuffer[offset++] = 0x00;
            this.romBuffer[offset] = 0xC8;

            // New code for battle track starting positions
            byte[] hack =
            {
                0xAD, 0x24, 0x01, 0xC9, 0x14, 0x00, 0x90, 0x35,
                0xE9, 0x14, 0x00, 0x0A, 0x0A, 0x0A, 0xAA, 0xBF,
                0x00, 0x00, 0xC8, 0x8D, 0x18, 0x10, 0xBF, 0x02,
                0x00, 0xC8, 0x8D, 0x1C, 0x10, 0xBF, 0x04, 0x00,
                0xC8, 0x8D, 0x18, 0x11, 0xBF, 0x06, 0x00, 0xC8,
                0x8D, 0x1C, 0x11, 0x9C, 0x20, 0x10, 0x9C, 0x20,
                0x11, 0xAD, 0x24, 0x01, 0x0A, 0xAA, 0xBC, 0x79,
                0x8A, 0x5C, 0x26, 0x8F, 0x81, 0x0A, 0x5C, 0x18,
                0x8F, 0x81
            };

            saveBuffer.Add(hack);
        }

        /// <summary>
        /// Saves all the object data (locations, zones, properties).
        /// Also applies hacks that make the track object engine more flexible.
        /// </summary>
        private void SaveObjectData(SaveBuffer saveBuffer)
        {
            /*
                The hacks below include the following improvements:

                - Make all tracks have independent object zones (Koopa Beach 1 and 2 share the same one in the original game)
                - Make it possible to change the object type of each track (regular or GV pillar)
                - Remove Donut Plains pipe / mole hacks with something cleaner and reusable
                - Make it possible to mix the properties of each object (tileset, interaction, routine)
            */

            this.RelocateObjectData();
            this.SaveObjectProperties(saveBuffer);
            Game.AddObjectCodeChunk1(saveBuffer);
            this.SaveObjectLocationsAndZones(saveBuffer);
            Game.AddObjectCodeChunk2(saveBuffer);
            this.SavePillars(saveBuffer);
            Game.AddObjectCodeChunk3(saveBuffer);
        }

        private void RelocateObjectData()
        {
            /*
                Update addresses to point to the relocated data:

                table tileset: c80062
                table interact: c8007a
                table routine: c80092
                table Z: c800aa
                table loading: c800c2
                A ptr: c800da
                B ptr: c80123
                C ptr: c8014f
                D ptr: c801ab
                normal zone table: c80216
                GV checkpoint table: c80328
                GV position data: c80d28
                E ptr: c85d28
            */

            // TODO: Move offsets to Offsets.cs, and support Japanese and Euro ROMs

            int offset = 0x18EDF;
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x23;
            this.romBuffer[offset++] = 0x01;
            this.romBuffer[offset] = 0xC8;

            offset = 0x19141;
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x4F;
            this.romBuffer[offset++] = 0x01;
            this.romBuffer[offset] = 0xC8;

            offset = 0x19E2B;
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x28;
            this.romBuffer[offset++] = 0x5D;
            this.romBuffer[offset] = 0xC8;

            offset = 0x1E992;
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0xDA;
            this.romBuffer[offset++] = 0x00;
            this.romBuffer[offset] = 0xC8;

            offset = 0x4DABC;
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0xAB;
            this.romBuffer[offset++] = 0x01;
            this.romBuffer[offset] = 0xC8;

            this.romBuffer[0x4DCA9] = 0xB7;
            this.romBuffer[0x4DCBD] = 0xB7;
            this.romBuffer[0x4DCC2] = 0xB7;
        }

        private void SaveObjectProperties(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();

            byte[] tilesetData = new byte[Track.Count];
            byte[] interactData = new byte[Track.Count];
            byte[] routineData = new byte[Track.Count];
            byte[] zData = new byte[Track.Count];
            byte[] loadingData = new byte[Track.Count];

            for (int i = 0; i < this.trackGroups.Length - 1; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();
                int trackGroupSize = tracks.Length;

                for (int j = 0; j < trackGroupSize; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    GPTrack gpTrack = tracks[j] as GPTrack;

                    tilesetData[trackIndex] = (byte)gpTrack.ObjectTileset;
                    interactData[trackIndex] = (byte)gpTrack.ObjectInteraction;
                    routineData[trackIndex] = (byte)gpTrack.ObjectRoutine;
                    zData[trackIndex] = routineData[trackIndex];
                    loadingData[trackIndex] = (byte)gpTrack.ObjectLoading;
                }
            }

            // Mark battle tracks as not having objects
            byte noObject = (byte)ObjectLoading.None;
            loadingData[GPTrack.Count] = noObject;
            loadingData[GPTrack.Count + 1] = noObject;
            loadingData[GPTrack.Count + 2] = noObject;
            loadingData[GPTrack.Count + 3] = noObject;

            saveBuffer.Add(tilesetData);
            saveBuffer.Add(interactData);
            saveBuffer.Add(routineData);
            saveBuffer.Add(zData);
            saveBuffer.Add(loadingData);
        }

        private static void AddObjectCodeChunk1(SaveBuffer saveBuffer)
        {
            byte[] hack =
            {
                0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF, 0x7A, 0x00,
                0xC8, 0xAA, 0xBF, 0x15, 0x01, 0xC8, 0x8D, 0x30,
                0x0E, 0x9C, 0x31, 0x0E, 0xAE, 0x24, 0x01, 0xBF,
                0x62, 0x00, 0xC8, 0xAA, 0xBF, 0x1C, 0x01, 0xC8,
                0x48, 0x4A, 0x18, 0x63, 0x01, 0x83, 0x01, 0x68,
                0xC2, 0x30, 0x29, 0xFF, 0x00, 0xAA, 0xBF, 0xD3,
                0xEB, 0x81, 0xA8, 0xBF, 0xD5, 0xEB, 0x81, 0x5C,
                0x98, 0xE9, 0x81, 0x02, 0x00, 0x0C, 0x04, 0x06,
                0x0A, 0x0C, 0x02, 0x00, 0x0C, 0x04, 0x06, 0x0A,
                0x0E, 0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF, 0xAA,
                0x00, 0xC8, 0xAA, 0xBF, 0x48, 0x01, 0xC8, 0xAA,
                0xC2, 0x20, 0xBF, 0xCD, 0x8B, 0x81, 0x8D, 0xFC,
                0x0F, 0xBF, 0xDD, 0x8B, 0x81, 0x8D, 0xFE, 0x0F,
                0xC2, 0x30, 0x5C, 0xEB, 0x8E, 0x81, 0x02, 0x00,
                0x0C, 0x04, 0x06, 0x0A, 0x0C, 0xE2, 0x30, 0xAE,
                0x24, 0x01, 0xBF, 0x92, 0x00, 0xC8, 0x0A, 0x48,
                0x0A, 0x18, 0x63, 0x01, 0x83, 0x01, 0xFA, 0xC2,
                0x20, 0xBF, 0x81, 0x01, 0xC8, 0x85, 0x06, 0xBF,
                0x83, 0x01, 0xC8, 0x8D, 0x98, 0x0F, 0x85, 0x08,
                0xBF, 0x85, 0x01, 0xC8, 0x8D, 0x9A, 0x0F, 0x85,
                0x0A, 0xC2, 0x10, 0x5C, 0x56, 0x91, 0x81, 0xD0,
                0x91, 0xE7, 0xE4, 0xF7, 0xE4, 0xD0, 0x91, 0xFE,
                0xE6, 0x08, 0xE7, 0xD0, 0x91, 0x8B, 0xE1, 0x9D,
                0xE1, 0xE4, 0x91, 0x6B, 0xE3, 0x7B, 0xE3, 0xD0,
                0x91, 0x6E, 0xE0, 0x7A, 0xE0, 0xC0, 0x91, 0x7E,
                0xDF, 0x8E, 0xDF, 0xD0, 0x91, 0x44, 0xE1, 0x56,
                0xE1, 0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF, 0xC2,
                0x00, 0xC8, 0x0A, 0xAA, 0xFC, 0xBF, 0x01, 0xC2,
                0x30, 0x5C, 0xC2, 0xDA, 0x84, 0xC8, 0x01, 0xDE,
                0x02, 0xEF, 0x02, 0xC7, 0x01, 0x60, 0xC2, 0x30,
                0xAD, 0x24, 0x01, 0x0A, 0x0A, 0x18, 0x6D, 0x24,
                0x01, 0x0A, 0x18, 0x69, 0x16, 0x02, 0x85, 0x08,
                0xA9, 0xC8, 0x00, 0x85, 0x0A, 0xAC, 0xE4, 0x1E,
                0xB6, 0xC8, 0x10, 0x11, 0xA5, 0x08, 0x18, 0x69,
                0x05, 0x00, 0x85, 0x08, 0x98, 0x49, 0x02, 0x00,
                0xA8, 0xB6, 0xC8, 0x30, 0x1E, 0xA0, 0x00, 0x00,
                0xE2, 0x20, 0xB5, 0xC0, 0xC9, 0xFF, 0xF0, 0x06,
                0x88, 0xC8, 0xD7, 0x08, 0xB0, 0xFB, 0xC2, 0x20,
                0x4B, 0x62, 0x06, 0x00, 0xF4, 0xAD, 0xBD, 0x5C,
                0x0B, 0xDC, 0x84, 0x60
            };

            saveBuffer.Add(hack);
        }

        private void SaveObjectLocationsAndZones(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();
            byte[] objectZonesData = new byte[GPTrack.Count * 10];

            for (int i = 0; i < this.trackGroups.Length - 1; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();
                int trackGroupSize = tracks.Length;

                for (int j = 0; j < trackGroupSize; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    GPTrack gpTrack = tracks[j] as GPTrack;

                    // Update object zones
                    byte[] data = gpTrack.ObjectZones.GetBytes();
                    Buffer.BlockCopy(data, 0, objectZonesData,
                                     trackIndex * data.Length,
                                     data.Length);

                    if (gpTrack.Modified) // Avoid saving data if not necessary
                    {
                        // Update object coordinates
                        data = gpTrack.Objects.GetBytes();
                        Buffer.BlockCopy(data, 0, this.romBuffer, this.offsets[Offset.TrackObjects] + trackIndex * 64, data.Length);
                    }
                }
            }

            saveBuffer.Add(objectZonesData);
        }
        
        private static void AddObjectCodeChunk2(SaveBuffer saveBuffer)
        {
            byte[] hack =
            {
                0x20, 0xC8, 0x01, 0x90, 0x0B, 0x4B, 0x62, 0x06,
                0x00, 0xF4, 0xAD, 0xBD, 0x5C, 0xBC, 0xDB, 0x84,
                0x60, 0xC2, 0x30, 0xAC, 0xE4, 0x1E, 0xB6, 0xC8,
                0x30, 0x2F, 0xB9, 0x7C, 0xDC, 0x85, 0x0C, 0xAD,
                0x24, 0x01, 0xEB, 0x4A, 0x85, 0x04, 0x0A, 0x0A,
                0x0A, 0x18, 0x69, 0x28, 0x0D, 0x85, 0x08, 0xA5,
                0x04, 0x18, 0x69, 0x28, 0x03, 0x85, 0x04, 0xA9,
                0xC8, 0x00, 0x85, 0x06, 0x85, 0x0A, 0x4B, 0x62,
                0x06, 0x00, 0xF4, 0xAD, 0xBD, 0x5C, 0xA3, 0xDC,
                0x84, 0x60
            };

            saveBuffer.Add(hack);
        }

        private void SavePillars(SaveBuffer saveBuffer)
        {
            // TODO: Load and save GV pillar data

            // GV checkpoint table: c80328 (20 * 128 bytes)
            // GV position data: c80d28 (20 * 1024 bytes)

            byte[] data = new byte[GPTrack.Count * (128 + 1024)];

            // Copy original pillar data to new location
            Buffer.BlockCopy(this.romBuffer, 0x4DE2E, data, 128, 128);
            Buffer.BlockCopy(this.romBuffer, 0x4DF08, data, 1024, 128);
            Buffer.BlockCopy(this.romBuffer, 0x4DD91, data, 2048, 128);
            Buffer.BlockCopy(this.romBuffer, 0x4DDB4, data, 3584, 172);
            Buffer.BlockCopy(this.romBuffer, 0x4DDB4, data, 6656, 120);
            Buffer.BlockCopy(this.romBuffer, 0x4DE60, data, 10752, 166);
            Buffer.BlockCopy(this.romBuffer, 0x4DD1D, data, 18944, 114);

            saveBuffer.Add(data);
        }

        private static void AddObjectCodeChunk3(SaveBuffer saveBuffer)
        {
            byte[] hack =
            {
                0xDA, 0xAE, 0x24, 0x01, 0xE2, 0x20, 0xBF, 0x7A,
                0x00, 0xC8, 0xFA, 0xC9, 0x06, 0xC2, 0x20, 0x5C,
                0x31, 0x9E, 0x81
            };

            saveBuffer.Add(hack);
        }

        private void SaveAIs(SaveBuffer saveBuffer)
        {
            int aiFirstAddressByteOffset = this.offsets[Offset.TrackAIDataFirstAddressByte];
            this.romBuffer[aiFirstAddressByteOffset] = 0xC8;

            byte[] trackOrder = this.GetTrackOrder();

            for (int i = 0; i < this.trackGroups.Length; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();
                int trackGroupSize = tracks.Length;

                for (int j = 0; j < trackGroupSize; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    this.SaveAI(tracks[j], trackIndex, saveBuffer);
                }
            }
        }

        private void SaveAI(Track track, int trackIndex, SaveBuffer saveBuffer)
        {
            byte[] trackAIData = track.AI.GetBytes();

            // Update AI offsets
            int trackAIZoneIndex = this.offsets[Offset.TrackAIZones] + trackIndex * 2;
            int trackAITargetIndex = this.offsets[Offset.TrackAITargets] + trackIndex * 2;

            byte[] aiZoneOffset = Utilities.OffsetToByteArray(saveBuffer.Index);
            byte[] aiTargetOffset = Utilities.OffsetToByteArray(saveBuffer.Index + trackAIData.Length - track.AI.ElementCount * 3);

            this.romBuffer[trackAIZoneIndex] = aiZoneOffset[0];
            this.romBuffer[trackAIZoneIndex + 1] = aiZoneOffset[1];
            this.romBuffer[trackAITargetIndex] = aiTargetOffset[0];
            this.romBuffer[trackAITargetIndex + 1] = aiTargetOffset[1];

            saveBuffer.Add(trackAIData);
        }

        private void SaveTracks(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();
            int[] mapAddresses = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Offset.TrackMaps], Track.Count);

            for (int i = 0; i < this.trackGroups.Length; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();
                int trackGroupSize = tracks.Length;

                for (int j = 0; j < trackGroupSize; j++)
                {
                    int iterator = i * GPTrack.CountPerGroup + j;
                    int trackIndex = trackOrder[iterator];

                    if (tracks[j].Modified)
                    {
                        this.SaveTrack(tracks[j], iterator, trackIndex, saveBuffer);
                    }
                    else
                    {
                        int trackOffset = mapAddresses[trackIndex];
                        bool isInZone = saveBuffer.Includes(trackOffset);

                        if (isInZone)
                        {
                            this.MoveTrackMap(trackIndex, trackOffset, saveBuffer);
                        }
                    }
                }
            }
        }

        private void SaveTrack(Track track, int iterator, int trackIndex, SaveBuffer saveBuffer)
        {
            bool quirksMode = this.region != Region.US;
            byte[] compressedTrack = Codec.Compress(Codec.Compress(track.Map.GetBytes(), quirksMode), quirksMode);

            // Update track theme id
            byte themeId = this.themes.GetThemeId(track.Theme);
            int themeIdOffset = this.offsets[Offset.TrackThemes] + trackIndex;
            this.romBuffer[themeIdOffset] = themeId;

            // Update overlay tiles
            byte[] overlayTileData = track.OverlayTiles.GetBytes();
            this.SaveOverlayTileData(trackIndex, overlayTileData);

            if (track is GPTrack)
            {
                GPTrack gpTrack = track as GPTrack;
                byte[] data;

                // Update driver starting position
                this.SaveGPStartPositionData(gpTrack, trackIndex);

                // Update lap line position and length
                data = gpTrack.LapLine.GetBytes();
                Buffer.BlockCopy(data, 0, this.romBuffer, this.offsets[Offset.TrackLapLines] + trackIndex * data.Length, data.Length);

                // Update lap line position on track preview
                int previewLapLineOffset = offsets[Offset.TrackPreviewLapLines] + iterator * 2;
                Point previewLapLineLocation = Game.GetPreviewLapLineLocation(gpTrack);
                this.romBuffer[previewLapLineOffset] = (byte)previewLapLineLocation.X;
                this.romBuffer[previewLapLineOffset + 1] = (byte)previewLapLineLocation.Y;
            }

            this.SaveTrackSub(trackIndex, compressedTrack, saveBuffer);
        }

        private static Point GetPreviewLapLineLocation(GPTrack track)
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

        private void MoveTrackMap(int trackIndex, int trackOffset, SaveBuffer saveBuffer)
        {
            int compressedTrackLength = Codec.GetLength(this.romBuffer, trackOffset);
            byte[] compressedTrack = new byte[compressedTrackLength];
            Buffer.BlockCopy(this.romBuffer, trackOffset, compressedTrack, 0, compressedTrackLength);

            this.SaveTrackSub(trackIndex, compressedTrack, saveBuffer);
        }

        private void SaveTrackSub(int trackIndex, byte[] compressedTrack, SaveBuffer saveBuffer)
        {
            // Update track offset
            byte[] offset = Utilities.OffsetToByteArray(saveBuffer.Index);
            int trackAddressIndex = this.offsets[Offset.TrackMaps] + trackIndex * 3;
            Buffer.BlockCopy(offset, 0, this.romBuffer, trackAddressIndex, 3);

            saveBuffer.Add(compressedTrack);
        }

        private void SaveItemProbabilities()
        {
            this.itemProbabilities.Save(this.romBuffer, offsets[Offset.ItemProbabilities]);
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
        #endregion Save data

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
