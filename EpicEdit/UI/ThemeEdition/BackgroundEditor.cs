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
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit backgrounds.
    /// </summary>
    internal partial class BackgroundEditor : UserControl
    {
        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add
            {
                this.frontLayerPanel.ColorSelected += value;
                this.backLayerPanel.ColorSelected += value;
                this.frontTilePanel.ColorSelected += value;
                this.backTilePanel.ColorSelected += value;
            }
            remove
            {
                this.frontLayerPanel.ColorSelected -= value;
                this.backLayerPanel.ColorSelected -= value;
                this.frontTilePanel.ColorSelected -= value;
                this.backTilePanel.ColorSelected -= value;
            }
        }

        private BackgroundDrawer drawer;

        private Theme Theme
        {
            get { return this.themeComboBox.SelectedItem as Theme; }
        }

        private byte TileId
        {
            get { return this.frontLayerPanel.TileId; }
            set
            {
                this.frontLayerPanel.TileId = value;
                this.backLayerPanel.TileId = value;
            }
        }

        private Tile2bppProperties TileProperties
        {
            get { return this.frontLayerPanel.TileProperties; }
            set
            {
                this.frontLayerPanel.TileProperties = value;
                this.backLayerPanel.TileProperties = value;
            }
        }

        public BackgroundEditor()
        {
            this.InitializeComponent();

            this.drawer = new BackgroundDrawer();

            this.frontLayerPanel.Height += SystemInformation.HorizontalScrollBarHeight;
            this.frontLayerPanel.Zoom = BackgroundDrawer.Zoom;
            this.frontLayerPanel.Drawer = this.drawer;
            this.frontLayerPanel.TileSelected += this.BackgroundLayerPanelTileSelected;

            this.backLayerPanel.Height += SystemInformation.HorizontalScrollBarHeight;
            this.backLayerPanel.Zoom = BackgroundDrawer.Zoom;
            this.backLayerPanel.Drawer = this.drawer;
            this.backLayerPanel.TileSelected += this.BackgroundLayerPanelTileSelected;

            this.backgroundPreviewer.Drawer = this.drawer;
        }

        public void Init()
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
            this.themeComboBox.SelectedIndex = 0;
        }

        public void ResetSettings()
        {
            this.PausePreview();
            this.drawer.RewindPreview();
            this.frontLayerPanel.AutoScrollPosition = Point.Empty;
            this.backLayerPanel.AutoScrollPosition = Point.Empty;
        }

        private void PlayPreview()
        {
            this.backgroundPreviewer.Play();
            this.playPauseButton.Text = "Pause";
        }

        private void PausePreview()
        {
            this.backgroundPreviewer.Pause();
            this.playPauseButton.Text = "Play";
        }

        public void UpdateBackground(Theme theme)
        {
            if (theme == this.Theme)
            {
                this.LoadTheme();
            }
        }

        private void LoadTheme()
        {
            Theme theme = this.Theme;
            this.drawer.LoadTheme(theme);
            this.frontLayerPanel.Background = this.backLayerPanel.Background = theme.Background;
            this.UpdateTilePanels();
            this.tilesetPanel.LoadTheme(theme);

            this.frontLayerPanel.Refresh();
            this.backLayerPanel.Refresh();
            this.backgroundPreviewer.Refresh();
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.ResetSettings();
            this.LoadTheme();
        }
        
        private void PlayPauseButtonClick(object sender, EventArgs e)
        {
            if (this.backgroundPreviewer.Paused)
            {
                this.PlayPreview();
            }
            else
            {
                this.PausePreview();
            }
        }

        private void RewindButtonClick(object sender, EventArgs e)
        {
            this.drawer.RewindPreview();
            this.backgroundPreviewer.Invalidate();
        }

        private void BackgroundLayerPanelTileChanged(object sender, EventArgs e)
        {
            this.backgroundPreviewer.Invalidate();
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            Tile2bppProperties properties = this.TileProperties;

            int value = (int)this.paletteNumericUpDown.Value - 1;
            properties.PaletteIndex = value / 4;
            properties.SubPaletteIndex = (value & 0x3) * 4;

            this.TileProperties = properties;
            this.UpdateTilePanels();
        }

        private void FlipXButtonCheckedChanged(object sender, EventArgs e)
        {
            Tile2bppProperties properties = this.TileProperties;
            properties.FlipX();

            this.TileProperties = properties;
            this.UpdateTilePanels();
        }

        private void FlipYButtonCheckedChanged(object sender, EventArgs e)
        {
            Tile2bppProperties properties = this.TileProperties;
            properties.FlipY();

            this.TileProperties = properties;
            this.UpdateTilePanels();
        }

        private void UpdateTilePanels()
        {
            Theme theme = this.Theme;
            byte tileId = this.TileId;
            byte properties = this.TileProperties.GetByte();

            this.frontTilePanel.UpdateTile(theme, tileId, properties);
            this.backTilePanel.UpdateTile(theme, tileId, properties);
        }

        private void BackgroundLayerPanelTileSelected(object sender, EventArgs<byte, Tile2bppProperties> e)
        {
            this.TileId = e.Value1;
            this.TileProperties = e.Value2;

            this.paletteNumericUpDown.ValueChanged -= this.PaletteNumericUpDownValueChanged;
            this.paletteNumericUpDown.Value = 1 + (this.TileProperties.PaletteIndex * 4) + (this.TileProperties.SubPaletteIndex / 4);
            this.paletteNumericUpDown.ValueChanged += this.PaletteNumericUpDownValueChanged;

            bool flipX = (this.TileProperties.Flip & Flip.X) != 0;
            if (this.flipXButton.Checked != flipX)
            {
                this.flipXButton.CheckedChanged -= this.FlipXButtonCheckedChanged;
                this.flipXButton.Checked = flipX;
                this.flipXButton.CheckedChanged += this.FlipXButtonCheckedChanged;
            }

            bool flipY = (this.TileProperties.Flip & Flip.Y) != 0;
            if (this.flipYButton.Checked != flipY)
            {
                this.flipYButton.CheckedChanged -= this.FlipYButtonCheckedChanged;
                this.flipYButton.Checked = flipY;
                this.flipYButton.CheckedChanged += this.FlipYButtonCheckedChanged;
            }

            this.UpdateTilePanels();
            this.tilesetPanel.SelectedTile = this.TileId;
        }
        
        private void TilesetPanelSelectedTileChanged(object sender, EventArgs e)
        {
            this.TileId = this.tilesetPanel.SelectedTile;
            this.UpdateTilePanels();
        }
    }
}
