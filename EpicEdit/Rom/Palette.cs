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

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represents a palette composed of 16 colors.
    /// </summary>
    public class Palette
    {
        private RomColor[] colors;

        public Palette()
        {
            // This is used to initialize a default palette, all black
            this.colors = new RomColor[16];
            for (int x = 0; x < 16; x++)
            {
                this.colors[x] = new RomColor();
            }
        }

        public Palette(RomColor[] palette)
        {
            if (palette.Length != 16)
            {
                throw new ArgumentException("The palette doesn't contain 16 colors.", "palette");
            }

            this.colors = palette;
        }

        public Palette(byte[] palette)
        {
            if (palette.Length != 32)
            {
                throw new ArgumentException("The palette is not 32-byte long.", "palette");
            }

            this.colors = new RomColor[16];
            for (int i = 0; i < 16; i++)
            {
                this.colors[i] = RomColor.FromBytes(palette, i * 2);
            }
        }

        public RomColor this[int index]
        {
            get { return this.colors[index]; }
            set { this.colors[index] = value; }
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[32];

            for (int i = 0; i < 16; i++)
            {
                Array.Copy(this.colors[i].GetBytes(), 0, bytes, i * 2, 2);
            }

            return bytes;
        }
    }
}
