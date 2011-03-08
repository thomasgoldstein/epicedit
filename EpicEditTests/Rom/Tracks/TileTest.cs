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
            Bitmap image = new Bitmap(1, 1, PixelFormat.Format24bppRgb);
            this.tile = new StillTile(image, TileGenre.Road);
        }

        [Test]
        public void TestTileBitmap()
        {
            Assert.IsInstanceOf(typeof(Bitmap), this.tile.Bitmap);
            Assert.AreEqual(1, Tile.Size);
            Assert.AreEqual(1, Tile.Size);
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
    }
}
