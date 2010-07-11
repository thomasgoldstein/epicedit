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
using System.Collections;

namespace EpicEdit.Rom.Tracks.Overlay
{
	/// <summary>
	/// Describes a single overlay pattern, including its dimensions.
	/// </summary>
	public class OverlayTilePattern : IEquatable<OverlayTilePattern>
	{
		/// <summary>
		/// Tiles[y][x]
		/// </summary>
		public byte[][] Tiles { get; private set; }

		public OverlayTileSize Size { get; private set; }

		public int Width
		{
			get { return this.Size.Width; }
		}

		public int Height
		{
			get { return this.Size.Height; }
		}

		public bool Modified { get; private set; }

		public OverlayTilePattern(byte[] data, OverlayTileSize size)
		{
			if (data.Length != size.Width * size.Height)
			{
				throw new ArgumentException("Data does not match size.");
			}

			this.Size = size;
			this.SetBytes(data);
			this.Modified = false;
		}

		/// <summary>
		/// Convert data into a nice y/x byte[][] to facilitate reading
		/// </summary>
		private void SetBytes(byte[] data)
		{
			this.Tiles = new byte[this.Height][];

			for (int y = 0; y < this.Height; y++)
			{
				this.Tiles[y] = new byte[this.Width];
				Array.Copy(data, y * this.Width, this.Tiles[y], 0, this.Width);
			}
		}

		/// <summary>
		/// Reconstructs the original byte[]
		/// </summary>
		public byte[] GetBytes()
		{
			byte[] buffer = new byte[this.Width * this.Height];

			for (int y = 0; y < this.Height; y++)
			{
				Array.Copy(this.Tiles[y], 0, buffer, y * this.Width, this.Width);
			}

			return buffer;
		}

		public bool Equals(OverlayTilePattern other)
		{
			if (other == null)
			{
				return false;
			}

			if (other.Width != this.Width ||
				other.Height != this.Height)
			{
				return false;
			}

			if (other.Size.Index != this.Size.Index)
			{
				return false;
			}

			for (int y = 0; y < this.Height; y++)
			{
				for (int x = 0; x < this.Width; x++)
				{
					if (other.Tiles[y][x] != this.Tiles[y][x])
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
