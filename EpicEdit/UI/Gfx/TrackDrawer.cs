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
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.UI.TrackEdition;

namespace EpicEdit.UI.Gfx
{
    /// <summary>
    /// Provides the ability to paint the graphics of a track.
    /// </summary>
    public sealed class TrackDrawer : IDisposable
    {
        private Control control;
        private Track track;
        private Size imageSize;

        private Point scrollPosition;
        public Point ScrollPosition
        {
            get { return this.scrollPosition; }
            set
            {
                this.scrollPosition = value;
                this.SetImageSize();
            }
        }
        private float zoom;

        private Bitmap trackCache;
        private Bitmap tileClipboardCache;
        private Bitmap objectZonesCache;

        /// <summary>
        /// The object zones of the track.
        /// They're cached in order to figure out when to update the objectZonesCache.
        /// </summary>
        private byte[][] zones;

        /// <summary>
        /// Determines whether a full repaint is needed or not.
        /// </summary>
        private bool fullRepaintNeeded;

        /// <summary>
        /// Region which needs to be repainted.
        /// </summary>
        private Region dirtyRegion;

        /// <summary>
        /// Used to resize the dirty region depending on the zoom level.
        /// </summary>
        private Matrix zoomMatrix;

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

        /// <summary>
        /// Used to paint a rectangle over the hovered overlay tile.
        /// </summary>
        private SolidBrush overlayHighlightBrush;

        /// <summary>
        /// Used to draw the lap line.
        /// </summary>
        private Pen lapLinePen;

        /// <summary>
        /// Used to draw the lap line outline.
        /// </summary>
        private Pen lapLineOutlinePen;

        /// <summary>
        /// Used to fill in the starting positions.
        /// </summary>
        private SolidBrush startPositionBrush;

        /// <summary>
        /// Used to draw the starting position outlines.
        /// </summary>
        private Pen startPositionPen;

        /// <summary>
        /// Used to paint object outlines.
        /// </summary>
        private Pen objectOutlinePen;

        /// <summary>
        /// Used to fill in objects depending on their group.
        /// </summary>
        private SolidBrush[] objectBrushes;

        /// <summary>
        /// Used to draw inside Match Race objects.
        /// </summary>
        private Pen objectMatchRacePen;

        /// <summary>
        /// Used to fill in AI zones.
        /// </summary>
        private SolidBrush[][] aiZoneBrushes;

        /// <summary>
        /// Used to draw the AI zone outlines.
        /// </summary>
        private Pen[] aiZonePens;

        /// <summary>
        /// Used to draw the hovered AI zone outlines.
        /// </summary>
        private Pen aiElementHighlightPen;

        /// <summary>
        /// Used to draw the selected AI zone outlines.
        /// </summary>
        private Pen aiElementSelectPen;

        /// <summary>
        /// Image attributes to draw images as semi-transparent.
        /// </summary>
        private ImageAttributes translucidImageAttr;

        public TrackDrawer(Control control, float zoom)
        {
            this.control = control;

            #region Pens and Brushes initialization

            this.tileHighlightPen = new Pen(Color.FromArgb(150, 255, 0, 0), 1);
            this.tileSelectPen = new Pen(Color.FromArgb(150, 20, 130, 255), 1);
            this.tileSelectBrush = new SolidBrush(Color.FromArgb(50, 20, 130, 255));

            this.overlayHighlightBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));

            this.lapLinePen = new Pen(Color.White);
            this.lapLineOutlinePen = new Pen(Color.Black, 3);
            this.startPositionBrush = new SolidBrush(Color.White);
            this.startPositionPen = new Pen(Color.Gray, 1);

            this.objectOutlinePen = new Pen(Color.White, 2);
            this.objectBrushes = new SolidBrush[5];
            this.objectBrushes[0] = new SolidBrush(Color.FromArgb(255, 204, 51, 51)); // Zone 1 object color
            this.objectBrushes[1] = new SolidBrush(Color.FromArgb(255, 21, 94, 177)); // Zone 2 object color
            this.objectBrushes[2] = new SolidBrush(Color.FromArgb(255, 230, 186, 64)); // Zone 3 object color
            this.objectBrushes[3] = new SolidBrush(Color.FromArgb(255, 16, 150, 24)); // Zone 4 object color
            this.objectBrushes[4] = new SolidBrush(Color.FromArgb(255, 0, 0, 0)); // Match Race object color
            this.objectMatchRacePen = new Pen(Color.SkyBlue, 1);

            this.aiZoneBrushes = new SolidBrush[4][];

            this.aiZoneBrushes[0] = new SolidBrush[3];
            this.aiZoneBrushes[0][0] = new SolidBrush(Color.FromArgb(60, 165, 204, 244)); // Top
            this.aiZoneBrushes[0][1] = new SolidBrush(Color.FromArgb(60, 125, 168, 255)); // Left / right
            this.aiZoneBrushes[0][2] = new SolidBrush(Color.FromArgb(60, 87, 126, 205)); // Bottom

            this.aiZoneBrushes[1] = new SolidBrush[3];
            this.aiZoneBrushes[1][0] = new SolidBrush(Color.FromArgb(60, 179, 244, 150)); // Top
            this.aiZoneBrushes[1][1] = new SolidBrush(Color.FromArgb(60, 141, 233, 109)); // Left / right
            this.aiZoneBrushes[1][2] = new SolidBrush(Color.FromArgb(60, 101, 185, 72));

            this.aiZoneBrushes[2] = new SolidBrush[3];
            this.aiZoneBrushes[2][0] = new SolidBrush(Color.FromArgb(60, 244, 231, 124)); // Top
            this.aiZoneBrushes[2][1] = new SolidBrush(Color.FromArgb(60, 228, 198, 80)); // Left / right
            this.aiZoneBrushes[2][2] = new SolidBrush(Color.FromArgb(60, 180, 153, 46)); // Bottom

            this.aiZoneBrushes[3] = new SolidBrush[3];
            this.aiZoneBrushes[3][0] = new SolidBrush(Color.FromArgb(60, 244, 172, 133)); // Top
            this.aiZoneBrushes[3][1] = new SolidBrush(Color.FromArgb(60, 222, 133, 90)); // Left / right
            this.aiZoneBrushes[3][2] = new SolidBrush(Color.FromArgb(60, 175, 94, 55)); // Bottom

