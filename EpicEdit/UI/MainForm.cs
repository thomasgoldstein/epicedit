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
using EpicEdit.UI.Tools;

namespace EpicEdit.UI
{
	/// <summary>
	/// The main window of the program.
	/// </summary>
	public partial class MainForm : Form
	{
		/// <summary>
		/// The Super Mario Kart Game.
		/// </summary>
		internal static Game SmkGame;

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
				this.TryToOpenRom(args[0]);
			}
		}

		private void UpdateApplicationTitle()
		{
			this.Text = MainForm.SmkGame.FileName + " - " + Application.ProductName;
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

			DialogResult dialogResult =
				MessageBox.Show(
					"Do you want to save the changes to \"" + MainForm.SmkGame.FileName + "\"?",
					Application.ProductName,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Warning);

			switch (dialogResult)
			{
				case DialogResult.Yes:
					this.SaveRom(MainForm.SmkGame.FilePath);
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
			if (MainForm.SmkGame != null)
			{
				return MainForm.SmkGame.HasPendingChanges();
			}

			return false;
		}

		private void TrackEditorOpenRomDialogRequested(object sender, EventArgs e)
		{
			this.OpenRomDialog();
		}

		private void OpenRomDialog()
		{
			if (MainForm.HasPendingChanges())
			{
				bool cancelAction = this.PromptToSaveRom();
				if (cancelAction)
				{
					return;
				}
			}

			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "SNES ROM file (*.bin, *.fig, *.sfc, *.smc, *.swc, *.zip)|*.bin; *.fig; *.sfc; *.smc; *.swc; *.zip|All files (*.*)|*.*";

				if (ofd.ShowDialog() == DialogResult.OK)
				{
					this.TryToOpenRom(ofd.FileName);
				}
			}
		}

		private void TryToOpenRom(string filePath)
		{
			try
			{
				this.OpenRom(filePath);
			}
			catch (UnauthorizedAccessException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			catch (IOException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
			catch (InvalidDataException ex)
			{
				MessageBox.Show(
					ex.Message,
					Application.ProductName,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		private void OpenRom(string filePath)
		{
			if (MainForm.SmkGame == null) // First ROM loading
			{
				MainForm.SmkGame = new Game(filePath);
				this.trackEditor.InitOnFirstRomLoad();
				this.themeEditor.InitOnFirstRomLoad();
			}
			else
			{
				Game newSmkGame = new Game(filePath);
				MainForm.SmkGame.Dispose();
				MainForm.SmkGame = null;
				MainForm.SmkGame = newSmkGame;
				this.trackEditor.InitOnRomLoad();
				this.themeEditor.InitOnRomLoad();
			}

			this.UpdateApplicationTitle();
		}

		private void TrackEditorFileDragged(object sender, EventArgs<string> e)
		{
			string filePath = e.Value;
			string ext = Path.GetExtension(filePath);

			if (".mkt".Equals(ext, StringComparison.OrdinalIgnoreCase) ||
				".smkc".Equals(ext, StringComparison.OrdinalIgnoreCase))
			{
				if (MainForm.SmkGame == null)
				{
					MessageBox.Show(
						"You cannot load a track before having loaded a ROM.",
						Application.ProductName,
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
				else
				{
					this.trackEditor.ImportTrack(filePath);
				}
			}
			else
			{
				this.TryToOpenRom(filePath);
			}
		}

		private void TrackEditorSaveRomDialogRequested(object sender, EventArgs e)
		{
			this.SaveRomDialog();
		}

		/// <summary>
		/// Call the dialog to save the ROM.
		/// </summary>
		private void SaveRomDialog()
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = "SNES ROM file (*.bin, *.fig, *.sfc, *.smc, *.swc)|*.bin; *.fig; *.sfc; *.smc; *.swc|All files (*.*)|*.*";
				sfd.FileName = MainForm.SmkGame.FilePath;

				if (sfd.ShowDialog() == DialogResult.OK)
				{
					try
					{
						this.SaveRom(sfd.FileName);
					}
					catch (UnauthorizedAccessException ex)
					{
						MessageBox.Show(
							ex.Message,
							Application.ProductName,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
					catch (IOException ex)
					{
						MessageBox.Show(
							ex.Message,
							Application.ProductName,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
					catch (InvalidOperationException ex)
					{
						MessageBox.Show(
							ex.Message,
							Application.ProductName,
							MessageBoxButtons.OK,
							MessageBoxIcon.Error);
					}
				}
			}
		}

		private void SaveRom(string filePath)
		{
			MainForm.SmkGame.SaveRom(filePath);
			this.trackEditor.RemoveModifiedHints();
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

				if (this.WindowState != FormWindowState.Maximized)
				{
					this.WindowState = FormWindowState.Maximized;
				}
				else
				{
					// HACK: Toggle form visibility to make it cover the task bar.
					this.Visible = false;
					this.Visible = true;
				}
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
