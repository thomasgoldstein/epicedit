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
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks.Scenery
{
    internal enum Flip : byte
    {
        None = 0x00,
        X = 0x40,
        Y = 0x80,
        XY = 0xC0
    }

    /// <summary>
    /// Represents a non-animated track background tile.
    /// </summary>
    internal sealed class BackgroundTile : Tile
    {
        private const int FrontPaletteStart = 4;
        private const int BackPaletteStart = 6;

        private Palettes palettes;

        public BackgroundTile(byte[] gfx, Palettes palettes)
        {
            this.Graphics = gfx;
            this.palettes = palettes;
        }

        protected override void GenerateBitmap()
        {
            throw new NotImplementedException();
        }

        protected override void GenerateGraphics()
        {
            throw new NotImplementedException();
        }

        private Bitmap GetBitmap(Palette palette, int subPaletteIndex)
        {
            if (palette == null)
            {
                throw new InvalidOperationException("Cannot generate Bitmap as the Palette has not been set.");
            }
            return GraphicsConverter.GetBitmapFrom2bppPlanar(this.Graphics, palette, subPaletteIndex, true);
        }

        public Bitmap GetBitmap(byte properties, bool front)
        {
            byte paletteData = (byte)(properties & 0x3F);
            int start = front ? FrontPaletteStart : BackPaletteStart;
            int palIndex = paletteData / 16;
            int subPalIndex = paletteData % 16;
            int subPaletteIndex = subPalIndex;
            Palette palette = this.palettes[start + palIndex];

            Bitmap bitmap = this.GetBitmap(palette, subPaletteIndex);

            Flip flip = (Flip)(properties & (byte)(Flip.XY));
            switch (flip)
            {
                case Flip.X:
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;

                case Flip.Y:
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;

                case Flip.XY:
                    bitmap.RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    break;
            }

            return bitmap;
        }

        public override int GetColorIndexAt(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
