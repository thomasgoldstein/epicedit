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
        private int _paletteIndex;
        public int PaletteIndex
        {
            get => _paletteIndex;
            set
            {
                if (value < 0 || value > 15)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The palette index value should be between 0 and 15.");
                }

                _paletteIndex = value;
            }
        }

        private int _subPaletteIndex;
        public int SubPaletteIndex
        {
            get => _subPaletteIndex;
            set
            {
                if (value != 0 && value != 4 && value != 8 && value != 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "The sub palette index value should be positive, a multiple of 4, and no higher than 12.");
                }

                _subPaletteIndex = value;
            }
        }

        public TileFlip Flip { get; set; }

        public Tile2bppProperties(byte data)
        {
            const byte flipMask = (byte)(TileFlip.X | TileFlip.Y);
            var paletteData = (byte)(data & ~flipMask);

            if ((paletteData & 0x03) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "Invalid 2bpp tile property data. The 2 lower bits should be 0.");
            }

            _paletteIndex = (paletteData & 0x30) >> 4;
            _subPaletteIndex = (paletteData & 0xC);
            Flip = (TileFlip)(data & flipMask);
        }

        public void FlipX()
        {
            FlipXy(TileFlip.X);
        }

        public void FlipY()
        {
            FlipXy(TileFlip.Y);
        }

        private void FlipXy(TileFlip value)
        {
            if ((Flip & value) != 0)
            {
                Flip ^= value;
            }
            else
            {
                Flip |= value;
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
            return (byte)((byte)Flip | (_paletteIndex << 4) | _subPaletteIndex);
        }
    }
}
