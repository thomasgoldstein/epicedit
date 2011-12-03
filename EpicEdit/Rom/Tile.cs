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
    /// An 8x8 graphic tile.
    /// </summary>
    public abstract class Tile : IDisposable
    {
        public const int Size = 8;

        private Palette palette;
        public Palette Palette
        {
            get { return this.palette; }
            set
            {
                if (this.palette == value)
                {
                    return;
                }

                this.palette = value;
                this.UpdateBitmap();
            }
        }

        public byte[] Graphics { get; protected set; }

        public abstract Bitmap Bitmap { get; }

        public abstract void UpdateBitmap();

        public abstract int GetColorIndexAt(int x, int y);

        public bool Contains(int colorIndex)
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

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
