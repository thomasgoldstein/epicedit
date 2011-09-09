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

        /// <summary>
        /// Draws the colors of the 16 colors to the different panels and sets the tooltips.
        /// </summary>
        private void DrawPalette()
        {
            for (int i = 0; i < this.panels.Length; i++)
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
                    this.Controls.Add(panels[i]);

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
            this.toolTips[paletteIndex].SetToolTip(this.panels[paletteIndex], string.Format("#{0}\r\n{1}", paletteIndex.ToString(), this.palette[paletteIndex].ToString()));
        }

        /// <summary>
        /// Catches the click of one of the panels.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PaletteEditorClick(object sender, EventArgs e)
        {
            for (int i = 0; i < this.panels.Length; i++)
            {
                if (this.panels[i] != null && this.panels[i].BorderStyle != BorderStyle.FixedSingle)
                {
                    // Undo the selection, redraw the border
                    this.panels[i].BorderStyle = BorderStyle.FixedSingle;
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

            this.DrawPalette();

            this.colorPicker.SetColor(this.panels[this.selectedColor].BackColor);
            this.panels[this.selectedColor].BorderStyle = BorderStyle.Fixed3D;
        }

        public void SetColorIndex(int index)
        {
            this.panels[this.selectedColor].BorderStyle = BorderStyle.FixedSingle;
            this.selectedColor = index;
            this.colorPicker.SetColor(this.panels[index].BackColor);
            this.panels[this.selectedColor].BorderStyle = BorderStyle.Fixed3D;
        }

        /// <summary>
        /// Catches the user setting the color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetColorButtonClick(object sender, EventArgs e)
        {
            // Draw the appropriate color back to the panel and update the tooltip
            RomColor color = this.colorPicker.SelectedColor;
            this.palette[this.selectedColor] = color;
            this.panels[this.selectedColor].BackColor = color;
            this.SetToolTip(this.selectedColor);

            this.palette.Modified = true;
            this.ColorChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Resets the palette.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButtonClick(object sender, EventArgs e)
        {
            this.palette.Reset();
            this.SetPalette(this.palette);

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
            Theme selectedTheme = this.themeComboBox.SelectedItem as Theme;
            this.SetPalette(selectedTheme.Palettes[(int)this.paletteNumericUpDown.Value]);
        }
    }
}
