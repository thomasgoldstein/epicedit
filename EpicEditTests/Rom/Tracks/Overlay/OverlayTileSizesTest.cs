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
using EpicEdit.Rom.Tracks.Overlay;
using NUnit.Framework;

namespace EpicEditTests.Rom.Tracks.Overlay
{
	[TestFixture]
	public class OverlayTileSizesTest
	{
		private Smk smk;

		public OverlayTileSizesTest()
		{
			this.smk = new Smk();
		}

		[Test]
		public void TestSave()
		{
			byte[] dataBefore = File.ReadBlock(this.smk.RomBuffer, this.smk.Offsets[Offset.TrackOverlaySizes], 8);
			OverlayTileSizes sizes = new OverlayTileSizes(this.smk.RomBuffer, this.smk.Offsets);
			this.smk.Offsets[Offset.TrackOverlaySizes] = 0; // Trick the OverlayTileSizes class to write at the beginning of our dataAfter array
			byte[] dataAfter = new byte[8];
			sizes.Save(dataAfter, this.smk.Offsets);

			Assert.AreEqual(dataBefore, dataAfter);
		}
	}
}
