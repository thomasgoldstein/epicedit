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
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    internal class BackgroundTilesetPanel : TilePanel
    {
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

        public BackgroundTilesetPanel()
        {
            this.Zoom = BackgroundTilesetDrawer.Zoom;
            this.drawer = new BackgroundTilesetDrawer();
        }

        public void LoadTheme(Theme theme)
        {
            this.drawer.LoadTheme(theme);
            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.drawer.DrawTileset(e.Graphics, this.selectedTile);
        }
    }
}
