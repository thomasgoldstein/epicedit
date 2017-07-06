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
    internal class TrackTest : TestBase
    {
        private Game game;

        public override void Init()
        {
            this.game = File.GetGame(Region.US);
        }

        private void TestMktImportExport(int trackGroupId, int trackId)
        {
            Track track1 = this.game.TrackGroups[trackGroupId][trackId];
            Track track2 = this.game.TrackGroups[0][0];

            track1.Export("track.mkt", this.game);
            track2.Import("track.mkt", this.game);

            Assert.AreEqual(track1.Map.GetBytes(), track2.Map.GetBytes());
            Assert.AreEqual(game.Themes.GetThemeId(track1.Theme), game.Themes.GetThemeId(track2.Theme));
        }

        [Test]
        public void TestMktImportExport1()
        {
            this.TestMktImportExport(0, 0);
        }

        [Test]
        public void TestMktImportExport2()
        {
            this.TestMktImportExport(0, 1);
        }

        [Test]
        public void TestMktImportExport3()
        {
            this.TestMktImportExport(0, 2);
        }

        [Test]
        public void TestMktImportExport4()
        {
            this.TestMktImportExport(0, 3);
        }

        [Test]
        public void TestMktImportExport5()
        {
            this.TestMktImportExport(0, 4);
        }

        [Test]
        public void TestMktImportExport6()
        {
            this.TestMktImportExport(1, 0);
        }

        [Test]
        public void TestMktImportExport7()
        {
            this.TestMktImportExport(1, 1);
        }

        [Test]
        public void TestMktImportExport8()
        {
            this.TestMktImportExport(1, 2);
        }

        [Test]
        public void TestMktImportExport9()
        {
            this.TestMktImportExport(1, 3);
        }

        [Test]
        public void TestMktImportExport10()
        {
            this.TestMktImportExport(1, 4);
        }

        [Test]
        public void TestMktImportExport11()
        {
            this.TestMktImportExport(2, 0);
        }

        [Test]
        public void TestMktImportExport12()
        {
            this.TestMktImportExport(2, 1);
        }

        [Test]
        public void TestMktImportExport13()
        {
            this.TestMktImportExport(2, 2);
        }

        [Test]
        public void TestMktImportExport14()
        {
            this.TestMktImportExport(2, 3);
        }

        [Test]
        public void TestMktImportExport15()
        {
            this.TestMktImportExport(2, 4);
        }

        [Test]
        public void TestMktImportExport16()
        {
            this.TestMktImportExport(3, 0);
        }

        [Test]
        public void TestMktImportExport17()
        {
            this.TestMktImportExport(3, 1);
        }

        [Test]
        public void TestMktImportExport18()
        {
            this.TestMktImportExport(3, 2);
        }

        [Test]
        public void TestMktImportExport19()
        {
            this.TestMktImportExport(3, 3);
        }

        [Test]
        public void TestMktImportExport20()
        {
            this.TestMktImportExport(3, 4);
        }

        [Test]
        public void TestMktImportExport21()
        {
            this.TestMktImportExport(4, 0);
        }

        [Test]
        public void TestMktImportExport22()
        {
            this.TestMktImportExport(4, 1);
        }

        [Test]
        public void TestMktImportExport23()
        {
            this.TestMktImportExport(4, 2);
        }

        [Test]
        public void TestMktImportExport24()
        {
            this.TestMktImportExport(4, 3);
        }
    }
}
