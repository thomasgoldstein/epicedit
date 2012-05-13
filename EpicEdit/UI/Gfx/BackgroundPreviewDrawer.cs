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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Scenery;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to preview a track background.
    /// </summary>
    internal class BackgroundPreviewDrawer
    {
        private const int Zoom = 2;
        private int Width
        {
            get { return BackgroundLayout.FrontLayerWidth * Tile.Size; }
        }

        private int Height
        {
            get { return BackgroundLayout.RowCount * Tile.Size; }
        }

        private Background background;
        private Bitmap frontLayer;
        private Bitmap backLayer;

        private int x;

        public BackgroundPreviewDrawer()
        {
            this.background = Context.Game.Themes[0].Background;

            // The following members are initialized so they can be disposed of
            // in each function without having to check if they're null beforehand
            this.frontLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
            this.backLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public void LoadBackground(Background background)
        {
            this.background = background;
            this.x = 0;
            this.InitFrontLayer();
            this.InitBackLayer();
        }

        private void InitFrontLayer()
        {
            this.frontLayer.Dispose();

            this.frontLayer = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage( this.frontLayer))
            {
                for (int y = 0; y < BackgroundLayout.RowCount; y++)
                {
                    for (int x = 0; x < BackgroundLayout.FrontLayerWidth; x++)
                    {
                        Bitmap tileBitmap = this.background.GetFrontTileBitmap(x, y, true);
                        g.DrawImage(tileBitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }
        }

        private void InitBackLayer()
        {
            this.backLayer.Dispose();

            this.backLayer = new Bitmap(BackgroundLayout.BackLayerWidth * Tile.Size, this.Height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(this.backLayer))
            {
                for (int y = 0; y < BackgroundLayout.RowCount; y++)
                {
                    for (int x = 0; x < BackgroundLayout.BackLayerWidth; x++)
                    {
                        Bitmap tileBitmap = this.background.GetBackTileBitmap(x, y);
                        g.DrawImage(tileBitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }
        }

        public void DrawBackground(Graphics g)
        {
            using (Bitmap image = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.DrawImage(this.backLayer, new Point(this.x, 0));
                backBuffer.DrawImage(this.backLayer, new Point(this.x + BackgroundLayout.BackLayerWidth * Tile.Size, 0));

                backBuffer.DrawImage(this.frontLayer, new Point(this.x * 2, 0));
                backBuffer.DrawImage(this.frontLayer, new Point(this.x * 2 + this.Width, 0));

                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(image, 0, 0, this.Width * Zoom, this.Height * Zoom);
            }
        }

        public void IncrementFrame()
        {
            this.x--;

            if (this.x < 512 - (BackgroundLayout.FrontLayerWidth * Tile.Size))
            {
                this.x = 0;
            }
        }
    }
}
