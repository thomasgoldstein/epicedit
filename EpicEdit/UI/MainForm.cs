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
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI
{
    /// <summary>
    /// The main window of the program.
    /// </summary>
    internal sealed partial class MainForm : Form
    {
        /// <summary>
        /// The state of the window before going full screen, to restore it later.
        /// </summary>
        private FormWindowState previousWindowState;

        [STAThread]
        public static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Application.Run(new MainForm(args));
        }

        public MainForm(string[] args)
        {
            this.Text = Application.ProductName;
            this.InitializeComponent();

            if (args.Length > 0 && File.Exists(args[0]))
            {
                UITools.ImportData(this.OpenRom, args[0]);
            }
        }

        private void UpdateApplicationTitle()
        {
            this.Text = Context.Game.FileName + " - " + Application.ProductName;
        }

        private void MainFormFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MainForm.HasPendingChanges())
            {
                bool cancelExit = this.PromptToSaveRom();
                e.Cancel = cancelExit;
            }
        }

        /// <summary>
        /// Prompts user to save the ROM (called when there are pending changes).
        /// </summary>
        /// <returns>Whether the action (ie: closing the application, or opening another ROM) must be cancelled.</returns>
        private bool PromptToSaveRom()
        {
            bool cancelPreviousAction = false;

            DialogResult dialogResult = UITools.ShowWarning(
                "Do you want to save the changes to \"" + Context.Game.FileName + "\"?",
                MessageBoxButtons.YesNoCancel);

            switch (dialogResult)
            {
                case DialogResult.Yes:
                    this.SaveRom(Context.Game.FilePath);
                    break;

                case DialogResult.No:
                    break;

                case DialogResult.Cancel:
                    cancelPreviousAction = true;
                    break;
            }

            return cancelPreviousAction;
        }

        private static bool HasPendingChanges()
        {
            return
                Context.Game != null &&
                Context.Game.HasPendingChanges();
        }

        private void TrackEditorOpenRomDialogRequested(object sender, EventArgs e)
        {
            this.ShowOpenRomDialog();
        }

        private void ShowOpenRomDialog()
        {
            if (MainForm.HasPendingChanges())
            {
                bool cancelAction = this.PromptToSaveRom();
                if (cancelAction)
                {
                    return;
                }
            }

            UITools.ShowImportDataDialog(this.OpenRom, FileDialogFilters.RomOrZippedRom);
        }

        private void OpenRom(string filePath)
        {
            // Do not directly set the Context.Game property,
            // in case the ROM is invalid (ie: Exception thrown in the Game constructor).
            Game game = new Game(filePath);

            if (Context.Game == null) // First ROM loading
            {
                Context.Game = game;
                this.trackEditor.InitOnFirstRomLoad();
            }
            else
            {
                Context.Game.Dispose();
                Context.Game = null;
                Context.Game = game;
                this.trackEditor.InitOnRomLoad();
            }

            this.UpdateApplicationTitle();
        }

        private void TrackEditorFileDragged(object sender, EventArgs<string> e)
        {
            string filePath = e.Value;

            if (filePath.EndsWith(".smkc", StringComparison.OrdinalIgnoreCase) ||
                filePath.EndsWith(".mkt", StringComparison.OrdinalIgnoreCase))
            {
                if (Context.Game == null)
                {
                    UITools.ShowError("You cannot load a track before having loaded a ROM.");
                }
                else
                {
                    this.trackEditor.ImportTrack(filePath);
                }
            }
            else
            {
                UITools.ImportData(this.OpenRom, filePath);
            }
        }

        private void TrackEditorSaveRomDialogRequested(object sender, EventArgs e)
        {
            this.ShowSaveRomDialog();
        }

        /// <summary>
        /// Calls the dialog to save the ROM.
        /// </summary>
        private void ShowSaveRomDialog()
        {
            string fileName = Context.Game.FileName;
            string ext = Path.GetExtension(fileName);

            // Make it so the loaded file extension is the default choice when resaving
            string filter = string.Format(FileDialogFilters.Rom, ext);

            fileName = Path.GetFileNameWithoutExtension(fileName);

            UITools.ShowExportDataDialog(this.SaveRom, fileName, filter);
        }

        private void SaveRom(string filePath)
        {
            Context.Game.SaveRom(filePath);
            this.UpdateApplicationTitle();
        }

        private void TrackEditorToggleScreenModeRequested(object sender, EventArgs e)
        {
            this.ToggleScreenMode();
        }

        private void ToggleScreenMode()
        {
            if (this.FormBorderStyle != FormBorderStyle.None)
            {
                // Go full screen
                this.FormBorderStyle = FormBorderStyle.None;
                this.previousWindowState = this.WindowState;
                this.WindowState = FormWindowState.Maximized;

                // HACK: Toggle form visibility to make it cover the task bar.
                // On Windows XP: if the form was already maximized, the task bar wouldn't be covered.
                // If the form wasn't maximized, it would be covered but not repainted right away.
                this.Visible = false;
                this.Visible = true;
            }
            else
            {
                // Go back to windowed mode
                this.FormBorderStyle = FormBorderStyle.Sizable;
                this.WindowState = this.previousWindowState;
            }
        }
    }
}
