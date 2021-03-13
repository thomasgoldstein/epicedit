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

using EpicEdit.Rom;
using NUnit.Framework;
using System.Drawing;

namespace EpicEdit.Tests.Rom
{
    internal static class TileTests
    {
        public static void TestGetColorIndexAt(Tile tile, Palette palette, bool transparency)
        {
            for (var y = 0; y < Tile.Size; y++)
            {
                for (var x = 0; x < Tile.Size; x++)
                {
                    var colorIndex = tile.GetColorIndexAt(x, y);
                    var color1 = tile.Bitmap.GetPixel(x, y);

                    if (transparency && color1.A == 0) // Transparent pixel
                    {
                        Assert.AreEqual(0, colorIndex);
                    }
                    else
                    {
                        Color color2 = palette[colorIndex];
                        Assert.AreEqual(color1.ToArgb(), color2.ToArgb());
                    }
                }
            }
        }
    }
}
