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
using System.Drawing;
using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks
{
    [TestFixture]
    public class ThemeTest
    {
        int count;
        Theme theme;

        public ThemeTest()
        {
            this.count = 64;
            this.theme = new Theme("Stifu's & Midwife theme", new Palettes(new byte[0]), new Tile[this.count], new Tile[this.count]);
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Stifu's & Midwife theme", this.theme.Name);
        }

        [Test]
        public void TestRoadTileset()
        {
            Tile[] tiles = new StillTile[this.count];
            for (int i = 0; i < this.count; i++)
            {
                tiles[i] = new StillTile(new Bitmap(1, 1), TileGenre.Road);
            }
            this.theme.SetRoadTileset(tiles);
            Assert.AreEqual(this.theme.GetRoadTile(0), tiles[0]);
            Assert.AreEqual(this.theme.GetRoadTile(63), tiles[63]);
        }

        [Test]
        public void TestRoadTiles()
        {
            Tile[] tiles = new StillTile[this.count];
            this.theme.ClearRoadTileset();
            for (int i = 0; i < this.count; i++)
            {
                tiles[i] = new StillTile(new Bitmap(1, 1), TileGenre.Road);
                this.theme.SetRoadTile(i, tiles[i]);
            }

            for (int i = 0; i < this.count; i++)
            {
                Assert.AreEqual(this.theme.GetRoadTile(i), tiles[i]);
            }
        }

        [Test]
        public void TestBackgroundTileset()
        {
            Tile[] tiles = new StillTile[this.count];
            for (int i = 0; i < this.count; i++)
            {
                tiles[i] = new StillTile(new Bitmap(1, 1), TileGenre.Road);
            }
            this.theme.SetBackgroundTileset(tiles);

            for (int i = 0; i < this.count; i++)
            {
                Assert.AreEqual(this.theme.GetBackgroundTile(i), tiles[i]);
            }
        }

        [Test]
        public void TestBackgroundTiles()
        {
            Tile[] tiles = new StillTile[this.count];
            this.theme.ClearBackgroundTileset();
            for (int i = 0; i < this.count; i++)
            {
                tiles[i] = new StillTile(new Bitmap(1, 1), TileGenre.Road);
                this.theme.SetBackgroundTile(i, tiles[i]);
            }

            for (int i = 0; i < this.count; i++)
            {
                Assert.AreEqual(this.theme.GetBackgroundTile(i), tiles[i]);
            }
        }
    }
}
