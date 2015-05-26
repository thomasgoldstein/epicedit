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

namespace EpicEdit.Test.Rom
{
    [TestFixture]
    internal class GameTest
    {
        private readonly Game game;

        public GameTest()
        {
            this.game = File.GetGame(Region.US);
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
            Assert.AreEqual("Mushroom Cup", this.game.TrackGroups[0].Name);
        }

        [Test]
        public void TestTrackGroupName2()
        {
            Assert.AreEqual("Flower Cup", this.game.TrackGroups[1].Name);
        }

        [Test]
        public void TestTrackGroupName3()
        {
            Assert.AreEqual("Star Cup", this.game.TrackGroups[2].Name);
        }

        [Test]
        public void TestTrackGroupName4()
        {
            Assert.AreEqual("Special Cup", this.game.TrackGroups[3].Name);
        }

        [Test]
        public void TestTrackGroupName5()
        {
            Assert.AreEqual("Battle Course ", this.game.TrackGroups[4].Name);
        }

        [Test]
        public void TestTrackName1()
        {
            Assert.AreEqual("Mario Circuit 1", this.game.TrackGroups[0][0].Name);
        }

        [Test]
        public void TestTrackName2()
        {
            Assert.AreEqual("Donut Plains 1", this.game.TrackGroups[0][1].Name);
        }

        [Test]
        public void TestTrackName3()
        {
            Assert.AreEqual("Ghost Valley 1", this.game.TrackGroups[0][2].Name);
        }

        [Test]
        public void TestTrackName20()
        {
            Assert.AreEqual("Rainbow Road ", this.game.TrackGroups[3][4].Name);
        }

        [Test]
        public void TestTrackTheme1()
        {
            Assert.AreEqual("Mario Circuit ", this.game.TrackGroups[0][0].Theme.Name);
        }

        [Test]
        public void TestTrackTheme2()
        {
            Assert.AreEqual("Donut Plains ", this.game.TrackGroups[0][1].Theme.Name);
        }

        [Test]
        public void TestTrackTheme3()
        {
            Assert.AreEqual("Ghost Valley ", this.game.TrackGroups[0][2].Theme.Name);
        }

        [Test]
        public void TestGPTrackReorderReferences()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            Track[] tracks =
            {
                game.TrackGroups[0][0],
                game.TrackGroups[0][1],
                game.TrackGroups[0][2],
                game.TrackGroups[0][3],
                game.TrackGroups[0][4],
                game.TrackGroups[1][0],
                game.TrackGroups[1][1],
                game.TrackGroups[1][2],
                game.TrackGroups[1][3],
                game.TrackGroups[1][4]
            };

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            Assert.AreEqual(tracks[0], game.TrackGroups[0][0]);
            Assert.AreEqual(tracks[2], game.TrackGroups[0][1]);
            Assert.AreEqual(tracks[3], game.TrackGroups[0][2]);
            Assert.AreEqual(tracks[4], game.TrackGroups[0][3]);
            Assert.AreEqual(tracks[5], game.TrackGroups[0][4]);
            Assert.AreEqual(tracks[6], game.TrackGroups[1][0]);
            Assert.AreEqual(tracks[7], game.TrackGroups[1][1]);
            Assert.AreEqual(tracks[8], game.TrackGroups[1][2]);
            Assert.AreEqual(tracks[1], game.TrackGroups[1][3]);
            Assert.AreEqual(tracks[9], game.TrackGroups[1][4]);
        }

