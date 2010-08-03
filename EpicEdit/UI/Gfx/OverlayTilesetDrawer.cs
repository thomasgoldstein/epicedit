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
		private HatchBrush transparentBrush;

		public OverlayTilesetDrawer(Control control)
		{
			this.overlayGfx = control.CreateGraphics();
			this.overlayGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.overlayGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			this.transparentBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.DarkGray, Color.White);

			this.InitPatternArray();
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

		public void DrawOverlayTileset()
		{
			int zoom = 2;
			int tilesetX = 0;
			int tilesetY = 0;
			int panelWidth = (int)this.overlayGfx.VisibleClipBounds.Width;
			int tallestPattern = 0; // The tallest tile pattern in a given row

			this.overlayGfx.FillRegion(this.transparentBrush, this.overlayGfx.Clip);

			foreach (OverlayTilePattern pattern in this.patterns)
			{
				if (tilesetX + pattern.Width * 8 * zoom > panelWidth)
				{
					tilesetX = 0;
					tilesetY += tallestPattern * 8 * zoom;
					tallestPattern = 0;
				}

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
						this.overlayGfx.DrawImage(tile.Bitmap,
						                          8 * (x * zoom) + tilesetX,
						                          8 * (y * zoom) + tilesetY,
												  8 * zoom,
												  8 * zoom);
					}
				}

				tilesetX += pattern.Width * 8 * zoom;
				if (pattern.Height > tallestPattern)
				{
					tallestPattern = pattern.Height;
				}
			}
		}

		public void SetTileset(Tile[] tileset)
		{
			this.tileset = tileset;
			this.DrawOverlayTileset();
		}

		public void Dispose()
		{
			this.overlayGfx.Dispose();
			this.transparentBrush.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
