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
using System.Drawing;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;
using EpicEdit.UI;
using EpicEdit.UI.Tools;

namespace EpicEdit.Rom.ThemeEdition
{
    public partial class PaletteEditor : UserControl
    {
        /// <summary>
        /// The 16 boxes where the colors of the palette are drawn
        /// </summary>
        private Panel[] panels = new Panel[16];

        /// <summary>
        /// The tooltips associated with the above panels.
        /// </summary>
        private ToolTip[] toolTips = new ToolTip[16];

        /// <summary>
        /// Index of the selected color from the palette
        /// </summary>
        private int selectedColor = 0;

        /// <summary>
        /// The palette object passed into the constructor
        /// </summary>
        private Palette palette;

        /// <summary>
        /// Used to reset the palette with the reset button
        /// </summary>
        private byte[] paletteBackup;

        /// <summary>
        /// Constructs the editor using the default palette (all black).
        /// </summary>
        public PaletteEditor() : this(new Palette()) { }

        /// <summary>
        /// Constructs the editor using the passed in palette.
        /// </summary>
        /// <param name="palette">The palette object coming from the ROM.</param>
        public PaletteEditor(Palette palette)
        {
            this.InitializeComponent();

            this.SetPalette(palette);
        }

        public void InitOnRomLoad()
        {
            this.themeComboBox.BeginUpdate();
            this.themeComboBox.Items.Clear();
            foreach (Theme theme in MainForm.SmkGame.Themes)
            {
                this.themeComboBox.Items.Add(theme);
            }
            this.themeComboBox.EndUpdate();
            this.themeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Draws the colors of the 16 colors to the different panels and sets the tooltips.
        /// </summary>
        private void DrawPalette()
        {
            for (int i = 0; i < 16; i++)
            {
                if (this.panels[i] == null)
                {
                    // Calculate the location of the panel on the control
                    int x = 12 + ((i % 4) * 32) + ((i % 4) * 8);
                    int y = this.colorPicker.Top + ((i / 4) * 32) + ((i / 4) * 8);

                    this.panels[i] = new Panel
                    {
                        Size = new Size(32, 32),
                        Location = new Point(x, y),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = palette[i],
                        Tag = i
                    };
                    this.panels[i].Click += new EventHandler(this.PaletteEditorClick);
                    this.paletteGroupBox.Controls.Add(panels[i]);

                    this.toolTips[i] = new ToolTip()
                    {
                        ReshowDelay = 1,
                        InitialDelay = 1
                    };
                    this.SetToolTip(i);

                    UITools.FixToolTip(this.panels[i], this.toolTips[i]);
                }
                else
                {
                    this.panels[i].BorderStyle = BorderStyle.FixedSingle;
                    this.panels[i].BackColor = this.palette[i];

                    this.SetToolTip(i);
                }
            }

            this.panels[this.selectedColor].BorderStyle = BorderStyle.Fixed3D;
        }

        /// <summary>
        /// Sets the tooltip message for a color.
        /// </summary>
        /// <param name="paletteIndex">The index of the palette to reinitialize the message.</param>
        private void SetToolTip(int paletteIndex)
        {
            this.toolTips[paletteIndex].RemoveAll();
            this.toolTips[paletteIndex].SetToolTip(this.panels[paletteIndex], String.Format("#{0}\r\n{1}", paletteIndex.ToString(), palette[paletteIndex].ToString()));
        }

        /// <summary>
        /// Catches the click of one of the panels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaletteEditorClick(object sender, EventArgs e)
        {
            for (int x = 0; x < 16; x++)
            {
                if (this.panels[x] != null && this.panels[x].BorderStyle != BorderStyle.FixedSingle)
                {
                    // Undo the selection, redraw the border
                    this.panels[x].BorderStyle = BorderStyle.FixedSingle;
                }
            }

            if (sender is Panel)
            {
                // Select the panel
                Panel panel = sender as Panel;
                panel.BorderStyle = BorderStyle.Fixed3D;
                this.colorPicker.SetColor(panel.BackColor);
                this.selectedColor = (int)panel.Tag;
            }
        }

        /// <summary>
        /// Sets the palette after the palette editor has been initialized.
        /// </summary>
        /// <param name="palette">The new palette object.</param>
        public void SetPalette(Palette palette)
        {
            this.palette = palette;
            this.paletteBackup = palette.GetBytes();

            this.DrawPalette();

            this.colorPicker.SetColor(this.panels[this.selectedColor].BackColor);
            this.panels[this.selectedColor].BorderStyle = BorderStyle.Fixed3D;
        }

        /// <summary>
        /// Returns the palette with modified colors (if any were changed).
        /// </summary>
        /// <returns></returns>
        public Palette GetPalette()
        {
            return this.palette;
        }

        /// <summary>
        /// Catches the user setting the color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetColorButtonClick(object sender, EventArgs e)
        {
            // Draw the appropriate color back to the panel and update the tooltip
            RomColor color = this.colorPicker.GetColor();
            this.palette[this.selectedColor] = color;
            this.panels[this.selectedColor].BackColor = color;
            this.SetToolTip(this.selectedColor);

            MainForm.SmkGame.Modified = true;
        }

        /// <summary>
        /// Resets the palette.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButtonClick(object sender, EventArgs e)
        {
            Palette palette = new Palette(this.paletteBackup);
            this.SetPalette(palette);
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
            Theme selectedTheme = this.themeComboBox.SelectedItem as Theme;
            this.SetPalette(selectedTheme.Palettes[(int)this.paletteNumericUpDown.Value]);
        }
    }
}
