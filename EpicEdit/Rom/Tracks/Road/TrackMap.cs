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
        /// Gets the tile value at the given coordinate.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(int x, int y)
        {
            return this.map[y][x];
        }

        /// <summary>
        /// Gets the tile value at the given coordinate.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(Point position)
        {
            return this.GetTile(position.X, position.Y);
        }

        /// <summary>
        /// Sets the tile value at the given coordinate.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <param name="tile">Tile value.</param>
        public void SetTile(int x, int y, byte tile)
        {
            if (this.map[y][x] == tile)
            {
                return;
            }

            this.map[y][x] = tile;
            this.OnDataChanged();
        }

        /// <summary>
        /// Sets the tile value at the given coordinate.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="tile">Tile value.</param>
        public void SetTile(Point position, byte tile)
        {
            this.SetTile(position.X, position.Y, tile);
        }

        /// <summary>
        /// Sets the value of a group of tiles.
        /// </summary>
        /// <param name="startingPosition">Top-left position of <paramref name="affectedSurface"/> and <paramref name="rectangleSize"/>.</param>
        /// <param name="affectedSurface">The area to be modified.</param>
        /// <param name="rectangleSize">The size of the rectangle of new tiles. It is usually the same as the <paramref name="affectedSurface"/>, but is bigger when the rectangle goes beyond the track bounds (in which case the <paramref name="affectedSurface"/> equals the <paramref name="rectangleSize"/> minus the part that's out of bounds).</param>
        /// <param name="tiles">List of tile values.</param>
        public void SetTiles(Point startingPosition, Size affectedSurface, Size rectangleSize, IList<byte> tiles)
        {
            for (int y = 0; y < affectedSurface.Height; y++)
            {
                for (int x = 0; x < affectedSurface.Width; x++)
                {
                    int positionX = startingPosition.X + x;
                    int positionY = startingPosition.Y + y;
                    int tileIndex = x + y * rectangleSize.Width;
                    byte tile = tiles[tileIndex];
                    this.SetTile(positionX, positionY, tile);
                }
            }
        }

        public void Clear(byte tile)
        {
            for (int x = 0; x < this.Width; x++)
            {
                this.SetTile(x, 0, tile);
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
