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
using System.Drawing;

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// An <see cref="OverlayTile"/> collection.
    /// </summary>
    internal class OverlayTiles : IEnumerable<OverlayTile>
    {
        public const int Size = 128;
        public const int MaxTileCount = 42;

        private OverlayTileSizes sizes;
        private OverlayTilePatterns patterns;
        private List<OverlayTile> overlayTiles;

        public OverlayTiles(byte[] data, OverlayTileSizes sizes, OverlayTilePatterns patterns)
        {
            this.sizes = sizes;
            this.patterns = patterns;
            this.SetBytes(data);
        }

        public IEnumerator<OverlayTile> GetEnumerator()
        {
            return this.overlayTiles.GetEnumerator();
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

        private void SetBytes(byte[] data)
        {
            if (data.Length != OverlayTiles.Size)
            {
                throw new ArgumentOutOfRangeException("data");
            }

            this.overlayTiles = new List<OverlayTile>();

            for (int overlayTileIndex = 0; overlayTileIndex < OverlayTiles.MaxTileCount; overlayTileIndex++)
            {
                int index = overlayTileIndex * 3;
                if (data[index + 1] == 0xFF &&
                    data[index + 2] == 0xFF)
                {
                    break;
                }

                OverlayTileSize size = this.sizes[(data[index] & 0xC0) >> 6];
                OverlayTilePattern pattern = this.patterns[data[index] & 0x3F];

                if (pattern.Size != size)
                {
                    // The overlay tile size is different from the expected pattern size,
                    // ignore this overlay tile, the editor cannot handle it.
                    continue;
                }

                int x = (data[index + 1] & 0x7F);
                int y = ((data[index + 2] & 0x3F) << 1) + ((data[index + 1] & 0x80) >> 7);
                Point location = new Point(x, y);

                OverlayTile overlayTile = new OverlayTile(pattern, location);
                this.overlayTiles.Add(overlayTile);
            }
        }

        /// <summary>
        /// Returns the OverlayTiles data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The OverlayTiles bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[OverlayTiles.Size];

            for (int overlayTileIndex = 0; overlayTileIndex < this.overlayTiles.Count; overlayTileIndex++)
            {
                int index = overlayTileIndex * 3;
                OverlayTile overlayTile = this.overlayTiles[overlayTileIndex];
                overlayTile.GetBytes(data, index, this.sizes, this.patterns);
            }

            for (int index = this.overlayTiles.Count * 3; index < data.Length; index++)
            {
                data[index] = 0xFF;
            }

            return data;
        }

        public void Add(OverlayTile overlayTile)
        {
            if (this.Count == OverlayTiles.MaxTileCount)
            {
                return;
            }
            this.overlayTiles.Add(overlayTile);
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
