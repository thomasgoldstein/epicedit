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
using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.Items;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks.Items
{
    [TestFixture]
    public class ItemIconTileTest
    {
        private void TestGetColorIndexAt(byte[] palData, int paletteIndex, byte[] gfx)
        {
            Palette palette = new Palette(palData);
            ItemIconTile tile = new ItemIconTile(palette, paletteIndex, gfx);

            TileTest.TestGetColorIndexAt(tile, palette, false);
        }

        [Test]
        public void TestGetColorIndexAt1()
        {
            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x40, 0x03, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0x7F, 0x1F, 0x00, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0x7F, 0xFF, 0x03, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0x7F, 0x4A, 0x7F, 0x00, 0x00
            };

            byte[] gfx =
            {
                0xFF, 0xFF, 0xFF, 0xFC, 0xFF, 0xF0, 0xFE, 0xE1,
                0xFC, 0xE3, 0xC1, 0xFE, 0xD9, 0xE6, 0xFC, 0xC3
            };

            this.TestGetColorIndexAt(palData, 4, gfx);
        }
    }
}
