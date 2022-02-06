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
    internal class RoadTileset : ITileset, INotifyPropertyChanged, IDisposable
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

        private readonly RoadTile[] _tileset;

        public bool Modified { get; private set; }

        public int BitsPerPixel => 4;

        public int Length => _tileset.Length;

        public Palettes Palettes => _tileset[0].Palette.Collection;

        public RoadTile this[int index] => GetTile(index);

        Tile ITileset.this[int index] => this[index];

        public RoadTileset(RoadTile[] tileset)
        {
            _tileset = tileset;

            foreach (Tile tile in _tileset)
            {
                tile.PropertyChanged += MarkAsModified;
            }
        }

        public RoadTile GetTile(int index)
        {
            return _tileset[index];
        }

        public byte[] GetTileGenreBytes()
        {
            var data = new byte[_tileset.Length];

            for (var i = 0; i < data.Length; i++)
            {
                data[i] = (byte)_tileset[i].Genre;
            }

            return data;
        }

        public byte[] GetTilePaletteBytes()
        {
            var data = new byte[_tileset.Length];

            for (var i = 0; i < _tileset.Length; i++)
            {
                data[i] = _tileset[i].PaletteByte;
            }

            return data;
        }

        public byte[] GetBytes()
        {
            var data = new byte[TileCount + (TileCount * 32)];

            Buffer.BlockCopy(GetTilePaletteBytes(), 0, data, 0, TileCount);

            for (var j = 0; j < TileCount; j++)
            {
                var tile = GetTile(j);
                Buffer.BlockCopy(tile.Graphics, 0, data, TileCount + (j * 32), tile.Graphics.Length);
            }

            return data;
        }

        public void SetTileGenreBytes(byte[] data)
        {
            if (data.Length != _tileset.Length)
            {
                throw new ArgumentException("Incorrect road tile type data size", nameof(data));
            }

            for (var i = 0; i < _tileset.Length; i++)
            {
                _tileset[i].Genre = (RoadTileGenre)data[i];
            }
        }

        public void SetTilePaletteBytes(byte[] data)
        {
            if (data.Length != _tileset.Length)
            {
                throw new ArgumentException("Incorrect road tile palette data size", nameof(data));
            }

            for (var i = 0; i < _tileset.Length; i++)
            {
                _tileset[i].PaletteByte = data[i];
            }
        }

        private void MarkAsModified(object sender, PropertyChangedEventArgs e)
        {
            Modified = true;
            OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void ResetModifiedState()
        {
            Modified = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Tile tile in _tileset)
                {
                    tile.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
