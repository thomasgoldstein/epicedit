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
using System.Collections.Generic;

namespace EpicEdit.Rom.Tracks.Overlay
{
	/// <summary>
	/// A collection of up to 42 <see cref="OverlayTile"/> objects.
	/// </summary>
	public class OverlayTiles : IEnumerable<OverlayTile>
	{
		private List<OverlayTile> overlayTiles;

		public OverlayTiles(byte[] data, OverlayTileSizes sizes, OverlayTilePatterns patterns)
		{
			this.SetBytes(data, sizes, patterns);
		}

		public IEnumerator<OverlayTile> GetEnumerator()
		{
			foreach (OverlayTile tObject in this.overlayTiles)
			{
				yield return tObject;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.overlayTiles.GetEnumerator();
		}

		public int Count
		{
			get { return this.overlayTiles.Count; }
		}

		public OverlayTile this[int index]
		{
			get { return this.overlayTiles[index]; }
		}

		private void SetBytes(byte[] data, OverlayTileSizes sizes, OverlayTilePatterns patterns)
		{
			if (data.Length != 128)
			{
				throw new ArgumentOutOfRangeException("data");
			}

			this.overlayTiles = new List<OverlayTile>();

			for (int overlayTileIndex = 0; overlayTileIndex < 42; overlayTileIndex++)
			{
				int index = overlayTileIndex * 3;
				if (data[index + 1] == 0xFF &&
					data[index + 2] == 0xFF)
				{
					break;
				}

				OverlayTile overlayTile = new OverlayTile(data, index, patterns, sizes);
				this.overlayTiles.Add(overlayTile);
			}
		}

		/// <summary>
		/// Returns the OverlayTiles data as a byte array, in the format the SMK ROM expects.
		/// </summary>
		/// <param name="sizes">The collection of overlay tile sizes.</param>
		/// <param name="patterns">The collection of overlay tile patterns.</param>
		/// <returns>The OverlayTiles bytes.</returns>
		public byte[] GetBytes(OverlayTileSizes sizes, OverlayTilePatterns patterns)
		{
			byte[] data = new byte[128];

			for (int overlayTileIndex = 0; overlayTileIndex < this.overlayTiles.Count; overlayTileIndex++)
			{
				int index = overlayTileIndex * 3;
				OverlayTile overlayTile = this.overlayTiles[overlayTileIndex];
				overlayTile.GetBytes(data, index, sizes, patterns);
			}

			for (int index = this.overlayTiles.Count * 3; index < data.Length; index++)
			{
				data[index] = 0xFF;
			}

			return data;
		}

		public void Remove(OverlayTile overlayTile)
		{
			this.overlayTiles.Remove(overlayTile);
		}

		/// <summary>
		/// Removes all the overlay tiles from the collection.
		/// </summary>
		public void Clear()
		{
			this.overlayTiles.Clear();
		}
	}
}
