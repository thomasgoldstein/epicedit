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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.UI.TrackEdition;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of a track background.
    /// </summary>
    internal sealed class BackgroundDrawer : IDisposable
    {
        public const int Zoom = 2;
        private const int FrontWidth = BackgroundLayout.FrontLayerWidth * Tile.Size;
        private const int BackWidth = BackgroundLayout.BackLayerWidth * Tile.Size;
        private const int Height = BackgroundLayout.RowCount * Tile.Size;
        private const int CanvasWidth = BackWidth / 2;

        private Theme _theme;

        public Theme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                InitFrontLayer();
                InitBackLayer();
            }
        }

        private Bitmap _frontLayer;
        private Bitmap _backLayer;

        private int _x;

        /// <summary>
        /// Used to draw the rectangle when highlighting tiles.
        /// </summary>
        private readonly Pen _tileHighlightPen;

        /// <summary>
        /// Used to draw the rectangle when selecting tiles.
        /// </summary>
        private readonly Pen _tileSelectPen;

        /// <summary>
        /// Used to paint the inside of the selection rectangle.
        /// </summary>
        private readonly SolidBrush _tileSelectBrush;

        public BackgroundDrawer()
        {
            _tileHighlightPen = new Pen(Color.FromArgb(150, 255, 0, 0), 1);
            _tileSelectPen = new Pen(Color.FromArgb(150, 20, 130, 255), 1);
            _tileSelectBrush = new SolidBrush(Color.FromArgb(50, 20, 130, 255));

            // The following members are initialized so they can be disposed of
            // in each function without having to check if they're null beforehand
            _frontLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
            _backLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        private void InitFrontLayer()
        {
            _frontLayer.Dispose();
            _frontLayer = CreateLayer(true);
        }

        private void InitBackLayer()
        {
            _backLayer.Dispose();
            _backLayer = CreateLayer(false);
        }

        private Bitmap CreateLayer(bool front)
        {
            int layerWidth = front ? BackgroundLayout.FrontLayerWidth : BackgroundLayout.BackLayerWidth;
            int imageWidth = layerWidth * Tile.Size;

            Bitmap bitmap = new Bitmap(imageWidth, Height, PixelFormat.Format32bppPArgb);
            Background background = _theme.Background;

            Dictionary<int, BackgroundTile> tileCache = new Dictionary<int, BackgroundTile>();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (!front)
                {
                    g.Clear(_theme.BackColor);
                }

                for (int y = 0; y < BackgroundLayout.RowCount; y++)
                {
                    for (int x = 0; x < layerWidth; x++)
                    {
                        background.Layout.GetTileData(x, y, front, out byte tileId, out byte properties);
                        int key = (tileId << 8) + properties;

                        if (!tileCache.TryGetValue(key, out BackgroundTile instance))
                        {
                            BackgroundTile tile = background.Tileset[tileId];
                            instance = new BackgroundTile(tile.Graphics, tile.Palettes, properties, front);
                            tileCache.Add(key, instance);
                        }

                        g.DrawImage(instance.Bitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }

            foreach (BackgroundTile tile in tileCache.Values)
            {
                tile.Dispose();
            }

            return bitmap;
        }

        public void DrawBackgroundLayer(Graphics g, Point cursorPosition, int x, bool front, bool selectingTile)
        {
            if (front)
            {
                DrawFrontBackgroundLayer(g, cursorPosition, x, selectingTile);
            }
            else
            {
                DrawBackBackgroundLayer(g, cursorPosition, x, selectingTile);
            }
        }

        private void DrawBackBackgroundLayer(Graphics g, Point cursorPosition, int x, bool selectingTile)
        {
            using (Bitmap image = new Bitmap(CanvasWidth, Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.DrawImage(_backLayer, x, 0);
                DrawTileSelection(backBuffer, cursorPosition, x, selectingTile);
                DrawImage(g, image, CanvasWidth);
            }
        }

        private void DrawFrontBackgroundLayer(Graphics g, Point cursorPosition, int x, bool selectingTile)
        {
            using (Bitmap image = new Bitmap(CanvasWidth, Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.Clear(_theme.BackColor);
                backBuffer.DrawImage(_frontLayer, x, 0);
                DrawTileSelection(backBuffer, cursorPosition, x, selectingTile);
                DrawImage(g, image, CanvasWidth);
            }
        }

        private void DrawTileSelection(Graphics g, Point cursorPosition, int x, bool selectingTile)
        {
            if (cursorPosition == TrackEditor.OutOfBounds || Context.ColorPickerMode)
            {
                return;
            }

            Rectangle rec = new Rectangle(cursorPosition.X * Tile.Size - 1 + (x % Tile.Size),
                                          cursorPosition.Y * Tile.Size - 1,
                                          Tile.Size + 1, Tile.Size + 1);

            if (selectingTile) // A tile selection is happening now
            {
                g.FillRectangle(_tileSelectBrush, rec);
                g.DrawRectangle(_tileSelectPen, rec);
            }
            else // The user is simply hovering tiles
            {
                g.DrawRectangle(_tileHighlightPen, rec);
            }
        }

        public void DrawBackgroundPreview(Graphics g)
        {
            using (Bitmap image = new Bitmap(CanvasWidth, Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                // Drawing the back and front layers twice, so that the background loops horizontally
                backBuffer.DrawImage(_backLayer, _x, 0);
                backBuffer.DrawImage(_backLayer, _x + BackWidth, 0);

                backBuffer.DrawImage(_frontLayer, _x * 2, 0);
                backBuffer.DrawImage(_frontLayer, _x * 2 + FrontWidth, 0);

                Bitmap topBorder = Context.Game.ItemIconGraphics.GetTopBorder(Theme.Palettes);

                for (int i = 0; i < CanvasWidth; i += Tile.Size)
                {
                    backBuffer.DrawImage(topBorder, i, -1);
                }

                DrawImage(g, image, CanvasWidth);
            }
        }

        private static void DrawImage(Graphics g, Bitmap image, int width)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(image, 0, 0, width * Zoom, Height * Zoom);
        }

        public void UpdateTile(int x, int y, bool front, byte tileId, byte properties)
        {
            Rectangle rec = new Rectangle(x * Tile.Size, y * Tile.Size, Tile.Size, Tile.Size);
            BackgroundTile tileModel = _theme.Background.Tileset[tileId];

            using (Graphics g = Graphics.FromImage(front ? _frontLayer : _backLayer))
            using (BackgroundTile tile = new BackgroundTile(tileModel.Graphics, tileModel.Palettes, properties, front))
            {
                g.SetClip(rec);
                g.Clear(front ? Color.Transparent : _theme.BackColor.Color);
                g.DrawImage(tile.Bitmap, rec.X, rec.Y);
            }
        }

        public int PreviewFrame
        {
            //get => -_x;
            set => _x = -value;
        }

        public void Dispose()
        {
            _frontLayer.Dispose();
            _backLayer.Dispose();
            _tileHighlightPen.Dispose();
            _tileSelectPen.Dispose();
            _tileSelectBrush.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
