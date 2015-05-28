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

namespace EpicEdit.Test.Rom.Tracks
{
    [TestFixture]
    internal class BattleTrackTest : TestBase
    {
        private Game game;

        public override void Init()
        {
            this.game = File.GetGame(Region.US);
        }

        private void TestSmkcImportExport(int trackGroupId, int trackId)
        {
            BattleTrack track1 = this.game.TrackGroups[trackGroupId][trackId] as BattleTrack;
            BattleTrack track2 = this.game.TrackGroups[4][0] as BattleTrack;

            track1.Export("track.smkc", this.game);
            track2.Import("track.smkc", this.game);

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
            this.TestSmkcImportExport(4, 0);
        }

        [Test]
        public void TestSmkcImportExport22()
        {
            this.TestSmkcImportExport(4, 1);
        }

        [Test]
        public void TestSmkcImportExport23()
        {
            this.TestSmkcImportExport(4, 2);
        }

        [Test]
        public void TestSmkcImportExport24()
        {
            this.TestSmkcImportExport(4, 3);
        }
    }
}
