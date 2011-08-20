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
            this.resetButton = new System.Windows.Forms.Button();
            this.colorPicker = new EpicEdit.UI.ThemeEdition.ColorPicker();
            this.paletteGroupBox = new System.Windows.Forms.GroupBox();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.paletteGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // setColorButton
            // 
            this.setColorButton.Location = new System.Drawing.Point(344, 213);
            this.setColorButton.Name = "setColorButton";
            this.setColorButton.Size = new System.Drawing.Size(75, 23);
            this.setColorButton.TabIndex = 1;
            this.setColorButton.Text = "Set Color";
            this.setColorButton.UseVisualStyleBackColor = true;
            this.setColorButton.Click += new System.EventHandler(this.SetColorButtonClick);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(11, 213);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 2;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // colorPicker
            // 
            this.colorPicker.Location = new System.Drawing.Point(174, 54);
            this.colorPicker.Name = "colorPicker";
            this.colorPicker.Size = new System.Drawing.Size(249, 153);
            this.colorPicker.TabIndex = 0;
            // 
            // paletteGroupBox
            // 
            this.paletteGroupBox.Controls.Add(this.themeComboBox);
            this.paletteGroupBox.Controls.Add(this.resetButton);
            this.paletteGroupBox.Controls.Add(this.colorPicker);
            this.paletteGroupBox.Controls.Add(this.setColorButton);
            this.paletteGroupBox.Location = new System.Drawing.Point(0, 0);
            this.paletteGroupBox.Name = "paletteGroupBox";
            this.paletteGroupBox.Size = new System.Drawing.Size(430, 250);
            this.paletteGroupBox.TabIndex = 3;
            this.paletteGroupBox.TabStop = false;
            this.paletteGroupBox.Text = "Color palettes";
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(11, 20);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(121, 21);
            this.themeComboBox.TabIndex = 3;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // PaletteEditor
            // 
            this.Controls.Add(this.paletteGroupBox);
            this.Name = "PaletteEditor";
            this.Size = new System.Drawing.Size(430, 250);
            this.paletteGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.GroupBox paletteGroupBox;

        #endregion

        private EpicEdit.UI.ThemeEdition.ColorPicker colorPicker;
        private System.Windows.Forms.Button setColorButton;
        private System.Windows.Forms.Button resetButton;
    }
}
