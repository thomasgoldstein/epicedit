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
        /// <summary>
        /// The number of bytes that compose a palette.
        /// </summary>
        public const int Size = 32;

        /// <summary>
        /// The number of colors that compose a palette.
        /// </summary>
        public const int ColorCount = Palette.Size / RomColor.Size;

        private byte[] backupData;

        public bool Modified { get; set; }

        private RomColor[] colors;

        public Palette()
        {
            // This is used to initialize a default palette, all black
            this.colors = new RomColor[Palette.ColorCount];
            for (int x = 0; x < this.colors.Length; x++)
            {
                this.colors[x] = new RomColor();
            }
        }

        public Palette(RomColor[] palette)
        {
            if (palette.Length != Palette.ColorCount)
            {
                throw new ArgumentException("The palette doesn't contain " + Palette.ColorCount + " colors.", "palette");
            }

            this.colors = palette;
        }

        public Palette(byte[] data)
        {
            this.colors = new RomColor[Palette.ColorCount];
            this.backupData = data;

            this.Load(data);
        }

        private void Load(byte[] data)
        {
            if (data.Length != Palette.Size)
            {
                throw new ArgumentException("The palette is not " + Palette.Size + "-byte long.", "data");
            }

            for (int i = 0; i < this.colors.Length; i++)
            {
                this.LoadColor(data, i);
            }
        }

        private void LoadColor(byte[] data, int index)
        {
            this.colors[index] = RomColor.FromBytes(data, index * RomColor.Size);
        }

        /// <summary>
        /// Resets the color at the given index.
        /// </summary>
        /// <param name="index">The color index.</param>
        public void ResetColor(int index)
        {
            this.LoadColor(this.backupData, index);
        }

        /// <summary>
        /// Resets the palette, reloading the original data.
        /// </summary>
        public void Reset()
        {
            this.Load(this.backupData);

            this.Modified = false;
        }

        public RomColor this[int index]
        {
            get { return this.colors[index]; }
            set { this.colors[index] = value; }
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[Palette.Size];

            for (int i = 0; i < this.colors.Length; i++)
            {
                Buffer.BlockCopy(this.colors[i].GetBytes(), 0, bytes, i * RomColor.Size, RomColor.Size);
            }

            return bytes;
        }
    }
}
