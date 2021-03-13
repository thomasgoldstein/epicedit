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
using System.Collections.Generic;
using System.Drawing;

namespace EpicEdit.UI.Tools.UndoRedo
{
    /// <summary>
    /// A change done on a rectangle of tiles.
    /// </summary>
    internal class TileChange : IMapBuffer
    {
        public int X { get; }
        public int Y { get; }

        private readonly byte[][] _data;

        public int Height => _data.Length;

        public int Width => _data[0].Length;

        public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);

        public TileChange(int x, int y, int width, int height, IMapBuffer buffer)
        {
            X = x;
            Y = y;
            _data = new byte[height][];

            for (var yIter = 0; yIter < height; yIter++)
            {
                _data[yIter] = new byte[width];

                for (var xIter = 0; xIter < width; xIter++)
                {
                    _data[yIter][xIter] = buffer[x + xIter, y + yIter];
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
            var xStart = buffer.Width;
            var yStart = buffer.Height;
            var xEnd = 0;
            var yEnd = 0;

            foreach (var change in changes)
            {
                xStart = Math.Min(xStart, change.X);
                xEnd = Math.Max(xEnd, change.X + change.Width);
                yStart = Math.Min(yStart, change.Y);
                yEnd = Math.Max(yEnd, change.Y + change.Height);
            }

            var width = xEnd - xStart;
            var height = yEnd - yStart;

            var data = new byte[height][];
            for (var y = 0; y < height; y++)
            {
                data[y] = new byte[width];
                for (var x = 0; x < width; x++)
                {
                    data[y][x] = buffer[xStart + x, yStart + y];
                }
            }

            foreach (var change in changes)
            {
                var offsetY = change.Y - yStart;
                var offsetX = change.X - xStart;
                for (var y = 0; y < change.Height; y++)
                {
                    for (var x = 0; x < change.Width; x++)
                    {
                        data[offsetY + y][offsetX + x] = change[x, y];
                    }
                }
            }

            X = xStart;
            Y = yStart;
            _data = data;
        }

        /// <summary>
        /// Gets the tile value at the given coordinates.
        /// </summary>
        /// <param name="x">Row.</param>
        /// <param name="y">Column.</param>
        /// <returns>Tile value.</returns>
        public byte GetTile(int x, int y)
        {
            return _data[y][x];
        }

        public byte this[int x, int y] => GetTile(x, y);
    }
}
