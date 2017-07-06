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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Utility;
using System;

namespace EpicEdit.Rom
{
    internal enum Offset
    {
        /// <summary>
        /// Offsets to the names of the modes on the title screen.
        /// </summary>
        ModeNames,

        /// <summary>
        /// Offsets to the cup texts as displayed on the GP cup selection screen.
        /// </summary>
        GPCupSelectTexts,

        /// <summary>
        /// Offsets to the cup texts displayed on the GP podium screen.
        /// </summary>
        GPPodiumCupTexts,

        /// <summary>
        /// Offsets to the cup and theme texts as displayed in Time Trial, Match Race and Battle Mode.
        /// </summary>
        CupAndThemeTexts,

        /// <summary>
        /// Offsets to the driver names on the GP results screen.
        /// </summary>
        DriverNamesGPResults,

        /// <summary>
        /// Offsets to the driver names on the GP podium screen.
        /// </summary>
        DriverNamesGPPodium,

        /// <summary>
        /// Offsets to the driver names in Time Trial.
        /// </summary>
        DriverNamesTimeTrial,

        /// <summary>
        /// Track map address index.
        /// </summary>
        TrackMaps,

        /// <summary>
        /// Track theme index.
        /// </summary>
        TrackThemes,

        /// <summary>
        /// Cup name index, with Special Cup locked (3 cups).
        /// </summary>
        CupNamesLocked,

        /// <summary>
        /// Cup name index, with Special Cup unlocked (4 cups).
        /// </summary>
        CupNames,

        /// <summary>
        /// Address to the start of the name references followed by a suffix.
        /// Only used for resaving, to define where to start saving this data.
        /// It's safer than deducing it from the game name addresses, which might have been tampered with.
        /// </summary>
        NamesAndSuffixes,

        /// <summary>
        /// GP track order index.
        /// </summary>
        GPTrackOrder,

        /// <summary>
        /// GP track name index.
        /// </summary>
        GPTrackNames,

        /// <summary>
        /// Battle track order index.
        /// </summary>
        BattleTrackOrder,

        /// <summary>
        /// Battle track name index.
        /// </summary>
        BattleTrackNames,

        /// <summary>
        /// Index of the first battle track (track displayed by default when entering the battle track selection).
        /// </summary>
        FirstBattleTrack,

        /// <summary>
        /// Track object graphics address index.
        /// </summary>
        TrackObjectGraphics,

        /// <summary>
        /// Match Race object graphics address.
        /// </summary>
        MatchRaceObjectGraphics,

        /// <summary>
        /// Theme background graphics address index.
        /// </summary>
        ThemeBackgroundGraphics,

        /// <summary>
        /// Theme background layout address index.
        /// </summary>
        ThemeBackgroundLayouts,

        /// <summary>
        /// Ghost Valley background animation graphics address index.
        /// </summary>
        GhostValleyBackgroundAnimationGraphics,

        /// <summary>
        /// The leading byte that composes AI-related addresses.
        /// </summary>
        TrackAIDataFirstAddressByte,

        /// <summary>
        /// AI zone index.
        /// </summary>
        TrackAIZones,

        /// <summary>
        /// AI target index.
        /// </summary>
        TrackAITargets,

        /// <summary>
        /// Track objects.
        /// </summary>
        TrackObjects,

        /// <summary>
        /// Track object zones.
        /// </summary>
        TrackObjectZones,

        /// <summary>
        /// Track overlay items.
        /// </summary>
        TrackOverlayItems,

        /// <summary>
        /// Track overlay sizes.
        /// </summary>
        TrackOverlaySizes,

        /// <summary>
        /// Track overlay pattern addresses.
        /// </summary>
        TrackOverlayPatterns,

        /// <summary>
        /// Starting position of the drivers on the GP tracks.
        /// </summary>
        GPTrackStartPositions,

        /// <summary>
        /// Track lap lines.
        /// </summary>
        TrackLapLines,

