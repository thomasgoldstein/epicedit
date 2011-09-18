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
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Class that can be used to convert images from one type to another.
    /// </summary>
    public static class GraphicsConverter
    {
        public static Bitmap GetBitmapFrom2bppPlanar(byte[] gfx, int gfxIndex, Palette palette, int paletteIndex, int width, int height)
        {
            // Each tile is made up of 8x8 pixels, coded on 16 bytes (2 bits per pixel)

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            for (int tileY = 0; tileY < height; tileY += Tile.Size)
            {
                for (int tileX = 0; tileX < width; tileX += Tile.Size)
                {
                    for (int y = 0; y < Tile.Size; y++)
                    {
                        byte val1 = gfx[gfxIndex++];
                        byte val2 = gfx[gfxIndex++];
                        for (int x = 0; x < Tile.Size; x++)
                        {
                            int mask = 1 << x;
                            int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
                            Color color = palette[paletteIndex + colIndex];
                            fBitmap.SetPixel(tileX + (Tile.Size - 1) - x, tileY + y, color);
                        }
                    }
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        public static Bitmap GetBitmapFrom4bppPlanarComposite(byte[] gfx, Palette palette)
        {
            return GraphicsConverter.GetBitmapFrom4bppPlanarComposite(gfx, 0, palette);
        }

        public static Bitmap GetBitmapFrom4bppPlanarComposite(byte[] gfx, int gfxIndex, Palette palette)
        {
            Bitmap bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            GraphicsConverter.SetBitmapFrom4bppPlanarComposite(bitmap, gfx, gfxIndex, palette, 0, 0);
            return bitmap;
        }

        internal static void SetBitmapFrom4bppPlanarComposite(Bitmap bitmap, byte[] gfx, int gfxIndex, Palette palette, int startX, int startY)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)
            // NOTE: We use a Bitmap rather than a FastBitmap, because FastBitmap does not support transparency

            for (int y = 0; y < 8; y++)
            {
                int val1 = gfx[gfxIndex + y * 2];
                int val2 = gfx[gfxIndex + y * 2 + 1];
                int val3 = gfx[gfxIndex + y * 2 + 16];
                int val4 = gfx[gfxIndex + y * 2 + 17];

                for (int x = 0; x < 8; x++)
                {
                    int mask = 1 << x;
                    int val1b = ((val1 & mask) >> x);
                    int val2b = (((val2 & mask) << 1) >> x);
                    int val3b = (((val3 & mask) << 2) >> x);
                    int val4b = (((val4 & mask) << 3) >> x);
                    int colIndex = val1b + val2b + val3b + val4b;
                    Color color = colIndex == 0 ? Color.Transparent : palette[colIndex].Color;
                    bitmap.SetPixel(startX + (Tile.Size - 1) - x, startY + y, color);
                }
            }
        }

        public static Bitmap GetBitmapFrom4bppLinearReversed(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            Bitmap bitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 8; y++)
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
