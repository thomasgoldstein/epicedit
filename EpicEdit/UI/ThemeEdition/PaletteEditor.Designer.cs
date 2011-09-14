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
            this.setColorButton = new System.Windows.Forms.Button();
            this.resetSelectedButton = new System.Windows.Forms.Button();
            this.colorPicker = new EpicEdit.UI.ThemeEdition.ColorPicker();
            this.paletteNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.resetAllButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // setColorButton
            // 
            this.setColorButton.Location = new System.Drawing.Point(339, 204);
            this.setColorButton.Name = "setColorButton";
            this.setColorButton.Size = new System.Drawing.Size(75, 23);
            this.setColorButton.TabIndex = 3;
            this.setColorButton.Text = "Set Color";
            this.setColorButton.UseVisualStyleBackColor = true;
            this.setColorButton.Click += new System.EventHandler(this.SetColorButtonClick);
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
            this.colorPicker.Size = new System.Drawing.Size(249, 153);
            this.colorPicker.TabIndex = 2;
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
            // PaletteEditor
            // 
            this.Controls.Add(this.resetAllButton);
            this.Controls.Add(this.paletteNumericUpDown);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.setColorButton);
            this.Controls.Add(this.resetSelectedButton);
            this.Controls.Add(this.colorPicker);
            this.Name = "PaletteEditor";
            this.Size = new System.Drawing.Size(420, 230);
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Button resetAllButton;
        private System.Windows.Forms.NumericUpDown paletteNumericUpDown;
        private System.Windows.Forms.ComboBox themeComboBox;

        #endregion

        private EpicEdit.UI.ThemeEdition.ColorPicker colorPicker;
        private System.Windows.Forms.Button setColorButton;
        private System.Windows.Forms.Button resetSelectedButton;
    }
}
