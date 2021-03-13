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

using EpicEdit.Rom.Utility;
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
        public const int MaxTileCount = 41;

        public event EventHandler<EventArgs> DataChanged;
        public event EventHandler<EventArgs<OverlayTile>> ElementAdded;
        public event EventHandler<EventArgs<OverlayTile>> ElementRemoved;
        public event EventHandler<EventArgs> ElementsCleared;

        private readonly OverlayTileSizes _sizes;
        private readonly OverlayTilePatterns _patterns;
        private readonly List<OverlayTile> _overlayTiles;

        public OverlayTiles(byte[] data, OverlayTileSizes sizes, OverlayTilePatterns patterns)
        {
            _sizes = sizes;
            _patterns = patterns;
            _overlayTiles = new List<OverlayTile>();
            SetBytes(data);
        }

        public IEnumerator<OverlayTile> GetEnumerator()
        {
            return _overlayTiles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _overlayTiles.GetEnumerator();
        }

        public int Count => _overlayTiles.Count;

        public OverlayTile this[int index] => _overlayTiles[index];

        public void SetBytes(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException("Incorrect overlay tile data size", nameof(data));
            }

            Clear();
            for (int overlayTileIndex = 0; overlayTileIndex < MaxTileCount; overlayTileIndex++)
            {
                int index = overlayTileIndex * 3;
                if (data[index + 1] == 0xFF &&
                    data[index + 2] == 0xFF)
                {
                    break;
                }

                OverlayTileSize size = _sizes[(data[index] & 0xC0) >> 6];
                OverlayTilePattern pattern = _patterns[data[index] & 0x3F];

                if (pattern.Size != size)
                {
                    // The overlay tile size is different from the expected pattern size,
                    // ignore this overlay tile, the editor cannot handle it.
                    continue;
                }

                int x = (data[index + 1] & 0x7F);
                int y = ((data[index + 2] & 0x3F) << 1) + ((data[index + 1] & 0x80) >> 7);
                Point location = new Point(x, y);

                Add(new OverlayTile(pattern, location));
            }
        }

        /// <summary>
        /// Returns the OverlayTiles data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The OverlayTiles bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[Size];

            for (int overlayTileIndex = 0; overlayTileIndex < _overlayTiles.Count; overlayTileIndex++)
            {
                int index = overlayTileIndex * 3;
                OverlayTile overlayTile = _overlayTiles[overlayTileIndex];
                overlayTile.GetBytes(data, index, _sizes, _patterns);
            }

            for (int index = _overlayTiles.Count * 3; index < data.Length; index++)
            {
                data[index] = 0xFF;
            }

            return data;
        }

        public void Add(OverlayTile overlayTile)
        {
            if (Count >= MaxTileCount)
            {
                return;
            }

            _overlayTiles.Add(overlayTile);
            overlayTile.DataChanged += overlayTile_DataChanged;
            OnElementAdded(overlayTile);
        }

        public void Remove(OverlayTile overlayTile)
        {
            overlayTile.DataChanged -= overlayTile_DataChanged;
            _overlayTiles.Remove(overlayTile);
            OnElementRemoved(overlayTile);
        }

        private void overlayTile_DataChanged(object sender, EventArgs e)
        {
            OnDataChanged();
        }

        /// <summary>
        /// Removes all the overlay tiles from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (OverlayTile tile in _overlayTiles)
            {
                tile.DataChanged -= overlayTile_DataChanged;
            }

            _overlayTiles.Clear();
            OnElementsCleared();
        }

        private void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnElementAdded(OverlayTile value)
        {
            ElementAdded?.Invoke(this, new EventArgs<OverlayTile>(value));
        }

        private void OnElementRemoved(OverlayTile value)
        {
            ElementRemoved?.Invoke(this, new EventArgs<OverlayTile>(value));
        }

        private void OnElementsCleared()
        {
            ElementsCleared?.Invoke(this, EventArgs.Empty);
        }
    }
}
