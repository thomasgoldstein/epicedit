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
using System.Drawing.Imaging;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Overlay;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks
{
	[TestFixture]
	public class TrackTest
	{
		private Smk smk;
		private Track track;
		private Theme theme;

		public TrackTest()
		{
			this.smk = new Smk();
			
			byte[] map = new byte[128 * 128];
			for (int x = 0; x < 128; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					map[x + y * 128] = 1;
				}
			}

			Tile[] tiles = new Tile[10];
			for (int i = 0; i < 10; i++)
			{
				Bitmap image = new Bitmap(10, 10, PixelFormat.Format24bppRgb);
				tiles[i] = new StillTile(image, TileGenre.Road);
			}
			this.theme = new Theme("Theme Test", new Palette[16], tiles, tiles, new Music());

			this.track = new GPTrack("Stifu & Midwife's Track", this.theme,
									 map, new byte[128],
									 new byte[0], new byte[0],
									 new byte[6], new byte[6],
									 new byte[44], new byte[10],
									 this.smk.OverlayTileSizes);
		}

		[Test]
		public void TestName()
		{
			Assert.AreEqual("Stifu & Midwife's Track", this.track.Name);
		}

		[Test]
		public void TestMap()
		{
			Assert.AreEqual(1, this.track.Map[0, 0]);
		}

		[Test]
		public void TestTheme()
		{
			Assert.AreEqual(this.theme, this.track.Theme);
		}

		[Test]
		public void TestRoadTiles()
		{
			Assert.AreEqual(10, this.track.GetRoadTile(0).Bitmap.Height);
		}

		[Test]
		public void TestBackgroundTiles()
		{
			Assert.AreEqual(10, this.track.GetBackgroundTile(0).Bitmap.Height);
		}

		// TODO: Test track map export and import
	}
}
