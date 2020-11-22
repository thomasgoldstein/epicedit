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

using EpicEdit.Rom.Tracks.Objects;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Tracks.Objects
{
    [TestFixture]
    internal class TrackObjectAreasTests
    {
        public void TestGetBytes(byte[] dataBefore)
        {
            TrackObjectAreas objectAreas = new TrackObjectAreas(dataBefore, null);
            byte[] dataAfter = objectAreas.GetBytes();
            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes2()
        {
            byte[] dataBefore = { 0x08, 0x14, 0x1C, 0x21, 0xFF, 0x0B, 0x15, 0x1E, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes3()
        {
            byte[] dataBefore = { 0x0C, 0x11, 0x1A, 0x24, 0xFF, 0x0E, 0x14, 0x1C, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes4()
        {
            byte[] dataBefore = { 0x0C, 0x11, 0x1D, 0x2C, 0xFF, 0x0E, 0x14, 0x21, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes6()
        {
            byte[] dataBefore = { 0x08, 0x0D, 0x13, 0x1C, 0xFF, 0x09, 0x11, 0x15, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes7()
        {
            byte[] dataBefore = { 0x0E, 0x17, 0x1C, 0x26, 0xFF, 0x0E, 0x17, 0x1C, 0x26, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes8()
        {
            byte[] dataBefore = { 0x09, 0x10, 0x17, 0x20, 0xFF, 0x0C, 0x13, 0x22, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes9()
        {
            byte[] dataBefore = { 0x0E, 0x20, 0x32, 0x3F, 0xFF, 0x0E, 0x28, 0x35, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes10()
        {
            byte[] dataBefore = { 0x08, 0x16, 0x1D, 0x38, 0xFF, 0x08, 0x16, 0x1D, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes11()
        {
            byte[] dataBefore = { 0x09, 0x1A, 0x27, 0x2C, 0xFF, 0x0A, 0x1C, 0x2A, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes12()
        {
            byte[] dataBefore = { 0x09, 0x0F, 0x1E, 0x24, 0xFF, 0x0B, 0x12, 0x20, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes13()
        {
            byte[] dataBefore = { 0x0E, 0x15, 0x1D, 0x24, 0xFF, 0x11, 0x18, 0x20, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }

        [Test]
        public void TestGetBytes14()
        {
            byte[] dataBefore = { 0x08, 0x12, 0x19, 0x22, 0xFF, 0x0A, 0x16, 0x1E, 0xFF, 0xFF };
            this.TestGetBytes(dataBefore);
        }
    }
}
