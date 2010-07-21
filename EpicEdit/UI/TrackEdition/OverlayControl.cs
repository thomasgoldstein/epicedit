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
using System.Windows.Forms;

using EpicEdit.Rom.Tracks.Overlay;

namespace EpicEdit.UI.TrackEdition
{
	/// <summary>
	/// Represents a collection of controls to edit <see cref="OverlayTiles"/>.
	/// </summary>
	public partial class OverlayControl : UserControl
	{
		[Browsable(true)]
		public event EventHandler<EventArgs> DeleteRequested;

		[Browsable(true)]
		public event EventHandler<EventArgs> DeleteAllRequested;

		private OverlayTile selectedTile = null;

		/// <summary>
		/// The selected track overlay tile.
		/// </summary>
		[Browsable(false), DefaultValue(typeof(OverlayTile), "")]
		public OverlayTile SelectedTile
		{
			get
			{
				return this.selectedTile;
			}
			set
			{
				this.selectedTile = value;
				this.deleteButton.Enabled = this.selectedTile != null;
			}
		}

		public OverlayControl()
		{
			this.InitializeComponent();
		}
		
		private void DeleteButtonClick(object sender, EventArgs e)
		{
			this.DeleteRequested(this, EventArgs.Empty);
		}

		private void DeleteAllButtonClick(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Do you really want to delete all overlay tiles?",
			                                      Application.ProductName,
			                                      MessageBoxButtons.YesNo,
			                                      MessageBoxIcon.Warning);

			if (result == DialogResult.Yes)
			{
				this.DeleteAllRequested(this, EventArgs.Empty);
			}
		}
	}
}
