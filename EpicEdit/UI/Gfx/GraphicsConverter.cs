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
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Road;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Converts various SNES graphics types to Bitmaps.
    /// </summary>
    internal static class GraphicsConverter
    {
        public static Bitmap CreateBitmapFrom2bppPlanar(byte[] gfx, Palettes palettes, Tile2bppProperties properties)
        {
            // Each tile is made up of 8x8 pixels, coded on 16 bytes (2 bits per pixel)

            var palette = palettes[properties.PaletteIndex];
            var bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Tile.Size; y++)
            {
                for (var x = 0; x < Tile.Size; x++)
                {
                    var colorIndex = Tile2bpp.GetColorIndexAt(gfx, properties, x, y);
                    if ((colorIndex % 4) == 0)
                    {
                        // Pixel is transparent
                        continue;
                    }

                    var color = palette[colorIndex];
                    fBitmap.SetPixel(x, y, color);
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        public static Bitmap CreateBitmapFrom4bppPlanarComposite(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            var bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Tile.Size; y++)
            {
                for (var x = 0; x < Tile.Size; x++)
                {
                    var colorIndex = TrackObjectTile.GetColorIndexAt(gfx, x, y);
                    if (colorIndex == 0)
                    {
                        // Pixel is transparent
                        continue;
                    }

                    var color = palette[colorIndex];
                    fBitmap.SetPixel(x, y, color);
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        public static Bitmap CreateBitmapFrom4bppLinearReversed(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            var bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Tile.Size; y++)
            {
                for (var x = 0; x < Tile.Size; x++)
                {
                    var colorIndex = RoadTile.GetColorIndexAt(gfx, x, y);
                    var color = palette[colorIndex];
                    fBitmap.SetPixel(x, y, color);
                }
            }

            fBitmap.Release();
            return bitmap;
        }
    }
}
