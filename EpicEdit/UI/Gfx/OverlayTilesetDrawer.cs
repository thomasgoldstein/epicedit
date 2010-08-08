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
		private Dictionary<OverlayTilePattern, Point> patternList;
		private Tile[] tileset;

		private Graphics overlayGfx;
		private Bitmap overlayCache;
		private HatchBrush transparentBrush;
		private Pen delimitPen;

		public OverlayTilesetDrawer(Control control)
		{
			this.Initialize(control);

			this.overlayGfx = control.CreateGraphics();
			this.overlayGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.overlayGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			this.transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);
			this.delimitPen = new Pen(Color.FromArgb(150, 60, 100, 255));

			// The following member is initialized so it can be disposed of
			// in each function without having to check if it's null beforehand
			this.overlayCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}

		/// <param name="control">The control the tileset is painted on.</param>
		private void Initialize(Control control)
		{
			int tilesetHeight = this.LoadPatternDictionary(control.Width);
			this.SetControlHeight(control, tilesetHeight);
		}

		/// <summary>
		/// Loads the dictionary of patterns, and their location.
		/// </summary>
		/// <param name="panelWidth">The width of the tileset Panel.</param>
		/// <returns>The height of the tileset.</returns>
		private int LoadPatternDictionary(int panelWidth)
		{
			this.patternList = new Dictionary<OverlayTilePattern, Point>();
			List<OverlayTilePattern> patterns = this.GetUniquePatterns();

			int zoom = 2;
			int tilesetX = 0; // Current horizontal drawing position in the tileset
			int tilesetY = 0; // Current vertical drawing position in the tileset
			int tallestPattern = 0; // The tallest tile pattern in a given row

			panelWidth /= (8 * zoom); // Take tile width and zoom in consideration
			int patternId = 0;
			int patternCountInRow = -1;

			while (patternCountInRow != 0)
			{
				patternCountInRow = 0;
				int rowWidth = 0;

				// Compute how many patterns will fit in the row
				for (int otherPatternId = patternId; otherPatternId < patterns.Count; otherPatternId++)
				{
					OverlayTilePattern pattern = patterns[otherPatternId];
					int newRowWidth = rowWidth + pattern.Width;

					if (newRowWidth > panelWidth)
					{
						break;
					}

					rowWidth = newRowWidth;
					patternCountInRow++;
				}

				int patternRowIterator = 0;
				tallestPattern = 0;
				if (rowWidth == panelWidth)
				{
					tilesetX = 0;
				}
				else
				{
					// If the row isn't totally filled, center the pattern(s)
					tilesetX = ((panelWidth - rowWidth) * 8) / 2;
				}

				// Store the pattern(s) of the row, and their location
				while (patternRowIterator < patternCountInRow)
				{
					OverlayTilePattern pattern = patterns[patternId];
					this.patternList.Add(pattern, new Point(tilesetX, tilesetY));

					tilesetX += pattern.Width * 8;
					if (pattern.Height > tallestPattern)
					{
						tallestPattern = pattern.Height;
					}

					patternRowIterator++;
					patternId++;
				}

				tilesetY += tallestPattern * 8;
			}

			if (tilesetX != 0)
			{
				tilesetY += tallestPattern;
			}

			return tilesetY * zoom;
		}

		/// <summary>
		/// Gets the overlay tile patterns of the game, skipping duplicate patterns.
		/// </summary>
		private List<OverlayTilePattern> GetUniquePatterns()
		{
			OverlayTilePattern previousPattern = null;
			List<OverlayTilePattern> patterns = new List<OverlayTilePattern>();
			foreach (OverlayTilePattern pattern in MainForm.SmkGame.OverlayTilePatterns)
			{
				if (pattern.Equals(previousPattern))
				{
					// Skip duplicate patterns
					continue;
				}

				previousPattern = pattern;
				patterns.Add(pattern);
			}

			return patterns;
		}

		/// <summary>
		/// Sets the height of the control and its parent depending on the tileset height.
		/// </summary>
		/// <param name="control">The control the tileset is painted on.</param>
		private void SetControlHeight(Control control, int tilesetHeight)
		{
			int difference = tilesetHeight - control.Height;
			control.Height = tilesetHeight;
			control.Parent.Height += difference;
		}

		public void DrawOverlayTileset()
		{
			int zoom = 2;

			this.overlayGfx.DrawImage(this.overlayCache, 0, 0,
									  this.overlayCache.Width * zoom,
									  this.overlayCache.Height * zoom);
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

				foreach (KeyValuePair<OverlayTilePattern, Point> kvp in this.patternList)
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

			GC.SuppressFinalize(this);
		}
	}
}
