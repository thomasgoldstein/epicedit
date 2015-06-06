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
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Tools.UndoRedo
{
    /// <summary>
    /// A change done on a rectangle of tiles.
    /// </summary>
    internal class TileChange
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private readonly byte[][] data;

        public int Height
        {
            get { return this.data.Length; }
        }

        public int Width
        {
            get { return this.data[0].Length; }
        }

        public TileChange(int x, int y, byte[][] data)
        {
            this.X = x;
            this.Y = y;
            this.data = Clone(data);
        }

        public TileChange(Point location, Size size, Track track)
        {
            this.X = location.X;
            this.Y = location.Y;
            this.data = new byte[size.Height][];

            for (int y = 0; y < size.Height; y++)
            {
                this.data[y] = new byte[size.Width];

                for (int x = 0; x < size.Width; x++)
                {
                    this.data[y][x] = track.Map[location.X + x, location.Y + y];
                }
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
            return this.data[y][x];
        }

        public byte[][] GetTiles()
        {
            return Clone(this.data);
        }

        private static byte[][] Clone(byte[][] data)
        {
            byte[][] copy = new byte[data.Length][];

            for (int y = 0; y < data.Length; y++)
            {
                copy[y] = new byte[data[y].Length];
                Buffer.BlockCopy(data[y], 0, copy[y], 0, data[y].Length);
            }

            return copy;
        }

        public byte this[int x, int y]
        {
            get { return this.GetTile(x, y); }
        }
    }
}
