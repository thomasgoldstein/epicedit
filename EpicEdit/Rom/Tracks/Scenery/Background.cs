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
using System.Drawing;

namespace EpicEdit.Rom.Tracks.Scenery
{
    /// <summary>
    /// Represents the background of a track.
    /// </summary>
    internal class Background : IDisposable
    {
        private BackgroundTileset tileset;
        private BackgroundLayout layout;

        public Background(BackgroundTileset tileset, BackgroundLayout layout)
        {
            this.tileset = tileset;
            this.layout = layout;
        }

        public Bitmap GetFrontTileBitmap(int x, int y)
        {
            return this.GetTileBitmap(true, x, y, false);
        }

        public Bitmap GetFrontTileBitmap(int x, int y, bool transparency)
        {
            return this.GetTileBitmap(true, x, y, transparency);
        }

        public Bitmap GetBackTileBitmap(int x, int y)
        {
            return this.GetTileBitmap(false, x, y, false);
        }

        private Bitmap GetTileBitmap(bool front, int x, int y, bool transparency)
        {
            byte tileId;
            byte properties;
            this.layout.GetTileData(front, x, y, out tileId, out properties);
            BackgroundTile tile = this.tileset[tileId];
            return tile.GetBitmap(properties, front, transparency);
        }

        public void Dispose()
        {
            this.tileset.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
