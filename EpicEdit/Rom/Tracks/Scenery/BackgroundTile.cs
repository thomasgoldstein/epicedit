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
        private const int FrontBackgroundPaletteStart = 4;

        /// <summary>
        /// Position at which back background layer palettes begin.
        /// </summary>
        private const int BackBackgroundPaletteStart = 6;

        public BackgroundTile(byte[] gfx, Palettes palettes) : base(gfx, palettes) { }

        public BackgroundTile(byte[] gfx, Palettes palettes, byte properties, bool front) : base(gfx, null, properties)
        {
            this.properties.PaletteIndex += front ? FrontBackgroundPaletteStart : BackBackgroundPaletteStart;
            this.Palettes = palettes;
        }
    }
}