            this.aiZonePens = new Pen[4];
            this.aiZonePens[0] = new Pen(Color.FromArgb(255, 165, 204, 244), 1);
            this.aiZonePens[1] = new Pen(Color.FromArgb(255, 179, 244, 150), 1);
            this.aiZonePens[2] = new Pen(Color.FromArgb(255, 244, 231, 124), 1);
            this.aiZonePens[3] = new Pen(Color.FromArgb(255, 244, 172, 133), 1);

            this.aiElementHighlightPen = new Pen(Color.FromArgb(150, 255, 255, 255), 1);
            this.aiElementSelectPen = new Pen(Color.White, 1);

            #endregion Pens and Brushes initialization

            this.translucidImageAttr = new ImageAttributes();
            ColorMatrix matrix = new ColorMatrix(new float[][]
            {
                new float[] { 1.0f, 0.0f, 0.0f, 0.0f, 0.0f },
                new float[] { 0.0f, 1.0f, 0.0f, 0.0f, 0.0f },
                new float[] { 0.0f, 0.0f, 1.0f, 0.0f, 0.0f },
                new float[] { 0.0f, 0.0f, 0.0f, 0.58f, 0.0f },
                new float[] { 0.0f, 0.0f, 0.0f, 0.0f, 1.0f }
            });
            this.translucidImageAttr.SetColorMatrix(matrix);

            // The following members are initialized so they can be disposed of
            // in each function without having to check if they're null beforehand
            this.trackCache = this.tileClipboardCache = this.objectZonesCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
            this.dirtyRegion = new Region();
            this.zoomMatrix = new Matrix();

