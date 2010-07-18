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
		private Track track;

		private float zoom;

		private Graphics trackGfx;

		private Bitmap trackCache;
		private Bitmap tileClipboardCache;

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
		/// Used to fill in object zones.
		/// </summary>
		private SolidBrush[] objectZoneBrushes;

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

		private bool fullRepaintNeeded;
		private Rectangle dirtyArea;

		public TrackDrawer(Control trackCtrl, float zoom)
		{
			this.trackGfx = trackCtrl.CreateGraphics();

			this.SetZoom(zoom);

			#region Pens and Brushes initialization

			this.tileHighlightPen = new Pen(Color.FromArgb(150, 255, 0, 0), 1);
			this.tileSelectPen = new Pen(Color.FromArgb(150, 20, 130, 255), 1);
			this.tileSelectBrush = new SolidBrush(Color.FromArgb(50, 20, 130, 255));

			this.overlayHighlightBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));

			this.lapLinePen = new Pen(Color.White);
			this.lapLineOutlinePen = new Pen(Color.Black, 3);
			this.startPositionBrush = new SolidBrush(Color.White);
			this.startPositionPen = new Pen(Color.Gray, 1);

			this.objectZoneBrushes = new SolidBrush[4];
			this.objectZoneBrushes[0] = new SolidBrush(Color.FromArgb(150, 204, 51, 51)); // Object zone 1 color
			this.objectZoneBrushes[1] = new SolidBrush(Color.FromArgb(150, 21, 94, 177)); // Object zone 2 color
			this.objectZoneBrushes[2] = new SolidBrush(Color.FromArgb(150, 16, 150, 24)); // Object zone 3 color
			this.objectZoneBrushes[3] = new SolidBrush(Color.FromArgb(150, 230, 186, 64)); // Object zone 4 color

			this.objectOutlinePen = new Pen(Color.White, 2);
			this.objectBrushes = new SolidBrush[5];
			this.objectBrushes[0] = new SolidBrush(Color.FromArgb(255, 204, 51, 51)); // Zone 1 object color
			this.objectBrushes[1] = new SolidBrush(Color.FromArgb(255, 21, 94, 177)); // Zone 2 object color
			this.objectBrushes[2] = new SolidBrush(Color.FromArgb(255, 16, 150, 24)); // Zone 3 object color
			this.objectBrushes[3] = new SolidBrush(Color.FromArgb(255, 230, 186, 64)); // Zone 4 object color
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

			// The following members are initialized so they can be disposed of
			// in each function without having to check if they're null beforehand
			this.trackCache = this.tileClipboardCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}

		/// <summary>
		/// Load a track and generate the track image cache.
		/// </summary>
		/// <param name="track">The track.</param>
		public void LoadTrack(Track track)
		{
			this.trackCache.Dispose();

			this.track = track;
			Tile[] tileset = track.GetRoadTileset();

			this.trackCache = new Bitmap(this.track.Map.Width * 8, this.track.Map.Height * 8, PixelFormat.Format32bppPArgb);
			using (Graphics gfx = Graphics.FromImage(this.trackCache))
			{
				for (int x = 0; x < this.track.Map.Width; x++)
				{
					for (int y = 0; y < this.track.Map.Height; y++)
					{
						Tile tile = tileset[this.track.Map[x, y]];
						gfx.DrawImage(tile.Bitmap, x * 8, y * 8);
					}
				}
			}

			this.NotifyFullRepaintNeed();
		}

		public void SetZoom(float zoom)
		{
			this.zoom = zoom;
			this.SetInterpolationMode();
			this.NotifyFullRepaintNeed();
		}

		private void SetInterpolationMode()
		{
			this.trackGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			if (this.zoom >= 1)
			{
				this.trackGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			}
			else
			{
				this.trackGfx.InterpolationMode = InterpolationMode.Bilinear;
			}
		}

		public void DrawTrack(Size panelSize, Point scrollPosition, Point cursorPosition, Size selectionSize, Point selectionStart, ActionButton action,
							  EditionMode editionMode, OverlayTile hoveredOverlayTile, TrackObject hoveredObject, bool frontZonesView,
							  TrackAIElement hoveredAIElem, TrackAIElement selectedAIElem, bool isAITargetHovered)
		{
			int imageWidth = (int)Math.Min(panelSize.Width / this.zoom, (this.track.Map.Width - scrollPosition.X) * 8);
			int imageHeight = (int)Math.Min(panelSize.Height / this.zoom, (this.track.Map.Height - scrollPosition.Y) * 8);

			if (imageWidth == 0 || imageHeight == 0)
			{
				return;
			}

			Rectangle clipRectangle;

			using (Bitmap trackImage = this.trackCache.Clone(new Rectangle(scrollPosition.X * 8, scrollPosition.Y * 8, imageWidth, imageHeight), this.trackCache.PixelFormat))
			using (Graphics trackGfxBackBuffer = Graphics.FromImage(trackImage))
			{
				if (editionMode == EditionMode.Tileset)
				{
					if (action != ActionButton.MiddleMouseButton)
					{
						Rectangle selectionRectangle = this.CreateAndDrawSelectionRectangle(trackGfxBackBuffer, scrollPosition, cursorPosition, selectionSize, selectionStart, action);
						clipRectangle = TrackDrawer.GetSelectionClipRectangle(selectionRectangle);
					}
					else
					{
						clipRectangle = Rectangle.Empty;
					}
				}
				else if (editionMode == EditionMode.Overlay)
				{
					clipRectangle = TrackDrawer.GetOverlayClipRectangle(hoveredOverlayTile, scrollPosition);
				}
				else if (editionMode == EditionMode.Start)
				{
					if (this.track is GPTrack)
					{
						GPTrack gpTrack = this.track as GPTrack;
						clipRectangle = TrackDrawer.GetGPStartClipRectangle(gpTrack.LapLine, gpTrack.StartPosition, scrollPosition);
					}
					else
					{
						clipRectangle = Rectangle.Empty;
						this.NotifyFullRepaintNeed();
					}
				}
				else if (editionMode == EditionMode.Objects)
				{
					clipRectangle = TrackDrawer.GetObjectClipRectangle(hoveredObject, scrollPosition);
				}
				else if (editionMode == EditionMode.AI)
				{
					clipRectangle = TrackDrawer.GetAIClipRectangle(hoveredAIElem, selectedAIElem, scrollPosition);
				}
				else
				{
					clipRectangle = Rectangle.Empty;
					this.NotifyFullRepaintNeed();
				}

				if (!this.fullRepaintNeeded)
				{
					// If a full repaint isn't needed, we set the clipping regions
					// to only partially repaint the panel
					this.SetPaintRegions(trackGfxBackBuffer, clipRectangle);
				}

				if (editionMode == EditionMode.Overlay)
				{
					this.DrawOverlay(trackGfxBackBuffer, scrollPosition, hoveredOverlayTile);
				}
				else if (editionMode == EditionMode.Start)
				{
					this.DrawStartData(trackGfxBackBuffer, scrollPosition);
				}
				else if (editionMode == EditionMode.Objects)
				{
					this.DrawObjectData(trackGfxBackBuffer, scrollPosition, frontZonesView);
				}
				else if (editionMode == EditionMode.AI)
				{
					this.DrawAI(trackGfxBackBuffer, scrollPosition);
					if (hoveredAIElem != selectedAIElem)
					{
						this.HighlightHoveredAIElement(trackGfxBackBuffer, scrollPosition, hoveredAIElem, isAITargetHovered);
					}
					this.HighlightSelectedAIElement(trackGfxBackBuffer, scrollPosition, selectedAIElem);
				}

				this.trackGfx.DrawImage(trackImage, 0, 0, imageWidth * this.zoom, imageHeight * this.zoom);
			}

			this.PaintTrackOutbounds(imageWidth, imageHeight, panelSize);

			this.dirtyArea = clipRectangle;
			this.fullRepaintNeeded = false;

			this.trackGfx.ResetClip();
		}

		private Rectangle CreateAndDrawSelectionRectangle(Graphics graphics, Point scrollPosition, Point cursorPosition, Size selectionSize, Point selectionStart, ActionButton action)
		{
			Rectangle selectionRectangle;
			if (action == ActionButton.RightMouseButton) // A multiple tile selection is happening now
			{
				selectionStart.X -= scrollPosition.X;
				selectionStart.Y -= scrollPosition.Y;
				selectionRectangle = new Rectangle((selectionStart.X * 8) - 1, (selectionStart.Y * 8) - 1, selectionSize.Width * 8 + 1, selectionSize.Height * 8 + 1);
				graphics.FillRectangle(this.tileSelectBrush, selectionRectangle);
				graphics.DrawRectangle(this.tileSelectPen, selectionRectangle);
			}
			else if (cursorPosition.X != -1) // The user is simply hovering tiles
			{
				selectionRectangle = new Rectangle((cursorPosition.X * 8) - 1, (cursorPosition.Y * 8) - 1, selectionSize.Width * 8 + 1, selectionSize.Height * 8 + 1);
				graphics.DrawRectangle(this.tileHighlightPen, selectionRectangle);
			}
			else
			{
				selectionRectangle = Rectangle.Empty;
			}
			return selectionRectangle;
		}

		private static Rectangle GetSelectionClipRectangle(Rectangle rectangle)
		{
			// Enlarge rectangle by 1px to account for the 1px border of the selection
			rectangle.Inflate(1, 1);

			return rectangle;
		}

		private static Rectangle GetOverlayClipRectangle(OverlayTile hoveredOverlayTile, Point scrollPosition)
		{
			Rectangle hoveredOverlayRectangle;

			if (hoveredOverlayTile == null)
			{
				hoveredOverlayRectangle = Rectangle.Empty;
			}
			else
			{
				hoveredOverlayRectangle =
					new Rectangle((hoveredOverlayTile.X - scrollPosition.X) * 8,
								  (hoveredOverlayTile.Y - scrollPosition.Y) * 8,
								  hoveredOverlayTile.Width * 8,
								  hoveredOverlayTile.Height * 8);
			}

			return hoveredOverlayRectangle;
		}

		private static Rectangle GetGPStartClipRectangle(LapLine lapLine, StartPosition startPosition, Point scrollPosition)
		{
			int x = Math.Min(lapLine.X, startPosition.X + Math.Min(0, startPosition.SecondRowOffset));
			int y = Math.Min(lapLine.Y, startPosition.Y);
			int width = Math.Max(lapLine.Right, startPosition.X + Math.Max(0, startPosition.SecondRowOffset)) - x;
			int height = Math.Max(lapLine.Y, startPosition.Y + this.Height) - y;

			Rectangle startRectangle =
				new Rectangle(x - (scrollPosition.X * 8) - 4,
				              y - (scrollPosition.Y * 8) - 4,
							  width + 8, height + 8);

			return startRectangle;
		}

		private static Rectangle GetObjectClipRectangle(TrackObject hoveredObject, Point scrollPosition)
		{
			Rectangle hoveredObjectRectangle;

			if (hoveredObject == null)
			{
				hoveredObjectRectangle = Rectangle.Empty;
			}
			else
			{
				hoveredObjectRectangle =
					new Rectangle((hoveredObject.X - scrollPosition.X) * 8,
								  (hoveredObject.Y - scrollPosition.Y) * 8, 8, 8);
			}

			return hoveredObjectRectangle;
		}

		private static Rectangle GetAIClipRectangle(TrackAIElement hoveredAIElem, TrackAIElement selectedAIElem, Point scrollPosition)
		{
			Rectangle clipRectangle = TrackDrawer.GetAIClipRectangleSub(hoveredAIElem, scrollPosition);

			if (hoveredAIElem != selectedAIElem)
			{
				Rectangle clipRectangle2 = TrackDrawer.GetAIClipRectangleSub(selectedAIElem, scrollPosition);
				int x = Math.Min(clipRectangle.X, clipRectangle2.X);
				int y = Math.Min(clipRectangle.Y, clipRectangle2.Y);
				int width = Math.Max(clipRectangle.Right, clipRectangle2.Right) - x;
				int height = Math.Max(clipRectangle.Bottom, clipRectangle2.Bottom) - y;
				clipRectangle = new Rectangle(x, y, width, height);
			}

			return clipRectangle;
		}

		private static Rectangle GetAIClipRectangleSub(TrackAIElement aiElement, Point scrollPosition)
		{
			// Get the rectangle that includes the whole AI element (zone + target).
			// NOTE: This clipping method isn't optimal,
			// returning a GraphicsPath would be more precise.

			if (aiElement == null)
			{
				return Rectangle.Empty;
			}

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

			Rectangle hoveredAIElemRectangle = new Rectangle((x - scrollPosition.X) * 8,
															 (y - scrollPosition.Y) * 8,
															 width * 8,
															 height * 8);

			return hoveredAIElemRectangle;
		}

		private void SetPaintRegions(Graphics graphics, Rectangle clipRectangle)
		{
			// This is for the main buffer,
			// which needs to know the zoom level to define the right clipping
			if (this.zoom != 1)
			{
				this.SetZoomedPaintRegion(this.trackGfx, clipRectangle);
			}
			else
			{
				this.SetPaintRegion(this.trackGfx, clipRectangle);
			}

			// This is for the back buffer, which doesn't need the zoom level
			this.SetPaintRegion(graphics, clipRectangle);
		}

		private void SetZoomedPaintRegion(Graphics graphics, Rectangle zoomedCurrentArea)
		{
			Region zoomedPaintRegion = new Region();
			zoomedPaintRegion.MakeEmpty();

			Rectangle zoomedDirtyArea = this.dirtyArea;
			zoomedDirtyArea.X = (int)Math.Ceiling(zoomedDirtyArea.X * this.zoom);
			zoomedDirtyArea.Y = (int)Math.Ceiling(zoomedDirtyArea.Y * this.zoom);
			zoomedDirtyArea.Width = (int)Math.Ceiling(zoomedDirtyArea.Width * this.zoom);
			zoomedDirtyArea.Height = (int)Math.Ceiling(zoomedDirtyArea.Height * this.zoom);

			zoomedCurrentArea.X = (int)Math.Ceiling(zoomedCurrentArea.X * this.zoom);
			zoomedCurrentArea.Y = (int)Math.Ceiling(zoomedCurrentArea.Y * this.zoom);
			zoomedCurrentArea.Width = (int)Math.Ceiling(zoomedCurrentArea.Width * this.zoom);
			zoomedCurrentArea.Height = (int)Math.Ceiling(zoomedCurrentArea.Height * this.zoom);

			zoomedPaintRegion.Union(zoomedDirtyArea);
			zoomedPaintRegion.Union(zoomedCurrentArea);

			graphics.Clip = zoomedPaintRegion;
		}

		private void SetPaintRegion(Graphics graphics, Rectangle currentArea)
		{
			Region paintRegion = new Region();
			paintRegion.MakeEmpty();

			paintRegion.Union(this.dirtyArea);
			paintRegion.Union(currentArea);

			graphics.Clip = paintRegion;
		}

		private void DrawOverlay(Graphics graphics, Point scrollPosition, OverlayTile hoveredOverlayTile)
		{
			Tile[] tiles = this.track.GetRoadTileset();

			foreach (OverlayTile overlayTile in this.track.OverlayTiles)
			{
				TrackDrawer.DrawOverlayTile(graphics, scrollPosition, overlayTile, tiles);
			}

			if (hoveredOverlayTile != null)
			{
				graphics.FillRectangle(this.overlayHighlightBrush,
									   (hoveredOverlayTile.X - scrollPosition.X) * 8,
									   (hoveredOverlayTile.Y - scrollPosition.Y) * 8,
									   hoveredOverlayTile.Width * 8,
									   hoveredOverlayTile.Height * 8);
			}
		}

		private static void DrawOverlayTile(Graphics graphics, Point scrollPosition, OverlayTile overlayTile, Tile[] tiles)
		{
			for (int x = 0; x < overlayTile.Width; x++)
			{
				for (int y = 0; y < overlayTile.Height; y++)
				{
					byte tileId = overlayTile.Pattern.Tiles[y][x];
					if (tileId == 0xFF)
					{
						continue;
					}

					Tile tile = tiles[tileId];

					graphics.DrawImage(tile.Bitmap,
									   (overlayTile.X + x - scrollPosition.X) * 8,
									   (overlayTile.Y + y - scrollPosition.Y) * 8);
				}
			}
		}

		private void DrawStartData(Graphics graphics, Point scrollPosition)
		{
			if (this.track is GPTrack)
			{
				this.DrawLapLine(graphics, scrollPosition);
				this.DrawStartPositions(graphics, scrollPosition);
			}
		}

		private void DrawLapLine(Graphics graphics, Point scrollPosition)
		{
			GPTrack gpTrack = this.track as GPTrack;

			Point location = new Point(gpTrack.LapLine.X - scrollPosition.X * 8,
									   gpTrack.LapLine.Y - scrollPosition.Y * 8);

			graphics.DrawLine(this.lapLineOutlinePen, location.X, location.Y,
							  location.X + gpTrack.LapLine.Length, location.Y);

			graphics.DrawLine(this.lapLinePen, location.X + 1, location.Y,
							  location.X + gpTrack.LapLine.Length - 2, location.Y);
		}

		private void DrawStartPositions(Graphics graphics, Point scrollPosition)
		{
			GPTrack gpTrack = this.track as GPTrack;

			int x = gpTrack.StartPosition.X - scrollPosition.X * 8;
			int y = gpTrack.StartPosition.Y - scrollPosition.Y * 8;
			int secondRowOffset = gpTrack.StartPosition.SecondRowOffset;

			Point[] triangle;
			for (int pos = 0; pos <= 18; pos += 6)
			{
				// 1st Column
				triangle = TrackDrawer.GetStartPositionShape(x, y + pos * 8);
				graphics.FillPolygon(this.startPositionBrush, triangle);
				graphics.DrawPolygon(this.startPositionPen, triangle);

				// 2nd Column
				triangle = TrackDrawer.GetStartPositionShape(x + secondRowOffset, y + ((3 + pos) * 8));
				graphics.FillPolygon(this.startPositionBrush, triangle);
				graphics.DrawPolygon(this.startPositionPen, triangle);
			}
		}

		private static Point[] GetStartPositionShape(int x, int y)
		{
			Point[] triangle = new Point[3];
			triangle[0] = new Point(x, y - 4);
			triangle[1] = new Point(x + 4, y + 3);
			triangle[2] = new Point(x - 4, y + 3);
			return triangle;
		}

		private void DrawObjectData(Graphics graphics, Point scrollPosition, bool frontZonesView)
		{
			if (this.track is GPTrack &&
				(this.track as GPTrack).ObjectZones != null)
			{
				this.DrawObjectZones(graphics, scrollPosition, frontZonesView);
				this.DrawObjects(graphics, scrollPosition);
			}
		}

		private void DrawObjectZones(Graphics graphics, Point scrollPosition, bool frontZonesView)
		{
			GPTrack gpTrack = this.track as GPTrack;

			foreach (TrackAIElement aiElem in gpTrack.AI)
			{
				int aiElemIndex = gpTrack.AI.GetElementIndex(aiElem);
				int zoneIndex = gpTrack.ObjectZones.GetZoneIndex(frontZonesView, aiElemIndex);

				if (aiElem.ZoneShape == Shape.Rectangle)
				{
					Rectangle zone = TrackDrawer.GetObjectZoneRectanglePart(aiElem, scrollPosition);
					graphics.FillRectangle(this.objectZoneBrushes[zoneIndex], zone);
				}
				else
				{
					Point[] points = TrackDrawer.GetObjectZoneTrianglePart(aiElem, scrollPosition);
					graphics.FillPolygon(this.objectZoneBrushes[zoneIndex], points);
				}
			}
		}

		private void DrawObjects(Graphics graphics, Point scrollPosition)
		{
			GPTrack gpTrack = this.track as GPTrack;

			// 2P Match Race objects (Chain Chomps or Bananas)
			for (int i = gpTrack.Objects.Count - 1; i >= 16; i--)
			{
				TrackObjectMatchRace trackObject = gpTrack.Objects[i] as TrackObjectMatchRace;
				int x = (trackObject.X - scrollPosition.X) * 8;
				int y = (trackObject.Y - scrollPosition.Y) * 8;
				Rectangle trackObjectRect = new Rectangle(x, y, 7, 7);
				graphics.DrawEllipse(this.objectOutlinePen, trackObjectRect);
				graphics.FillEllipse(this.objectBrushes[4], trackObjectRect);

				if (trackObject.Direction == Direction.Horizontal)
				{
					graphics.DrawLine(this.objectMatchRacePen, trackObjectRect.X + 2, trackObjectRect.Y + 2, trackObjectRect.X + 2, trackObjectRect.Y + 5);
					graphics.DrawLine(this.objectMatchRacePen, trackObjectRect.X + 2, trackObjectRect.Y + 3, trackObjectRect.X + 5, trackObjectRect.Y + 3);
					graphics.DrawLine(this.objectMatchRacePen, trackObjectRect.X + 5, trackObjectRect.Y + 2, trackObjectRect.X + 5, trackObjectRect.Y + 5);
				}
				else if (trackObject.Direction == Direction.Vertical)
				{
					graphics.DrawLine(this.objectMatchRacePen, trackObjectRect.X + 2, trackObjectRect.Y + 2, trackObjectRect.X + 3, trackObjectRect.Y + 5);
					graphics.DrawLine(this.objectMatchRacePen, trackObjectRect.X + 4, trackObjectRect.Y + 5, trackObjectRect.X + 5, trackObjectRect.Y + 2);
				}
			}

			// Regular objects (Pipes, Piranha Plants...)
			for (int i = 15; i >= 0; i--)
			{
				TrackObject trackObject = gpTrack.Objects[i];
				int x = (trackObject.X - scrollPosition.X) * 8;
				int y = (trackObject.Y - scrollPosition.Y) * 8;
				Rectangle trackObjectRect = new Rectangle(x, y, 7, 7);
				graphics.DrawEllipse(this.objectOutlinePen, trackObjectRect);
				graphics.FillEllipse(this.objectBrushes[i / 4], trackObjectRect);
			}
		}

		private static Rectangle GetObjectZoneRectanglePart(TrackAIElement aiElem, Point scrollPosition)
		{
			int zoneX = (aiElem.Zone.X - scrollPosition.X) * 8;
			int zoneY = (aiElem.Zone.Y - scrollPosition.Y) * 8;
			int zoneWidth = aiElem.Zone.Width * 8;
			int zoneHeight = aiElem.Zone.Height * 8;

			return new Rectangle(zoneX, zoneY, zoneWidth, zoneHeight);
		}

		private static Point[] GetObjectZoneTrianglePart(TrackAIElement aiElem, Point scrollPosition)
		{
			Point[] points = aiElem.GetTriangle();

			for (int i = 0; i < points.Length; i++)
			{
				points[i] =
					new Point((points[i].X - scrollPosition.X) * 8,
							  (points[i].Y - scrollPosition.Y) * 8);
			}

			return points;
		}

		private void DrawAI(Graphics graphics, Point scrollPosition)
		{
			foreach (TrackAIElement aiElem in this.track.AI)
			{
				int pointX = (aiElem.Target.X - scrollPosition.X) * 8;
				int pointY = (aiElem.Target.Y - scrollPosition.Y) * 8;
				graphics.DrawEllipse(this.objectOutlinePen, pointX + 1, pointY + 1, 5, 5);

				Rectangle zone = TrackDrawer.GetAIZoneRectangle(aiElem, scrollPosition);

				Point target = new Point(pointX + 4, pointY + 4);

				int speed = aiElem.Speed;

				if (aiElem.ZoneShape == Shape.Rectangle)
				{
					this.PaintTopSide(graphics, zone, target, speed);
					this.PaintRightSide(graphics, zone, target, speed);
					this.PaintBottomSide(graphics, zone, target, speed);
					this.PaintLeftSide(graphics, zone, target, speed);

					graphics.DrawRectangle(this.aiZonePens[speed], zone);
				}
				else
				{
					Point[] points = TrackDrawer.GetAIZoneTriangle(aiElem, scrollPosition);

					graphics.DrawPolygon(this.aiZonePens[aiElem.Speed], points);

					switch (aiElem.ZoneShape)
					{
						case Shape.TriangleTopLeft:
							this.PaintTopSide(graphics, zone, target, speed);
							this.PaintLeftSide(graphics, zone, target, speed);
							TrackDrawer.PaintTriangleDiagonalSide(graphics, points, target, this.aiZoneBrushes[speed][2]);
							break;

						case Shape.TriangleTopRight:
							this.PaintTopSide(graphics, zone, target, speed);
							this.PaintRightSide(graphics, zone, target, speed);
							TrackDrawer.PaintTriangleDiagonalSide(graphics, points, target, this.aiZoneBrushes[speed][2]);
							break;

						case Shape.TriangleBottomRight:
							this.PaintBottomSide(graphics, zone, target, speed);
							this.PaintRightSide(graphics, zone, target, speed);
							TrackDrawer.PaintTriangleDiagonalSide(graphics, points, target, this.aiZoneBrushes[speed][0]);
							break;

						case Shape.TriangleBottomLeft:
							this.PaintBottomSide(graphics, zone, target, speed);
							this.PaintLeftSide(graphics, zone, target, speed);
							TrackDrawer.PaintTriangleDiagonalSide(graphics, points, target, this.aiZoneBrushes[speed][0]);
							break;
					}
				}
			}
		}

		private void PaintTopSide(Graphics graphics, Rectangle zone, Point target, int speed)
		{
			if (target.Y > zone.Top)
			{
				Point[] side =
				{
					new Point(zone.Left, zone.Top),
					new Point(zone.Right, zone.Top),
					target
				};

				graphics.FillPolygon(this.aiZoneBrushes[speed][0], side);
			}
		}

		private void PaintRightSide(Graphics graphics, Rectangle zone, Point target, int speed)
		{
			if (target.X < zone.Right)
			{
				Point[] side =
				{
					new Point(zone.Right, zone.Top),
					new Point(zone.Right, zone.Bottom),
					target
				};

				graphics.FillPolygon(this.aiZoneBrushes[speed][1], side);
			}
		}

		private void PaintBottomSide(Graphics graphics, Rectangle zone, Point target, int speed)
		{
			if (target.Y < zone.Bottom)
			{
				Point[] side =
				{
					new Point(zone.Right, zone.Bottom),
					new Point(zone.Left, zone.Bottom),
					target
				};

				graphics.FillPolygon(this.aiZoneBrushes[speed][2], side);
			}
		}

		private void PaintLeftSide(Graphics graphics, Rectangle zone, Point target, int speed)
		{
			if (target.X > zone.Left)
			{
				Point[] side =
				{
					new Point(zone.Left, zone.Bottom),
					new Point(zone.Left, zone.Top),
					target
				};

				graphics.FillPolygon(this.aiZoneBrushes[speed][1], side);
			}
		}

		private static void PaintTriangleDiagonalSide(Graphics graphics, Point[] points, Point target, SolidBrush brush)
		{
			points[points.Length - 2] = target;
			graphics.FillPolygon(brush, points);
		}

		private void HighlightHoveredAIElement(Graphics graphics, Point scrollPosition, TrackAIElement hoveredAIElem, bool isAITargetHovered)
		{
			if (hoveredAIElem == null)
			{
				return;
			}

			Rectangle zone = TrackDrawer.GetAIZoneRectangle(hoveredAIElem, scrollPosition);

			if (isAITargetHovered)
			{
				TrackDrawer.DrawAITargetLines(graphics, scrollPosition, hoveredAIElem, zone, this.aiElementHighlightPen);
			}
			else
			{
				if (hoveredAIElem.ZoneShape == Shape.Rectangle)
				{
					graphics.DrawRectangle(this.aiElementHighlightPen, zone);
				}
				else
				{
					Point[] points = TrackDrawer.GetAIZoneTriangle(hoveredAIElem, scrollPosition);

					graphics.DrawLines(this.aiElementHighlightPen, points);
				}
			}
		}

		private void HighlightSelectedAIElement(Graphics graphics, Point scrollPosition, TrackAIElement selectedAIElem)
		{
			if (selectedAIElem == null)
			{
				return;
			}

			Rectangle zone = TrackDrawer.GetAIZoneRectangle(selectedAIElem, scrollPosition);

			TrackDrawer.DrawAITargetLines(graphics, scrollPosition, selectedAIElem, zone, this.aiElementSelectPen);

			if (selectedAIElem.ZoneShape == Shape.Rectangle)
			{
				graphics.DrawRectangle(this.aiElementSelectPen, zone);
				graphics.FillRectangle(this.aiZoneBrushes[selectedAIElem.Speed][0], zone);
			}
			else
			{
				Point[] points = TrackDrawer.GetAIZoneTriangle(selectedAIElem, scrollPosition);

				graphics.DrawPolygon(this.aiElementSelectPen, points);
				graphics.FillPolygon(this.aiZoneBrushes[selectedAIElem.Speed][0], points);
			}
		}

		private static Rectangle GetAIZoneRectangle(TrackAIElement aiElem, Point scrollPosition)
		{
			int zoneX = (aiElem.Zone.X - scrollPosition.X) * 8;
			int zoneY = (aiElem.Zone.Y - scrollPosition.Y) * 8;
			int zoneWidth = aiElem.Zone.Width * 8;
			int zoneHeight = aiElem.Zone.Height * 8;

			return new Rectangle(zoneX, zoneY, zoneWidth - 1, zoneHeight - 1);
		}

		private static Point[] GetAIZoneTriangle(TrackAIElement aiElem, Point scrollPosition)
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
					new Point((points[i].X - scrollPosition.X) * 8 - xCorrection,
							  (points[i].Y - scrollPosition.Y) * 8 - yCorrection);
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

		private static void DrawAITargetLines(Graphics graphics, Point scrollPosition, TrackAIElement aiElem, Rectangle zone, Pen pen)
		{
			int pointX = (aiElem.Target.X - scrollPosition.X) * 8;
			int pointY = (aiElem.Target.Y - scrollPosition.Y) * 8;

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
						graphics.DrawLines(pen, targetLines);
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
						graphics.DrawLines(pen, targetLines);
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
						graphics.DrawLines(pen, targetLines);
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
						graphics.DrawLines(pen, targetLines);
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
						graphics.DrawLines(pen, targetLines);
						break;
					}
			}
		}

		private void PaintTrackOutbounds(int imageWidth, int imageHeight, Size panelSize)
		{
			Rectangle trackArea = new Rectangle(0, 0, (int)(imageWidth * this.zoom), (int)(imageHeight * this.zoom));
			Rectangle[] outBounds = new Rectangle[]
			{
				// Right outbounds
				new Rectangle(trackArea.Right, 0, panelSize.Width - trackArea.Width, trackArea.Height),
				
				// Bottom outbounds
				new Rectangle(0, trackArea.Bottom, panelSize.Width, panelSize.Height - trackArea.Height)
			};
			this.trackGfx.FillRectangles(Brushes.Black, outBounds);
		}

		public void NotifyFullRepaintNeed()
		{
			this.fullRepaintNeeded = true;
		}

		public void ResizeWindow(Control ctrl)
		{
			if (ctrl.Width > 0 && ctrl.Height > 0)
			{
				this.trackGfx = ctrl.CreateGraphics();
				this.SetInterpolationMode();
				this.NotifyFullRepaintNeed();
			}
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
			using (Graphics gfx = Graphics.FromImage(this.tileClipboardCache))
			{
				for (int y = 0; y < clipboardSize.Height; y++)
				{
					for (int x = 0; x < clipboardSize.Width; x++)
					{
						Tile tile = tileset[tiles[x + y * clipboardSize.Width]];
						gfx.DrawImage(tile.Bitmap, x * 8, y * 8);
					}
				}
			}
		}

		public void Dispose()
		{
			this.trackCache.Dispose();
			this.tileClipboardCache.Dispose();

			this.trackGfx.Dispose();

			this.tileHighlightPen.Dispose();
			this.tileSelectPen.Dispose();
			this.tileSelectBrush.Dispose();

			this.overlayHighlightBrush.Dispose();

			this.lapLinePen.Dispose();
			this.lapLineOutlinePen.Dispose();
			this.startPositionBrush.Dispose();
			this.startPositionPen.Dispose();

			foreach (SolidBrush objectZoneBrush in this.objectZoneBrushes)
			{
				objectZoneBrush.Dispose();
			}

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

			GC.SuppressFinalize(this);
		}
	}
}
