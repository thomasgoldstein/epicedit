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

using EpicEdit.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace EpicEdit.UI
{
    /// <summary>
    /// Represents the application menu bar.
    /// </summary>
    internal partial class MenuBar : UserControl
    {
        #region Events
        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> OpenRomDialogRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> SaveRomDialogRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> TrackImportDialogRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> TrackExportDialogRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> TrackImportAllDialogRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> TrackExportAllDialogRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> UndoRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> RedoRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> ZoomInRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> ZoomOutRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> ZoomResetRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> ToggleScreenModeRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> PaletteEditorRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> BackgroundEditorRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> SettingEditorRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> CodecRequested;
        #endregion Events

        #region Constructor
        public MenuBar()
        {
            InitializeComponent();
        }
        #endregion Constructor

        #region Menu items

        #region Auto-focus
        private void MenuToolStripMouseMove(object sender, MouseEventArgs e)
        {
            // Force focus, so that when another form is focused,
            // it doesn't take 2 clicks to call a menu item
            menuToolStrip.Focus();
        }
        #endregion Auto-focus

        #region Open ROM
        private void OpenRomToolStripButtonClick(object sender, EventArgs e)
        {
            OpenRomDialogRequested(this, EventArgs.Empty);
        }

        public void EnableControls()
        {
            saveRomToolStripButton.Enabled = true;
            importTrackToolStripButton.Enabled = true;
            exportTrackToolStripButton.Enabled = true;
            importAllTrackToolStripButton.Enabled = true;
            exportAllTracksTooltipButton.Enabled = true;

            zoomOutToolStripButton.Enabled = true;
            zoomInToolStripButton.Enabled = true;
            paletteToolStripButton.Enabled = true;
            backgroundToolStripButton.Enabled = true;
            settingToolStripButton.Enabled = true;
            codecToolStripButton.Enabled = true;

            // Enable key shortcuts
            saveRomToolStripMenuItem.Enabled = true;
            saveRomAsToolStripMenuItem.Enabled = true;
            importTrackToolStripMenuItem.Enabled = true;
            exportTrackToolStripMenuItem.Enabled = true;
            zoomInToolStripMenuItem.Enabled = true;
            zoomOutToolStripMenuItem.Enabled = true;
            resetZoomToolStripMenuItem.Enabled = true;
            resetZoomNumToolStripMenuItem.Enabled = true;
        }
        #endregion Open ROM

        #region Save ROM
        private void SaveRomToolStripButtonClick(object sender, EventArgs e)
        {
            SaveRomDialogRequested(this, EventArgs.Empty);
        }
        #endregion Save ROM

        #region Import / Export tracks
        private void ImportTrackToolStripButtonClick(object sender, EventArgs e)
        {
            TrackImportDialogRequested(this, EventArgs.Empty);
        }

        private void ExportTrackToolStripButtonClick(object sender, EventArgs e)
        {
            TrackExportDialogRequested(this, EventArgs.Empty);
        }

        private void ImportAllTrackToolStripButton_Click(object sender, EventArgs e)
        {
            TrackImportAllDialogRequested(this, EventArgs.Empty);
        }

        private void ExportAllTracksTooltipButton_Click(object sender, EventArgs e)
        {
            TrackExportAllDialogRequested(this, EventArgs.Empty);
        }
        #endregion Import / Export tracks

        #region Undo / Redo
        private void Undo()
        {
            UndoRequested(this, EventArgs.Empty);
        }

        private void Redo()
        {
            RedoRequested(this, EventArgs.Empty);
        }

        private void TryToUndo()
        {
            if (undoToolStripButton.Enabled)
            {
                Undo();
            }
        }

        private void TryToRedo()
        {
            if (redoToolStripButton.Enabled)
            {
                Redo();
            }
        }

        private void UndoToolStripButtonClick(object sender, EventArgs e)
        {
            Undo();
        }

        private void RedoToolStripButtonClick(object sender, EventArgs e)
        {
            Redo();
        }

        public bool UndoEnabled
        {
            get => undoToolStripButton.Enabled;
            set => undoToolStripButton.Enabled = value;
        }

        public bool RedoEnabled
        {
            get => redoToolStripButton.Enabled;
            set => redoToolStripButton.Enabled = value;
        }
        #endregion Undo / Redo

        #region Zoom in / out
        private void ZoomInToolStripButtonClick(object sender, EventArgs e)
        {
            ZoomInRequested(this, e);
        }

        private void ZoomOutToolStripButtonClick(object sender, EventArgs e)
        {
            ZoomOutRequested(this, e);
        }

        public bool ZoomInEnabled
        {
            get => zoomInToolStripButton.Enabled;
            set => zoomInToolStripButton.Enabled = value;
        }

        public bool ZoomOutEnabled
        {
            get => zoomOutToolStripButton.Enabled;
            set => zoomOutToolStripButton.Enabled = value;
        }
        #endregion Zoom in / out

        #region Full screen
        private void FullScreenToolStripButtonClick(object sender, EventArgs e)
        {
            ToggleScreenMode();
        }

        private void ToggleScreenMode()
        {
            ToggleScreenModeRequested(this, EventArgs.Empty);

            if (ParentForm.FormBorderStyle == FormBorderStyle.None)
            {
                fullScreenToolStripButton.ToolTipText = "Exit Full Screen";
                fullScreenToolStripButton.Image = Resources.ExitFullScreenButton;
            }
            else
            {
                fullScreenToolStripButton.ToolTipText = "Full Screen";
                fullScreenToolStripButton.Image = Resources.FullScreenButton;
            }
        }
        #endregion Full screen

        #region Palette editor
        private void PaletteToolStripButtonClick(object sender, EventArgs e)
        {
            PaletteEditorRequested(this, EventArgs.Empty);
        }
        #endregion Palette editor

        #region Background editor
        private void BackgroundToolStripButtonClick(object sender, EventArgs e)
        {
            BackgroundEditorRequested(this, EventArgs.Empty);
        }
        #endregion Background editor

        #region Setting editor
        private void SettingToolStripButtonClick(object sender, EventArgs e)
        {
            SettingEditorRequested(this, EventArgs.Empty);
        }
        #endregion Setting editor

        #region Codec
        private void CodecToolStripButtonClick(object sender, EventArgs e)
        {
            CodecRequested(this, EventArgs.Empty);
        }
        #endregion Codec

        #region About
        private void AboutToolStripLabelClick(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            var versionText = version.Build == 0 ?
                version.Major + "." + version.Minor :
                version.Major + "." + version.Minor + "." + version.Build;

            MessageBox.Show(Application.ProductName + " - Super Mario Kart track editor" + Environment.NewLine +
                            "Version: " + versionText + " (2007-2021)" + Environment.NewLine +
                            "Site: https://epicedit.stifu.fr/" + Environment.NewLine + Environment.NewLine +
                            "By Stifu" + Environment.NewLine +
                            "Contributors: teknix1, mcintyre321 and nub1604" + Environment.NewLine + Environment.NewLine +
                            "Special thanks to:" + Environment.NewLine +
                            "Presea, Midwife, Cragz, smkdan, Ok Impala! and ScouB",
                            "About " + Application.ProductName,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
        #endregion About

        #region Cursor coordinates Label
        public void ResetCoordinates()
        {
            coordinatesToolStripLabel.Text = "(X,Y)";
        }

        public void UpdateCoordinates(Point location)
        {
            coordinatesToolStripLabel.Text = "(" + location.X + "," + location.Y + ")";
        }
        #endregion Cursor coordinate Label

        #endregion Menu items

        #region Menu shortcut keys
        private void OpenRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenRomDialogRequested(this, EventArgs.Empty);
        }

        private void SaveRomToolStripMenuItemClick(object sender, EventArgs e)
        {
            Context.Game.SaveRom();
        }

        private void SaveRomAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveRomDialogRequested(this, EventArgs.Empty);
        }

        private void ImportTrackToolStripMenuItemClick(object sender, EventArgs e)
        {
            TrackImportDialogRequested(this, EventArgs.Empty);
        }

        private void ExportTrackToolStripMenuItemClick(object sender, EventArgs e)
        {
            TrackExportDialogRequested(this, EventArgs.Empty);
        }

        private void ZoomInToolStripMenuItemClick(object sender, EventArgs e)
        {
            ZoomInRequested(this, EventArgs.Empty);
        }

        private void ZoomOutToolStripMenuItemClick(object sender, EventArgs e)
        {
            ZoomOutRequested(this, EventArgs.Empty);
        }

        private void ResetZoomToolStripMenuItemClick(object sender, EventArgs e)
        {
            ZoomResetRequested(this, EventArgs.Empty);
        }

        private void ResetZoomNumToolStripMenuItemClick(object sender, EventArgs e)
        {
            ZoomResetRequested(this, EventArgs.Empty);
        }

        private void FullScreenToolStripMenuItemClick(object sender, EventArgs e)
        {
            ToggleScreenMode();
        }

        private void UndoToolStripMenuItemClick(object sender, EventArgs e)
        {
            TryToUndo();
        }

        private void RedoToolStripMenuItemClick(object sender, EventArgs e)
        {
            TryToRedo();
        }

        private void RedoToolStripMenuItem2Click(object sender, EventArgs e)
        {
            TryToRedo();
        }
        #endregion Menu shortcut keys
    }
}
