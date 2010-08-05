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

				if (this.overlayDrawer != null)
				{
					this.overlayDrawer.Dispose();
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
			this.deleteAllButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.overlayPanel = new EpicEdit.UI.Tools.EpicPanel();
			this.deletePanel = new System.Windows.Forms.Panel();
			this.deletePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// deleteAllButton
			// 
			this.deleteAllButton.Image = global::EpicEdit.Properties.Resources.button_nuke;
			this.deleteAllButton.Location = new System.Drawing.Point(103, 3);
			this.deleteAllButton.Name = "deleteAllButton";
			this.deleteAllButton.Size = new System.Drawing.Size(24, 24);
			this.deleteAllButton.TabIndex = 0;
			this.deleteAllButton.UseVisualStyleBackColor = true;
			this.deleteAllButton.Click += new System.EventHandler(this.DeleteAllButtonClick);
			// 
			// deleteButton
			// 
			this.deleteButton.Enabled = false;
			this.deleteButton.Image = global::EpicEdit.Properties.Resources.button_delete;
			this.deleteButton.Location = new System.Drawing.Point(73, 3);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(24, 24);
			this.deleteButton.TabIndex = 1;
			this.deleteButton.UseVisualStyleBackColor = true;
			this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
			// 
			// overlayPanel
			// 
			this.overlayPanel.Location = new System.Drawing.Point(0, 0);
			this.overlayPanel.Name = "overlayPanel";
			this.overlayPanel.Size = new System.Drawing.Size(128, 512);
			this.overlayPanel.TabIndex = 2;
			// 
			// deletePanel
			// 
			this.deletePanel.Controls.Add(this.deleteButton);
			this.deletePanel.Controls.Add(this.deleteAllButton);
			this.deletePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.deletePanel.Location = new System.Drawing.Point(0, 516);
			this.deletePanel.Name = "deletePanel";
			this.deletePanel.Size = new System.Drawing.Size(130, 32);
			this.deletePanel.TabIndex = 0;
			// 
			// OverlayControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.deletePanel);
			this.Controls.Add(this.overlayPanel);
			this.Name = "OverlayControl";
			this.Size = new System.Drawing.Size(130, 548);
			this.deletePanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Panel deletePanel;
		private EpicEdit.UI.Tools.EpicPanel overlayPanel;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button deleteAllButton;
	}
}