        [Test]
        public void TestGPTrackReorderNames()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);
        }

        [Test]
        public void TestGPTrackReorderNamesReload()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            string fileNameAfter = "SMK_U_After_Track_Reorder.smc";

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            game.SaveRom(fileNameAfter);
            game = new Game(fileNameAfter); // Reload ROM

            Assert.AreEqual("Mario Circuit 1", game.TrackGroups[0][0].Name);
            Assert.AreEqual("Ghost Valley 1", game.TrackGroups[0][1].Name);
            Assert.AreEqual("Bowser Castle 1", game.TrackGroups[0][2].Name);
            Assert.AreEqual("Mario Circuit 2", game.TrackGroups[0][3].Name);
            Assert.AreEqual("Choco Island 1", game.TrackGroups[0][4].Name);
            Assert.AreEqual("Ghost Valley 2", game.TrackGroups[1][0].Name);
            Assert.AreEqual("Donut Plains 2", game.TrackGroups[1][1].Name);
            Assert.AreEqual("Bowser Castle 2", game.TrackGroups[1][2].Name);
            Assert.AreEqual("Donut Plains 1", game.TrackGroups[1][3].Name);
            Assert.AreEqual("Mario Circuit 3", game.TrackGroups[1][4].Name);
        }

        [Test]
        public void TestGPTrackReorderRomData()
        {
            // Since we actually modify the Game object in this method,
            // do not use the private Game member, because that would affect other tests.
            Game game = File.GetGame(Region.US);

            string fileNameBefore = "SMK_U_Before_Track_Reorder.smc";
            string fileNameAfter = "SMK_U_After_Track_Reorder.smc";

            game.SaveRom(fileNameBefore);
            byte[] romBefore = System.IO.File.ReadAllBytes(fileNameBefore);

            // Check the order value of each GP track
            Assert.AreEqual(0x07, romBefore[0x1EC1B]);
            Assert.AreEqual(0x13, romBefore[0x1EC1C]);
            Assert.AreEqual(0x10, romBefore[0x1EC1D]);
            Assert.AreEqual(0x11, romBefore[0x1EC1E]);
            Assert.AreEqual(0x0F, romBefore[0x1EC1F]);
            Assert.AreEqual(0x12, romBefore[0x1EC20]);
            Assert.AreEqual(0x01, romBefore[0x1EC21]);
            Assert.AreEqual(0x02, romBefore[0x1EC22]);
            Assert.AreEqual(0x03, romBefore[0x1EC23]);
            Assert.AreEqual(0x00, romBefore[0x1EC24]);
            Assert.AreEqual(0x0D, romBefore[0x1EC25]);
            Assert.AreEqual(0x0A, romBefore[0x1EC26]);
            Assert.AreEqual(0x0C, romBefore[0x1EC27]);
            Assert.AreEqual(0x09, romBefore[0x1EC28]);
            Assert.AreEqual(0x0E, romBefore[0x1EC29]);
            Assert.AreEqual(0x0B, romBefore[0x1EC2A]);
            Assert.AreEqual(0x06, romBefore[0x1EC2B]);
            Assert.AreEqual(0x08, romBefore[0x1EC2C]);
            Assert.AreEqual(0x04, romBefore[0x1EC2D]);
            Assert.AreEqual(0x05, romBefore[0x1EC2E]);

            // Take Donut Plains, and move it where Bowser Castle 2 is
            game.ReorderTracks(0, 1, 1, 3);

            game.SaveRom(fileNameAfter);
            byte[] romAfter = System.IO.File.ReadAllBytes(fileNameAfter);

            // Check the order value of each GP track
            Assert.AreEqual(0x07, romAfter[0x1EC1B]);
            Assert.AreEqual(0x10, romAfter[0x1EC1C]);
            Assert.AreEqual(0x11, romAfter[0x1EC1D]);
            Assert.AreEqual(0x0F, romAfter[0x1EC1E]);
            Assert.AreEqual(0x12, romAfter[0x1EC1F]);
            Assert.AreEqual(0x01, romAfter[0x1EC20]);
            Assert.AreEqual(0x02, romAfter[0x1EC21]);
            Assert.AreEqual(0x03, romAfter[0x1EC22]);
            Assert.AreEqual(0x13, romAfter[0x1EC23]);
            Assert.AreEqual(0x00, romAfter[0x1EC24]);
            Assert.AreEqual(0x0D, romAfter[0x1EC25]);
            Assert.AreEqual(0x0A, romAfter[0x1EC26]);
            Assert.AreEqual(0x0C, romAfter[0x1EC27]);
            Assert.AreEqual(0x09, romAfter[0x1EC28]);
            Assert.AreEqual(0x0E, romAfter[0x1EC29]);
            Assert.AreEqual(0x0B, romAfter[0x1EC2A]);
            Assert.AreEqual(0x06, romAfter[0x1EC2B]);
            Assert.AreEqual(0x08, romAfter[0x1EC2C]);
            Assert.AreEqual(0x04, romAfter[0x1EC2D]);
            Assert.AreEqual(0x05, romAfter[0x1EC2E]);
        }
    }
}
