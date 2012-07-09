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
    /// Represents a collection of background tiles.
    /// </summary>
    internal class BackgroundTileset : IDisposable
    {
        /// <summary>
        /// Total number of tiles in the tileset.
        /// </summary>
        public const int TileCount = 48;

        // TODO: Make setter private
        public bool Modified { get; set; }

        private BackgroundTile[] tileset;

        public BackgroundTileset(BackgroundTile[] tileset)
        {
            this.tileset = tileset;
        }

        public BackgroundTile[] GetTiles()
        {
            return this.tileset;
        }

        public BackgroundTile GetTile(int index)
        {
            return this.tileset[index];
        }

        public BackgroundTile this[int index]
        {
            get { return this.GetTile(index); }
        }

        public void ResetModifiedFlag()
        {
            this.Modified = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Tile tile in this.tileset)
                {
                    tile.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