        /// <summary>
        /// Position of the track lap lines shown in track previews (Match Race / Time Trial).
        /// </summary>
        TrackPreviewLapLines,

        /// <summary>
        /// Addresses to the starting position of the drivers on the battle tracks.
        /// Used to retrieve the positions in the original game.
        /// </summary>
        BattleTrackStartPositions,

        /// <summary>
        /// Address to the starting position of the drivers on the battle tracks.
        /// Used to relocate the starting positions.
        /// </summary>
        BattleTrackStartPositionsIndex,

        /// <summary>
        /// Theme road graphics address index.
        /// </summary>
        ThemeRoadGraphics,

        /// <summary>
        /// Theme color palette address index.
        /// </summary>
        ThemePalettes,

        /// <summary>
        /// Common road tileset graphics address index (upper 8 bits).
        /// </summary>
        CommonTilesetGraphicsUpperByte,

        /// <summary>
        /// Common road tileset graphics address index (lower 16 bytes).
        /// </summary>
        CommonTilesetGraphicsLowerBytes,

        /// <summary>
        /// The associations between tracks and item probabilities.
        /// </summary>
        TrackItemProbabilityIndexes,

        /// <summary>
        /// Item probabilities.
        /// </summary>
        ItemProbabilities,

        /// <summary>
        /// Item icon graphics.
        /// </summary>
        ItemIconGraphics,

        /// <summary>
        /// Item graphics.
        /// </summary>
        ItemGraphics,

        /// <summary>
        /// Item icon tile indexes and properties (color palette and flip flag).
        /// </summary>
        ItemIconTileLayout,

        /// <summary>
        /// Top border tile index and properties (color palette and flip flag).
        /// </summary>
        TopBorderTileLayout,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack1,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack2,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack3,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack4,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack5,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack6,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack7,

        /// <summary>
        /// Offset for hack to extend the object engine.
        /// </summary>
        TrackObjectHack8,

        /// <summary>
        /// The track object properties (tileset, interaction, routine) for each track.
        /// </summary>
        TrackObjectProperties,

        /// <summary>
        /// Track object zones (new offset, after relocation by the editor).
        /// </summary>
        TrackObjectZonesRelocated,

        /// <summary>
        /// Offset for hack to make it possible to define the
        /// track object color palette and flashing settings for each track.
        /// </summary>
        TrackObjectPalHack1,

        /// <summary>
        /// Offset for hack to make it possible to define the
        /// track object color palette and flashing settings for each track.
        /// </summary>
        TrackObjectPalHack2,

        /// <summary>
        /// Offset for hack to make it possible to define the
        /// track object color palette and flashing settings for each track.
        /// </summary>
        TrackObjectPalHack3,

        /// <summary>
        /// Offset for hack to make it possible to define the
        /// track object color palette and flashing settings for each track.
        /// </summary>
        TrackObjectPalHack4,

        /// <summary>
        /// The object color palette indexes.
        /// </summary>
        TrackObjectPaletteIndexes,

        /// <summary>
        /// The object color flashing setting.
        /// </summary>
        TrackObjectFlashing,

        /// <summary>
        /// The tile types for each theme tileset.
        /// </summary>
        TileGenres,

        /// <summary>
        /// The tile type indexes for each theme tileset.
        /// </summary>
        TileGenreIndexes,

        /// <summary>
        /// Jump bar check address index.
        /// </summary>
        JumpBarCheck,

        /// <summary>
        /// Tile genre loading routine address index.
        /// </summary>
        TileGenreLoad,

        /// <summary>
        /// The tile types for each theme tileset (after relocation).
        /// </summary>
        TileGenresRelocated,

        /// <summary>
        /// Offset for hack to make it so each road tileset
        /// has 256 unique tiles, and no more shared tiles.
        /// </summary>
        RoadTilesetHack1,

        /// <summary>
        /// Offset for hack to make it so each road tileset
        /// has 256 unique tiles, and no more shared tiles.
        /// </summary>
        RoadTilesetHack2,

