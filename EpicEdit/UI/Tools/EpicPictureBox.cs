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
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
	/// <summary>
	/// Represents a PictureBox that displays a grayed-out image when disabled.
	/// </summary>
	public class EpicPictureBox : PictureBox
	{
		private Image disabledImage;

		public new Image Image
		{
			get { return base.Image; }
			set
			{
				base.Image = value;

				if (this.disabledImage != null)
				{
					this.disabledImage.Dispose();
					this.disabledImage = null;
				}
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			if (this.Enabled)
			{
				base.OnPaint(pe);
			}
			else if (this.Image != null)
			{
				if (this.disabledImage == null)
				{
					this.disabledImage = ToolStripRenderer.CreateDisabledImage(this.Image);
				}
				pe.Graphics.DrawImage(this.disabledImage, 0, 0);
			}
		}
	}
}
