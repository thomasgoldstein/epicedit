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
using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// An EpicPanel with the ability to retrieve a tile pixel color when the Context.ColorPickerMode is on.
    /// </summary>
    public class TilePanel : EpicPanel
    {
        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected;

        private float zoom = 1;
        public float Zoom
        {
            get { return this.zoom; }
            set { this.zoom = value; }
        }

        private static Cursor colorPickerCursor;
        private static Cursor ColorPickerCursor
        {
            get
            {
                if (TilePanel.colorPickerCursor == null)
                {
                    var resources = new ComponentResourceManager(typeof(Properties.Resources));
                    TilePanel.colorPickerCursor = resources.GetObject("ColorPickerCursor") as Cursor;
                }

                return TilePanel.colorPickerCursor;
            }
        }

        private void GetColorAt(int x, int y, out Palette palette, out int colorIndex)
        {
            x = (int)(x / this.Zoom);
            y = (int)(y / this.Zoom);

            // Convert from pixel precision to tile precision
            int tileX = x / Tile.Size;
            int tileY = y / Tile.Size;

            Tile tile = this.GetTileAt(tileX, tileY);

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

            this.Cursor = TilePanel.ColorPickerCursor;
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
                var pea = new EventArgs<Palette, int>(palette, colorIndex);
                this.ColorSelected(this, pea);
            }
        }
    }
}