        /// <summary>
        /// Offset for hack to make it so each road tileset
        /// has 256 unique tiles, and no more shared tiles.
        /// </summary>
        RoadTilesetHack3,

        /// <summary>
        /// Offset for hack to make it so each road tileset
        /// has 256 unique tiles, and no more shared tiles.
        /// </summary>
        RoadTilesetHack4,

        /// <summary>
        /// How many points drivers get depending on their finishing position, from 1st to 8th.
        /// </summary>
        RankPoints
    }

    internal class Offsets
    {
        private readonly int[] offsets;

        /// <summary>
        /// Loads all the needed offsets depending on the ROM region.
        /// </summary>
        public Offsets(byte[] romBuffer, Region region)
        {
            this.offsets = new int[Enum.GetValues(typeof(Offset)).Length];

            switch (region)
            {
                case Region.Jap:
                    this[Offset.ModeNames] = 0x58B19;
                    this[Offset.GPCupSelectTexts] = 0x4F6D7;
                    this[Offset.GPPodiumCupTexts] = 0x5A092;
                    this[Offset.DriverNamesGPResults] = 0x5C1EC;
                    this[Offset.DriverNamesGPPodium] = 0x5A0E0;
                    this[Offset.DriverNamesTimeTrial] = 0x1DDCA;
                    this[Offset.BattleTrackOrder] = 0x1C022;
                    this[Offset.FirstBattleTrack] = 0x1BF0A;
                    this[Offset.TrackMaps] = Utilities.BytesToOffset(Utilities.ReadBlock(romBuffer, 0x1E74D, 3));
                    this[Offset.TrackAIDataFirstAddressByte] = 0x1FBC4;
                    this[Offset.TrackAIZones] = 0x1FF8C;
                    this[Offset.BattleTrackStartPositions] = 0x18B5F;
                    this[Offset.TrackPreviewLapLines] = 0x1C886;
                    this[Offset.ItemIconTileLayout] = 0x1B1DC;
                    this[Offset.TrackOverlayPatterns] = 0x4F0B5;
                    this[Offset.TrackObjectHack1] = 0x18EF3;
                    this[Offset.TrackObjectHack2] = 0x19155;
                    this[Offset.TrackObjectHack3] = 0x19E8E;
                    this[Offset.TrackObjectHack4] = 0x1E996;
                    this[Offset.TrackObjectPalHack1] = 0xBD33;
                    this[Offset.JumpBarCheck] = 0xB795;
                    this[Offset.CommonTilesetGraphicsUpperByte] = 0x1E6C1;
                    this[Offset.RoadTilesetHack1] = 0x1E695;
                    this[Offset.GhostValleyBackgroundAnimationGraphics] = 0x3F04B;
                    this[Offset.RankPoints] = 0x5BE52;
                    break;

                case Region.US:
                    this[Offset.ModeNames] = 0x58B00;
                    this[Offset.GPCupSelectTexts] = 0x4F85F;
                    this[Offset.GPPodiumCupTexts] = 0x5A0EE;
                    this[Offset.DriverNamesGPResults] = 0x5C25B;
                    this[Offset.DriverNamesGPPodium] = 0x5A148;
                    this[Offset.DriverNamesTimeTrial] = 0x1DDD3;
                    this[Offset.BattleTrackOrder] = 0x1C15C;
                    this[Offset.FirstBattleTrack] = 0x1C04C;
                    this[Offset.TrackMaps] = Utilities.BytesToOffset(Utilities.ReadBlock(romBuffer, 0x1E749, 3));
                    this[Offset.TrackAIDataFirstAddressByte] = 0x1FBD3;
                    this[Offset.TrackAIZones] = 0x1FF9B;
                    this[Offset.BattleTrackStartPositions] = 0x18B4B;
                    this[Offset.TrackPreviewLapLines] = 0x1C915;
                    this[Offset.ItemIconTileLayout] = 0x1B320;
                    this[Offset.TrackOverlayPatterns] = 0x4F23D;
                    this[Offset.TrackObjectHack1] = 0x18EDF;
                    this[Offset.TrackObjectHack2] = 0x19141;
                    this[Offset.TrackObjectHack3] = 0x19E2B;
                    this[Offset.TrackObjectHack4] = 0x1E992;
                    this[Offset.TrackObjectPalHack1] = 0xBD0E;
                    this[Offset.JumpBarCheck] = 0xB79E;
                    this[Offset.CommonTilesetGraphicsUpperByte] = 0x1E6BD;
                    this[Offset.RoadTilesetHack1] = 0x1E691;
                    this[Offset.GhostValleyBackgroundAnimationGraphics] = 0x3F058;
                    this[Offset.RankPoints] = 0x5BEB4;
                    break;

                case Region.Euro:
                    this[Offset.ModeNames] = 0x58AF2;
                    this[Offset.GPCupSelectTexts] = 0x4F778;
                    this[Offset.GPPodiumCupTexts] = 0x5A0F8;
                    this[Offset.DriverNamesGPResults] = 0x5C263;
                    this[Offset.DriverNamesGPPodium] = 0x5A152;
                    this[Offset.DriverNamesTimeTrial] = 0x1DC81;
                    this[Offset.BattleTrackOrder] = 0x1BFF8;
                    this[Offset.FirstBattleTrack] = 0x1BEE8;
                    this[Offset.TrackMaps] = Utilities.BytesToOffset(Utilities.ReadBlock(romBuffer, 0x1E738, 3));
                    this[Offset.TrackAIDataFirstAddressByte] = 0x1FB9D;
                    this[Offset.TrackAIZones] = 0x1FF6D;
                    this[Offset.BattleTrackStartPositions] = 0x18B64;
                    this[Offset.TrackPreviewLapLines] = 0x1C7B1;
                    this[Offset.ItemIconTileLayout] = 0x1B1BC;
                    this[Offset.TrackOverlayPatterns] = 0x4F159;
                    this[Offset.TrackObjectHack1] = 0x18EF8;
                    this[Offset.TrackObjectHack2] = 0x1915A;
                    this[Offset.TrackObjectHack3] = 0x19E68;
                    this[Offset.TrackObjectHack4] = 0x1E981;
                    this[Offset.TrackObjectPalHack1] = 0xBD33;
                    this[Offset.JumpBarCheck] = 0xB7A3;
                    this[Offset.CommonTilesetGraphicsUpperByte] = 0x1E6AC;
                    this[Offset.RoadTilesetHack1] = 0x1E680;
                    this[Offset.GhostValleyBackgroundAnimationGraphics] = 0x3F058;
                    this[Offset.RankPoints] = 0x5BEBC;
                    break;
            }

            this[Offset.TileGenres] = 0x7FDBA;
            this[Offset.TileGenresRelocated] = Utilities.BytesToOffset(Utilities.ReadBlock(romBuffer, this[Offset.JumpBarCheck] + 1, 3)) + 0x12;
            this[Offset.ItemIconGraphics] = 0x112F8;
            this[Offset.TrackObjects] = 0x5C800;
            this[Offset.TrackObjectZones] = 0x4DB93;
            this[Offset.TrackOverlayItems] = 0x5D000;
            this[Offset.TrackLapLines] = 0x180D4;
            this[Offset.MatchRaceObjectGraphics] = 0x60000;
            this[Offset.ItemGraphics] = 0x40594;
            this[Offset.TrackObjectHack5] = 0x4DABC;
            this[Offset.TrackObjectHack6] = 0x4DCA9;
            this[Offset.TrackObjectHack7] = 0x4DCBD;
            this[Offset.TrackObjectHack8] = 0x4DCC2;
            this[Offset.TrackObjectProperties] = 0x80062;
            this[Offset.TrackObjectZonesRelocated] = Utilities.BytesToOffset(Utilities.ReadBlock(romBuffer, this[Offset.TrackObjectHack5] + 1, 3)) + 0x6E;
            this[Offset.TrackObjectPaletteIndexes] = Utilities.BytesToOffset(Utilities.ReadBlock(romBuffer, this[Offset.TrackObjectHack3] + 1, 3)) + 0x13;
            this[Offset.TrackObjectFlashing] = this[Offset.TrackObjectPaletteIndexes] + Track.Count * 4;
            this[Offset.CommonTilesetGraphicsLowerBytes] = this[Offset.CommonTilesetGraphicsUpperByte] + 3;
            this[Offset.TileGenreLoad] = this[Offset.CommonTilesetGraphicsUpperByte] + 0x454;
            this[Offset.RoadTilesetHack2] = this[Offset.RoadTilesetHack1] + 0x28;
            this[Offset.RoadTilesetHack3] = this[Offset.RoadTilesetHack2] + 0x38;
            this[Offset.RoadTilesetHack4] = this[Offset.RoadTilesetHack3] + 0x92;

            this[Offset.TopBorderTileLayout] = this[Offset.ItemIconTileLayout] + 0x507;
            this[Offset.TrackItemProbabilityIndexes] = this[Offset.BattleTrackStartPositions] + 0x28;
            this[Offset.GPTrackStartPositions] = this[Offset.BattleTrackStartPositions] + 0xC8;
            this[Offset.BattleTrackStartPositionsIndex] = this[Offset.BattleTrackStartPositions] + 0x3C9;
            this[Offset.TrackAITargets] = this[Offset.TrackAIZones] + 0x30;
            this[Offset.BattleTrackNames] = this[Offset.TrackPreviewLapLines] + 0x28;
            this[Offset.CupNamesLocked] = this[Offset.BattleTrackNames] + 0x12;
            this[Offset.CupNames] = this[Offset.CupNamesLocked] + 0xE;
            this[Offset.GPTrackNames] = this[Offset.CupNames] + 0x12;
            this[Offset.NamesAndSuffixes] = this[Offset.GPTrackNames] + 0x58;
            this[Offset.CupAndThemeTexts] = this[Offset.NamesAndSuffixes] + 0x6B;
            this[Offset.TrackOverlaySizes] = this[Offset.TrackOverlayPatterns] + 0x147;
            this[Offset.ItemProbabilities] = this[Offset.ItemIconTileLayout] + 0x1C3;

            this[Offset.TileGenreIndexes] = this[Offset.TrackMaps] - Theme.Count * 2;
            this[Offset.ThemeRoadGraphics] = this[Offset.TrackMaps] + Track.Count * 3;
            this[Offset.ThemePalettes] = this[Offset.ThemeRoadGraphics] + Theme.Count * 3;
            this[Offset.TrackObjectGraphics] = this[Offset.ThemePalettes] + Theme.Count * 3;
            this[Offset.ThemeBackgroundGraphics] = this[Offset.TrackObjectGraphics] + Theme.Count * 3;
            this[Offset.ThemeBackgroundLayouts] = this[Offset.ThemeBackgroundGraphics] + Theme.Count * 3;
            this[Offset.GPTrackOrder] = this[Offset.ThemeBackgroundLayouts] + Theme.Count * 3;
            this[Offset.TrackThemes] = this[Offset.GPTrackOrder] + GPTrack.Count;

            this[Offset.TrackObjectPalHack2] = this[Offset.TrackObjectPalHack1] + 0x0E;
            this[Offset.TrackObjectPalHack3] = this[Offset.TrackObjectPalHack2] + 0x6A;
            this[Offset.TrackObjectPalHack4] = this[Offset.TrackObjectPalHack3] + 0x2750;
        }

        public int this[Offset offset]
        {
            get { return offsets[(int)offset]; }
            set { this.offsets[(int)offset] = value; }
        }
    }
}
