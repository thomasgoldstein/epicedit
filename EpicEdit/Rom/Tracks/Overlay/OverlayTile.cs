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
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace EpicEdit.Rom.Tracks.Overlay
{
	/// <summary>
	/// Represents an element located over the track map. E.g.: a coin, an item block...
	/// </summary>
	public class OverlayTile
	{
		private Point location;
		public Point Location
		{
			get
			{
				return this.location;
			}
			set
			{
				int x = value.X;
				int y = value.Y;

				if (x < 0)
				{
					x = 0;
				}
				else if (x + this.Width > 128)
				{
					x = 128 - this.Width;
				}

				if (y < 0)
				{
					y = 0;
				}
				else if (y + this.Height > 128)
				{
					y = 128 - this.Height;
				}

				this.location = new Point(x, y);
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

		public int Width
		{
			get { return this.Size.Width; }
			set { this.Size.Width = value; }
		}

		public int Height
		{
			get { return this.Size.Height; }
			set { this.Size.Height = value; }
		}

		public OverlayTileSize Size { get; set; }
		public OverlayTilePattern Pattern { get; set; }

		/// <summary>
		/// Initializes an OverlayTile.
		/// </summary>
		/// <param name="data">The byte array to get the data from.</param>
		/// <param name="index">The index to use in the byte array.</param>
		/// <param name="sizes">The collection of overlay tile sizes.</param>
		/// <param name="patterns">The collection of overlay tile patterns.</param>
		public OverlayTile(byte[] data, int index, OverlayTilePatterns patterns, OverlayTileSizes sizes)
		{
			this.SetBytes(data, index, patterns, sizes);
		}

		public bool IntersectsWith(Point point)
		{
			return
				point.X >= this.X &&
				point.X < this.X + this.Width &&
				point.Y >= this.Y &&
				point.Y < this.Y + this.Height;
		}

		private void SetBytes(byte[] data, int index, OverlayTilePatterns patterns, OverlayTileSizes sizes)
		{
			this.Size = sizes[(data[index] & 0xC0) >> 6];
			this.Pattern = patterns[data[index] & 0x3F];

			int x = (data[index + 1] & 0x7F);
			int y = ((data[index + 2] & 0x3F) << 1) + ((data[index + 1] & 0x80) >> 7);
			this.location = new Point(x, y);
		}

		/// <summary>
		/// Fills the passed byte array with data in the format the SMK ROM expects.
		/// </summary>
		/// <param name="data">The byte array to fill.</param>
		/// <param name="index">The array position where the data will be copied.</param>
		/// <param name="sizes">The collection of overlay tile sizes.</param>
		/// <param name="patterns">The collection of overlay tile patterns.</param>
		public void GetBytes(byte[] data, int index, OverlayTileSizes sizes, OverlayTilePatterns patterns)
		{
			int sizeIndex = sizes.IndexOf(this.Size);
			int patternIndex = patterns.IndexOf(this.Pattern);
			data[index] = (byte)((byte)(sizeIndex << 6) | patternIndex);

			data[index + 1] = (byte)(this.X + ((this.Y << 7) & 0x80));
			data[index + 2] = (byte)(this.Y >> 1);
		}
	}
}
