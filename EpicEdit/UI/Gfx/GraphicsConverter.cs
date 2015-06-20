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
using System.Drawing.Imaging;

using EpicEdit.Rom;

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

            Palette palette = palettes[properties.PaletteIndex];
            int subPalIndex = properties.SubPaletteIndex;
            Flip flip = properties.Flip;
            Bitmap bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            for (int y = 0; y < Tile.Size; y++)
            {
                byte val1 = gfx[y * 2];
                byte val2 = gfx[y * 2 + 1];
                for (int x = 0; x < Tile.Size; x++)
                {
                    int mask = 1 << x;
                    int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);

                    if (colIndex > 0) // If pixel is not transparent
                    {
                        int xPos = (flip & Flip.X) != 0 ?
                            x : (Tile.Size - 1) - x;

                        int yPos = (flip & Flip.Y) == 0 ?
                            y : (Tile.Size - 1) - y;

                        Color color = palette[subPalIndex + colIndex];

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

            Bitmap bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            for (int y = 0; y < Tile.Size; y++)
            {
                int val1 = gfx[y * 2];
                int val2 = gfx[y * 2 + 1];
                int val3 = gfx[y * 2 + 16];
                int val4 = gfx[y * 2 + 17];

                for (int x = 0; x < Tile.Size; x++)
                {
                    int mask = 1 << x;
                    int val1b = ((val1 & mask) >> x);
                    int val2b = (((val2 & mask) << 1) >> x);
                    int val3b = (((val3 & mask) << 2) >> x);
                    int val4b = (((val4 & mask) << 3) >> x);
                    int colIndex = val1b + val2b + val3b + val4b;

                    if (colIndex > 0)
                    {
                        Color color = palette[colIndex].Color;
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

            Bitmap bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            for (int x = 0; x < Tile.Size / 2; x++)
            {
                for (int y = 0; y < Tile.Size; y++)
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
