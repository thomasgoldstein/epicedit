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
using System.Drawing;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    internal class BackgroundTilesetPanel : TilePanel
    {
        /// <summary>
        /// Raised when a new tile has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> SelectedTileChanged;

        private BackgroundTilesetDrawer drawer;

        private byte selectedTile = 0;

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte SelectedTile
        {
            get { return this.selectedTile; }
            set
            {
                this.selectedTile = value;
                this.Invalidate();
            }
        }

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get { return this.drawer.Theme; }
            set
            {
                this.drawer.Theme = value;
                this.Refresh();
            }
        }

        public Tile2bppProperties TileProperties
        {
            get { return this.drawer.TileProperties; }
            set
            {
                this.drawer.TileProperties = value;
                this.Refresh();
            }
        }

        [Browsable(true), DefaultValue(typeof(bool), "True")]
        public bool Front
        {
            get { return this.drawer.Front; }
            set
            {
                this.drawer.Front = value;
                this.Refresh();
            }
        }

        public BackgroundTilesetPanel()
        {
            this.Zoom = BackgroundTilesetDrawer.Zoom;
            this.MouseDown += this.BackgroundTilesetPanel_MouseDown;
        }

        public void ExportImage()
        {
            UITools.ExportImage(this.drawer.Image, this.Theme.Name + "bg");
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.drawer = new BackgroundTilesetDrawer(this.Size);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            this.drawer.DrawTileset(e.Graphics, this.selectedTile);
        }

        private void BackgroundTilesetPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            int zoom = BackgroundTilesetDrawer.Zoom;
            int rowTileCount = this.Width / (Tile.Size * zoom);
            byte newSelectedTile = (byte)((e.X / (Tile.Size * zoom)) + (e.Y / (Tile.Size * zoom)) * rowTileCount);

            if (this.selectedTile != newSelectedTile)
            {
                this.SelectedTile = newSelectedTile;
                this.SelectedTileChanged(this, EventArgs.Empty);
            }
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            int tileCountX = this.Width / (Tile.Size * BackgroundTilesetDrawer.Zoom);
            return this.Theme.Background.Tileset[x + (y * tileCountX)];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.drawer != null)
                {
                    this.drawer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