            this.SetZoom(zoom);
        }

        /// <summary>
        /// Loads a track and generate the track image cache.
        /// </summary>
        /// <param name="track">The track.</param>
        public void LoadTrack(Track track)
        {
            this.trackCache.Dispose();

            this.track = track;
            Tile[] tileset = track.GetRoadTileset();

            this.trackCache = new Bitmap(this.track.Map.Width * 8,
                                         this.track.Map.Height * 8,
                                         PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(this.trackCache))
            {
                for (int x = 0; x < this.track.Map.Width; x++)
                {
                    for (int y = 0; y < this.track.Map.Height; y++)
                    {
                        Tile tile = tileset[this.track.Map[x, y]];
                        g.DrawImage(tile.Bitmap, x * 8, y * 8);
                    }
                }
            }

            this.NotifyFullRepaintNeed();
        }

        public void SetZoom(float zoom)
        {
            this.zoom = zoom;

            this.zoomMatrix.Dispose();
            this.zoomMatrix = new Matrix();
            this.zoomMatrix.Scale(this.zoom, this.zoom);

            this.SetImageSize();
            this.NotifyFullRepaintNeed();
        }

        private void SetGraphics(Graphics g)
        {
            // Solves a GDI+ bug which crops scaled images
            g.PixelOffsetMode = PixelOffsetMode.Half;

            if (this.zoom >= 1)
            {
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
            }
            else
            {
                g.InterpolationMode = InterpolationMode.Bilinear;
            }
        }

        private Bitmap CloneTrackImage()
        {
            return this.trackCache.Clone(
                new Rectangle(this.scrollPosition.X * 8,
                              this.scrollPosition.Y * 8,
                              this.imageSize.Width,
                              this.imageSize.Height),
                this.trackCache.PixelFormat);
        }

        public void DrawTrackTileset(Graphics g, Point cursorPosition, MouseButtons mouseButtons, Size selectionSize, Point selectionStart)
        {
            this.SetGraphics(g);
            Region clipRegion = new Region(Rectangle.Empty);

            using (Bitmap image = this.CloneTrackImage())
            {
                using (Graphics backBuffer = Graphics.FromImage(image))
                {
                    Rectangle selectionRectangle;

                    if (mouseButtons == MouseButtons.Middle)
                    {
                        selectionRectangle = Rectangle.Empty;
                    }
                    else
                    {
                        selectionRectangle = this.GetTileSelectionRectangle(cursorPosition, selectionSize, selectionStart, mouseButtons);
                        TrackDrawer.SetTileSelectionClipRegion(clipRegion, selectionRectangle);
                    }

                    if (!this.fullRepaintNeeded)
                    {
                        // If a full repaint isn't needed, we set the clipping regions
                        // to only partially repaint the panel
                        this.SetPaintRegions(g, backBuffer, clipRegion);
                    }

                    this.DrawTileSelection(backBuffer, selectionRectangle, mouseButtons);
                }
                g.DrawImage(image, 0, 0, this.imageSize.Width * this.zoom, this.imageSize.Height * this.zoom);
            }

            this.PaintTrackOutbounds(g);

            this.dirtyRegion.Dispose();
            this.dirtyRegion = clipRegion;
            this.fullRepaintNeeded = false;
        }

        public void DrawTrackOverlay(Graphics g, OverlayTile hoveredOverlayTile, OverlayTile selectedOverlayTile, OverlayTilePattern selectedPattern, Point selectedPatternLocation)
        {
            this.SetGraphics(g);
            Region clipRegion = new Region(Rectangle.Empty);

            using (Bitmap image = this.CloneTrackImage())
            {
                using (Graphics backBuffer = Graphics.FromImage(image))
                {
                    if (selectedPatternLocation.X == -1)
                    {
                        // The selected overlay tile pattern is out of the screen,
                        // act as if there isn't one
                        selectedPattern = null;
                    }
                    this.SetOverlayClipRegion(clipRegion, hoveredOverlayTile, selectedOverlayTile, selectedPattern, selectedPatternLocation);

                    if (!this.fullRepaintNeeded)
                    {
                        // If a full repaint isn't needed, we set the clipping regions
                        // to only partially repaint the panel
                        this.SetPaintRegions(g, backBuffer, clipRegion);
                    }

                    this.DrawOverlay(backBuffer, hoveredOverlayTile, selectedOverlayTile, selectedPattern, selectedPatternLocation);
                }
                g.DrawImage(image, 0, 0, this.imageSize.Width * this.zoom, this.imageSize.Height * this.zoom);
            }

            this.PaintTrackOutbounds(g);

            this.dirtyRegion.Dispose();
            this.dirtyRegion = clipRegion;
            this.fullRepaintNeeded = false;
        }

        public void DrawTrackStart(Graphics g)
        {
            this.SetGraphics(g);
            Region clipRegion = new Region(Rectangle.Empty);

            using (Bitmap image = this.CloneTrackImage())
            {
                using (Graphics backBuffer = Graphics.FromImage(image))
                {
                    if (this.track is GPTrack)
                    {
                        GPTrack gpTrack = this.track as GPTrack;
                        this.SetGPStartClipRegion(clipRegion, gpTrack.LapLine, gpTrack.StartPosition);
                    }
                    else
                    {
                        BattleTrack bTrack = this.track as BattleTrack;
                        this.SetBattleStartClipRegion(clipRegion, bTrack.StartPositionP1, bTrack.StartPositionP2);
                    }

                    if (!this.fullRepaintNeeded)
                    {
                        // If a full repaint isn't needed, we set the clipping regions
                        // to only partially repaint the panel
                        this.SetPaintRegions(g, backBuffer, clipRegion);
                    }

                    this.DrawStartData(backBuffer);
                }
                g.DrawImage(image, 0, 0, this.imageSize.Width * this.zoom, this.imageSize.Height * this.zoom);
            }

            this.PaintTrackOutbounds(g);

            this.dirtyRegion.Dispose();
            this.dirtyRegion = clipRegion;
            this.fullRepaintNeeded = false;
        }

        public void DrawTrackObjects(Graphics g, TrackObject hoveredObject, bool frontZonesView)
        {
            this.SetGraphics(g);
            Region clipRegion = new Region(Rectangle.Empty);

            using (Bitmap image = this.CloneTrackImage())
            {
                using (Graphics backBuffer = Graphics.FromImage(image))
                {
                    this.SetObjectClipRegion(clipRegion, hoveredObject);

                    if (!this.fullRepaintNeeded)
                    {
                        // If a full repaint isn't needed, we set the clipping regions
                        // to only partially repaint the panel
                        this.SetPaintRegions(g, backBuffer, clipRegion);
                    }

                    this.DrawObjectData(backBuffer, frontZonesView);
                }
                g.DrawImage(image, 0, 0, this.imageSize.Width * this.zoom, this.imageSize.Height * this.zoom);
            }

            this.PaintTrackOutbounds(g);

            this.dirtyRegion.Dispose();
            this.dirtyRegion = clipRegion;
            this.fullRepaintNeeded = false;
        }

        public void DrawTrackAI(Graphics g, TrackAIElement hoveredAIElem, TrackAIElement selectedAIElem, bool isAITargetHovered)
        {
            this.SetGraphics(g);
            Region clipRegion = new Region(Rectangle.Empty);

            using (Bitmap image = this.CloneTrackImage())
            {
                using (Graphics backBuffer = Graphics.FromImage(image))
                {
                    this.SetAIClipRegion(clipRegion, hoveredAIElem, selectedAIElem);

                    if (!this.fullRepaintNeeded)
                    {
                        // If a full repaint isn't needed, we set the clipping regions
                        // to only partially repaint the panel
                        this.SetPaintRegions(g, backBuffer, clipRegion);
                    }

                    this.DrawAI(backBuffer, hoveredAIElem, selectedAIElem, isAITargetHovered);
                }
                g.DrawImage(image, 0, 0, this.imageSize.Width * this.zoom, this.imageSize.Height * this.zoom);
            }

            this.PaintTrackOutbounds(g);

            this.dirtyRegion.Dispose();
            this.dirtyRegion = clipRegion;
            this.fullRepaintNeeded = false;
        }

        private Rectangle GetTileSelectionRectangle(Point cursorPosition, Size selectionSize, Point selectionStart, MouseButtons mouseButtons)
        {
            Rectangle selectionRectangle;
            if (mouseButtons == MouseButtons.Right) // A multiple tile selection is happening now
            {
                selectionStart.X -= this.scrollPosition.X;
                selectionStart.Y -= this.scrollPosition.Y;
                selectionRectangle = new Rectangle((selectionStart.X * 8) - 1, (selectionStart.Y * 8) - 1,
                                                   selectionSize.Width * 8 + 1, selectionSize.Height * 8 + 1);
            }
            else if (cursorPosition.X != -1) // The user is simply hovering tiles
            {
                selectionRectangle = new Rectangle((cursorPosition.X * 8) - 1, (cursorPosition.Y * 8) - 1,
                                                   selectionSize.Width * 8 + 1, selectionSize.Height * 8 + 1);
            }
            else // The cursor isn't on the track map
            {
                selectionRectangle = Rectangle.Empty;
            }
            return selectionRectangle;
        }

        private static void SetTileSelectionClipRegion(Region clipRegion, Rectangle rectangle)
        {
            // Enlarge rectangle by 1px to account for the 1px border of the selection
            rectangle.Inflate(1, 1);
            clipRegion.Union(rectangle);
        }

        private void SetOverlayClipRegion(Region clipRegion, OverlayTile hoveredOverlayTile, OverlayTile selectedOverlayTile, OverlayTilePattern selectedPattern, Point selectedPatternLocation)
        {
            if (hoveredOverlayTile != null)
            {
                Rectangle rec = this.GetOverlayClipRectangle(hoveredOverlayTile.Pattern, hoveredOverlayTile.Location);
                clipRegion.Union(rec);
            }

            if (selectedOverlayTile != null &&
                selectedOverlayTile != hoveredOverlayTile)
            {
                Rectangle rec = this.GetOverlayClipRectangle(selectedOverlayTile.Pattern, selectedOverlayTile.Location);
                clipRegion.Union(rec);
            }

            if (selectedPattern != null)
            {
                Rectangle rec = this.GetOverlayClipRectangle(selectedPattern, selectedPatternLocation);
                clipRegion.Union(rec);
            }
        }

        private Rectangle GetOverlayClipRectangle(OverlayTilePattern overlayTilePattern, Point location)
        {
            return new Rectangle((location.X - this.scrollPosition.X) * 8,
                                 (location.Y - this.scrollPosition.Y) * 8,
                                 overlayTilePattern.Width * 8,
                                 overlayTilePattern.Height * 8);
        }

        private void SetGPStartClipRegion(Region clipRegion, LapLine lapLine, GPStartPosition startPosition)
        {
            Rectangle lapLineRectangle =
                new Rectangle(lapLine.X - (this.scrollPosition.X * 8),
                              lapLine.Y - (this.scrollPosition.Y * 8) - 1,
                              lapLine.Length, 3);

            if (this.zoom < 1)
            {
                // HACK: Avoid clipping issues (rounding differences)
                lapLineRectangle.Inflate(0, 1);
            }

            Rectangle startRectangle1 = new Rectangle(startPosition.X - (this.scrollPosition.X * 8) - 4,
                                                      startPosition.Y - (this.scrollPosition.Y * 8) - 4,
                                                      9,
                                                      GPStartPosition.Height - 12);

            Rectangle startRectangle2 = new Rectangle(startRectangle1.X + startPosition.SecondRowOffset,
                                                      startRectangle1.Y + 20,
                                                      startRectangle1.Width,
                                                      startRectangle1.Height + 8);

            if (!PlatformInformation.IsWindows)
            {
                // HACK: Workaround for differences with Mono out of Windows
                // (differently-sized lap line / arrow graphics).
                lapLineRectangle.X++;
                startRectangle1.Y--;
                startRectangle2.Y--;
            }

            clipRegion.Union(lapLineRectangle);
            clipRegion.Union(startRectangle1);
            clipRegion.Union(startRectangle2);
        }

        private void SetBattleStartClipRegion(Region clipRegion, BattleStartPosition startPositionP1, BattleStartPosition startPositionP2)
        {
            this.SetBattleStartClipRegionSub1(clipRegion, startPositionP1);
            this.SetBattleStartClipRegionSub2(clipRegion, startPositionP2);
        }

        private void SetBattleStartClipRegionSub1(Region clipRegion, BattleStartPosition startPosition)
        {
            Rectangle startRectangle = new Rectangle(startPosition.X - (this.scrollPosition.X * 8) - 4,
                                                     startPosition.Y - (this.scrollPosition.Y * 8) - 4,
                                                     9, 8);

            if (!PlatformInformation.IsWindows)
            {
                // HACK: Workaround for differences with Mono out of Windows
                // (differently-sized arrow graphics).
                startRectangle.Y--;
                startRectangle.Inflate(0, 1);
            }

            clipRegion.Union(startRectangle);
        }

        private void SetBattleStartClipRegionSub2(Region clipRegion, BattleStartPosition startPosition)
        {
            Rectangle startRectangle = new Rectangle(startPosition.X - (this.scrollPosition.X * 8) - 4,
                                                     startPosition.Y - (this.scrollPosition.Y * 8) - 3,
                                                     9, 8);

            if (!PlatformInformation.IsWindows)
            {
                // HACK: Workaround for differences with Mono out of Windows
                // (differently-sized arrow graphics).
                startRectangle.Inflate(0, 1);
            }

            clipRegion.Union(startRectangle);
        }

        private void SetObjectClipRegion(Region clipRegion, TrackObject hoveredObject)
        {
            if (hoveredObject != null)
            {
                int x = (hoveredObject.X - this.scrollPosition.X) * 8;
                int y = (hoveredObject.Y - this.scrollPosition.Y) * 8;
                int width = 8;
                int height = 8;

                if (hoveredObject is TrackObjectMatchRace)
                {
                    x -= 11;
                    y -= 11;
                    width += 23;
                    height += 23;
                    // HACK: Should be 22 rather than 23,
                    // but this is needed if the zoom level is below 1,
                    // due to the hack in the SetPaintRegions method.
                }

                Rectangle hoveredObjectRectangle = new Rectangle(x, y, width, height);

                clipRegion.Union(hoveredObjectRectangle);
            }
        }

        private void SetAIClipRegion(Region clipRegion, TrackAIElement hoveredAIElem, TrackAIElement selectedAIElem)
        {
            if (hoveredAIElem != null)
            {
                Rectangle clipRectangle = this.GetAIClipRectangle(hoveredAIElem);
                clipRegion.Union(clipRectangle);
            }

            if (selectedAIElem != null &&
                selectedAIElem != hoveredAIElem)
            {
                Rectangle clipRectangle = this.GetAIClipRectangle(selectedAIElem);
                clipRegion.Union(clipRectangle);
            }
        }

        private Rectangle GetAIClipRectangle(TrackAIElement aiElement)
        {
            // Get the rectangle that includes the whole AI element (zone + target).
            // NOTE: Returning a GraphicsPath would be more precise.

            int x;
            int y;
            int width;
            int height;

            if (aiElement.Zone.X <= aiElement.Target.X)
            {
                x = aiElement.Zone.X;
                if (aiElement.Target.X >= aiElement.Zone.Right)
                {
                    width = aiElement.Target.X - aiElement.Zone.Left + 1;
                }
                else
                {
                    width = aiElement.Zone.Width;
                }
            }
            else
            {
                x = aiElement.Target.X;
                width = aiElement.Zone.Right - aiElement.Target.X;
            }

            if (aiElement.Zone.Y <= aiElement.Target.Y)
            {
                y = aiElement.Zone.Y;
                if (aiElement.Target.Y >= aiElement.Zone.Bottom)
                {
                    height = aiElement.Target.Y - aiElement.Zone.Top + 1;
                }
                else
                {
                    height = aiElement.Zone.Height;
                }
            }
            else
            {
                y = aiElement.Target.Y;
                height = aiElement.Zone.Bottom - aiElement.Target.Y;
            }

            Rectangle hoveredAIElemRectangle = new Rectangle((x - this.scrollPosition.X) * 8,
                                                             (y - this.scrollPosition.Y) * 8,
                                                             width * 8,
                                                             height * 8);

            return hoveredAIElemRectangle;
        }

        private void SetPaintRegions(Graphics g, Graphics backBuffer, Region clipRegion)
        {
            backBuffer.Clip = clipRegion;
            backBuffer.SetClip(this.dirtyRegion, CombineMode.Union);

            // This is for the front buffer,
            // which needs to clip graphics depending on the zoom level
            if (this.zoom == 1)
            {
                g.Clip = backBuffer.Clip;
            }
            else
            {
                Region zoomedClipRegion = backBuffer.Clip;
                zoomedClipRegion.Transform(this.zoomMatrix);

                if (this.zoom < 1)
                {
                    // HACK: Avoid clipping issues (rounding differences)
                    // with the DrawTrackTileset method.
                    zoomedClipRegion.Translate(0.5f, 0.5f);
                }

                g.Clip = zoomedClipRegion;
            }
        }

        private void DrawTileSelection(Graphics g, Rectangle selectionRectangle, MouseButtons mouseButtons)
        {
            if (mouseButtons == MouseButtons.Right) // A multiple tile selection is happening now
            {
                g.FillRectangle(this.tileSelectBrush, selectionRectangle);
                g.DrawRectangle(this.tileSelectPen, selectionRectangle);
            }
            else if (selectionRectangle != Rectangle.Empty) // The user is simply hovering tiles
            {
                g.DrawRectangle(this.tileHighlightPen, selectionRectangle);
            }
        }

        private void DrawOverlay(Graphics g, OverlayTile hoveredOverlayTile, OverlayTile selectedOverlayTile, OverlayTilePattern selectedPattern, Point selectedPatternLocation)
        {
            Tile[] tiles = this.track.GetRoadTileset();

            foreach (OverlayTile overlayTile in this.track.OverlayTiles)
            {
                this.DrawOverlayTile(g, overlayTile, tiles);
            }

            if (hoveredOverlayTile != null)
            {
                g.FillRectangle(this.overlayHighlightBrush,
                                (hoveredOverlayTile.X - this.scrollPosition.X) * 8,
                                (hoveredOverlayTile.Y - this.scrollPosition.Y) * 8,
                                hoveredOverlayTile.Width * 8,
                                hoveredOverlayTile.Height * 8);
            }

            if (selectedOverlayTile != null &&
                selectedOverlayTile != hoveredOverlayTile)
            {
                g.FillRectangle(this.overlayHighlightBrush,
                                (selectedOverlayTile.X - this.scrollPosition.X) * 8,
                                (selectedOverlayTile.Y - this.scrollPosition.Y) * 8,
                                selectedOverlayTile.Width * 8,
                                selectedOverlayTile.Height * 8);
            }

            if (selectedPattern != null)
            {
                this.DrawOverlayPattern(g, selectedPattern, selectedPatternLocation, tiles);
            }
        }

        private void DrawOverlayTile(Graphics g, OverlayTile overlayTile, Tile[] tiles)
        {
            this.DrawOverlayTileSub(g, null, overlayTile.Pattern, overlayTile.Location, tiles);
        }

        private void DrawOverlayPattern(Graphics g, OverlayTilePattern overlayTilePattern, Point location, Tile[] tiles)
        {
            this.DrawOverlayTileSub(g, this.translucidImageAttr, overlayTilePattern, location, tiles);
        }

        private void DrawOverlayTileSub(Graphics g, ImageAttributes imageAttr, OverlayTilePattern overlayTilePattern, Point location, Tile[] tiles)
        {
            for (int x = 0; x < overlayTilePattern.Width; x++)
            {
                for (int y = 0; y < overlayTilePattern.Height; y++)
                {
                    byte tileId = overlayTilePattern.Tiles[y][x];
                    if (tileId == 0xFF)
                    {
                        continue;
                    }

                    Tile tile = tiles[tileId];

                    g.DrawImage(tile.Bitmap,
                                new Rectangle((location.X + x - this.scrollPosition.X) * 8,
                                              (location.Y + y - this.scrollPosition.Y) * 8,
                                              tile.Bitmap.Width, tile.Bitmap.Height),
                                0, 0, tile.Bitmap.Width, tile.Bitmap.Height,
                                GraphicsUnit.Pixel, imageAttr);
                }
            }
        }

        private void DrawStartData(Graphics g)
        {
            if (this.track is GPTrack)
            {
                this.DrawLapLine(g);
                this.DrawGPStartPositions(g);
            }
            else
            {
                this.DrawBattleStartPositions(g);
            }
        }

        private void DrawLapLine(Graphics g)
        {
            GPTrack gpTrack = this.track as GPTrack;

            Point location = new Point(gpTrack.LapLine.X - this.scrollPosition.X * 8,
                                       gpTrack.LapLine.Y - this.scrollPosition.Y * 8);

            g.DrawLine(this.lapLineOutlinePen, location.X, location.Y,
                       location.X + gpTrack.LapLine.Length, location.Y);

            g.DrawLine(this.lapLinePen, location.X + 1, location.Y,
                       location.X + gpTrack.LapLine.Length - 2, location.Y);
        }

        private void DrawGPStartPositions(Graphics g)
        {
            GPTrack gpTrack = this.track as GPTrack;

            int x = gpTrack.StartPosition.X - this.scrollPosition.X * 8;
            int y = gpTrack.StartPosition.Y - this.scrollPosition.Y * 8;
            int secondRowOffset = gpTrack.StartPosition.SecondRowOffset;

            for (int pos = 0; pos <= 18; pos += 6)
            {
                // 1st Column
                this.DrawUpArrow(g, x, y + pos * 8);

                // 2nd Column
                this.DrawUpArrow(g, x + secondRowOffset, y + ((3 + pos) * 8));
            }
        }

        private void DrawBattleStartPositions(Graphics g)
        {
            BattleTrack bTrack = this.track as BattleTrack;

            this.DrawBattleStartPosition(g, bTrack.StartPositionP2, this.DrawDownArrow);
            this.DrawBattleStartPosition(g, bTrack.StartPositionP1, this.DrawUpArrow);
        }

        private void DrawBattleStartPosition(Graphics g, Point location, ArrowDrawer arrowDrawer)
        {
            int x = location.X - this.scrollPosition.X * 8;
            int y = location.Y - this.scrollPosition.Y * 8;
            arrowDrawer(g, x, y);
        }

        private delegate void ArrowDrawer(Graphics g, int x, int y);

        private void DrawUpArrow(Graphics g, int x, int y)
        {
            Point[] arrow = new Point[]
            {
                new Point(x, y - 4),
                new Point(x + 4, y + 3),
                new Point(x - 4, y + 3)
            };
            this.DrawArrow(g, arrow);
        }

        private void DrawDownArrow(Graphics g, int x, int y)
        {
            Point[] arrow = new Point[]
            {
                new Point(x - 4, y - 3),
                new Point(x + 4, y - 3),
                new Point(x, y + 4)
            };
            this.DrawArrow(g, arrow);
        }

        private void DrawSmallUpArrow(Graphics g, int x, int y)
        {
            Point[] arrow = new Point[]
            {
                new Point(x, y - 4),
                new Point(x - 1, y - 4),
                new Point(x + 3, y + 3),
                new Point(x - 4, y + 3)
            };
            this.DrawArrow(g, arrow);
        }

        private void DrawSmallDownArrow(Graphics g, int x, int y)
        {
            Point[] arrow = new Point[]
            {
                new Point(x - 4, y - 3),
                new Point(x + 3, y - 3),
                new Point(x - 1, y + 4),
                new Point(x, y + 4)
            };
            this.DrawArrow(g, arrow);
        }

        private void DrawSmallLeftArrow(Graphics g, int x, int y)
        {
            Point[] arrow = new Point[]
            {
                new Point(x - 4, y),
                new Point(x - 4, y - 1),
                new Point(x + 3, y + 3),
                new Point(x + 3, y - 4)
            };
            this.DrawArrow(g, arrow);
        }

        private void DrawSmallRightArrow(Graphics g, int x, int y)
        {
            Point[] arrow = new Point[]
            {
                new Point(x - 3, y - 4),
                new Point(x - 3, y + 3),
                new Point(x + 4, y - 1),
                new Point(x + 4, y)
            };
            this.DrawArrow(g, arrow);
        }

        private void DrawArrow(Graphics g, Point[] arrow)
        {
            g.FillPolygon(this.startPositionBrush, arrow);
            g.DrawPolygon(this.startPositionPen, arrow);
        }

        private void DrawObjectData(Graphics g, bool frontZonesView)
        {
            if (this.track is GPTrack &&
                (this.track as GPTrack).ObjectZones != null)
            {
                this.DrawObjectZones(g, frontZonesView);
                this.DrawObjects(g);
            }
        }

        private void DrawObjectZones(Graphics g, bool frontZonesView)
        {
            g.PixelOffsetMode = PixelOffsetMode.Half;

            Bitmap bitmap = this.GetObjectZonesBitmap(frontZonesView);
            g.DrawImage(bitmap,
                        new Rectangle(-this.scrollPosition.X * 8,
                                      -this.scrollPosition.Y * 8,
                                      this.trackCache.Width,
                                      this.trackCache.Height),
                        0, 0, bitmap.Width, bitmap.Height,
                        GraphicsUnit.Pixel, this.translucidImageAttr);
            // NOTE: Do not dispose bitmap, it's cached (field objectZonesCache).

            g.PixelOffsetMode = PixelOffsetMode.Default;
        }

        private Bitmap GetObjectZonesBitmap(bool frontZonesView)
        {
            GPTrack gpTrack = this.track as GPTrack;
            byte[][] zones = gpTrack.ObjectZones.GetGrid(frontZonesView);

            if (this.zones != zones)
            {
                this.zones = zones;
                this.objectZonesCache.Dispose();
                this.objectZonesCache = this.CreateObjectZonesBitmap();
            }

            return this.objectZonesCache;
        }

        private Bitmap CreateObjectZonesBitmap()
        {
            Bitmap bitmap = new Bitmap(64, 64, this.trackCache.PixelFormat);
            FastBitmap fBitmap = new FastBitmap(bitmap);

            for (int y = 0; y < this.zones.Length; y++)
            {
                for (int x = 0; x < this.zones[y].Length; x++)
                {
                    byte zoneIndex = this.zones[y][x];
                    fBitmap.SetPixel(x, y, this.objectBrushes[zoneIndex].Color);
                }
            }

            fBitmap.Release();
            return bitmap;
        }

        private void DrawObjects(Graphics g)
        {
            GPTrack gpTrack = this.track as GPTrack;

            // 2P Match Race objects (Chain Chomps or Bananas)
            for (int i = gpTrack.Objects.Count - 1; i >= 16; i--)
            {
                TrackObjectMatchRace trackObject = gpTrack.Objects[i] as TrackObjectMatchRace;
                int x = (trackObject.X - this.scrollPosition.X) * 8;
                int y = (trackObject.Y - this.scrollPosition.Y) * 8;
                Rectangle trackObjectRect = new Rectangle(x, y, 7, 7);
                g.DrawEllipse(this.objectOutlinePen, trackObjectRect);
                g.FillEllipse(this.objectBrushes[4], trackObjectRect);

                if (trackObject.Direction == Direction.Horizontal)
                {
                    this.DrawSmallLeftArrow(g, x - 7, y + 4);
                    this.DrawSmallRightArrow(g, x + 14, y + 4);
                    g.DrawLine(this.objectMatchRacePen, x + 2, y + 2, x + 2, y + 5);
                    g.DrawLine(this.objectMatchRacePen, x + 2, y + 3, x + 5, y + 3);
                    g.DrawLine(this.objectMatchRacePen, x + 5, y + 2, x + 5, y + 5);
                }
                else if (trackObject.Direction == Direction.Vertical)
                {
                    this.DrawSmallUpArrow(g, x + 4, y - 7);
                    this.DrawSmallDownArrow(g, x + 4, y + 14);
                    g.DrawLine(this.objectMatchRacePen, x + 2, y + 2, x + 3, y + 5);
                    g.DrawLine(this.objectMatchRacePen, x + 4, y + 5, x + 5, y + 2);
                }
            }

            // Regular objects (Pipes, Piranha Plants...)
            for (int i = 15; i >= 0; i--)
            {
                TrackObject trackObject = gpTrack.Objects[i];
                int x = (trackObject.X - this.scrollPosition.X) * 8;
                int y = (trackObject.Y - this.scrollPosition.Y) * 8;
                Rectangle trackObjectRect = new Rectangle(x, y, 7, 7);
                g.DrawEllipse(this.objectOutlinePen, trackObjectRect);
                g.FillEllipse(this.objectBrushes[i / 4], trackObjectRect);
            }
        }

        private void DrawAI(Graphics g, TrackAIElement hoveredAIElem, TrackAIElement selectedAIElem, bool isAITargetHovered)
        {
            this.DrawAllAIElements(g);

            if (hoveredAIElem != selectedAIElem)
            {
                this.HighlightHoveredAIElement(g, hoveredAIElem, isAITargetHovered);
            }
            this.HighlightSelectedAIElement(g, selectedAIElem);
        }

        private void DrawAllAIElements(Graphics g)
        {
            foreach (TrackAIElement aiElem in this.track.AI)
            {
                int pointX = (aiElem.Target.X - this.scrollPosition.X) * 8;
                int pointY = (aiElem.Target.Y - this.scrollPosition.Y) * 8;
                g.DrawEllipse(this.objectOutlinePen, pointX + 1, pointY + 1, 5, 5);

                Rectangle zone = this.GetAIZoneRectangle(aiElem);

                Point target = new Point(pointX + 4, pointY + 4);

                int speed = aiElem.Speed;

                if (aiElem.ZoneShape == Shape.Rectangle)
                {
                    this.PaintTopSide(g, zone, target, speed);
                    this.PaintRightSide(g, zone, target, speed);
                    this.PaintBottomSide(g, zone, target, speed);
                    this.PaintLeftSide(g, zone, target, speed);

                    g.DrawRectangle(this.aiZonePens[speed], zone);
                }
                else
                {
                    Point[] points = this.GetAIZoneTriangle(aiElem);

                    g.DrawPolygon(this.aiZonePens[aiElem.Speed], points);

                    switch (aiElem.ZoneShape)
                    {
                        case Shape.TriangleTopLeft:
                            this.PaintTopSide(g, zone, target, speed);
                            this.PaintLeftSide(g, zone, target, speed);
                            TrackDrawer.PaintTriangleDiagonalSide(g, points, target, this.aiZoneBrushes[speed][2]);
                            break;

                        case Shape.TriangleTopRight:
                            this.PaintTopSide(g, zone, target, speed);
                            this.PaintRightSide(g, zone, target, speed);
                            TrackDrawer.PaintTriangleDiagonalSide(g, points, target, this.aiZoneBrushes[speed][2]);
                            break;

                        case Shape.TriangleBottomRight:
                            this.PaintBottomSide(g, zone, target, speed);
                            this.PaintRightSide(g, zone, target, speed);
                            TrackDrawer.PaintTriangleDiagonalSide(g, points, target, this.aiZoneBrushes[speed][0]);
                            break;

                        case Shape.TriangleBottomLeft:
                            this.PaintBottomSide(g, zone, target, speed);
                            this.PaintLeftSide(g, zone, target, speed);
                            TrackDrawer.PaintTriangleDiagonalSide(g, points, target, this.aiZoneBrushes[speed][0]);
                            break;
                    }
                }
            }
        }

        private void PaintTopSide(Graphics g, Rectangle zone, Point target, int speed)
        {
            if (target.Y > zone.Top)
            {
                Point[] side =
                {
                    new Point(zone.Left, zone.Top),
                    new Point(zone.Right, zone.Top),
                    target
                };

                g.FillPolygon(this.aiZoneBrushes[speed][0], side);
            }
        }

        private void PaintRightSide(Graphics g, Rectangle zone, Point target, int speed)
        {
            if (target.X < zone.Right)
            {
                Point[] side =
                {
                    new Point(zone.Right, zone.Top),
                    new Point(zone.Right, zone.Bottom),
                    target
                };

                g.FillPolygon(this.aiZoneBrushes[speed][1], side);
            }
        }

        private void PaintBottomSide(Graphics g, Rectangle zone, Point target, int speed)
        {
            if (target.Y < zone.Bottom)
            {
                Point[] side =
                {
                    new Point(zone.Right, zone.Bottom),
                    new Point(zone.Left, zone.Bottom),
                    target
                };

                g.FillPolygon(this.aiZoneBrushes[speed][2], side);
            }
        }

        private void PaintLeftSide(Graphics g, Rectangle zone, Point target, int speed)
        {
            if (target.X > zone.Left)
            {
                Point[] side =
                {
                    new Point(zone.Left, zone.Bottom),
                    new Point(zone.Left, zone.Top),
                    target
                };

                g.FillPolygon(this.aiZoneBrushes[speed][1], side);
            }
        }

        private static void PaintTriangleDiagonalSide(Graphics g, Point[] points, Point target, SolidBrush brush)
        {
            points[points.Length - 2] = target;
            g.FillPolygon(brush, points);
        }

        private void HighlightHoveredAIElement(Graphics g, TrackAIElement hoveredAIElem, bool isAITargetHovered)
        {
            if (hoveredAIElem == null)
            {
                return;
            }

            Rectangle zone = this.GetAIZoneRectangle(hoveredAIElem);

            if (isAITargetHovered)
            {
                this.DrawAITargetLines(g, hoveredAIElem, zone, this.aiElementHighlightPen);
            }
            else
            {
                if (hoveredAIElem.ZoneShape == Shape.Rectangle)
                {
                    g.DrawRectangle(this.aiElementHighlightPen, zone);
                }
                else
                {
                    Point[] points = this.GetAIZoneTriangle(hoveredAIElem);

                    g.DrawLines(this.aiElementHighlightPen, points);
                }
            }
        }

        private void HighlightSelectedAIElement(Graphics g, TrackAIElement selectedAIElem)
        {
            if (selectedAIElem == null)
            {
                return;
            }

            Rectangle zone = this.GetAIZoneRectangle(selectedAIElem);

            this.DrawAITargetLines(g, selectedAIElem, zone, this.aiElementSelectPen);

            if (selectedAIElem.ZoneShape == Shape.Rectangle)
            {
                g.DrawRectangle(this.aiElementSelectPen, zone);
                g.FillRectangle(this.aiZoneBrushes[selectedAIElem.Speed][0], zone);
            }
            else
            {
                Point[] points = this.GetAIZoneTriangle(selectedAIElem);

                g.DrawPolygon(this.aiElementSelectPen, points);
                g.FillPolygon(this.aiZoneBrushes[selectedAIElem.Speed][0], points);
            }
        }

        private Rectangle GetAIZoneRectangle(TrackAIElement aiElem)
        {
            int zoneX = (aiElem.Zone.X - this.scrollPosition.X) * 8;
            int zoneY = (aiElem.Zone.Y - this.scrollPosition.Y) * 8;
            int zoneWidth = aiElem.Zone.Width * 8;
            int zoneHeight = aiElem.Zone.Height * 8;

            return new Rectangle(zoneX, zoneY, zoneWidth - 1, zoneHeight - 1);
        }

        private Point[] GetAIZoneTriangle(TrackAIElement aiElem)
        {
            Point[] points = aiElem.GetTriangle();

            int xCorrectionStep;
            int yCorrectionStep;

            switch (aiElem.ZoneShape)
            {
                default:
                case Shape.TriangleTopLeft:
                    xCorrectionStep = 1;
                    yCorrectionStep = 1;
                    break;

                case Shape.TriangleTopRight:
                    xCorrectionStep = 0;
                    yCorrectionStep = 1;
                    break;

                case Shape.TriangleBottomRight:
                    xCorrectionStep = 0;
                    yCorrectionStep = 0;
                    break;

                case Shape.TriangleBottomLeft:
                    xCorrectionStep = 1;
                    yCorrectionStep = 0;
                    break;
            }

            for (int i = 0; i < points.Length; i++)
            {
                int xCorrection;
                int yCorrection;

                if (points[i].X == aiElem.Zone.Left)
                {
                    xCorrection = 0;
                }
                else
                {
                    xCorrection = xCorrectionStep;
                }

                if (points[i].Y == aiElem.Zone.Top)
                {
                    yCorrection = 0;
                }
                else
                {
                    yCorrection = yCorrectionStep;
                }

                points[i] =
                    new Point((points[i].X - this.scrollPosition.X) * 8 - xCorrection,
                              (points[i].Y - this.scrollPosition.Y) * 8 - yCorrection);
            }

            switch (aiElem.ZoneShape)
            {
                case Shape.TriangleTopRight:
                    points[0].X--;
                    points[points.Length - 2].X--;
                    points[points.Length - 1].X--;
                    break;

                case Shape.TriangleBottomRight:
                    points[0].X--;
                    points[points.Length - 3].Y--;
                    points[points.Length - 2].Offset(-1, -1);
                    points[points.Length - 1].X--;
                    break;

                case Shape.TriangleBottomLeft:
                    points[points.Length - 3].Y--;
                    points[points.Length - 2].Y--;
                    break;
            }

            return points;
        }

        private void DrawAITargetLines(Graphics g, TrackAIElement aiElem, Rectangle zone, Pen pen)
        {
            int pointX = (aiElem.Target.X - this.scrollPosition.X) * 8;
            int pointY = (aiElem.Target.Y - this.scrollPosition.Y) * 8;

            Point targetPoint = new Point(pointX + 4, pointY + 4);

            switch (aiElem.ZoneShape)
            {
                case Shape.Rectangle:
                    {
                        Point[] targetLines =
                        {
                            targetPoint,
                            new Point(zone.Left, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Bottom),
                            targetPoint,
                            new Point(zone.Left, zone.Bottom)
                        };
                        g.DrawLines(pen, targetLines);
                        break;
                    }

                case Shape.TriangleTopLeft:
                    {
                        Point[] targetLines =
                        {
                            targetPoint,
                            new Point(zone.Left, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Top),
                            targetPoint,
                            new Point(zone.Left, zone.Bottom)
                        };
                        g.DrawLines(pen, targetLines);
                        break;
                    }

                case Shape.TriangleTopRight:
                    {
                        Point[] targetLines =
                        {
                            targetPoint,
                            new Point(zone.Left, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Bottom)
                        };
                        g.DrawLines(pen, targetLines);
                        break;
                    }

                case Shape.TriangleBottomRight:
                    {
                        Point[] targetLines =
                        {
                            targetPoint,
                            new Point(zone.Right, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Bottom),
                            targetPoint,
                            new Point(zone.Left, zone.Bottom)
                        };
                        g.DrawLines(pen, targetLines);
                        break;
                    }

                case Shape.TriangleBottomLeft:
                    {
                        Point[] targetLines =
                        {
                            targetPoint,
                            new Point(zone.Left, zone.Top),
                            targetPoint,
                            new Point(zone.Right, zone.Bottom),
                            targetPoint,
                            new Point(zone.Left, zone.Bottom)
                        };
                        g.DrawLines(pen, targetLines);
                        break;
                    }
            }
        }

        private void PaintTrackOutbounds(Graphics g)
        {
            Rectangle trackArea = new Rectangle(0, 0, (int)(this.imageSize.Width * this.zoom), (int)(this.imageSize.Height * this.zoom));
            Rectangle[] outBounds = new Rectangle[]
            {
                // Right outbounds
                new Rectangle(trackArea.Right, 0, this.control.Width - trackArea.Width, trackArea.Height),

                // Bottom outbounds
                new Rectangle(0, trackArea.Bottom, this.control.Width, this.control.Height - trackArea.Height)
            };
            g.FillRectangles(Brushes.Black, outBounds);
        }

        public void NotifyFullRepaintNeed()
        {
            this.fullRepaintNeeded = true;
        }

        public void ResizeWindow()
        {
            this.SetImageSize();

            if (this.imageSize.Width == 0 || this.imageSize.Height == 0)
            {
                // Ensure the image width and height are at least equal to 1,
                // so that the Bitmap creation doesn't fail
                this.imageSize.Width = 1;
                this.imageSize.Height = 1;
            }
            else
            {
                this.NotifyFullRepaintNeed();
            }
        }

        private void SetImageSize()
        {
            int trackMapSize = 128;
            int imageWidth = (int)Math.Min(this.control.Width / this.zoom, (trackMapSize - this.scrollPosition.X) * 8);
            int imageHeight = (int)Math.Min(this.control.Height / this.zoom, (trackMapSize - this.scrollPosition.Y) * 8);
            this.imageSize = new Size(imageWidth, imageHeight);
        }

        public void UpdateCacheAfterTileLaying(Point absolutePosition)
        {
            using (Graphics g = Graphics.FromImage(this.trackCache))
            {
                g.DrawImageUnscaled(this.tileClipboardCache, absolutePosition.X * 8, absolutePosition.Y * 8);
            }
        }

        public void UpdateTileClipboard(Tile tile)
        {
            this.tileClipboardCache.Dispose();
            this.tileClipboardCache = (Bitmap)tile.Bitmap.Clone();
        }

        public void UpdateTileClipboard(int x, int y, Size size)
        {
            this.tileClipboardCache.Dispose();

            Rectangle clipboardRectangle = new Rectangle(x * 8, y * 8, size.Width * 8, size.Height * 8);
            this.tileClipboardCache = this.trackCache.Clone(clipboardRectangle, this.trackCache.PixelFormat);
        }

        public void UpdateTileClipboardOnThemeChange(IList<byte> tiles, Size clipboardSize, Tile[] tileset)
        {
            this.tileClipboardCache.Dispose();

            this.tileClipboardCache = new Bitmap(clipboardSize.Width * 8, clipboardSize.Height * 8, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(this.tileClipboardCache))
            {
                for (int y = 0; y < clipboardSize.Height; y++)
                {
                    for (int x = 0; x < clipboardSize.Width; x++)
                    {
                        Tile tile = tileset[tiles[x + y * clipboardSize.Width]];
                        g.DrawImage(tile.Bitmap, x * 8, y * 8);
                    }
                }
            }
        }

        public void Dispose()
        {
            this.trackCache.Dispose();
            this.tileClipboardCache.Dispose();

            this.dirtyRegion.Dispose();
            this.zoomMatrix.Dispose();

            this.tileHighlightPen.Dispose();
            this.tileSelectPen.Dispose();
            this.tileSelectBrush.Dispose();

            this.overlayHighlightBrush.Dispose();

            this.lapLinePen.Dispose();
            this.lapLineOutlinePen.Dispose();
            this.startPositionBrush.Dispose();
            this.startPositionPen.Dispose();

            this.objectMatchRacePen.Dispose();
            this.objectOutlinePen.Dispose();
            foreach (SolidBrush objectBrush in this.objectBrushes)
            {
                objectBrush.Dispose();
            }

            foreach (SolidBrush[] aiZoneBrushes in this.aiZoneBrushes)
            {
                foreach (SolidBrush aiZoneBrush in aiZoneBrushes)
                {
                    aiZoneBrush.Dispose();
                }
            }
            foreach (Pen aiZonePen in this.aiZonePens)
            {
                aiZonePen.Dispose();
            }
            this.aiElementHighlightPen.Dispose();
            this.aiElementSelectPen.Dispose();

            this.translucidImageAttr.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
