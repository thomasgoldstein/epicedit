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
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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

        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add => tilesetPanel.ColorSelected += value;
            remove => tilesetPanel.ColorSelected -= value;
        }
        #endregion Events

        private Dictionary<OverlayTilePattern, Point> _patternList;

        /// <summary>
        /// Used to draw the overlay tileset.
        /// </summary>
        private OverlayTilesetDrawer _drawer;

        /// <summary>
        /// The hovered overlay tile pattern.
        /// </summary>
        private OverlayTilePattern _hoveredPattern;

        /// <summary>
        /// The selected overlay tile pattern.
        /// </summary>
        private OverlayTilePattern _selectedPattern;

        /// <summary>
        /// Gets or sets the selected overlay tile pattern.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(OverlayTilePattern), "")]
        public OverlayTilePattern SelectedPattern
        {
            get => _selectedPattern;
            set
            {
                if (value != null && !_patternList.ContainsKey(value))
                {
                    // Since duplicates are removed from the pattern list,
                    // it's possible that we can't find the exact same pattern
                    // referenced by the overlay tile.
                    // In this case, find another pattern with the same properties.

                    foreach (KeyValuePair<OverlayTilePattern, Point> kvp in _patternList)
                    {
                        OverlayTilePattern pattern = kvp.Key;
                        if (pattern.Equals(value))
                        {
                            value = pattern;
                            break;
                        }
                    }
                }

                SelectedPatternInternal = value;
            }
        }

        private OverlayTilePattern SelectedPatternInternal
        {
            get => _selectedPattern;
            set
            {
                if (value != null && SelectedTile != null)
                {
                    SelectedTile = null;
                    RepaintRequested(this, EventArgs.Empty);
                }

                if (_selectedPattern != value)
                {
                    _selectedPattern = value;
                    _drawer.SelectedPattern = value;
                    tilesetPanel.Invalidate();
                }
            }
        }

        /// <summary>
        /// The selected track overlay tile.
        /// </summary>
        private OverlayTile _selectedTile;

        /// <summary>
        /// Gets or sets the selected track overlay tile.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(OverlayTile), "")]
        public OverlayTile SelectedTile
        {
            get => _selectedTile;
            set
            {
                _selectedTile = value;
                deleteButton.Enabled = _selectedTile != null;
            }
        }

        private Track _track;

        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track
        {
            get => _track;
            set
            {
                if (_track == value)
                {
                    return;
                }

                if (_track != null)
                {
                    _track.OverlayTiles.ElementAdded -= track_OverlayTiles_ElementAdded;
                    _track.OverlayTiles.ElementRemoved -= track_OverlayTiles_ElementRemoved;
                    _track.OverlayTiles.ElementsCleared -= track_OverlayTiles_ElementsCleared;
                    _track.PropertyChanged -= track_PropertyChanged;
                    _track.ColorGraphicsChanged -= track_ColorsGraphicsChanged;
                    _track.ColorsGraphicsChanged -= track_ColorsGraphicsChanged;
                }

                _track = value;

                _track.OverlayTiles.ElementAdded += track_OverlayTiles_ElementAdded;
                _track.OverlayTiles.ElementRemoved += track_OverlayTiles_ElementRemoved;
                _track.OverlayTiles.ElementsCleared += track_OverlayTiles_ElementsCleared;
                _track.PropertyChanged += track_PropertyChanged;
                _track.ColorGraphicsChanged += track_ColorsGraphicsChanged;
                _track.ColorsGraphicsChanged += track_ColorsGraphicsChanged;

                SelectedTile = null;
                LoadTileset(_track.RoadTileset);
                UpdateTileCount();
            }
        }

        public OverlayControl()
        {
            InitializeComponent();

            tilesetPanel.Zoom = OverlayTilesetDrawer.Zoom;
        }

        public void InitOnFirstRomLoad()
        {
            _drawer = new OverlayTilesetDrawer();
            InitOnRomLoad();

            // The following event handler is added here rather than in the Designer.cs
            // to save us a null check on this.drawer in each of the corresponding functions,
            // because the drawer doesn't exist yet before a ROM is loaded.
            tilesetPanel.Paint += TilesetPanelPaint;
        }

        public void InitOnRomLoad()
        {
            int tilesetHeight = LoadPatternDictionary();
            SetTilesetHeight(tilesetHeight);

            _drawer.PatternList = _patternList;
        }

        /// <summary>
        /// Loads the dictionary of patterns, and their location.
        /// </summary>
        /// <returns>The height of the tileset.</returns>
        private int LoadPatternDictionary()
        {
            _patternList = new Dictionary<OverlayTilePattern, Point>();
            List<OverlayTilePattern> patterns = GetUniquePatterns();

            int tilesetX = 0; // Current horizontal drawing position in the tileset
            int tilesetY = 0; // Current vertical drawing position in the tileset
            int tallestPattern = 0; // The tallest tile pattern in a given row

            int panelWidth = tilesetPanel.Width / (Tile.Size * OverlayTilesetDrawer.Zoom); // Take tile width and zoom in consideration
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
                    _patternList.Add(pattern, new Point(tilesetX, tilesetY));

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
            int difference = tilesetHeight - tilesetPanel.Height;
            tilesetPanel.Height = tilesetHeight;
            Height += difference;
            _drawer.SetImageSize(tilesetPanel.Size);
        }

        private RoadTileset Tileset
        {
            get => _drawer?.Tileset;
            set
            {
                if (_drawer == null ||
                    _drawer.Tileset == value)
                {
                    return;
                }

                LoadTileset(value);
            }
        }

        private void LoadTileset(RoadTileset tileset)
        {
            _drawer.Tileset = tileset;
            tilesetPanel.Refresh();
        }

        public void UpdateTileset()
        {
            _drawer.ReloadTileset();
            tilesetPanel.Refresh();
        }

        private void UpdateTileCount()
        {
            int count = _track.OverlayTiles.Count;
            tileCountLabel.Text = $"{count}/{OverlayTiles.MaxTileCount}";
            tileCountLabel.ForeColor = count >= OverlayTiles.MaxTileCount ? Color.Red : SystemColors.ControlText;
        }

        private void TilesetPanelPaint(object sender, PaintEventArgs e)
        {
            _drawer.DrawTileset(e.Graphics);
        }

        private void TilesetPanelMouseMove(object sender, MouseEventArgs e)
        {
            _hoveredPattern = GetPatternAt(e.Location);

            if (_drawer.HoveredPattern != _hoveredPattern)
            {
                _drawer.HoveredPattern = _hoveredPattern;
                tilesetPanel.Invalidate();
            }
        }

        private OverlayTilePattern GetPatternAt(Point point)
        {
            return GetPatternAt(point.X, point.Y);
        }

        private OverlayTilePattern GetPatternAt(int x, int y)
        {
            x /= OverlayTilesetDrawer.Zoom;
            y /= OverlayTilesetDrawer.Zoom;

            foreach (KeyValuePair<OverlayTilePattern, Point> kvp in _patternList)
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
            _drawer.HoveredPattern = null;
            tilesetPanel.Invalidate();
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            _track.OverlayTiles.Remove(SelectedTile);
        }

        private void TilesetPanelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            SelectedPatternInternal = _hoveredPattern;
        }

        private void DeleteAllButtonClick(object sender, EventArgs e)
        {
            DialogResult result = UITools.ShowWarning("Do you really want to delete all overlay tiles?");

            if (result == DialogResult.Yes)
            {
                _track.OverlayTiles.Clear();
            }
        }

        private void track_OverlayTiles_ElementAdded(object sender, EventArgs<OverlayTile> e)
        {
            UpdateTileCount();
        }

        private void track_OverlayTiles_ElementRemoved(object sender, EventArgs<OverlayTile> e)
        {
            SelectedTile = null;
            UpdateTileCount();
        }

        private void track_OverlayTiles_ElementsCleared(object sender, EventArgs e)
        {
            SelectedTile = null;
            UpdateTileCount();
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.Track.Theme)
            {
                LoadTileset(_track.RoadTileset);
            }
        }

        private void track_ColorsGraphicsChanged(object sender, EventArgs e)
        {
            if ((sender as Palette).Index < Palettes.SpritePaletteStart)
            {
                UpdateTileset();
            }
        }

        private sealed class OverlayPanel : TilePanel
        {
            protected override Tile GetTileAt(int x, int y)
            {
                OverlayControl parent = Parent as OverlayControl;
                OverlayTilePattern pattern = parent.GetPatternAt((int)(x * Zoom), (int)(y * Zoom));

                if (pattern == null)
                {
                    return null;
                }

                Point location = parent._patternList[pattern];
                x = (x - location.X) / Tile.Size;
                y = (y - location.Y) / Tile.Size;

                byte tileId = pattern[x, y];

                return tileId == OverlayTile.None ? null : parent.Tileset[tileId];
            }
        }
    }
}
