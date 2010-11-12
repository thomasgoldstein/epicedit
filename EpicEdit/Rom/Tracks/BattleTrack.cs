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
using EpicEdit.Rom.Tracks.Overlay;

namespace EpicEdit.Rom.Tracks
{
	/// <summary>
	/// A Battle track (Battle Mode).
	/// </summary>
	public class BattleTrack : Track
	{
		public BattleStartPosition StartPositionP1 { get; private set; }
		public BattleStartPosition StartPositionP2 { get; private set; }

		public BattleTrack(string name, Theme theme,
						   byte[] map, byte[] overlayTileData,
						   byte[] aiZoneData, byte[] aiTargetData,
						   byte[] startPositionData,
						   OverlayTileSizes overlayTileSizes,
						   OverlayTilePatterns overlayTilePatterns) :
			base(name, theme, map, overlayTileData, aiZoneData, aiTargetData, overlayTileSizes, overlayTilePatterns)
		{
			this.StartPositionP1 = new BattleStartPosition(startPositionData, 4);
			this.StartPositionP2 = new BattleStartPosition(startPositionData, 0);
		}
	}
}
