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
using NUnit.Framework;

namespace EpicEditTests.Rom
{
    [TestFixture]
    public class PaletteTest
    {
        [Test]
        public void Test0000()
        {
            byte[] polo = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                polo[i] = 0;
            }
            Palette pal = new Palette(polo);
            Assert.AreEqual(Color.FromArgb(0, 0, 0), pal[0].Color);
        }

        [Test]
        public void TestFfff()
        {
            byte[] polo = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                polo[i] = 0xFF;
            }
            Palette pal = new Palette(polo);
            Assert.AreEqual(Color.FromArgb(255, 255, 255), pal[0].Color);
        }
    }
}
