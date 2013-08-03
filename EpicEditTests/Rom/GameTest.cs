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
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEditTests.Rom
{
    [TestFixture]
    internal class GameTest
    {
        private Game rom;

        public GameTest()
        {
            this.rom = new Game(File.RelativePath + "smk.smc");
        }

        [Test]
        public void TestTrackGroupNumber()
        {
            Assert.AreEqual(5, this.rom.TrackGroups.Count);
        }

        [Test]
        public void TestTrackSizes()
        {
            foreach (TrackGroup trackGroup in this.rom.TrackGroups)
            {
                foreach (Track track in trackGroup)
                {
                    Assert.AreEqual(128, track.Map.Width);
                    Assert.AreEqual(128, track.Map.Height);
                }
            }
        }

        [Test]
        public void TestTrackGroupName1()
        {
            Assert.AreEqual("Mushroom Cup", this.rom.TrackGroups[0].Name);
        }

        [Test]
        public void TestTrackGroupName2()
        {
            Assert.AreEqual("Flower Cup", this.rom.TrackGroups[1].Name);
        }

        [Test]
        public void TestTrackGroupName3()
        {
            Assert.AreEqual("Star Cup", this.rom.TrackGroups[2].Name);
        }

        [Test]
        public void TestTrackGroupName4()
        {
            Assert.AreEqual("Special Cup", this.rom.TrackGroups[3].Name);
        }

        [Test]
        public void TestTrackGroupName5()
        {
            Assert.AreEqual("Battle Course ", this.rom.TrackGroups[4].Name);
        }

        [Test]
        public void TestTrackName1()
        {
            Assert.AreEqual("Mario Circuit 1", this.rom.TrackGroups[0][0].Name);
        }

        [Test]
        public void TestTrackName2()
        {
            Assert.AreEqual("Donut Plains 1", this.rom.TrackGroups[0][1].Name);
        }

        [Test]
        public void TestTrackName3()
        {
            Assert.AreEqual("Ghost Valley 1", this.rom.TrackGroups[0][2].Name);
        }

        [Test]
        public void TestTrackName20()
        {
            Assert.AreEqual("Rainbow Road ", this.rom.TrackGroups[3][4].Name);
        }

        [Test]
        public void TestTrackTheme1()
        {
            Assert.AreEqual("Mario Circuit ", this.rom.TrackGroups[0][0].Theme.Name);
        }

        [Test]
        public void TestTrackTheme2()
        {
            Assert.AreEqual("Donut Plains ", this.rom.TrackGroups[0][1].Theme.Name);
        }

        [Test]
        public void TestTrackTheme3()
        {
            Assert.AreEqual("Ghost Valley ", this.rom.TrackGroups[0][2].Theme.Name);
        }
    }
}
