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

namespace EpicEdit.UI.SettingEdition
{
    partial class SuffixedNameControl
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private EpicEdit.UI.Tools.TextItemComboBox nameComboBox;
        private System.Windows.Forms.TextBox suffixTextBox;
        
        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
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
            this.suffixTextBox = new System.Windows.Forms.TextBox();
            this.nameComboBox = new EpicEdit.UI.Tools.TextItemComboBox();
            this.SuspendLayout();
            // 
            // suffixTextBox
            // 
            this.suffixTextBox.Location = new System.Drawing.Point(97, 0);
            this.suffixTextBox.Name = "suffixTextBox";
            this.suffixTextBox.Size = new System.Drawing.Size(21, 20);
            this.suffixTextBox.TabIndex = 1;
            this.suffixTextBox.TextChanged += new System.EventHandler(this.SuffixTextBoxTextChanged);
            // 
            // nameComboBox
            // 
            this.nameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.nameComboBox.FormattingEnabled = true;
            this.nameComboBox.Location = new System.Drawing.Point(0, 0);
            this.nameComboBox.Name = "nameComboBox";
            this.nameComboBox.Size = new System.Drawing.Size(97, 21);
            this.nameComboBox.TabIndex = 0;
            this.nameComboBox.SelectedIndexChanged += new System.EventHandler(this.NameComboBoxSelectedIndexChanged);
            // 
            // SuffixedNameControl
            // 
            this.Controls.Add(this.nameComboBox);
            this.Controls.Add(this.suffixTextBox);
            this.Name = "SuffixedNameControl";
            this.Size = new System.Drawing.Size(118, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
