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
	public class OverlayTilesetDrawer : IDisposable
	{
		private OverlayTilePattern[] patterns;
		private Tile[] tileset;

		private Graphics overlayGfx;
		private Bitmap overlayCache;
		private HatchBrush transparentBrush;

		public OverlayTilesetDrawer(Control control)
		{
			this.InitPatternArray();
			this.SetControlHeight(control);

			this.overlayGfx = control.CreateGraphics();
			this.overlayGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.overlayGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			this.transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);

			// The following member is initialized so it can be disposed of
			// in each function without having to check if it's null beforehand
			this.overlayCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}

		/// <summary>
		/// Loads the overlay tile patterns of the game, skipping duplicate patterns.
		/// </summary>
		private void InitPatternArray()
		{
			OverlayTilePattern previousPattern = null;
			List<OverlayTilePattern> patternList = new List<OverlayTilePattern>();
			foreach (OverlayTilePattern pattern in MainForm.SmkGame.GetOverlayTilePatterns())
			{
				if (pattern.Equals(previousPattern))
				{
					// Skip duplicate patterns
					continue;
				}

				previousPattern = pattern;
				patternList.Add(pattern);
			}

			this.patterns = patternList.ToArray();
		}

		/// <summary>
		/// Sets the height of the control and its parent depending on the tileset height.
		/// </summary>
		/// <param name="control">The control the tileset is painted on.</param>
		private void SetControlHeight(Control control)
		{
			int tilesetHeight = this.GetTilesetHeight(control.Width);
			int difference = tilesetHeight - control.Height;
			control.Height = tilesetHeight;
			control.Parent.Height += difference;
		}

		private int GetTilesetHeight(int panelWidth)
		{
			int zoom = 2;
			int tilesetX = 0;
			int tilesetY = 0;

			int tallestPattern = 0; // The tallest tile pattern in a given row

			foreach (OverlayTilePattern pattern in this.patterns)
			{
				if ((tilesetX + pattern.Width * 8) * zoom > panelWidth)
				{
					tilesetX = 0;
					tilesetY += tallestPattern;
					tallestPattern = 0;
				}

				tilesetX += pattern.Width * 8;
				if (pattern.Height > tallestPattern)
				{
					tallestPattern = pattern.Height;
				}
			}

			if (tilesetX != 0)
			{
				tilesetY += tallestPattern;
			}

			return tilesetY * 8 * zoom;
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

			int zoom = 2;
			int tilesetY = 0; // Current vertical drawing position in the tileset

			RectangleF bounds = this.overlayGfx.VisibleClipBounds;
			int panelWidth = (int)bounds.Width;
			int panelHeight = (int)bounds.Height;

			this.overlayCache = new Bitmap(panelWidth, panelHeight, PixelFormat.Format32bppPArgb);
			using (Graphics gfx = Graphics.FromImage(this.overlayCache))
			{
				gfx.FillRegion(this.transparentBrush, this.overlayGfx.Clip);

				panelWidth /= (8 * zoom); // Take tile width and zoom in consideration
				int patternId = 0;
				int patternCountInRow = -1;

				while (patternCountInRow != 0)
				{
					patternCountInRow = 0;
					int rowWidth = 0;

					// Compute how many patterns will fit in the row
					for (int otherPatternId = patternId; otherPatternId < this.patterns.Length; otherPatternId++)
					{
						OverlayTilePattern pattern = this.patterns[otherPatternId];
						int newRowWidth = rowWidth + pattern.Width;

						if (newRowWidth > panelWidth)
						{
							break;
						}

						rowWidth = newRowWidth;
						patternCountInRow++;
					}

					int patternRowIterator = 0;
					int tallestPattern = 0; // The tallest tile pattern in a given row
					int tilesetX; // Current horizontal drawing position in the tileset
					if (rowWidth == panelWidth)
					{
						tilesetX = 0;
					}
					else
					{
						// If the row isn't totally filled, center the pattern(s)
						tilesetX = ((panelWidth - rowWidth) * 8) / 2;
					}

					// Draw the pattern(s) of the row
					while (patternRowIterator < patternCountInRow)
					{
						OverlayTilePattern pattern = this.patterns[patternId];

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
											  8 * x + tilesetX,
											  8 * y + tilesetY,
											  8, 8);
							}
						}

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
			}
		}

		public void Dispose()
		{
			this.overlayGfx.Dispose();
			this.overlayCache.Dispose();
			this.transparentBrush.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
