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
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Road
{
    /// <summary>
    /// Represents a collection of road tiles.
    /// </summary>
    internal class RoadTileset : INotifyPropertyChanged, IDisposable
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

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly RoadTile[] tileset;

        public bool Modified { get; private set; }

        public RoadTileset(RoadTile[] tileset)
        {
            this.tileset = tileset;

            foreach (Tile tile in this.tileset)
            {
                tile.PropertyChanged += this.MarkAsModified;
            }
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

        public byte[] GetTileGenreBytes()
        {
            byte[] data = new byte[this.tileset.Length];

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (byte)this.tileset[i].Genre;
            }

            return data;
        }

        public byte[] GetTilePaletteBytes()
        {
            byte[] data = new byte[this.tileset.Length];

            for (int i = 0; i < this.tileset.Length; i++)
            {
                data[i] = this.tileset[i].PaletteByte;
            }

            return data;
        }

        public void SetTileGenreBytes(byte[] data)
        {
            if (data.Length != this.tileset.Length)
            {
                throw new ArgumentException("Incorrect road tile type data size", nameof(data));
            }

            for (int i = 0; i < this.tileset.Length; i++)
            {
                this.tileset[i].Genre = (RoadTileGenre)data[i];
            }
        }

        public void SetTilePaletteBytes(byte[] data)
        {
            if (data.Length != this.tileset.Length)
            {
                throw new ArgumentException("Incorrect road tile palette data size", nameof(data));
            }

            for (int i = 0; i < this.tileset.Length; i++)
            {
                this.tileset[i].PaletteByte = data[i];
            }
        }

        private void MarkAsModified(object sender, PropertyChangedEventArgs e)
        {
            this.Modified = true;
            this.OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(sender, e);
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
