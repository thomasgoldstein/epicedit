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
    internal class OverlayTileSizesTest
    {
        private Smk smk;

        public OverlayTileSizesTest()
        {
            this.smk = new Smk();
        }

        [Test]
        public void TestSave()
        {
            int offset = this.smk.Offsets[Offset.TrackOverlaySizes];
            byte[] dataBefore = File.ReadBlock(this.smk.RomBuffer, offset, 8);
            OverlayTileSizes sizes = new OverlayTileSizes(this.smk.RomBuffer, offset);

            byte[] dataAfter = new byte[8];
            sizes.Save(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }
    }
}
