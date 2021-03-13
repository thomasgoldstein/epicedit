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

using EpicEdit.UI.Gfx;
using System;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// Represents a track object tile.
    /// </summary>
    internal sealed class TrackObjectTile : Tile
    {
        public TrackObjectTile(byte[] gfx)
        {
            Graphics = gfx;
        }

        protected override void GenerateBitmap()
        {
            InternalBitmap = GraphicsConverter.GetBitmapFrom4bppPlanarComposite(Graphics, Palette);
        }

        protected override void GenerateGraphics()
        {
            throw new NotImplementedException();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            x = (Size - 1) - x;
            int val1 = Graphics[y * 2];
            int val2 = Graphics[y * 2 + 1];
            int val3 = Graphics[y * 2 + 16];
            int val4 = Graphics[y * 2 + 17];
            int mask = 1 << x;
            int val1b = ((val1 & mask) >> x);
            int val2b = (((val2 & mask) << 1) >> x);
            int val3b = (((val3 & mask) << 2) >> x);
            int val4b = (((val4 & mask) << 3) >> x);
            return val1b + val2b + val3b + val4b;
        }

        public override bool Contains(int colorIndex)
        {
            // TrackObjectTile instances have transparent pixels where the color 0 is,
            // so consider they don't contain it. This lets us avoid unnecessarily recreating
            // the tile image when the color 0 is changed.
            return colorIndex != 0 && base.Contains(colorIndex);
        }
    }
}
