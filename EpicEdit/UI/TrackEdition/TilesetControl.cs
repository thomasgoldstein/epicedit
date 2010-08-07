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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;

namespace EpicEdit.UI.TrackEdition
{
	/// <summary>
	/// Represents a list of <see cref="Theme"/> objects, and the tileset of the selected theme.
	/// </summary>
	public partial class TilesetControl : UserControl
	{
		/// <summary>
		/// Raised when the theme of the current track has been modified.
		/// </summary>
		[Browsable(true)]
		public event EventHandler<EventArgs> TrackThemeChanged;

		/// <summary>
		/// Raised when the selected theme has been changed.
		/// </summary>
		[Browsable(true)]
		public event EventHandler<EventArgs> SelectedThemeChanged;

		/// <summary>
		/// Raised when a new tile has been selected.
		/// </summary>
		[Browsable(true)]
		public event EventHandler<EventArgs> SelectedTileChanged;

		/// <summary>
		/// Raised when the track map has been changed.
		/// </summary>
		[Browsable(true)]
		public event EventHandler<EventArgs> TrackMapChanged;

		/// <summary>
		/// Used to draw the tileset.
		/// </summary>
		private TilesetDrawer tilesetDrawer;

		private Track track = null;

		[Browsable(false), DefaultValue(typeof(Track), "")]
		public Track Track
		{
			get
			{
				return this.track;
			}
			set
			{
				this.track = value;
			}
		}

		private byte selectedTile = 0;

		[Browsable(false), DefaultValue(typeof(byte), "0")]
		public byte SelectedTile
		{
			get
			{
				return this.selectedTile;
			}
			set
			{
				this.selectedTile = value;
				this.tilesetDrawer.DrawTileset(this.selectedTile);
			}
		}

		[Browsable(false)]
		public int SelectedThemeIndex
		{
			get
			{
				return this.themeComboBox.SelectedIndex;
			}
		}

		public TilesetControl()
		{
			this.InitializeComponent();
		}

		public void InitOnRomLoading()
		{
			this.tilesetDrawer = new TilesetDrawer(this.tilesetPanel);

			// The following event handler is added here rather than in the Designer.cs
			// to save us a null check on this.drawer in each of the corresponding functions,
			// because the drawer doesn't exist yet before a ROM is loaded.
			this.tilesetPanel.Paint += this.TilesetPanelPaint;

			// The following event handler is added here rather than in the Designer.cs
			// to avoid an extra repaint triggered by
			// selecting the current theme in the theme ComboBox.
			this.themeComboBox.SelectedIndexChanged += this.ThemeComboBoxSelectedIndexChanged;
		}

		public void InitializeThemeList()
		{
			this.themeComboBox.BeginUpdate();
			this.themeComboBox.Items.Clear();
			foreach (Theme theme in MainForm.SmkGame.GetThemes())
			{
				this.themeComboBox.Items.Add(theme);
			}
			this.themeComboBox.EndUpdate();
		}

		private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			Theme selectedTheme = (Theme)this.themeComboBox.SelectedItem;
			if (selectedTheme != this.track.Theme)
			{
				this.track.Theme = selectedTheme;
				this.TrackThemeChanged(this, EventArgs.Empty);
			}

			Tile[] tileset = this.track.GetRoadTileset();
			this.tilesetDrawer.SetTileset(tileset);
			this.tilesetDrawer.DrawTileset(this.selectedTile);
			this.SelectedThemeChanged(this, EventArgs.Empty);
		}

		public void SelectCurrentTrackTheme()
		{
			this.themeComboBox.SelectedItem = this.track.Theme;
		}

		private void TilesetPanelPaint(object sender, PaintEventArgs e)
		{
			this.tilesetDrawer.DrawTileset(this.selectedTile);
		}

		private void TilesetPanelMouseDown(object sender, MouseEventArgs e)
		{
			int zoom = 2;
			byte newSelectedTile = (byte)((e.X / (8 * zoom)) + (e.Y / (8 * zoom)) * 8);

			if (this.selectedTile != newSelectedTile)
			{
				this.selectedTile = newSelectedTile;
				this.tilesetDrawer.DrawTileset(this.selectedTile);
				this.SelectedTileChanged(this, EventArgs.Empty);
			}
		}
		
		private void ResetMapButtonClick(object sender, EventArgs e)
		{
			DialogResult result = MessageBox.Show("Do you really want to reset the map?",
			                                      Application.ProductName,
			                                      MessageBoxButtons.YesNo,
			                                      MessageBoxIcon.Warning);

			if (result == DialogResult.Yes)
			{
				this.track.Map.Clear(this.selectedTile);
				this.TrackMapChanged(this, EventArgs.Empty);
			}
		}
	}
}
