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
    public class TileTest
    {
        private StillTile tile;

        public TileTest()
        {
            Bitmap image = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
            this.tile = new StillTile(new Palette(), image, TileGenre.Road);
        }

        [Test]
        public void TestTileBitmap()
        {
            Assert.IsInstanceOf(typeof(Bitmap), this.tile.Bitmap);
            Assert.AreEqual(8, Tile.Size);
        }

        [Test]
        public void TestTileGenre()
        {
            Assert.AreEqual(this.tile.Genre, TileGenre.Road);
        }

        [Test]
        public void TestTileSnesColorBlack()
        {
            this.tile.Bitmap.SetPixel(0, 0, Color.FromArgb(0, 0, 0));
            Assert.AreEqual(0x0000, this.tile.ToSnesBitmap()[0]);
        }

        [Test]
        public void TestTileSnesColorWhite()
        {
            this.tile.Bitmap.SetPixel(0, 0, Color.FromArgb(255, 255, 255));
            Assert.AreEqual(0xFF7F, this.tile.ToSnesBitmap()[0]);
        }

        [Test]
        public void TestTileSnesColorGray()
        {
            this.tile.Bitmap.SetPixel(0, 0, Color.FromArgb(80, 80, 80));
            Assert.AreEqual(0x4A29, this.tile.ToSnesBitmap()[0]);
        }

        [Test]
        public void TestTileSnesColorAverageHigh()
        {
            this.tile.Bitmap.SetPixel(0, 0, Color.FromArgb(80, 80, 88));
            Assert.AreEqual(0x4B29, this.tile.ToSnesBitmap()[0]);
        }

        [Test]
        public void TestTileSnesColorAverageMid()
        {
            this.tile.Bitmap.SetPixel(0, 0, Color.FromArgb(80, 80, 86));
            Assert.AreEqual(0x4B29, this.tile.ToSnesBitmap()[0]);
        }

        [Test]
        public void TestTileSnesColorAverageLow()
        {
            this.tile.Bitmap.SetPixel(0, 0, Color.FromArgb(80, 80, 82));
            Assert.AreEqual(0x4A29, this.tile.ToSnesBitmap()[0]);
        }

        private void TestGetColorIndexAt(byte[] palData, byte[] gfx)
        {
            Palette palette = new Palette(palData);
            this.tile = new StillTile(palette, gfx, TileGenre.Road);

            byte[] gfxCopy = new byte[gfx.Length];

            for (int y = 0; y < Tile.Size; y++)
            {
                for (int x = 0; x < Tile.Size; x++)
                {
                    int colorIndex = this.tile.GetColorIndexAt(x, y);
                    Color color1 = this.tile.Bitmap.GetPixel(x, y);
                    Color color2 = palette[colorIndex];
                    Assert.AreEqual(color1.ToArgb(), color2.ToArgb());

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
            byte[] palData =
            {
                0xBF, 0x4B, 0x00, 0x00, 0xFF, 0x7F, 0x00, 0x7C,
                0x1F, 0x00, 0xE0, 0x03, 0xFF, 0x03, 0x40, 0x03,
                0xA0, 0x02, 0x00, 0x02, 0x60, 0x01, 0x5F, 0x02,
                0xFF, 0x02, 0x54, 0x32, 0x96, 0x3A, 0x12, 0x2E
            };

            byte[] gfx =
            {
                0x88, 0x78, 0x89, 0x88, 0x89, 0x88, 0x88, 0x88,
                0x88, 0x98, 0x88, 0x87, 0x98, 0x88, 0x88, 0x98,
                0x88, 0x87, 0x88, 0x88, 0x88, 0x98, 0x98, 0x88,
                0x78, 0x88, 0x88, 0x88, 0x88, 0x89, 0x88, 0x79
            };

            this.TestGetColorIndexAt(palData, gfx);
        }
    }
}
