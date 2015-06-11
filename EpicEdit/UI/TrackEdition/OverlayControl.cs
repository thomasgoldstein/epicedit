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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="OverlayTiles"/>.
    /// </summary>
    internal partial class OverlayControl : UserControl
    {
        #region Events
        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> RepaintRequested;

        [Browsable(true), Category("Data")]
        public event EventHandler<OverlayTileEventArgs> ElementRemoved;

        [Browsable(true), Category("Data")]
        public event EventHandler<EventArgs> ElementsCleared;

        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add { this.tilesetPanel.ColorSelected += value; }
            remove { this.tilesetPanel.ColorSelected -= value; }
        }
        #endregion Events

        private Dictionary<OverlayTilePattern, Point> patternList;

        /// <summary>
        /// Used to draw the overlay tileset.
        /// </summary>
        private OverlayTilesetDrawer drawer;

        /// <summary>
        /// The hovered overlay tile pattern.
        /// </summary>
        private OverlayTilePattern hoveredPattern;

        /// <summary>
        /// The selected overlay tile pattern.
        /// </summary>
        private OverlayTilePattern selectedPattern;

        /// <summary>
        /// Gets or sets the selected overlay tile pattern.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(OverlayTilePattern), "")]
        public OverlayTilePattern SelectedPattern
        {
            get { return this.selectedPattern; }
            set
            {
                if (value != null && !this.patternList.ContainsKey(value))
                {
                    // Since duplicates are removed from the pattern list,
                    // it's possible that we can't find the exact same pattern
                    // referenced by the overlay tile.
                    // In this case, find another pattern with the same properties.

                    foreach (KeyValuePair<OverlayTilePattern, Point> kvp in this.patternList)
                    {
                        OverlayTilePattern pattern = kvp.Key;
                        if (pattern.Equals(value))
                        {
                            value = pattern;
                            break;
                        }
                    }
                }

                this.SelectedPatternInternal = value;
            }
        }

        private OverlayTilePattern SelectedPatternInternal
        {
            get { return this.selectedPattern; }
            set
            {
                if (value != null && this.SelectedTile != null)
                {
                    this.SelectedTile = null;
                    this.RepaintRequested(this, EventArgs.Empty);
                }

                if (this.selectedPattern != value)
                {
                    this.selectedPattern = value;
                    this.drawer.SelectedPattern = value;
                    this.tilesetPanel.Invalidate();
                }
            }
        }

        /// <summary>
        /// The selected track overlay tile.
        /// </summary>
        private OverlayTile selectedTile;

        /// <summary>
        /// Gets or sets the selected track overlay tile.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(OverlayTile), "")]
        public OverlayTile SelectedTile
        {
            get { return this.selectedTile; }
            set
            {
                this.selectedTile = value;
                this.deleteButton.Enabled = this.selectedTile != null;
            }
        }

        private Track track;

        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track
        {
            get { return this.track; }
            set
            {
                if (this.track == value)
                {
                    return;
                }
 
                if (this.track != null)
                {
                    this.track.OverlayTiles.ElementAdded -= this.track_OverlayTiles_ElementAdded;
                    this.track.OverlayTiles.ElementRemoved -= this.track_OverlayTiles_ElementRemoved;
                    this.track.OverlayTiles.ElementsCleared -= this.track_OverlayTiles_ElementsCleared;
                    this.track.PropertyChanged -= this.track_PropertyChanged;
                }

                this.SelectedTile = null;

                this.track = value;

                this.track.OverlayTiles.ElementAdded += this.track_OverlayTiles_ElementAdded;
                this.track.OverlayTiles.ElementRemoved += this.track_OverlayTiles_ElementRemoved;
                this.track.OverlayTiles.ElementsCleared += this.track_OverlayTiles_ElementsCleared;
                this.track.PropertyChanged += this.track_PropertyChanged;

                this.Tileset = this.track.RoadTileset;
                this.UpdateTileCount();
            }
        }

        public OverlayControl()
        {
            this.InitializeComponent();

            this.tilesetPanel.Zoom = OverlayTilesetDrawer.Zoom;
        }

        public void InitOnFirstRomLoad()
        {
            this.drawer = new OverlayTilesetDrawer();
            this.InitOnRomLoad();

            // The following event handler is added here rather than in the Designer.cs
            // to save us a null check on this.drawer in each of the corresponding functions,
            // because the drawer doesn't exist yet before a ROM is loaded.
            this.tilesetPanel.Paint += this.TilesetPanelPaint;
        }

        public void InitOnRomLoad()
        {
            int tilesetHeight = this.LoadPatternDictionary();
            this.SetTilesetHeight(tilesetHeight);

            this.drawer.PatternList = this.patternList;
        }

        /// <summary>
        /// Loads the dictionary of patterns, and their location.
        /// </summary>
        /// <returns>The height of the tileset.</returns>
        private int LoadPatternDictionary()
        {
            this.patternList = new Dictionary<OverlayTilePattern, Point>();
            List<OverlayTilePattern> patterns = OverlayControl.GetUniquePatterns();

            int tilesetX = 0; // Current horizontal drawing position in the tileset
            int tilesetY = 0; // Current vertical drawing position in the tileset
            int tallestPattern = 0; // The tallest tile pattern in a given row

            int panelWidth = this.tilesetPanel.Width / (Tile.Size * OverlayTilesetDrawer.Zoom); // Take tile width and zoom in consideration
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
                    tilesetX = ((panelWidth - rowWidth) * Tile.Size) / 2;
                }

                // Store the pattern(s) of the row, and their location
                while (patternRowIterator < patternCountInRow)
                {
                    OverlayTilePattern pattern = patterns[patternId];
                    this.patternList.Add(pattern, new Point(tilesetX, tilesetY));

                    tilesetX += pattern.Width * Tile.Size;
                    if (pattern.Height > tallestPattern)
                    {
                        tallestPattern = pattern.Height;
                    }

                    patternRowIterator++;
                    patternId++;
                }

                tilesetY += tallestPattern * Tile.Size;
            }

            if (tilesetX != 0)
            {
                tilesetY += tallestPattern;
            }

            return tilesetY * OverlayTilesetDrawer.Zoom;
        }

        /// <summary>
        /// Gets the overlay tile patterns of the game, skipping duplicate patterns.
        /// </summary>
        private static List<OverlayTilePattern> GetUniquePatterns()
        {
            OverlayTilePattern previousPattern = null;
            List<OverlayTilePattern> patterns = new List<OverlayTilePattern>();
            foreach (OverlayTilePattern pattern in Context.Game.OverlayTilePatterns)
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
        /// Sets the height of the tileset Panel (and its parent) depending on the tileset height.
        /// </summary>
        private void SetTilesetHeight(int tilesetHeight)
        {
            int difference = tilesetHeight - this.tilesetPanel.Height;
            this.tilesetPanel.Height = tilesetHeight;
            this.Height += difference;
            this.drawer.SetImageSize(this.tilesetPanel.Size);
        }

        private RoadTileset Tileset
        {
            get
            {
                if (this.drawer == null)
                {
                    return null;
                }

                return this.drawer.Tileset;
            }
            set
            {
                if (this.drawer == null)
                {
                    return;
                }

                this.drawer.Tileset = value;
                this.tilesetPanel.Refresh();
            }
        }

        public void UpdateTileset()
        {
            this.Tileset = this.drawer.Tileset;
        }

        private void UpdateTileCount()
        {
            int count = this.track.OverlayTiles.Count;
            this.tileCountLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}", count, OverlayTiles.MaxTileCount);
            this.tileCountLabel.ForeColor = count >= OverlayTiles.MaxTileCount ? Color.Red : SystemColors.ControlText;
        }

        private void TilesetPanelPaint(object sender, PaintEventArgs e)
        {
            this.drawer.DrawTileset(e.Graphics);
        }

        private void TilesetPanelMouseMove(object sender, MouseEventArgs e)
        {
            this.hoveredPattern = this.GetPatternAt(e.Location);

            if (this.drawer.HoveredPattern != this.hoveredPattern)
            {
                this.drawer.HoveredPattern = this.hoveredPattern;
                this.tilesetPanel.Invalidate();
            }
        }

        private OverlayTilePattern GetPatternAt(Point point)
        {
            return this.GetPatternAt(point.X, point.Y);
        }

        private OverlayTilePattern GetPatternAt(int x, int y)
        {
            x /= OverlayTilesetDrawer.Zoom;
            y /= OverlayTilesetDrawer.Zoom;

            foreach (KeyValuePair<OverlayTilePattern, Point> kvp in this.patternList)
            {
                OverlayTilePattern pattern = kvp.Key;
                Point location = kvp.Value;

                if (x >= location.X &&
                    x < location.X + pattern.Width * Tile.Size &&
                    y >= location.Y &&
                    y < location.Y + pattern.Height * Tile.Size)
                {
                    return pattern;
                }
            }

            return null;
        }

        private void TilesetPanelMouseLeave(object sender, EventArgs e)
        {
            this.drawer.HoveredPattern = null;
            this.tilesetPanel.Invalidate();
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            this.track.OverlayTiles.Remove(this.SelectedTile);
        }

        private void TilesetPanelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            this.SelectedPatternInternal = this.hoveredPattern;
        }

        private void DeleteAllButtonClick(object sender, EventArgs e)
        {
            DialogResult result = UITools.ShowWarning("Do you really want to delete all overlay tiles?");

            if (result == DialogResult.Yes)
            {
                this.track.OverlayTiles.Clear();
            }
        }

        private void track_OverlayTiles_ElementAdded(object sender, EventArgs<OverlayTile> e)
        {
            this.UpdateTileCount();
        }

        private void track_OverlayTiles_ElementRemoved(object sender, EventArgs<OverlayTile> e)
        {
            this.SelectedTile = null;
            this.UpdateTileCount();
            this.ElementRemoved(this, new OverlayTileEventArgs(e.Value));
        }

        private void track_OverlayTiles_ElementsCleared(object sender, EventArgs e)
        {
            this.SelectedTile = null;
            this.UpdateTileCount();
            this.ElementsCleared(this, EventArgs.Empty);
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Theme")
            {
                this.Tileset = this.track.RoadTileset;
            }
        }

        private sealed class OverlayPanel : TilePanel
        {
            protected override Tile GetTileAt(int x, int y)
            {
                OverlayControl parent = this.Parent as OverlayControl;
                OverlayTilePattern pattern = parent.GetPatternAt((int)(x * this.Zoom), (int)(y * this.Zoom));

                if (pattern == null)
                {
                    return null;
                }

                Point location = parent.patternList[pattern];
                x = (x - location.X) / Tile.Size;
                y = (y - location.Y) / Tile.Size;

                byte tileId = pattern[x, y];

                return tileId == OverlayTile.None ? null : parent.Tileset[tileId];
            }
        }
    }

    /// <summary>
    /// Wrapper class around a generic EventArgs to make it work with the WinForms designer.
    /// </summary>
    internal class OverlayTileEventArgs : EventArgs<OverlayTile>
    {
        public OverlayTileEventArgs(OverlayTile value) : base(value) { }
    }
}
