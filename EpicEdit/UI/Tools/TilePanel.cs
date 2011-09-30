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

        [Browsable(true), DefaultValue(typeof(float), "1")]
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
                    TilePanel.colorPickerCursor = Resources.ColorPickerCursor;

                    if (TilePanel.colorPickerCursor.Size.IsEmpty)
                    {
                        // HACK: Mitigate the effects of Mono bug #749
                        // http://bugzilla.xamarin.com/show_bug.cgi?id=749
                        TilePanel.colorPickerCursor = Cursors.Hand;
                    }
                }

                return TilePanel.colorPickerCursor;
            }
        }

        protected virtual void GetColorAt(int x, int y, out Palette palette, out int colorIndex)
        {
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

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!Context.ColorPickerMode ||
                e.Button == MouseButtons.Middle)
            {
                base.OnMouseUp(e);

                if (Context.ColorPickerMode)
                {
                    this.Cursor = TilePanel.ColorPickerCursor;
                }
            }
        }
    }
}
