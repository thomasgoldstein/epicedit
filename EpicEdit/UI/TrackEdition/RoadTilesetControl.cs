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
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a list of <see cref="Theme"/> objects, and the tileset of the selected theme.
    /// </summary>
    internal partial class RoadTilesetControl : UserControl
    {
        #region Events
        /// <summary>
        /// Raised when the selected theme has been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SelectedThemeChanged;

        /// <summary>
        /// Raised when a new tile has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SelectedTileChanged;

        /// <summary>
        /// Raised when the track map has been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> TrackMapChanged;

        /// <summary>
        /// Raised when tile graphics have been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<byte>> TileChanged;

        /// <summary>
        /// Raised when the theme tileset has been changed.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> TilesetChanged;

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

        /// <summary>
        /// Used to draw the tileset.
        /// </summary>
        private RoadTilesetDrawer _drawer;

        private Track _track;

        /// <summary>
        /// Flag to differentiate user actions and automatic actions.
        /// </summary>
        private bool _fireEvents;

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
                    _track.ColorGraphicsChanged -= track_ColorsGraphicsChanged;
                    _track.ColorsGraphicsChanged -= track_ColorsGraphicsChanged;
                    _track.PropertyChanged -= track_PropertyChanged;
                }

                _track = value;

                _track.ColorGraphicsChanged += track_ColorsGraphicsChanged;
                _track.ColorsGraphicsChanged += track_ColorsGraphicsChanged;
                _track.PropertyChanged += track_PropertyChanged;

                SelectTrackTheme();
            }
        }

        private byte _selectedTile;

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte SelectedTile
        {
            get => _selectedTile;
            set
            {
                if (_selectedTile == value)
                {
                    return;
                }

                _fireEvents = false;
                _selectedTile = value;
                SetCurrentTile();
                _fireEvents = true;
            }
        }

        private RoadTile SelectedRoadTile => _track.RoadTileset[_selectedTile];

        public RoadTilesetControl()
        {
            InitializeComponent();

            tilesetPanel.Zoom = RoadTilesetDrawer.Zoom;
        }

        private void track_ColorsGraphicsChanged(object sender, EventArgs e)
        {
            if (((Palette)sender).Index < Palettes.SpritePaletteStart)
            {
                UpdateTileset();
            }
        }

        private void track_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.Track.Theme)
            {
                SelectTrackTheme();
            }
        }

        public void InitOnFirstRomLoad()
        {
            _drawer = new RoadTilesetDrawer(tilesetPanel.Size);

            // The following event handler is added here rather than in the Designer.cs
            // to save us a null check on this.drawer in each of the corresponding functions,
            // because the drawer doesn't exist yet before a ROM is loaded.
            tilesetPanel.Paint += TilesetPanelPaint;

            // The following event handler is added here rather than in the Designer.cs
            // to avoid an extra repaint triggered by
            // selecting the current theme in the theme ComboBox.
            themeComboBox.SelectedIndexChanged += ThemeComboBoxSelectedIndexChanged;

            InitTileGenreComboBox();

            InitOnRomLoad();
        }

        public void InitOnRomLoad()
        {
            themeComboBox.Init();
        }

        private void InitTileGenreComboBox()
        {
            tileGenreComboBox.DataSource = Enum.GetValues(typeof(RoadTileGenre));
            tileGenreComboBox.SelectedIndexChanged += TileGenreComboBoxSelectedIndexChanged;
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            _track.Theme = themeComboBox.SelectedTheme;

            tilesetPanel.Tileset = _track.RoadTileset;
            ResetTileset();
            SetCurrentTile();

            SelectedThemeChanged(this, EventArgs.Empty);
        }

        private void SetCurrentTile()
        {
            SelectTileGenre();
            SelectTilePalette();
            tilesetPanel.Invalidate();
        }

        private void TileGenreComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            SelectedRoadTile.Genre = (RoadTileGenre)tileGenreComboBox.SelectedItem;
        }

        private void TilePaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            var palIndex = (int)tilePaletteNumericUpDown.Value;
            SelectedRoadTile.Palette = _track.Theme.Palettes[palIndex];

            // Could be optimized by not updating the whole cache,
            // and not repainting the whole panel (but it's already fast enough)
            UpdateTileset();

            TileChanged(this, new EventArgs<byte>(_selectedTile));
        }

        public void UpdateTileset()
        {
            ResetTileset();
            tilesetPanel.Refresh();
        }

        public void SetTheme(int number)
        {
            var ud = tilePaletteNumericUpDown;
            if (number >= ud.Minimum && number <= ud.Maximum)
            {
                ud.Value = number;
            }
        }

        public void SelectPenTool()
        {
            pencilButton.PerformClick();
        }

        public void SelectPaintBucketTool()
        {
            bucketButton.PerformClick();
        }

        private void ResetTileset()
        {
            _drawer.Tileset = _track.RoadTileset;
        }

        private void SelectTrackTheme()
        {
            _fireEvents = false;
            themeComboBox.SelectedItem = _track.Theme;
            _fireEvents = true;
        }

        private void SelectTileGenre()
        {
            tileGenreComboBox.SelectedItem = SelectedRoadTile.Genre;
        }

        private void SelectTilePalette()
        {
            tilePaletteNumericUpDown.Value = SelectedRoadTile.Palette.Index;
        }

        private void TilesetPanelPaint(object sender, PaintEventArgs e)
        {
            _drawer.DrawTileset(e.Graphics, _selectedTile);
        }

        private void TilesetPanelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            const int zoom = RoadTilesetDrawer.Zoom;
            var rowTileCount = tilesetPanel.Width / (Tile.Size * zoom);
            var newSelectedTile = (byte)((e.X / (Tile.Size * zoom)) + (e.Y / (Tile.Size * zoom)) * rowTileCount);

            if (_selectedTile != newSelectedTile)
            {
                SelectedTile = newSelectedTile;

                SelectedTileChanged(this, EventArgs.Empty);
            }
        }

        private void TileGenreComboBoxFormat(object sender, ListControlConvertEventArgs e)
        {
            var val = e.Value;
            e.Value = Utilities.ByteToHexString((byte)val) + "- " + UITools.GetDescription(val);
        }

        private void ImportExportRoadTilesetButtonClick(object sender, EventArgs e)
        {
            using (var form = new RoadTilesetImportExportForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.Action == RoadTilesetImportExportAction.Import)
                    {
                        switch (form.Type)
                        {
                            case RoadTilesetImportExportType.Graphics:
                                ShowImportTilesetGraphicsDialog();
                                break;

                            case RoadTilesetImportExportType.Genres:
                                ShowImportTilesetGenresDialog();
                                break;

                            case RoadTilesetImportExportType.Palettes:
                                ShowImportTilesetPalettesDialog();
                                break;
                        }
                    }
                    else
                    {
                        switch (form.Type)
                        {
                            case RoadTilesetImportExportType.Graphics:
                                ShowExportTilesetGraphicsDialog();
                                break;

                            case RoadTilesetImportExportType.Genres:
                                ShowExportTilesetGenresDialog();
                                break;

                            case RoadTilesetImportExportType.Palettes:
                                ShowExportTilesetPalettesDialog();
                                break;
                        }
                    }
                }
            }
        }

        private void ShowImportTilesetGraphicsDialog()
        {
            if (UITools.ShowImportTilesetGraphicsDialog(_track.RoadTileset))
            {
                UpdateTileset();
                TilesetChanged(this, EventArgs.Empty);
            }
        }

        private void ShowImportTilesetGenresDialog()
        {
            if (UITools.ShowImportBinaryDataDialog(_track.RoadTileset.SetTileGenreBytes))
            {
                SelectTileGenre();
            }
        }

        private void ShowImportTilesetPalettesDialog()
        {
            if (UITools.ShowImportBinaryDataDialog(_track.RoadTileset.SetTilePaletteBytes))
            {
                UpdateTileset();
                SelectTilePalette();
                TilesetChanged(this, EventArgs.Empty);
            }
        }

        private void ShowExportTilesetGraphicsDialog()
        {
            UITools.ShowExportTilesetGraphicsDialog(_drawer.Image, _track.Theme.RoadTileset, _track.Theme.Name + "road gfx");
        }

        private void ShowExportTilesetGenresDialog()
        {
            UITools.ShowExportBinaryDataDialog(_track.RoadTileset.GetTileGenreBytes, _track.Theme.Name + "road types");
        }

        private void ShowExportTilesetPalettesDialog()
        {
            UITools.ShowExportBinaryDataDialog(_track.RoadTileset.GetTilePaletteBytes, _track.Theme.Name + "road pals");
        }

        private void ResetMapButtonClick(object sender, EventArgs e)
        {
            var result = UITools.ShowWarning("Do you really want to reset the map?");

            if (result == DialogResult.Yes)
            {
                _track.Map.Clear(_selectedTile);
                TrackMapChanged(this, EventArgs.Empty);
            }
        }

        public bool BucketMode => bucketButton.Checked;

        private sealed class TilesetPanel : TilePanel
        {
            [Browsable(false), DefaultValue(typeof(RoadTileset), "")]
            public RoadTileset Tileset { get; set; }

            private int TilesPerRow => (int)(Width / (Tile.Size * Zoom));

            protected override Tile GetTileAt(int x, int y)
            {
                // Convert from pixel precision to tile precision
                x /= Tile.Size;
                y /= Tile.Size;

                var index = y * TilesPerRow + x;
                return Tileset[index];
            }
        }
    }
}
