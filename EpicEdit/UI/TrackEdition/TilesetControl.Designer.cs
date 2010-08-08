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
	partial class TilesetControl
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
				if (components != null)
				{
					components.Dispose();
				}

				if (this.tilesetDrawer != null)
				{
					this.tilesetDrawer.Dispose();
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
			this.themeComboBox = new System.Windows.Forms.ComboBox();
			this.tilesetPanel = new EpicEdit.UI.Tools.EpicPanel();
			this.resetMapButton = new System.Windows.Forms.Button();
			this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// themeComboBox
			// 
			this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.themeComboBox.FormattingEnabled = true;
			this.themeComboBox.Location = new System.Drawing.Point(3, 3);
			this.themeComboBox.Name = "themeComboBox";
			this.themeComboBox.Size = new System.Drawing.Size(121, 21);
			this.themeComboBox.TabIndex = 6;
			// 
			// tilesetPanel
			// 
			this.tilesetPanel.BackColor = System.Drawing.Color.Black;
			this.tilesetPanel.Location = new System.Drawing.Point(0, 30);
			this.tilesetPanel.Name = "tilesetPanel";
			this.tilesetPanel.Size = new System.Drawing.Size(128, 512);
			this.tilesetPanel.TabIndex = 5;
			this.tilesetPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilesetPanelMouseDown);
			// 
			// resetMapButton
			// 
			this.resetMapButton.Image = global::EpicEdit.Properties.Resources.button_nuke;
			this.resetMapButton.Location = new System.Drawing.Point(103, 549);
			this.resetMapButton.Name = "resetMapButton";
			this.resetMapButton.Size = new System.Drawing.Size(24, 24);
			this.resetMapButton.TabIndex = 7;
			this.buttonToolTip.SetToolTip(this.resetMapButton, "Reset map");
			this.resetMapButton.UseVisualStyleBackColor = true;
			this.resetMapButton.Click += new System.EventHandler(this.ResetMapButtonClick);
			// 
			// TilesetControl
			// 
			this.Controls.Add(this.resetMapButton);
			this.Controls.Add(this.themeComboBox);
			this.Controls.Add(this.tilesetPanel);
			this.Name = "TilesetControl";
			this.Size = new System.Drawing.Size(130, 580);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.ToolTip buttonToolTip;
		private System.Windows.Forms.Button resetMapButton;
		private EpicEdit.UI.Tools.EpicPanel tilesetPanel;
		private System.Windows.Forms.ComboBox themeComboBox;
	}
}
