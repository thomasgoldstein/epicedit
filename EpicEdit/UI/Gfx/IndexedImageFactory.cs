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
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    internal static class IndexedImageFactory
    {
        public static Image Create(ITileset tileset, int width, int height)
        {
            var bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            var bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            var is2bpp = tileset.BitsPerPixel == 2;
            bmp.Palette = is2bpp ?
                CreateIndexed4ColorPalette(bmp, tileset) :
                CreateIndexed256ColorPalette(bmp, tileset);

            var stride = Math.Abs(bmpData.Stride);

            unsafe
            {
                var scan0 = (byte*)bmpData.Scan0;

                for (var tileIndex = 0; tileIndex < tileset.Length; tileIndex++)
                {
                    var tile = tileset[tileIndex];
                    var tileGfx = CreateBitmapBytesFromTile(tile);
                    var tilePositionX = (tileIndex * Tile.Size) % width;
                    var globalColorIndex = is2bpp ? 0 : tile.Palette.Index * Palette.ColorCount;

                    for (var y = 0; y < Tile.Size; y++)
                    {
                        for (var x = 0; x < Tile.Size; x++)
                        {
                            var colorIndex = (int)tileGfx[y * Tile.Size + x];
                            if (is2bpp)
                            {
                                // The image palette will only contain 4 colors, so force the color within the first 3 colors
                                colorIndex %= 4;
                            }
                            else
                            {
                                if (colorIndex % Palette.ColorCount == 0)
                                {
                                    // Transparent color. Use the first color of the first palette for expected rendering
                                    colorIndex = 0;
                                }
                                else
                                {
                                    // Account for the color position across all palettes, since the image will contain all 16 palettes (256 colors)
                                    colorIndex += globalColorIndex;
                                }
                            }
                            scan0[tilePositionX + x] = (byte)colorIndex;
                        }
                        scan0 += stride; // Move to the next pixel row
                    }

                    if ((tilePositionX + Tile.Size) < width)
                    {
                        // The row is not finished, go back up to draw the next tile
                        scan0 -= stride * Tile.Size;
                    }
                }
            }

            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private static ColorPalette CreateIndexed4ColorPalette(Image image, ITileset tileset)
        {
            var pal = image.Palette;
            var palettes = tileset.Palettes;
            var tile = (Tile2bpp)tileset[0];
            var paletteIndex = tile.Palette.Index;
            var subPaletteIndex = tile.Properties.SubPaletteIndex;

            // Only 4 colors are needed for 2bpp tiles
            pal.Entries[0] = palettes.BackColor;
            pal.Entries[1] = palettes[paletteIndex][subPaletteIndex + 1];
            pal.Entries[2] = palettes[paletteIndex][subPaletteIndex + 2];
            pal.Entries[3] = palettes[paletteIndex][subPaletteIndex + 3];

            for (var i = 4; i < pal.Entries.Length; i++)
            {
                // Clear the rest of the palette
                pal.Entries[i] = Color.Black;
            }

            return pal;
        }

        private static ColorPalette CreateIndexed256ColorPalette(Image image, ITileset tileset)
        {
            var pal = image.Palette;
            var palettes = tileset.Palettes;

            for (var paletteIndex = 0; paletteIndex < palettes.Count; paletteIndex++)
            {
                var palette = palettes[paletteIndex];
                var globalColorIndex = paletteIndex * Palette.ColorCount;

                for (var colorIndex = 0; colorIndex < Palette.ColorCount; colorIndex++)
                {
                    var color = palette[colorIndex];
                    pal.Entries[globalColorIndex + colorIndex] = color;
                }
            }

            return pal;
        }

        private static byte[] CreateBitmapBytesFromTile(Tile tile)
        {
            var bytes = new byte[Tile.Size * Tile.Size];

            for (var y = 0; y < Tile.Size; y++)
            {
                for (var x = 0; x < Tile.Size; x++)
                {
                    bytes[y * Tile.Size + x] = (byte)tile.GetColorIndexAt(x, y);
                }
            }

            return bytes;
        }
    }
}
