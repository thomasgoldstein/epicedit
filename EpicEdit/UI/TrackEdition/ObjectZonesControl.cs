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
using System.Globalization;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.TrackEdition
{
	/// <summary>
	/// Represents a collection of controls to edit <see cref="TrackObjectZones"/>.
	/// </summary>
	public partial class ObjectZonesControl : UserControl
	{
		[Browsable(true)]
		public event EventHandler<EventArgs> ValueChanged;

		[Category("Data")]
		public bool FrontViewZones { get; set; }

		private GPTrack track = null;

		[Category("Data"), Browsable(false), DefaultValue(typeof(GPTrack), "")]
		public GPTrack Track
		{
			get { return this.track; }
			set
			{
				this.track = value;
				this.zone1TrackBar.Enabled = !this.track.ObjectZones.ReadOnly;
				this.zone2TrackBar.Enabled = !this.track.ObjectZones.ReadOnly;
				this.zone3TrackBar.Enabled = !this.track.ObjectZones.ReadOnly;
				this.zone4TrackBar.Enabled = !this.track.ObjectZones.ReadOnly;

				this.Maximum = 128;
				int max = this.track.AI.ElementCount;

				byte zone1Value = this.track.ObjectZones.GetZoneValue(this.FrontViewZones, 0);
				byte zone2Value = this.track.ObjectZones.GetZoneValue(this.FrontViewZones, 1);
				byte zone3Value = this.track.ObjectZones.GetZoneValue(this.FrontViewZones, 2);
				byte zone4Value = this.track.ObjectZones.GetZoneValue(this.FrontViewZones, 3);

				this.zone1TrackBar.ValueChanged -= new EventHandler(this.Zone1TrackBarValueChanged);
				this.zone2TrackBar.ValueChanged -= new EventHandler(this.Zone2TrackBarValueChanged);
				this.zone3TrackBar.ValueChanged -= new EventHandler(this.Zone3TrackBarValueChanged);
				this.zone4TrackBar.ValueChanged -= new EventHandler(this.Zone4TrackBarValueChanged);

				this.zone1TrackBar.Value = zone1Value > max ? max : zone1Value;
				this.zone2TrackBar.Value = zone2Value > max ? max : zone2Value;
				this.zone3TrackBar.Value = zone3Value > max ? max : zone3Value;
				this.zone4TrackBar.Value = zone4Value > max ? max : zone4Value;

				this.zone1TrackBar.ValueChanged += new EventHandler(this.Zone1TrackBarValueChanged);
				this.zone2TrackBar.ValueChanged += new EventHandler(this.Zone2TrackBarValueChanged);
				this.zone3TrackBar.ValueChanged += new EventHandler(this.Zone3TrackBarValueChanged);
				this.zone4TrackBar.ValueChanged += new EventHandler(this.Zone4TrackBarValueChanged);

				this.Maximum = max;

				ObjectZonesControl.UpdateTrackBarLabel(this.zone1Label, 0, this.zone1TrackBar.Value);
				ObjectZonesControl.UpdateTrackBarLabel(this.zone2Label, this.zone1TrackBar.Value, this.zone2TrackBar.Value);
				ObjectZonesControl.UpdateTrackBarLabel(this.zone3Label, this.zone2TrackBar.Value, this.zone3TrackBar.Value);
				ObjectZonesControl.UpdateTrackBarLabel(this.zone4Label, this.zone3TrackBar.Value, this.zone4TrackBar.Value);
			}
		}

		[Category("Appearance")]
		public string Title
		{
			get { return this.zoneGroupBox.Text; }
			set { this.zoneGroupBox.Text = value; }
		}

		private int Maximum
		{
			set
			{
				this.zone1TrackBar.Maximum = value;
				this.zone2TrackBar.Maximum = value;
				this.zone3TrackBar.Maximum = value;
				this.zone4TrackBar.Maximum = value;
			}
		}

		/// <summary>
		/// Used to ensure the ValueChanged event is only raised once in case of chain reactions.
		/// </summary>
		private bool[] raiseValueChanged;

		public ObjectZonesControl()
		{
			this.InitializeComponent();
			this.raiseValueChanged = new bool[4];
		}

		private void Zone1TrackBarScroll(object sender, EventArgs e)
		{
			this.raiseValueChanged[0] = true;
		}

		private void Zone2TrackBarScroll(object sender, EventArgs e)
		{
			this.raiseValueChanged[1] = true;
		}

		private void Zone3TrackBarScroll(object sender, EventArgs e)
		{
			this.raiseValueChanged[2] = true;
		}

		private void Zone4TrackBarScroll(object sender, EventArgs e)
		{
			this.raiseValueChanged[3] = true;
		}

		private void Zone1TrackBarValueChanged(object sender, EventArgs e)
		{
			if (this.zone2TrackBar.Value < this.zone1TrackBar.Value)
			{
				this.zone2TrackBar.Value = this.zone1TrackBar.Value;
			}

			ObjectZonesControl.UpdateTrackBarLabel(this.zone1Label, 0, this.zone1TrackBar.Value);
			ObjectZonesControl.UpdateTrackBarLabel(this.zone2Label, this.zone1TrackBar.Value, this.zone2TrackBar.Value);

			this.track.ObjectZones.SetZoneValue(this.FrontViewZones, 0, (byte)this.zone1TrackBar.Value);

			if (this.raiseValueChanged[0])
			{
				this.ValueChanged(this, EventArgs.Empty);
				this.raiseValueChanged[0] = false;
			}
		}

		private void Zone2TrackBarValueChanged(object sender, EventArgs e)
		{
			if (this.zone3TrackBar.Value < this.zone2TrackBar.Value)
			{
				this.zone3TrackBar.Value = this.zone2TrackBar.Value;
			}
			else if (this.zone1TrackBar.Value > this.zone2TrackBar.Value)
			{
				this.zone1TrackBar.Value = this.zone2TrackBar.Value;
			}

			ObjectZonesControl.UpdateTrackBarLabel(this.zone2Label, this.zone1TrackBar.Value, this.zone2TrackBar.Value);
			ObjectZonesControl.UpdateTrackBarLabel(this.zone3Label, this.zone2TrackBar.Value, this.zone3TrackBar.Value);

			this.track.ObjectZones.SetZoneValue(this.FrontViewZones, 1, (byte)this.zone2TrackBar.Value);

			if (this.raiseValueChanged[1])
			{
				this.ValueChanged(this, EventArgs.Empty);
				this.raiseValueChanged[1] = false;
			}
		}

		private void Zone3TrackBarValueChanged(object sender, EventArgs e)
		{
			if (this.zone4TrackBar.Value < this.zone3TrackBar.Value)
			{
				this.zone4TrackBar.Value = this.zone3TrackBar.Value;
			}
			else if (this.zone2TrackBar.Value > this.zone3TrackBar.Value)
			{
				this.zone2TrackBar.Value = this.zone3TrackBar.Value;
			}

			ObjectZonesControl.UpdateTrackBarLabel(this.zone3Label, this.zone2TrackBar.Value, this.zone3TrackBar.Value);
			ObjectZonesControl.UpdateTrackBarLabel(this.zone4Label, this.zone3TrackBar.Value, this.zone4TrackBar.Value);

			this.track.ObjectZones.SetZoneValue(this.FrontViewZones, 2, (byte)this.zone3TrackBar.Value);

			if (this.raiseValueChanged[2])
			{
				this.ValueChanged(this, EventArgs.Empty);
				this.raiseValueChanged[2] = false;
			}
		}

		private void Zone4TrackBarValueChanged(object sender, EventArgs e)
		{
			if (this.zone3TrackBar.Value > this.zone4TrackBar.Value)
			{
				this.zone3TrackBar.Value = this.zone4TrackBar.Value;
			}

			ObjectZonesControl.UpdateTrackBarLabel(this.zone4Label, this.zone3TrackBar.Value, this.zone4TrackBar.Value);

			this.track.ObjectZones.SetZoneValue(this.FrontViewZones, 3, (byte)this.zone4TrackBar.Value);

			if (this.raiseValueChanged[3])
			{
				this.ValueChanged(this, EventArgs.Empty);
				this.raiseValueChanged[3] = false;
			}
		}

		private static void UpdateTrackBarLabel(Label label, int value1, int value2)
		{
			label.Text = string.Format(CultureInfo.InvariantCulture, "{0:X2}-{1:X2}", value1, value2);
		}
	}
}
