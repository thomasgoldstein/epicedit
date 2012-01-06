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
        [Test]
        public void TestGraphicsUpdate()
        {
            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x44, 0x00, 0xA7, 0x04,
                0xC8, 0x0C, 0x4C, 0x19, 0xAF, 0x25, 0x54, 0x32,
                0x96, 0x3A, 0x09, 0x15, 0x4C, 0x15, 0x5F, 0x02,
                0xFF, 0x02, 0x54, 0x32, 0x96, 0x3A, 0x12, 0x2E
            };

            Palette pal = new Palette(null, palData);

            byte[] gfx =
            {
                0x56, 0x55, 0x55, 0x25, 0x45, 0x43, 0x34, 0x24,
                0x35, 0x42, 0x24, 0x23, 0x35, 0x34, 0x43, 0x23,
                0x45, 0x33, 0x33, 0x24, 0x45, 0x43, 0x34, 0x24,
                0x35, 0x44, 0x44, 0x23, 0x35, 0x34, 0x43, 0x23
            };
            byte[] gfxCopy = gfx.Clone() as byte[];

            RoadTile tile = new RoadTile(gfx, pal, TileGenre.Road);

            Bitmap bitmap = tile.Bitmap;
            tile.Bitmap = bitmap; // Trigger graphics update

            Assert.AreEqual(gfxCopy, tile.Graphics);
        }
    }
}
