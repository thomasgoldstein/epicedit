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
using System.Drawing;

namespace EpicEdit.Rom.Tracks.Road
{
    /// <summary>
    /// The map layout of a track.
    /// </summary>
    internal class TrackMap : IMapBuffer
    {
        public const int Size = 128;
        public const int Limit = Size - 1;
        public const int SquareSize = Size * Size;

        public event EventHandler<EventArgs> DataChanged;

        private readonly byte[][] _map;

        public TrackMap(byte[] data)
        {
            _map = new byte[(int)Math.Sqrt(SquareSize)][];
            SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            if (data.Length != SquareSize)
            {
                throw new ArgumentException($"The map array must have a length of {SquareSize} ({Size} * {Size}).", nameof(data));
            }

            for (var y = 0; y < _map.Length; y++)
            {
                _map[y] = Utilities.ReadBlock(data, y * _map.Length, _map.Length);
            }
        }

        /// <summary>
        /// Gets the tile value at the given coordinates.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(int x, int y)
        {
            return _map[y][x];
        }

        /// <summary>
        /// Gets the tile value at the given coordinates.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(Point position)
        {
            return GetTile(position.X, position.Y);
        }

        /// <summary>
        /// Sets the tile value at the given coordinates.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="tile">Tile value.</param>
        public void SetTile(int x, int y, byte tile)
        {
            if (SetTileInternal(x, y, tile))
            {
                OnDataChanged();
            }
        }

        /// <summary>
        /// Sets the tile value at the given coordinates without raising a DataChanged event.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="tile">Tile value.</param>
        /// <returns>True if the new value is different from the old one, false otherwise.</returns>
        private bool SetTileInternal(int x, int y, byte tile)
        {
            var dataChanged = _map[y][x] != tile;
            _map[y][x] = tile;
            return dataChanged;
        }

        /// <summary>
        /// Sets the value of a group of tiles.
        /// </summary>
        /// <param name="startingPosition">Top-left position of tiles to be changed.</param>
        /// <param name="tileBuffer">The tile buffer.</param>
        public void SetTiles(Point startingPosition, IMapBuffer tileBuffer)
        {
            SetTiles(startingPosition.X, startingPosition.Y, tileBuffer);
        }

        /// <summary>
        /// Sets the value of a group of tiles.
        /// </summary>
        /// <param name="startX">Left-most position of the tiles to be changed.</param>
        /// <param name="startY">Top-most position of the tiles to be changed.</param>
        /// <param name="tileBuffer">The tile buffer.</param>
        public void SetTiles(int startX, int startY, IMapBuffer tileBuffer)
        {
            var dataChanged = false;
            var yLimit = Math.Min(tileBuffer.Height, Size - startY);
            var xLimit = Math.Min(tileBuffer.Width, Size - startX);

            for (var y = 0; y < yLimit; y++)
            {
                var positionY = startY + y;

                for (var x = 0; x < xLimit; x++)
                {
                    var positionX = startX + x;

                    if (SetTileInternal(positionX, positionY, tileBuffer[x, y]))
                    {
                        dataChanged = true;
                    }
                }
            }

            if (dataChanged)
            {
                OnDataChanged();
            }
        }

        public void Clear(byte tile)
        {
            for (var x = 0; x < Width; x++)
            {
                _map[0][x] = tile;
            }

            for (var y = 1; y < Height; y++)
            {
                Buffer.BlockCopy(_map[0], 0, _map[y], 0, Width);
            }

            OnDataChanged();
        }

        private void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        public int Width => _map[0].Length;

        public int Height => _map.Length;

        public byte this[int x, int y]
        {
            get => GetTile(x, y);
            set => SetTile(x, y, value);
        }

        public byte[] GetBytes()
        {
            var data = new byte[Width * Height];

            for (var y = 0; y < Height; y++)
            {
                Buffer.BlockCopy(_map[y], 0, data, y * Width, Width);
            }

            return data;
        }
    }
}
