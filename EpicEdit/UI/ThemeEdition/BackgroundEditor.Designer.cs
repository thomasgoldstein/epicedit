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

namespace EpicEdit.UI.ThemeEdition
{
    partial class BackgroundEditor
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
            this.backgroundPreviewer = new EpicEdit.UI.ThemeEdition.BackgroundPreviewer();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.playPauseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // backgroundPreviewer
            // 
            this.backgroundPreviewer.Location = new System.Drawing.Point(5, 30);
            this.backgroundPreviewer.Name = "backgroundPreviewer";
            this.backgroundPreviewer.Size = new System.Drawing.Size(512, 48);
            this.backgroundPreviewer.TabIndex = 0;
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(4, 3);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(121, 21);
            this.themeComboBox.TabIndex = 1;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Location = new System.Drawing.Point(131, 3);
            this.playPauseButton.Name = "playStopButton";
            this.playPauseButton.Size = new System.Drawing.Size(75, 23);
            this.playPauseButton.TabIndex = 2;
            this.playPauseButton.Text = "Play";
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.PlayPauseButtonClick);
            // 
            // BackgroundEditor
            // 
            this.Controls.Add(this.playPauseButton);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.backgroundPreviewer);
            this.Name = "BackgroundEditor";
            this.Size = new System.Drawing.Size(520, 100);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Button playPauseButton;
        private System.Windows.Forms.ComboBox themeComboBox;
        private EpicEdit.UI.ThemeEdition.BackgroundPreviewer backgroundPreviewer;
    }
}
