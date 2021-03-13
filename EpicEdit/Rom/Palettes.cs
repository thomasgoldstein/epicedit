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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represents a collection of <see cref="Palette">palettes</see>.
    /// </summary>
    internal class Palettes : IEnumerable<Palette>, INotifyPropertyChanged
    {
        private const int PaletteCount = 16;
        private const int Size = PaletteCount * Palette.Size;

        /// <summary>
        /// Position at which sprite palettes begin.
        /// From 0 to 7: non-sprite palettes, from 8 to 15: sprite palettes.
        /// </summary>
        public const int SpritePaletteStart = 8;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The theme the palettes belong to.
        /// </summary>
        public Theme Theme { get; internal set; }

        private readonly Palette[] _palettes;

        public Palettes(byte[] data)
        {
            _palettes = new Palette[data.Length / Palette.Size];
            Init(data);
        }

        private void Init(byte[] data)
        {
            for (int i = 0; i < _palettes.Length; i++)
            {
                byte[] paletteData = GetPaletteData(data, i);
                _palettes[i] = new Palette(this, i, paletteData);
                _palettes[i].ColorChanged += palette_ColorsChanged;
                _palettes[i].ColorsChanged += palette_ColorsChanged;
            }
        }

        private void palette_ColorsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(PropertyNames.Palettes.Palette);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetBytes(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException($"Palettes data should have a size of {Size} bytes. Actual: {data.Length} bytes.", nameof(data));
            }

            int count = data.Length / Palette.Size;

            for (int i = 0; i < count; i++)
            {
                byte[] paletteData = GetPaletteData(data, i);
                _palettes[i].SetBytes(paletteData);
            }
        }

        private static byte[] GetPaletteData(byte[] data, int index)
        {
            return Utilities.ReadBlock(data, index * Palette.Size, Palette.Size);
        }

        public int Count => _palettes.Length;

        public Palette this[int index]
        {
            get => _palettes[index];
            set => _palettes[index] = value;
        }

        public RomColor BackColor => _palettes[0][0];

        public bool Modified
        {
            get
            {
                foreach (Palette palette in _palettes)
                {
                    if (palette.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[_palettes.Length * Palette.Size];

            for (int i = 0; i < _palettes.Length; i++)
            {
                Palette palette = _palettes[i];
                byte[] paletteData = palette.GetBytes();
                Buffer.BlockCopy(paletteData, 0, data, i * Palette.Size, paletteData.Length);
            }

            return data;
        }

        public void ResetModifiedState()
        {
            foreach (Palette palette in _palettes)
            {
                palette.ResetModifiedState();
            }
        }

        public IEnumerator<Palette> GetEnumerator()
        {
            foreach (Palette palette in _palettes)
            {
                yield return palette;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _palettes.GetEnumerator();
        }
    }
}
