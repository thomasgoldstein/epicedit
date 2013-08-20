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
using EpicEdit.Rom.Tracks.Road;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Tracks.Road
{
    [TestFixture]
    internal class TrackMapTest
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
            this.TestTrackMap("track1.smc");
        }

        [Test]
        public void TestTrackMap2()
        {
            this.TestTrackMap("track2.smc");
        }

        [Test]
        public void TestTrackMap3()
        {
            this.TestTrackMap("track3.smc");
        }

        [Test]
        public void TestTrackMap4()
        {
            this.TestTrackMap("track4.smc");
        }

        [Test]
        public void TestTrackMap5()
        {
            this.TestTrackMap("track5.smc");
        }

        [Test]
        public void TestTrackMap6()
        {
            this.TestTrackMap("track6.smc");
        }

        [Test]
        public void TestTrackMap7()
        {
            this.TestTrackMap("track7.smc");
        }

        [Test]
        public void TestTrackMap8()
        {
            this.TestTrackMap("track8.smc");
        }

        [Test]
        public void TestTrackMap9()
        {
            this.TestTrackMap("track9.smc");
        }

        [Test]
        public void TestTrackMap10()
        {
            this.TestTrackMap("track10.smc");
        }

        [Test]
        public void TestTrackMap11()
        {
            this.TestTrackMap("track11.smc");
        }

        [Test]
        public void TestTrackMap12()
        {
            this.TestTrackMap("track12.smc");
        }

        [Test]
        public void TestTrackMap13()
        {
            this.TestTrackMap("track13.smc");
        }

        [Test]
        public void TestTrackMap14()
        {
            this.TestTrackMap("track14.smc");
        }

        [Test]
        public void TestTrackMap15()
        {
            this.TestTrackMap("track15.smc");
        }

        [Test]
        public void TestTrackMap16()
        {
            this.TestTrackMap("track16.smc");
        }

        [Test]
        public void TestTrackMap17()
        {
            this.TestTrackMap("track17.smc");
        }

        [Test]
        public void TestTrackMap18()
        {
            this.TestTrackMap("track18.smc");
        }

        [Test]
        public void TestTrackMap19()
        {
            this.TestTrackMap("track19.smc");
        }

        [Test]
        public void TestTrackMap20()
        {
            this.TestTrackMap("track20.smc");
        }
    }
}
