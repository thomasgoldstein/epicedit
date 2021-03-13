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

using EpicEdit.Properties;
using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add
            {
                frontLayerPanel.ColorSelected += value;
                backLayerPanel.ColorSelected += value;
                frontTilePanel.ColorSelected += value;
                backTilePanel.ColorSelected += value;
                tilesetPanel.ColorSelected += value;
            }
            remove
            {
                frontLayerPanel.ColorSelected -= value;
                backLayerPanel.ColorSelected -= value;
                frontTilePanel.ColorSelected -= value;
                backTilePanel.ColorSelected -= value;
                tilesetPanel.ColorSelected -= value;
            }
        }

        private readonly BackgroundDrawer _drawer;
        private readonly Timer _previewTimer;

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get => themeComboBox.SelectedTheme;
            set => themeComboBox.SelectedTheme = value;
        }

        private byte TileId
        {
            get => frontLayerPanel.TileId;
            set
            {
                frontLayerPanel.TileId = value;
                backLayerPanel.TileId = value;
            }
        }

        private Tile2bppProperties TileProperties
        {
            get => frontLayerPanel.TileProperties;
            set
            {
                frontLayerPanel.TileProperties = value;
                backLayerPanel.TileProperties = value;
                tilesetPanel.TileProperties = value;
            }
        }

        public BackgroundEditor()
        {
            InitializeComponent();

            _drawer = new BackgroundDrawer();

            frontLayerPanel.Height += SystemInformation.HorizontalScrollBarHeight;
            frontLayerPanel.Zoom = BackgroundDrawer.Zoom;
            frontLayerPanel.Drawer = _drawer;
            frontLayerPanel.TileSelected += BackgroundLayerPanelTileSelected;

            backLayerPanel.Height += SystemInformation.HorizontalScrollBarHeight;
            backLayerPanel.Zoom = BackgroundDrawer.Zoom;
            backLayerPanel.Drawer = _drawer;
            backLayerPanel.TileSelected += BackgroundLayerPanelTileSelected;

            backgroundPreviewer.Drawer = _drawer;
            SelectTilePanel(frontTilePanel);

            _previewTimer = new Timer();
            SetPreviewSpeed();
            _previewTimer.Tick += delegate
            {
                if (playerTrackBar.Value == playerTrackBar.Maximum)
                {
                    playerTrackBar.Value = 0;
                }
                else
                {
                    playerTrackBar.Value++;
                }
            };
        }

        public void Init()
        {
            themeComboBox.Init();
            themeComboBox.SelectedIndex = 0;
        }

        public void ResetScrollPositions()
        {
            frontLayerPanel.AutoScrollPosition = Point.Empty;
            backLayerPanel.AutoScrollPosition = Point.Empty;
            playerTrackBar.Value = 0;
        }

        public void PlayPreview()
        {
            _previewTimer.Start();
            playPauseButton.Text = "Pause";
            playPauseButton.Image = Resources.PauseButton;
        }

        public void PausePreview()
        {
            _previewTimer.Stop();
            playPauseButton.Text = "Play";
            playPauseButton.Image = Resources.PlayButton;
        }

        private void PlayPauseButtonClick(object sender, EventArgs e)
        {
            if (!_previewTimer.Enabled)
            {
                PlayPreview();
            }
            else
            {
                PausePreview();
            }
        }

        private void PreviewSpeedNumericUpDownValueChanged(object sender, EventArgs e)
        {
            SetPreviewSpeed();
        }

        private void SetPreviewSpeed()
        {
            var value = (int)previewSpeedNumericUpDown.Value;
            _previewTimer.Interval = 70 - (value * 20);
        }

        private void PlayerTrackBarValueChanged(object sender, EventArgs e)
        {
            backgroundPreviewer.SetFrame(playerTrackBar.Value);
        }

        private void SetTheme()
        {
            if (_drawer.Theme != null)
            {
                for (var i = 0; i < Palettes.SpritePaletteStart; i++)
                {
                    var palette = _drawer.Theme.Palettes[i];
                    palette.ColorChanged -= palette_ColorsGraphicsChanged;
                    palette.ColorsChanged -= palette_ColorsGraphicsChanged;
                }
            }

            LoadTheme();

            for (var i = 0; i < Palettes.SpritePaletteStart; i++)
            {
                var palette = _drawer.Theme.Palettes[i];
                palette.ColorGraphicsChanged += palette_ColorsGraphicsChanged;
                palette.ColorsGraphicsChanged += palette_ColorsGraphicsChanged;
            }
        }

        private void palette_ColorsGraphicsChanged(object sender, EventArgs e)
        {
            LoadTheme();
        }

        private void LoadTheme()
        {
            _drawer.Theme = Theme;
            frontLayerPanel.Background = backLayerPanel.Background = Theme.Background;
            UpdateTilePanels();
            tilesetPanel.Theme = Theme;

            frontLayerPanel.Refresh();
            backLayerPanel.Refresh();
            backgroundPreviewer.Refresh();
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            ResetScrollPositions();
            SetTheme();
        }

        private void BackgroundLayerPanelTileChanged(object sender, EventArgs e)
        {
            backgroundPreviewer.Invalidate();
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            var properties = TileProperties;

            var value = (int)paletteNumericUpDown.Value;
            properties.PaletteIndex = value / 4;
            properties.SubPaletteIndex = (value & 0x3) * 4;

            TileProperties = properties;
            UpdateTilePanels();
        }

        private void FlipXButtonCheckedChanged(object sender, EventArgs e)
        {
            var properties = TileProperties;
            properties.FlipX();

            TileProperties = properties;
            UpdateTilePanels();
        }

        private void FlipYButtonCheckedChanged(object sender, EventArgs e)
        {
            var properties = TileProperties;
            properties.FlipY();

            TileProperties = properties;
            UpdateTilePanels();
        }

        private void UpdateTilePanels()
        {
            var tileId = TileId;
            var properties = TileProperties.GetByte();

            frontTilePanel.UpdateTile(Theme, tileId, properties);
            backTilePanel.UpdateTile(Theme, tileId, properties);
        }

        private void BackgroundLayerPanelTileSelected(object sender, EventArgs<byte, Tile2bppProperties> e)
        {
            TileId = e.Value1;
            TileProperties = e.Value2;

            paletteNumericUpDown.ValueChanged -= PaletteNumericUpDownValueChanged;
            paletteNumericUpDown.Value = (TileProperties.PaletteIndex * 4) + (TileProperties.SubPaletteIndex / 4);
            paletteNumericUpDown.ValueChanged += PaletteNumericUpDownValueChanged;

            var flipX = (TileProperties.Flip & TileFlip.X) != 0;
            if (flipXButton.Checked != flipX)
            {
                flipXButton.CheckedChanged -= FlipXButtonCheckedChanged;
                flipXButton.Checked = flipX;
                flipXButton.CheckedChanged += FlipXButtonCheckedChanged;
            }

            var flipY = (TileProperties.Flip & TileFlip.Y) != 0;
            if (flipYButton.Checked != flipY)
            {
                flipYButton.CheckedChanged -= FlipYButtonCheckedChanged;
                flipYButton.Checked = flipY;
                flipYButton.CheckedChanged += FlipYButtonCheckedChanged;
            }

            UpdateTilePanels();
            tilesetPanel.SelectedTile = TileId;
        }

        private void TilesetPanelSelectedTileChanged(object sender, EventArgs e)
        {
            TileId = tilesetPanel.SelectedTile;
            UpdateTilePanels();
        }

        private void BackgroundLayerPanelMouseDown(object sender, MouseEventArgs e)
        {
            var front = ((BackgroundPanel)sender).Front;
            ToggleTilesetFrontMode(front);
        }

        private void ToggleTilesetFrontMode(bool front)
        {
            if (tilesetPanel.Front != front)
            {
                tilesetPanel.Front = front;
                SelectTilePanel(front);
            }
        }

        private void SelectTilePanel(bool front)
        {
            if (front)
            {
                DeselectTilePanel(backTilePanel);
                SelectTilePanel(frontTilePanel);
            }
            else
            {
                DeselectTilePanel(frontTilePanel);
                SelectTilePanel(backTilePanel);
            }
        }

        private static void DeselectTilePanel(BackgroundTilePanel panel)
        {
            var borderSize = SystemInformation.Border3DSize;
            panel.SuspendLayout();
            panel.BorderStyle = BorderStyle.None;
            panel.Size = new Size(panel.Width - borderSize.Width * 2, panel.Height - borderSize.Height * 2);
            panel.Location = new Point(panel.Location.X + borderSize.Width, panel.Location.Y + borderSize.Height);
            panel.ResumeLayout();
        }

        private static void SelectTilePanel(BackgroundTilePanel panel)
        {
            var borderSize = SystemInformation.Border3DSize;
            panel.SuspendLayout();
            panel.Location = new Point(panel.Location.X - borderSize.Width, panel.Location.Y - borderSize.Height);
            panel.Size = new Size(panel.Width + borderSize.Width * 2, panel.Height + borderSize.Height * 2);
            panel.BorderStyle = BorderStyle.Fixed3D;
            panel.ResumeLayout();
        }

        private void BackgroundTilePanelMouseDown(object sender, MouseEventArgs e)
        {
            var front = ((BackgroundTilePanel)sender).Front;
            ToggleTilesetFrontMode(front);
        }

        private void ImportGraphicsButtonClick(object sender, EventArgs e)
        {
            if (UITools.ShowImportTilesetGraphicsDialog(Theme.Background.Tileset.GetTiles()))
            {
                LoadTheme();
            }
        }

        private void ExportGraphicsButtonClick(object sender, EventArgs e)
        {
            tilesetPanel.ShowExportImageImage();
        }

        private void ImportLayoutButtonClick(object sender, EventArgs e)
        {
            if (UITools.ShowImportBinaryDataDialog(Theme.Background.Layout.SetBytes))
            {
                LoadTheme();
            }
        }

        private void ExportLayoutButtonClick(object sender, EventArgs e)
        {
            ShowExportBackgroundLayoutDialog();
        }

        private void ShowExportBackgroundLayoutDialog()
        {
            var theme = Theme;
            UITools.ShowExportBinaryDataDialog(theme.Background.Layout.GetBytes, theme.Name + "bg map");
        }
    }
}
