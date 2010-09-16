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
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Objects;

namespace EpicEdit.UI.TrackEdition
{
	/// <summary>
	/// Represents a collection of controls to edit <see cref="TrackObjects"/>.
	/// </summary>
	public partial class ObjectsControl : UserControl
	{
		[Browsable(true)]
		public event EventHandler<EventArgs> ObjectZonesChanged;
		
		[Browsable(true)]
		public event EventHandler<EventArgs> ObjectZonesViewChanged;

		public ObjectsControl()
		{
			this.InitializeComponent();
		}

		[Browsable(false), DefaultValue(typeof(GPTrack), "")]
		public GPTrack Track
		{
			get
			{
				return this.frontObjectZonesControl.Track;
			}
			set
			{
				this.frontObjectZonesControl.Track = value;
				this.rearObjectZonesControl.Track = value;
				this.readOnlyZonesLabel.Visible = value.ObjectZones.ReadOnly;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the current view is the front-zones one.
		/// </summary>
		public bool FrontZonesView
		{
			get
			{
				return this.frontZonesRadioButton.Checked;
			}
		}

		private void FrontZonesRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			this.ObjectZonesViewChanged(this, EventArgs.Empty);

			this.frontObjectZonesControl.Enabled = this.frontZonesRadioButton.Checked;
			this.rearObjectZonesControl.Enabled = this.rearZonesRadioButton.Checked;
		}

		private void FrontObjectZonesControlValueChanged(object sender, EventArgs e)
		{
			this.ObjectZonesChanged(this, EventArgs.Empty);
		}

		private void RearObjectZonesControlValueChanged(object sender, EventArgs e)
		{
			this.ObjectZonesChanged(this, EventArgs.Empty);
		}
	}
}
