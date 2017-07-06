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
using EpicEdit.Rom.Utility;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;

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
    internal sealed class Game : INotifyPropertyChanged, IDisposable
    {
        #region Constants

        private const int RegionOffset = 0xFFD9;

        #endregion Constants

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<EventArgs> TracksReordered;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the track groups, each of which contains several tracks.
        /// </summary>
        public TrackGroups TrackGroups { get; private set; }

        /// <summary>
        /// Gets the track themes.
        /// </summary>
        public Themes Themes { get; private set; }

        /// <summary>
        /// Gets the overlay tile sizes.
        /// </summary>
        public OverlayTileSizes OverlayTileSizes { get; private set; }

        /// <summary>
        /// Gets the overlay tile patterns.
        /// </summary>
        public OverlayTilePatterns OverlayTilePatterns { get; private set; }

        /// <summary>
        /// Gets the path to the loaded ROM file.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets the file name of the loaded ROM.
        /// </summary>
        public string FileName
        {
            get { return Path.GetFileName(this.FilePath); }
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

        public int HeaderSize
        {
            get { return this.romHeader.Length; }
        }

        public bool Modified { get; private set; }

        #endregion Public properties

        #region Private members

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

        #endregion Private members

        #region Constructor

        /// <param name="filePath">The path to the ROM file.</param>
        public Game(string filePath)
        {
            this.FilePath = filePath;
            this.LoadRom();
            this.ValidateRom();
            this.LoadData();
            this.HandleChanges();
        }

        #endregion Constructor

        #region Read ROM & validate

        /// <summary>
        /// Loads all of the ROM content in a buffer.
        /// </summary>
        private void LoadRom()
        {
            if (!this.FilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
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
            this.romBuffer = File.ReadAllBytes(this.FilePath);
        }

        private void LoadZippedRom()
        {
            try
            {
                using (FileStream fs = new FileStream(this.FilePath, FileMode.Open, FileAccess.Read))
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
                            using (ZipFile zf = new ZipFile(this.FilePath))
                            using (InflaterInputStream iis = (InflaterInputStream)zf.GetInputStream(entry))
                            {
                                this.romBuffer = new byte[entry.Size];
                                iis.Read(this.romBuffer, 0, romBuffer.Length);

                                // Update the file path so that it includes the name of the ROM file
                                // inside the zip, rather than the name of the zip archive itself
                                this.FilePath =
                                    this.FilePath.Substring(0, this.FilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1) + entry.Name;

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
                throw new InvalidDataException($"\"{this.FileName}\" is not an SNES ROM.");
            }

            if (!this.IsSuperMarioKart())
            {
                throw new InvalidDataException($"\"{this.FileName}\" is not a Super Mario Kart ROM.");
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
            Codec.SetRegion(this.region);
            this.offsets = new Offsets(this.romBuffer, this.region);

            this.Settings = new GameSettings(this.romBuffer, this.offsets, this.region);
            this.TrackGroups = new TrackGroups();

            this.Themes = new Themes(this.romBuffer, this.offsets, this.Settings.CupAndThemeTexts);
            byte[] overlayTileSizesData = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackOverlaySizes], OverlayTileSizes.Size);
            this.OverlayTileSizes = new OverlayTileSizes(overlayTileSizesData);
            this.OverlayTilePatterns = new OverlayTilePatterns(this.romBuffer, this.offsets, this.OverlayTileSizes);

            byte[] trackThemes = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackThemes], Track.Count);
            byte[] trackOrder = this.GetTrackOrder();
            byte[][] cupNameIndexes = this.GetCupNameIndexes();
            byte[][] trackNameIndexes = this.GetTrackNameIndexes();

            int[] mapOffsets = Utilities.ReadBlockOffset(this.romBuffer, this.offsets[Offset.TrackMaps], Track.Count);

            byte aiOffsetBase = this.romBuffer[this.offsets[Offset.TrackAIDataFirstAddressByte]];
            byte[] aiZoneOffsets = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackAIZones], Track.Count * 2); // 2 offset bytes per track
            byte[] aiTargetOffsets = Utilities.ReadBlock(this.romBuffer, this.offsets[Offset.TrackAITargets], Track.Count * 2); // 2 offset bytes per track

            for (int i = 0; i < this.TrackGroups.Count; i++)
            {
                int trackCountInGroup;
                SuffixedTextItem trackGroupNameItem;
                if (i != this.TrackGroups.Count - 1) // GP track group
                {
                    trackCountInGroup = GPTrack.CountPerGroup;
                    TextItem trackGroupTextItem = this.Settings.CupAndThemeTexts[cupNameIndexes[i][1]];
                    byte[] trackGroupNameSuffixData = Utilities.ReadBlock(cupNameIndexes[i], 2, cupNameIndexes[i].Length - 2);
                    string trackGroupNameSuffix = trackGroupTextItem.Converter.DecodeText(trackGroupNameSuffixData, false);
                    trackGroupNameItem = new SuffixedTextItem(trackGroupTextItem, trackGroupNameSuffix, this.Settings.CupAndTrackNameSuffixCollection);
                }
                else // Battle track group
                {
                    trackCountInGroup = BattleTrack.Count;
                    TextItem trackGroupTextItem = this.Settings.CupAndThemeTexts[trackNameIndexes[GPTrack.Count][1]];

                    // NOTE: The "Battle Course" track group doesn't actually exist in the game.
                    // It's only created in the editor to have a logical group that contains the Battle Courses.
                    trackGroupNameItem = new SuffixedTextItem(trackGroupTextItem, null, null);
                }

                Track[] tracks = new Track[trackCountInGroup];

                for (int j = 0; j < trackCountInGroup; j++)
                {
                    int iterator = i * GPTrack.CountPerGroup + j;
                    int trackIndex = trackOrder[iterator];

                    TextItem trackNameItem = this.Settings.CupAndThemeTexts[trackNameIndexes[iterator][1]];
                    byte[] trackNameSuffixData = Utilities.ReadBlock(trackNameIndexes[iterator], 2, trackNameIndexes[iterator].Length - 2);
                    string trackNameSuffix = trackNameItem.Converter.DecodeText(trackNameSuffixData, false);
                    SuffixedTextItem suffixedTrackNameItem = new SuffixedTextItem(trackNameItem, trackNameSuffix, this.Settings.CupAndTrackNameSuffixCollection);

                    int themeId = trackThemes[trackIndex] >> 1;
                    Theme trackTheme = this.Themes[themeId];

                    byte[] trackMap = Codec.Decompress(Codec.Decompress(this.romBuffer, mapOffsets[trackIndex]), 0, TrackMap.SquareSize);

                    byte[] overlayTileData = this.GetOverlayTileData(trackIndex);

                    this.LoadAIData(trackIndex, aiOffsetBase, aiZoneOffsets, aiTargetOffsets, out byte[] aiZoneData, out byte[] aiTargetData);

                    if (trackIndex < GPTrack.Count) // GP track
                    {
                        byte[] startPositionData = this.GetGPStartPositionData(trackIndex);
                        byte[] lapLineData = this.GetLapLineData(trackIndex);
                        byte[] objectData = this.GetObjectData(trackIndex);
                        byte[] objectZoneData = this.GetObjectZoneData(trackIndex);
                        byte[] objectPropData = this.GetObjectPropertiesData(trackIndex, themeId);
                        int itemProbaIndex = this.romBuffer[this.offsets[Offset.TrackItemProbabilityIndexes] + trackIndex] >> 1;

                        tracks[j] = new GPTrack(suffixedTrackNameItem, trackTheme,
                                                trackMap, overlayTileData,
                                                aiZoneData, aiTargetData,
                                                startPositionData, lapLineData,
                                                objectData, objectZoneData, objectPropData,
                                                this.OverlayTileSizes,
                                                this.OverlayTilePatterns,
                                                itemProbaIndex);
                    }
                    else // Battle track
                    {
                        byte[] startPositionData = this.GetBattleStartPositionData(trackIndex);

                        tracks[j] = new BattleTrack(suffixedTrackNameItem, trackTheme,
                                                    trackMap, overlayTileData,
                                                    aiZoneData, aiTargetData,
                                                    startPositionData,
                                                    this.OverlayTileSizes,
                                                    this.OverlayTilePatterns);
                    }
                }

                this.TrackGroups[i] = new TrackGroup(trackGroupNameItem, tracks);
            }

            this.ObjectGraphics = new TrackObjectGraphics(this.romBuffer, this.offsets);
            this.ItemIconGraphics = new ItemIconGraphics(this.romBuffer, this.offsets);
        }

        private void SetRegion()
        {
            int region = this.romBuffer[Game.RegionOffset];

            if (!Enum.IsDefined(typeof(Region), region))
            {
                throw new InvalidDataException($"\"{this.FileName}\" has an invalid region. Value at {(Game.RegionOffset + this.romHeader.Length):X} must be 0, 1 or 2, was: {region:X}.");
            }

            this.region = (Region)region;
        }

        public static Region GetRegion(byte[] romBuffer)
        {
            return (Region)romBuffer[Game.RegionOffset];
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

        private byte[][] GetCupNameIndexes()
        {
            byte[][] cupNameIndexes = Utilities.ReadBlockGroup(this.romBuffer, this.offsets[Offset.CupNames] + 2, 4, GPTrack.GroupCount);

            for (int i = 0; i < cupNameIndexes.Length; i++)
            {
                int offset = Utilities.BytesToOffset(cupNameIndexes[i][0], cupNameIndexes[i][1], 1);
                cupNameIndexes[i] = Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF);
                cupNameIndexes[i][1] = (byte)(cupNameIndexes[i][1] & 0xF);
            }

            return cupNameIndexes;
        }

        private byte[][] GetTrackNameIndexes()
        {
            int gpTrackNamesOffset = this.offsets[Offset.GPTrackNames];

            byte[][] gpTrackPointers = new byte[GPTrack.Count][];

            for (int i = 0; i < GPTrack.GroupCount; i++)
            {
                gpTrackNamesOffset += 2; // Skip leading bytes
                Utilities.ReadBlockGroup(this.romBuffer, gpTrackNamesOffset, 4, GPTrack.CountPerGroup).CopyTo(gpTrackPointers, GPTrack.CountPerGroup * i);
                gpTrackNamesOffset += GPTrack.CountPerGroup * GPTrack.GroupCount;
            }

            byte[][] battleTrackPointers = Utilities.ReadBlockGroup(this.romBuffer, this.offsets[Offset.BattleTrackNames] + 2, 4, BattleTrack.Count);
            byte[][] trackNameIndexes = new byte[Track.Count][];

            for (int i = 0; i < gpTrackPointers.Length; i++)
            {
                int offset = Utilities.BytesToOffset(gpTrackPointers[i][0], gpTrackPointers[i][1], 1);
                trackNameIndexes[i] = Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF);
            }

            for (int i = 0; i < battleTrackPointers.Length; i++)
            {
                int offset = Utilities.BytesToOffset(battleTrackPointers[i][0], battleTrackPointers[i][1], 1);
                trackNameIndexes[gpTrackPointers.Length + i] = Utilities.ReadBlockUntil(this.romBuffer, offset, 0xFF);
            }

            for (int i = 0; i < trackNameIndexes.Length; i++)
            {
                trackNameIndexes[i][1] = (byte)(trackNameIndexes[i][1] & 0xF);
            }

            return trackNameIndexes;
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

        private byte[] GetObjectPropertiesData(int trackIndex, int themeId)
        {
            byte[] data = new byte[8];
            byte[] paletteIndexes;

            if (this.ObjectZonesRelocated)
            {
                int offset = this.offsets[Offset.TrackObjectProperties] + trackIndex;
                data[0] = this.romBuffer[offset];
                data[1] = this.romBuffer[offset + Track.Count];
                data[2] = this.romBuffer[offset + Track.Count * 2];
                data[7] = this.romBuffer[this.offsets[Offset.TrackObjectFlashing] + trackIndex];
                paletteIndexes = this.GetObjectPaletteIndexes(trackIndex);
            }
            else
            {
                byte objectType = (byte)Game.GetObjectType(themeId, trackIndex);
                data[0] = objectType;
                data[1] = objectType;
                data[2] = objectType;
                data[7] = themeId == 7 ? (byte)1 : (byte)0; // Rainbow Road
                paletteIndexes = Game.GetObjectPaletteIndexes(themeId, trackIndex);
            }

            data[3] = paletteIndexes[0];
            data[4] = paletteIndexes[1];
            data[5] = paletteIndexes[2];
            data[6] = paletteIndexes[3];

            return data;
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

                    default: throw new ArgumentOutOfRangeException(nameof(themeId));
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

                    default: throw new ArgumentOutOfRangeException(nameof(themeId));
                }
            }

            return paletteIndexes;
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

        #region Track reordering
        public void ReorderTracks(int sourceTrackGroupId, int sourceTrackId, int destinationTrackGroupId, int destinationTrackId)
        {
            if (sourceTrackGroupId == destinationTrackGroupId &&
                sourceTrackId == destinationTrackId)
            {
                return;
            }

            // TODO: This method is complex and could be simplified a lot.
            // At the moment, it reorders tracks and updates the ROM data to reflect the reordering.
            // Instead, it could only reorder the track objects, and let the SaveRom method update all the data in the ROM.
            // This would also allow us to move this method to the TrackGroups class.

            if (sourceTrackGroupId < GPTrack.GroupCount) // GP track reordering
            {
                this.ReorderGPTracks(sourceTrackGroupId, sourceTrackId, destinationTrackGroupId, destinationTrackId);

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
            }
            else // Battle track reordering
            {
                this.ReorderBattleTracks(sourceTrackId, destinationTrackId);

                #region Battle track specific data update
                int trackOrderOffset = this.offsets[Offset.BattleTrackOrder];

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

            this.MarkAsModified();

            this.TracksReordered?.Invoke(this, EventArgs.Empty);
        }

        private void ReorderGPTracks(int sourceTrackGroupId, int sourceTrackId, int destinationTrackGroupId, int destinationTrackId)
        {
            #region Global track array creation
            // To make the treatment easier, we simply create an array with all the GP tracks
            Track[] tracks = new Track[GPTrack.Count];
            for (int i = 0; i < this.TrackGroups.Count - 1; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    tracks[i * trackGroup.Count + j] = trackGroup[j];
                }
            }

            sourceTrackId = sourceTrackGroupId * GPTrack.CountPerGroup + sourceTrackId;
            destinationTrackId = destinationTrackGroupId * GPTrack.CountPerGroup + destinationTrackId;
            #endregion Global track array creation

            this.ReorderTracks(tracks, sourceTrackId, destinationTrackId, this.offsets[Offset.GPTrackOrder]);

            #region Update track pointers in track groups
            for (int i = 0; i < this.TrackGroups.Count - 1; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    trackGroup[j] = tracks[i * trackGroup.Count + j];
                }
            }
            #endregion Update track pointers in track groups
        }

        private void ReorderBattleTracks(int sourceTrackId, int destinationTrackId)
        {
            #region Track array creation
            Track[] tracks = new Track[BattleTrack.Count];

            for (int i = 0; i < tracks.Length; i++)
            {
                tracks[i] = this.TrackGroups[GPTrack.GroupCount][i];
            }
            #endregion Track array creation

            this.ReorderTracks(tracks, sourceTrackId, destinationTrackId, this.offsets[Offset.BattleTrackOrder]);

            #region Update track pointers in track groups
            TrackGroup trackGroup = this.TrackGroups[GPTrack.GroupCount];

            for (int i = 0; i < trackGroup.Count; i++)
            {
                trackGroup[i] = tracks[i];
            }
            #endregion Update track pointers in track groups
        }

        private void ReorderTracks(Track[] tracks, int sourceTrackId, int destinationTrackId, int trackOrderOffset)
        {
            Track sourceTrack = tracks[sourceTrackId];
            byte sourceTrackOrder = this.romBuffer[trackOrderOffset + sourceTrackId];

            if (sourceTrackId < destinationTrackId)
            {
                for (int i = sourceTrackId; i < destinationTrackId; i++)
                {
                    this.RemapTrack(tracks, i + 1, i, trackOrderOffset);
                }
            }
            else
            {
                for (int i = sourceTrackId; i > destinationTrackId; i--)
                {
                    this.RemapTrack(tracks, i - 1, i, trackOrderOffset);
                }
            }

            tracks[destinationTrackId] = sourceTrack;
            this.romBuffer[trackOrderOffset + destinationTrackId] = sourceTrackOrder;
        }

        private void RemapTrack(Track[] tracks, int sourceTrackId, int destinationTrackId, int trackOrderOffset)
        {
            tracks[destinationTrackId] = tracks[sourceTrackId];
            this.romBuffer[trackOrderOffset + destinationTrackId] = this.romBuffer[trackOrderOffset + sourceTrackId];
        }
        #endregion Track reordering

        #region Save data
        public void SaveRom()
        {
            this.SaveRom(this.FilePath);
        }

        public void SaveRom(string filePath)
        {
            this.FilePath = filePath;

            this.SaveDataToBuffer();
            this.SetChecksum();
            this.SaveFile();
            this.ResetModifiedState();
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

            this.Settings.Save(this.romBuffer);
            this.SaveCupAndTrackNames();
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

            TrackGroup trackGroup = this.TrackGroups[GPTrack.GroupCount];

            for (int i = 0; i < trackGroup.Count; i++)
            {
                int iterator = GPTrack.Count + i;
                int trackIndex = trackOrder[iterator];
                int bTrackIndex = trackIndex - GPTrack.Count;

                this.SaveBattleStartPositions(trackGroup[bTrackIndex] as BattleTrack, saveBuffer);
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

            for (int i = 0; i < this.TrackGroups.Count - 1; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    GPTrack gpTrack = trackGroup[j] as GPTrack;

                    tilesetData[trackIndex] = (byte)gpTrack.Objects.Tileset;
                    interactData[trackIndex] = (byte)gpTrack.Objects.Interaction;
                    routineData[trackIndex] = (byte)gpTrack.Objects.Routine;
                    zData[trackIndex] = routineData[trackIndex];
                    loadingData[trackIndex] = (byte)gpTrack.Objects.Loading;
                }
            }

            // Mark battle tracks as not having objects
            const byte NoObject = (byte)ObjectLoading.None;
            loadingData[GPTrack.Count] = NoObject;
            loadingData[GPTrack.Count + 1] = NoObject;
            loadingData[GPTrack.Count + 2] = NoObject;
            loadingData[GPTrack.Count + 3] = NoObject;

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

            for (int i = 0; i < this.TrackGroups.Count - 1; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    GPTrack gpTrack = trackGroup[j] as GPTrack;

                    // Update object zones
                    byte[] data = gpTrack.Objects.Zones.GetBytes();
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
            const int FlashingOffset = Track.Count * 4;

            for (int i = 0; i < this.TrackGroups.Count - 1; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    int offset = trackIndex * 4;
                    GPTrack gpTrack = trackGroup[j] as GPTrack;

                    objectPalData[offset++] = (byte)(gpTrack.Objects.PaletteIndexes[0] << 1);
                    objectPalData[offset++] = (byte)(gpTrack.Objects.PaletteIndexes[1] << 1);
                    objectPalData[offset++] = (byte)(gpTrack.Objects.PaletteIndexes[2] << 1);
                    objectPalData[offset] = (byte)(gpTrack.Objects.PaletteIndexes[3] << 1);

                    objectPalData[FlashingOffset + trackIndex] =
                        gpTrack.Objects.Flashing ? (byte)1 : (byte)0;
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
            foreach (Theme theme in this.Themes)
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

            for (int i = 0; i < this.TrackGroups.Count; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    int trackIndex = trackOrder[i * GPTrack.CountPerGroup + j];
                    this.SaveAI(trackGroup[j], trackIndex, saveBuffer);
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

            for (int i = 0; i < this.TrackGroups.Count; i++)
            {
                TrackGroup trackGroup = this.TrackGroups[i];

                for (int j = 0; j < trackGroup.Count; j++)
                {
                    int iterator = i * GPTrack.CountPerGroup + j;
                    int trackIndex = trackOrder[iterator];

                    if (trackGroup[j].Modified)
                    {
                        this.SaveTrack(trackGroup[j], iterator, trackIndex, saveBuffer);
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
            // Update track map
            byte[] compressedMap = Codec.Compress(track.Map.GetBytes(), true, false);
            this.SaveTrackMap(trackIndex, compressedMap, saveBuffer);

            // Update track theme id
            byte themeId = this.Themes.GetThemeId(track.Theme);
            int themeIdOffset = this.offsets[Offset.TrackThemes] + trackIndex;
            this.romBuffer[themeIdOffset] = themeId;

            // Update overlay tiles
            byte[] overlayTileData = track.OverlayTiles.GetBytes();
            this.SaveOverlayTileData(overlayTileData, trackIndex);

            if (track is GPTrack gpTrack)
            {
                // Update driver starting position
                this.SaveGPStartPositionData(gpTrack, trackIndex);

                // Update lap line position and length
                byte[] lapLineData = gpTrack.LapLine.GetBytes();
                Buffer.BlockCopy(lapLineData, 0, this.romBuffer, this.offsets[Offset.TrackLapLines] + trackIndex * lapLineData.Length, lapLineData.Length);

                // Update lap line position on track preview
                int previewLapLineOffset = offsets[Offset.TrackPreviewLapLines] + iterator * 2;
                Point previewLapLineLocation = Game.GetPreviewLapLineLocation(gpTrack);
                this.romBuffer[previewLapLineOffset] = (byte)previewLapLineLocation.X;
                this.romBuffer[previewLapLineOffset + 1] = (byte)previewLapLineLocation.Y;

                // Update item probability index
                this.romBuffer[this.offsets[Offset.TrackItemProbabilityIndexes] + trackIndex] = (byte)(gpTrack.ItemProbabilityIndex << 1);
            }
        }

        private static Point GetPreviewLapLineLocation(GPTrack track)
        {
            // Track coordinates:
            const int XTopLeft = 40; // Top-left X value
            const int XBottomLeft = 6; // Bottom-left X value
            const int XBottomRight = 235; // Bottom-right X value
            const int YTop = 16; // Top value
            const int YBottom = 104; // Bottom value

            float yRelative = (1023 - track.LapLine.Y) * (XBottomRight - XBottomLeft) / 1023;
            int y = (int)(YBottom - (yRelative * Math.Sin(0.389)) - 7);

            float xPercent = (float)(track.StartPosition.X + track.StartPosition.SecondRowOffset / 2) / 1023;
            float yPercent = (float)(y - YTop) / (YBottom - YTop);
            int xStart = (int)(XTopLeft - (XTopLeft - XBottomLeft) * yPercent);
            int mapWidth = XBottomRight - (xStart - XBottomLeft) * 2;
            int x = (int)(xStart + mapWidth * xPercent);
            if (x < (XBottomRight - XBottomLeft) / 2)
            {
                // If the lap line is on the left side, shift its position a bit
                x -= 5;
            }

            return new Point(x, y);
        }

        private void MoveTrackMap(int trackIndex, int trackOffset, SaveBuffer saveBuffer)
        {
            byte[] compressedMap = Codec.GetCompressedChunk(this.romBuffer, trackOffset);
            this.SaveTrackMap(trackIndex, compressedMap, saveBuffer);
        }

        private void SaveTrackMap(int trackIndex, byte[] compressedMap, SaveBuffer saveBuffer)
        {
            // Update track map offset
            int mapOffsetIndex = this.offsets[Offset.TrackMaps] + trackIndex * 3;
            saveBuffer.AddCompressed(compressedMap, mapOffsetIndex);
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

            for (int i = 0; i < this.Themes.Count; i++)
            {
                Theme theme = this.Themes[i];
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

                Buffer.BlockCopy(theme.RoadTileset.GetTilePaletteBytes(), 0, roadTileGfxData, 0, RoadTileset.TileCount);

                for (int j = 0; j < RoadTileset.TileCount; j++)
                {
                    RoadTile tile = theme.RoadTileset[j];
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

                if (themeIndex == 0)
                {
                    // Update the Ghost Valley animated graphics source
                    // to point to the new Ghost Valley background tileset location
                    int index = this.offsets[Offset.GhostValleyBackgroundAnimationGraphics];
                    this.romBuffer[index + 3] = this.romBuffer[bgTileGfxIndex];
                    this.romBuffer[index + 4] = this.romBuffer[bgTileGfxIndex + 1];
                    this.romBuffer[index] = this.romBuffer[bgTileGfxIndex + 2];
                }
            }
        }

        private void SaveCupAndTrackNames()
        {
            int nameOffset = this.offsets[Offset.NamesAndSuffixes];

            // Update battle track names
            int battleTrackNameOffsetIndex = this.offsets[Offset.BattleTrackNames] + 2; // Skip leading bytes
            foreach (Track track in this.TrackGroups[GPTrack.GroupCount])
            {
                this.SaveCupOrTrackName(track.SuffixedNameItem, battleTrackNameOffsetIndex, ref nameOffset);
                battleTrackNameOffsetIndex += 4;
            }

            // Update cup names
            int allCupNameOffsetIndex = this.offsets[Offset.CupNames] + 2; // Skip leading bytes
            int lockedCupNameOffsetIndex = this.offsets[Offset.CupNamesLocked] + 2; // Skip leading bytes
            for (int i = 0; i < GPTrack.GroupCount; i++)
            {
                // Update cup name + name index (including Special Cup)
                this.SaveCupOrTrackName(this.TrackGroups[i].SuffixedNameItem, allCupNameOffsetIndex, ref nameOffset);

                if (i < GPTrack.GroupCount - 1)
                {
                    // Update cup name index (excluding Special Cup)
                    this.romBuffer[lockedCupNameOffsetIndex] = this.romBuffer[allCupNameOffsetIndex];
                    this.romBuffer[lockedCupNameOffsetIndex + 1] = this.romBuffer[allCupNameOffsetIndex + 1];
                    lockedCupNameOffsetIndex += 4;
                }

                allCupNameOffsetIndex += 4;
            }

            // Update GP track names
            int gpTrackNameOffsetIndex = this.offsets[Offset.GPTrackNames];
            for (int i = 0; i < GPTrack.GroupCount; i++)
            {
                gpTrackNameOffsetIndex += 2; // Skip leading bytes

                foreach (var track in this.TrackGroups[i])
                {
                    this.SaveCupOrTrackName(track.SuffixedNameItem, gpTrackNameOffsetIndex, ref nameOffset);
                    gpTrackNameOffsetIndex += 4;
                }
            }
        }

        private void SaveCupOrTrackName(SuffixedTextItem nameItem, int nameOffsetIndex, ref int nameOffset)
        {
            byte[] offsetAddressData = Utilities.OffsetToBytes(nameOffset);
            this.romBuffer[nameOffsetIndex] = offsetAddressData[0];
            this.romBuffer[nameOffsetIndex + 1] = offsetAddressData[1];

            this.romBuffer[nameOffset++] = 0x29;
            this.romBuffer[nameOffset++] = (byte)(0xE0 + this.Settings.CupAndThemeTexts.IndexOf(nameItem.TextItem));

            byte[] nameSuffixData = nameItem.TextItem.Converter.EncodeText(nameItem.Suffix.Value, null);

            for (int i = 0; i < nameSuffixData.Length; i++)
            {
                this.romBuffer[nameOffset++] = nameSuffixData[i];
            }

            this.romBuffer[nameOffset++] = 0xFF;
        }

        private void SaveFile()
        {
            using (FileStream fs = new FileStream(this.FilePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(this.romHeader);
                bw.Write(this.romBuffer);
            }
        }

        private void HandleChanges()
        {
            this.TrackGroups.PropertyChanged += this.MarkAsModified;
            this.Themes.PropertyChanged += this.MarkAsModified;
            this.Settings.PropertyChanged += this.MarkAsModified;
        }

        private void MarkAsModified()
        {
            // NOTE: Dummy property name
            this.MarkAsModified(this, new PropertyChangedEventArgs(PropertyNames.Game.Data));
        }

        private void MarkAsModified(object sender, PropertyChangedEventArgs e)
        {
            this.Modified = true;
            this.OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(sender, e);
        }

        private void ResetModifiedState()
        {
            this.TrackGroups.ResetModifiedState();
            this.Themes.ResetModifiedState();
            this.Settings.ResetModifiedState();

            this.Modified = false;
            this.OnPropertyChanged(PropertyNames.Game.Modified);
        }
        #endregion Save data

        #region Compression

        public void InsertData(byte[] data, int offset)
        {
            offset -= this.romHeader.Length;
            Buffer.BlockCopy(data, 0, this.romBuffer, offset, data.Length);
            this.MarkAsModified();
        }

        public byte[] Decompress(int offset, bool twice)
        {
            offset -= this.romHeader.Length;
            return Codec.Decompress(this.romBuffer, offset, twice);
        }

        public int GetCompressedChunkLength(int offset)
        {
            offset -= this.romHeader.Length;
            return Codec.GetCompressedLength(this.romBuffer, offset);
        }

        #endregion Compression

        #region IDisposable

        public void Dispose()
        {
            this.Themes.Dispose();
            this.ObjectGraphics.Dispose();
            this.ItemIconGraphics.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
