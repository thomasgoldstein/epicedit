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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using EpicEdit.UI.TrackEdition;

namespace EpicEdit.UI.ThemeEdition
{
    internal sealed class BackgroundPanel : TilePanel
    {
        /// <summary>
        /// Raised when a tile has been modified.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> TileChanged;

        /// <summary>
        /// Raised when a tile has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs<byte, Tile2bppProperties>> TileSelected;

        [Browsable(false), DefaultValue(typeof(BackgroundDrawer), "")]
        public BackgroundDrawer Drawer { get; set; }

        [Browsable(false), DefaultValue(typeof(Background), "")]
        public Background Background { get; set; }

        public bool Front { get; set; }

        public Point TilePosition { get; private set; }

        public int ScrollPixelPositionX
        {
            get { return (int)(this.AutoScrollPosition.X / this.Zoom); }
        }

        public int ScrollTilePositionX
        {
            get { return this.ScrollPixelPositionX / Tile.Size; }
        }

        public Point AbsoluteTilePosition
        {
            get
            {
                if (this.TilePosition == TrackEditor.OutOfBounds)
                {
                    return this.TilePosition;
                }

                return new Point(this.TilePosition.X - this.ScrollTilePositionX, this.TilePosition.Y);
            }
        }

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte TileId { get; set; }

        private byte tileProperties;

        /// <summary>
        /// Specifies whether the user is selecting a tile.
        /// </summary>
        private bool tileSelection;

        public Tile2bppProperties TileProperties
        {
            get { return new Tile2bppProperties(this.tileProperties); }
            set { this.tileProperties = value.GetByte(); }
        }

        public BackgroundPanel()
        {
            this.TilePosition = TrackEditor.OutOfBounds;
            this.HorizontalScroll.SmallChange = Tile.Size * BackgroundDrawer.Zoom;
            this.MouseMove += this.BackgroundPanel_MouseMove;
            this.MouseLeave += this.BackgroundPanel_MouseLeave;
            this.MouseDown += this.BackgroundPanel_MouseDown;
            this.MouseUp += this.BackgroundPanel_MouseUp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Drawer == null)
            {
                return;
            }

            this.Drawer.DrawBackgroundLayer(e.Graphics, this.TilePosition, this.ScrollPixelPositionX, this.Front, this.tileSelection);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            this.Invalidate();
        }

        private void BackgroundPanel_MouseMove(object sender, MouseEventArgs e)
        {
            Point tilePositionBefore = this.TilePosition;
            this.SetPosition(e.Location);

            if (tilePositionBefore != this.TilePosition)
            {
                this.InitAction(e.Button);
                this.Invalidate();
            }
        }

        private void BackgroundPanel_MouseLeave(object sender, EventArgs e)
        {
            this.TilePosition = TrackEditor.OutOfBounds;
            this.Invalidate();
        }

        private void BackgroundPanel_MouseDown(object sender, MouseEventArgs e)
        {
            this.InitAction(e.Button);
        }

        private void BackgroundPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.tileSelection)
            {
                this.tileSelection = false;
                this.Invalidate();
            }
        }

        private void SetPosition(Point location)
        {
            int zoomedTileSize = (int)(Tile.Size * this.Zoom);
            int x = (location.X - (this.AutoScrollPosition.X % zoomedTileSize)) / zoomedTileSize;
            int y = location.Y / zoomedTileSize;

            // We check that the new position isn't out of the track limits, if it is,
            // we set it to the lowest or highest (depending on case) possible coordinate
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= this.Width / zoomedTileSize)
            {
                x = this.Width / zoomedTileSize - 1;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y >= this.AutoScrollMinSize.Height / zoomedTileSize)
            {
                // Using AutoScrollMinSize.Height rather than Height,
                // because Height includes the horizontal scroll bar height
                y = this.AutoScrollMinSize.Height / zoomedTileSize - 1;
            }

            this.TilePosition = new Point(x, y);
        }

        private void InitAction(MouseButtons mouseButton)
        {
            if (this.TilePosition == TrackEditor.OutOfBounds)
            {
                return;
            }

            if (mouseButton == MouseButtons.Left)
            {
                this.LayTile();
                this.Invalidate();
            }
            else if (mouseButton == MouseButtons.Right)
            {
                this.SelectTile();
                this.Invalidate();
            }
        }

        private void LayTile()
        {
            Point position = this.AbsoluteTilePosition;
            this.Background.Layout.SetTileData(position.X, position.Y, this.Front, this.TileId, this.tileProperties);
            this.Drawer.UpdateTile(position.X, position.Y, this.Front, this.TileId, this.tileProperties);

            this.TileChanged(this, EventArgs.Empty);
        }

        private void SelectTile()
        {
            this.tileSelection = true;

            Point position = this.AbsoluteTilePosition;
            byte tileId;
            byte properties;
            this.Background.Layout.GetTileData(position.X, position.Y, this.Front, out tileId, out properties);

            EventArgs<byte, Tile2bppProperties> ea = new EventArgs<byte, Tile2bppProperties>(tileId, new Tile2bppProperties(properties));
            this.TileSelected(this, ea);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            // NOTE: We're leaking a bit of memory here, as the instance is not explicitly disposed
            return this.Background.GetTileInstance(x, y, this.Front);
        }
    }
}
