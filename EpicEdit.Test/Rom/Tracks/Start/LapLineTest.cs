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

using EpicEdit.Rom.Tracks.Start;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Tracks.Start
{
    [TestFixture]
    internal class LapLineTest
    {
        private void TestGetBytes(byte[] data)
        {
            LapLine lapLine = new LapLine(data);
            byte[] data2 = lapLine.GetBytes();

            Assert.AreEqual(data[0], data2[0]);
            Assert.AreEqual(data[1], data2[1]);
            Assert.AreEqual(data[2], data2[2]);
            Assert.AreEqual(data[4], data2[4]);
            // NOTE: bytes at index 3 and 5 will be different,
            // due to the fact Epic Edit simplifies the lap line display.
        }

        [Test]
        public void TestGetBytes1()
        {
            byte[] data = { 0x30, 0x02, 0x30, 0x08, 0x10, 0x08 };
            this.TestGetBytes(data);
        }

        [Test]
        public void TestGetBytes2()
        {
            byte[] data = { 0x00, 0x02, 0x00, 0x07, 0x0F, 0x08 };
            this.TestGetBytes(data);
        }

        [Test]
        public void TestGetBytes3()
        {
            byte[] data = { 0x40, 0x02, 0x38, 0x08, 0x08, 0x08 };
            this.TestGetBytes(data);
        }
    }
}
