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

using EpicEdit.Rom.Tracks.Scenery;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Tracks.Scenery
{
    [TestFixture]
    internal class BackgroundLayoutTests
    {
        private void TestGetBytes(string fileName)
        {
            var dataBefore = File.ReadFile(fileName);
            var layout = new BackgroundLayout(dataBefore);
            var dataAfter = layout.GetBytes();
            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes1()
        {
            TestGetBytes("bglayoutgv.smc");
        }

        [Test]
        public void TestGetBytes2()
        {
            TestGetBytes("bglayoutmc.smc");
        }

        [Test]
        public void TestGetBytes3()
        {
            TestGetBytes("bglayoutdp.smc");
        }

        [Test]
        public void TestGetBytes4()
        {
            TestGetBytes("bglayoutci.smc");
        }

        [Test]
        public void TestGetBytes5()
        {
            TestGetBytes("bglayoutvl.smc");
        }

        [Test]
        public void TestGetBytes6()
        {
            TestGetBytes("bglayoutkp.smc");
        }

        [Test]
        public void TestGetBytes7()
        {
            TestGetBytes("bglayoutbc.smc");
        }

        [Test]
        public void TestGetBytes8()
        {
            TestGetBytes("bglayoutrr.smc");
        }
    }
}
