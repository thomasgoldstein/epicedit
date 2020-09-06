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
using System;

namespace EpicEdit.Tests.Rom
{
    [TestFixture]
    internal class Tile2bppTests
    {
        private void TestGetColorIndexAt(byte[] gfx, byte[] palData, byte properties)
        {
            Palettes palettes = new Palettes(palData);
            Tile2bpp tile = new Tile2bpp(gfx, palettes, properties);

            Palette palette = new Palette(null, 0, palData);
            TileTests.TestGetColorIndexAt(tile, palette, false);
        }

        private void TestGenerateGraphics(byte[] palData, byte[] gfx)
        {
            this.TestGenerateGraphics(palData, gfx, new Tile2bppProperties());
        }

        private void TestGenerateGraphics(byte[] palData, byte[] gfx, Tile2bppProperties properties)
        {
            byte[] palsData = new byte[512];
            Buffer.BlockCopy(palData, 0, palsData, 0, palData.Length);

            Palettes pals = new Palettes(palsData);

            byte props = properties.GetByte();
            Tile2bpp tile = new Tile2bpp(gfx, pals, props);
            Tile2bpp tile2 = new Tile2bpp(new byte[gfx.Length], pals, props);

            tile2.Bitmap = tile.Bitmap; // Trigger graphics update

            Assert.AreEqual(gfx, tile2.Graphics);
        }

        [Test]
        public void TestGetColorIndexAt1()
        {
            byte[] gfx =
            {
                0xFF, 0xFF, 0xFF, 0xFC, 0xFF, 0xF0, 0xFE, 0xE1,
                0xFC, 0xE3, 0xC1, 0xFE, 0xD9, 0xE6, 0xFC, 0xC3
            };

            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x40, 0x03, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0x7F, 0x1F, 0x00, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0x7F, 0xFF, 0x03, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0x7F, 0x4A, 0x7F, 0x00, 0x00
            };

            this.TestGetColorIndexAt(gfx, palData, 0);
        }

        [Test]
        public void TestPropertiesGetByte()
        {
            for (int i = 0; i <= 0xFF; i++)
            {
                byte value = (byte)i;
                if ((value & 0x3) != 0)
                {
                    Assert.Throws(typeof(ArgumentOutOfRangeException), delegate { new Tile2bppProperties(value); });
                }
                else
                {
                    Tile2bppProperties properties = new Tile2bppProperties(value);
                    Assert.AreEqual(i, properties.GetByte());
                }
            }
        }

        [Test]
        public void TestGenerateGraphics1()
        {
            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x55, 0x62, 0xB0, 0x49,
                0x00, 0x00, 0x5A, 0x73, 0xB0, 0x49, 0x2C, 0x39,
                0x00, 0x00, 0xD3, 0x7E, 0x20, 0x78, 0x00, 0x3C,
                0x00, 0x00, 0xDE, 0x3F, 0x9E, 0x02, 0x1B, 0x00
            };

            byte[] gfx =
            {
                0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0xFF,
                0x00, 0xFF, 0xFF, 0x00, 0xFF, 0x00, 0xFF, 0x00
            };

            this.TestGenerateGraphics(palData, gfx);
        }

        [Test]
        public void TestGenerateGraphics2()
        {
            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x55, 0x62, 0xB0, 0x49,
                0x00, 0x00, 0x5A, 0x73, 0xB0, 0x49, 0x2C, 0x39,
                0x00, 0x00, 0xD3, 0x7E, 0x20, 0x78, 0x00, 0x3C,
                0x00, 0x00, 0xDE, 0x3F, 0x9E, 0x02, 0x1B, 0x00
            };

            byte[] gfx =
            {
                0x00, 0x00, 0x00, 0x07, 0x07, 0x18, 0x0F, 0x30,
                0x0B, 0x20, 0x0B, 0x60, 0x0B, 0x60, 0x3F, 0x40
            };

            this.TestGenerateGraphics(palData, gfx);
        }

        [Test]
        public void TestGenerateGraphics3()
        {
            // Ensure that when a sub-palette (4-color palette) doesn't have a first color
            // that matches the theme BackColor (ie: the first color of the first palette),
            // then the graphics generation still works as expected, using the theme BackColor
            // rather than the first sub-palette color.
            byte[] palData =
            {
                0x00, 0x00, 0xFF, 0x7F, 0x55, 0x62, 0xB0, 0x49,
                0xF0, 0xF0, 0x5A, 0x73, 0xB0, 0x49, 0x2C, 0x39, // Sub-palette used
                0x00, 0x00, 0xD3, 0x7E, 0x20, 0x78, 0x00, 0x3C,
                0x00, 0x00, 0xDE, 0x3F, 0x9E, 0x02, 0x1B, 0x00
            };

            byte[] gfx =
            {
                0x00, 0x00, 0x00, 0x07, 0x07, 0x18, 0x0F, 0x30,
                0x0B, 0x20, 0x0B, 0x60, 0x0B, 0x60, 0x3F, 0x40
            };

            Tile2bppProperties properties = new Tile2bppProperties { SubPaletteIndex = 4 };

            this.TestGenerateGraphics(palData, gfx, properties);
        }
    }
}
