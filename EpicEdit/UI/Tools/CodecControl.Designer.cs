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

namespace EpicEdit.UI.Tools
{
    partial class CodecControl
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.compressRadioButton = new System.Windows.Forms.RadioButton();
            this.decompressRadioButton = new System.Windows.Forms.RadioButton();
            this.twiceCheckBox = new System.Windows.Forms.CheckBox();
            this.offsetNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.offsetLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.offsetNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // compressRadioButton
            // 
            this.compressRadioButton.Location = new System.Drawing.Point(6, 6);
            this.compressRadioButton.Name = "compressRadioButton";
            this.compressRadioButton.Size = new System.Drawing.Size(104, 24);
            this.compressRadioButton.TabIndex = 0;
            this.compressRadioButton.TabStop = true;
            this.compressRadioButton.Text = "Compress";
            this.compressRadioButton.UseVisualStyleBackColor = true;
            // 
            // decompressRadioButton
            // 
            this.decompressRadioButton.Location = new System.Drawing.Point(84, 6);
            this.decompressRadioButton.Name = "decompressRadioButton";
            this.decompressRadioButton.Size = new System.Drawing.Size(104, 24);
            this.decompressRadioButton.TabIndex = 1;
            this.decompressRadioButton.TabStop = true;
            this.decompressRadioButton.Text = "Decompress";
            this.decompressRadioButton.UseVisualStyleBackColor = true;
            // 
            // twiceCheckBox
            // 
            this.twiceCheckBox.Location = new System.Drawing.Point(6, 33);
            this.twiceCheckBox.Name = "twiceCheckBox";
            this.twiceCheckBox.Size = new System.Drawing.Size(104, 24);
            this.twiceCheckBox.TabIndex = 2;
            this.twiceCheckBox.Text = "Twice";
            this.twiceCheckBox.UseVisualStyleBackColor = true;
            // 
            // offsetNumericUpDown
            // 
            this.offsetNumericUpDown.Hexadecimal = true;
            this.offsetNumericUpDown.Location = new System.Drawing.Point(103, 33);
            this.offsetNumericUpDown.Name = "offsetNumericUpDown";
            this.offsetNumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.offsetNumericUpDown.TabIndex = 4;
            // 
            // offsetLabel
            // 
            this.offsetLabel.Location = new System.Drawing.Point(74, 37);
            this.offsetLabel.Name = "offsetLabel";
            this.offsetLabel.Size = new System.Drawing.Size(23, 23);
            this.offsetLabel.TabIndex = 3;
            this.offsetLabel.Text = "At";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(49, 63);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 5;
            this.browseButton.Text = "Select file...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // CodecControl
            // 
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.offsetLabel);
            this.Controls.Add(this.offsetNumericUpDown);
            this.Controls.Add(this.twiceCheckBox);
            this.Controls.Add(this.decompressRadioButton);
            this.Controls.Add(this.compressRadioButton);
            this.Name = "CodecControl";
            this.Size = new System.Drawing.Size(170, 100);
            ((System.ComponentModel.ISupportInitialize)(this.offsetNumericUpDown)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label offsetLabel;
        private System.Windows.Forms.NumericUpDown offsetNumericUpDown;
        private System.Windows.Forms.CheckBox twiceCheckBox;
        private System.Windows.Forms.RadioButton decompressRadioButton;
        private System.Windows.Forms.RadioButton compressRadioButton;
    }
}
