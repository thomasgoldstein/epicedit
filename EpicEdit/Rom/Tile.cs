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

using EpicEdit.Rom.Utility;
using System;
using System.ComponentModel;
using System.Drawing;

namespace EpicEdit.Rom
{
    /// <summary>
    /// An 8x8 graphic tile.
    /// </summary>
    internal abstract class Tile : IDisposable, INotifyPropertyChanged
    {
        public const int Size = 8;

        public event PropertyChangedEventHandler PropertyChanged;

        private Palette palette;
        public virtual Palette Palette
        {
            get => palette;
            set
            {
                if (palette == value)
                {
                    return;
                }

                if (palette != null)
                {
                    palette.ColorChanged -= palette_ColorChanged;
                    palette.ColorsChanged -= palette_ColorsChanged;
                }

                palette = value;

                palette.ColorChanged += palette_ColorChanged;
                palette.ColorsChanged += palette_ColorsChanged;

                UpdateBitmap();
                OnPropertyChanged(PropertyNames.Tile.Palette);
            }
        }

        private void palette_ColorChanged(object sender, EventArgs<int> e)
        {
            if (Contains(e.Value))
            {
                UpdateBitmap();
            }
        }

        private void palette_ColorsChanged(object sender, EventArgs e)
        {
            UpdateBitmap();
        }

        private byte[] graphics;
        public byte[] Graphics
        {
            get => graphics;
            set
            {
                graphics = value;
                UpdateBitmap();
                OnPropertyChanged(PropertyNames.Tile.Graphics);
            }
        }

        protected Bitmap InternalBitmap;
        public Bitmap Bitmap
        {
            get => InternalBitmap;
            set
            {
                InternalBitmap = value;
                GenerateGraphics();
                OnPropertyChanged(PropertyNames.Tile.Bitmap);
            }
        }

        protected void UpdateBitmap()
        {
            if (Palette == null)
            {
                return;
            }

            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }

            GenerateBitmap();
        }

        protected abstract void GenerateBitmap();

        protected abstract void GenerateGraphics();

        public abstract int GetColorIndexAt(int x, int y);

        public virtual bool Contains(int colorIndex)
        {
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    if (GetColorIndexAt(x, y) == colorIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (InternalBitmap != null)
                {
                    InternalBitmap.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
