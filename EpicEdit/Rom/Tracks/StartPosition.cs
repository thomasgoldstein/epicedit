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
	/// The starting position of the drivers on the track.
	/// </summary>
	public class StartPosition
	{
		public Point Location { get; private set; }
		public int SecondRowOffset { get; set; }

		public StartPosition(byte[] data)
		{
			int x = (data[1] << 8) + data[0];
			int y = (data[3] << 8) + data[2];
			this.Location = new Point(x, y);

			this.SecondRowOffset = data[4];
			if (data[5] != 0x00)
			{
				// All original tracks have either has 0x00 or 0xFF for the 6th byte,
				// but we would need something more flexible to match the game behavior
				// if the value is neither 0x00 nor 0xFF (something which shouldn't happen).
				this.SecondRowOffset -= 256;
			}
		}

		/// <summary>
		/// Returns the StartPosition data as a byte array, in the format the SMK ROM expects.
		/// </summary>
		/// <returns>The StartPosition bytes.</returns>
		public byte[] GetBytes()
		{
			byte[] data = new byte[6];
			data[0] = (byte)(this.Location.X & 0xFF);
			data[1] = (byte)((this.Location.X & 0xFF00) >> 8);
			data[2] = (byte)(this.Location.Y & 0xFF);
			data[3] = (byte)((this.Location.Y & 0xFF00) >> 8);

			if (this.SecondRowOffset < 0)
			{
				data[4] = (byte)(this.SecondRowOffset + 256);
				data[5] = 0xFF;
			}
			else
			{
				data[4] = (byte)this.SecondRowOffset;
				data[5] = 0x00;
			}

			return data;
		}

		public bool IntersectsWith(Point point)
		{
			if (this.SecondRowOffset > 0)
			{
				return point.X >= this.Location.X - 8 &&
					point.X <= this.Location.X + this.SecondRowOffset + 7 &&
					point.Y >= this.Location.Y - 8 &&
					point.Y <= this.Location.Y + 176;
			}
			else
			{
				return point.X >= this.Location.X + this.SecondRowOffset - 8 &&
					point.X <= this.Location.X + 7 &&
					point.Y >= this.Location.Y - 8 &&
					point.Y <= this.Location.Y + 176;
			}
		}

		public void MoveTo(int x, int y)
		{
			int limit = 128 * 8;

			if (this.SecondRowOffset > 0)
			{
				if (x < 8)
				{
					x = 8;
				}
				else if (x + this.SecondRowOffset > limit - 8)
				{
					x = limit - 8 - this.SecondRowOffset;
				}
			}
			else
			{
				if (x + this.SecondRowOffset < 8)
				{
					x = 8 - this.SecondRowOffset;
				}
				else if (x > limit - 8)
				{
					x = limit - 8;
				}
			}

			if (y < 8)
			{
				y = 8;
			}
			else if (y > limit - 176)
			{
				y = limit - 176;
			}

			this.Location = new Point(x, y);
		}
	}
}
