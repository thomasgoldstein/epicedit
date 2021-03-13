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

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// Describes a single overlay pattern, including its dimensions.
    /// </summary>
    internal class OverlayTilePattern : IEquatable<OverlayTilePattern>
    {
        private byte[][] _tiles;

        public OverlayTileSize Size { get; }

        public int Width => Size.Width;

        public int Height => Size.Height;

        public bool Modified { get; private set; }

        public OverlayTilePattern(byte[] data, OverlayTileSize size)
        {
            if (data.Length != size.Width * size.Height)
            {
                throw new ArgumentException("Data does not match size.");
            }

            Size = size;
            SetBytes(data);
            Modified = false;
        }

        public byte GetTile(int x, int y)
        {
            return _tiles[y][x];
        }

        public byte this[int x, int y] => GetTile(x, y);

        /// <summary>
        /// Convert data into a nice y/x byte[][] to facilitate reading.
        /// </summary>
        private void SetBytes(byte[] data)
        {
            _tiles = new byte[Height][];

            for (var y = 0; y < Height; y++)
            {
                _tiles[y] = Utilities.ReadBlock(data, y * Width, Width);
            }
        }

        /// <summary>
        /// Reconstructs the original byte[].
        /// </summary>
        public byte[] GetBytes()
        {
            var buffer = new byte[Width * Height];

            for (var y = 0; y < Height; y++)
            {
                Buffer.BlockCopy(_tiles[y], 0, buffer, y * Width, Width);
            }

            return buffer;
        }

        public bool Equals(OverlayTilePattern other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.Width != Width ||
                other.Height != Height)
            {
                return false;
            }

            if (other.Size != Size)
            {
                return false;
            }

            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    if (other._tiles[y][x] != _tiles[y][x])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
