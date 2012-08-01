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
                if (this.components != null)
                {
                    this.components.Dispose();
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
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuToolStrip = new System.Windows.Forms.ToolStrip();
            this.openRomToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveRomToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.importTrackToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.exportTrackToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.undoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.redoToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomOutToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomInToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.fullScreenToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.paletteToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.backgroundToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.itemProbaToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.aboutToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.coordinatesToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.hiddenMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveRomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetZoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetZoomNumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolStrip.SuspendLayout();
            this.hiddenMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
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
                                    this.undoToolStripButton,
                                    this.redoToolStripButton,
                                    this.zoomOutToolStripButton,
                                    this.zoomInToolStripButton,
                                    this.fullScreenToolStripButton,
                                    this.toolStripSeparator2,
                                    this.paletteToolStripButton,
                                    this.backgroundToolStripButton,
                                    this.itemProbaToolStripButton,
                                    this.toolStripSeparator3,
                                    this.aboutToolStripLabel,
                                    this.toolStripSeparator4,
                                    this.coordinatesToolStripLabel});
            this.menuToolStrip.Location = new System.Drawing.Point(0, 0);
            this.menuToolStrip.Name = "menuToolStrip";
            this.menuToolStrip.Size = new System.Drawing.Size(668, 25);
            this.menuToolStrip.Stretch = true;
            this.menuToolStrip.TabIndex = 5;
            this.menuToolStrip.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MenuToolStripMouseMove);
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
            this.importTrackToolStripButton.Image = global::EpicEdit.Properties.Resources.ImportButton;
            this.importTrackToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.importTrackToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.importTrackToolStripButton.Name = "importTrackToolStripButton";
            this.importTrackToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.importTrackToolStripButton.Text = "Import track";
            this.importTrackToolStripButton.Click += new System.EventHandler(this.ImportTrackToolStripButtonClick);
            // 
            // exportTrackToolStripButton
            // 
            this.exportTrackToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.exportTrackToolStripButton.Enabled = false;
            this.exportTrackToolStripButton.Image = global::EpicEdit.Properties.Resources.ExportButton;
            this.exportTrackToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.exportTrackToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.exportTrackToolStripButton.Name = "exportTrackToolStripButton";
            this.exportTrackToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.exportTrackToolStripButton.Text = "Export track";
            this.exportTrackToolStripButton.Click += new System.EventHandler(this.ExportTrackToolStripButtonClick);
            // 
            // undoToolStripButton
            // 
            this.undoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoToolStripButton.Enabled = false;
            this.undoToolStripButton.Image = global::EpicEdit.Properties.Resources.UndoButton;
            this.undoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.undoToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.undoToolStripButton.Name = "undoToolStripButton";
            this.undoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.undoToolStripButton.Text = "Undo";
            this.undoToolStripButton.Click += new System.EventHandler(this.UndoToolStripButtonClick);
            // 
            // redoToolStripButton
            // 
            this.redoToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoToolStripButton.Enabled = false;
            this.redoToolStripButton.Image = global::EpicEdit.Properties.Resources.RedoButton;
            this.redoToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.redoToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.redoToolStripButton.Name = "redoToolStripButton";
            this.redoToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.redoToolStripButton.Text = "Redo";
            this.redoToolStripButton.Click += new System.EventHandler(this.RedoToolStripButtonClick);
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
            this.zoomOutToolStripButton.Text = "Zoom out";
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
            this.zoomInToolStripButton.Text = "Zoom in";
            this.zoomInToolStripButton.Click += new System.EventHandler(this.ZoomInToolStripButtonClick);
            // 
            // fullScreenToolStripButton
            // 
            this.fullScreenToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fullScreenToolStripButton.Image = global::EpicEdit.Properties.Resources.FullScreenButton;
            this.fullScreenToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fullScreenToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.fullScreenToolStripButton.Name = "fullScreenToolStripButton";
            this.fullScreenToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.fullScreenToolStripButton.Text = "Full screen";
            this.fullScreenToolStripButton.Click += new System.EventHandler(this.FullScreenToolStripButtonClick);
            // 
            // paletteToolStripButton
            // 
            this.paletteToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.paletteToolStripButton.Enabled = false;
            this.paletteToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("paletteToolStripButton.Image")));
            this.paletteToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.paletteToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.paletteToolStripButton.Name = "paletteToolStripButton";
            this.paletteToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.paletteToolStripButton.Text = "Color palette editor";
            this.paletteToolStripButton.Click += new System.EventHandler(this.PaletteToolStripButtonClick);
            // 
            // backgroundToolStripButton
            // 
            this.backgroundToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backgroundToolStripButton.Enabled = false;
            this.backgroundToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("backgroundToolStripButton.Image")));
            this.backgroundToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backgroundToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.backgroundToolStripButton.Name = "backgroundToolStripButton";
            this.backgroundToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.backgroundToolStripButton.Text = "Background editor";
            this.backgroundToolStripButton.Click += new System.EventHandler(this.BackgroundToolStripButtonClick);
            // 
            // itemProbaToolStripButton
            // 
            this.itemProbaToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.itemProbaToolStripButton.Enabled = false;
            this.itemProbaToolStripButton.Image = global::EpicEdit.Properties.Resources.ItemProbaButton;
            this.itemProbaToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemProbaToolStripButton.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.itemProbaToolStripButton.Name = "itemProbaToolStripButton";
            this.itemProbaToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.itemProbaToolStripButton.Text = "Item probability editor";
            this.itemProbaToolStripButton.Click += new System.EventHandler(this.ItemProbaToolStripButtonClick);
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
            // coordinatesToolStripLabel
            // 
            this.coordinatesToolStripLabel.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.coordinatesToolStripLabel.Name = "coordinatesToolStripLabel";
            this.coordinatesToolStripLabel.Size = new System.Drawing.Size(32, 22);
            this.coordinatesToolStripLabel.Text = "(X,Y)";
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
                                    this.undoToolStripMenuItem,
                                    this.redoToolStripMenuItem,
                                    this.redoToolStripMenuItem2,
                                    this.zoomInToolStripMenuItem,
                                    this.zoomOutToolStripMenuItem,
                                    this.resetZoomToolStripMenuItem,
                                    this.resetZoomNumToolStripMenuItem,
                                    this.fullScreenToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openRomToolStripMenuItem
            // 
            this.openRomToolStripMenuItem.Name = "openRomToolStripMenuItem";
            this.openRomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openRomToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.openRomToolStripMenuItem.Text = "Open ROM";
            this.openRomToolStripMenuItem.Click += new System.EventHandler(this.OpenRomToolStripMenuItemClick);
            // 
            // saveRomToolStripMenuItem
            // 
            this.saveRomToolStripMenuItem.Enabled = false;
            this.saveRomToolStripMenuItem.Name = "saveRomToolStripMenuItem";
            this.saveRomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveRomToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.saveRomToolStripMenuItem.Text = "Save ROM";
            this.saveRomToolStripMenuItem.Click += new System.EventHandler(this.SaveRomToolStripMenuItemClick);
            // 
            // importTrackToolStripMenuItem
            // 
            this.importTrackToolStripMenuItem.Enabled = false;
            this.importTrackToolStripMenuItem.Name = "importTrackToolStripMenuItem";
            this.importTrackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.importTrackToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.importTrackToolStripMenuItem.Text = "Import Track";
            this.importTrackToolStripMenuItem.Click += new System.EventHandler(this.ImportTrackToolStripMenuItemClick);
            // 
            // exportTrackToolStripMenuItem
            // 
            this.exportTrackToolStripMenuItem.Enabled = false;
            this.exportTrackToolStripMenuItem.Name = "exportTrackToolStripMenuItem";
            this.exportTrackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportTrackToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.exportTrackToolStripMenuItem.Text = "Export Track";
            this.exportTrackToolStripMenuItem.Click += new System.EventHandler(this.ExportTrackToolStripMenuItemClick);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItemClick);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.RedoToolStripMenuItemClick);
            // 
            // redoToolStripMenuItem2
            // 
            this.redoToolStripMenuItem2.Name = "redoToolStripMenuItem2";
            this.redoToolStripMenuItem2.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
                                    | System.Windows.Forms.Keys.Z)));
            this.redoToolStripMenuItem2.Size = new System.Drawing.Size(224, 22);
            this.redoToolStripMenuItem2.Text = "Redo";
            this.redoToolStripMenuItem2.Click += new System.EventHandler(this.RedoToolStripMenuItem2Click);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Enabled = false;
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add)));
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.ZoomInToolStripMenuItemClick);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Enabled = false;
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Subtract)));
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.ZoomOutToolStripMenuItemClick);
            // 
            // resetZoomToolStripMenuItem
            // 
            this.resetZoomToolStripMenuItem.Enabled = false;
            this.resetZoomToolStripMenuItem.Name = "resetZoomToolStripMenuItem";
            this.resetZoomToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D0)));
            this.resetZoomToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.resetZoomToolStripMenuItem.Text = "Reset Zoom";
            this.resetZoomToolStripMenuItem.Click += new System.EventHandler(this.ResetZoomToolStripMenuItemClick);
            // 
            // resetZoomNumToolStripMenuItem
            // 
            this.resetZoomNumToolStripMenuItem.Enabled = false;
            this.resetZoomNumToolStripMenuItem.Name = "resetZoomNumToolStripMenuItem";
            this.resetZoomNumToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.NumPad0)));
            this.resetZoomNumToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.resetZoomNumToolStripMenuItem.Text = "Reset Zoom";
            this.resetZoomNumToolStripMenuItem.Click += new System.EventHandler(this.ResetZoomNumToolStripMenuItemClick);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            this.fullScreenToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.fullScreenToolStripMenuItem.Size = new System.Drawing.Size(224, 22);
            this.fullScreenToolStripMenuItem.Text = "Full Screen";
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.FullScreenToolStripMenuItemClick);
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
        private System.Windows.Forms.ToolStripButton backgroundToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton itemProbaToolStripButton;
        private System.Windows.Forms.ToolStrip menuToolStrip;
        private System.Windows.Forms.ToolStripButton paletteToolStripButton;
        private System.Windows.Forms.ToolStripButton openRomToolStripButton;
        private System.Windows.Forms.ToolStripButton saveRomToolStripButton;
        private System.Windows.Forms.ToolStripButton importTrackToolStripButton;
        private System.Windows.Forms.ToolStripButton exportTrackToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomOutToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomInToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton redoToolStripButton;
        private System.Windows.Forms.ToolStripButton undoToolStripButton;
        private System.Windows.Forms.ToolStripButton fullScreenToolStripButton;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel coordinatesToolStripLabel;
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
    }
}
