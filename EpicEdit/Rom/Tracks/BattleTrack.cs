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
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Start;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// A Battle track (Battle Mode).
    /// </summary>
    internal class BattleTrack : Track
    {
        /// <summary>
        /// Number of battle tracks.
        /// </summary>
        public new const int Count = 4;

        /// <summary>
        /// Number of battle track groups.
        /// </summary>
        public new const int GroupCount = 1;

        /// <summary>
        /// The starting position of Player 1.
        /// </summary>
        public BattleStartPosition StartPositionP1 { get; private set; }

        /// <summary>
        /// The starting position of Player 2.
        /// </summary>
        public BattleStartPosition StartPositionP2 { get; private set; }

        public BattleTrack(SuffixedTextItem nameItem, Theme theme,
                           byte[] map, byte[] overlayTileData,
                           byte[] aiZoneData, byte[] aiTargetData,
                           byte[] startPositionData,
                           OverlayTileSizes overlayTileSizes,
                           OverlayTilePatterns overlayTilePatterns) :
            base(nameItem, theme, map, overlayTileData, aiZoneData, aiTargetData, overlayTileSizes, overlayTilePatterns)
        {
            byte[] startPosition1Data = new byte[] { startPositionData[0], startPositionData[1], startPositionData[2], startPositionData[3] };
            byte[] startPosition2Data = new byte[] { startPositionData[4], startPositionData[5], startPositionData[6], startPositionData[7] };
            this.StartPositionP1 = new BattleStartPosition(startPosition1Data);
            this.StartPositionP2 = new BattleStartPosition(startPosition2Data);
        }

        /// <summary>
        /// Loads the BattleTrack-specific items from the MakeTrack object.
        /// </summary>
        protected override void LoadDataFrom(MakeTrack track)
        {
            base.LoadDataFrom(track);

            this.StartPositionP1 = track.StartPositionP1;
            this.StartPositionP2 = track.StartPositionP2;
        }

        /// <summary>
        /// Loads the BattleTrack-specific items to the MakeTrack object.
        /// </summary>
        protected override void LoadDataTo(MakeTrack track)
        {
            base.LoadDataTo(track);

            track.StartPositionP1 = this.StartPositionP1;
            track.StartPositionP2 = this.StartPositionP2;
        }
    }
}
