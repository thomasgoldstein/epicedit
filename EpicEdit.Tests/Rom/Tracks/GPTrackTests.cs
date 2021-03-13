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

namespace EpicEdit.Tests.Rom.Tracks
{
    [TestFixture]
    internal class GPTrackTests
    {
        private void TestSmkcImportExport(int trackGroupId, int trackId)
        {
            Game game = File.GetGame(Region.US);

            GPTrack track1 = game.TrackGroups[trackGroupId][trackId] as GPTrack;
            GPTrack track2 = game.TrackGroups[0][0] as GPTrack;
            string filePath = File.GetOutputPath($"track_{trackGroupId}_{trackId}.smkc");

            track1.Export(filePath, game);
            track2.Import(filePath, game);

            Assert.AreEqual(track1.Map.GetBytes(), track2.Map.GetBytes());
            Assert.AreEqual(track1.Theme, track2.Theme);
            Assert.AreEqual(track1.OverlayTiles.GetBytes(), track2.OverlayTiles.GetBytes());
            Assert.AreEqual(track1.StartPosition.GetBytes(), track2.StartPosition.GetBytes());
            Assert.AreEqual(track1.LapLine.GetBytes(), track2.LapLine.GetBytes());
            Assert.AreEqual(track1.Objects.GetBytes(), track2.Objects.GetBytes());
            Assert.AreEqual(track1.Objects.Areas.GetBytes(), track2.Objects.Areas.GetBytes());
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
            TestSmkcImportExport(0, 0);
        }

        [Test]
        public void TestSmkcImportExport2()
        {
            TestSmkcImportExport(0, 1);
        }

        [Test]
        public void TestSmkcImportExport3()
        {
            TestSmkcImportExport(0, 2);
        }

        [Test]
        public void TestSmkcImportExport4()
        {
            TestSmkcImportExport(0, 3);
        }

        [Test]
        public void TestSmkcImportExport5()
        {
            TestSmkcImportExport(0, 4);
        }

        [Test]
        public void TestSmkcImportExport6()
        {
            TestSmkcImportExport(1, 0);
        }

        [Test]
        public void TestSmkcImportExport7()
        {
            TestSmkcImportExport(1, 1);
        }

        [Test]
        public void TestSmkcImportExport8()
        {
            TestSmkcImportExport(1, 2);
        }

        [Test]
        public void TestSmkcImportExport9()
        {
            TestSmkcImportExport(1, 3);
        }

        [Test]
        public void TestSmkcImportExport10()
        {
            TestSmkcImportExport(1, 4);
        }

        [Test]
        public void TestSmkcImportExport11()
        {
            TestSmkcImportExport(2, 0);
        }

        [Test]
        public void TestSmkcImportExport12()
        {
            TestSmkcImportExport(2, 1);
        }

        [Test]
        public void TestSmkcImportExport13()
        {
            TestSmkcImportExport(2, 2);
        }

        [Test]
        public void TestSmkcImportExport14()
        {
            TestSmkcImportExport(2, 3);
        }

        [Test]
        public void TestSmkcImportExport15()
        {
            TestSmkcImportExport(2, 4);
        }

        [Test]
        public void TestSmkcImportExport16()
        {
            TestSmkcImportExport(3, 0);
        }

        [Test]
        public void TestSmkcImportExport17()
        {
            TestSmkcImportExport(3, 1);
        }

        [Test]
        public void TestSmkcImportExport18()
        {
            TestSmkcImportExport(3, 2);
        }

        [Test]
        public void TestSmkcImportExport19()
        {
            TestSmkcImportExport(3, 3);
        }

        [Test]
        public void TestSmkcImportExport20()
        {
            TestSmkcImportExport(3, 4);
        }
    }
}
