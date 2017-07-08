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
    internal class BattleStartPositionTest
    {
        private byte[] allData;

        [OneTimeSetUp]
        public void Init()
        {
            this.allData = new byte[]
            {
                0x00, 0x02, 0x88, 0x01, 0x00, 0x02, 0x78, 0x02,
                0x00, 0x02, 0x8A, 0x01, 0x00, 0x02, 0x78, 0x02
            };
        }

        private void TestGetBytes(int id)
        {
            byte[] dataBefore = new byte[4];

            int index = id * dataBefore.Length;
            for (int i = 0; i < dataBefore.Length; i++)
            {
                dataBefore[i] = this.allData[index + i];
            }

            BattleStartPosition startPosition = new BattleStartPosition(dataBefore);
            byte[] dataAfter = startPosition.GetBytes();

            Assert.AreEqual(dataBefore, dataAfter);
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
    }
}
