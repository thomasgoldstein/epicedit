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
        private bool _front = true;
        public bool Front
        {
            get => _front;
            set
            {
                if (_front == value)
                {
                    return;
                }

                _front = value;
                SetPalette();
            }
        }

        public BackgroundTile(byte[] gfx, Palettes palettes) : this(gfx, palettes, 0, true) { }

        public BackgroundTile(byte[] gfx, Palettes palettes, byte properties, bool front) : base(gfx, null, properties)
        {
            Front = front;
            Palettes = palettes;
        }

        public override Tile2bppProperties Properties
        {
            get
            {
                const int frontPaletteStart = 4; // Position at which front background layer palettes begin
                const int backPaletteStart = 6; // Position at which back background layer palettes begin

                var paletteStart = _front ? frontPaletteStart : backPaletteStart;
                var props = base.Properties;
                props.PaletteIndex += paletteStart;
                return props;
            }
        }
    }
}
