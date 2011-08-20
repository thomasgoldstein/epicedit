﻿#region GPL statement
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
    /// <summary>
    /// Represents a collection of <see cref="Palette">palettes</see>.
    /// </summary>
    public class Palettes
    {
        private Palette[] palettes;

        public Palettes(byte[] data)
        {
            int count = data.Length / 32;
            this.palettes = new Palette[count];

            for (int i = 0; i < count; i++)
            {
                byte[] paletteData = new byte[32];
                Buffer.BlockCopy(data, i * 32, paletteData, 0, 32);
                this.palettes[i] = new Palette(paletteData);
            }
        }

        public int Count
        {
            get
            {
                return this.palettes.Length;
            }
        }

        public Palette this[int index]
        {
            get
            {
                return this.palettes[index];
            }
            set
            {
                this.palettes[index] = value;
            }
        }
    }
}
