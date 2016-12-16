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
using System.ComponentModel;
using System.Drawing;
using EpicEdit.Rom.Utility;

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
            get { return this.palette; }
            set
            {
                if (this.palette == value)
                {
                    return;
                }

                if (this.palette != null)
                {
                    this.palette.ColorChanged -= this.palette_ColorChanged;
                    this.palette.ColorsChanged -= this.palette_ColorsChanged;
                }

                this.palette = value;

                this.palette.ColorChanged += this.palette_ColorChanged;
                this.palette.ColorsChanged += this.palette_ColorsChanged;

                this.UpdateBitmap();
                this.OnPropertyChanged(PropertyNames.Tile.Palette);
            }
        }

        private void palette_ColorChanged(object sender, EventArgs<int> e)
        {
            if (this.Contains(e.Value))
            {
                this.UpdateBitmap();
            }
        }

        private void palette_ColorsChanged(object sender, EventArgs e)
        {
            this.UpdateBitmap();
        }

        private byte[] graphics;
        public byte[] Graphics
        {
            get { return this.graphics; }
            set
            {
                this.graphics = value;
                this.UpdateBitmap();
                this.OnPropertyChanged(PropertyNames.Tile.Graphics);
            }
        }

        protected Bitmap bitmap;
        public Bitmap Bitmap
        {
            get { return this.bitmap; }
            set
            {
                this.bitmap = value;
                this.GenerateGraphics();
                this.OnPropertyChanged(PropertyNames.Tile.Bitmap);
            }
        }

        protected void UpdateBitmap()
        {
            if (this.Palette == null)
            {
                return;
            }

            if (this.Bitmap != null)
            {
                this.Bitmap.Dispose();
            }

            this.GenerateBitmap();
        }

        protected abstract void GenerateBitmap();

        protected abstract void GenerateGraphics();

        public abstract int GetColorIndexAt(int x, int y);

        public virtual bool Contains(int colorIndex)
        {
            for (int y = 0; y < Tile.Size; y++)
            {
                for (int x = 0; x < Tile.Size; x++)
                {
                    if (this.GetColorIndexAt(x, y) == colorIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.bitmap != null)
                {
                    this.bitmap.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
