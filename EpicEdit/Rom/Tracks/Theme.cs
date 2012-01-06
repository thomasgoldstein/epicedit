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

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents the graphics set and music of a track.
    /// </summary>
    internal sealed class Theme : IDisposable
    {
        /// <summary>
        /// Number of themes.
        /// </summary>
        public const int Count = 8;

        public string Name { get; private set; }
        public Palettes Palettes { get; private set; }
        public RoadTileset RoadTileset { get; private set; }
        // TODO: Add support for background tilesets

        public Theme(string name, Palettes palettes, RoadTile[] roadTileset)
        {
            this.Name = name;
            this.Palettes = palettes;
            this.Palettes.Theme = this;
            this.RoadTileset = new RoadTileset(roadTileset);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Dispose()
        {
            this.RoadTileset.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
