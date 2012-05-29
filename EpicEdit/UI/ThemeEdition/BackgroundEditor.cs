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
using EpicEdit.Rom.Tracks.Scenery;
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
            }
            remove
            {
                this.frontLayerPanel.ColorSelected -= value;
                this.backLayerPanel.ColorSelected -= value;
            }
        }

        private BackgroundDrawer drawer;

        private Theme Theme
        {
            get { return this.themeComboBox.SelectedItem as Theme; }
        }

        public BackgroundEditor()
        {
            this.InitializeComponent();

            this.drawer = new BackgroundDrawer();

            this.frontLayerPanel.Height += SystemInformation.HorizontalScrollBarHeight;
            this.frontLayerPanel.Zoom = BackgroundDrawer.Zoom;
            this.frontLayerPanel.Paint += this.BackgroundLayerPanelPaint;

            this.backLayerPanel.Height += SystemInformation.HorizontalScrollBarHeight;
            this.backLayerPanel.Zoom = BackgroundDrawer.Zoom;
            this.backLayerPanel.Paint += this.BackgroundLayerPanelPaint;

            this.backgroundPreviewer.Drawer = this.drawer;
        }

        private void BackgroundLayerPanelPaint(object sender, PaintEventArgs e)
        {
            BackgroundPanel panel = sender as BackgroundPanel;
            int x = (int)(panel.AutoScrollPosition.X / panel.Zoom);
            this.drawer.DrawBackgroundLayer(e.Graphics, x, panel.Front);
        }

        private void BackgroundLayerPanelScroll(object sender, ScrollEventArgs e)
        {
            (sender as Control).Invalidate();
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

        private sealed class BackgroundPanel : TilePanel
        {
            public Background Background { get; set; }
            public bool Front { get; set; }

            protected override Tile GetTileAt(int x, int y)
            {
                // Convert from pixel precision to tile precision
                x /= Tile.Size;
                y /= Tile.Size;

                byte tileId;
                byte properties;
                this.Background.Layout.GetTileData(x, y, this.Front, out tileId, out properties);

                Tile2bpp tile = this.Background.Tileset[tileId];
                int paletteStart = this.Front ? Background.FrontPaletteStart : Background.BackPaletteStart;
                Tile2bpp clone = new Tile2bpp(tile.Graphics, tile.Palettes, properties, paletteStart);
                return clone; // NOTE: We're leaking a bit of memory here, as the clone is not explicitly disposed
            }
        }
    }
}
