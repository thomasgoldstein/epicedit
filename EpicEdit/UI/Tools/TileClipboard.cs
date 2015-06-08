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
using EpicEdit.Rom.Tracks.Road;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A collection of tiles copied by the user.
    /// </summary>
    internal class TileClipboard
    {
        /// <summary>
        /// Where copied tiles are stored.
        /// </summary>
        private List<byte> data;

        /// <summary>
        /// Gets or sets the rectangle representing the tile clipboard selection.
        /// </summary>
        public Rectangle Rectangle { get; set; }

        /// <summary>
        /// Gets the top-left position of the clipboard rectangle, as a tile selection is occuring.
        /// </summary>
        public Point Location
        {
            get { return this.Rectangle.Location; }
        }

        /// <summary>
        /// Gets the dimension of the tile clipboard.
        /// </summary>
        public Size Size
        {
            get { return this.Rectangle.Size; }
        }

        /// <summary>
        /// Gets the first tile stored in the clipboard.
        /// </summary>
        public byte FirstTile
        {
            get { return this.data[0]; }
        }

        public TileClipboard(byte tile)
        {
            this.data = new List<byte>();
            this.Add(tile);
        }

        /// <summary>
        /// Fills the clipboard with a given tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        public void Fill(byte tile)
        {
            this.data.Clear();
            this.Add(tile);
        }

        private void Add(byte tile)
        {
            this.data.Add(tile);
            this.Rectangle = new Rectangle(0, 0, 1, 1);
        }

        /// <summary>
        /// Fills the clipboard with the selected tiles.
        /// </summary>
        /// <param name="trackMap">The track map the tiles will be copied from.</param>
        public void Fill(TrackMap trackMap)
        {
            this.data.Clear();

            for (int y = this.Rectangle.Y; y < this.Rectangle.Bottom; y++)
            {
                for (int x = this.Rectangle.X; x < this.Rectangle.Right; x++)
                {
                    this.data.Add(trackMap[x, y]);
                }
            }
        }

        /// <summary>
        /// Gets the tiles stored in the clipboard.
        /// </summary>
        /// <returns>The tiles.</returns>
        public byte[][] GetData()
        {
            byte[][] tiles = new byte[this.Rectangle.Height][];

            for (int y = 0; y < tiles.Length; y++)
            {
                tiles[y] = new byte[this.Rectangle.Width];

                for (int x = 0; x < tiles[y].Length; x++)
                {
                    tiles[y][x] = this.data[x + y * this.Rectangle.Width];
                }
            }

            return tiles;
        }
    }
}
