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
using EpicEdit.Rom.Utility;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="EpicEdit.Rom.Tracks.Objects.TrackObjectZones"/>.
    /// </summary>
    internal partial class ObjectZonesControl : UserControl
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

                int max = this.track.AI.ElementCount;
                this.Maximum = max;

                byte zone1Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 0);
                byte zone2Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 1);
                byte zone3Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 2);
                byte zone4Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 3);

                this.zone1TrackBar.ValueChanged -= this.Zone1TrackBarValueChanged;
                this.zone2TrackBar.ValueChanged -= this.Zone2TrackBarValueChanged;
                this.zone3TrackBar.ValueChanged -= this.Zone3TrackBarValueChanged;
                this.zone4TrackBar.ValueChanged -= this.Zone4TrackBarValueChanged;

                this.zone1TrackBar.Value = Math.Min(zone1Value, max);
                this.zone2TrackBar.Value = Math.Min(zone2Value, max);
                this.zone3TrackBar.Value = Math.Min(zone3Value, max);
                this.zone4TrackBar.Value = Math.Min(zone4Value, max);

                this.zone1TrackBar.ValueChanged += this.Zone1TrackBarValueChanged;
                this.zone2TrackBar.ValueChanged += this.Zone2TrackBarValueChanged;
                this.zone3TrackBar.ValueChanged += this.Zone3TrackBarValueChanged;
                this.zone4TrackBar.ValueChanged += this.Zone4TrackBarValueChanged;

                ObjectZonesControl.UpdateTrackBarLabel(this.zone1Label, 0, this.zone1TrackBar.Value);
                ObjectZonesControl.UpdateTrackBarLabel(this.zone2Label, this.zone1TrackBar.Value, this.zone2TrackBar.Value);
                ObjectZonesControl.UpdateTrackBarLabel(this.zone3Label, this.zone2TrackBar.Value, this.zone3TrackBar.Value);
                ObjectZonesControl.UpdateTrackBarLabel(this.zone4Label, this.zone3TrackBar.Value, this.zone4TrackBar.Value);
            }
        }

        [Category("Appearance")]
        public string Title
        {
            get { return this.groupBox.Text; }
            set { this.groupBox.Text = value; }
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
        private readonly bool[] raiseValueChanged;

        public ObjectZonesControl()
        {
            this.InitializeComponent();

            this.raiseValueChanged = new bool[4];
            this.zone1TrackBar.Tag = 0;
            this.zone2TrackBar.Tag = 1;
            this.zone3TrackBar.Tag = 2;
            this.zone4TrackBar.Tag = 3;
        }

        private void ZoneTrackBarScroll(object sender, EventArgs e)
        {
            int zoneIndex = (int)(sender as Control).Tag;
            this.raiseValueChanged[zoneIndex] = true;
        }

        private void Zone1TrackBarValueChanged(object sender, EventArgs e)
        {
            this.ZoneTrackBarValueChanged(null,
                                          this.zone1TrackBar, this.zone1Label,
                                          this.zone2TrackBar, this.zone2Label);
        }

        private void Zone2TrackBarValueChanged(object sender, EventArgs e)
        {
            this.ZoneTrackBarValueChanged(this.zone1TrackBar,
                                          this.zone2TrackBar, this.zone2Label,
                                          this.zone3TrackBar, this.zone3Label);
        }

        private void Zone3TrackBarValueChanged(object sender, EventArgs e)
        {
            this.ZoneTrackBarValueChanged(this.zone2TrackBar,
                                          this.zone3TrackBar, this.zone3Label,
                                          this.zone4TrackBar, this.zone4Label);
        }

        private void Zone4TrackBarValueChanged(object sender, EventArgs e)
        {
            this.ZoneTrackBarValueChanged(this.zone3TrackBar,
                                          this.zone4TrackBar, this.zone4Label,
                                          null, null);
        }

        private void ZoneTrackBarValueChanged(TrackBar prevTrackBar, TrackBar trackBar, Label label, TrackBar nextTrackBar, Label nextLabel)
        {
            if (nextTrackBar != null && nextTrackBar.Value < trackBar.Value)
            {
                nextTrackBar.Value = trackBar.Value;
            }
            else if (prevTrackBar != null && prevTrackBar.Value > trackBar.Value)
            {
                prevTrackBar.Value = trackBar.Value;
            }

            ObjectZonesControl.UpdateTrackBarLabel(label, prevTrackBar == null ? 0 : prevTrackBar.Value, trackBar.Value);

            if (nextTrackBar != null)
            {
                ObjectZonesControl.UpdateTrackBarLabel(nextLabel, trackBar.Value, nextTrackBar.Value);
            }

            int zoneIndex = (int)trackBar.Tag;
            this.track.Objects.Zones.SetZoneValue(this.FrontViewZones, zoneIndex, (byte)trackBar.Value);

            if (this.raiseValueChanged[zoneIndex])
            {
                this.ValueChanged(this, EventArgs.Empty);
                this.raiseValueChanged[zoneIndex] = false;
            }
        }

        private static void UpdateTrackBarLabel(Label label, int value1, int value2)
        {
            label.Text =
                Utilities.ByteToHexString((byte)value1) + "-" +
                Utilities.ByteToHexString((byte)value2);
        }
    }
}
