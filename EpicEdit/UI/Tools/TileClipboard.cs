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

using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A collection of tiles copied by the user.
    /// </summary>
    internal class TileClipboard : IMapBuffer
    {
        public event EventHandler<EventArgs<byte>> TileChanged;
        public event EventHandler<EventArgs<Rectangle>> TilesChanged;

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
        public Point Location => this.Rectangle.Location;

        /// <summary>
        /// Gets the dimension of the tile clipboard.
        /// </summary>
        public Size Size => this.Rectangle.Size;

        public int Width => this.Rectangle.Width;

        public int Height => this.Rectangle.Height;

        /// <summary>
        /// Gets the first tile stored in the clipboard.
        /// </summary>
        public byte FirstTile => this.data[0];

        public TileClipboard(byte tile)
        {
            this.data = new List<byte>();
            this.Add(tile);
        }

        private void Add(byte tile)
        {
            this.data.Add(tile);
            this.Rectangle = new Rectangle(0, 0, 1, 1);
        }

        /// <summary>
        /// Fills the clipboard with a given tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        public void Fill(byte tile)
        {
            this.data.Clear();
            this.Add(tile);
            this.OnTileChanged(tile);
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

            this.OnTilesChanged(this.Rectangle);
        }

        private void OnTileChanged(byte value)
        {
            this.TileChanged?.Invoke(this, new EventArgs<byte>(value));
        }

        private void OnTilesChanged(Rectangle value)
        {
            this.TilesChanged?.Invoke(this, new EventArgs<Rectangle>(value));
        }

        public byte GetByte(int x, int y)
        {
            return this.data[x + y * this.Rectangle.Width];
        }

        public byte this[int x, int y] => this.GetByte(x, y);
    }
}
