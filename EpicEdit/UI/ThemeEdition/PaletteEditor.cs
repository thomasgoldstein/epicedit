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
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit color palettes.
    /// </summary>
    internal partial class PaletteEditor : UserControl
    {
        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get => themeComboBox.SelectedTheme;
            set => themeComboBox.SelectedTheme = value;
        }

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Palette), "")]
        public Palette Palette
        {
            get => Theme.Palettes[(int)paletteNumericUpDown.Value];
            set
            {
                Theme = value.Theme;
                paletteNumericUpDown.Value = value.Index;
            }
        }

        /// <summary>
        /// Index of the selected color from the palette.
        /// </summary>
        private int _colorIndex;

        /// <summary>
        /// Gets or sets the index of the selected color from the palette.
        /// </summary>
        public int ColorIndex
        {
            get => _colorIndex;
            set
            {
                // Move the focus of the R/G/B numeric fields, to force save the current RGB value if unsaved (happens when text editing the value)
                Focus();

                // Deselect previous panel
                _panels[_colorIndex].BorderStyle = BorderStyle.FixedSingle;

                _colorIndex = value;
                colorPicker.SelectedColor = _panels[_colorIndex].BackColor;
                hexTextBox.Text = colorPicker.SelectedColor.ToHexString();

                // Select new panel
                _panels[_colorIndex].BorderStyle = BorderStyle.Fixed3D;
            }
        }

        /// <summary>
        /// The 16 boxes where the colors of the palette are drawn
        /// </summary>
        private readonly Panel[] _panels = new Panel[Palette.ColorCount];

        /// <summary>
        /// The tool tips associated with the above panels.
        /// </summary>
        private readonly ToolTip[] _toolTips = new ToolTip[Palette.ColorCount];

        /// <summary>
        /// Constructs the editor.
        /// </summary>
        public PaletteEditor()
        {
            InitializeComponent();
            InitColorPanels();
        }

        /// <summary>
        /// Loads the game themes.
        /// </summary>
        public void Init()
        {
            themeComboBox.Init();
            themeComboBox.SelectedIndex = 0;
        }

        private void InitColorPanels()
        {
            for (var i = 0; i < _panels.Length; i++)
            {
                // Calculate the location of the panel on the control
                var x = 12 + ((i % 4) * 32) + ((i % 4) * 8);
                var y = colorPicker.Top + ((i / 4) * 32) + ((i / 4) * 8);

                _panels[i] = new Panel
                {
                    Size = new Size(32, 32),
                    Location = new Point(x, y),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = i
                };
                _panels[i].Click += PaletteEditorClick;
                Controls.Add(_panels[i]);

                _toolTips[i] = new ToolTip()
                {
                    ReshowDelay = 1,
                    InitialDelay = 1
                };
            }

            _panels[_colorIndex].BorderStyle = BorderStyle.Fixed3D;
        }

        /// <summary>
        /// Updates the 16 color panels.
        /// </summary>
        private void UpdatePalette()
        {
            for (var i = 0; i < _panels.Length; i++)
            {
                UpdateColor(i);
            }

            colorPicker.SelectedColor = _panels[_colorIndex].BackColor;
            hexTextBox.Text = colorPicker.SelectedColor.ToHexString();
        }

        /// <summary>
        /// Updates the color panel at the given index.
        /// </summary>
        /// <param name="index">The color panel index.</param>
        private void UpdateColor(int index)
        {
            _panels[index].BackColor = Palette[index];
            SetToolTip(index);
        }

        /// <summary>
        /// Sets the tool tip message for a color.
        /// </summary>
        /// <param name="paletteIndex">The index of the palette to reinitialize the message.</param>
        private void SetToolTip(int paletteIndex)
        {
            var toolTipText = "#" + paletteIndex + Environment.NewLine + Palette[paletteIndex];
            _toolTips[paletteIndex].RemoveAll();
            _toolTips[paletteIndex].SetToolTip(_panels[paletteIndex], toolTipText);
        }

        /// <summary>
        /// Catches the click of one of the panels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaletteEditorClick(object sender, EventArgs e)
        {
            var index = (int)((Control)sender).Tag;
            ColorIndex = index;
        }

        /// <summary>
        /// Resets the selected color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetSelectedButtonClick(object sender, EventArgs e)
        {
            Palette.ResetColor(_colorIndex);
            UpdateColor(_colorIndex);
            colorPicker.SelectedColor = _panels[_colorIndex].BackColor;
        }

        /// <summary>
        /// Resets the palette.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetAllButtonClick(object sender, EventArgs e)
        {
            Palette.Reset();
            UpdatePalette();
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePalette();
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            UpdatePalette();
        }

        private void ColorPickerColorChanged(object sender, EventArgs e)
        {
            // Draw the appropriate color back to the panel and update the tool tip
            var color = colorPicker.SelectedColor;
            Palette[_colorIndex] = color;
            _panels[_colorIndex].BackColor = color;
            SetToolTip(_colorIndex);
            _panels[_colorIndex].Refresh();
            hexTextBox.Text = color.ToHexString();

        }

        private void ImportPalettesButtonClick(object sender, EventArgs e)
        {
            if (UITools.ShowImportBinaryDataDialog(Theme.Palettes.SetBytes, FileDialogFilters.ColorPalette))
            {
                UpdatePalette();
            }
        }

        private void ExportPalettesButtonClick(object sender, EventArgs e)
        {
            UITools.ShowExportBinaryDataDialog(Theme.Palettes.GetBytes, Theme.Name.Trim(), FileDialogFilters.ColorPalette);
        }

        private void HexTextBoxValueChanged(object sender, EventArgs e)
        {
            var color = RomColor.FromHex(hexTextBox.Text);

            Palette[_colorIndex] = color;
            _panels[_colorIndex].BackColor = color;
            SetToolTip(_colorIndex);
            _panels[_colorIndex].Refresh();
            colorPicker.SelectedColor = color;
        }
    }
}
