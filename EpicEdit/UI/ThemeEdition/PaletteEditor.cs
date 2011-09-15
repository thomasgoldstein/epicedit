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

using EpicEdit.Rom.Tracks;
using EpicEdit.UI;
using EpicEdit.UI.Tools;

namespace EpicEdit.Rom.ThemeEdition
{
    public partial class PaletteEditor : UserControl
    {
        [Browsable(true)]
        public event EventHandler<EventArgs> ColorChanged;

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get { return this.themeComboBox.SelectedItem as Theme; }
            set { this.themeComboBox.SelectedItem = value; }
        }

        [Browsable(false), DefaultValue(typeof(Palette), "")]
        public Palette Palette
        {
            get { return this.palette; }
            private set
            {
                this.palette = value;
                this.UpdatePalette();
                this.colorPicker.SetColor(this.panels[this.selectedColor].BackColor);
            }
        }

        /// <summary>
        /// The 16 boxes where the colors of the palette are drawn
        /// </summary>
        private Panel[] panels = new Panel[Palette.ColorCount];

        /// <summary>
        /// The tooltips associated with the above panels.
        /// </summary>
        private ToolTip[] toolTips = new ToolTip[Palette.ColorCount];

        /// <summary>
        /// Index of the selected color from the palette.
        /// </summary>
        private int selectedColor = 0;

        /// <summary>
        /// The current color palette.
        /// </summary>
        private Palette palette;

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
            this.themeComboBox.BeginUpdate();
            this.themeComboBox.Items.Clear();
            foreach (Theme theme in Context.Game.Themes)
            {
                this.themeComboBox.Items.Add(theme);
            }
            this.themeComboBox.EndUpdate();
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

                UITools.FixToolTip(this.panels[i], this.toolTips[i]);
            }

            this.panels[this.selectedColor].BorderStyle = BorderStyle.Fixed3D;
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
        }

        /// <summary>
        /// Updates the color panel at the given index.
        /// </summary>
        /// <param name="index">The color panel index.</param>
        private void UpdateColor(int index)
        {
            this.panels[index].BackColor = this.palette[index];
            this.SetToolTip(index);
        }

        /// <summary>
        /// Sets the tooltip message for a color.
        /// </summary>
        /// <param name="paletteIndex">The index of the palette to reinitialize the message.</param>
        private void SetToolTip(int paletteIndex)
        {
            string toolTipText = string.Format("#{0}" + Environment.NewLine + "{1}",
                                               paletteIndex, this.palette[paletteIndex]);

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
            this.SetColorIndex(index);
        }

        /// <summary>
        /// Sets the palette after the palette editor has been initialized.
        /// </summary>
        /// <param name="palette">The new palette object.</param>
        public void SetPalette(Palette palette)
        {
            this.paletteNumericUpDown.Value = this.Theme.Palettes.IndexOf(palette);
        }

        public void SetColorIndex(int index)
        {
            // Deselect previous panel
            this.panels[this.selectedColor].BorderStyle = BorderStyle.FixedSingle;

            this.selectedColor = index;
            this.colorPicker.SetColor(this.panels[index].BackColor);

            // Select new panel
            this.panels[this.selectedColor].BorderStyle = BorderStyle.Fixed3D;
        }

        /// <summary>
        /// Resets the selected color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetSelectedButtonClick(object sender, EventArgs e)
        {
            this.palette.ResetColor(this.selectedColor);
            this.UpdateColor(this.selectedColor);
            this.colorPicker.SetColor(this.panels[this.selectedColor].BackColor);

            this.ColorChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Resets the palette.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetAllButtonClick(object sender, EventArgs e)
        {
            this.palette.Reset();
            this.Palette = this.palette;

            this.ColorChanged(this, EventArgs.Empty);
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.SetCurrentPalette();
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            this.SetCurrentPalette();
        }

        private void SetCurrentPalette()
        {
            this.Palette = this.Theme.Palettes[(int)this.paletteNumericUpDown.Value];
        }
        
        private void ColorPickerColorChanged(object sender, EventArgs e)
        {
            // Draw the appropriate color back to the panel and update the tooltip
            RomColor color = this.colorPicker.SelectedColor;
            this.palette[this.selectedColor] = color;
            this.panels[this.selectedColor].BackColor = color;
            this.SetToolTip(this.selectedColor);

            this.palette.Modified = true;
            this.ColorChanged(sender, e);
        }
    }
}
