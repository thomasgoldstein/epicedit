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
    public sealed class Theme : IDisposable
    {
        /// <summary>
        /// Number of themes.
        /// </summary>
        public const int Count = 8;

        public string Name { get; private set; }
        public Palettes Palettes { get; private set; }
        private Tile[] roadTileset;
        private Tile[] backgroundTileset;

        public Theme(string name, Palettes palettes, Tile[] roadTileset, Tile[] backgroundTileset)
        {
            this.Name = name;
            this.Palettes = palettes;
            this.roadTileset = roadTileset;
            this.backgroundTileset = backgroundTileset;
        }

        public void SetRoadTileset(Tile[] tiles)
        {
            this.roadTileset = tiles;
        }

        public void SetRoadTile(int index, Tile tile)
        {
            this.roadTileset[index] = tile;
        }

        public void SetBackgroundTileset(Tile[] tiles)
        {
            this.backgroundTileset = tiles;
        }

        public void SetBackgroundTile(int index, Tile tile)
        {
            this.backgroundTileset[index] = tile;
        }

        public Tile[] GetRoadTileset()
        {
            return this.roadTileset;
        }

        public Tile GetRoadTile(int index)
        {
            return this.roadTileset[index];
        }

        public Tile[] GetBackgroundTileset()
        {
            return this.backgroundTileset;
        }

        public Tile GetBackgroundTile(int index)
        {
            return this.backgroundTileset[index];
        }

        public void ClearRoadTileset()
        {
            for (int i = 0; i < this.roadTileset.Length; i++)
            {
                this.roadTileset[i] = null;
            }
        }

        public void ClearBackgroundTileset()
        {
            for (int i = 0; i < this.backgroundTileset.Length; i++)
            {
                this.backgroundTileset[i] = null;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void Dispose()
        {
            foreach (Tile tile in this.roadTileset)
            {
                tile.Dispose();
            }

            foreach (Tile tile in this.backgroundTileset)
            {
                tile.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
