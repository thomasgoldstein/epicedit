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
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Utility;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="EpicEdit.Rom.Tracks.Objects.TrackObjectZones"/>.
    /// </summary>
    internal partial class ObjectZonesControl : UserControl
    {
        [Category("Data")]
        public bool FrontViewZones { get; set; }

        private bool fireEvents;

        private GPTrack track;

        [Category("Data"), Browsable(false), DefaultValue(typeof(GPTrack), "")]
        public GPTrack Track
        {
            get { return this.track; }
            set
            {
                if (this.track != null)
                {
                    this.track.Objects.Zones.DataChanged -= this.track_Objects_Zones_DataChanged;
                    this.track.AI.ElementAdded -= this.track_AI_CollectionChanged;
                    this.track.AI.ElementRemoved -= this.track_AI_CollectionChanged;
                }

                this.track = value;

                if (this.track == null) // BattleTrack
                {
                    return;
                }

                this.track.Objects.Zones.DataChanged += this.track_Objects_Zones_DataChanged;
                this.track.AI.ElementAdded += this.track_AI_CollectionChanged;
                this.track.AI.ElementRemoved += this.track_AI_CollectionChanged;

                int max = this.track.AI.ElementCount;
                this.Maximum = max;

                byte zone1Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 0);
                byte zone2Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 1);
                byte zone3Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 2);
                byte zone4Value = this.track.Objects.Zones.GetZoneValue(this.FrontViewZones, 3);

                fireEvents = false;

                this.zone1TrackBar.Value = Math.Min(zone1Value, max);
                this.zone2TrackBar.Value = Math.Min(zone2Value, max);
                this.zone3TrackBar.Value = Math.Min(zone3Value, max);
                this.zone4TrackBar.Value = Math.Min(zone4Value, max);

                fireEvents = true;

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

        public ObjectZonesControl()
        {
            this.InitializeComponent();

            this.zone1TrackBar.Tag = 0;
            this.zone2TrackBar.Tag = 1;
            this.zone3TrackBar.Tag = 2;
            this.zone4TrackBar.Tag = 3;
        }

        private void track_Objects_Zones_DataChanged(object sender, EventArgs<bool, int> e)
        {
            if (!this.Visible || e.Value1 != this.FrontViewZones)
            {
                // Rear zones were modified while this control shows front zones, or vice versa.
                // No need to update the control values, as they're not visible.
                return;
            }

            TrackBar trackBar =
                e.Value2 == 0 ? this.zone1TrackBar :
                e.Value2 == 1 ? this.zone2TrackBar :
                e.Value2 == 2 ? this.zone3TrackBar :
                this.zone4TrackBar;

            fireEvents = false;

            trackBar.Value = Math.Min(this.track.Objects.Zones.GetZoneValue(e.Value1, e.Value2), this.track.AI.ElementCount);

            fireEvents = true;

            ObjectZonesControl.UpdateTrackBarLabel(this.zone1Label, 0, this.zone1TrackBar.Value);
            ObjectZonesControl.UpdateTrackBarLabel(this.zone2Label, this.zone1TrackBar.Value, this.zone2TrackBar.Value);
            ObjectZonesControl.UpdateTrackBarLabel(this.zone3Label, this.zone2TrackBar.Value, this.zone3TrackBar.Value);
            ObjectZonesControl.UpdateTrackBarLabel(this.zone4Label, this.zone3TrackBar.Value, this.zone4TrackBar.Value);
        }

        private void track_AI_CollectionChanged(object sender, EventArgs<TrackAIElement> e)
        {
            this.Maximum = this.track.AI.ElementCount;
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
            if (!this.fireEvents)
            {
                return;
            }

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

            this.track.Objects.Zones.SetZoneValue(this.FrontViewZones, (int)trackBar.Tag, (byte)trackBar.Value);
        }

        private static void UpdateTrackBarLabel(Label label, int value1, int value2)
        {
            label.Text =
                Utilities.ByteToHexString((byte)value1) + "-" +
                Utilities.ByteToHexString((byte)value2);
        }
    }
}
