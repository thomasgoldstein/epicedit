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
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.UI.Tools;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Panel that displays a background tile (typically the selected one).
    /// </summary>
    internal class BackgroundTilePanel : TilePanel
    {
        public bool Front { get; set; }

        private BackgroundTile _tile;
        private Image _image;

        public BackgroundTilePanel()
        {
            // Initializing fields to avoid null checks before disposing

            if (Context.Game != null) // Avoid designer issues
            {
                var tile = Context.Game.Themes[0].Background.Tileset[0];
                _tile = new BackgroundTile(tile.Graphics, null);
            }

            _image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public void UpdateTile(Theme theme, byte tileId, byte properties)
        {
            var tileModel = theme.Background.Tileset[tileId];
            _tile.Dispose();
            _tile = new BackgroundTile(tileModel.Graphics, tileModel.Palettes, properties, Front);

            var width = Width;
            var height = Height;

            if (BorderStyle == BorderStyle.Fixed3D)
            {
                width -= SystemInformation.Border3DSize.Width * 2;
                height -= SystemInformation.Border3DSize.Height * 2;
            }

            var zoomedBitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);

            using (var g = Graphics.FromImage(zoomedBitmap))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.Clear(theme.BackColor);
                g.DrawImage(_tile.Bitmap, 0, 0, width, height);
            }

            _image.Dispose();
            _image = zoomedBitmap;

            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_image, 0, 0);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            return _tile;
        }

        protected override void Dispose(bool disposing)
        {
            if (_tile != null)
            {
                _tile.Dispose();
            }

            _image.Dispose();
            base.Dispose(disposing);
        }
    }
}
