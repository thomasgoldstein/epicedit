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

namespace EpicEdit.Rom
{
    /// <summary>
    /// A 2-bit per pixel tile.
    /// </summary>
    internal class Tile2bpp : Tile
    {
        protected void SetPalette()
        {
            if (base.Palette != this.Palette)
            {
                // Setting the base Palette lets us listen to the palette color change events,
                // and will update the Bitmap if the Palette has changed.
                base.Palette = this.Palette;
            }
            else
            {
                // Force the update of the bitmap. This is necessary because even though
                // the palette is the same, Tile2bppProperties changes still affect the tile image.
                this.UpdateBitmap();
            }
        }

        public override Palette Palette
        {
            get => this.palettes?[this.Properties.PaletteIndex];
            set
            {
                if (value == null)
                {
                    return;
                }

                this.Properties = new Tile2bppProperties
                {
                    PaletteIndex = value.Index,
                    SubPaletteIndex = this.Properties.SubPaletteIndex,
                    Flip = this.Properties.Flip
                };

                base.Palette = this.Palette;
            }
        }

        private Palettes palettes;
        public Palettes Palettes
        {
            get => this.palettes;
            set
            {
                if (this.palettes == value)
                {
                    return;
                }

                this.palettes = value;
                this.SetPalette();
            }
        }

        private Tile2bppProperties properties;
        public virtual Tile2bppProperties Properties
        {
            get => this.properties;
            set
            {
                if (this.properties == value)
                {
                    return;
                }

                this.properties = value;
                this.SetPalette();
            }
        }

        private RomColor[] GetSubPalette()
        {
            Palette palette = this.Palette;
            int subPalIndex = this.Properties.SubPaletteIndex;

            return new[]
            {
                this.Palettes.BackColor,
                palette[subPalIndex + 1],
                palette[subPalIndex + 2],
                palette[subPalIndex + 3]
            };
        }

        public Tile2bpp(byte[] gfx, Palettes palettes) : this(gfx, palettes, 0) { }

        public Tile2bpp(byte[] gfx, byte properties) : this(gfx, null, properties) { }

        public Tile2bpp(byte[] gfx, Palettes palettes, byte properties)
        {
            this.Graphics = gfx;
            this.Properties = new Tile2bppProperties(properties);
            this.Palettes = palettes;
        }

        protected override void GenerateBitmap()
        {
            this.bitmap = GraphicsConverter.GetBitmapFrom2bppPlanar(this.Graphics, this.Palettes, this.Properties);
        }

        protected override void GenerateGraphics()
        {
            FastBitmap fBitmap = new FastBitmap(this.bitmap);
            RomColor[] palette = this.GetSubPalette();

            for (int y = 0; y < Tile.Size; y++)
            {
                byte val1 = 0;
                byte val2 = 0;
                for (int x = 0; x < Tile.Size; x++)
                {
                    int xPos = (Tile.Size - 1) - x;
                    RomColor color = (RomColor)fBitmap.GetPixel(xPos, y);
                    int colorIndex = Utilities.GetColorIndex(color, palette);
                    val1 |= (byte)((colorIndex & 0x01) << x);
                    val2 |= (byte)(((colorIndex & 0x02) << x) >> 1);
                }

                this.Graphics[y * 2] = val1;
                this.Graphics[y * 2 + 1] = val2;
            }

            fBitmap.Release();

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            this.UpdateBitmap();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            if ((this.Properties.Flip & Flip.X) == 0)
            {
                x = (Tile.Size - 1) - x;
            }

            if ((this.Properties.Flip & Flip.Y) != 0)
            {
                y = (Tile.Size - 1) - y;
            }

            byte val1 = this.Graphics[y * 2];
            byte val2 = this.Graphics[y * 2 + 1];
            int mask = 1 << x;
            int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
            return this.Properties.SubPaletteIndex + colIndex;
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
