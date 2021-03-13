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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.ThemeEdition
{
    internal class BackgroundTilesetPanel : TilePanel
    {
        /// <summary>
        /// Raised when a new tile has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SelectedTileChanged;

        private BackgroundTilesetDrawer _drawer;

        private byte _selectedTile = 0;

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte SelectedTile
        {
            get => _selectedTile;
            set
            {
                _selectedTile = value;
                Invalidate();
            }
        }

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get => _drawer.Theme;
            set
            {
                _drawer.Theme = value;
                Refresh();
            }
        }

        public Tile2bppProperties TileProperties
        {
            get => _drawer.TileProperties;
            set
            {
                _drawer.TileProperties = value;
                Refresh();
            }
        }

        [Browsable(true), Category("Behavior"), DefaultValue(typeof(bool), "True")]
        public bool Front
        {
            get => _drawer.Front;
            set
            {
                _drawer.Front = value;
                Refresh();
            }
        }

        public BackgroundTilesetPanel()
        {
            Zoom = BackgroundTilesetDrawer.Zoom;
            MouseDown += BackgroundTilesetPanel_MouseDown;
        }

        public void ShowExportImageImage()
        {
            UITools.ShowExportTilesetGraphicsDialog(_drawer.Image, Theme.Background.Tileset.GetTiles(), Theme.Name + "bg gfx");
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            _drawer = new BackgroundTilesetDrawer(Size);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            _drawer.DrawTileset(e.Graphics, _selectedTile);
        }

        private void BackgroundTilesetPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            const int zoom = BackgroundTilesetDrawer.Zoom;
            int rowTileCount = Width / (Tile.Size * zoom);
            byte newSelectedTile = (byte)((e.X / (Tile.Size * zoom)) + (e.Y / (Tile.Size * zoom)) * rowTileCount);

            if (_selectedTile != newSelectedTile)
            {
                SelectedTile = newSelectedTile;
                SelectedTileChanged(this, EventArgs.Empty);
            }
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            int tileCountX = Width / (Tile.Size * BackgroundTilesetDrawer.Zoom);
            return Theme.Background.Tileset[x + (y * tileCountX)];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_drawer != null)
                {
                    _drawer.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
