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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Gfx
{
	/// <summary>
	/// Provides the ability to paint the graphics of a tileset.
	/// </summary>
	public sealed class TilesetDrawer : IDisposable
	{
		private Tile[] tileset;

		private Graphics tilesetGfx;
		private Bitmap tilesetCache;
		private Pen tilesetPen;

		public TilesetDrawer(Control control)
		{
			this.tilesetGfx = control.CreateGraphics();
			this.tilesetGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.tilesetGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			this.tilesetPen = new Pen(Color.FromArgb(150, 255, 0, 0));

			// The following member is initialized so it can be disposed of
			// in each function without having to check if it's null beforehand
			this.tilesetCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
		}

		public void SetTileset(Tile[] tileset)
		{
			this.tileset = tileset;

			this.UpdateCache();
		}

		private void UpdateCache()
		{
			this.tilesetCache.Dispose();

			this.tilesetCache = new Bitmap(64, 256, PixelFormat.Format32bppPArgb);
			using (Graphics gfx = Graphics.FromImage(this.tilesetCache))
			{
				for (int x = 0; x < 8; x++)
				{
					for (int y = 0; y < 32; y++)
					{
						Tile tile = this.tileset[x + (y * 8)];
						gfx.DrawImage(tile.Bitmap, x * 8, y * 8);
					}
				}
			}
		}

		public void DrawTileset(byte selectedTile)
		{
			int zoom = 2;

			using (Bitmap image = new Bitmap(this.tilesetCache.Width, this.tilesetCache.Height, PixelFormat.Format32bppPArgb))
			using (Graphics gfx = Graphics.FromImage(image))
			{
				gfx.DrawImage(this.tilesetCache, 0, 0,
							  this.tilesetCache.Width,
							  this.tilesetCache.Height);

				int tilePosX = selectedTile % 8;
				int tilePosY = selectedTile / 8;
				Point selectedTilePosition = new Point(tilePosX, tilePosY);

				gfx.DrawRectangle(this.tilesetPen,
								  selectedTilePosition.X * 8,
								  selectedTilePosition.Y * 8,
								  8 - 1,
								  8 - 1);

				this.tilesetGfx.DrawImage(image, 0, 0,
										  image.Width * zoom,
										  image.Height * zoom);
			}
		}

		public void Dispose()
		{
			this.tilesetGfx.Dispose();
			this.tilesetCache.Dispose();
			this.tilesetPen.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
