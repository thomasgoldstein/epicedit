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

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// Collection of all the overlay tile sizes in the game.
    /// </summary>
    internal class OverlayTileSizes : IEnumerable<OverlayTileSize>
    {
        public const int Count = 4;
        public const int Size = Count * OverlayTileSize.Size;

        private readonly OverlayTileSize[] _sizes;

        public OverlayTileSize this[int index] => _sizes[index];

        public bool Modified
        {
            get
            {
                foreach (OverlayTileSize size in _sizes)
                {
                    if (size.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public OverlayTileSizes(byte[] data)
        {
            _sizes = new OverlayTileSize[Count];
            SetBytes(data);
        }

        private void SetBytes(byte[] data)
        {
            byte[][] mData = Utilities.ReadBlockGroup(data, 0, OverlayTileSize.Size, Count);
            for (int i = 0; i < mData.Length; i++)
            {
                _sizes[i] = new OverlayTileSize(mData[i]);
            }
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[_sizes.Length * OverlayTileSize.Size];

            for (int i = 0; i < _sizes.Length; i++)
            {
                _sizes[i].GetBytes(data, i * OverlayTileSize.Size);
            }

            return data;
        }

        public IEnumerator<OverlayTileSize> GetEnumerator()
        {
            foreach (OverlayTileSize size in _sizes)
            {
                yield return size;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sizes.GetEnumerator();
        }

        public int IndexOf(OverlayTileSize size)
        {
            for (int i = 0; i < _sizes.Length; i++)
            {
                if (_sizes[i] == size)
                {
                    return i;
                }
            }

            throw new ArgumentException("Size not found.", nameof(size));
        }
    }
}
