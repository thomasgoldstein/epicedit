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
using EpicEdit.Rom.Tracks.Overlay;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks
{
    [TestFixture]
    public class TrackGroupTest
    {
        private Smk smk;
        private TrackGroup trackGroup;

        public TrackGroupTest()
        {
            this.smk = new Smk();

            byte[] map = new byte[128 * 128];

            Tile[] tiles = new MapTile[2];
            tiles[0] = new MapTile(new Bitmap(1, 1), new Palette(), TileGenre.Road);
            tiles[1] = new MapTile(new Bitmap(1, 1), new Palette(), TileGenre.Road);

            Theme theme = new Theme("TestTheme", new Palettes(new byte[0]), tiles, tiles);

            Track[] tracks = new Track[2];
            tracks[0] = new GPTrack("Test 1", theme,
                                    map, new byte[128],
                                    new byte[0], new byte[0],
                                    new byte[6], new byte[6],
                                    new byte[44], new byte[10],
                                    this.smk.OverlayTileSizes,
                                    this.smk.OverlayTilePatterns);

            tracks[1] = new GPTrack("Test 2", theme,
                                    map, new byte[128],
                                    new byte[0], new byte[0],
                                    new byte[6], new byte[6],
                                    new byte[44], new byte[10],
                                    this.smk.OverlayTileSizes,
                                    this.smk.OverlayTilePatterns);

            this.trackGroup = new TrackGroup("Flower Cup", tracks);
        }

        [Test]
        public void TestName()
        {
            Assert.AreEqual("Flower Cup", this.trackGroup.Name);
        }

        [Test]
        public void TestNumberOfTracksInside()
        {
            Assert.AreEqual(2, this.trackGroup.GetTracks().Length);
        }
    }
}
