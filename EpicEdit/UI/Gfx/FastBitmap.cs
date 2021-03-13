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
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    internal unsafe class FastBitmap
    {
        private struct PixelData
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        private int _subjectWidth;
        private BitmapData _bitmapData;
        private Byte* _pBase;

        public FastBitmap(Bitmap subjectBitmap)
        {
            Bitmap = subjectBitmap;
            LockBitmap();
        }

        public void Release()
        {
            UnlockBitmap();
        }

        public static implicit operator Bitmap(FastBitmap fBitmap)
        {
            return fBitmap.Bitmap;
        }

        public Bitmap ToBitmap()
        {
            return Bitmap;
        }

        public Bitmap Bitmap { get; }

        public void SetPixel(int x, int y, Color color)
        {
            PixelData* p = PixelAt(x, y);
            p->Alpha = color.A;
            p->Red = color.R;
            p->Green = color.G;
            p->Blue = color.B;
        }

        public Color GetPixel(int x, int y)
        {
            PixelData* p = PixelAt(x, y);
            return Color.FromArgb(p->Alpha, p->Red, p->Green, p->Blue);
        }

        private PixelData* PixelAt(int x, int y)
        {
            return (PixelData*)(_pBase + y * _subjectWidth + x * sizeof(PixelData));
        }

        private void LockBitmap()
        {
            Rectangle bounds = new Rectangle(Point.Empty, Bitmap.Size);

            _subjectWidth = bounds.Width * sizeof(PixelData);
            if (_subjectWidth % 4 != 0)
            {
                _subjectWidth = 4 * (_subjectWidth / 4 + 1);
            }

            _bitmapData = Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            _pBase = (Byte*)_bitmapData.Scan0.ToPointer();
        }

        private void UnlockBitmap()
        {
            Bitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
        }
    }
}
