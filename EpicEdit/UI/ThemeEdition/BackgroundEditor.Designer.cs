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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                this.drawer.Dispose();
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
            this.previewGroupBox = new System.Windows.Forms.GroupBox();
            this.frontLayerPanel = new EpicEdit.UI.ThemeEdition.BackgroundEditor.BackgroundPanel();
            this.frontLayerGroupBox = new System.Windows.Forms.GroupBox();
            this.backLayerGroupBox = new System.Windows.Forms.GroupBox();
            this.backLayerPanel = new EpicEdit.UI.ThemeEdition.BackgroundEditor.BackgroundPanel();
            this.previewGroupBox.SuspendLayout();
            this.frontLayerGroupBox.SuspendLayout();
            this.backLayerGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // backgroundPreviewer
            // 
            this.backgroundPreviewer.Drawer = null;
            this.backgroundPreviewer.Location = new System.Drawing.Point(7, 46);
            this.backgroundPreviewer.Name = "backgroundPreviewer";
            this.backgroundPreviewer.Size = new System.Drawing.Size(512, 48);
            this.backgroundPreviewer.TabIndex = 0;
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(3, 3);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(121, 21);
            this.themeComboBox.TabIndex = 1;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Location = new System.Drawing.Point(7, 19);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(75, 23);
            this.playPauseButton.TabIndex = 2;
            this.playPauseButton.Text = "Play";
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.PlayPauseButtonClick);
            // 
            // previewGroupBox
            // 
            this.previewGroupBox.Controls.Add(this.playPauseButton);
            this.previewGroupBox.Controls.Add(this.backgroundPreviewer);
            this.previewGroupBox.Location = new System.Drawing.Point(3, 228);
            this.previewGroupBox.Name = "previewGroupBox";
            this.previewGroupBox.Size = new System.Drawing.Size(526, 107);
            this.previewGroupBox.TabIndex = 3;
            this.previewGroupBox.TabStop = false;
            this.previewGroupBox.Text = "Preview";
            // 
            // frontLayerPanel
            // 
            this.frontLayerPanel.AutoScroll = true;
            this.frontLayerPanel.AutoScrollMinSize = new System.Drawing.Size(2048, 48);
            this.frontLayerPanel.Front = true;
            this.frontLayerPanel.Location = new System.Drawing.Point(7, 19);
            this.frontLayerPanel.Name = "frontLayerPanel";
            this.frontLayerPanel.Size = new System.Drawing.Size(512, 48);
            this.frontLayerPanel.TabIndex = 4;
            this.frontLayerPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.BackgroundLayerPanelScroll);
            // 
            // frontLayerGroupBox
            // 
            this.frontLayerGroupBox.Controls.Add(this.frontLayerPanel);
            this.frontLayerGroupBox.Location = new System.Drawing.Point(3, 30);
            this.frontLayerGroupBox.Name = "frontLayerGroupBox";
            this.frontLayerGroupBox.Size = new System.Drawing.Size(526, 93);
            this.frontLayerGroupBox.TabIndex = 5;
            this.frontLayerGroupBox.TabStop = false;
            this.frontLayerGroupBox.Text = "Front layer";
            // 
            // backLayerGroupBox
            // 
            this.backLayerGroupBox.Controls.Add(this.backLayerPanel);
            this.backLayerGroupBox.Location = new System.Drawing.Point(3, 129);
            this.backLayerGroupBox.Name = "backLayerGroupBox";
            this.backLayerGroupBox.Size = new System.Drawing.Size(526, 93);
            this.backLayerGroupBox.TabIndex = 6;
            this.backLayerGroupBox.TabStop = false;
            this.backLayerGroupBox.Text = "Back layer";
            // 
            // backLayerPanel
            // 
            this.backLayerPanel.AutoScroll = true;
            this.backLayerPanel.AutoScrollMinSize = new System.Drawing.Size(1024, 48);
            this.backLayerPanel.Front = false;
            this.backLayerPanel.Location = new System.Drawing.Point(7, 19);
            this.backLayerPanel.Name = "backLayerPanel";
            this.backLayerPanel.Size = new System.Drawing.Size(512, 48);
            this.backLayerPanel.TabIndex = 4;
            this.backLayerPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.BackgroundLayerPanelScroll);
            // 
            // BackgroundEditor
            // 
            this.Controls.Add(this.backLayerGroupBox);
            this.Controls.Add(this.frontLayerGroupBox);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.previewGroupBox);
            this.Name = "BackgroundEditor";
            this.Size = new System.Drawing.Size(532, 340);
            this.previewGroupBox.ResumeLayout(false);
            this.frontLayerGroupBox.ResumeLayout(false);
            this.backLayerGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private EpicEdit.UI.ThemeEdition.BackgroundEditor.BackgroundPanel backLayerPanel;
        private System.Windows.Forms.GroupBox backLayerGroupBox;
        private System.Windows.Forms.GroupBox frontLayerGroupBox;
        private EpicEdit.UI.ThemeEdition.BackgroundEditor.BackgroundPanel frontLayerPanel;
        private System.Windows.Forms.GroupBox previewGroupBox;
        private System.Windows.Forms.Button playPauseButton;
        private System.Windows.Forms.ComboBox themeComboBox;
        private EpicEdit.UI.ThemeEdition.BackgroundPreviewer backgroundPreviewer;
    }
}
