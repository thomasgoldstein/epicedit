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
    /// Represents the background of a track.
    /// </summary>
    internal class Background : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public BackgroundTileset Tileset { get; }
        public BackgroundLayout Layout { get; }

        public Background(BackgroundTileset tileset, BackgroundLayout layout)
        {
            Tileset = tileset;
            Layout = layout;

            Tileset.PropertyChanged += OnPropertyChanged;
            Layout.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public BackgroundTile GetTileInstance(int x, int y, bool front)
        {
            Layout.GetTileData(x, y, front, out byte tileId, out byte properties);

            return GetTileInstance(tileId, properties, front);
        }

        public BackgroundTile GetTileInstance(int tileId, byte properties, bool front)
        {
            BackgroundTile tile = Tileset[tileId];
            return new BackgroundTile(tile.Graphics, tile.Palettes, properties, front);
        }

        public void Dispose()
        {
            Tileset.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
