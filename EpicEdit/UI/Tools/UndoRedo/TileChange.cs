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
using EpicEdit.Rom.Utility;

namespace EpicEdit.UI.Tools.UndoRedo
{
    /// <summary>
    /// A change done on a rectangle of tiles.
    /// </summary>
    internal class TileChange : IMapBuffer
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

        public TileChange(int x, int y, int width, int height, IMapBuffer buffer)
        {
            this.X = x;
            this.Y = y;
            this.data = new byte[height][];

            for (int yIter = 0; yIter < height; yIter++)
            {
                this.data[yIter] = new byte[width];

                for (int xIter = 0; xIter < width; xIter++)
                {
                    this.data[yIter][xIter] = buffer[x + xIter, y + yIter];
                }
            }
        }

        /// <summary>
        /// Consolidates all changes into a single one.
        /// </summary>
        /// <param name="changes">The tile changes.</param>
        /// <param name="buffer">The map buffer the changes are applied on.</param>
        public TileChange(IEnumerable<TileChange> changes, IMapBuffer buffer)
        {
            int xStart = buffer.Width;
            int yStart = buffer.Height;
            int xEnd = 0;
            int yEnd = 0;

            foreach (TileChange change in changes)
            {
                xStart = Math.Min(xStart, change.X);
                xEnd = Math.Max(xEnd, change.X + change.Width);
                yStart = Math.Min(yStart, change.Y);
                yEnd = Math.Max(yEnd, change.Y + change.Height);
            }

            int width = xEnd - xStart;
            int height = yEnd - yStart;

            byte[][] data = new byte[height][];
            for (int y = 0; y < height; y++)
            {
                data[y] = new byte[width];
                for (int x = 0; x < width; x++)
                {
                    data[y][x] = buffer[xStart + x, yStart + y];
                }
            }

            foreach (TileChange change in changes)
            {
                int offsetY = change.Y - yStart;
                int offsetX = change.X - xStart;
                for (int y = 0; y < change.Height; y++)
                {
                    for (int x = 0; x < change.Width; x++)
                    {
                        data[offsetY + y][offsetX + x] = change[x, y];
                    }
                }
            }

            this.X = xStart;
            this.Y = yStart;
            this.data = data;
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

        public byte this[int x, int y]
        {
            get { return this.GetTile(x, y); }
        }
    }
}
