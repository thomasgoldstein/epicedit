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
using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.Objects;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks.Objects
{
    [TestFixture]
    public class TrackObjectTileTest
    {
        private void TestGetColorIndexAt(byte[] palData, byte[] gfx)
        {
            Palette palette = new Palette(palData);
            TrackObjectTile tile = new TrackObjectTile(palette, gfx);

            for (int y = 0; y < Tile.Size; y++)
            {
                for (int x = 0; x < Tile.Size; x++)
                {
                    int colorIndex = tile.GetColorIndexAt(x, y);
                    Color color1 = tile.Bitmap.GetPixel(x, y);

                    if (color1 == Color.Transparent)
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

        [Test]
        public void TestGetColorIndexAt1()
        {
            byte[] palData =
            {
                0xBF, 0x4B, 0x4D, 0x7E, 0x14, 0x7F, 0xBA, 0x7F,
                0xE0, 0x09, 0xF9, 0x3E, 0xF1, 0x19, 0x75, 0x2A,
                0xA0, 0x0E, 0x60, 0x13, 0xE0, 0x03, 0x60, 0x03,
                0xE0, 0x02, 0x80, 0x01, 0x00, 0x00, 0xBF, 0x4B
            };

            byte[] gfx =
            {
                0x1F, 0x00, 0x20, 0x0F, 0x20, 0x00, 0x26, 0x07,
                0x26, 0x07, 0x26, 0x07, 0x36, 0x07, 0x1F, 0x20,
                0x1F, 0x1F, 0x3F, 0x3F, 0x3F, 0x3F, 0x38, 0x3F,
                0x38, 0x3F, 0x38, 0x3F, 0x38, 0x3F, 0x3F, 0x3F
            };

            this.TestGetColorIndexAt(palData, gfx);
        }
    }
}
