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
using EpicEdit.Rom.Utility;
using NUnit.Framework;

namespace EpicEditTests.Rom
{
    [TestFixture]
    internal class UtilitiesTest
    {
        [Test]
        public void TestBytesToOffset000000()
        {
            byte[] buffer = { 0x00, 0x00, 0x00 };
            int offset = Utilities.BytesToOffset(buffer);

            Assert.AreEqual(0x000000, offset);
        }

        [Test]
        public void TestBytesToOffset0fffff()
        {
            byte[] buffer = { 0xFF, 0xFF, 0xFF };
            int offset = Utilities.BytesToOffset(buffer);

            Assert.AreEqual(0x0FFFFF, offset);
        }

        [Test]
        public void TestBytesToOffset063412()
        {
            byte[] buffer = { 0x12, 0x34, 0x56 };
            int offset = Utilities.BytesToOffset(buffer);

            Assert.AreEqual(0x063412, offset);
        }

        [Test]
        public void TestBytesToOffset080400()
        {
            byte[] buffer = { 0x00, 0x04, 0xC8 };
            int offset = Utilities.BytesToOffset(buffer);

            Assert.AreEqual(0x080400, offset);
        }

        [Test]
        public void TestOffsetToBytes0fffff()
        {
            byte[] buffer = { 0xFF, 0xFF, 0xCF };
            byte[] buffer2 = Utilities.OffsetToBytes(0x0FFFFF);

            Assert.AreEqual(buffer, buffer2);
        }

        [Test]
        public void TestOffsetToBytes063412()
        {
            byte[] buffer = { 0x12, 0x34, 0xC6 };
            byte[] buffer2 = Utilities.OffsetToBytes(0x063412);

            Assert.AreEqual(buffer, buffer2);
        }

        [Test]
        public void TestOffsetToBytes080400()
        {
            byte[] buffer = { 0x00, 0x04, 0xC8 };
            byte[] buffer2 = Utilities.OffsetToBytes(0x080400);

            Assert.AreEqual(buffer, buffer2);
        }
    }
}
