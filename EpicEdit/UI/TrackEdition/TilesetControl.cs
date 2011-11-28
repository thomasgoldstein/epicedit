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
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a list of <see cref="Theme"/> objects, and the tileset of the selected theme.
    /// </summary>
    public partial class TilesetControl : UserControl
    {
        #region Events
        /// <summary>
        /// Raised when the theme of the current track has been modified.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> TrackThemeChanged;

        /// <summary>
        /// Raised when the selected theme has been changed.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> SelectedThemeChanged;

        /// <summary>
        /// Raised when a new tile has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> SelectedTileChanged;

        /// <summary>
        /// Raised when the track map has been changed.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> TrackMapChanged;

        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add { this.tilesetPanel.ColorSelected += value; }
            remove { this.tilesetPanel.ColorSelected -= value; }
        }
        #endregion Events

        /// <summary>
        /// Used to draw the tileset.
        /// </summary>
        private TilesetDrawer tilesetDrawer;

        private Track track = null;

        /// <summary>
        /// Flag to differentiate user actions and automatic actions.
        /// </summary>
        private bool userAction = false;

        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track
        {
            get { return this.track; }
            set
            {
                this.userAction = false;

                if (this.track == null)
                {
                    this.tileGenreComboBox.SelectedIndexChanged += TileGenreComboBoxSelectedIndexChanged;
                }

                this.track = value;
                this.SelectTrackTheme();
                this.userAction = true;
            }
        }

        private byte selectedTile = 0;

        [Browsable(false), DefaultValue(typeof(byte), "0")]
        public byte SelectedTile
        {
            get { return this.selectedTile; }
            set
            {
                this.userAction = false;
                this.selectedTile = value;
                this.tilesetPanel.Invalidate();
                this.SelectTileGenre();
                this.userAction = true;
            }
        }

        public TilesetControl()
        {
            this.InitializeComponent();

            this.tilesetPanel.Zoom = TilesetDrawer.Zoom;
        }

        public void InitOnFirstRomLoad()
        {
            this.tilesetDrawer = new TilesetDrawer(this.tilesetPanel);

            // The following event handler is added here rather than in the Designer.cs
            // to save us a null check on this.drawer in each of the corresponding functions,
            // because the drawer doesn't exist yet before a ROM is loaded.
            this.tilesetPanel.Paint += this.TilesetPanelPaint;

            // The following event handler is added here rather than in the Designer.cs
            // to avoid an extra repaint triggered by
            // selecting the current theme in the theme ComboBox.
            this.themeComboBox.SelectedIndexChanged += this.ThemeComboBoxSelectedIndexChanged;

            this.InitTileGenreComboBox();

            this.InitOnRomLoad();
        }

        public void InitOnRomLoad()
        {
            this.InitThemeComboBox();
        }

        private void InitThemeComboBox()
        {
            this.themeComboBox.BeginUpdate();
            this.themeComboBox.Items.Clear();
            foreach (Theme theme in Context.Game.Themes)
            {
                this.themeComboBox.Items.Add(theme);
            }
            this.themeComboBox.EndUpdate();
        }

        private void InitTileGenreComboBox()
        {
            this.tileGenreComboBox.DataSource = Enum.GetValues(typeof(TileGenre));
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Theme theme = this.themeComboBox.SelectedItem as Theme;
            if (this.track.Theme != theme)
            {
                this.track.Theme = theme;
                this.TrackThemeChanged(this, EventArgs.Empty);
            }

            this.SelectTileGenre();

            this.tilesetPanel.SetTileset(theme.GetRoadTileset());
            this.ResetTileset();
            this.tilesetPanel.Invalidate();

            this.SelectedThemeChanged(this, EventArgs.Empty);
        }

        private void TileGenreComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.track.GetRoadTile(this.selectedTile).Genre = (TileGenre)this.tileGenreComboBox.SelectedItem;

            if (this.userAction)
            {
                this.track.Theme.Modified = true;
            }
        }

        public void UpdateTileset()
        {
            this.ResetTileset();
            this.tilesetPanel.Refresh();
        }

        private void ResetTileset()
        {
            Tile[] tileset = this.track.GetRoadTileset();
            this.tilesetDrawer.SetTileset(tileset);
        }

        private void SelectTrackTheme()
        {
            this.themeComboBox.SelectedItem = this.track.Theme;
        }

        private void SelectTileGenre()
        {
            this.tileGenreComboBox.SelectedItem = this.track.GetRoadTile(this.selectedTile).Genre;
        }

        private void TilesetPanelPaint(object sender, PaintEventArgs e)
        {
            this.tilesetDrawer.DrawTileset(e.Graphics, this.selectedTile);
        }

        private void TilesetPanelMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
            {
                return;
            }

            int zoom = TilesetDrawer.Zoom;
            int rowTileCount = this.tilesetPanel.Width / (Tile.Size * zoom);
            byte newSelectedTile = (byte)((e.X / (Tile.Size * zoom)) + (e.Y / (Tile.Size * zoom)) * rowTileCount);

            if (this.selectedTile != newSelectedTile)
            {
                this.userAction = false;
                this.SelectedTile = newSelectedTile;
                this.userAction = true;

                this.SelectedTileChanged(this, EventArgs.Empty);
            }
        }

        private void TileGenreComboBoxFormat(object sender, ListControlConvertEventArgs e)
        {
            object val = e.Value;
            e.Value = ((byte)val).ToString("X") + "- " + UITools.GetDescription(val);
        }

        private void ResetMapButtonClick(object sender, EventArgs e)
        {
            DialogResult result = UITools.ShowWarning("Do you really want to reset the map?");

            if (result == DialogResult.Yes)
            {
                this.track.Map.Clear(this.selectedTile);
                this.TrackMapChanged(this, EventArgs.Empty);
            }
        }

        private sealed class TilesetPanel : TilePanel
        {
            private Tile[] tileset;

            public void SetTileset(Tile[] tileset)
            {
                this.tileset = tileset;
            }

            private int TilesPerRow
            {
                get { return (int)(this.Width / (Tile.Size * this.Zoom)); }
            }

            protected override Tile GetTileAt(int x, int y)
            {
                // Convert from pixel precision to tile precision
                x /= Tile.Size;
                y /= Tile.Size;

                int index = y * this.TilesPerRow + x;
                return this.tileset[index];
            }
        }
    }
}
