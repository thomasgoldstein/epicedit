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

namespace EpicEdit.UI
{
	partial class MenuBar
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuBar));
			this.menuToolStrip = new System.Windows.Forms.ToolStrip();
			this.openRomToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.saveRomToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.importTrackToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.exportTrackToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.zoomOutToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.zoomInToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.aboutToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.positionToolStripLabel = new System.Windows.Forms.ToolStripLabel();
			this.hiddenMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetZoomNumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuToolStrip.SuspendLayout();
			this.hiddenMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuToolStrip
			// 
			this.menuToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.menuToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.openRomToolStripButton,
									this.saveRomToolStripButton,
									this.importTrackToolStripButton,
									this.exportTrackToolStripButton,
									this.toolStripSeparator1,
									this.zoomOutToolStripButton,
									this.zoomInToolStripButton,
									this.toolStripSeparator2,
									this.aboutToolStripLabel,
									this.toolStripSeparator3,
									this.positionToolStripLabel});
			this.menuToolStrip.Location = new System.Drawing.Point(0, 0);
			this.menuToolStrip.Name = "menuToolStrip";
			this.menuToolStrip.Size = new System.Drawing.Size(668, 25);
			this.menuToolStrip.Stretch = true;
			this.menuToolStrip.TabIndex = 5;
			// 
			// openRomToolStripButton
			// 
			this.openRomToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.openRomToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openRomToolStripButton.Image")));
			this.openRomToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.openRomToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.openRomToolStripButton.Name = "openRomToolStripButton";
			this.openRomToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.openRomToolStripButton.Text = "Open ROM";
			this.openRomToolStripButton.Click += new System.EventHandler(this.OpenRomToolStripButtonClick);
			// 
			// saveRomToolStripButton
			// 
			this.saveRomToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveRomToolStripButton.Enabled = false;
			this.saveRomToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveRomToolStripButton.Image")));
			this.saveRomToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveRomToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.saveRomToolStripButton.Name = "saveRomToolStripButton";
			this.saveRomToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.saveRomToolStripButton.Text = "Save ROM";
			this.saveRomToolStripButton.Click += new System.EventHandler(this.SaveRomToolStripButtonClick);
			// 
			// importTrackToolStripButton
			// 
			this.importTrackToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.importTrackToolStripButton.Enabled = false;
			this.importTrackToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("importTrackToolStripButton.Image")));
			this.importTrackToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.importTrackToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.importTrackToolStripButton.Name = "importTrackToolStripButton";
			this.importTrackToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.importTrackToolStripButton.Text = "Import Track";
			this.importTrackToolStripButton.Click += new System.EventHandler(this.ImportTrackToolStripButtonClick);
			// 
			// exportTrackToolStripButton
			// 
			this.exportTrackToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.exportTrackToolStripButton.Enabled = false;
			this.exportTrackToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("exportTrackToolStripButton.Image")));
			this.exportTrackToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.exportTrackToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.exportTrackToolStripButton.Name = "exportTrackToolStripButton";
			this.exportTrackToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.exportTrackToolStripButton.Text = "Export Track";
			this.exportTrackToolStripButton.Click += new System.EventHandler(this.ExportTrackToolStripButtonClick);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// zoomOutToolStripButton
			// 
			this.zoomOutToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.zoomOutToolStripButton.Enabled = false;
			this.zoomOutToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomOutToolStripButton.Image")));
			this.zoomOutToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.zoomOutToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.zoomOutToolStripButton.Name = "zoomOutToolStripButton";
			this.zoomOutToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.zoomOutToolStripButton.Text = "Zoom Out";
			this.zoomOutToolStripButton.Click += new System.EventHandler(this.ZoomOutToolStripButtonClick);
			// 
			// zoomInToolStripButton
			// 
			this.zoomInToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.zoomInToolStripButton.Enabled = false;
			this.zoomInToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("zoomInToolStripButton.Image")));
			this.zoomInToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.zoomInToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.zoomInToolStripButton.Name = "zoomInToolStripButton";
			this.zoomInToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.zoomInToolStripButton.Text = "Zoom In";
			this.zoomInToolStripButton.Click += new System.EventHandler(this.ZoomInToolStripButtonClick);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// aboutToolStripLabel
			// 
			this.aboutToolStripLabel.Margin = new System.Windows.Forms.Padding(10, 3, 4, 4);
			this.aboutToolStripLabel.Name = "aboutToolStripLabel";
			this.aboutToolStripLabel.Size = new System.Drawing.Size(12, 18);
			this.aboutToolStripLabel.Text = "?";
			this.aboutToolStripLabel.ToolTipText = "About Epic Edit";
			this.aboutToolStripLabel.Click += new System.EventHandler(this.AboutToolStripLabelClick);
			// 
			// positionToolStripLabel
			// 
			this.positionToolStripLabel.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
			this.positionToolStripLabel.Name = "positionToolStripLabel";
			this.positionToolStripLabel.Size = new System.Drawing.Size(31, 22);
			this.positionToolStripLabel.Text = "(X,Y)";
			// 
			// hiddenMenuStrip
			// 
			this.hiddenMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.fileToolStripMenuItem});
			this.hiddenMenuStrip.Location = new System.Drawing.Point(0, 25);
			this.hiddenMenuStrip.Name = "hiddenMenuStrip";
			this.hiddenMenuStrip.Size = new System.Drawing.Size(668, 24);
			this.hiddenMenuStrip.TabIndex = 6;
			this.hiddenMenuStrip.Visible = false;
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.openRomToolStripMenuItem,
									this.saveRomToolStripMenuItem,
									this.importTrackToolStripMenuItem,
									this.exportTrackToolStripMenuItem,
									this.zoomInToolStripMenuItem,
									this.zoomOutToolStripMenuItem,
									this.resetZoomToolStripMenuItem,
									this.resetZoomNumToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// openRomToolStripMenuItem
			// 
			this.openRomToolStripMenuItem.Name = "openRomToolStripMenuItem";
			this.openRomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openRomToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.openRomToolStripMenuItem.Text = "Open ROM";
			this.openRomToolStripMenuItem.Click += new System.EventHandler(this.OpenRomToolStripMenuItemClick);
			// 
			// saveRomToolStripMenuItem
			// 
			this.saveRomToolStripMenuItem.Enabled = false;
			this.saveRomToolStripMenuItem.Name = "saveRomToolStripMenuItem";
			this.saveRomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveRomToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.saveRomToolStripMenuItem.Text = "Save ROM";
			this.saveRomToolStripMenuItem.Click += new System.EventHandler(this.SaveRomToolStripMenuItemClick);
			// 
			// importTrackToolStripMenuItem
			// 
			this.importTrackToolStripMenuItem.Enabled = false;
			this.importTrackToolStripMenuItem.Name = "importTrackToolStripMenuItem";
			this.importTrackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
			this.importTrackToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.importTrackToolStripMenuItem.Text = "Import Track";
			this.importTrackToolStripMenuItem.Click += new System.EventHandler(this.ImportTrackToolStripMenuItemClick);
			// 
			// exportTrackToolStripMenuItem
			// 
			this.exportTrackToolStripMenuItem.Enabled = false;
			this.exportTrackToolStripMenuItem.Name = "exportTrackToolStripMenuItem";
			this.exportTrackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
			this.exportTrackToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.exportTrackToolStripMenuItem.Text = "Export Track";
			this.exportTrackToolStripMenuItem.Click += new System.EventHandler(this.ExportTrackToolStripMenuItemClick);
			// 
			// zoomInToolStripMenuItem
			// 
			this.zoomInToolStripMenuItem.Enabled = false;
			this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
			this.zoomInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add)));
			this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.zoomInToolStripMenuItem.Text = "Zoom In";
			this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.ZoomInToolStripMenuItemClick);
			// 
			// zoomOutToolStripMenuItem
			// 
			this.zoomOutToolStripMenuItem.Enabled = false;
			this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
			this.zoomOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Subtract)));
			this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.zoomOutToolStripMenuItem.Text = "Zoom Out";
			this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.ZoomOutToolStripMenuItemClick);
			// 
			// resetZoomToolStripMenuItem
			// 
			this.resetZoomToolStripMenuItem.Enabled = false;
			this.resetZoomToolStripMenuItem.Name = "resetZoomToolStripMenuItem";
			this.resetZoomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
			this.resetZoomToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.resetZoomToolStripMenuItem.Text = "Reset Zoom";
			this.resetZoomToolStripMenuItem.Click += new System.EventHandler(this.ResetZoomToolStripMenuItemClick);
			// 
			// resetZoomNumToolStripMenuItem
			// 
			this.resetZoomNumToolStripMenuItem.Enabled = false;
			this.resetZoomNumToolStripMenuItem.Name = "resetZoomNumToolStripMenuItem";
			this.resetZoomNumToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad0)));
			this.resetZoomNumToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
			this.resetZoomNumToolStripMenuItem.Text = "Reset Zoom";
			this.resetZoomNumToolStripMenuItem.Click += new System.EventHandler(this.ResetZoomNumToolStripMenuItemClick);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// MenuBar
			// 
			this.Controls.Add(this.hiddenMenuStrip);
			this.Controls.Add(this.menuToolStrip);
			this.Name = "MenuBar";
			this.Size = new System.Drawing.Size(668, 259);
			this.menuToolStrip.ResumeLayout(false);
			this.menuToolStrip.PerformLayout();
			this.hiddenMenuStrip.ResumeLayout(false);
			this.hiddenMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripLabel positionToolStripLabel;
		private System.Windows.Forms.ToolStripMenuItem resetZoomNumToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetZoomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportTrackToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importTrackToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveRomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openRomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.MenuStrip hiddenMenuStrip;
		private System.Windows.Forms.ToolStripLabel aboutToolStripLabel;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton zoomInToolStripButton;
		private System.Windows.Forms.ToolStripButton zoomOutToolStripButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton exportTrackToolStripButton;
		private System.Windows.Forms.ToolStripButton importTrackToolStripButton;
		private System.Windows.Forms.ToolStripButton saveRomToolStripButton;
		private System.Windows.Forms.ToolStripButton openRomToolStripButton;
		private System.Windows.Forms.ToolStrip menuToolStrip;
	}
}
