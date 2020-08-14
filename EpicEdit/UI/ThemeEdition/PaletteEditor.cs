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
            get => this.themeComboBox.SelectedTheme;
            set => this.themeComboBox.SelectedTheme = value;
        }

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Palette), "")]
        public Palette Palette
        {
            get => this.Theme.Palettes[(int)this.paletteNumericUpDown.Value];
            set
            {
                this.Theme = value.Theme;
                this.paletteNumericUpDown.Value = value.Index;
            }
        }

        /// <summary>
        /// Index of the selected color from the palette.
        /// </summary>
        private int colorIndex;

        /// <summary>
        /// Gets or sets the index of the selected color from the palette.
        /// </summary>
        public int ColorIndex
        {
            get => this.colorIndex;
            set
            {
                // Move the focus of the R/G/B numeric fields, to force save the current RGB value if unsaved (happens when text editing the value)
                this.Focus();

                // Deselect previous panel
                this.panels[this.colorIndex].BorderStyle = BorderStyle.FixedSingle;

                this.colorIndex = value;
                this.colorPicker.SelectedColor = this.panels[this.colorIndex].BackColor;

                // Select new panel
                this.panels[this.colorIndex].BorderStyle = BorderStyle.Fixed3D;
            }
        }

        /// <summary>
        /// The 16 boxes where the colors of the palette are drawn
        /// </summary>
        private readonly Panel[] panels = new Panel[Palette.ColorCount];

        /// <summary>
        /// The tool tips associated with the above panels.
        /// </summary>
        private readonly ToolTip[] toolTips = new ToolTip[Palette.ColorCount];

        /// <summary>
        /// Constructs the editor.
        /// </summary>
        public PaletteEditor()
        {
            this.InitializeComponent();
            this.InitColorPanels();
        }

        /// <summary>
        /// Loads the game themes.
        /// </summary>
        public void Init()
        {
            this.themeComboBox.Init();
            this.themeComboBox.SelectedIndex = 0;
        }

        private void InitColorPanels()
        {
            for (int i = 0; i < this.panels.Length; i++)
            {
                // Calculate the location of the panel on the control
                int x = 12 + ((i % 4) * 32) + ((i % 4) * 8);
                int y = this.colorPicker.Top + ((i / 4) * 32) + ((i / 4) * 8);

                this.panels[i] = new Panel
                {
                    Size = new Size(32, 32),
                    Location = new Point(x, y),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = i
                };
                this.panels[i].Click += this.PaletteEditorClick;
                this.Controls.Add(panels[i]);

                this.toolTips[i] = new ToolTip()
                {
                    ReshowDelay = 1,
                    InitialDelay = 1
                };
            }

            this.panels[this.colorIndex].BorderStyle = BorderStyle.Fixed3D;
        }

        /// <summary>
        /// Updates the 16 color panels.
        /// </summary>
        private void UpdatePalette()
        {
            for (int i = 0; i < this.panels.Length; i++)
            {
                this.UpdateColor(i);
            }

            this.colorPicker.SelectedColor = this.panels[this.colorIndex].BackColor;
        }

        /// <summary>
        /// Updates the color panel at the given index.
        /// </summary>
        /// <param name="index">The color panel index.</param>
        private void UpdateColor(int index)
        {
            this.panels[index].BackColor = this.Palette[index];
            this.SetToolTip(index);
        }

        /// <summary>
        /// Sets the tool tip message for a color.
        /// </summary>
        /// <param name="paletteIndex">The index of the palette to reinitialize the message.</param>
        private void SetToolTip(int paletteIndex)
        {
            string toolTipText = "#" + paletteIndex + Environment.NewLine + this.Palette[paletteIndex];
            this.toolTips[paletteIndex].RemoveAll();
            this.toolTips[paletteIndex].SetToolTip(this.panels[paletteIndex], toolTipText);
        }

        /// <summary>
        /// Catches the click of one of the panels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaletteEditorClick(object sender, EventArgs e)
        {
            int index = (int)(sender as Control).Tag;
            this.ColorIndex = index;
        }

        /// <summary>
        /// Resets the selected color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetSelectedButtonClick(object sender, EventArgs e)
        {
            this.Palette.ResetColor(this.colorIndex);
            this.UpdateColor(this.colorIndex);
            this.colorPicker.SelectedColor = this.panels[this.colorIndex].BackColor;
        }

        /// <summary>
        /// Resets the palette.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetAllButtonClick(object sender, EventArgs e)
        {
            this.Palette.Reset();
            this.UpdatePalette();
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdatePalette();
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            // Allow looping from the first palette to the last palette, and vice versa
            if (this.paletteNumericUpDown.Value == this.paletteNumericUpDown.Maximum)
            {
                this.paletteNumericUpDown.Value = this.paletteNumericUpDown.Minimum + 1;
            }
            else if (this.paletteNumericUpDown.Value == this.paletteNumericUpDown.Minimum)
            {
                this.paletteNumericUpDown.Value = this.paletteNumericUpDown.Maximum - 1;
            }

            this.UpdatePalette();
        }

        private void ColorPickerColorChanged(object sender, EventArgs e)
        {
            // Draw the appropriate color back to the panel and update the tool tip
            RomColor color = this.colorPicker.SelectedColor;
            this.Palette[this.colorIndex] = color;
            this.panels[this.colorIndex].BackColor = color;
            this.SetToolTip(this.colorIndex);
            this.panels[this.colorIndex].Refresh();
        }

        private void ImportPalettesButtonClick(object sender, EventArgs e)
        {
            if (UITools.ShowImportBinaryDataDialog(this.Theme.Palettes.SetBytes, FileDialogFilters.ColorPalette))
            {
                this.UpdatePalette();
            }
        }

        private void ExportPalettesButtonClick(object sender, EventArgs e)
        {
            UITools.ShowExportBinaryDataDialog(this.Theme.Palettes.GetBytes, this.Theme.Name.Trim(), FileDialogFilters.ColorPalette);
        }
    }
}
