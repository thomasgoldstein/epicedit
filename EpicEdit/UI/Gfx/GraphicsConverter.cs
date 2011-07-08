﻿#region GPL statement
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

        public static Bitmap GetBitmapFrom4bppLinearReversed(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            Bitmap tileBitmap = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            FastBitmap fTileBitmap = new FastBitmap(tileBitmap);

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Color color1 = palette[(gfx[x + y * 4]) & 0x0F];
                    Color color2 = palette[((gfx[x + y * 4]) & 0xF0) >> 4];
                    fTileBitmap.SetPixel(x * 2, y, color1);
                    fTileBitmap.SetPixel(x * 2 + 1, y, color2);
                }
            }

            fTileBitmap.Release();
            return tileBitmap;
        }
    }
}
