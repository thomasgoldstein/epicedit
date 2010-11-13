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

namespace EpicEdit.Rom.Tracks
{
	/// <summary>
	/// The starting position of the drivers on a battle track.
	/// </summary>
	public class BattleStartPosition
	{
		public Point Location { get; set; }
		public int X
		{
			get { return this.Location.X; }
		}
		public int Y
		{
			get { return this.Location.Y; }
		}

		public BattleStartPosition(byte[] data, int index)
		{
			int x = (data[index + 1] << 8) + data[index + 0];
			int y = (data[index + 3] << 8) + data[index + 2];
			this.Location = new Point(x, y);
		}

		public static implicit operator Point(BattleStartPosition position)
		{
			return position.Location;
		}

		public bool IntersectsWith(Point point)
		{
			return point.X >= this.X - 8 &&
				point.X <= this.X + 7 &&
				point.Y >= this.Y - 8 &&
				point.Y <= this.Y + 7;
		}
	}
}
