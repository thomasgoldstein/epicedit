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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using EpicEdit.Properties;
using EpicEdit.UI.ThemeEdition;
using EpicEdit.UI.Tools.UndoRedo;

namespace EpicEdit.UI
{
    /// <summary>
    /// Represents the application menu bar.
    /// </summary>
    internal partial class MenuBar : UserControl
    {
        #region Events
        [Browsable(true)]
        public event EventHandler<EventArgs> OpenRomDialogRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> SaveRomDialogRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> TrackImportDialogRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> TrackExportDialogRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> UndoRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> RedoRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ZoomInRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ZoomOutRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ZoomResetRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ToggleScreenModeRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> PaletteEditorRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ItemProbaEditorRequested;
        #endregion Events

        #region Constructor
        public MenuBar()
        {
            this.InitializeComponent();
        }
        #endregion Constructor

        #region Menu items

        #region Auto-focus
        private void MenuToolStripMouseMove(object sender, MouseEventArgs e)
        {
            // Force focus, so that when another form is focused (palette editor or item proba form),
            // it doesn't take 2 clicks to call a menu item
            this.menuToolStrip.Focus();
        }
        #endregion Auto-focus

        #region Open ROM
        private void OpenRomToolStripButtonClick(object sender, EventArgs e)
        {
            this.OpenRomDialogRequested(this, EventArgs.Empty);
        }

        public void EnableControls()
        {
            this.saveRomToolStripButton.Enabled = true;
            this.importTrackToolStripButton.Enabled = true;
            this.exportTrackToolStripButton.Enabled = true;
            this.zoomOutToolStripButton.Enabled = true;
            this.zoomInToolStripButton.Enabled = true;
            this.paletteToolStripButton.Enabled = true;
            this.itemProbaToolStripButton.Enabled = true;

            // Enable hidden key shortcuts
            this.saveRomToolStripMenuItem.Enabled = true;
            this.importTrackToolStripMenuItem.Enabled = true;
            this.exportTrackToolStripMenuItem.Enabled = true;
            this.zoomInToolStripMenuItem.Enabled = true;
            this.zoomOutToolStripMenuItem.Enabled = true;
            this.resetZoomToolStripMenuItem.Enabled = true;
            this.resetZoomNumToolStripMenuItem.Enabled = true;
        }
        #endregion Open ROM

        #region Save ROM
        private void SaveRomToolStripButtonClick(object sender, EventArgs e)
        {
            this.SaveRomDialogRequested(this, EventArgs.Empty);
        }

        private void SaveRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.SaveRomDialogRequested(this, EventArgs.Empty);
        }
        #endregion Save ROM

        #region Import / Export track
        private void ImportTrackToolStripButtonClick(object sender, EventArgs e)
        {
            this.TrackImportDialogRequested(this, EventArgs.Empty);
        }

        private void ExportTrackToolStripButtonClick(object sender, EventArgs e)
        {
            this.TrackExportDialogRequested(this, EventArgs.Empty);
        }
        #endregion Import / Export track

        #region Undo / Redo
        private void Undo()
        {
            this.UndoRequested(this, EventArgs.Empty);
        }

        private void Redo()
        {
            this.RedoRequested(this, EventArgs.Empty);
        }

        private void TryToUndo()
        {
            if (this.undoToolStripButton.Enabled)
            {
                this.Undo();
            }
        }

        private void TryToRedo()
        {
            if (this.redoToolStripButton.Enabled)
            {
                this.Redo();
            }
        }

        private void UndoToolStripButtonClick(object sender, EventArgs e)
        {
            this.Undo();
        }

        private void RedoToolStripButtonClick(object sender, EventArgs e)
        {
            this.Redo();
        }

        public bool UndoEnabled
        {
            get { return this.undoToolStripButton.Enabled; }
            set { this.undoToolStripButton.Enabled = value; }
        }

        public bool RedoEnabled
        {
            get { return this.redoToolStripButton.Enabled; }
            set { this.redoToolStripButton.Enabled = value; }
        }
        #endregion Undo / Redo

        #region Zoom in / out
        private void ZoomInToolStripButtonClick(object sender, EventArgs e)
        {
            this.ZoomInRequested(this, e);
        }

