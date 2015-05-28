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
using EpicEdit.Rom.Tracks.Start;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Tracks.Start
{
    [TestFixture]
    internal class GPStartPositionTest
    {
        private byte[] allData;

        [TestFixtureSetUp]
        public void Init()
        {
            this.allData = new byte[]
            {
                0x79, 0x8F, 0x90, 0x00, 0x1C, 0x02, 0xE0, 0xFF,
                0x79, 0x8F, 0x70, 0x03, 0x4C, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0xB8, 0x03, 0x60, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0x90, 0x03, 0xAC, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0x98, 0x03, 0x4C, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0xB8, 0x00, 0x20, 0x01, 0xE0, 0xFF,
                0x79, 0x8F, 0x6C, 0x00, 0xAC, 0x02, 0xE8, 0xFF,
                0x79, 0x8F, 0x90, 0x03, 0x60, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0x50, 0x00, 0x80, 0x01, 0xE0, 0xFF,
                0x79, 0x8F, 0x88, 0x03, 0xC4, 0x01, 0x20, 0x00,
                0x79, 0x8F, 0x68, 0x00, 0x40, 0x01, 0xE0, 0xFF,
                0x79, 0x8F, 0xC0, 0x00, 0x2C, 0x02, 0xE0, 0xFF,
                0x79, 0x8F, 0x70, 0x00, 0x30, 0x02, 0x28, 0x00,
                0x79, 0x8F, 0x90, 0x03, 0x50, 0x01, 0x20, 0x00,
                0x79, 0x8F, 0x60, 0x00, 0xCC, 0x01, 0xE0, 0xFF,
                0x79, 0x8F, 0x68, 0x03, 0x14, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0xB8, 0x02, 0x48, 0x01, 0x20, 0x00,
                0x79, 0x8F, 0x68, 0x00, 0x40, 0x01, 0xE0, 0xFF,
                0x79, 0x8F, 0x98, 0x03, 0x4C, 0x02, 0x20, 0x00,
                0x79, 0x8F, 0x4C, 0x00, 0xD0, 0x01, 0xE8, 0xFF
            };
        }

        private void TestGetBytes(int id)
        {
            byte[] data = new byte[6];

            int index = id * 8 + 2;
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = this.allData[index + i];
            }

            this.TestGetBytes(data);
        }

        private void TestGetBytes(byte[] data)
        {
            GPStartPosition startPosition = new GPStartPosition(data);
            byte[] dataAfter = startPosition.GetBytes();

            Assert.AreEqual(data, dataAfter);
        }

        [Test]
        public void TestGetBytes1()
        {
            this.TestGetBytes(0);
        }

        [Test]
        public void TestGetBytes2()
        {
            this.TestGetBytes(1);
        }

        [Test]
        public void TestGetBytes3()
        {
            this.TestGetBytes(2);
        }

        [Test]
        public void TestGetBytes4()
        {
            this.TestGetBytes(3);
        }

        [Test]
        public void TestGetBytes5()
        {
            this.TestGetBytes(4);
        }

        [Test]
        public void TestGetBytes6()
        {
            this.TestGetBytes(5);
        }

        [Test]
        public void TestGetBytes7()
        {
            this.TestGetBytes(6);
        }

        [Test]
        public void TestGetBytes8()
        {
            this.TestGetBytes(7);
        }

        [Test]
        public void TestGetBytes9()
        {
            this.TestGetBytes(8);
        }

        [Test]
        public void TestGetBytes10()
        {
            this.TestGetBytes(9);
        }

        [Test]
        public void TestGetBytes11()
        {
            this.TestGetBytes(10);
        }

        [Test]
        public void TestGetBytes12()
        {
            this.TestGetBytes(11);
        }

        [Test]
        public void TestGetBytes13()
        {
            this.TestGetBytes(12);
        }

        [Test]
        public void TestGetBytes14()
        {
            this.TestGetBytes(13);
        }

        [Test]
        public void TestGetBytes15()
        {
            this.TestGetBytes(14);
        }

        [Test]
        public void TestGetBytes16()
        {
            this.TestGetBytes(15);
        }

        [Test]
        public void TestGetBytes17()
        {
            this.TestGetBytes(16);
        }

        [Test]
        public void TestGetBytes18()
        {
            this.TestGetBytes(17);
        }

        [Test]
        public void TestGetBytes19()
        {
            this.TestGetBytes(18);
        }

        [Test]
        public void TestGetBytes20()
        {
            this.TestGetBytes(19);
        }

        [Test]
        public void TestGetBytes21()
        {
            byte[] data = { 0xC8, 0x03, 0x08, 0x02, 0xE4, 0xFF };
            this.TestGetBytes(data);
        }

        [Test]
        public void TestGetBytes22()
        {
            byte[] data = { 0x90, 0x03, 0x74, 0x02, 0xE4, 0xFF };
            this.TestGetBytes(data);
        }
    }
}
