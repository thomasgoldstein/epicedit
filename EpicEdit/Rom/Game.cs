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
using System.Drawing;
using System.Globalization;
using System.IO;

using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.Rom.Tracks.Start;
using EpicEdit.UI.Gfx;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace EpicEdit.Rom
{
    internal enum Region
    {
        Jap,
        US,
        Euro
    }

    /// <summary>
    /// The Super Mario Kart Game class, which contains all of the game data.
    /// </summary>
    internal sealed class Game : IDisposable
    {
        #region Public properties and methods

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
            if (this.modeNames == null)
            {
                this.modeNames = new string[3];

                int offset = this.offsets[Offset.ModeStrings];
                int nameOffset = Utilities.BytesToOffset(this.romBuffer[offset], this.romBuffer[offset + 1], 5);
                int lengthOffset = offset + 6;

                for (int i = 0; i < this.modeNames.Length; i++)
                {
                    int length = this.romBuffer[lengthOffset] * 2;
                    byte[] hexText = Utilities.ReadBlock(this.romBuffer, nameOffset, length);
                    this.modeNames[i] = Utilities.DecryptRomTextOdd(hexText, this.region);
                    nameOffset += length;
                    lengthOffset += 2;
                }
            }

            return this.modeNames;
        }

        /// <summary>
        /// Gets the game settings.
        /// </summary>
        public GameSettings Settings { get; private set; }

        /// <summary>
        /// Gets the track object graphics.
        /// </summary>
        public TrackObjectGraphics ObjectGraphics { get; private set; }

        /// <summary>
        /// Gets the item icon graphics.
        /// </summary>
        public ItemIconGraphics ItemIconGraphics { get; private set; }

        public byte[] Compress(byte[] data, bool twice)
        {
            Codec.Optimal = true;

            bool quirksMode = this.region != Region.US;
            data = Codec.Compress(data, quirksMode);

            if (twice)
            {
                data = Codec.Compress(data);
            }

            Codec.Optimal = false;

            return data;
        }

        public void InsertData(byte[] data, int offset)
        {
            offset -= this.romHeader.Length;
            Buffer.BlockCopy(data, 0, this.romBuffer, offset, data.Length);
            this.modified = true;
        }

        public byte[] Decompress(int offset, bool twice)
        {
            offset -= this.romHeader.Length;

            byte[] data = Codec.Decompress(this.romBuffer, offset);

            if (twice)
            {
                data = Codec.Decompress(data);
            }

            return data;
        }

        public int GetCompressedChunkLength(int offset, bool doubleCompressed)
        {
            offset -= this.romHeader.Length;

            if (!doubleCompressed)
            {
                return Codec.GetLength(this.romBuffer, offset);
            }

            byte[] data = Codec.Decompress(this.romBuffer, offset);
            return Codec.GetLength(data);
        }

        public int HeaderSize
        {
            get { return this.romHeader.Length; }
        }
        #endregion Public properties and methods

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

        private string[] modeNames;

        /// <summary>
        /// The different overlay tile sizes.
        /// </summary>
        private OverlayTileSizes overlayTileSizes;

        /// <summary>
        /// The different overlay tile patterns.
        /// </summary>
        private OverlayTilePatterns overlayTilePatterns;

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

        #region Read ROM & validate

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

            if (romHeaderSize == 0)
            {
                this.romHeader = new byte[0];
            }
            else if (romHeaderSize == 512)
            {
                this.romHeader = Utilities.ReadBlock(this.romBuffer, 0, romHeaderSize);
                byte[] romBufferWithoutHeader = Utilities.ReadBlock(this.romBuffer, romHeaderSize, this.romBuffer.Length - romHeaderSize);
                this.romBuffer = romBufferWithoutHeader;
            }
            else
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

        public int Size
        {
            get { return this.romBuffer.Length; }
        }

        #endregion Read ROM & validate

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
            byte[] overlayTileSizesData = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackOverlaySizes], OverlayTileSizes.Size);
            this.overlayTileSizes = new OverlayTileSizes(overlayTileSizesData);
            this.overlayTilePatterns = new OverlayTilePatterns(this.romBuffer, this.offsets, this.overlayTileSizes);

            byte[] trackThemes = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackThemes], Track.Count);
            byte[] trackOrder = this.GetTrackOrder();
            byte[][] trackNameIndex = this.GetTrackNameIndexes();

            int[] mapOffsets = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Offset.TrackMaps], Track.Count);

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

                    byte[] trackMap = Codec.Decompress(Codec.Decompress(this.romBuffer, mapOffsets[trackIndex]), 0, TrackMap.SquareSize);

                    byte[] overlayTileData = this.GetOverlayTileData(trackIndex);

                    byte[] aiZoneData, aiTargetData;
                    this.LoadAIData(trackIndex, aiOffsetBase, aiZoneOffsets, aiTargetOffsets, out aiZoneData, out aiTargetData);

                    if (trackIndex < GPTrack.Count) // GP track
                    {
                        byte[] startPositionData = this.GetGPStartPositionData(trackIndex);
                        byte[] lapLineData = this.GetLapLineData(trackIndex);
                        byte[] objectData = this.GetObjectData(trackIndex);
                        byte[] objectZoneData = this.GetObjectZoneData(trackIndex);
                        int itemProbaIndex = this.romBuffer[this.offsets[Offset.TrackItemProbabilityIndexes] + trackIndex] >> 1;

                        GPTrack gpTrack = new GPTrack(trackName, trackTheme,
                                                      trackMap, overlayTileData,
                                                      aiZoneData, aiTargetData,
                                                      startPositionData, lapLineData,
                                                      objectData, objectZoneData,
                                                      this.overlayTileSizes,
                                                      this.overlayTilePatterns,
                                                      itemProbaIndex);

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

            this.Settings = new GameSettings(this.romBuffer, this.offsets);
            this.ObjectGraphics = new TrackObjectGraphics(this.romBuffer, this.offsets);
            this.ItemIconGraphics = new ItemIconGraphics(this.romBuffer, this.offsets);
        }

        private void SetRegion()
        {
            int regionOffset = 0xFFD9;
            int region = this.romBuffer[regionOffset];

            if (!Enum.IsDefined(typeof(Region), region))
            {
                regionOffset += this.romHeader.Length;

                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                                                             "\"{0}\" has an invalid region. Value at {1:X} must be 0, 1 or 2, was: {2:X}.",
                                                             this.FileName, regionOffset, region));
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

        #region Get / set, load / save specific data

        #region Track overlay tiles

        private byte[] GetOverlayTileData(int trackIndex)
        {
            int offset = this.GetOverlayTileDataOffset(trackIndex);
            return Utilities.ReadBlock(this.romBuffer, offset, OverlayTiles.Size);
        }

        private void SaveOverlayTileData(byte[] data, int trackIndex)
        {
            int offset = this.GetOverlayTileDataOffset(trackIndex);
            Buffer.BlockCopy(data, 0, this.romBuffer, offset, OverlayTiles.Size);
        }

        private int GetOverlayTileDataOffset(int trackIndex)
        {
            return this.offsets[Offset.TrackOverlayItems] + trackIndex * OverlayTiles.Size;
        }

        #endregion Track overlay tiles

        #region GP start positions

        private byte[] GetGPStartPositionData(int trackIndex)
        {
            int offset = this.GetGPStartPositionDataOffset(trackIndex);
            return Utilities.ReadBlock(this.romBuffer, offset, GPStartPosition.Size);
        }

        private void SaveGPStartPositionData(GPTrack track, int trackIndex)
        {
            byte[] data = track.StartPosition.GetBytes();
            int offset = this.GetGPStartPositionDataOffset(trackIndex);
            Buffer.BlockCopy(data, 0, this.romBuffer, offset, GPStartPosition.Size);
        }

        private int GetGPStartPositionDataOffset(int trackIndex)
        {
            // TODO: Retrieve order dynamically from the ROM
            int[] reorder = { 14, 10, 7, 8, 15, 19, 16, 4, 17, 13, 6, 12, 11, 5, 18, 9, 2, 3, 1, 0 };
            return this.offsets[Offset.GPTrackStartPositions] + reorder[trackIndex] * 8;
        }

        #endregion GP start positions

        #region Lap line

        private byte[] GetLapLineData(int trackIndex)
        {
            int offset = this.offsets[Offset.TrackLapLines] + trackIndex * LapLine.Size;
            return Utilities.ReadBlock(this.romBuffer, offset, LapLine.Size);
        }

        #endregion Lap line

        #region Battle start positions

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

        #endregion Battle start positions

        #region Objects

        private byte[] GetObjectData(int trackIndex)
        {
            int objectOffset = this.offsets[Offset.TrackObjects] + (trackIndex * 64);
            return Utilities.ReadBlock(this.romBuffer, objectOffset, TrackObjects.Size);
        }

        private void SetObjectProperties(GPTrack track, int trackIndex, int themeId)
        {
            byte[] paletteIndexes;

            if (this.ObjectZonesRelocated)
            {
                int offset = this.offsets[Offset.TrackObjectProperties] + trackIndex;
                track.ObjectTileset = (ObjectType)this.romBuffer[offset];
                track.ObjectInteraction = (ObjectType)this.romBuffer[offset + Track.Count];
                track.ObjectRoutine = (ObjectType)this.romBuffer[offset + Track.Count * 2];
                paletteIndexes = this.GetObjectPaletteIndexes(trackIndex);
                track.ObjectFlashing = this.GetObjectFlashing(trackIndex);
            }
            else
            {
                ObjectType objectType = Game.GetObjectType(themeId, trackIndex);
                track.ObjectTileset = objectType;
                track.ObjectInteraction = objectType;
                track.ObjectRoutine = objectType;
                paletteIndexes = Game.GetObjectPaletteIndexes(themeId, trackIndex);
                track.ObjectFlashing = themeId == 7; // Rainbow Road
            }

            track.ObjectPaletteIndexes[0] = paletteIndexes[0];
            track.ObjectPaletteIndexes[1] = paletteIndexes[1];
            track.ObjectPaletteIndexes[2] = paletteIndexes[2];
            track.ObjectPaletteIndexes[3] = paletteIndexes[3];
        }

        private static ObjectType GetObjectType(int themeId, int trackIndex)
        {
            ObjectType objectType;

            if (trackIndex == 19) // Donut Plains 1
            {
                // This track is an exception
                objectType = ObjectType.Pipe;
            }
            else
            {
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
                        objectType = ObjectType.Mole;
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

                    default: throw new ArgumentOutOfRangeException("themeId");
                }
            }

            return objectType;
        }

        private byte[] GetObjectPaletteIndexes(int trackIndex)
        {
            int offset = this.offsets[Offset.TrackObjectPaletteIndexes] + trackIndex * 4;
            byte[] data =
            {
                (byte)(this.romBuffer[offset++] >> 1),
                (byte)(this.romBuffer[offset++] >> 1),
                (byte)(this.romBuffer[offset++] >> 1),
                (byte)(this.romBuffer[offset] >> 1)
            };

            return data;
        }

        /// <summary>
        /// Guesses the object palette indexes from the theme id (and track index for Donut Plains 1).
        /// </summary>
        private static byte[] GetObjectPaletteIndexes(int themeId, int trackIndex)
        {
            byte[] paletteIndexes = new byte[4];

            if (trackIndex == 19) // Donut Plains 1
            {
                // This track is an exception
                paletteIndexes[0] = 5;
            }
            else
            {
                switch (themeId)
                {
                    case 0: // Ghost Valley
                    case 1: // Mario Circuit
                    case 2: // Donut Plains
                    case 4: // Vanilla Lake
                        paletteIndexes[0] = 7;
                        break;

                    case 3: // Choco Island
                    case 5: // Koopa Beach
                        paletteIndexes[0] = 6;
                        break;

                    case 6: // Bowser Castle
                        paletteIndexes[0] = 4;
                        break;

                    case 7: // Rainbow Road
                        paletteIndexes[0] = 1;
                        paletteIndexes[1] = 7;
                        paletteIndexes[2] = 4;
                        paletteIndexes[3] = 7;
                        break;

                    default: throw new ArgumentOutOfRangeException("themeId");
                }
            }

            return paletteIndexes;
        }

        private bool GetObjectFlashing(int trackIndex)
        {
            int offset = this.offsets[Offset.TrackObjectFlashing] + trackIndex;
            return this.romBuffer[offset] != 0;
        }

        #endregion Objects

        #region Object zones

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
                return this.offsets[Offset.TrackObjectZonesRelocated] + trackIndex * 10;
            }

            // TODO: Retrieve order dynamically from the ROM
            int[] reorder =
            {
                2, -1 /* Ghost Valley x */, 12, 8, 15,
                10, 17, 0, -1 /* Ghost Valley x */, 9,
                5, 13, 14, 17, 3,
                1, -1 /* Ghost Valley x */, 7, 4, 11
            };

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
            get { return this.romBuffer[this.offsets[Offset.TrackObjectHack6]] == 0xB7; }
        }

        #endregion Object zones

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

        #endregion Get / set, load / save specific data

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
            this.SaveObjectData(saveBuffer);
            this.SaveTileGenres(saveBuffer);
            this.SaveAIs(saveBuffer);
            this.SaveTracks(saveBuffer);
            this.SaveThemes(saveBuffer);
            this.romBuffer = saveBuffer.GetRomBuffer();

            this.SaveSettings();
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

                if (this.romBuffer.Length < romSizes[sizeIndex])
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
            // Saves data from 0x80000 to 0x80061
            byte[] trackOrder = this.GetTrackOrder();

            Track[] tracks = this.trackGroups[GPTrack.GroupCount].GetTracks();

            for (int i = 0; i < tracks.Length; i++)
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

            // Some values differ depending on the ROM region
            byte diff;

            switch (this.region)
            {
                case Region.Jap:
                    diff = 0x14;
                    break;

                default:
                case Region.US:
                    diff = 0;
                    break;

                case Region.Euro:
                    diff = 0x19;
                    break;
            }

            byte val1 = (byte)(0x79 + diff);
            byte val2 = (byte)(0x26 + diff);
            byte val3 = (byte)(0x18 + diff);

            // New code for battle track starting positions
            byte[] hack =
            {
                0xAD, 0x24, 0x01, 0xC9, 0x14, 0x00, 0x90, 0x35,
                0xE9, 0x14, 0x00, 0x0A, 0x0A, 0x0A, 0xAA, 0xBF,
                0x00, 0x00, 0xC8, 0x8D, 0x18, 0x10, 0xBF, 0x02,
                0x00, 0xC8, 0x8D, 0x1C, 0x10, 0xBF, 0x04, 0x00,
                0xC8, 0x8D, 0x18, 0x11, 0xBF, 0x06, 0x00, 0xC8,
                0x8D, 0x1C, 0x11, 0x9C, 0x20, 0x10, 0x9C, 0x20,
                0x11, 0xAD, 0x24, 0x01, 0x0A, 0xAA, 0xBC, val1,
                0x8A, 0x5C, val2, 0x8F, 0x81, 0x0A, 0x5C, val3,
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
                - Make it possible to define the object palette used for each track
                - Make it possible to add Rainbow Road Thwomp-like flashing (palette cycler) for each track
            */

            this.RelocateObjectData();
            this.SaveObjectProperties(saveBuffer); // From 0x80062 to 0x800D9
            Game.AddObjectCodeChunk1(saveBuffer, region); // From 0x800DA to 0x80218
            this.SaveObjectLocationsAndZones(saveBuffer); // From 0x80219 to 0x802E1
            Game.AddObjectCodeChunk2(saveBuffer); // From 0x802E2 to 0x80330
            this.SavePillars(saveBuffer); // From 0x80331 to 0x85D30
            Game.AddObjectCodeChunk3(saveBuffer, region); // From 0x85D31 to 0x85D43
            this.SaveObjectPalettes(saveBuffer); // From 0x85D44 to 0x85DBB
            Game.AddObjectPaletteCodeChunk(saveBuffer, region); // From 0x85DBC to 0x85EFC
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
                normal zone table: c80219
                GV checkpoint table: c80331
                GV position data: c80d31
                E ptr: c85d31
                palettes: c85d44
                palette cycle flags: c85da4
                pw1: c85dbc
                pw2: c85e1d
                pw3: c85e49
                co: c85e84
                getcp: c85ed1
            */

            int offset = this.offsets[Offset.TrackObjectHack1];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x23;
            this.romBuffer[offset++] = 0x01;
            this.romBuffer[offset] = 0xC8;

            offset = this.offsets[Offset.TrackObjectHack2];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x4F;
            this.romBuffer[offset++] = 0x01;
            this.romBuffer[offset] = 0xC8;

            offset = this.offsets[Offset.TrackObjectHack3];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x31;
            this.romBuffer[offset++] = 0x5D;
            this.romBuffer[offset] = 0xC8;

            offset = this.offsets[Offset.TrackObjectHack4];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0xDA;
            this.romBuffer[offset++] = 0x00;
            this.romBuffer[offset] = 0xC8;

            offset = this.offsets[Offset.TrackObjectHack5];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0xAB;
            this.romBuffer[offset++] = 0x01;
            this.romBuffer[offset] = 0xC8;

            this.romBuffer[this.offsets[Offset.TrackObjectHack6]] = 0xB7;
            this.romBuffer[this.offsets[Offset.TrackObjectHack7]] = 0xB7;
            this.romBuffer[this.offsets[Offset.TrackObjectHack8]] = 0xB7;

            // Object palette changes:

            offset = this.offsets[Offset.TrackObjectPalHack1];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0xBC;
            this.romBuffer[offset++] = 0x5D;
            this.romBuffer[offset] = 0xC8;

            offset = this.offsets[Offset.TrackObjectPalHack2];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x49;
            this.romBuffer[offset++] = 0x5E;
            this.romBuffer[offset] = 0xC8;

            offset = this.offsets[Offset.TrackObjectPalHack3];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x1D;
            this.romBuffer[offset++] = 0x5E;
            this.romBuffer[offset] = 0xC8;

            this.romBuffer[this.offsets[Offset.TrackObjectPalHack4]] = 0x80;
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

                for (int j = 0; j < tracks.Length; j++)
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

        private static void AddObjectCodeChunk1(SaveBuffer saveBuffer, Region region)
        {
            byte[] hack1 =
            {
                0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF, 0x7A, 0x00,
                0xC8, 0xAA, 0xBF, 0x15, 0x01, 0xC8, 0x8D, 0x30,
                0x0E, 0x9C, 0x31, 0x0E, 0xAE, 0x24, 0x01, 0xBF,
                0x62, 0x00, 0xC8, 0xAA, 0xBF, 0x1C, 0x01, 0xC8,
                0x48, 0x4A, 0x18, 0x63, 0x01, 0x83, 0x01, 0x68,
                0xC2, 0x30, 0x29, 0xFF, 0x00, 0xAA, 0xBF
            };

            byte[] hack2;

            switch (region)
            {
                case Region.Jap:
                    hack2 = new byte[]
                    {
                        0xD7, 0xEB, 0x81, 0xA8, 0xBF, 0xD9, 0xEB, 0x81,
                        0x5C, 0x9C, 0xE9, 0x81, 0x02, 0x00, 0x0C, 0x04,
                        0x06, 0x0A, 0x0C, 0x02, 0x00, 0x0C, 0x04, 0x06,
                        0x0A, 0x0E, 0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF,
                        0xAA, 0x00, 0xC8, 0xAA, 0xBF, 0x48, 0x01, 0xC8,
                        0xAA, 0xC2, 0x20, 0xBF, 0xE1, 0x8B, 0x81, 0x8D,
                        0xFC, 0x0F, 0xBF, 0xF1, 0x8B, 0x81, 0x8D, 0xFE,
                        0x0F, 0xC2, 0x30, 0x5C, 0xFF, 0x8E, 0x81, 0x02,
                        0x00, 0x0C, 0x04, 0x06, 0x0A, 0x0C, 0xE2, 0x30,
                        0xAE, 0x24, 0x01, 0xBF, 0x92, 0x00, 0xC8, 0x0A,
                        0x48, 0x0A, 0x18, 0x63, 0x01, 0x83, 0x01, 0xFA,
                        0xC2, 0x20, 0xBF, 0x81, 0x01, 0xC8, 0x85, 0x06,
                        0xBF, 0x83, 0x01, 0xC8, 0x8D, 0x98, 0x0F, 0x85,
                        0x08, 0xBF, 0x85, 0x01, 0xC8, 0x8D, 0x9A, 0x0F,
                        0x85, 0x0A, 0xC2, 0x10, 0x5C, 0x6A, 0x91, 0x81,
                        0xE4, 0x91, 0x0C, 0xE5, 0x1C, 0xE5, 0xE4, 0x91,
                        0x04, 0xE7, 0x0E, 0xE7, 0xE4, 0x91, 0xB0, 0xE1,
                        0xC2, 0xE1, 0xF8, 0x91, 0x90, 0xE3, 0xA0, 0xE3,
                        0xE4, 0x91, 0x93, 0xE0, 0x9F, 0xE0, 0xD4, 0x91,
                        0xA3, 0xDF, 0xB3, 0xDF, 0xE4, 0x91, 0x69, 0xE1,
                        0x7B
                    };
                    break;

                default:
                case Region.US:
                    hack2 = new byte[]
                    {
                        0xD3, 0xEB, 0x81, 0xA8, 0xBF, 0xD5, 0xEB, 0x81,
                        0x5C, 0x98, 0xE9, 0x81, 0x02, 0x00, 0x0C, 0x04,
                        0x06, 0x0A, 0x0C, 0x02, 0x00, 0x0C, 0x04, 0x06,
                        0x0A, 0x0E, 0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF,
                        0xAA, 0x00, 0xC8, 0xAA, 0xBF, 0x48, 0x01, 0xC8,
                        0xAA, 0xC2, 0x20, 0xBF, 0xCD, 0x8B, 0x81, 0x8D,
                        0xFC, 0x0F, 0xBF, 0xDD, 0x8B, 0x81, 0x8D, 0xFE,
                        0x0F, 0xC2, 0x30, 0x5C, 0xEB, 0x8E, 0x81, 0x02,
                        0x00, 0x0C, 0x04, 0x06, 0x0A, 0x0C, 0xE2, 0x30,
                        0xAE, 0x24, 0x01, 0xBF, 0x92, 0x00, 0xC8, 0x0A,
                        0x48, 0x0A, 0x18, 0x63, 0x01, 0x83, 0x01, 0xFA,
                        0xC2, 0x20, 0xBF, 0x81, 0x01, 0xC8, 0x85, 0x06,
                        0xBF, 0x83, 0x01, 0xC8, 0x8D, 0x98, 0x0F, 0x85,
                        0x08, 0xBF, 0x85, 0x01, 0xC8, 0x8D, 0x9A, 0x0F,
                        0x85, 0x0A, 0xC2, 0x10, 0x5C, 0x56, 0x91, 0x81,
                        0xD0, 0x91, 0xE7, 0xE4, 0xF7, 0xE4, 0xD0, 0x91,
                        0xFE, 0xE6, 0x08, 0xE7, 0xD0, 0x91, 0x8B, 0xE1,
                        0x9D, 0xE1, 0xE4, 0x91, 0x6B, 0xE3, 0x7B, 0xE3,
                        0xD0, 0x91, 0x6E, 0xE0, 0x7A, 0xE0, 0xC0, 0x91,
                        0x7E, 0xDF, 0x8E, 0xDF, 0xD0, 0x91, 0x44, 0xE1,
                        0x56
                    };
                    break;

                case Region.Euro:
                    hack2 = new byte[]
                    {
                        0xC2, 0xEB, 0x81, 0xA8, 0xBF, 0xC4, 0xEB, 0x81,
                        0x5C, 0x87, 0xE9, 0x81, 0x02, 0x00, 0x0C, 0x04,
                        0x06, 0x0A, 0x0C, 0x02, 0x00, 0x0C, 0x04, 0x06,
                        0x0A, 0x0E, 0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF,
                        0xAA, 0x00, 0xC8, 0xAA, 0xBF, 0x48, 0x01, 0xC8,
                        0xAA, 0xC2, 0x20, 0xBF, 0xE6, 0x8B, 0x81, 0x8D,
                        0xFC, 0x0F, 0xBF, 0xF6, 0x8B, 0x81, 0x8D, 0xFE,
                        0x0F, 0xC2, 0x30, 0x5C, 0x01, 0x8F, 0x81, 0x02,
                        0x00, 0x0C, 0x04, 0x06, 0x0A, 0x0C, 0xE2, 0x30,
                        0xAE, 0x24, 0x01, 0xBF, 0x92, 0x00, 0xC8, 0x0A,
                        0x48, 0x0A, 0x18, 0x63, 0x01, 0x83, 0x01, 0xFA,
                        0xC2, 0x20, 0xBF, 0x81, 0x01, 0xC8, 0x85, 0x06,
                        0xBF, 0x83, 0x01, 0xC8, 0x8D, 0x98, 0x0F, 0x85,
                        0x08, 0xBF, 0x85, 0x01, 0xC8, 0x8D, 0x9A, 0x0F,
                        0x85, 0x0A, 0xC2, 0x10, 0x5C, 0x6F, 0x91, 0x81,
                        0xE9, 0x91, 0x0C, 0xE5, 0x1C, 0xE5, 0xE9, 0x91,
                        0x23, 0xE7, 0x2D, 0xE7, 0xE9, 0x91, 0xB0, 0xE1,
                        0xC2, 0xE1, 0xFD, 0x91, 0x90, 0xE3, 0xA0, 0xE3,
                        0xE9, 0x91, 0x93, 0xE0, 0x9F, 0xE0, 0xD9, 0x91,
                        0xA3, 0xDF, 0xB3, 0xDF, 0xE9, 0x91, 0x69, 0xE1,
                        0x7B
                    };
                    break;
            }

            byte[] hack3 =
            {
                0xE1, 0xE2, 0x30, 0xAE, 0x24, 0x01, 0xBF, 0xC2,
                0x00, 0xC8, 0x0A, 0xAA, 0xFC, 0xBF, 0x01, 0xC2,
                0x30, 0x5C, 0xC2, 0xDA, 0x84, 0xC8, 0x01, 0xE1,
                0x02, 0xF5, 0x02, 0xC7, 0x01, 0x60, 0xC2, 0x30,
                0xAD, 0x24, 0x01, 0x0A, 0x0A, 0x18, 0x6D, 0x24,
                0x01, 0x0A, 0x18, 0x69, 0x19, 0x02, 0x85, 0x08,
                0xA9, 0xC8, 0x00, 0x85, 0x0A, 0xAC, 0xE4, 0x1E,
                0xB6, 0xC8, 0x10, 0x11, 0xA5, 0x08, 0x18, 0x69,
                0x05, 0x00, 0x85, 0x08, 0x98, 0x49, 0x02, 0x00,
                0xA8, 0xB6, 0xC8, 0x30, 0x20, 0xA0, 0x00, 0x00,
                0xE2, 0x20, 0xB5, 0xC0, 0xC9, 0xFF, 0xF0, 0x06,
                0x88, 0xC8, 0xD7, 0x08, 0xB0, 0xFB, 0xC2, 0x20,
                0xF4, 0x6B, 0x00, 0x3B, 0x4B, 0xF4, 0x16, 0x02,
                0x48, 0x5C, 0x0B, 0xDC, 0x84, 0x68, 0x60
            };

            saveBuffer.Add(hack1);
            saveBuffer.Add(hack2);
            saveBuffer.Add(hack3);
        }

        private void SaveObjectLocationsAndZones(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();
            byte[] objectZonesData = new byte[GPTrack.Count * TrackObjectZones.Size];

            for (int i = 0; i < this.trackGroups.Length - 1; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();

                for (int j = 0; j < tracks.Length; j++)
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
                0x20, 0xC8, 0x01, 0x90, 0x0E, 0xF4, 0x6B, 0x00,
                0x3B, 0x4B, 0xF4, 0xF2, 0x02, 0x48, 0x5C, 0xBC,
                0xDB, 0x84, 0x68, 0x60, 0xC2, 0x30, 0xAC, 0xE4,
                0x1E, 0xB6, 0xC8, 0x30, 0x32, 0xB9, 0x7C, 0xDC,
                0x85, 0x0C, 0xAD, 0x24, 0x01, 0xEB, 0x4A, 0x85,
                0x04, 0x0A, 0x0A, 0x0A, 0x18, 0x69, 0x31, 0x0D,
                0x85, 0x08, 0xA5, 0x04, 0x18, 0x69, 0x31, 0x03,
                0x85, 0x04, 0xA9, 0xC8, 0x00, 0x85, 0x06, 0x85,
                0x0A, 0xF4, 0x6B, 0x00, 0x3B, 0x4B, 0xF4, 0x2E,
                0x03, 0x48, 0x5C, 0xA3, 0xDC, 0x84, 0x68, 0x60
            };

            saveBuffer.Add(hack);
        }

        private void SavePillars(SaveBuffer saveBuffer)
        {
            // TODO: Load and save GV pillar data

            // GV checkpoint table: c80331 (20 * 128 bytes)
            // GV position data: c80d31 (20 * 1024 bytes)

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

        private static void AddObjectCodeChunk3(SaveBuffer saveBuffer, Region region)
        {
            byte val;

            // A value differs depending on the ROM region
            switch (region)
            {
                case Region.Jap:
                    val = 0x94;
                    break;

                default:
                case Region.US:
                    val = 0x31;
                    break;

                case Region.Euro:
                    val = 0x6E;
                    break;
            }

            byte[] hack =
            {
                0xDA, 0xAE, 0x24, 0x01, 0xE2, 0x20, 0xBF, 0x7A,
                0x00, 0xC8, 0xFA, 0xC9, 0x06, 0xC2, 0x20, 0x5C,
                val, 0x9E, 0x81
            };

            saveBuffer.Add(hack);
        }

        private void SaveObjectPalettes(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();
            byte[] objectPalData = new byte[Track.Count * 5];
            int flashingOffset = Track.Count * 4;

            for (int i = 0; i < this.trackGroups.Length - 1; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();

                for (int j = 0; j < tracks.Length; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    int offset = trackIndex * 4;
                    GPTrack gpTrack = tracks[j] as GPTrack;

                    objectPalData[offset++] = (byte)(gpTrack.ObjectPaletteIndexes[0] << 1);
                    objectPalData[offset++] = (byte)(gpTrack.ObjectPaletteIndexes[1] << 1);
                    objectPalData[offset++] = (byte)(gpTrack.ObjectPaletteIndexes[2] << 1);
                    objectPalData[offset] = (byte)(gpTrack.ObjectPaletteIndexes[3] << 1);

                    objectPalData[flashingOffset + trackIndex] =
                        gpTrack.ObjectFlashing ? (byte)1 : (byte)0;
                }
            }

            saveBuffer.Add(objectPalData);
        }

        private static void AddObjectPaletteCodeChunk(SaveBuffer saveBuffer, Region region)
        {
            byte val1;
            byte val2;

            if (region == Region.US)
            {
                val1 = 0x2A;
                val2 = 0x8F;
            }
            else
            {
                val1 = 0x4F;
                val2 = 0xB4;
            }

            byte[] hack1 =
            {
                0x20, 0x84, 0x5E, 0xD0, 0x3C, 0xD4, 0x00, 0x20,
                0xD1, 0x5E, 0xB9, 0x06, 0x00, 0x45, 0x1A, 0x29,
                0xFF, 0xF1, 0x05, 0x00, 0x95, 0x0E, 0xB9, 0x04,
                0x00, 0x45, 0x1A, 0x29, 0xFF, 0xF1, 0x05, 0x00,
                0x95, 0x0A, 0xB9, 0x02, 0x00, 0x45, 0x1A, 0x29,
                0xFF, 0xF1, 0x05, 0x00, 0x95, 0x06, 0xB9, 0x00,
                0x00, 0x45, 0x1A, 0x29, 0xFF, 0xF1, 0x05, 0x00,
                0x95, 0x02, 0x68, 0x85, 0x00, 0x5C, val1, 0xBD,
                0x80, 0xB9, 0x06, 0x00, 0x45, 0x1A, 0x95, 0x0E,
                0xB9, 0x04, 0x00, 0x45, 0x1A, 0x95, 0x0A, 0xB9,
                0x02, 0x00, 0x45, 0x1A, 0x95, 0x06, 0xB9, 0x00,
                0x00, 0x45, 0x1A, 0x95, 0x02, 0x5C, val1, 0xBD,
                0x80, 0x20, 0x84, 0x5E, 0xD0, 0x1A, 0xD4, 0x00,
                0x20, 0xD1, 0x5E, 0xBD, 0x02, 0x00, 0xA6, 0x3C,
                0x45, 0x1A, 0x29, 0xFF, 0xF1, 0x05, 0x00, 0x95,
                0x02, 0x68, 0x85, 0x00, 0x5C, val2, 0xBD, 0x80,
                0xBD, 0x02, 0x00, 0xA6, 0x3C, 0x45, 0x1A, 0x95,
                0x02, 0x5C, val2, 0xBD, 0x80, 0x20, 0x84, 0x5E,
                0xD0, 0x24, 0xD4, 0x00, 0x20, 0xD1, 0x5E, 0xB9,
                0x02, 0x00, 0x45, 0x1A, 0x29, 0xFF, 0xF1, 0x05,
                0x00, 0x95, 0x06, 0xB9, 0x00, 0x00, 0x45, 0x1A,
                0x29, 0xFF, 0xF1, 0x05, 0x00, 0x95, 0x02, 0x68,
                0x85, 0x00, 0x5C, val1, 0xBD, 0x80, 0xB9, 0x02,
                0x00, 0x45, 0x1A, 0x95, 0x06, 0xB9, 0x00, 0x00,
                0x45, 0x1A, 0x95, 0x02, 0x5C, val1, 0xBD, 0x80,
                0xDA, 0xA6, 0xB4, 0xB5, 0x04, 0xAE, 0x24, 0x01,
                0x48, 0xBF, 0x92, 0x00, 0xC8, 0x29, 0xFF, 0x00,
                0x0A, 0xAA, 0x68, 0xDF, 0xB5, 0x5E, 0xC8, 0xF0,
                0x11, 0xDF, 0xC3, 0x5E, 0xC8, 0xF0, 0x0B, 0xC0
            };

            byte[] hack2;

            switch (region)
            {
                case Region.Jap:
                    hack2 = new byte[]
                    {
                        0x73, 0xE4, 0xF0, 0x06, 0xFA, 0x18, 0xA9, 0x01,
                        0x00, 0x60, 0x18, 0xFA, 0x18, 0xA9, 0x00, 0x00,
                        0x60, 0x0C, 0xE5, 0x04, 0xE7, 0xB0, 0xE1, 0x90,
                        0xE3, 0x93, 0xE0, 0xA3, 0xDF, 0x69, 0xE1, 0x1C,
                        0xE5, 0x0E, 0xE7, 0xC2, 0xE1, 0xA0, 0xE3, 0x9F,
                        0xE0, 0xB3, 0xDF, 0x7B
                    };
                    break;

                default:
                case Region.US:
                    hack2 = new byte[]
                    {
                        0x4E, 0xE4, 0xF0, 0x06, 0xFA, 0x18, 0xA9, 0x01,
                        0x00, 0x60, 0x18, 0xFA, 0x18, 0xA9, 0x00, 0x00,
                        0x60, 0xE7, 0xE4, 0xFE, 0xE6, 0x8B, 0xE1, 0x6B,
                        0xE3, 0x6E, 0xE0, 0x7E, 0xDF, 0x44, 0xE1, 0xF7,
                        0xE4, 0x08, 0xE7, 0x9D, 0xE1, 0x7B, 0xE3, 0x7A,
                        0xE0, 0x8E, 0xDF, 0x56
                    };
                    break;

                case Region.Euro:
                    hack2 = new byte[]
                    {
                        0x7E, 0xE4, 0xF0, 0x06, 0xFA, 0x18, 0xA9, 0x01,
                        0x00, 0x60, 0x18, 0xFA, 0x18, 0xA9, 0x00, 0x00,
                        0x60, 0x0C, 0xE5, 0x23, 0xE7, 0xB0, 0xE1, 0x90,
                        0xE3, 0x93, 0xE0, 0xA3, 0xDF, 0x69, 0xE1, 0x1C,
                        0xE5, 0x2D, 0xE7, 0xC2, 0xE1, 0xA0, 0xE3, 0x9F,
                        0xE0, 0xB3, 0xDF, 0x7B
                    };
                    break;
            }

            byte[] hack3 =
            {
                0xE1, 0xDA, 0xAE, 0x24, 0x01, 0xE2, 0x20, 0xBF,
                0xA4, 0x5D, 0xC8, 0xF0, 0x0E, 0x8A, 0x0A, 0x0A,
                0x85, 0x00, 0xA5, 0x38, 0x29, 0x03, 0x18, 0x65,
                0x00, 0x80, 0x03, 0x8A, 0x0A, 0x0A, 0xAA, 0xBF,
                0x44, 0x5D, 0xC8, 0xC2, 0x20, 0x29, 0xFF, 0x00,
                0xEB, 0x85, 0x00, 0xFA, 0x60
            };

            saveBuffer.Add(hack1);
            saveBuffer.Add(hack2);
            saveBuffer.Add(hack3);
        }

        private void SaveTileGenres(SaveBuffer saveBuffer)
        {
            // Saves data from 0x85EFD to 0x86731

            // Apply a hack that gives a full 256-byte behavior table for each theme
            // in uncompressed form. It helps the load times a little bit,
            // and allows theme-specific tile genre values for shared tiles.
            // Also reimplement the theme-specific behavior of the Browser Castle jump bars
            // that slows you down, to make it reusable.

            /*
                LoadBehaviour: c85efd
                JumpBarCheck: c85f18
                behaviour tables: c85f2a
                jump bar table: c8672a
            */

            // JumpBarCheck offset (make it point to 85F18)
            int offset = this.offsets[Offset.JumpBarCheck];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0x18;
            this.romBuffer[offset++] = 0x5F;
            this.romBuffer[offset] = 0xC8;

            // LoadBehavior offset (make it point to 85EFD)
            offset = this.offsets[Offset.TileGenreLoad];
            this.romBuffer[offset++] = 0x5C;
            this.romBuffer[offset++] = 0xFD;
            this.romBuffer[offset++] = 0x5E;
            this.romBuffer[offset] = 0xC8;

            Region r = this.region;
            byte val1 = r == Region.Jap ? (byte)0x4E : r == Region.Euro ? (byte)0x39 : (byte)0x4A;
            byte val2 = r == Region.Jap ? (byte)0x9B : r == Region.Euro ? (byte)0xA9 : (byte)0xA4;
            byte[] data =
            {
                0xC2, 0x20, 0xAD, 0x26, 0x01, 0x4A, 0xEB, 0x18,
                0x69, 0x2A, 0x5F, 0xAA, 0xA0, 0x00, 0x0B, 0xA9,
                0xFF, 0x00, 0x8B, 0x54, 0x7E, 0xC8, 0xAB, 0x5C,
                val1, 0xEB, 0xC1, 0xDA, 0xAD, 0x26, 0x01, 0x4A,
                0xAA, 0xBF, 0x2A, 0x67, 0xC8, 0xFA, 0x29, 0xFF,
                0x00, 0x5C, val2, 0xB7, 0xC0
            };

            saveBuffer.Add(data);

            // "behavior tables" is 256 byte behavior tables for each theme.
            foreach (Theme theme in this.themes)
            {
                saveBuffer.Add(theme.RoadTileset.GetTileGenreBytes());
            }

            // "jump bar table" has 1 byte per theme.
            // If it is zero, then jump bars will slow you down.
            // If it is not zero, they act like they do outside of BC tracks.
            data = new byte[] { 1, 1, 1, 1, 1, 1, 0, 1 };

            saveBuffer.Add(data);
        }

        private void SaveAIs(SaveBuffer saveBuffer)
        {
            int aiFirstAddressByteOffset = this.offsets[Offset.TrackAIDataFirstAddressByte];
            this.romBuffer[aiFirstAddressByteOffset] = 0xC8;

            byte[] trackOrder = this.GetTrackOrder();

            for (int i = 0; i < this.trackGroups.Length; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();

                for (int j = 0; j < tracks.Length; j++)
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

            byte[] aiZoneOffset = Utilities.OffsetToBytes(saveBuffer.Index);
            byte[] aiTargetOffset = Utilities.OffsetToBytes(saveBuffer.Index + trackAIData.Length - track.AI.ElementCount * 3);

            this.romBuffer[trackAIZoneIndex] = aiZoneOffset[0];
            this.romBuffer[trackAIZoneIndex + 1] = aiZoneOffset[1];
            this.romBuffer[trackAITargetIndex] = aiTargetOffset[0];
            this.romBuffer[trackAITargetIndex + 1] = aiTargetOffset[1];

            saveBuffer.Add(trackAIData);
        }

        private void SaveTracks(SaveBuffer saveBuffer)
        {
            byte[] trackOrder = this.GetTrackOrder();
            int[] mapOffsets = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Offset.TrackMaps], Track.Count);

            for (int i = 0; i < this.trackGroups.Length; i++)
            {
                Track[] tracks = this.trackGroups[i].GetTracks();

                for (int j = 0; j < tracks.Length; j++)
                {
                    int iterator = i * GPTrack.CountPerGroup + j;
                    int trackIndex = trackOrder[iterator];

                    if (tracks[j].Modified)
                    {
                        this.SaveTrack(tracks[j], iterator, trackIndex, saveBuffer);
                    }
                    else
                    {
                        int mapOffset = mapOffsets[trackIndex];

                        if (saveBuffer.Includes(mapOffset))
                        {
                            this.MoveTrackMap(trackIndex, mapOffset, saveBuffer);
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
            this.SaveOverlayTileData(overlayTileData, trackIndex);

            GPTrack gpTrack = track as GPTrack;

            if (gpTrack != null)
            {
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

                // Update item probability index
                this.romBuffer[this.offsets[Offset.TrackItemProbabilityIndexes] + trackIndex] = (byte)(gpTrack.ItemProbabilityIndex << 1);
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
            byte[] compressedTrack = Codec.GetCompressedChunk(this.romBuffer, trackOffset);
            this.SaveTrackSub(trackIndex, compressedTrack, saveBuffer);
        }

        private void SaveTrackSub(int trackIndex, byte[] compressedTrack, SaveBuffer saveBuffer)
        {
            // Update track offset
            int trackOffsetIndex = this.offsets[Offset.TrackMaps] + trackIndex * 3;
            saveBuffer.AddCompressed(compressedTrack, trackOffsetIndex);
        }

        private void SaveThemes(SaveBuffer saveBuffer)
        {
            // In the original game, a road tileset is composed of 192 theme-specific tiles,
            // followed by 64 tiles that are shared across all themes.
            // Modify the game so that each theme has 256 unique tiles. Nothing is shared anymore.
            this.romBuffer[this.offsets[Offset.RoadTilesetHack1]] = 0x60;
            this.romBuffer[this.offsets[Offset.RoadTilesetHack2]] = 0x60;
            this.romBuffer[this.offsets[Offset.RoadTilesetHack3]] = 0x00;
            this.romBuffer[this.offsets[Offset.RoadTilesetHack4]] = 0x40;

            for (int i = 0; i < this.themes.Count; i++)
            {
                Theme theme = this.themes[i];
                this.SaveRoadTiles(theme, i, saveBuffer);
                this.SavePalettes(theme, i, saveBuffer);
                this.SaveBackgroundLayout(theme, i, saveBuffer);
                this.SaveBackgroundTiles(theme, i, saveBuffer);
            }
        }

        private void SaveRoadTiles(Theme theme, int themeIndex, SaveBuffer saveBuffer)
        {
            int roadTileGfxIndex = this.offsets[Offset.ThemeRoadGraphics] + themeIndex * 3;
            int roadTileGfxOffset = Utilities.BytesToOffset(this.romBuffer, roadTileGfxIndex);
            byte[] roadTileGfxData;

            if (!theme.RoadTileset.Modified && saveBuffer.Includes(roadTileGfxOffset))
            {
                // Do not recompress road tileset data (perf optimization),
                // simply copy the existing compressed data
                roadTileGfxData = Codec.GetCompressedChunk(this.romBuffer, roadTileGfxOffset);
            }
            else
            {
                // Recompress road tileset data
                roadTileGfxData = new byte[RoadTileset.TileCount + (RoadTileset.TileCount * 32)];

                for (int j = 0; j < RoadTileset.TileCount; j++)
                {
                    RoadTile tile = theme.RoadTileset[j];
                    roadTileGfxData[j] = (byte)(tile.Palette.Index << 4);
                    Buffer.BlockCopy(tile.Graphics, 0, roadTileGfxData, RoadTileset.TileCount + (j * 32), tile.Graphics.Length);
                }

                roadTileGfxData = Codec.Compress(roadTileGfxData);
            }

            saveBuffer.AddCompressed(roadTileGfxData, roadTileGfxIndex);
        }

        private void SavePalettes(Theme theme, int themeIndex, SaveBuffer saveBuffer)
        {
            int palettesIndex = this.offsets[Offset.ThemePalettes] + themeIndex * 3;
            int palettesOffset = Utilities.BytesToOffset(this.romBuffer, palettesIndex);

            if (theme.Palettes.Modified || saveBuffer.Includes(palettesOffset))
            {
                byte[] palettesData;

                if (!theme.Palettes.Modified)
                {
                    // Do not recompress palettes (perf optimization),
                    // simply copy the existing compressed data
                    palettesData = Codec.GetCompressedChunk(this.romBuffer, palettesOffset);
                }
                else
                {
                    // Recompress palettes
                    palettesData = Codec.Compress(theme.Palettes.GetBytes());
                }

                saveBuffer.AddCompressed(palettesData, palettesIndex);
            }
        }

        private void SaveBackgroundLayout(Theme theme, int themeIndex, SaveBuffer saveBuffer)
        {
            int bgLayoutIndex = this.offsets[Offset.ThemeBackgroundLayouts] + themeIndex * 3;
            int bgLayoutOffset = Utilities.BytesToOffset(this.romBuffer, bgLayoutIndex);

            if (theme.Background.Layout.Modified || saveBuffer.Includes(bgLayoutOffset))
            {
                byte[] bgLayoutData;

                if (!theme.Background.Layout.Modified)
                {
                    // Do not recompress background layout (perf optimization),
                    // simply copy the existing compressed data
                    bgLayoutData = Codec.GetCompressedChunk(this.romBuffer, bgLayoutOffset);
                }
                else
                {
                    // Recompress background layout
                    bgLayoutData = Codec.Compress(theme.Background.Layout.GetBytes());
                }

                saveBuffer.AddCompressed(bgLayoutData, bgLayoutIndex);
            }
        }

        private void SaveBackgroundTiles(Theme theme, int themeIndex, SaveBuffer saveBuffer)
        {
            int bgTileGfxIndex = this.offsets[Offset.ThemeBackgroundGraphics] + themeIndex * 3;
            int bgTileGfxOffset = Utilities.BytesToOffset(this.romBuffer, bgTileGfxIndex);

            if (theme.Background.Tileset.Modified || saveBuffer.Includes(bgTileGfxOffset))
            {
                byte[] bgTileGfxData;

                if (!theme.Background.Tileset.Modified)
                {
                    // Do not recompress background tileset graphics (perf optimization),
                    // simply copy the existing compressed data
                    bgTileGfxData = Codec.GetCompressedChunk(this.romBuffer, bgTileGfxOffset);
                }
                else
                {
                    // Recompress background tileset graphics
                    bgTileGfxData = new byte[BackgroundTileset.TileCount * 16];

                    for (int j = 0; j < BackgroundTileset.TileCount; j++)
                    {
                        BackgroundTile tile = theme.Background.Tileset[j];
                        Buffer.BlockCopy(tile.Graphics, 0, bgTileGfxData, j * 16, tile.Graphics.Length);
                    }

                    bgTileGfxData = Codec.Compress(bgTileGfxData);
                }

                saveBuffer.AddCompressed(bgTileGfxData, bgTileGfxIndex);
            }
        }

        private void SaveSettings()
        {
            this.SaveItemProbabilities();
            this.SaveRankPoints();
        }

        private void SaveItemProbabilities()
        {
            if (this.Settings.ItemProbabilities.Modified)
            {
                byte[] data = this.Settings.ItemProbabilities.GetBytes();
                Buffer.BlockCopy(data, 0, this.romBuffer, this.offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            }
        }

        private void SaveRankPoints()
        {
            if (this.Settings.RankPoints.Modified)
            {
                byte[] data = this.Settings.RankPoints.GetBytes();
                Buffer.BlockCopy(data, 0, this.romBuffer, this.offsets[Offset.RankPoints], RankPoints.Size);
            }
        }

        private void SaveFile()
        {
            using (FileStream fs = new FileStream(this.filePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(this.romHeader);
                bw.Write(this.romBuffer);
            }
        }

        private void ResetModifiedFlags()
        {
            this.modified = false;
            this.themes.ResetModifiedFlags();
            this.Settings.ResetModifiedFlags();

            foreach (TrackGroup trackGroup in this.trackGroups)
            {
                trackGroup.ResetModifiedFlags();
            }
        }

        public bool HasPendingChanges()
        {
            if (this.modified ||
                this.themes.Modified ||
                this.Settings.Modified)
            {
                return true;
            }

            foreach (TrackGroup trackGroup in this.trackGroups)
            {
                if (trackGroup.Modified)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion Save data

        public void Dispose()
        {
            this.themes.Dispose();
            this.ObjectGraphics.Dispose();
            this.ItemIconGraphics.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
