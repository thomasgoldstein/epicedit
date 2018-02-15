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
    internal struct Tile2bppProperties
    {
        private int paletteIndex;
        public int PaletteIndex
        {
            get => this.paletteIndex;
            set
            {
                if (value < 0 || value > 15)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The palette index value should be between 0 and 15.");
                }

                this.paletteIndex = value;
            }
        }

        private int subPaletteIndex;
        public int SubPaletteIndex
        {
            get => this.subPaletteIndex;
            set
            {
                if (value != 0 && value != 4 && value != 8 && value != 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The sub palette index value should be positive, a multiple of 4, and no higher than 12.");
                }

                this.subPaletteIndex = value;
            }
        }

        private TileFlip flip;
        public TileFlip Flip
        {
            get => this.flip;
            set => this.flip = value;
        }

        public Tile2bppProperties(byte data)
        {
            const byte FlipMask = (byte)(TileFlip.X | TileFlip.Y);
            byte paletteData = (byte)(data & ~FlipMask);

            if ((paletteData & 0x03) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Invalid 2bpp tile property data. The 2 lower bits should be 0.");
            }

            this.paletteIndex = (paletteData & 0x30) >> 4;
            this.subPaletteIndex = (paletteData & 0xC);
            this.flip = (TileFlip)(data & FlipMask);
        }

        public void FlipX()
        {
            this.FlipXY(TileFlip.X);
        }

        public void FlipY()
        {
            this.FlipXY(TileFlip.Y);
        }

        private void FlipXY(TileFlip value)
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
