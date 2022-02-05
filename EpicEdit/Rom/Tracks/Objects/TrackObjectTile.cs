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

using EpicEdit.UI.Gfx;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// Represents a track object tile.
    /// </summary>
    internal sealed class TrackObjectTile : Tile
    {
        public TrackObjectTile(byte[] gfx)
        {
            Graphics = gfx;
        }

        protected override void GenerateBitmap()
        {
            InternalBitmap = CreateBitmapFrom4bppPlanarComposite(Graphics, Palette);
        }

        private static Bitmap CreateBitmapFrom4bppPlanarComposite(byte[] gfx, Palette palette)
        {
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            var bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var colorIndex = GetColorIndexAt(gfx, x, y);
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

        protected override void GenerateGraphics()
        {
            throw new NotImplementedException();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            return GetColorIndexAt(Graphics, x, y);
        }

        public static int GetColorIndexAt(byte[] gfx, int x, int y)
        {
            x = (Size - 1) - x;
            int val1 = gfx[y * 2];
            int val2 = gfx[y * 2 + 1];
            int val3 = gfx[y * 2 + 16];
            int val4 = gfx[y * 2 + 17];
            var mask = 1 << x;
            var val1b = ((val1 & mask) >> x);
            var val2b = (((val2 & mask) << 1) >> x);
            var val3b = (((val3 & mask) << 2) >> x);
            var val4b = (((val4 & mask) << 3) >> x);
            return val1b + val2b + val3b + val4b;
        }

        public override bool Contains(int colorIndex)
        {
            // TrackObjectTile instances have transparent pixels where the color 0 is,
            // so consider they don't contain it. This lets us avoid unnecessarily recreating
            // the tile image when the color 0 is changed.
            return colorIndex != 0 && base.Contains(colorIndex);
        }
    }
}
