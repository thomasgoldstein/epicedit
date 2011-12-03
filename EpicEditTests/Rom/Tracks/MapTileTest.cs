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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks
{
    [TestFixture]
    public class MapTileTest
    {
        private void TestGetColorIndexAt(byte[] gfx, byte[] palData)
        {
            Palette palette = new Palette(null, palData);
            RoadTile tile = new RoadTile(gfx, palette, TileGenre.Road);

            this.TestGetColorIndexAt(gfx, palette, tile);
        }

        private void TestGetColorIndexAt(byte[] gfx, Palette palette, RoadTile tile)
        {
            TileTest.TestGetColorIndexAt(tile, palette, false);

            byte[] gfxCopy = new byte[gfx.Length];

            for (int y = 0; y < Tile.Size; y++)
            {
                for (int x = 0; x < Tile.Size; x++)
                {
                    int colorIndex = tile.GetColorIndexAt(x, y);

                    int index = y * Tile.Size + x;
                    if (index % 2 == 1)
                    {
                        colorIndex <<= 4;
                    }

                    gfxCopy[index / 2] |= (byte)colorIndex;
                }
            }

            Assert.AreEqual(gfx, gfxCopy);
        }

        [Test]
        public void TestGetColorIndexAt1()
        {
            byte[] gfx =
            {
                0x88, 0x78, 0x89, 0x88, 0x89, 0x88, 0x88, 0x88,
                0x88, 0x98, 0x88, 0x87, 0x98, 0x88, 0x88, 0x98,
                0x88, 0x87, 0x88, 0x88, 0x88, 0x98, 0x98, 0x88,
                0x78, 0x88, 0x88, 0x88, 0x88, 0x89, 0x88, 0x79
            };

            byte[] palData =
            {
                0xBF, 0x4B, 0x00, 0x00, 0xFF, 0x7F, 0x00, 0x7C,
                0x1F, 0x00, 0xE0, 0x03, 0xFF, 0x03, 0x40, 0x03,
                0xA0, 0x02, 0x00, 0x02, 0x60, 0x01, 0x5F, 0x02,
                0xFF, 0x02, 0x54, 0x32, 0x96, 0x3A, 0x12, 0x2E
            };

            this.TestGetColorIndexAt(gfx, palData);
        }

        [Test]
        public void TestGetColorIndexAt2()
        {
            byte[] gfx =
            {
                0x88, 0x78, 0x89, 0x88, 0x89, 0x88, 0x88, 0x88,
                0x88, 0x98, 0x88, 0x87, 0x98, 0x88, 0x88, 0x98,
                0x88, 0x87, 0x88, 0x88, 0x88, 0x98, 0x98, 0x88,
                0x78, 0x88, 0x88, 0x88, 0x88, 0x89, 0x88, 0x79
            };

            byte[] palData =
            {
                // Give the same value to all colors, to check
                // that color indexes are properly preserved
                0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
                0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
                0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
                0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08
            };

            this.TestGetColorIndexAt(gfx, palData);
        }
    }
}
