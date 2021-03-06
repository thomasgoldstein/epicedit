﻿#region GPL statement
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

using EpicEdit.Rom;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EpicEdit.UI.Gfx
{
    internal sealed class TilesetHelper : IDisposable
    {
        public static readonly TilesetHelper Instance = new TilesetHelper();

        private Pen _selectionPen;

        private TilesetHelper()
        {
            _selectionPen = new Pen(Color.FromArgb(150, 255, 0, 0));
        }

        public void DrawTileset(Graphics g, Image image, Size imageSize, int zoom, byte selectedTile)
        {
            using (var imageCopy = (Image)image.Clone())
            using (var backBuffer = Graphics.FromImage(imageCopy))
            {
                var xTileCount = imageSize.Width / Tile.Size;
                var tilePosX = selectedTile % xTileCount;
                var tilePosY = selectedTile / xTileCount;
                var selectedTilePosition = new Point(tilePosX, tilePosY);

                backBuffer.DrawRectangle(_selectionPen,
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
            _selectionPen.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
