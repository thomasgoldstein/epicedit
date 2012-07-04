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
using System.IO;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;
using EpicEdit.UI;
using EpicEdit.UI.Tools;

namespace EpicEdit.Rom.ThemeEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit color palettes.
    /// </summary>
    internal partial class PaletteEditor : UserControl
    {
        [Browsable(true)]
        public event EventHandler<EventArgs> ColorChanged;

        [Browsable(true)]
        public event EventHandler<EventArgs> ColorsChanged;

        [Browsable(true)]
        public event EventHandler<EventArgs> PalettesChanged;

        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get { return this.themeComboBox.SelectedItem as Theme; }
            set { this.themeComboBox.SelectedItem = value; }
        }

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Palette), "")]
        public Palette Palette
        {
            get { return this.Theme.Palettes[(int)this.paletteNumericUpDown.Value]; }
            set
            {
                this.Theme = value.Theme;
                this.paletteNumericUpDown.Value = value.Index;
            }
        }

        /// <summary>
        /// Index of the selected color from the palette.
        /// </summary>
        private int colorIndex = 0;

        /// <summary>
        /// Gets or sets the index of the selected color from the palette.
        /// </summary>
        public int ColorIndex
        {
            get { return this.colorIndex; }
            set
            {
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
        private Panel[] panels = new Panel[Palette.ColorCount];

        /// <summary>
        /// The tool tips associated with the above panels.
        /// </summary>
        private ToolTip[] toolTips = new ToolTip[Palette.ColorCount];

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

            this.ColorChanged(this, EventArgs.Empty);
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

            this.ColorsChanged(this, EventArgs.Empty);
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdatePalette();
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
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

            this.Palette.Modified = true;
            this.ColorChanged(sender, e);
        }

        private void ImportPalettesButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter =
                    "Color palettes (*.pal)|*.pal|" +
                    "All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.ImportPalettes(ofd.FileName);
                }
            }
        }

        private void ImportPalettes(string filePath)
        {
            try
            {
                byte[] data = File.ReadAllBytes(filePath);
                this.Theme.Palettes.Load(data);
                this.UpdatePalette();
                this.PalettesChanged(this, EventArgs.Empty);
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }
        
        private void ExportPalettesButtonClick(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter =
                    "Color palettes (*.pal)|*.pal|" +
                    "All files (*.*)|*.*";

                sfd.FileName = UITools.SanitizeFileName(this.Theme.Name).Trim();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    this.ExportPalettes(sfd.FileName);
                }
            }
        }

        private void ExportPalettes(string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, this.Theme.Palettes.GetBytes());
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }
    }
}
