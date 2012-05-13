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
using EpicEdit.Rom.Tracks.Scenery;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks.Scenery
{
    [TestFixture]
    internal class BackgroundLayoutTest
    {
        private void TestGetBytes(string fileName)
        {
            byte[] dataBefore = File.ReadFile(fileName);
            BackgroundLayout layout = new BackgroundLayout(dataBefore);
            byte[] dataAfter = layout.GetBytes();
            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes1()
        {
            this.TestGetBytes("bglayoutgv.smc");
        }

        [Test]
        public void TestGetBytes2()
        {
            this.TestGetBytes("bglayoutmc.smc");
        }

        [Test]
        public void TestGetBytes3()
        {
            this.TestGetBytes("bglayoutdp.smc");
        }

        [Test]
        public void TestGetBytes4()
        {
            this.TestGetBytes("bglayoutci.smc");
        }

        [Test]
        public void TestGetBytes5()
        {
            this.TestGetBytes("bglayoutvl.smc");
        }

        [Test]
        public void TestGetBytes6()
        {
            this.TestGetBytes("bglayoutkp.smc");
        }

        [Test]
        public void TestGetBytes7()
        {
            this.TestGetBytes("bglayoutbc.smc");
        }

        [Test]
        public void TestGetBytes8()
        {
            this.TestGetBytes("bglayoutrr.smc");
        }
    }
}
