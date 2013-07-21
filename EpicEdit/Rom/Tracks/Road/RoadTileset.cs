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

namespace EpicEdit.Rom.Tracks.Road
{
    /// <summary>
    /// Represents a collection of road tiles.
    /// </summary>
    internal class RoadTileset : IDisposable
    {
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

        private RoadTile[] tileset;

        // TODO: Make setter private
        public bool Modified { get; set; }

        public RoadTileset(RoadTile[] tileset)
        {
            this.tileset = tileset;
        }

        public RoadTile[] GetTiles()
        {
            return this.tileset;
        }

        public RoadTile GetTile(int index)
        {
            return this.tileset[index];
        }

        public RoadTile this[int index]
        {
            get { return this.GetTile(index); }
        }

        public void UpdateTiles(Palette palette)
        {
            foreach (Tile tile in this.tileset)
            {
                if (tile.Palette == palette)
                {
                    tile.UpdateBitmap();
                }
            }
        }

        public bool[] UpdateTiles(Palette palette, int colorIndex)
        {
            bool[] tileUpdates = new bool[this.tileset.Length];
            int index = 0;

            foreach (Tile tile in this.tileset)
            {
                if (tile.Palette == palette && tile.Contains(colorIndex))
                {
                    tileUpdates[index] = true;
                    tile.UpdateBitmap();
                }

                index++;
            }

            return tileUpdates;
        }

        public byte[] GetTileGenreBytes()
        {
            byte[] data = new byte[this.tileset.Length];

            for (int i = 0; i < data.Length; i++)
            {
                RoadTile tile = this.tileset[i];
                data[i] = (byte)tile.Genre;
            }

            return data;
        }

        public void ResetModifiedState()
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
