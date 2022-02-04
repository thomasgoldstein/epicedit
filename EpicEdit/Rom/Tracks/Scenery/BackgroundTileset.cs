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

namespace EpicEdit.Rom.Tracks.Scenery
{
    /// <summary>
    /// Represents a collection of background tiles.
    /// </summary>
    internal class BackgroundTileset : ITileset, INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Total number of tiles in the tileset.
        /// </summary>
        public const int TileCount = 48;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Modified { get; private set; }

        public int BitsPerPixel => 2;

        public int Length => _tileset.Length;

        public BackgroundTile this[int index] => GetTile(index);

        Tile ITileset.this[int index] => this[index];

        private readonly BackgroundTile[] _tileset;

        public BackgroundTileset(BackgroundTile[] tileset)
        {
            _tileset = tileset;

            foreach (Tile tile in _tileset)
            {
                tile.PropertyChanged += tile_PropertyChanged;
            }
        }

        private void tile_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != PropertyNames.Tile.Bitmap &&
                e.PropertyName != PropertyNames.Tile.Graphics)
            {
                // Changes that are not related to graphics do not concern the tileset.
                // For 2-bpp tiles, other tile properties are specific to instances.
                return;
            }

            MarkAsModified(sender, e);
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

        public BackgroundTile GetTile(int index)
        {
            return _tileset[index];
        }

        public byte[] GetBytes()
        {
            var data = new byte[TileCount * 16];

            for (var j = 0; j < TileCount; j++)
            {
                var tile = GetTile(j);
                Buffer.BlockCopy(tile.Graphics, 0, data, j * 16, tile.Graphics.Length);
            }

            return data;
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
