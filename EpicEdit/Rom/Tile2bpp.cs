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
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom
{
    internal struct Tile2bppProperties
    {
        private int paletteIndex;
        public int PaletteIndex
        {
            get { return this.paletteIndex; }
            set { this.paletteIndex = value; }
        }

        private int subPaletteIndex;
        public int SubPaletteIndex
        {
            get { return this.subPaletteIndex; }
            set
            {
                if ((value & 0xC) != 0)
                {
                    throw new ArgumentOutOfRangeException("value", "The sub palette index value should be positive, a multiple of 4, and no higher than 12.");
                }
                
                this.subPaletteIndex = value;
            }
        }

        private Flip flip;
        public Flip Flip
        {
            get { return this.flip; }
            set { this.flip = value; }
        }

        public Tile2bppProperties(byte data)
        {
            byte flipMask = (byte)(Flip.X | Flip.Y);
            byte paletteData = (byte)(data &~ flipMask);

            if ((paletteData & 0x03) != 0)
            {
                throw new ArgumentOutOfRangeException("data", "Invalid 2bpp tile property data. The 2 lower bits should be 0.");
            }

            this.paletteIndex = (paletteData & 0x30) >> 4;
            this.subPaletteIndex = (paletteData & 0xC);
            this.flip = (Flip)(data & flipMask);
        }

        public byte GetByte()
        {
            return (byte)((byte)this.flip | (this.paletteIndex << 4) | this.subPaletteIndex);
        }
    }

    /// <summary>
    /// A 2-bit per pixel tile.
    /// </summary>
    internal sealed class Tile2bpp : Tile
    {
        public override Palette Palette
        {
            get { return this.palettes[this.properties.PaletteIndex]; }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.properties.PaletteIndex = value.Index;
            }
        }

        private Palettes palettes;
        public Palettes Palettes
        {
            get { return this.palettes; }
            set
            {
                if (this.palettes == value)
                {
                    return;
                }

                this.palettes = value;

                if (value != null)
                {
                    this.UpdateBitmap();
                }
            }
        }

        private Tile2bppProperties properties;

        public Tile2bpp(byte[] gfx, Palettes palettes)
        {
            this.Graphics = gfx;
            this.Palettes = palettes;
        }

        public Tile2bpp(byte[] gfx, byte properties) : this(gfx, null, properties) { }

        public Tile2bpp(byte[] gfx, Palettes palettes, byte properties) : this(gfx, palettes)
        {
            this.properties = new Tile2bppProperties(properties);
        }

        public Tile2bpp(byte[] gfx, Palettes palettes, byte properties, int paletteStart) : this(gfx, palettes, properties)
        {
            this.properties.PaletteIndex += paletteStart;
        }

        protected override void GenerateBitmap()
        {
            if (this.Palettes == null)
            {
                throw new InvalidOperationException("Cannot generate Bitmap as the Palettes have not been set.");
            }
            this.image = GraphicsConverter.GetBitmapFrom2bppPlanar(this.Graphics, this.Palettes, this.properties);
        }

        protected override void GenerateGraphics()
        {
            throw new NotImplementedException();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            if ((this.properties.Flip & Flip.X) == 0)
            {
                x = (Tile.Size - 1) - x;
            }

            if ((this.properties.Flip & Flip.Y) != 0)
            {
                y = (Tile.Size - 1) - y;
            }

            byte val1 = this.Graphics[(y * 2)];
            byte val2 = this.Graphics[(y * 2) + 1];
            int mask = 1 << x;
            int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
            return this.properties.SubPaletteIndex + colIndex;
        }
    }
}
