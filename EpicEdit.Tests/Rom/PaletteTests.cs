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
    [TestFixture]
    internal class PaletteTests
    {
        [Test]
        public void TestBlack()
        {
            var palData = new byte[32];
            palData[0] = 0x00;
            palData[1] = 0x00;
            var pal = new Palette(null, 0, palData);
            Assert.AreEqual(Color.FromArgb(0, 0, 0), pal[0].Color);
        }

        [Test]
        public void TestWhite()
        {
            var palData = new byte[32];
            palData[0] = 0xFF;
            palData[1] = 0xFF;
            var pal = new Palette(null, 0, palData);
            Assert.AreEqual(Color.FromArgb(255, 255, 255), pal[0].Color);
        }

        [Test]
        public void TestRed()
        {
            var palData = new byte[32];
            palData[0] = 0x1F;
            palData[1] = 0x00;
            var pal = new Palette(null, 0, palData);
            Assert.AreEqual(Color.FromArgb(255, 0, 0), pal[0].Color);
        }

        [Test]
        public void TestGreen()
        {
            var palData = new byte[32];
            palData[0] = 0xE0;
            palData[1] = 0x03;
            var pal = new Palette(null, 0, palData);
            Assert.AreEqual(Color.FromArgb(0, 255, 0), pal[0].Color);
        }

        [Test]
        public void TestBlue()
        {
            var palData = new byte[32];
            palData[0] = 0x00;
            palData[1] = 0xFC;
            var pal = new Palette(null, 0, palData);
            Assert.AreEqual(Color.FromArgb(0, 0, 255), pal[0].Color);
        }
    }
}
