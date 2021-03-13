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
    internal class BattleTrackTests
    {
        private void TestSmkcImportExport(int trackGroupId, int trackId)
        {
            Game game = File.GetGame(Region.US);

            BattleTrack track1 = (BattleTrack)game.TrackGroups[trackGroupId][trackId];
            BattleTrack track2 = (BattleTrack)game.TrackGroups[4][0];
            string filePath = File.GetOutputPath($"track_{trackGroupId}_{trackId}.smkc");

            track1.Export(filePath, game);
            track2.Import(filePath, game);

            Assert.AreEqual(track1.Map.GetBytes(), track2.Map.GetBytes());
            Assert.AreEqual(game.Themes.GetThemeId(track1.Theme), game.Themes.GetThemeId(track2.Theme));
            Assert.AreEqual(track1.OverlayTiles.GetBytes(), track2.OverlayTiles.GetBytes());
            Assert.AreEqual(track1.StartPositionP1.GetBytes(), track2.StartPositionP1.GetBytes());
            Assert.AreEqual(track1.StartPositionP2.GetBytes(), track2.StartPositionP2.GetBytes());
            Assert.AreEqual(track1.AI.GetBytes(), track2.AI.GetBytes());
        }

        [Test]
        public void TestSmkcImportExport21()
        {
            TestSmkcImportExport(4, 0);
        }

        [Test]
        public void TestSmkcImportExport22()
        {
            TestSmkcImportExport(4, 1);
        }

        [Test]
        public void TestSmkcImportExport23()
        {
            TestSmkcImportExport(4, 2);
        }

        [Test]
        public void TestSmkcImportExport24()
        {
            TestSmkcImportExport(4, 3);
        }
    }
}
