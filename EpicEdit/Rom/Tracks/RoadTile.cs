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
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents a non-animated track road tile.
    /// </summary>
    internal sealed class RoadTile : Tile
    {
        private RoadTileGenre genre = RoadTileGenre.Road;
        public RoadTileGenre Genre
        {
            get { return this.genre; }
            set
            {
                if (!Enum.IsDefined(typeof(RoadTileGenre), value))
                {
                    throw new ArgumentException("Invalid tile type value: " + value.ToString("X") + ".", "value");
                }

                this.genre = value;
            }
        }

        public RoadTile(byte[] gfx, Palette palette, RoadTileGenre genre)
        {
            this.Graphics = gfx;
            this.Palette = palette;
            this.Genre = genre;
        }

        protected override void GenerateBitmap()
        {
            this.bitmap = GraphicsConverter.GetBitmapFrom4bppLinearReversed(this.Graphics, this.Palette);
        }

        protected override void GenerateGraphics()
        {
            FastBitmap fBitmap = new FastBitmap(this.bitmap);

            int pixelIndex = 0;
            for (int y = 0; y < Tile.Size; y++)
            {
                for (int x = 0; x < Tile.Size / 2; x++)
                {
                    RomColor color1 = (RomColor)fBitmap.GetPixel(x * 2, y);
                    RomColor color2 = (RomColor)fBitmap.GetPixel(x * 2 + 1, y);
                    int colorIndex1 = Utilities.GetColorIndex(color1, this.Palette);
                    int colorIndex2 = Utilities.GetColorIndex(color2, this.Palette);

                    this.Graphics[pixelIndex++] = (byte)(colorIndex1 + (colorIndex2 << 4));
                }
            }

            fBitmap.Release();

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            this.GenerateBitmap();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            int xSub = x % 2;
            x /= 2;
            byte px = this.Graphics[y * 4 + x];
            int index = xSub == 0 ?
                px & 0x0F : (px & 0xF0) >> 4;

            return index;
        }
    }
}
