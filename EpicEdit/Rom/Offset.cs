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
        RankPoints,

        /// <summary>
        /// Offset to bytes that MAKE modifies, and that Epic Edit needs to reset.
        /// </summary>
        MakeDataReset1,

        /// <summary>
        /// Offset to bytes that MAKE modifies, and that Epic Edit needs to reset.
        /// </summary>
        MakeDataReset2
    }
}
