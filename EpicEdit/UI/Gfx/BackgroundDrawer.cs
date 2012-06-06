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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.UI.TrackEdition;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of a track background.
    /// </summary>
    internal sealed class BackgroundDrawer : IDisposable
    {
        public const int Zoom = 2;

        private int FrontWidth
        {
            get { return BackgroundLayout.FrontLayerWidth * Tile.Size; }
        }

        private int BackWidth
        {
            get { return BackgroundLayout.BackLayerWidth * Tile.Size; }
        }

        private int Height
        {
            get { return BackgroundLayout.RowCount * Tile.Size; }
        }

        private Theme theme;
        private Bitmap frontLayer;
        private Bitmap backLayer;

        private int x;

        /// <summary>
        /// Used to draw the rectangle when highlighting tiles.
        /// </summary>
        private Pen tileHighlightPen;

        /// <summary>
        /// Used to draw the rectangle when selecting tiles.
        /// </summary>
        private Pen tileSelectPen;

        /// <summary>
        /// Used to paint the inside of the selection rectangle.
        /// </summary>
        private SolidBrush tileSelectBrush;

        public BackgroundDrawer()
        {
            this.tileHighlightPen = new Pen(Color.FromArgb(150, 255, 0, 0), 1);
            this.tileSelectPen = new Pen(Color.FromArgb(150, 20, 130, 255), 1);
            this.tileSelectBrush = new SolidBrush(Color.FromArgb(50, 20, 130, 255));

            // The following members are initialized so they can be disposed of
            // in each function without having to check if they're null beforehand
            this.frontLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
            this.backLayer = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public void LoadTheme(Theme theme)
        {
            this.theme = theme;
            this.InitFrontLayer();
            this.InitBackLayer();
        }

        public void RewindPreview()
        {            
            this.x = 0;
        }

        private void InitFrontLayer()
        {
            this.frontLayer.Dispose();
            this.frontLayer = this.CreateLayer(true);
        }

        private void InitBackLayer()
        {
            this.backLayer.Dispose();
            this.backLayer = this.CreateLayer(false);
        }

        private Bitmap CreateLayer(bool front)
        {
            int layerWidth = front ? BackgroundLayout.FrontLayerWidth : BackgroundLayout.BackLayerWidth;
            int imageWidth = layerWidth * Tile.Size;

            Bitmap bitmap = new Bitmap(imageWidth, this.Height, PixelFormat.Format32bppPArgb);
            Background background = this.theme.Background;

            Dictionary<int, BackgroundTile> tileCache = new Dictionary<int, BackgroundTile>();

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (!front)
                {
                    g.Clear(this.theme.BackColor);
                }

                for (int y = 0; y < BackgroundLayout.RowCount; y++)
                {
                    for (int x = 0; x < layerWidth; x++)
                    {
                        byte tileId;
                        byte properties;
                        this.theme.Background.Layout.GetTileData(x, y, front, out tileId, out properties);
                        int key = (tileId << 8) + properties;
   
                        if (!tileCache.ContainsKey(key))
                        {
                            BackgroundTile tile = this.theme.Background.Tileset[tileId];
                            BackgroundTile instance = new BackgroundTile(tile.Graphics, tile.Palettes, properties, front);
                            tileCache.Add(key, instance);
                        }

                        g.DrawImage(tileCache[key].Bitmap, x * Tile.Size, y * Tile.Size);
                    }
                }
            }

            foreach (BackgroundTile tile in tileCache.Values)
            {
                tile.Dispose();
            }

            return bitmap;
        }

        public void DrawBackgroundLayer(Graphics g, Point cursorPosition, int x, bool front, bool tileSelection)
        {
            if (front)
            {
                this.DrawFrontBackgroundLayer(g, cursorPosition, x, tileSelection);
            }
            else
            {
                this.DrawBackBackgroundLayer(g, cursorPosition, x, tileSelection);
            }
        }

        private void DrawBackBackgroundLayer(Graphics g, Point cursorPosition, int x, bool tileSelection)
        {
            using (Bitmap image = new Bitmap(this.BackWidth, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.DrawImage(this.backLayer, x, 0);
                this.DrawTileSelection(backBuffer, cursorPosition, x, tileSelection);
                this.DrawImage(g, image, this.BackWidth);
            }
        }

        private void DrawFrontBackgroundLayer(Graphics g, Point cursorPosition, int x,  bool tileSelection)
        {
            using (Bitmap image = new Bitmap(this.FrontWidth, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.Clear(this.theme.BackColor);
                backBuffer.DrawImage(this.frontLayer, x, 0);
                this.DrawTileSelection(backBuffer, cursorPosition, x, tileSelection);
                this.DrawImage(g, image, this.FrontWidth);
            }
        }

        private void DrawTileSelection(Graphics g, Point cursorPosition, int x, bool tileSelection)
        {
            if (cursorPosition == TrackEditor.OutOfBounds || Context.ColorPickerMode)
            {
                return;
            }

            Rectangle rec = new Rectangle(cursorPosition.X * Tile.Size - 1 + (x % Tile.Size),
                                          cursorPosition.Y * Tile.Size - 1,
                                          Tile.Size + 1, Tile.Size + 1);

            if (tileSelection) // A tile selection is happening now
            {
                g.FillRectangle(this.tileSelectBrush, rec);
                g.DrawRectangle(this.tileSelectPen, rec);
            }
            else // The user is simply hovering tiles
            {
                g.DrawRectangle(this.tileHighlightPen, rec);
            }
        }

        public void DrawBackgroundPreview(Graphics g)
        {
            using (Bitmap image = new Bitmap(this.FrontWidth, this.Height, PixelFormat.Format32bppPArgb))
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                backBuffer.DrawImage(this.backLayer, x, 0);
                backBuffer.DrawImage(this.backLayer, x + this.BackWidth, 0);

                backBuffer.DrawImage(this.frontLayer, x * 2, 0);
                backBuffer.DrawImage(this.frontLayer, x * 2 + this.FrontWidth, 0);

                this.DrawImage(g, image, this.FrontWidth);
            }
        }

        private void DrawImage(Graphics g, Bitmap image, int width)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.DrawImage(image, 0, 0, width * Zoom, this.Height * Zoom);
        }

        public void UpdateTile(int x, int y, bool front, byte tileId, byte properties)
        {
            if (!Platform.IsWindows)
            {
                // HACK: Work around Mono bug #492299 (slower)
                // https://bugzilla.novell.com/show_bug.cgi?id=492299
                if (front)
                {
                    this.InitFrontLayer();
                }
                else
                {
                    this.InitBackLayer();
                }
                return;
            }

            Rectangle rec = new Rectangle(x * Tile.Size, y * Tile.Size, Tile.Size, Tile.Size);
            BackgroundTile tileModel = this.theme.Background.Tileset[tileId];

            using (Graphics g = Graphics.FromImage(front ? this.frontLayer : this.backLayer))
            using (BackgroundTile tile = new BackgroundTile(tileModel.Graphics, tileModel.Palettes, properties, front))
            {
                g.SetClip(rec);
                g.Clear(front ? Color.Transparent : this.theme.BackColor.Color);
                g.DrawImage(tile.Bitmap, rec.X, rec.Y);
            }
        }

        public void IncrementPreviewFrame()
        {
            this.x--;

            if (this.x < -this.BackWidth)
            {
                this.x = 0;
            }
        }

        public void Dispose()
        {
            this.frontLayer.Dispose();
            this.backLayer.Dispose();
            this.tileHighlightPen.Dispose();
            this.tileSelectPen.Dispose();
            this.tileSelectBrush.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
