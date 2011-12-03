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

        /// <summary>
        /// Number of theme-specific tiles in the tileset.
        /// </summary>
        public const int ThemeTileCount = 192;

        /// <summary>
        /// Number of shared tiles in the tileset.
        /// </summary>
        public const int CommonTileCount = 64;

        /// <summary>
        /// Total number of tiles in the tileset.
        /// </summary>
        public const int TileCount = ThemeTileCount + CommonTileCount;

        public string Name { get; private set; }
        public Palettes Palettes { get; private set; }
        private MapTile[] roadTileset;
        private Tile[] backgroundTileset;

        private bool modified;
        public bool Modified
        {
            get
            {
                return this.modified || this.Palettes.Modified;
            }
            set
            {
                this.modified = value;
                this.Palettes.Modified = value;
            }
        }

        public Theme(string name, Palettes palettes, MapTile[] roadTileset, Tile[] backgroundTileset)
        {
            this.Name = name;
            this.Palettes = palettes;
            this.Palettes.Theme = this;
            this.roadTileset = roadTileset;
            this.backgroundTileset = backgroundTileset;
        }

        public MapTile[] GetRoadTileset()
        {
            return this.roadTileset;
        }

        public MapTile GetRoadTile(int index)
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

        public void UpdateTiles(Palette palette)
        {
            foreach (Tile tile in this.roadTileset)
            {
                if (tile.Palette == palette)
                {
                    tile.UpdateBitmap();
                }
            }

            // TODO: Update background tiles
        }

        public bool[] UpdateTiles(Palette palette, int colorIndex)
        {
            bool[] tileUpdates = new bool[this.roadTileset.Length];
            int index = 0;

            foreach (Tile tile in this.roadTileset)
            {
                if (tile.Palette == palette && tile.Contains(colorIndex))
                {
                    tileUpdates[index] = true;
                    tile.UpdateBitmap();
                }

                index++;
            }

            // TODO: Update background tiles

            return tileUpdates;
        }

        public byte[] GetTileGenreBytes()
        {
            byte[] data = new byte[this.roadTileset.Length];

            for (int i = 0; i < data.Length; i++)
            {
                MapTile tile = this.roadTileset[i];
                data[i] = (byte)tile.Genre;
            }

            return data;
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
