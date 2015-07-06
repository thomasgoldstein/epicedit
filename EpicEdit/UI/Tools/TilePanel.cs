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
using System.ComponentModel;
using System.Windows.Forms;

using EpicEdit.Properties;
using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// An EpicPanel with the ability to retrieve a tile pixel color when the Context.ColorPickerMode is on.
    /// </summary>
    internal class TilePanel : EpicPanel
    {
        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected;

        private float zoom = 1;

        [Browsable(true), Category("Appearance"), DefaultValue(typeof(float), "1")]
        public float Zoom
        {
            get { return this.zoom; }
            set { this.zoom = value; }
        }

        protected virtual void GetColorAt(int x, int y, out Palette palette, out int colorIndex)
        {
            // Take scrolling position in consideration
            x -= this.AutoScrollPosition.X;
            y -= this.AutoScrollPosition.Y;

            x = (int)(x / this.Zoom);
            y = (int)(y / this.Zoom);

            Tile tile = this.GetTileAt(x, y);

            if (tile == null)
            {
                palette = null;
                colorIndex = -1;
                return;
            }

            palette = tile.Palette;

            // Get the pixel coordinates within the tile
            x %= Tile.Size;
            y %= Tile.Size;

            colorIndex = tile.GetColorIndexAt(x, y);

            if (tile is RoadTile && colorIndex == 0 ||
                tile is Tile2bpp && (colorIndex % 4) == 0)
            {
                // The back color was selected, which is the first color of the first palette
                colorIndex = 0;
                palette = tile.Palette.Theme.Palettes[0];
            }
        }

        protected virtual Tile GetTileAt(int x, int y)
        {
            throw new NotImplementedException();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!Context.ColorPickerMode)
            {
                base.OnMouseEnter(e);
                return;
            }

            this.Cursor = EpicCursors.ColorPickerCursor;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!Context.ColorPickerMode)
            {
                base.OnMouseLeave(e);
                return;
            }

            this.Cursor = Cursors.Default;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!Context.ColorPickerMode ||
                e.Button == MouseButtons.Middle)
            {
                base.OnMouseDown(e);
                return;
            }

            Palette palette;
            int colorIndex;
            this.GetColorAt(e.X, e.Y, out palette, out colorIndex);

            if (colorIndex != -1) // Not an empty tile
            {
                EventArgs<Palette, int> pea = new EventArgs<Palette, int>(palette, colorIndex);
                this.ColorSelected(this, pea);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Context.ColorPickerMode ||
                e.Button == MouseButtons.Middle)
            {
                base.OnMouseUp(e);

                if (Context.ColorPickerMode)
                {
                    this.Cursor = EpicCursors.ColorPickerCursor;
                }
            }
        }
    }
}
