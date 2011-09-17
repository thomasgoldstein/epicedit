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
using System.Collections;
using System.Collections.Generic;

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represents a collection of <see cref="Palette">palettes</see>.
    /// </summary>
    public class Palettes : IEnumerable<Palette>
    {
        /// <summary>
        /// Position at which sprite palettes begin.
        /// From 0 to 7: non-sprite palettes, from 8 to 15: sprite palettes.
        /// </summary>
        public const int SpritePaletteStart = 8;

        private Palette[] palettes;

        public Palettes(byte[] data)
        {
            int count = data.Length / Palette.Size;
            this.palettes = new Palette[count];

            for (int i = 0; i < count; i++)
            {
                byte[] paletteData = new byte[Palette.Size];
                Buffer.BlockCopy(data, i * Palette.Size, paletteData, 0, Palette.Size);
                this.palettes[i] = new Palette(paletteData);
            }
        }

        public int Count
        {
            get { return this.palettes.Length; }
        }

        public Palette this[int index]
        {
            get { return this.palettes[index]; }
            set { this.palettes[index] = value; }
        }

        public int IndexOf(Palette palette)
        {
            for (int i = 0; i < this.palettes.Length; i++)
            {
                if (palette == this.palettes[i])
                {
                    return i;
                }
            }

            return -1;
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[this.palettes.Length * Palette.Size];

            for (int i = 0; i < this.palettes.Length; i++)
            {
                Palette palette = this.palettes[i];
                byte[] paletteData = palette.GetBytes();
                Buffer.BlockCopy(paletteData, 0, data, i * Palette.Size, paletteData.Length);
            }

            return data;
        }

        public IEnumerator<Palette> GetEnumerator()
        {
            foreach (Palette palette in this.palettes)
            {
                yield return palette;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.palettes.GetEnumerator();
        }
    }
}
