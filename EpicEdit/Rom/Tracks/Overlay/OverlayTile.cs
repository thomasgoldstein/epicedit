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
using System.Text;

namespace EpicEdit.Rom.Tracks.Overlay
{
	/// <summary>
	/// Represents an element located over the track map. E.g.: a coin, an item block...
	/// </summary>
	public class OverlayTile
	{
		private int x;
		public int X
		{
			get { return x; }
			set
			{
				if (value > 127)
				{
					this.x = 127;
				}
				else if (value < 0)
				{
					this.x = 0;
				}
				else
				{
					this.x = value;
				}
			}
		}

		private int y;
		public int Y
		{
			get { return y; }
			set
			{
				if (value > 127)
				{
					this.y = 127;
				}
				else if (value < 0)
				{
					this.y = 0;
				}
				else
				{
					this.y = value;
				}
			}
		}

		public OverlayTileSize Size { get; set; }
		public OverlayTilePattern Pattern { get; set; }

		/// <summary>
		/// Initializes an OverlayTile.
		/// </summary>
		/// <param name="data">The byte array to get the data from.</param>
		/// <param name="index">The index to use in the byte array.</param>
		/// <param name="patterns">The collection of overlay tile patterns.</param>
		/// <param name="sizes">The collection of overlay tile sizes.</param>
		public OverlayTile(byte[] data, int index, OverlayTilePatterns patterns, OverlayTileSizes sizes)
		{
			this.SetBytes(data, index, patterns, sizes);
		}

		private void SetBytes(byte[] data, int index, OverlayTilePatterns patterns, OverlayTileSizes sizes)
		{
			this.Size = sizes[(data[index] & 0xC0) >> 6];
			this.Pattern = patterns[data[index] & 0x3F];

			this.x = (data[index + 1] & 0x7F);
			this.y = ((data[index + 2] & 0x3F) << 1) + ((data[index + 1] & 0x80) >> 7);
		}

		/// <summary>
		/// Fills the passed byte array with data in the format the SMK ROM expects.
		/// </summary>
		/// <param name="data">The byte array to fill.</param>
		/// <param name="index">The array position where the data will be copied.</param>
		public void GetBytes(byte[] data, int index, OverlayTilePatterns patterns)
		{
			int sizeIndex = this.Size.Index;
			int patternIndex = patterns.IndexOf(this.Pattern);
			data[index] = (byte)((byte)(sizeIndex << 6) | patternIndex);

			data[index + 1] = (byte)(this.x + ((this.y << 7) & 0x80));
			data[index + 2] = (byte)(this.y >> 1);
		}
	}
}
