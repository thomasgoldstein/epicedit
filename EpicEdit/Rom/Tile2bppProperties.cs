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

namespace EpicEdit.Rom
{
    [Flags]
    internal enum Flip : byte
    {
        None = 0x00,
        X = 0x40,
        Y = 0x80
    }

    internal struct Tile2bppProperties
    {
        private int paletteIndex;
        public int PaletteIndex
        {
            get { return this.paletteIndex; }
            set
            {
                if (value < 0 || value > 15)
                {
                    throw new ArgumentOutOfRangeException("value", "The palette index value should be between 0 and 15.");
                }

                this.paletteIndex = value;
            }
        }

        private int subPaletteIndex;
        public int SubPaletteIndex
        {
            get { return this.subPaletteIndex; }
            set
            {
                if (value != 0 && value != 4 && value != 8 && value != 12)
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

        public void FlipX()
        {
            this.FlipXY(Flip.X);
        }

        public void FlipY()
        {
            this.FlipXY(Flip.Y);
        }

        private void FlipXY(Flip value)
        {
            if ((this.flip & value) != 0)
            {
                this.flip ^= value;
            }
            else
            {
                this.flip |= value;
            }
        }

        public static bool operator ==(Tile2bppProperties left, Tile2bppProperties right)
        {
            return left.GetByte() == right.GetByte();
        }

        public static bool operator !=(Tile2bppProperties left, Tile2bppProperties right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is Tile2bppProperties && this == (Tile2bppProperties)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public byte GetByte()
        {
            return (byte)((byte)this.flip | (this.paletteIndex << 4) | this.subPaletteIndex);
        }
    }
}
