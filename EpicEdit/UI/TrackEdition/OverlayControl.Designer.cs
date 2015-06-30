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

namespace EpicEdit.UI.TrackEdition
{
    partial class OverlayControl
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

                if (this.drawer != null)
                {
                    this.drawer.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.deleteAllButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.tilesetPanel = new EpicEdit.UI.TrackEdition.OverlayControl.OverlayPanel();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.tileCountLabel = new System.Windows.Forms.Label();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // deleteAllButton
            // 
            this.deleteAllButton.Image = global::EpicEdit.Properties.Resources.NukeButton;
            this.deleteAllButton.Location = new System.Drawing.Point(103, 3);
            this.deleteAllButton.Name = "deleteAllButton";
            this.deleteAllButton.Size = new System.Drawing.Size(24, 24);
            this.deleteAllButton.TabIndex = 2;
            this.buttonToolTip.SetToolTip(this.deleteAllButton, "Delete all");
            this.deleteAllButton.UseVisualStyleBackColor = true;
            this.deleteAllButton.Click += new System.EventHandler(this.DeleteAllButtonClick);
            // 
            // deleteButton
            // 
            this.deleteButton.Enabled = false;
            this.deleteButton.Image = global::EpicEdit.Properties.Resources.DeleteButton;
            this.deleteButton.Location = new System.Drawing.Point(73, 3);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(24, 24);
            this.deleteButton.TabIndex = 1;
            this.buttonToolTip.SetToolTip(this.deleteButton, "Delete selected");
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // tilesetPanel
            // 
            this.tilesetPanel.Location = new System.Drawing.Point(0, 0);
            this.tilesetPanel.Name = "tilesetPanel";
            this.tilesetPanel.Size = new System.Drawing.Size(128, 512);
            this.tilesetPanel.TabIndex = 0;
            this.tilesetPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilesetPanelMouseDown);
            this.tilesetPanel.MouseLeave += new System.EventHandler(this.TilesetPanelMouseLeave);
            this.tilesetPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TilesetPanelMouseMove);
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.tileCountLabel);
            this.buttonPanel.Controls.Add(this.deleteButton);
            this.buttonPanel.Controls.Add(this.deleteAllButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 516);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(130, 32);
            this.buttonPanel.TabIndex = 1;
            // 
            // tileCountLabel
            // 
            this.tileCountLabel.Location = new System.Drawing.Point(3, 8);
            this.tileCountLabel.Name = "tileCountLabel";
            this.tileCountLabel.Size = new System.Drawing.Size(45, 23);
            this.tileCountLabel.TabIndex = 0;
            this.tileCountLabel.Text = "...";
            // 
            // OverlayControl
            // 
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.tilesetPanel);
            this.Name = "OverlayControl";
            this.Size = new System.Drawing.Size(130, 548);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label tileCountLabel;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private EpicEdit.UI.TrackEdition.OverlayControl.OverlayPanel tilesetPanel;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button deleteAllButton;
    }
}
