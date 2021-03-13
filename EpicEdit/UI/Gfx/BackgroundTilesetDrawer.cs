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

        private Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                UpdateCache();
            }
        }

        private Tile2bppProperties _tileProperties;
        public Tile2bppProperties TileProperties
        {
            get => _tileProperties;
            set
            {
                value.Flip = TileFlip.None;
                _tileProperties = value;
                UpdateCache();
            }
        }

        private bool _front = true;
        public bool Front
        {
            get => _front;
            set
            {
                _front = value;
                UpdateCache();
            }
        }

        private readonly Size _imageSize;

        public Bitmap Image { get; private set; }

        public BackgroundTilesetDrawer(Size size)
        {
            _imageSize = new Size(size.Width / Zoom, size.Height / Zoom);

            // The following member is initialized so it can be disposed of
            // in each function without having to check if it's null beforehand
            Image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        private void UpdateCache()
        {
            var tileCountX = _imageSize.Width / Tile.Size;
            var tileCountY = _imageSize.Height / Tile.Size;
            var tileset = _theme.Background.Tileset;

            Image.Dispose();
            Image = new Bitmap(_imageSize.Width, _imageSize.Height, PixelFormat.Format32bppPArgb);

            using (var g = Graphics.FromImage(Image))
            {
                g.Clear(_theme.BackColor);

                for (var x = 0; x < tileCountX; x++)
                {
                    for (var y = 0; y < tileCountY; y++)
                    {
                        var tileId = x + (y * tileCountX);
                        var tile = tileset[tileId];

                        tile.Front = _front;
                        tile.Properties = TileProperties;

                        g.DrawImage(tile.Bitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }
        }

        public void DrawTileset(Graphics g, byte selectedTile)
        {
            TilesetHelper.Instance.DrawTileset(g, Image, _imageSize, Zoom, selectedTile);
        }

        public void Dispose()
        {
            Image.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
