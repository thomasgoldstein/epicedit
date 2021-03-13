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
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Converts various SNES graphics types to Bitmaps.
    /// </summary>
    internal static class GraphicsConverter
    {
        public static Bitmap GetBitmapFrom2bppPlanar(byte[] gfx, Palettes palettes, Tile2bppProperties properties)
        {
            // Each tile is made up of 8x8 pixels, coded on 16 bytes (2 bits per pixel)

            var palette = palettes[properties.PaletteIndex];
            var subPalIndex = properties.SubPaletteIndex;
            var flip = properties.Flip;
            var bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Tile.Size; y++)
            {
                var val1 = gfx[y * 2];
                var val2 = gfx[y * 2 + 1];
                for (var x = 0; x < Tile.Size; x++)
                {
                    var mask = 1 << x;
                    var colorIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);

                    if (colorIndex > 0) // If pixel is not transparent
                    {
                        var xPos = (flip & TileFlip.X) != 0 ?
                            x : (Tile.Size - 1) - x;

                        var yPos = (flip & TileFlip.Y) == 0 ?
                            y : (Tile.Size - 1) - y;

                        Color color = palette[subPalIndex + colorIndex];

                        fBitmap.SetPixel(xPos, yPos, color);
                    }
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        public static Bitmap GetBitmapFrom4bppPlanarComposite(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            var bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Tile.Size; y++)
            {
                int val1 = gfx[y * 2];
                int val2 = gfx[y * 2 + 1];
                int val3 = gfx[y * 2 + 16];
                int val4 = gfx[y * 2 + 17];

                for (var x = 0; x < Tile.Size; x++)
                {
                    var mask = 1 << x;
                    var val1b = ((val1 & mask) >> x);
                    var val2b = (((val2 & mask) << 1) >> x);
                    var val3b = (((val3 & mask) << 2) >> x);
                    var val4b = (((val4 & mask) << 3) >> x);
                    var colorIndex = val1b + val2b + val3b + val4b;

                    if (colorIndex > 0) // If pixel is not transparent
                    {
                        var color = palette[colorIndex].Color;
                        fBitmap.SetPixel((Tile.Size - 1) - x, y, color);
                    }
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        public static Bitmap GetBitmapFrom4bppLinearReversed(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            var bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var x = 0; x < Tile.Size / 2; x++)
            {
                for (var y = 0; y < Tile.Size; y++)
                {
                    Color color1 = palette[gfx[x + y * 4] & 0x0F];
                    Color color2 = palette[(gfx[x + y * 4] & 0xF0) >> 4];
                    fBitmap.SetPixel(x * 2, y, color1);
                    fBitmap.SetPixel(x * 2 + 1, y, color2);
                }
            }

            fBitmap.Release();
            return bitmap;
        }
    }
}
