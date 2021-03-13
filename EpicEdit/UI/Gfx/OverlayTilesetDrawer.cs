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
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of an overlay tileset.
    /// </summary>
    internal sealed class OverlayTilesetDrawer : IDisposable
    {
        public const int Zoom = 2;

        public Dictionary<OverlayTilePattern, Point> PatternList { get; set; }
        private RoadTileset _tileset;
        private Size _imageSize;

        public OverlayTilePattern HoveredPattern { get; set; }
        public OverlayTilePattern SelectedPattern { get; set; }

        private Bitmap _tilesetCache;
        private readonly HatchBrush _transparentBrush;

        private readonly Pen _delimitPen;
        private readonly Pen _highlightPen;
        private readonly SolidBrush _selectBrush;

        public OverlayTilesetDrawer()
        {
            _transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);

            _delimitPen = new Pen(Color.FromArgb(150, 60, 100, 255));
            _highlightPen = new Pen(Color.FromArgb(200, 255, 255, 255));
            _selectBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));

            // The following member is initialized so it can be disposed of
            // in each function without having to check if it's null beforehand
            _tilesetCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
        }

        public RoadTileset Tileset
        {
            get => _tileset;
            set
            {
                _tileset = value;
                UpdateCache();
            }
        }

        public void ReloadTileset()
        {
            UpdateCache();
        }

        public void SetImageSize(Size size)
        {
            _imageSize = new Size(size.Width / Zoom, size.Height / Zoom);
        }

        private void UpdateCache()
        {
            _tilesetCache.Dispose();
            _tilesetCache = new Bitmap(_imageSize.Width, _imageSize.Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(_tilesetCache))
            {
                g.FillRegion(_transparentBrush, g.Clip);

                foreach (KeyValuePair<OverlayTilePattern, Point> kvp in PatternList)
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

                            Tile tile = _tileset[tileId];
                            g.DrawImage(tile.Bitmap,
                                        Tile.Size * x + location.X,
                                        Tile.Size * y + location.Y,
                                        Tile.Size, Tile.Size);
                        }
                    }

                    // Delimit the pattern
                    g.DrawRectangle(_delimitPen,
                                    location.X,
                                    location.Y,
                                    pattern.Width * Tile.Size - 1,
                                    pattern.Height * Tile.Size - 1);
                }
            }
        }

        public void DrawTileset(Graphics g)
        {
            using (Bitmap image = (Bitmap)_tilesetCache.Clone())
            using (Graphics backBuffer = Graphics.FromImage(image))
            {
                OutlinePattern(backBuffer, HoveredPattern);

                if (HoveredPattern != SelectedPattern)
                {
                    OutlinePattern(backBuffer, SelectedPattern);
                }

                HighlightPattern(backBuffer, SelectedPattern);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images
                g.DrawImage(image, 0, 0,
                            _imageSize.Width * Zoom,
                            _imageSize.Height * Zoom);
            }
        }

        private void OutlinePattern(Graphics g, OverlayTilePattern pattern)
        {
            if (pattern != null && !Context.ColorPickerMode)
            {
                PatternList.TryGetValue(pattern, out Point location);
                g.DrawRectangle(_highlightPen,
                                location.X, location.Y,
                                pattern.Width * Tile.Size - 1,
                                pattern.Height * Tile.Size - 1);
            }
        }

        private void HighlightPattern(Graphics g, OverlayTilePattern pattern)
        {
            if (pattern != null)
            {
                PatternList.TryGetValue(pattern, out Point location);
                g.FillRectangle(_selectBrush,
                                location.X, location.Y,
                                pattern.Width * Tile.Size - 1,
                                pattern.Height * Tile.Size - 1);
            }
        }

        public void Dispose()
        {
            _tilesetCache.Dispose();
            _transparentBrush.Dispose();

            _delimitPen.Dispose();
            _highlightPen.Dispose();
            _selectBrush.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
