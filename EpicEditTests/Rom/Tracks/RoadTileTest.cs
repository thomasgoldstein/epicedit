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
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks
{
    [TestFixture]
    internal class RoadTileTest
    {
        private void TestGetColorIndexAt(byte[] gfx, byte[] palData)
        {
            Palette palette = new Palette(null, palData);
            RoadTile tile = new RoadTile(gfx, palette, RoadTileGenre.Road);

            this.TestGetColorIndexAt(gfx, palette, tile);
        }

        private void TestGenerateGraphics(byte[] palData, byte[] gfx)
        {
            Palette pal = new Palette(null, palData);

            RoadTile tile = new RoadTile(gfx, pal, RoadTileGenre.Road);
            RoadTile tile2 = new RoadTile(new byte[gfx.Length], pal, RoadTileGenre.Road);

            tile2.Bitmap = tile.Bitmap; // Trigger graphics update

            Assert.AreEqual(gfx, tile2.Graphics);
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

        [Test]
        public void TestGenerateGraphics1()
        {
            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x44, 0x00, 0xA7, 0x04,
                0xC8, 0x0C, 0x4C, 0x19, 0xAF, 0x25, 0x54, 0x32,
                0x96, 0x3A, 0x09, 0x15, 0x4C, 0x15, 0x5F, 0x02,
                0xFF, 0x02, 0x54, 0x32, 0x96, 0x3A, 0x12, 0x2E
            };

            byte[] gfx =
            {
                0x56, 0x55, 0x55, 0x25, 0x45, 0x43, 0x34, 0x24,
                0x35, 0x42, 0x24, 0x23, 0x35, 0x34, 0x43, 0x23,
                0x45, 0x33, 0x33, 0x24, 0x45, 0x43, 0x34, 0x24,
                0x35, 0x44, 0x44, 0x23, 0x35, 0x34, 0x43, 0x23
            };

            this.TestGenerateGraphics(palData, gfx);
        }
    }
}
