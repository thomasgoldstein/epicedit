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
using System.Drawing.Imaging;
using System.IO;
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
        /// Raised when tile graphics have been changed.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs<byte>> TileChanged;

        /// <summary>
        /// Raised when the theme tileset has been changed.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs> TilesetChanged;

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
                this.SetCurrentTile();
                this.userAction = true;
            }
        }

        private RoadTile SelectedRoadTile
        {
            get { return this.track.GetRoadTile(this.selectedTile); }
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
            this.tileGenreComboBox.SelectedIndexChanged += this.TileGenreComboBoxSelectedIndexChanged;
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Theme theme = this.themeComboBox.SelectedItem as Theme;
            if (this.track.Theme != theme)
            {
                this.track.Theme = theme;
                this.TrackThemeChanged(this, EventArgs.Empty);
            }

            this.tilesetPanel.SetTileset(theme.GetRoadTileset());
            this.ResetTileset();
            this.SetCurrentTile();

            this.SelectedThemeChanged(this, EventArgs.Empty);
        }

        private void SetCurrentTile()
        {
            this.SelectTileGenre();
            this.SelectTilePalette();
            this.tilesetPanel.Invalidate();
        }

        private void TileGenreComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.userAction)
            {
                return;
            }

            this.SelectedRoadTile.Genre = (TileGenre)this.tileGenreComboBox.SelectedItem;
            this.track.Theme.RoadTileset.Modified = true;
        }

        private void TilePaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!this.userAction)
            {
                return;
            }

            int palIndex = (int)this.tilePaletteNumericUpDown.Value - 1;
            this.SelectedRoadTile.Palette = this.track.Theme.Palettes[palIndex];
            this.track.Theme.RoadTileset.Modified = true;

            // Could be optimized by not updating the whole cache,
            // and not repainting the whole panel (but it's already fast enough)
            this.tilesetDrawer.UpdateCache();
            this.tilesetPanel.Refresh();

            this.TileChanged(this, new EventArgs<byte>(this.selectedTile));
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
            this.tileGenreComboBox.SelectedItem = this.SelectedRoadTile.Genre;
        }

        private void SelectTilePalette()
        {
            this.tilePaletteNumericUpDown.Value = this.SelectedRoadTile.Palette.Index + 1;
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

        private void ImportGraphicsButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter =
                    "PNG (*.png)|*.png|" +
                    "BMP (*.bmp)|*.bmp";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.ImportGraphics(ofd.FileName);
                }
            }
        }

        private void ImportGraphics(string filePath)
        {
            try
            {
                using (Bitmap tilesetImage = new Bitmap(filePath))
                {
                    int width = tilesetImage.Width;
                    int height = tilesetImage.Height;

                    if (width % Tile.Size != 0 ||
                        height % Tile.Size != 0 ||
                        (width * height) != (RoadTileset.TileCount * Tile.Size * Tile.Size))
                    {
                        throw new InvalidDataException("Invalid tileset size.");
                    }

                    int yTileCount = height / Tile.Size;
                    int xTileCount = width / Tile.Size;

                    for (int y = 0; y < yTileCount; y++)
                    {
                        for (int x = 0; x < xTileCount; x++)
                        {
                            Bitmap tileImage = tilesetImage.Clone(
                                new Rectangle(x * Tile.Size,
                                              y * Tile.Size,
                                              Tile.Size,
                                              Tile.Size),
                                PixelFormat.Format32bppPArgb);

                            RoadTile tile = this.track.GetRoadTile(y * xTileCount + x);
                            tile.Bitmap = tileImage;
                        }
                    }

                    this.tilesetDrawer.UpdateCache();
                    this.tilesetPanel.Refresh();
                    this.track.Theme.RoadTileset.Modified = true;
                    this.TilesetChanged(this, EventArgs.Empty);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }

        private void ExportGraphicsButtonClick(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter =
                    "PNG (*.png)|*.png|" +
                    "BMP (*.bmp)|*.bmp";

                string fileName = this.track.Theme.Name;
                fileName = UITools.SanitizeFileName(fileName);

                sfd.FileName = fileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ImageFormat format;

                    switch (Path.GetExtension(sfd.FileName).ToUpperInvariant())
                    {
                        default:
                        case ".PNG":
                            format = ImageFormat.Png;
                            break;

                        case ".BMP":
                            format = ImageFormat.Bmp;
                            break;
                    }

                    this.tilesetDrawer.Image.Save(sfd.FileName, format);
                }
            }
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
