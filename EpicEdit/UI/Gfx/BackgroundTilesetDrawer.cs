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
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    internal sealed class BackgroundTilesetDrawer : IDisposable
    {
        public const int Zoom = 2;

        private Theme theme;
        public Theme Theme
        {
            get => this.theme;
            set
            {
                this.theme = value;
                this.UpdateCache();
            }
        }

        private Tile2bppProperties tileProperties;
        public Tile2bppProperties TileProperties
        {
            get => this.tileProperties;
            set
            {
                value.Flip = TileFlip.None;
                this.tileProperties = value;
                this.UpdateCache();
            }
        }

        private bool front = true;
        public bool Front
        {
            get => this.front;
            set
            {
                this.front = value;
                this.UpdateCache();
            }
        }

        private readonly Size imageSize;

        private Bitmap tilesetCache;

        public Bitmap Image => this.tilesetCache;

        public BackgroundTilesetDrawer(Size size)
        {
            this.imageSize = new Size(size.Width / Zoom, size.Height / Zoom);

            // The following member is initialized so it can be disposed of
            // in each function without having to check if it's null beforehand
            this.tilesetCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        private void UpdateCache()
        {
            int tileCountX = this.imageSize.Width / Tile.Size;
            int tileCountY = this.imageSize.Height / Tile.Size;
            BackgroundTileset tileset = this.theme.Background.Tileset;

            this.tilesetCache.Dispose();
            this.tilesetCache = new Bitmap(this.imageSize.Width, this.imageSize.Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(this.tilesetCache))
            {
                g.Clear(this.theme.BackColor);

                for (int x = 0; x < tileCountX; x++)
                {
                    for (int y = 0; y < tileCountY; y++)
                    {
                        int tileId = x + (y * tileCountX);
                        BackgroundTile tile = tileset[tileId];

                        tile.Front = this.front;
                        tile.Properties = this.TileProperties;

                        g.DrawImage(tile.Bitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }
        }

        public void DrawTileset(Graphics g, byte selectedTile)
        {
            TilesetHelper.Instance.DrawTileset(g, this.tilesetCache, this.imageSize, Zoom, selectedTile);
        }

        public void Dispose()
        {
            this.tilesetCache.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
