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
		private Control control;
		public const int Zoom = 2;
		public Dictionary<OverlayTilePattern, Point> PatternList { get; set; }
		private Tile[] tileset;

		public OverlayTilePattern HoveredPattern { get; set; }
		public OverlayTilePattern SelectedPattern { get; set; }

		private Bitmap overlayCache;
		private HatchBrush transparentBrush;

		private Pen delimitPen;
		private Pen highlightPen;
		private SolidBrush selectBrush;

		public OverlayTilesetDrawer(Control control)
		{
			this.control = control;

			this.transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);

			this.delimitPen = new Pen(Color.FromArgb(150, 60, 100, 255));
			this.highlightPen = new Pen(Color.FromArgb(200, 255, 255, 255));
			this.selectBrush = new SolidBrush(Color.FromArgb(80, 255, 255, 255));

			// The following member is initialized so it can be disposed of
			// in each function without having to check if it's null beforehand
			this.overlayCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}

		public void DrawOverlayTileset(Graphics graphics)
		{
			using (Bitmap image = new Bitmap(this.overlayCache.Width, this.overlayCache.Height, PixelFormat.Format32bppPArgb))
			using (Graphics gfx = Graphics.FromImage(image))
			{
				graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
				graphics.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

				gfx.DrawImage(this.overlayCache, 0, 0,
							  this.overlayCache.Width,
							  this.overlayCache.Height);

				this.OutlinePattern(gfx, this.HoveredPattern);

				if (this.HoveredPattern != this.SelectedPattern)
				{
					this.OutlinePattern(gfx, this.SelectedPattern);
				}

				this.HighlightPattern(gfx, this.SelectedPattern);

				graphics.DrawImage(image, 0, 0,
								   image.Width * Zoom,
								   image.Height * Zoom);
			}
		}

		private void OutlinePattern(Graphics graphics, OverlayTilePattern pattern)
		{
			if (pattern != null)
			{
				Point location;
				this.PatternList.TryGetValue(pattern, out location);
				graphics.DrawRectangle(this.highlightPen,
									   location.X, location.Y,
									   pattern.Width * 8 - 1,
									   pattern.Height * 8 - 1);
			}
		}

		private void HighlightPattern(Graphics graphics, OverlayTilePattern pattern)
		{
			if (pattern != null)
			{
				Point location;
				this.PatternList.TryGetValue(pattern, out location);
				graphics.FillRectangle(this.selectBrush,
									   location.X, location.Y,
									   pattern.Width * 8 - 1,
									   pattern.Height * 8 - 1);
			}
		}

		public void SetTileset(Tile[] tileset)
		{
			this.tileset = tileset;
			this.UpdateCache();
		}

		private void UpdateCache()
		{
			this.overlayCache.Dispose();

			int panelWidth = (int)this.control.Width / Zoom;
			int panelHeight = (int)this.control.Height / Zoom;

			this.overlayCache = new Bitmap(panelWidth, panelHeight, PixelFormat.Format32bppPArgb);
			using (Graphics gfx = Graphics.FromImage(this.overlayCache))
			{
				gfx.FillRegion(this.transparentBrush, gfx.Clip);

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
			this.overlayCache.Dispose();
			this.transparentBrush.Dispose();

			this.delimitPen.Dispose();
			this.highlightPen.Dispose();
			this.selectBrush.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
