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

namespace EpicEdit.UI.ThemeEdition
{
	partial class ThemeEditor
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
			this.itemProbaControl = new EpicEdit.UI.ThemeEdition.ItemProbaControl();
			this.SuspendLayout();
			// 
			// itemProbaControl
			// 
			this.itemProbaControl.Location = new System.Drawing.Point(6, 6);
			this.itemProbaControl.Name = "itemProbaControl";
			this.itemProbaControl.Size = new System.Drawing.Size(430, 270);
			this.itemProbaControl.TabIndex = 0;
			// 
			// ThemeEditor
			// 
			this.Controls.Add(this.itemProbaControl);
			this.Name = "ThemeEditor";
			this.Size = new System.Drawing.Size(596, 508);
			this.ResumeLayout(false);
		}
		private EpicEdit.UI.ThemeEdition.ItemProbaControl itemProbaControl;
	}
}
