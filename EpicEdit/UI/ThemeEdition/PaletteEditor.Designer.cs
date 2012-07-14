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

namespace EpicEdit.Rom.ThemeEdition
{
    partial class PaletteEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.resetSelectedButton = new System.Windows.Forms.Button();
            this.colorPicker = new EpicEdit.UI.ThemeEdition.ColorPicker();
            this.paletteNumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.resetAllButton = new System.Windows.Forms.Button();
            this.exportPalettesButton = new System.Windows.Forms.Button();
            this.importPalettesButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // resetSelectedButton
            // 
            this.resetSelectedButton.Location = new System.Drawing.Point(6, 204);
            this.resetSelectedButton.Name = "resetSelectedButton";
            this.resetSelectedButton.Size = new System.Drawing.Size(100, 23);
            this.resetSelectedButton.TabIndex = 4;
            this.resetSelectedButton.Text = "Reset selected";
            this.resetSelectedButton.UseVisualStyleBackColor = true;
            this.resetSelectedButton.Click += new System.EventHandler(this.ResetSelectedButtonClick);
            // 
            // colorPicker
            // 
            this.colorPicker.Location = new System.Drawing.Point(169, 40);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Size = new System.Drawing.Size(189, 153);
            this.colorPicker.TabIndex = 2;
            this.colorPicker.ColorChanged += new System.EventHandler<System.EventArgs>(this.ColorPickerColorChanged);
            // 
            // paletteNumericUpDown
            // 
            this.paletteNumericUpDown.Location = new System.Drawing.Point(134, 6);
            this.paletteNumericUpDown.Maximum = new decimal(new int[] {
                                    15,
                                    0,
                                    0,
                                    0});
            this.paletteNumericUpDown.Name = "paletteNumericUpDown";
            this.paletteNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.paletteNumericUpDown.TabIndex = 1;
            this.paletteNumericUpDown.ValueChanged += new System.EventHandler(this.PaletteNumericUpDownValueChanged);
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(6, 6);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(121, 21);
            this.themeComboBox.TabIndex = 0;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // resetAllButton
            // 
            this.resetAllButton.Location = new System.Drawing.Point(112, 204);
            this.resetAllButton.Name = "resetAllButton";
            this.resetAllButton.Size = new System.Drawing.Size(75, 23);
            this.resetAllButton.TabIndex = 5;
            this.resetAllButton.Text = "Reset all";
            this.resetAllButton.UseVisualStyleBackColor = true;
            this.resetAllButton.Click += new System.EventHandler(this.ResetAllButtonClick);
            // 
            // exportPalettesButton
            // 
            this.exportPalettesButton.Image = global::EpicEdit.Properties.Resources.ExportButton;
            this.exportPalettesButton.Location = new System.Drawing.Point(335, 203);
            this.exportPalettesButton.Name = "exportPalettesButton";
            this.exportPalettesButton.Size = new System.Drawing.Size(24, 24);
            this.exportPalettesButton.TabIndex = 7;
            this.buttonToolTip.SetToolTip(this.exportPalettesButton, "Export all 16 theme palettes");
            this.exportPalettesButton.UseVisualStyleBackColor = true;
            this.exportPalettesButton.Click += new System.EventHandler(this.ExportPalettesButtonClick);
            // 
            // importPalettesButton
            // 
            this.importPalettesButton.Image = global::EpicEdit.Properties.Resources.ImportButton;
            this.importPalettesButton.Location = new System.Drawing.Point(305, 203);
            this.importPalettesButton.Name = "importPalettesButton";
            this.importPalettesButton.Size = new System.Drawing.Size(24, 24);
            this.importPalettesButton.TabIndex = 6;
            this.buttonToolTip.SetToolTip(this.importPalettesButton, "Import all 16 theme palettes");
            this.importPalettesButton.UseVisualStyleBackColor = true;
            this.importPalettesButton.Click += new System.EventHandler(this.ImportPalettesButtonClick);
            // 
            // PaletteEditor
            // 
            this.Controls.Add(this.exportPalettesButton);
            this.Controls.Add(this.importPalettesButton);
            this.Controls.Add(this.resetAllButton);
            this.Controls.Add(this.paletteNumericUpDown);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.resetSelectedButton);
            this.Controls.Add(this.colorPicker);
            this.Name = "PaletteEditor";
            this.Size = new System.Drawing.Size(360, 230);
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button importPalettesButton;
        private System.Windows.Forms.Button exportPalettesButton;
        private System.Windows.Forms.Button resetAllButton;
        private EpicEdit.UI.Tools.EpicNumericUpDown paletteNumericUpDown;
        private System.Windows.Forms.ComboBox themeComboBox;

        #endregion

        private EpicEdit.UI.ThemeEdition.ColorPicker colorPicker;
        private System.Windows.Forms.Button resetSelectedButton;
    }
}
