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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using EpicEdit.UI.TrackEdition;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Panel that displays a track background.
    /// </summary>
    internal sealed class BackgroundPanel : TilePanel
    {
        /// <summary>
        /// Raised when a tile has been modified.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> TileChanged;

        /// <summary>
        /// Raised when a tile has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<byte, Tile2bppProperties>> TileSelected;

        [Browsable(false), DefaultValue(typeof(BackgroundDrawer), "")]
        public BackgroundDrawer Drawer { get; set; }

        [Browsable(false), DefaultValue(typeof(Background), "")]
        public Background Background { get; set; }

        public bool Front { get; set; }

        public Point TilePosition { get; private set; }

        public int ScrollPixelPositionX => (int)(AutoScrollPosition.X / Zoom);

        public int ScrollTilePositionX => ScrollPixelPositionX / Tile.Size;

        public Point AbsoluteTilePosition
        {
            get
            {
                if (TilePosition == TrackEditor.OutOfBounds)
                {
                    return TilePosition;
                }

                return new Point(TilePosition.X - ScrollTilePositionX, TilePosition.Y);
            }
        }

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte TileId { get; set; }

        private byte _tileProperties;

        /// <summary>
        /// Specifies whether the user is selecting a tile.
        /// </summary>
        private bool _tileSelection;

        public Tile2bppProperties TileProperties
        {
            get => new Tile2bppProperties(_tileProperties);
            set => _tileProperties = value.GetByte();
        }

        public BackgroundPanel()
        {
            TilePosition = TrackEditor.OutOfBounds;
            HorizontalScroll.SmallChange = Tile.Size * BackgroundDrawer.Zoom;
            MouseMove += BackgroundPanel_MouseMove;
            MouseLeave += BackgroundPanel_MouseLeave;
            MouseDown += BackgroundPanel_MouseDown;
            MouseUp += BackgroundPanel_MouseUp;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Drawer == null)
            {
                return;
            }

            Drawer.DrawBackgroundLayer(e.Graphics, TilePosition, ScrollPixelPositionX, Front, _tileSelection);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            Invalidate();
        }

        private void BackgroundPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (Context.ColorPickerMode)
            {
                return;
            }

            Cursor = EpicCursors.PencilCursor;

            Point tilePositionBefore = TilePosition;
            SetPosition(e.Location);

            if (tilePositionBefore != TilePosition)
            {
                InitAction(e.Button);
                Invalidate();
            }
        }

        private void BackgroundPanel_MouseLeave(object sender, EventArgs e)
        {
            TilePosition = TrackEditor.OutOfBounds;
            Invalidate();
        }

        private void BackgroundPanel_MouseDown(object sender, MouseEventArgs e)
        {
            InitAction(e.Button);
        }

        private void BackgroundPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_tileSelection)
            {
                _tileSelection = false;
                Invalidate();
            }
        }

        private void SetPosition(Point location)
        {
            int zoomedTileSize = (int)(Tile.Size * Zoom);
            int x = (location.X - (AutoScrollPosition.X % zoomedTileSize)) / zoomedTileSize;
            int y = location.Y / zoomedTileSize;

            // We check that the new position isn't out of the track limits, if it is,
            // we set it to the lowest or highest (depending on case) possible coordinate
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= Width / zoomedTileSize)
            {
                x = Width / zoomedTileSize - 1;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y >= AutoScrollMinSize.Height / zoomedTileSize)
            {
                // Using AutoScrollMinSize.Height rather than Height,
                // because Height includes the horizontal scroll bar height
                y = AutoScrollMinSize.Height / zoomedTileSize - 1;
            }

            TilePosition = new Point(x, y);
        }

        private void InitAction(MouseButtons mouseButton)
        {
            if (TilePosition == TrackEditor.OutOfBounds)
            {
                return;
            }

            if (mouseButton == MouseButtons.Left)
            {
                LayTile();
                Invalidate();
            }
            else if (mouseButton == MouseButtons.Right)
            {
                SelectTile();
                Invalidate();
            }
        }

        private void LayTile()
        {
            Point position = AbsoluteTilePosition;
            Background.Layout.SetTileData(position.X, position.Y, Front, TileId, _tileProperties);
            Drawer.UpdateTile(position.X, position.Y, Front, TileId, _tileProperties);

            TileChanged(this, EventArgs.Empty);
        }

        private void SelectTile()
        {
            _tileSelection = true;

            Point position = AbsoluteTilePosition;
            Background.Layout.GetTileData(position.X, position.Y, Front, out byte tileId, out byte properties);

            EventArgs<byte, Tile2bppProperties> ea = new EventArgs<byte, Tile2bppProperties>(tileId, new Tile2bppProperties(properties));
            TileSelected(this, ea);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            // NOTE: We're leaking a bit of memory here, as the instance is not explicitly disposed
            return Background.GetTileInstance(x, y, Front);
        }
    }
}
