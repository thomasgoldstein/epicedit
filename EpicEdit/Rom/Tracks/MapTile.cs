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

        public MapTile(byte[] gfx, Palette palette, TileGenre genre)
        {
            this.Palette = palette;
            this.graphics = gfx;
            this.Genre = genre;
            this.GenerateBitmap();
        }

        public MapTile(Bitmap image, Palette palette, TileGenre genre)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.image.Dispose();
            }
        }
    }
}
