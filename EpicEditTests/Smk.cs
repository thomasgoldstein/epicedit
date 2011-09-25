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

namespace EpicEditTests
{
    public class Smk
    {
        public byte[] RomBuffer { get; private set; }
        public Offsets Offsets { get; private set; }
        public OverlayTilePatterns OverlayTilePatterns { get; private set; }
        public OverlayTileSizes OverlayTileSizes { get; private set; }

        public Smk()
        {
            byte[] headeredRomBuffer = File.ReadFile("smk.smc");
            this.RomBuffer = File.ReadBlock(headeredRomBuffer, 512, headeredRomBuffer.Length - 512);
            this.Offsets = new Offsets(this.RomBuffer, Region.US);
            this.OverlayTileSizes = new OverlayTileSizes(this.RomBuffer, this.Offsets[Offset.TrackOverlaySizes]);
            this.OverlayTilePatterns = new OverlayTilePatterns(this.RomBuffer, this.Offsets, this.OverlayTileSizes);
        }
    }
}
