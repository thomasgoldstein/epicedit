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

namespace EpicEdit.Rom.Tracks.Scenery
{
    /// <summary>
    /// Description of BackgroundTile.
    /// </summary>
    internal sealed class BackgroundTile : Tile2bpp
    {
        /// <summary>
        /// Position at which front background layer palettes begin.
        /// </summary>
        private const int FrontPaletteStart = 4;

        /// <summary>
        /// Position at which back background layer palettes begin.
        /// </summary>
        private const int BackPaletteStart = 6;

        private int paletteStart = FrontPaletteStart;

        private bool front = true;
        public bool Front
        {
            get => this.front;
            set
            {
                if (this.front == value)
                {
                    return;
                }

                this.front = value;
                this.paletteStart = this.front ? FrontPaletteStart : BackPaletteStart;
                this.SetPalette();
            }
        }

        public BackgroundTile(byte[] gfx, Palettes palettes) : this(gfx, palettes, 0, true) { }

        public BackgroundTile(byte[] gfx, Palettes palettes, byte properties, bool front) : base(gfx, null, properties)
        {
            this.Front = front;
            this.Palettes = palettes;
        }

        public override Tile2bppProperties Properties
        {
            get
            {
                Tile2bppProperties props = base.Properties;
                props.PaletteIndex += this.paletteStart;
                return props;
            }
        }
    }
}
