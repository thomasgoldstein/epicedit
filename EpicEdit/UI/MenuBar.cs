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

namespace EpicEdit.UI
{
	/// <summary>
	/// Represents the application menu bar.
	/// </summary>
	public partial class MenuBar : UserControl
	{
		[Browsable(true)]
		public event EventHandler<EventArgs> OpenRomDialogRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> SaveRomDialogRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> TrackImportDialogRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> TrackExportDialogRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> ZoomInRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> ZoomOutRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> ZoomResetRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> ToggleScreenModeRequested;

		public MenuBar()
		{
			this.InitializeComponent();
		}

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

		#region Zoom in / out
		private void ZoomInToolStripButtonClick(object sender, EventArgs e)
		{
			this.ZoomInRequested(this, e);
		}

		private void ZoomOutToolStripButtonClick(object sender, EventArgs e)
		{
			this.ZoomOutRequested(this, e);
		}
		#endregion Zoom in / out

		#region Full Screen
		private void FullScreenToolStripButtonClick(object sender, EventArgs e)
		{
			this.ToggleScreenModeRequested(this, e);
			this.UpdateFullScreenButton();
		}

		private void UpdateFullScreenButton()
		{
			ComponentResourceManager resources = new ComponentResourceManager(typeof(MenuBar));

			if (this.fullScreenToolStripButton.ToolTipText.Equals("Full Screen", StringComparison.OrdinalIgnoreCase))
			{
				this.fullScreenToolStripButton.ToolTipText = "Exit Full Screen";
				this.fullScreenToolStripButton.Image = ((Image)(resources.GetObject("fullScreenToolStripButton.Image2")));
			}
			else
			{
				this.fullScreenToolStripButton.ToolTipText = "Full Screen";
				this.fullScreenToolStripButton.Image = ((Image)(resources.GetObject("fullScreenToolStripButton.Image")));
			}
		}
		#endregion Full Screen

		#region About
		private void AboutToolStripLabelClick(object sender, EventArgs e)
		{
			Version version = Assembly.GetExecutingAssembly().GetName().Version;
			string versionText;

			if (version.Build == 0)
			{
				versionText =
					version.Major.ToString(CultureInfo.InvariantCulture) +
					"." + version.Minor.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				versionText =
					version.Major.ToString(CultureInfo.InvariantCulture) +
					"." + version.Minor.ToString(CultureInfo.InvariantCulture) +
					"." + version.Build.ToString();
			}

			MessageBox.Show(Application.ProductName + " - Super Mario Kart track editor" + Environment.NewLine +
							"Version: " + versionText + " (2007-2010)" + Environment.NewLine +
							"By Stifu" + Environment.NewLine +
							"Assistant coders: Midwife and teknix1" + Environment.NewLine + Environment.NewLine +
							"Special thanks to:" + Environment.NewLine +
							"Presea, Cragz, smkdan, Ok Impala! and ScouB",
							"About " + Application.ProductName,
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
		}
		#endregion

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
		#endregion Menu shortcut keys
	}
}
