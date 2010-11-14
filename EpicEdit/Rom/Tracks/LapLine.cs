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
	/// The lap line of a track.
	/// </summary>
	public class LapLine
	{
		private Point location;
		public Point Location
		{
			get { return this.location; }
			set
			{
				// Divide x precision, so that the lap line
				// is horizontally positioned following a 2-tile (16-px) step
				int x = (value.X / 16) * 16;
				int y = value.Y;

				if (x < 0)
				{
					x = 0;
				}
				else if (x + this.Length > 128 * 8)
				{
					x = 128 * 8 - this.Length;
				}

				if (y < 0)
				{
					y = 0;
				}
				else if (y >= 128 * 8)
				{
					y = 128 * 8 - 1;
				}

				this.location = new Point(x, y);
			}
		}

		private int length;
		public int Length
		{
			get { return this.length; }
			private set
			{
				if (value < 16)
				{
					this.length = 16;
				}
				else
				{
					this.length = value;
				}
			}
		}

		public int X
		{
			get { return this.location.X; }
		}

		public int Y
		{
			get { return this.location.Y; }
		}

		/// <summary>
		/// Gets the x-coordinate that is the sum of X and Length property values of this LapLine.
		/// </summary>
		public int Right
		{
			get { return this.X + this.Length; }
		}

		public LapLine(byte[] data)
		{
			// In the game, the lap line data consists of a vertical value (the 2 first bytes),
			// and a rectangle (the 4 following bytes: x, y, width, height).
			// The only point of interest is where the vertical value intersects with
			// the rectangle, so, to simplify things, we only load the data as a line
			// defined by a point and a length, ignoring the 2 last bytes.

			this.Length = (data[4] & 0x3F) * 16;

			int y = (((data[1] & 0x03) << 8) + data[0]);
			int x = (data[2] & 0x3F) * 16;
			this.Location = new Point(x, y);
			// The bit mask on x is required for some of the original SMK track lap line zones
			// to work properly, as some of them have the 2 highest bits needlessly set to 1.
			// So it's necessary to only use the 6 lowest bits, like the game does.
		}

		public bool IntersectsWith(Point point)
		{
			return point.X >= this.X - 8 &&
				point.X <= this.Right + 7 &&
				point.Y >= this.Y - 8 &&
				point.Y <= this.Y + 7;
		}

		public ResizeHandle GetResizeHandle(Point point)
		{
			if (point.X <= this.X + 7)
			{
				return ResizeHandle.Left;
			}

			if (point.X >= this.Right - 8)
			{
				return ResizeHandle.Right;
			}

			return ResizeHandle.None;
		}

		public void Resize(ResizeHandle resizeHandle, int x)
		{
			// Divide x precision, so that the lap line
			// is horizontally positioned following a 2-tile (16-px) step
			x = (x / 16) * 16;

			if (resizeHandle == ResizeHandle.Left)
			{
				if (this.Right - x <= 0)
				{
					x = this.Right - 16;
				}

				this.Length += this.X - x;
				this.location = new Point(x, this.Y);
			}
			else if (resizeHandle == ResizeHandle.Right)
			{
				x += 16; // This makes the resizing behavior symmetrical

				if (x <= this.X)
				{
					this.Length = 16;
				}
				else
				{
					this.Length = x - this.X;
				}
			}
		}

		public byte[] GetBytes()
		{
			byte[] data = new byte[6];

			int y = this.Y;
			data[0] = (byte)(y & 0xFF);
			data[1] = (byte)(y >> 8);

			int recX = this.X / 16;
			data[2] = (byte)recX;

			int recY = (int)Math.Round((float)this.Y / 64) - 1;
			// The minus 1 is to make the rectangle start at least 8 tiles above the lap line Y value

			if (recY < 0)
			{
				recY = 0;
			}
			data[3] = (byte)recY;

			int width = this.Length / 16;
			data[4] = (byte)width;

			int height = 8; // 8 = 16 tiles
			if (recY + height > 64) // 64 = 128 tiles, the height of a track map.
			{
				height = 64 - recY;
			}
			data[5] = (byte)height;

			return data;
		}
	}
}
