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

namespace EpicEdit.Rom.Tracks.Items
{
    /// <summary>
    /// Represents an item icon tile (shown in the item roulette).
    /// </summary>
    public class ItemIconTile : Tile
    {
        private int paletteIndex;
        private Bitmap image;

        public override Bitmap Bitmap
        {
            get { return this.image; }
        }

        public ItemIconTile(Palette palette, int paletteIndex, byte[] gfx)
        {
            this.graphics = gfx;
            this.paletteIndex = paletteIndex;
            this.Palette = palette;
        }

        public override void UpdateBitmap()
        {
            if (this.image != null)
            {
                this.image.Dispose();
            }
            this.GenerateBitmap();
        }

        private void GenerateBitmap()
        {
            if (this.Palette == null)
            {
                throw new InvalidOperationException("Cannot generate Bitmap as the Palette has not been set.");
            }
            this.image = GraphicsConverter.GetBitmapFrom2bppPlanar(this.graphics, this.Palette, this.paletteIndex);
        }

        public override int GetColorIndexAt(int x, int y)
        {
            x = (Tile.Size - 1) - x;
            byte val1 = this.graphics[(y * 2)];
            byte val2 = this.graphics[(y * 2) + 1];
            int mask = 1 << x;
            int colIndex = ((val1 & mask) >> x) + (((val2 & mask) >> x) << 1);
            return this.paletteIndex + colIndex;
        }

        public override void Dispose()
        {
            if (this.image != null)
            {
                this.image.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
