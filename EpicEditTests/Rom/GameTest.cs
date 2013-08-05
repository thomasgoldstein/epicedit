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
        private Game game;

        public GameTest()
        {
            this.game = new Game(File.RelativePath + "Super Mario Kart (U) [!].smc");
        }

        [Test]
        public void TestTrackGroupNumber()
        {
            Assert.AreEqual(5, this.game.TrackGroups.Count);
        }

        [Test]
        public void TestTrackSizes()
        {
            foreach (TrackGroup trackGroup in this.game.TrackGroups)
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
            Assert.AreEqual("Mushroom Cup", this.game.TrackGroups[0].Name);
        }

        [Test]
        public void TestTrackGroupName2()
        {
            Assert.AreEqual("Flower Cup", this.game.TrackGroups[1].Name);
        }

        [Test]
        public void TestTrackGroupName3()
        {
            Assert.AreEqual("Star Cup", this.game.TrackGroups[2].Name);
        }

        [Test]
        public void TestTrackGroupName4()
        {
            Assert.AreEqual("Special Cup", this.game.TrackGroups[3].Name);
        }

        [Test]
        public void TestTrackGroupName5()
        {
            Assert.AreEqual("Battle Course ", this.game.TrackGroups[4].Name);
        }

        [Test]
        public void TestTrackName1()
        {
            Assert.AreEqual("Mario Circuit 1", this.game.TrackGroups[0][0].Name);
        }

        [Test]
        public void TestTrackName2()
        {
            Assert.AreEqual("Donut Plains 1", this.game.TrackGroups[0][1].Name);
        }

        [Test]
        public void TestTrackName3()
        {
            Assert.AreEqual("Ghost Valley 1", this.game.TrackGroups[0][2].Name);
        }

        [Test]
        public void TestTrackName20()
        {
            Assert.AreEqual("Rainbow Road ", this.game.TrackGroups[3][4].Name);
        }

        [Test]
        public void TestTrackTheme1()
        {
            Assert.AreEqual("Mario Circuit ", this.game.TrackGroups[0][0].Theme.Name);
        }

        [Test]
        public void TestTrackTheme2()
        {
            Assert.AreEqual("Donut Plains ", this.game.TrackGroups[0][1].Theme.Name);
        }

        [Test]
        public void TestTrackTheme3()
        {
            Assert.AreEqual("Ghost Valley ", this.game.TrackGroups[0][2].Theme.Name);
        }
    }
}
