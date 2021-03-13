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

using EpicEdit.Rom.Tracks.Road;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Tracks.Road
{
    [TestFixture]
    internal class TrackMapTests
    {
        private void TestTrackMap(string fileName)
        {
            byte[] dataBefore = File.ReadFile(fileName);
            TrackMap map = new TrackMap(dataBefore);
            byte[] dataAfter = map.GetBytes();
            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestTrackMap1()
        {
            TestTrackMap("track1.smc");
        }

        [Test]
        public void TestTrackMap2()
        {
            TestTrackMap("track2.smc");
        }

        [Test]
        public void TestTrackMap3()
        {
            TestTrackMap("track3.smc");
        }

        [Test]
        public void TestTrackMap4()
        {
            TestTrackMap("track4.smc");
        }

        [Test]
        public void TestTrackMap5()
        {
            TestTrackMap("track5.smc");
        }

        [Test]
        public void TestTrackMap6()
        {
            TestTrackMap("track6.smc");
        }

        [Test]
        public void TestTrackMap7()
        {
            TestTrackMap("track7.smc");
        }

        [Test]
        public void TestTrackMap8()
        {
            TestTrackMap("track8.smc");
        }

        [Test]
        public void TestTrackMap9()
        {
            TestTrackMap("track9.smc");
        }

        [Test]
        public void TestTrackMap10()
        {
            TestTrackMap("track10.smc");
        }

        [Test]
        public void TestTrackMap11()
        {
            TestTrackMap("track11.smc");
        }

        [Test]
        public void TestTrackMap12()
        {
            TestTrackMap("track12.smc");
        }

        [Test]
        public void TestTrackMap13()
        {
            TestTrackMap("track13.smc");
        }

        [Test]
        public void TestTrackMap14()
        {
            TestTrackMap("track14.smc");
        }

        [Test]
        public void TestTrackMap15()
        {
            TestTrackMap("track15.smc");
        }

        [Test]
        public void TestTrackMap16()
        {
            TestTrackMap("track16.smc");
        }

        [Test]
        public void TestTrackMap17()
        {
            TestTrackMap("track17.smc");
        }

        [Test]
        public void TestTrackMap18()
        {
            TestTrackMap("track18.smc");
        }

        [Test]
        public void TestTrackMap19()
        {
            TestTrackMap("track19.smc");
        }

        [Test]
        public void TestTrackMap20()
        {
            TestTrackMap("track20.smc");
        }
    }
}