        private void ZoomOutToolStripButtonClick(object sender, EventArgs e)
        {
            this.ZoomOutRequested(this, e);
        }

        public bool ZoomInEnabled
        {
            get { return this.zoomInToolStripButton.Enabled; }
            set { this.zoomInToolStripButton.Enabled = value; }
        }

        public bool ZoomOutEnabled
        {
            get { return this.zoomOutToolStripButton.Enabled; }
            set { this.zoomOutToolStripButton.Enabled = value; }
        }
        #endregion Zoom in / out

        #region Full screen
        private void FullScreenToolStripButtonClick(object sender, EventArgs e)
        {
            this.ToggleScreenModeRequested(this, e);
            this.UpdateFullScreenButton();
        }

        private void UpdateFullScreenButton()
        {
            if (this.ParentForm.FormBorderStyle == FormBorderStyle.None)
            {
                if (this.exitFullScreenButtonImage == null)
                {
                    this.exitFullScreenButtonImage = Resources.ExitFullScreenButton;
                    this.fullScreenButtonImage = this.fullScreenToolStripButton.Image;
                }
                this.fullScreenToolStripButton.ToolTipText = "Exit Full Screen";
                this.fullScreenToolStripButton.Image = this.exitFullScreenButtonImage;
            }
            else
            {
                this.fullScreenToolStripButton.ToolTipText = "Full Screen";
                this.fullScreenToolStripButton.Image = this.fullScreenButtonImage;
            }
        }

        private Image fullScreenButtonImage;

        private Image exitFullScreenButtonImage;
        #endregion Full screen

        #region Palette editor
        private void PaletteToolStripButtonClick(object sender, EventArgs e)
        {
            this.PaletteEditorRequested(this, EventArgs.Empty);
        }
        #endregion Palette editor

        #region Item probability editor
        private void ItemProbaToolStripButtonClick(object sender, EventArgs e)
        {
            this.ItemProbaEditorRequested(this, EventArgs.Empty);
        }
        #endregion Item probability editor

        #region About
        private void AboutToolStripLabelClick(object sender, EventArgs e)
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            string versionText = version.Build == 0 ?
                version.Major + "." + version.Minor :
                version.Major + "." + version.Minor + "." + version.Build;

            MessageBox.Show(Application.ProductName + " - Super Mario Kart track editor" + Environment.NewLine +
                            "Version: " + versionText + " (2007-2012)" + Environment.NewLine +
                            "By Stifu" + Environment.NewLine +
                            "Assistant coders: Midwife and teknix1" + Environment.NewLine + Environment.NewLine +
                            "Special thanks to:" + Environment.NewLine +
                            "Presea, Cragz, smkdan, Ok Impala! and ScouB",
                            "About " + Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
        #endregion About

        #region Cursor coordinates Label
        public void ResetCoordinates()
        {
            this.coordinatesToolStripLabel.Text = "(X,Y)";
        }

        public void UpdateCoordinates(Point location)
        {
            this.coordinatesToolStripLabel.Text = "(" + location.X + "," + location.Y + ")";
        }
        #endregion Cursor coordinate Label

        #endregion Menu items

        #region Menu shortcut keys
        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.OpenRomDialogRequested(this, EventArgs.Empty);
        }

        private void ImportTrackToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.TrackImportDialogRequested(this, EventArgs.Empty);
        }

        private void ExportTrackToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.TrackExportDialogRequested(this, EventArgs.Empty);
        }

        private void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ZoomInRequested(this, EventArgs.Empty);
        }

        private void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ZoomOutRequested(this, EventArgs.Empty);
        }

        private void ResetZoomToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ZoomResetRequested(this, EventArgs.Empty);
        }

        private void ResetZoomNumToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ZoomResetRequested(this, EventArgs.Empty);
        }

        private void FullScreenToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ToggleScreenModeRequested(this, e);
            this.UpdateFullScreenButton();
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.TryToUndo();
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.TryToRedo();
        }

        private void RedoToolStripMenuItem2Click(object sender, EventArgs e)
        {
            this.TryToRedo();
        }
        #endregion Menu shortcut keys
    }
}
