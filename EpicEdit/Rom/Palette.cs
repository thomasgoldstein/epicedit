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

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represents a palette composed of 16 colors.
    /// </summary>
    internal class Palette
    {
        /// <summary>
        /// The number of bytes used by a palette.
        /// </summary>
        public const int Size = 32;

        /// <summary>
        /// The number of colors that compose a palette.
        /// </summary>
        public const int ColorCount = Size / RomColor.Size;

        /// <summary>
        /// Raised when a single color has been changed.
        /// </summary>
        public event EventHandler<EventArgs<int>> ColorChanged;

        /// <summary>
        /// Raised when multiple colors have been changed.
        /// </summary>
        public event EventHandler<EventArgs> ColorsChanged;

        /// <summary>
        /// Raised after the ColorChanged event has been raised and all related event handlers have been called,
        /// ensuring graphics that used this color have been updated at this point.
        /// </summary>
        public event EventHandler<EventArgs<int>> ColorGraphicsChanged;

        /// <summary>
        /// Raised after the ColorsChanged event has been raised and all related event handlers have been called,
        /// ensuring graphics that used these colors have been updated at this point.
        /// </summary>
        public event EventHandler<EventArgs> ColorsGraphicsChanged;

        /// <summary>
        /// The collection the palette belongs to.
        /// </summary>
        public Palettes Collection { get; }

        public int Index { get; }

        public Theme Theme => Collection.Theme;

        private byte[] _backupData;

        public bool Modified { get; private set; }

        private readonly RomColor[] _colors;

        public Palette(Palettes collection, int index, byte[] data)
        {
            Collection = collection;
            Index = index;

            _colors = new RomColor[ColorCount];
            _backupData = data;

            SetBytesInternal(data);

            if (Index > 0)
            {
                // Listen to the events of the first palette of the collection,
                // because the first color of the first palette (back color) is shared by all palettes
                // TODO: Consider turning RomColor into a class, make palettes share the same first RomColor,
                // and make colors raise changed events. This way, these hacks will no longer be necessary.
                Collection[0].ColorChanged += firstPalette_ColorChanged;
                Collection[0].ColorsChanged += firstPalette_ColorsChanged;
            }
        }

        private void firstPalette_ColorChanged(object sender, EventArgs<int> e)
        {
            if (e.Value == 0)
            {
                // The first color of the first palette (back color) is part of all palettes
                OnColorChanged(0);
            }
        }

        private void firstPalette_ColorsChanged(object sender, EventArgs e)
        {
            // The first color of the first palette (back color) is part of all palettes

            // NOTE: this event is raised unnecessarily when the ColorsChanged event is raised
            // for all of the palettes (which happens when importing new palettes), as it leads us
            // to raise both a ColorChanged and a ColorsChanged event for each palette after the first.
            // Implementing the TODO described in the constructor would fix this.
            OnColorChanged(0);
        }

        private void SetBytesInternal(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException($"The palette is not {Size}-byte long.", nameof(data));
            }

            for (var i = 0; i < _colors.Length; i++)
            {
                _colors[i] = GetColor(data, i);
            }
        }

        public void SetBytes(byte[] data)
        {
            SetBytesInternal(data);
            Modified = true;
            OnColorsChanged();
        }

        private static RomColor GetColor(byte[] data, int index)
        {
            return RomColor.FromBytes(data, index * RomColor.Size);
        }

        /// <summary>
        /// Resets the color at the given index.
        /// </summary>
        /// <param name="index">The color index.</param>
        public void ResetColor(int index)
        {
            this[index] = GetColor(_backupData, index);
        }

        /// <summary>
        /// Resets the palette, reloading the original data.
        /// </summary>
        public void Reset()
        {
            SetBytesInternal(_backupData);
            Modified = false;
            OnColorsChanged();
        }

        public RomColor this[int index]
        {
            get => _colors[index];
            set
            {
                if (_colors[index] == value)
                {
                    return;
                }

                _colors[index] = value;
                Modified = true;

                if (index == 0 && Index != 0)
                {
                    // The first color of each palette after the first is never used.
                    // No need to raise a ColorChanged event.
                    return;
                }

                OnColorChanged(index);
            }
        }

        private void OnColorChanged(int value)
        {
            var colorChanged = ColorChanged;
            if (colorChanged != null)
            {
                colorChanged(this, new EventArgs<int>(value));
                OnColorGraphicsChanged(value);
            }
        }

        private void OnColorsChanged()
        {
            var colorsChanged = ColorsChanged;
            if (colorsChanged != null)
            {
                colorsChanged(this, EventArgs.Empty);
                OnColorsGraphicsChanged();
            }
        }

        private void OnColorGraphicsChanged(int value)
        {
            ColorGraphicsChanged?.Invoke(this, new EventArgs<int>(value));
        }

        private void OnColorsGraphicsChanged()
        {
            ColorsGraphicsChanged?.Invoke(this, EventArgs.Empty);
        }

        public byte[] GetBytes()
        {
            var data = new byte[Size];

            for (var i = 0; i < _colors.Length; i++)
            {
                Buffer.BlockCopy(_colors[i].GetBytes(), 0, data, i * RomColor.Size, RomColor.Size);
            }

            return data;
        }

        public void ResetModifiedState()
        {
            // Update the backup data, so that resetting the data will reload the last saved data
            _backupData = GetBytes();
            Modified = false;
        }
    }
}
