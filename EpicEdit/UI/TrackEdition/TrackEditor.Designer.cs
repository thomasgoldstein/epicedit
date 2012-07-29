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
    partial class TrackEditor
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

                if (this.drawer != null)
                {
                    this.drawer.Dispose();
                }

                if (this.paletteForm != null)
                {
                    this.paletteForm.Dispose();
                }

                if (this.itemProbaForm != null)
                {
                    this.itemProbaForm.Dispose();
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
            System.Windows.Forms.Label tabFocusRemover;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrackEditor));
            this.trackDisplay = new EpicEdit.UI.TrackEdition.TrackPanel();
            this.trackDisplayHScrollBar = new System.Windows.Forms.HScrollBar();
            this.trackDisplayVScrollBar = new System.Windows.Forms.VScrollBar();
            this.modeTabControl = new System.Windows.Forms.TabControl();
            this.tilesetTabPage = new System.Windows.Forms.TabPage();
            this.tilesetControl = new EpicEdit.UI.TrackEdition.RoadTilesetControl();
            this.overlayTabPage = new System.Windows.Forms.TabPage();
            this.overlayControl = new EpicEdit.UI.TrackEdition.OverlayControl();
            this.startTabPage = new System.Windows.Forms.TabPage();
            this.startControl = new EpicEdit.UI.TrackEdition.StartControl();
            this.objectsTabPage = new System.Windows.Forms.TabPage();
            this.objectsControl = new EpicEdit.UI.TrackEdition.ObjectsControl();
            this.aiTabPage = new System.Windows.Forms.TabPage();
            this.aiControl = new EpicEdit.UI.TrackEdition.AIControl();
            this.tabImageList = new System.Windows.Forms.ImageList(this.components);
            this.trackTreeView = new EpicEdit.UI.TrackEdition.TrackTreeView();
            this.menuBar = new EpicEdit.UI.MenuBar();
            tabFocusRemover = new System.Windows.Forms.Label();
            this.trackDisplay.SuspendLayout();
            this.modeTabControl.SuspendLayout();
            this.tilesetTabPage.SuspendLayout();
            this.overlayTabPage.SuspendLayout();
            this.startTabPage.SuspendLayout();
            this.objectsTabPage.SuspendLayout();
            this.aiTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabFocusRemover
            // 
            tabFocusRemover.BackColor = System.Drawing.Color.Transparent;
            tabFocusRemover.Location = new System.Drawing.Point(0, 25);
            tabFocusRemover.Name = "tabFocusRemover";
            tabFocusRemover.Size = new System.Drawing.Size(1, 1);
            tabFocusRemover.TabIndex = 2;
            // 
            // trackDisplay
            // 
            this.trackDisplay.BackColor = System.Drawing.Color.Black;
            this.trackDisplay.Controls.Add(this.trackDisplayHScrollBar);
            this.trackDisplay.Controls.Add(this.trackDisplayVScrollBar);
            this.trackDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackDisplay.Enabled = false;
            this.trackDisplay.Location = new System.Drawing.Point(144, 25);
            this.trackDisplay.Name = "trackDisplay";
            this.trackDisplay.Size = new System.Drawing.Size(403, 451);
            this.trackDisplay.TabIndex = 1;
            this.trackDisplay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TrackDisplayKeyDown);
            this.trackDisplay.GotFocus += new System.EventHandler(this.TrackDisplayGotFocus);
            this.trackDisplay.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayMouseDoubleClick);
            this.trackDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayMouseDown);
            this.trackDisplay.MouseLeave += new System.EventHandler(this.TrackDisplayMouseLeave);
            this.trackDisplay.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayMouseMove);
            this.trackDisplay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayMouseUp);
            this.trackDisplay.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayMouseWheel);
            // 
            // trackDisplayHScrollBar
            // 
            this.trackDisplayHScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackDisplayHScrollBar.Location = new System.Drawing.Point(0, 434);
            this.trackDisplayHScrollBar.Name = "trackDisplayHScrollBar";
            this.trackDisplayHScrollBar.Size = new System.Drawing.Size(386, 17);
            this.trackDisplayHScrollBar.TabIndex = 0;
            this.trackDisplayHScrollBar.Visible = false;
            this.trackDisplayHScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.TrackDisplayHScrollBarScroll);
            this.trackDisplayHScrollBar.ValueChanged += new System.EventHandler(this.TrackDisplayHScrollBarValueChanged);
            // 
            // trackDisplayVScrollBar
            // 
            this.trackDisplayVScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackDisplayVScrollBar.Location = new System.Drawing.Point(386, 0);
            this.trackDisplayVScrollBar.Name = "trackDisplayVScrollBar";
            this.trackDisplayVScrollBar.Size = new System.Drawing.Size(17, 451);
            this.trackDisplayVScrollBar.TabIndex = 1;
            this.trackDisplayVScrollBar.Visible = false;
            this.trackDisplayVScrollBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayVScrollBarMouseMove);
            this.trackDisplayVScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.TrackDisplayVScrollBarScroll);
            this.trackDisplayVScrollBar.ValueChanged += new System.EventHandler(this.TrackDisplayVScrollBarValueChanged);
            this.trackDisplayVScrollBar.MouseLeave += new System.EventHandler(this.TrackDisplayVScrollBarMouseLeave);
            this.trackDisplayVScrollBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.TrackDisplayMouseWheel);
            // 
            // modeTabControl
            // 
            this.modeTabControl.Controls.Add(this.tilesetTabPage);
            this.modeTabControl.Controls.Add(this.overlayTabPage);
            this.modeTabControl.Controls.Add(this.startTabPage);
            this.modeTabControl.Controls.Add(this.objectsTabPage);
            this.modeTabControl.Controls.Add(this.aiTabPage);
            this.modeTabControl.Dock = System.Windows.Forms.DockStyle.Right;
            this.modeTabControl.Enabled = false;
            this.modeTabControl.ImageList = this.tabImageList;
            this.modeTabControl.ItemSize = new System.Drawing.Size(28, 19);
            this.modeTabControl.Location = new System.Drawing.Point(547, 25);
            this.modeTabControl.Name = "modeTabControl";
            this.modeTabControl.Padding = new System.Drawing.Point(0, 3);
            this.modeTabControl.SelectedIndex = 0;
            this.modeTabControl.ShowToolTips = true;
            this.modeTabControl.Size = new System.Drawing.Size(144, 451);
            this.modeTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.modeTabControl.TabIndex = 2;
            this.modeTabControl.SelectedIndexChanged += new System.EventHandler(this.ModeTabControlSelectedIndexChanged);
            this.modeTabControl.ClientSizeChanged += new System.EventHandler(this.ModeTabControlClientSizeChanged);
            // 
            // tilesetTabPage
            // 
            this.tilesetTabPage.AutoScroll = true;
            this.tilesetTabPage.Controls.Add(this.tilesetControl);
            this.tilesetTabPage.ImageIndex = 0;
            this.tilesetTabPage.Location = new System.Drawing.Point(4, 23);
            this.tilesetTabPage.Name = "tilesetTabPage";
            this.tilesetTabPage.Size = new System.Drawing.Size(136, 424);
            this.tilesetTabPage.TabIndex = 0;
            this.tilesetTabPage.ToolTipText = "Tileset";
            // 
            // tilesetControl
            // 
            this.tilesetControl.Location = new System.Drawing.Point(3, 3);
            this.tilesetControl.Name = "tilesetControl";
            this.tilesetControl.Size = new System.Drawing.Size(130, 660);
            this.tilesetControl.TabIndex = 0;
            this.tilesetControl.TrackThemeChanged += new System.EventHandler<System.EventArgs>(this.TilesetControlTrackThemeChanged);
            this.tilesetControl.SelectedThemeChanged += new System.EventHandler<System.EventArgs>(this.TilesetControlSelectedThemeChanged);
            this.tilesetControl.SelectedTileChanged += new System.EventHandler<System.EventArgs>(this.TilesetControlSelectedTileChanged);
            this.tilesetControl.TrackMapChanged += new System.EventHandler<System.EventArgs>(this.TilesetControlTrackMapChanged);
            this.tilesetControl.TileChanged += new System.EventHandler<EpicEdit.UI.Tools.EventArgs<byte>>(this.TilesetControlTileChanged);
            this.tilesetControl.TilesetChanged += new System.EventHandler<System.EventArgs>(this.TilesetControlTilesetChanged);
            // 
            // overlayTabPage
            // 
            this.overlayTabPage.AutoScroll = true;
            this.overlayTabPage.Controls.Add(this.overlayControl);
            this.overlayTabPage.ImageIndex = 1;
            this.overlayTabPage.Location = new System.Drawing.Point(4, 23);
            this.overlayTabPage.Name = "overlayTabPage";
            this.overlayTabPage.Size = new System.Drawing.Size(136, 424);
            this.overlayTabPage.TabIndex = 4;
            this.overlayTabPage.ToolTipText = "Overlay";
            // 
            // overlayControl
            // 
            this.overlayControl.Location = new System.Drawing.Point(3, 3);
            this.overlayControl.Name = "overlayControl";
            this.overlayControl.Size = new System.Drawing.Size(130, 548);
            this.overlayControl.TabIndex = 0;
            this.overlayControl.DeleteRequested += new System.EventHandler<System.EventArgs>(this.OverlayControlDeleteRequested);
            this.overlayControl.DeleteAllRequested += new System.EventHandler<System.EventArgs>(this.OverlayControlDeleteAllRequested);
            this.overlayControl.RepaintRequested += new System.EventHandler<System.EventArgs>(this.OverlayControlRepaintRequested);
            // 
            // startTabPage
            // 
            this.startTabPage.AutoScroll = true;
            this.startTabPage.Controls.Add(this.startControl);
            this.startTabPage.ImageIndex = 2;
            this.startTabPage.Location = new System.Drawing.Point(4, 23);
            this.startTabPage.Name = "startTabPage";
            this.startTabPage.Size = new System.Drawing.Size(136, 424);
            this.startTabPage.TabIndex = 3;
            this.startTabPage.ToolTipText = "Start";
            // 
            // startControl
            // 
            this.startControl.Location = new System.Drawing.Point(3, 3);
            this.startControl.Name = "startControl";
            this.startControl.Size = new System.Drawing.Size(130, 270);
            this.startControl.TabIndex = 0;
            this.startControl.DataChanged += new System.EventHandler<System.EventArgs>(this.StartControlDataChanged);
            // 
            // objectsTabPage
            // 
            this.objectsTabPage.AutoScroll = true;
            this.objectsTabPage.Controls.Add(this.objectsControl);
            this.objectsTabPage.ImageIndex = 3;
            this.objectsTabPage.Location = new System.Drawing.Point(4, 23);
            this.objectsTabPage.Name = "objectsTabPage";
            this.objectsTabPage.Size = new System.Drawing.Size(136, 424);
            this.objectsTabPage.TabIndex = 1;
            this.objectsTabPage.ToolTipText = "Objects";
            // 
            // objectsControl
            // 
            this.objectsControl.Location = new System.Drawing.Point(3, 3);
            this.objectsControl.Name = "objectsControl";
            this.objectsControl.Size = new System.Drawing.Size(130, 550);
            this.objectsControl.TabIndex = 0;
            this.objectsControl.DataChanged += new System.EventHandler<System.EventArgs>(this.ObjectsControlDataChanged);
            this.objectsControl.DataChangedNoRepaint += new System.EventHandler<System.EventArgs>(this.ObjectsControlDataChangedNoRepaint);
            this.objectsControl.ViewChanged += new System.EventHandler<System.EventArgs>(this.ObjectsControlViewChanged);
            // 
            // aiTabPage
            // 
            this.aiTabPage.AutoScroll = true;
            this.aiTabPage.Controls.Add(this.aiControl);
            this.aiTabPage.ImageIndex = 4;
            this.aiTabPage.Location = new System.Drawing.Point(4, 23);
            this.aiTabPage.Name = "aiTabPage";
            this.aiTabPage.Size = new System.Drawing.Size(136, 424);
            this.aiTabPage.TabIndex = 2;
            this.aiTabPage.ToolTipText = "AI";
            // 
            // aiControl
            // 
            this.aiControl.Location = new System.Drawing.Point(3, 3);
            this.aiControl.Name = "aiControl";
            this.aiControl.Size = new System.Drawing.Size(130, 360);
            this.aiControl.TabIndex = 0;
            this.aiControl.DataChanged += new System.EventHandler<System.EventArgs>(this.AIControlDataChanged);
            this.aiControl.DataChangedNoRepaint += new System.EventHandler<System.EventArgs>(this.AIControlDataChangedNoRepaint);
            this.aiControl.DeleteRequested += new System.EventHandler<System.EventArgs>(this.AIControlDeleteRequested);
            this.aiControl.AddRequested += new System.EventHandler<System.EventArgs>(this.AIControlAddRequested);
            this.aiControl.DeleteAllRequested += new System.EventHandler<System.EventArgs>(this.AIControlDeleteAllRequested);
            // 
            // tabImageList
            // 
            this.tabImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabImageList.ImageStream")));
            this.tabImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.tabImageList.Images.SetKeyName(0, "TilesetTab");
            this.tabImageList.Images.SetKeyName(1, "OverlayTab");
            this.tabImageList.Images.SetKeyName(2, "StartTab");
            this.tabImageList.Images.SetKeyName(3, "ObjectTab");
            this.tabImageList.Images.SetKeyName(4, "AITab");
            // 
            // trackTreeView
            // 
            this.trackTreeView.Dock = System.Windows.Forms.DockStyle.Left;
            this.trackTreeView.Location = new System.Drawing.Point(0, 25);
            this.trackTreeView.Name = "trackTreeView";
            this.trackTreeView.Size = new System.Drawing.Size(144, 451);
            this.trackTreeView.TabIndex = 0;
            this.trackTreeView.SelectedTrackChanged += new System.EventHandler<System.EventArgs>(this.TrackTreeViewSelectedTrackChanged);
            // 
            // menuBar
            // 
            this.menuBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.RedoEnabled = false;
            this.menuBar.Size = new System.Drawing.Size(691, 25);
            this.menuBar.TabIndex = 4;
            this.menuBar.UndoEnabled = false;
            this.menuBar.ZoomInEnabled = false;
            this.menuBar.ZoomOutEnabled = false;
            this.menuBar.TrackImportDialogRequested += new System.EventHandler<System.EventArgs>(this.MenuBarTrackImportDialogRequested);
            this.menuBar.TrackExportDialogRequested += new System.EventHandler<System.EventArgs>(this.MenuBarTrackExportDialogRequested);
            this.menuBar.UndoRequested += new System.EventHandler<System.EventArgs>(this.MenuBarUndoRequested);
            this.menuBar.RedoRequested += new System.EventHandler<System.EventArgs>(this.MenuBarRedoRequested);
            this.menuBar.ZoomInRequested += new System.EventHandler<System.EventArgs>(this.MenuBarZoomInRequested);
            this.menuBar.ZoomOutRequested += new System.EventHandler<System.EventArgs>(this.MenuBarZoomOutRequested);
            this.menuBar.ZoomResetRequested += new System.EventHandler<System.EventArgs>(this.MenuBarZoomResetRequested);
            this.menuBar.PaletteEditorRequested += new System.EventHandler<System.EventArgs>(this.MenuBarPaletteEditorRequested);
            this.menuBar.BackgroundEditorRequested += new System.EventHandler<System.EventArgs>(this.MenuBarBackgroundEditorRequested);
            this.menuBar.ItemProbaEditorRequested += new System.EventHandler<System.EventArgs>(this.MenuBarItemProbaEditorRequested);
            // 
            // TrackEditor
            // 
            this.AllowDrop = true;
            this.Controls.Add(tabFocusRemover);
            this.Controls.Add(this.trackDisplay);
            this.Controls.Add(this.modeTabControl);
            this.Controls.Add(this.trackTreeView);
            this.Controls.Add(this.menuBar);
            this.Name = "TrackEditor";
            this.Size = new System.Drawing.Size(691, 476);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.TrackEditorDragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.TrackEditorDragEnter);
            this.trackDisplay.ResumeLayout(false);
            this.modeTabControl.ResumeLayout(false);
            this.tilesetTabPage.ResumeLayout(false);
            this.overlayTabPage.ResumeLayout(false);
            this.startTabPage.ResumeLayout(false);
            this.objectsTabPage.ResumeLayout(false);
            this.aiTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private EpicEdit.UI.TrackEdition.OverlayControl overlayControl;
        private EpicEdit.UI.TrackEdition.RoadTilesetControl tilesetControl;
        private EpicEdit.UI.TrackEdition.StartControl startControl;
        private EpicEdit.UI.TrackEdition.ObjectsControl objectsControl;
        private EpicEdit.UI.TrackEdition.AIControl aiControl;
        private System.Windows.Forms.TabControl modeTabControl;
        private System.Windows.Forms.TabPage overlayTabPage;
        private System.Windows.Forms.TabPage startTabPage;
        private System.Windows.Forms.ImageList tabImageList;
        private System.Windows.Forms.TabPage aiTabPage;
        private System.Windows.Forms.TabPage objectsTabPage;
        private System.Windows.Forms.TabPage tilesetTabPage;
        private EpicEdit.UI.MenuBar menuBar;
        private EpicEdit.UI.TrackEdition.TrackTreeView trackTreeView;
        private System.Windows.Forms.VScrollBar trackDisplayVScrollBar;
        private System.Windows.Forms.HScrollBar trackDisplayHScrollBar;
        private EpicEdit.UI.TrackEdition.TrackPanel trackDisplay;
    }
}