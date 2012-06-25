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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    internal class BackgroundTilePanel : TilePanel
    {
        public bool Front { get; set; }

        private BackgroundTile tile;
        private Image image;

        public BackgroundTilePanel()
        {
            // Initializing fields to avoid null checks before disposing

            if (Context.Game != null) // Avoid designer issues
            {
                Tile2bpp tile = Context.Game.Themes[0].Background.Tileset[0];
                this.tile = new BackgroundTile(tile.Graphics, null);
            }

            this.image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public void UpdateTile(Theme theme, byte tileId, byte properties)
        {
            Tile2bpp tileModel = theme.Background.Tileset[tileId];
            this.tile.Dispose();
            this.tile = new BackgroundTile(tileModel.Graphics, tileModel.Palettes, properties, this.Front);

            int width = this.Width;
            int height = this.Height;

            Bitmap zoomedBitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(zoomedBitmap))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.Clear(theme.BackColor);
                g.DrawImage(this.tile.Bitmap, 0, 0, width, height);
            }

            this.image.Dispose();
            this.image = zoomedBitmap;

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.image, 0, 0);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            return this.tile;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.tile != null)
            {
                this.tile.Dispose();
            }

            this.image.Dispose();
            base.Dispose(disposing);
        }
    }
}
