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
		private Pen tilesetPen;

		public TilesetDrawer(Control tilesetCtrl)
		{
			this.tilesetGfx = tilesetCtrl.CreateGraphics();
			this.tilesetGfx.InterpolationMode = InterpolationMode.NearestNeighbor;
			this.tilesetGfx.PixelOffsetMode = PixelOffsetMode.Half; // Solves a GDI+ bug which crops scaled images

			this.tilesetPen = new Pen(Color.FromArgb(150, 255, 0, 0), 2);
		}

		public void SetTileset(Tile[] tileset)
		{
			this.tileset = tileset;
		}

		public void DrawTileset(byte selectedTile)
		{
			int zoom = 2;

			using (Bitmap tilesetImage = new Bitmap(64, 256, PixelFormat.Format32bppPArgb))
			{
				using (Graphics gfx = Graphics.FromImage(tilesetImage))
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

				this.tilesetGfx.DrawImage(tilesetImage, 0, 0, tilesetImage.Width * zoom, tilesetImage.Height * zoom);
			}

			int tilePosX = selectedTile % 8;
			int tilePosY = selectedTile / 8;
			Point selectedTilePosition = new Point(tilePosX, tilePosY);

			this.tilesetGfx.DrawRectangle(this.tilesetPen, (selectedTilePosition.X * 8 * zoom), (selectedTilePosition.Y * 8 * zoom), 8 * zoom - 1, 8 * zoom - 1);
		}

		public void Dispose()
		{
			this.tilesetGfx.Dispose();
			this.tilesetPen.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
