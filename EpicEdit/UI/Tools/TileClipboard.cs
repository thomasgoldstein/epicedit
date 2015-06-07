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
        /// Gets or sets the top-left position of the clipboard rectangle, as a tile selection is occuring.
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Gets or sets the dimension of the tile clipboard.
        /// </summary>
        public Size Size { get; set; }

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
            this.Size = new Size(1, 1);
        }

        /// <summary>
        /// Fills the clipboard with the selected tiles.
        /// </summary>
        /// <param name="trackMap">The track map the tiles will be copied from.</param>
        public void Fill(TrackMap trackMap)
        {
            this.data.Clear();

            int xLimit = this.Location.X + this.Size.Width;
            int yLimit = this.Location.Y + this.Size.Height;
            for (int y = this.Location.Y; y < yLimit; y++)
            {
                for (int x = this.Location.X; x < xLimit; x++)
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
            return this.GetData(this.Size);
        }

        /// <summary>
        /// Gets the tiles stored in the clipboard, within a given surface.
        /// </summary>
        /// <param name="surface">The surface.</param>
        /// <returns>The tiles.</returns>
        public byte[][] GetData(Size surface)
        {
            byte[][] tiles = new byte[surface.Height][];

            for (int y = 0; y < tiles.Length; y++)
            {
                tiles[y] = new byte[surface.Width];

                for (int x = 0; x < tiles[y].Length; x++)
                {
                    tiles[y][x] = this.data[x + y * this.Size.Width];
                }
            }

            return tiles;
        }
    }
}
