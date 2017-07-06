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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of an overlay tileset.
    /// </summary>
    internal sealed class OverlayTilesetDrawer : IDisposable
    {
        public const int Zoom = 2;

        public Dictionary<OverlayTilePattern, Point> PatternList { get; set; }
        private RoadTileset tileset;
        private Size imageSize;

        public OverlayTilePattern HoveredPattern { get; set; }
        public OverlayTilePattern SelectedPattern { get; set; }

        private Bitmap tilesetCache;
        private readonly HatchBrush transparentBrush;

        private readonly Pen delimitPen;
        private readonly Pen highlightPen;
        private readonly SolidBrush selectBrush;

        public OverlayTilesetDrawer()
        {
            this.transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);

            this.delimitPen = new Pen(Color.FromArgb(150, 60, 100, 255));
            this.highlightPen = new Pen(Color.FromArgb(200, 255, 255, 255));
            this.selectBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));

            // The following member is initialized so it can be disposed of
            // in each function without having to check if it's null beforehand
            this.tilesetCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public RoadTileset Tileset
        {
            get { return this.tileset; }
            set
            {
                this.tileset = value;
                this.UpdateCache();
            }
        }

        public void ReloadTileset()
        {
            this.UpdateCache();
        }

        public void SetImageSize(Size size)
        {
            this.imageSize = new Size(size.Width / Zoom, size.Height / Zoom);
        }

        private void UpdateCache()
        {
            this.tilesetCache.Dispose();
            this.tilesetCache = new Bitmap(this.imageSize.Width, this.imageSize.Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(this.tilesetCache))
            {
                g.FillRegion(this.transparentBrush, g.Clip);

                foreach (KeyValuePair<OverlayTilePattern, Point> kvp in this.PatternList)
                {
                    OverlayTilePattern pattern = kvp.Key;
                    Point location = kvp.Value;

                    // Draw the pattern
                    for (int y = 0; y < pattern.Height; y++)
                    {
                        for (int x = 0; x < pattern.Width; x++)
                        {
                            int tileId = pattern[x, y];

                            if (tileId == OverlayTile.None)
                            {
                                continue;
                            }

                            Tile tile = this.tileset[tileId];
                            g.DrawImage(tile.Bitmap,
                                        Tile.Size * x + location.X,
                                        Tile.Size * y + location.Y,
                                        Tile.Size, Tile.Size);
                        }
                    }

                    // Delimit the pattern
                    g.DrawRectangle(this.delimitPen,
                                    location.X,
                                    location.Y,
                                    pattern.Width * Tile.Size - 1,
                                    pattern.Height * Tile.Size - 1);
                }
            }
        }

        public void DrawTileset(Graphics g)
        {
            using (Bitmap image = this.tilesetCache.Clone() as Bitmap)
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                this.OutlinePattern(backBuffer, this.HoveredPattern);

                if (this.HoveredPattern != this.SelectedPattern)
                {
                    this.OutlinePattern(backBuffer, this.SelectedPattern);
                }

                this.HighlightPattern(backBuffer, this.SelectedPattern);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images
                g.DrawImage(image, 0, 0,
                            this.imageSize.Width * Zoom,
                            this.imageSize.Height * Zoom);
            }
        }

        private void OutlinePattern(Graphics g, OverlayTilePattern pattern)
        {
            if (pattern != null && !Context.ColorPickerMode)
            {
                this.PatternList.TryGetValue(pattern, out Point location);
                g.DrawRectangle(this.highlightPen,
                                location.X, location.Y,
                                pattern.Width * Tile.Size - 1,
                                pattern.Height * Tile.Size - 1);
            }
        }

        private void HighlightPattern(Graphics g, OverlayTilePattern pattern)
        {
            if (pattern != null)
            {
                this.PatternList.TryGetValue(pattern, out Point location);
                g.FillRectangle(this.selectBrush,
                                location.X, location.Y,
                                pattern.Width * Tile.Size - 1,
                                pattern.Height * Tile.Size - 1);
            }
        }

        public void Dispose()
        {
            this.tilesetCache.Dispose();
            this.transparentBrush.Dispose();

            this.delimitPen.Dispose();
            this.highlightPen.Dispose();
            this.selectBrush.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
