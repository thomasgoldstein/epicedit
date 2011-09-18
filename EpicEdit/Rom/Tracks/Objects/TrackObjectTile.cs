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

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// Represents a track object tile.
    /// </summary>
    public class TrackObjectTile : Tile
    {
        private Bitmap image;

        public override Bitmap Bitmap
        {
            get { return this.image; }
        }

        public TrackObjectTile(byte[] gfx)
        {
            this.graphics = gfx;
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
            this.image = GraphicsConverter.GetBitmapFrom4bppPlanarComposite(this.graphics, this.Palette);
        }

        public override int GetColorIndexAt(int x, int y)
        {
            throw new NotImplementedException();
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
