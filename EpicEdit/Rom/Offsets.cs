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
using EpicEdit.Rom.Tracks;

namespace EpicEdit.Rom
{
    public enum Offset
    {
        /// <summary>
        /// Offset to the names of the modes on the title screen.
        /// </summary>
        ModeStrings,

        /// <summary>
        /// Name texts (cups and themes).
        /// </summary>
        NameStrings,

        /// <summary>
        /// Track map address index.
        /// </summary>
        TrackMaps,

        /// <summary>
        /// Track theme index.
        /// </summary>
        TrackThemes,

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
        /// Track background graphics address index.
        /// </summary>
        TrackBackgroundGraphics,

        /// <summary>
        /// Track background layout address index.
        /// </summary>
        TrackBackgroundLayouts,

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
        ThemeColorPalettes,

        /// <summary>
        /// Common tileset graphics.
        /// </summary>
        CommonTilesetGraphics,

        /// <summary>
        /// Item probabilities.
        /// </summary>
        ItemProbabilities,

        /// <summary>
        /// Item icon graphics.
        /// </summary>
        ItemIcons,

        /// <summary>
        /// Item graphics.
        /// </summary>
        ItemGraphics,

        /// <summary>
        /// Item icon color tile and palette indexes.
        /// </summary>
        ItemIconTilesPalettes
    }

    public class Offsets
    {
        private int[] offsets;

        /// <summary>
        /// Loads all the needed offsets depending on the ROM region.
        /// </summary>
        public Offsets(byte[] romBuffer, Region region)
        {
            this.offsets = new int[Enum.GetValues(typeof(Offset)).Length];

            switch (region)
            {
                case Region.Jap:
                    this[Offset.ModeStrings] = 0x58B19;
                    this[Offset.BattleTrackOrder] = 0x1C022;
                    this[Offset.FirstBattleTrack] = 0x1BF0A;
                    this[Offset.TrackMaps] = Utilities.ByteArrayToOffset(Utilities.ReadBlock(romBuffer, 0x1E74D, 3));
                    this[Offset.TrackAIDataFirstAddressByte] = 0x1FBC4;
                    this[Offset.TrackAIZones] = 0x1FF8C;
                    this[Offset.BattleTrackStartPositions] = 0x18B5F;
                    this[Offset.TrackPreviewLapLines] = 0x1C886;
                    this[Offset.ItemIconTilesPalettes] = 0x1B1DC;
                    this[Offset.TrackOverlayPatterns] = 0x4F0B5;
                    break;

                case Region.US:
                    this[Offset.ModeStrings] = 0x58B00;
                    this[Offset.BattleTrackOrder] = 0x1C15C;
                    this[Offset.FirstBattleTrack] = 0x1C04C;
                    this[Offset.TrackMaps] = Utilities.ByteArrayToOffset(Utilities.ReadBlock(romBuffer, 0x1E749, 3));
                    this[Offset.TrackAIDataFirstAddressByte] = 0x1FBD3;
                    this[Offset.TrackAIZones] = 0x1FF9B;
                    this[Offset.BattleTrackStartPositions] = 0x18B4B;
                    this[Offset.TrackPreviewLapLines] = 0x1C915;
                    this[Offset.ItemIconTilesPalettes] = 0x1B320;
                    this[Offset.TrackOverlayPatterns] = 0x4F23D;
                    //this[Offsets.UnknownMakeRelated] = Utilities.ByteArrayToOffset(Utilities.ReadBlock(romBuffer, 0x1E765, 3)); // TODO: Figure out what that offset is (MAKE-compatibility related)
                    break;

                case Region.Euro:
                    this[Offset.ModeStrings] = 0x58AF2;
                    this[Offset.BattleTrackOrder] = 0x1BFF8;
                    this[Offset.FirstBattleTrack] = 0x1BEE8;
                    this[Offset.TrackMaps] = Utilities.ByteArrayToOffset(Utilities.ReadBlock(romBuffer, 0x1E738, 3));
                    this[Offset.TrackAIDataFirstAddressByte] = 0x1FB9D;
                    this[Offset.TrackAIZones] = 0x1FF6D;
                    this[Offset.BattleTrackStartPositions] = 0x18B64;
                    this[Offset.TrackPreviewLapLines] = 0x1C7B1;
                    this[Offset.ItemIconTilesPalettes] = 0x1B1BC;
                    this[Offset.TrackOverlayPatterns] = 0x4F159;
                    break;
            }

            this[Offset.ItemIcons] = 0x112F8;
            this[Offset.TrackObjects] = 0x5C800;
            this[Offset.TrackObjectZones] = 0x4DB93;
            this[Offset.TrackOverlayItems] = 0x5D000;
            this[Offset.TrackLapLines] = 0x180D4;
            this[Offset.CommonTilesetGraphics] = 0x40000;
            this[Offset.MatchRaceObjectGraphics] = 0x60000;
            this[Offset.ItemGraphics] = 0x40594;

            this[Offset.GPTrackStartPositions] = this[Offset.BattleTrackStartPositions] + 0xC8;
            this[Offset.BattleTrackStartPositionsIndex] = this[Offset.BattleTrackStartPositions] + 0x3C9;
            this[Offset.TrackAITargets] = this[Offset.TrackAIZones] + 0x30;
            this[Offset.BattleTrackNames] = this[Offset.TrackPreviewLapLines] + 0x2A;
            this[Offset.GPTrackNames] = this[Offset.BattleTrackNames] + 0x32;
            this[Offset.NameStrings] = this[Offset.GPTrackNames] + 0xC1;
            this[Offset.TrackOverlaySizes] = this[Offset.TrackOverlayPatterns] + 0x147;
            this[Offset.ItemProbabilities] = this[Offset.ItemIconTilesPalettes] + 0x1C3;

            this[Offset.ThemeRoadGraphics] = this[Offset.TrackMaps] + Track.Count * 3;
            this[Offset.ThemeColorPalettes] = this[Offset.ThemeRoadGraphics] + Theme.Count * 3;
            this[Offset.TrackObjectGraphics] = this[Offset.ThemeColorPalettes] + Theme.Count * 3;
            this[Offset.TrackBackgroundGraphics] = this[Offset.TrackObjectGraphics] + Theme.Count * 3;
            this[Offset.TrackBackgroundLayouts] = this[Offset.TrackBackgroundGraphics] + Theme.Count * 3;
            this[Offset.GPTrackOrder] = this[Offset.TrackBackgroundLayouts] + Theme.Count * 3;
            this[Offset.TrackThemes] = this[Offset.GPTrackOrder] + GPTrack.Count;
        }

        public int this[Offset offset]
        {
            get { return offsets[(int)offset]; }
            set { this.offsets[(int)offset] = value; }
        }
    }
}
