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
using EpicEdit.Rom.Tracks.Road;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of a road tileset.
    /// </summary>
    internal sealed class RoadTilesetDrawer : IDisposable
    {
        public const int Zoom = 2;

        private RoadTileset _tileset;

        private Bitmap _image;

        public Size ImageSize { get; }

        public RoadTilesetDrawer(Size size)
        {
            ImageSize = new Size(size.Width / Zoom, size.Height / Zoom);

            // The following member is initialized so it can be disposed of
            // in each function without having to check if it's null beforehand
            _image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public RoadTileset Tileset
        {
            //get => _tileset;
            set
            {
                _tileset = value;
                UpdateCache();
            }
        }

        private void UpdateCache()
        {
            var tileCountX = ImageSize.Width / Tile.Size;
            var tileCountY = ImageSize.Height / Tile.Size;

            _image.Dispose();
            _image = new Bitmap(ImageSize.Width, ImageSize.Height, PixelFormat.Format32bppPArgb);

            using (var g = Graphics.FromImage(_image))
            {
                for (var x = 0; x < tileCountX; x++)
                {
                    for (var y = 0; y < tileCountY; y++)
                    {
                        Tile tile = _tileset[x + (y * tileCountX)];
                        g.DrawImage(tile.Bitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }
        }

        public void DrawTileset(Graphics g, byte selectedTile)
        {
            TilesetHelper.Instance.DrawTileset(g, _image, ImageSize, Zoom, selectedTile);
        }

        public void Dispose()
        {
            _image.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
