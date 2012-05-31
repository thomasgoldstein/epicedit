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
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    internal sealed class BackgroundPanel : TilePanel
    {
        [Browsable(false), DefaultValue(typeof(BackgroundDrawer), "")]
        public BackgroundDrawer Drawer { get; set; }
        
        [Browsable(false), DefaultValue(typeof(Background), "")]
        public Background Background { get; set; }

        public bool Front { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Drawer == null)
            {
                return;
            }

            base.OnPaint(e);
            int x = (int)(this.AutoScrollPosition.X / this.Zoom);
            this.Drawer.DrawBackgroundLayer(e.Graphics, x, this.Front);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);

            this.Invalidate();
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            byte tileId;
            byte properties;
            this.Background.Layout.GetTileData(x, y, this.Front, out tileId, out properties);

            Tile2bpp tile = this.Background.Tileset[tileId];
            int paletteStart = this.Front ? Background.FrontPaletteStart : Background.BackPaletteStart;
            Tile2bpp clone = new Tile2bpp(tile.Graphics, tile.Palettes, properties, paletteStart);
            return clone; // NOTE: We're leaking a bit of memory here, as the clone is not explicitly disposed
        }
    }
}
