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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.UI.Gfx;

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

		/// <summary>
		/// Used to draw the overlay tileset.
		/// </summary>
		private OverlayTilesetDrawer overlayDrawer;

		/// <summary>
		/// The selected track overlay tile.
		/// </summary>
		private OverlayTile selectedTile = null;

		/// <summary>
		/// Gets or sets the selected track overlay tile.
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

		public void InitOnRomLoading()
		{
			this.overlayDrawer = new OverlayTilesetDrawer(this.overlayPanel);

			// The following event handler is added here rather than in the Designer.cs
			// to save us a null check on this.drawer in each of the corresponding functions,
			// because the drawer doesn't exist yet before a ROM is loaded.
			this.overlayPanel.Paint += this.OverlayPanelPaint;
		}

		public void SetTileset(Tile[] tileset)
		{
			this.overlayDrawer.SetTileset(tileset);
		}

		public void UpdateTileCount(int count)
		{
			this.tileCountLabel.Text = count + " / 42";
		}

		private void OverlayPanelPaint(object sender, PaintEventArgs e)
		{
			this.overlayDrawer.DrawOverlayTileset();
		}
		
		private void OverlayPanelMouseMove(object sender, MouseEventArgs e)
		{
			this.overlayDrawer.HighlightTileAt(e.Location);
		}
		
		private void OverlayPanelMouseLeave(object sender, EventArgs e)
		{
			this.overlayDrawer.ResetTileHighlighting();
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
