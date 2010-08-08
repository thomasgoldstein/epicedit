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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Overlay;

namespace EpicEdit.UI.Gfx
{
	/// <summary>
	/// Provides the ability to paint the graphics of an overlay tileset.
	/// </summary>
	public sealed class OverlayTilesetDrawer : IDisposable
	{
		public const int Zoom = 2;
		public Dictionary<OverlayTilePattern, Point> PatternList { get; set; }

		private Tile[] tileset;

		private Graphics overlayGfx;
		private Bitmap overlayCache;
		private HatchBrush transparentBrush;

		private Pen delimitPen;
		private Pen higlightPen;

		private OverlayTilePattern hoveredPattern;

		public OverlayTilesetDrawer(Control control)
		{
			this.overlayGfx = control.CreateGraphics();
			this.overlayGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.overlayGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			this.transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);

			this.delimitPen = new Pen(Color.FromArgb(150, 60, 100, 255));
			this.higlightPen = new Pen(Color.White);

			// The following member is initialized so it can be disposed of
			// in each function without having to check if it's null beforehand
			this.overlayCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}

		public void DrawOverlayTileset()
		{
			this.overlayGfx.DrawImage(this.overlayCache, 0, 0,
									  this.overlayCache.Width * Zoom,
									  this.overlayCache.Height * Zoom);

			if (this.hoveredPattern != null)
			{
				Point location;
				this.PatternList.TryGetValue(this.hoveredPattern, out location);
				this.overlayGfx.DrawRectangle(this.higlightPen,
											  location.X * Zoom, location.Y * Zoom,
											  this.hoveredPattern.Width * 8 * Zoom,
											  this.hoveredPattern.Height * 8 * Zoom);
			}
		}

		public void HighlightPattern(OverlayTilePattern pattern)
		{
			if (this.hoveredPattern != pattern)
			{
				this.hoveredPattern = pattern;
				this.DrawOverlayTileset();
			}
		}

		public void ResetTileHighlighting()
		{
			if (this.hoveredPattern != null)
			{
				this.hoveredPattern = null;
				this.DrawOverlayTileset();
			}
		}

		public void SetTileset(Tile[] tileset)
		{
			this.tileset = tileset;

			this.UpdateCache();

			this.DrawOverlayTileset();
		}

		private void UpdateCache()
		{
			this.overlayCache.Dispose();

			RectangleF bounds = this.overlayGfx.VisibleClipBounds;
			int panelWidth = (int)bounds.Width;
			int panelHeight = (int)bounds.Height;

			this.overlayCache = new Bitmap(panelWidth, panelHeight, PixelFormat.Format32bppPArgb);
			using (Graphics gfx = Graphics.FromImage(this.overlayCache))
			{
				gfx.FillRegion(this.transparentBrush, this.overlayGfx.Clip);

				foreach (KeyValuePair<OverlayTilePattern, Point> kvp in this.PatternList)
				{
					OverlayTilePattern pattern = kvp.Key;
					Point location = kvp.Value;

					// Draw the pattern
					for (int y = 0; y < pattern.Height; y++)
					{
						for (int x = 0; x < pattern.Width; x++)
						{
							int tileId = pattern.Tiles[y][x];
	
							if (tileId == 0xFF)
							{
								continue;
							}

							Tile tile = this.tileset[tileId];
							gfx.DrawImage(tile.Bitmap,
										  8 * x + location.X,
										  8 * y + location.Y,
										  8, 8);
						}
					}

					// Delimit the pattern
					gfx.DrawRectangle(this.delimitPen,
									  location.X,
									  location.Y,
									  pattern.Width * 8 - 1,
									  pattern.Height * 8 - 1);
				}
			}
		}

		public void Dispose()
		{
			this.overlayGfx.Dispose();
			this.overlayCache.Dispose();
			this.transparentBrush.Dispose();

			this.delimitPen.Dispose();
			this.higlightPen.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
