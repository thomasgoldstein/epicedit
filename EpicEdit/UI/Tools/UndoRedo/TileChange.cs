﻿#region GPL statement
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
    public class TileChange
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private byte[][] data;

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
            this.Init(x, y, data);
        }

        public TileChange(Point location, Size size, Track track)
        {
            byte[][] data = new byte[size.Height][];

            for (int y = 0; y < size.Height; y++)
            {
                data[y] = new byte[size.Width];

                for (int x = 0; x < size.Width; x++)
                {
                    data[y][x] = track.Map[location.X + x, location.Y + y];
                }
            }

            this.Init(location.X, location.Y, data);
        }

        private void Init(int x, int y, byte[][] data)
        {
            this.X = x;
            this.Y = y;
            this.data = data;
        }

        /// <summary>
        /// Gets the tile value at the given coordinate.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(int x, int y)
        {
            return this.data[y][x];
        }

        public byte this[int x, int y]
        {
            get { return this.GetTile(x, y); }
        }
    }
}
