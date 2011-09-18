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

namespace EpicEdit.Rom.Tracks
{
    public enum TileGenre { Road };

    /// <summary>
    /// Represents a non-animated track map tile.
    /// </summary>
    public class MapTile : Tile
    {
        private Bitmap image;

        private TileGenre genre = TileGenre.Road;
        public TileGenre Genre
        {
            get { return this.genre; }
            protected set { this.genre = value; }
        }

        public override Bitmap Bitmap
        {
            get { return this.image; }
        }

        public MapTile(Palette palette, byte[] gfx, TileGenre genre)
        {
            this.Palette = palette;
            this.graphics = gfx;
            this.Genre = genre;
            this.GenerateBitmap();
        }

        public MapTile(Palette palette, Bitmap image, TileGenre genre)
        {
            this.Palette = palette;
            this.image = image;
            this.Genre = genre;
        }

        public override void UpdateBitmap()
        {
            if (this.graphics == null)
            {
                // This is an empty tile, contains no data
                return;
            }

            this.image.Dispose();
            this.GenerateBitmap();
        }

        private void GenerateBitmap()
        {
            this.image = GraphicsConverter.GetBitmapFrom4bppLinearReversed(this.graphics, this.Palette);
        }

        public ushort[] ToSnesBitmap()
        {
            ushort[] buffer = new ushort[Tile.Size * Tile.Size];
            ushort tempo;
            byte r, g, b;

            for (int x = 0; x < Tile.Size; x++)
            {
                for (int y = 0; y < Tile.Size; y++)
                {
                    r = this.image.GetPixel(x, y).R;
                    if (((r & 0x07) >= 4) && (r < 251))
                    {
                        r += 8;
                    }

                    g = this.image.GetPixel(x, y).G;
                    if (((g & 0x07) >= 4) && (g < 251))
                    {
                        g += 8;
                    }

                    b = this.image.GetPixel(x, y).B;
                    if (((b & 0x07) >= 4) && (b < 251))
                    {
                        b += 8;
                    }

                    tempo = (ushort)(((r >> 3) << 10) + ((g >> 3) << 5) + (b >> 3));
                    tempo = (ushort)((tempo / 256) + ((tempo & 0x00FF) * 256));
                    buffer[x + y * Tile.Size] = tempo;
                }
            }
            return buffer;
        }

        public override int GetColorIndexAt(int x, int y)
        {
            if (this.graphics == null)
            {
                // Empty tile, no data
                return -1;
            }

            int xSub = x % 2;
            x /= 2;
            byte px = this.graphics[y * 4 + x];
            int index = xSub == 0 ?
                px & 0x0F : (px & 0xF0) >> 4;

            return index;
        }

        public override void Dispose()
        {
            this.image.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
