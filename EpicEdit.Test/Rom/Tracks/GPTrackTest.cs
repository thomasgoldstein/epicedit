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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Tracks
{
    [TestFixture]
    internal class GPTrackTest : TestBase
    {
        private Game game;

        public override void Init()
        {
            this.game = File.GetGame(Region.US);
        }

        private void TestSmkcImportExport(int trackGroupId, int trackId)
        {
            GPTrack track1 = this.game.TrackGroups[trackGroupId][trackId] as GPTrack;
            GPTrack track2 = this.game.TrackGroups[0][0] as GPTrack;

            track1.Export("track.smkc", this.game);
            track2.Import("track.smkc", this.game);

            Assert.AreEqual(track1.Map.GetBytes(), track2.Map.GetBytes());
            Assert.AreEqual(track1.Theme, track2.Theme);
            Assert.AreEqual(track1.OverlayTiles.GetBytes(), track2.OverlayTiles.GetBytes());
            Assert.AreEqual(track1.StartPosition.GetBytes(), track2.StartPosition.GetBytes());
            Assert.AreEqual(track1.LapLine.GetBytes(), track2.LapLine.GetBytes());
            Assert.AreEqual(track1.Objects.GetBytes(), track2.Objects.GetBytes());
            Assert.AreEqual(track1.Objects.Zones.GetBytes(), track2.Objects.Zones.GetBytes());
            Assert.AreEqual(track1.Objects.Tileset, track2.Objects.Tileset);
            Assert.AreEqual(track1.Objects.Interaction, track2.Objects.Interaction);
            Assert.AreEqual(track1.Objects.Routine, track2.Objects.Routine);
            Assert.AreEqual(track1.Objects.PaletteIndexes.GetBytes(), track2.Objects.PaletteIndexes.GetBytes());
            Assert.AreEqual(track1.Objects.Flashing, track2.Objects.Flashing);
            Assert.AreEqual(track1.AI.GetBytes(), track2.AI.GetBytes());
        }

        [Test]
        public void TestSmkcImportExport1()
        {
            this.TestSmkcImportExport(0, 0);
        }

        [Test]
        public void TestSmkcImportExport2()
        {
            this.TestSmkcImportExport(0, 1);
        }

        [Test]
        public void TestSmkcImportExport3()
        {
            this.TestSmkcImportExport(0, 2);
        }

        [Test]
        public void TestSmkcImportExport4()
        {
            this.TestSmkcImportExport(0, 3);
        }

        [Test]
        public void TestSmkcImportExport5()
        {
            this.TestSmkcImportExport(0, 4);
        }

        [Test]
        public void TestSmkcImportExport6()
        {
            this.TestSmkcImportExport(1, 0);
        }

        [Test]
        public void TestSmkcImportExport7()
        {
            this.TestSmkcImportExport(1, 1);
        }

        [Test]
        public void TestSmkcImportExport8()
        {
            this.TestSmkcImportExport(1, 2);
        }

        [Test]
        public void TestSmkcImportExport9()
        {
            this.TestSmkcImportExport(1, 3);
        }

        [Test]
        public void TestSmkcImportExport10()
        {
            this.TestSmkcImportExport(1, 4);
        }

        [Test]
        public void TestSmkcImportExport11()
        {
            this.TestSmkcImportExport(2, 0);
        }

        [Test]
        public void TestSmkcImportExport12()
        {
            this.TestSmkcImportExport(2, 1);
        }

        [Test]
        public void TestSmkcImportExport13()
        {
            this.TestSmkcImportExport(2, 2);
        }

        [Test]
        public void TestSmkcImportExport14()
        {
            this.TestSmkcImportExport(2, 3);
        }

        [Test]
        public void TestSmkcImportExport15()
        {
            this.TestSmkcImportExport(2, 4);
        }

        [Test]
        public void TestSmkcImportExport16()
        {
            this.TestSmkcImportExport(3, 0);
        }

        [Test]
        public void TestSmkcImportExport17()
        {
            this.TestSmkcImportExport(3, 1);
        }

        [Test]
        public void TestSmkcImportExport18()
        {
            this.TestSmkcImportExport(3, 2);
        }

        [Test]
        public void TestSmkcImportExport19()
        {
            this.TestSmkcImportExport(3, 3);
        }

        [Test]
        public void TestSmkcImportExport20()
        {
            this.TestSmkcImportExport(3, 4);
        }
    }
}
