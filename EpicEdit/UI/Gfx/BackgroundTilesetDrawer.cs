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
    internal sealed class BackgroundTilesetDrawer : IDisposable
    {
        public const int Zoom = 2;

        private Theme theme;
        public Theme Theme
        {
            get { return this.theme; }
            set
            {
                this.theme = value;
                this.UpdateCache();
            }
        }

        private Size imageSize;

        private Bitmap tilesetCache;
        private Pen tilesetPen;

        public BackgroundTilesetDrawer(Size size)
        {
            this.imageSize = new Size(size.Width / Zoom, size.Height / Zoom);

            this.tilesetPen = new Pen(Color.FromArgb(150, 255, 0, 0));

            // The following member is initialized so it can be disposed of
            // in each function without having to check if it's null beforehand
            this.tilesetCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        private void UpdateCache()
        {
            this.tilesetCache.Dispose();

            int tileCountX = this.imageSize.Width / Tile.Size;
            int tileCountY = this.imageSize.Height / Tile.Size;
            BackgroundTileset tileset = this.theme.Background.Tileset;

            this.tilesetCache = new Bitmap(this.imageSize.Width, this.imageSize.Height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(this.tilesetCache))
            {
                g.Clear(this.theme.BackColor);

                for (int x = 0; x < tileCountX; x++)
                {
                    for (int y = 0; y < tileCountY; y++)
                    {
                        int tileId = x + (y * tileCountX);
                        if (tileId >= tileset.TileCount) break;
                        g.DrawImage(tileset[tileId].Bitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }
        }

        public void DrawTileset(Graphics g, byte selectedTile)
        {
            using (Bitmap image = new Bitmap(this.imageSize.Width, this.imageSize.Height, PixelFormat.Format32bppPArgb))
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

                using (Graphics backBuffer = Graphics.FromImage(image))
                {
                    backBuffer.DrawImage(this.tilesetCache, 0, 0,
                                         this.imageSize.Width,
                                         this.imageSize.Height);

                    int xTileCount = this.imageSize.Width / Tile.Size;
                    int tilePosX = selectedTile % xTileCount;
                    int tilePosY = selectedTile / xTileCount;
                    Point selectedTilePosition = new Point(tilePosX, tilePosY);

                    backBuffer.DrawRectangle(this.tilesetPen,
                                             selectedTilePosition.X * Tile.Size,
                                             selectedTilePosition.Y * Tile.Size,
                                             Tile.Size - 1,
                                             Tile.Size - 1);
                }
                g.DrawImage(image, 0, 0,
                            this.imageSize.Width * Zoom,
                            this.imageSize.Height * Zoom);
            }
        }

        public void Dispose()
        {
            this.tilesetCache.Dispose();
            this.tilesetPen.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
