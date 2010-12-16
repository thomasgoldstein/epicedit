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
    /// Class that can be used to convert images from one type to another.
    /// </summary>
    public static class GraphicsConverter
    {
        public static Bitmap GetBitmapFrom2bppPlanar(byte[] data, int start, Color[] palette, int width, int height)
        {
            // Each tile is made up of 8x8 pixels, coded on 16 bytes (2 bits per pixel)

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            int index = start;
            for (int tileY = 0; tileY < bitmap.Height; tileY += 8)
            {
                for (int tileX = 0; tileX < bitmap.Width; tileX += 8)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        byte val1 = data[index++];
                        byte val2 = data[index++];
                        for (int x = 0; x < 8; x++)
                        {
                            int mask = 1 << x;
                            int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
                            fBitmap.SetPixel(tileX + 7 - x, tileY + y, palette[colIndex]);
                        }
                    }
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        public static void GetBitmapFrom4bppLinearReversed(Bitmap[] tileBitmaps, Palette[] colorPalettes, byte[] paletteIndexes, byte[][] gfx, int start, int count)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            for (int i = 0; i < count; i++)
            {
                Bitmap tileBitmap = new Bitmap(8, 8, PixelFormat.Format32bppPArgb);
                FastBitmap fTileBitmap = new FastBitmap(tileBitmap);

                if (i < gfx.Length)
                {
                    Palette colorPalette = colorPalettes[paletteIndexes[i]];

                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            Color color1 = colorPalette[(gfx[i][x + y * 4]) & 0x0F];
                            Color color2 = colorPalette[((gfx[i][x + y * 4]) & 0xF0) >> 4];
                            fTileBitmap.SetPixel(x * 2, y, color1);
                            fTileBitmap.SetPixel(x * 2 + 1, y, color2);
                        }
                    }
                }

                fTileBitmap.Release();
                tileBitmaps[start + i] = tileBitmap;
            }
        }
    }
}
