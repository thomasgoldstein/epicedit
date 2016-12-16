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
    internal class BackgroundTileset : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Total number of tiles in the tileset.
        /// </summary>
        public const int TileCount = 48;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Modified { get; private set; }

        private readonly BackgroundTile[] tileset;

        public BackgroundTileset(BackgroundTile[] tileset)
        {
            this.tileset = tileset;

            foreach (Tile tile in this.tileset)
            {
                tile.PropertyChanged += this.tile_PropertyChanged;
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

            this.MarkAsModified(sender, e);
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
