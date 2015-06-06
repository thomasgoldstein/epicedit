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

using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Tracks.Road
{
    /// <summary>
    /// The map layout of a track.
    /// </summary>
    internal class TrackMap
    {
        public const int Size = 128;
        public const int Limit = TrackMap.Size - 1;
        public const int SquareSize = TrackMap.Size * TrackMap.Size;

        public event EventHandler<EventArgs> DataChanged;

        private byte[][] map;

        public TrackMap(byte[] data)
        {
            if (data.Length != TrackMap.SquareSize)
            {
                throw new ArgumentException(
                    "The map array must have a length of " + TrackMap.SquareSize +
                    " (" + TrackMap.Size + " * " + TrackMap.Size + ").", "data");
            }

            int dimension = (int)Math.Sqrt(data.Length);

            this.map = new byte[dimension][];

            for (int y = 0; y < dimension; y++)
            {
                this.map[y] = Utilities.ReadBlock(data, y * dimension, dimension);
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
            return this.map[y][x];
        }

        /// <summary>
        /// Gets the tile value at the given coordinates.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(Point position)
        {
            return this.GetTile(position.X, position.Y);
        }

        /// <summary>
        /// Sets the tile value at the given coordinates.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="tile">Tile value.</param>
        public void SetTile(int x, int y, byte tile)
        {
            if (this.SetTileInternal(x, y, tile))
            {
                this.OnDataChanged();
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
            bool dataChanged = this.map[y][x] != tile;
            this.map[y][x] = tile;
            return dataChanged;
        }

        /// <summary>
        /// Sets the value of a group of tiles.
        /// </summary>
        /// <param name="startingPosition">Top-left position of tiles to be changed.</param>
        /// <param name="tiles">The new tile values.</param>
        public void SetTiles(Point startingPosition, byte[][] tiles)
        {
            this.SetTiles(startingPosition.X, startingPosition.Y, tiles);
        }

        /// <summary>
        /// Sets the value of a group of tiles.
        /// </summary>
        /// <param name="startX">Left-most position of the tiles to be changed.</param>
        /// <param name="startY">Top-most position of the tiles to be changed.</param>
        /// <param name="tiles">The new tile values.</param>
        public void SetTiles(int startX, int startY, byte[][] tiles)
        {
            bool dataChanged = false;

            for (int y = 0; y < tiles.Length; y++)
            {
                int positionY = startY + y;

                for (int x = 0; x < tiles[y].Length; x++)
                {
                    int positionX = startX + x;
                    byte tile = tiles[y][x];

                    if (this.SetTileInternal(positionX, positionY, tile))
                    {
                        dataChanged = true;
                    }
                }
            }

            if (dataChanged)
            {
                this.OnDataChanged();
            }
        }

        public void Clear(byte tile)
        {
            for (int x = 0; x < this.Width; x++)
            {
                this.map[0][x] = tile;
            }

            for (int y = 1; y < this.Height; y++)
            {
                Buffer.BlockCopy(this.map[0], 0, this.map[y], 0, this.Width);
            }

            this.OnDataChanged();
        }

        private void OnDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }

        public int Width
        {
            get { return this.map[0].Length; }
        }

        public int Height
        {
            get { return this.map.Length; }
        }

        public byte this[int x, int y]
        {
            get { return this.GetTile(x, y); }
            set { this.SetTile(x, y, value); }
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[this.Width * this.Height];

            for (int y = 0; y < this.Height; y++)
            {
                Buffer.BlockCopy(this.map[y], 0, data, y * this.Width, this.Width);
            }

            return data;
        }
    }
}
