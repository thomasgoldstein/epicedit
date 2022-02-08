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

using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace EpicEdit.Rom
{
    /// <summary>
    /// A 2-bit per pixel tile.
    /// </summary>
    internal class Tile2bpp : Tile
    {
        protected void SetPalette()
        {
            if (base.Palette != Palette)
            {
                // Setting the base Palette lets us listen to the palette color change events,
                // and will update the Bitmap if the Palette has changed.
                base.Palette = Palette;
            }
            else
            {
                // Force the update of the bitmap. This is necessary because even though
                // the palette is the same, Tile2bppProperties changes still affect the tile image.
                UpdateBitmap();
            }
        }

        public override Palette Palette
        {
            get => _palettes?[Properties.PaletteIndex];
            set
            {
                if (value == null)
                {
                    return;
                }

                Properties = new Tile2bppProperties
                {
                    PaletteIndex = value.Index,
                    SubPaletteIndex = Properties.SubPaletteIndex,
                    Flip = Properties.Flip
                };

                base.Palette = Palette;
            }
        }

        private Palettes _palettes;
        public Palettes Palettes
        {
            get => _palettes;
            set
            {
                if (_palettes == value)
                {
                    return;
                }

                _palettes = value;
                SetPalette();
            }
        }

        private Tile2bppProperties _properties;
        public virtual Tile2bppProperties Properties
        {
            get => _properties;
            set
            {
                if (_properties == value)
                {
                    return;
                }

                _properties = value;
                SetPalette();
            }
        }

        private RomColor[] GetSubPalette()
        {
            var palette = Palette;
            var subPalIndex = Properties.SubPaletteIndex;

            return new[]
            {
                Palettes.BackColor,
                palette[subPalIndex + 1],
                palette[subPalIndex + 2],
                palette[subPalIndex + 3]
            };
        }

        public Tile2bpp(byte[] gfx, Palettes palettes) : this(gfx, palettes, 0) { }

        public Tile2bpp(byte[] gfx, byte properties) : this(gfx, null, properties) { }

        public Tile2bpp(byte[] gfx, Palettes palettes, byte properties)
        {
            Graphics = gfx;
            Properties = new Tile2bppProperties(properties);
            Palettes = palettes;
        }

        protected override void GenerateBitmap()
        {
            // Format: 2bpp planar
            // Each tile is made up of 8x8 pixels, coded on 16 bytes (2 bits per pixel)

            var palette = Palettes[Properties.PaletteIndex];
            var bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var colorIndex = GetColorIndexAt(x, y);
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
            InternalBitmap = bitmap;
        }

        protected override void GenerateGraphics()
        {
            if (InternalBitmap.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                GenerateGraphicsFromIndexedImage();
            }
            else
            {
                GenerateGraphicsFromRgbImage();
            }
        }

        private void GenerateGraphicsFromIndexedImage()
        {
            var bmpBytes = new byte[Size * Size];
            var bmpData = InternalBitmap.LockBits(new Rectangle(0, 0, Size, Size), ImageLockMode.ReadOnly, InternalBitmap.PixelFormat);
            Marshal.Copy(bmpData.Scan0, bmpBytes, 0, bmpBytes.Length);
            InternalBitmap.UnlockBits(bmpData);

            for (var y = 0; y < Size; y++)
            {
                byte val1 = 0;
                byte val2 = 0;
                for (var x = 0; x < Size; x++)
                {
                    var xPos = (Size - 1) - x;
                    var colorIndex = bmpBytes[y * Size + xPos] % Palette.ColorCount;
                    val1 |= (byte)((colorIndex & 0x01) << x);
                    val2 |= (byte)(((colorIndex & 0x02) << x) >> 1);
                }

                Graphics[y * 2] = val1;
                Graphics[y * 2 + 1] = val2;
            }

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            UpdateBitmap();
        }

        private void GenerateGraphicsFromRgbImage()
        {
            var fBitmap = new FastBitmap(InternalBitmap);
            var palette = GetSubPalette();

            for (var y = 0; y < Size; y++)
            {
                byte val1 = 0;
                byte val2 = 0;
                for (var x = 0; x < Size; x++)
                {
                    var xPos = (Size - 1) - x;
                    RomColor color = fBitmap.GetPixel(xPos, y);
                    var colorIndex = Utilities.GetColorIndex(color, palette);
                    val1 |= (byte)((colorIndex & 0x01) << x);
                    val2 |= (byte)(((colorIndex & 0x02) << x) >> 1);
                }

                Graphics[y * 2] = val1;
                Graphics[y * 2 + 1] = val2;
            }

            fBitmap.Release();

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            UpdateBitmap();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            if ((Properties.Flip & TileFlip.X) == 0)
            {
                x = (Size - 1) - x;
            }

            if ((Properties.Flip & TileFlip.Y) != 0)
            {
                y = (Size - 1) - y;
            }

            var val1 = Graphics[y * 2];
            var val2 = Graphics[y * 2 + 1];
            var mask = 1 << x;
            var colorIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
            return Properties.SubPaletteIndex + colorIndex;
        }

        public override bool Contains(int colorIndex)
        {
            // Tile2bpp instances have transparent pixels where the color 0 is,
            // so consider they don't contain it. This lets us avoid unnecessarily recreating
            // the tile image when the color 0 is changed.
            return colorIndex != 0 && base.Contains(colorIndex);
        }
    }
}
