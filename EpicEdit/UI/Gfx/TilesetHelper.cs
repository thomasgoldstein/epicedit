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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using EpicEdit.Rom;

namespace EpicEdit.UI.Gfx
{
    internal sealed class TilesetHelper : IDisposable
    {
        public static readonly TilesetHelper Instance = new TilesetHelper();

        private Pen selectionPen;

        private TilesetHelper()
        {
            this.selectionPen = new Pen(Color.FromArgb(150, 255, 0, 0));
        }

        public void DrawTileset(Graphics g, Image image, Size imageSize, int zoom, byte selectedTile)
        {
            using (Image imageCopy = image.Clone() as Image)
            using (Graphics backBuffer = Graphics.FromImage(imageCopy))
            {
                int xTileCount = imageSize.Width / Tile.Size;
                int tilePosX = selectedTile % xTileCount;
                int tilePosY = selectedTile / xTileCount;
                Point selectedTilePosition = new Point(tilePosX, tilePosY);

                backBuffer.DrawRectangle(this.selectionPen,
                                         selectedTilePosition.X * Tile.Size,
                                         selectedTilePosition.Y * Tile.Size,
                                         Tile.Size - 1,
                                         Tile.Size - 1);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images
                g.DrawImage(imageCopy, 0, 0,
                            imageSize.Width * zoom,
                            imageSize.Height * zoom);
            }
        }

        public void Dispose()
        {
            this.selectionPen.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
