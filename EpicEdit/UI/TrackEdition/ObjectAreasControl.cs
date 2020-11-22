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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Utility;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="TrackObjectAreasView"/>.
    /// </summary>
    internal partial class ObjectAreasControl : UserControl
    {
        private bool fireEvents;

        private TrackObjectAreasView areasView;

        [Category("Data"), Browsable(false), DefaultValue(typeof(GPTrack), "")]
        public TrackObjectAreasView AreasView
        {
            get => this.areasView;
            set
            {
                if (this.areasView != null)
                {
                    this.areasView.DataChanged -= this.areasView_DataChanged;
                    this.areasView.AI.ElementAdded -= this.areasView_AI_CollectionChanged;
                    this.areasView.AI.ElementRemoved -= this.areasView_AI_CollectionChanged;
                }

                this.areasView = value;

                this.areasView.DataChanged += this.areasView_DataChanged;
                this.areasView.AI.ElementAdded += this.areasView_AI_CollectionChanged;
                this.areasView.AI.ElementRemoved += this.areasView_AI_CollectionChanged;

                int max = this.areasView.AI.ElementCount;
                this.Maximum = max;

                byte area1Value = this.areasView.GetAreaValue(0);
                byte area2Value = this.areasView.GetAreaValue(1);
                byte area3Value = this.areasView.GetAreaValue(2);
                byte area4Value = this.areasView.GetAreaValue(3);

                this.fireEvents = false;

                this.area1TrackBar.Value = Math.Min(area1Value, max);
                this.area2TrackBar.Value = Math.Min(area2Value, max);
                this.area3TrackBar.Value = Math.Min(area3Value, max);
                this.area4TrackBar.Value = Math.Min(area4Value, max);

                this.fireEvents = true;

                this.UpdateTrackBarLabels();
            }
        }

        [Category("Appearance")]
        public string Title
        {
            get => this.groupBox.Text;
            set => this.groupBox.Text = value;
        }

        private int Maximum
        {
            set
            {
                this.area1TrackBar.Maximum = value;
                this.area2TrackBar.Maximum = value;
                this.area3TrackBar.Maximum = value;
                this.area4TrackBar.Maximum = value;
            }
        }

        public ObjectAreasControl()
        {
            this.InitializeComponent();

            this.area1TrackBar.Tag = 0;
            this.area2TrackBar.Tag = 1;
            this.area3TrackBar.Tag = 2;
            this.area4TrackBar.Tag = 3;
        }

        private void areasView_DataChanged(object sender, EventArgs<int> e)
        {
            TrackBar trackBar =
                e.Value == 0 ? this.area1TrackBar :
                e.Value == 1 ? this.area2TrackBar :
                e.Value == 2 ? this.area3TrackBar :
                this.area4TrackBar;

            this.fireEvents = false;

            trackBar.Value = Math.Min(this.areasView.GetAreaValue(e.Value), this.areasView.AI.ElementCount);

            this.fireEvents = true;

            this.UpdateTrackBarLabels();
        }

        private void areasView_AI_CollectionChanged(object sender, EventArgs<TrackAIElement> e)
        {
            this.Maximum = this.areasView.AI.ElementCount;
        }

        private void Area1TrackBarValueChanged(object sender, EventArgs e)
        {
            this.AreaTrackBarValueChanged(null,
                                          this.area1TrackBar, this.area1Label,
                                          this.area2TrackBar, this.area2Label);
        }

        private void Area2TrackBarValueChanged(object sender, EventArgs e)
        {
            this.AreaTrackBarValueChanged(this.area1TrackBar,
                                          this.area2TrackBar, this.area2Label,
                                          this.area3TrackBar, this.area3Label);
        }

        private void Area3TrackBarValueChanged(object sender, EventArgs e)
        {
            this.AreaTrackBarValueChanged(this.area2TrackBar,
                                          this.area3TrackBar, this.area3Label,
                                          this.area4TrackBar, this.area4Label);
        }

        private void Area4TrackBarValueChanged(object sender, EventArgs e)
        {
            this.AreaTrackBarValueChanged(this.area3TrackBar,
                                          this.area4TrackBar, this.area4Label,
                                          null, null);
        }

        private void AreaTrackBarValueChanged(TrackBar prevTrackBar, TrackBar trackBar, Label label, TrackBar nextTrackBar, Label nextLabel)
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

            ObjectAreasControl.UpdateTrackBarLabel(label, prevTrackBar == null ? 0 : prevTrackBar.Value, trackBar.Value);

            if (nextTrackBar != null)
            {
                ObjectAreasControl.UpdateTrackBarLabel(nextLabel, trackBar.Value, nextTrackBar.Value);
            }

            this.areasView.SetAreaValue((int)trackBar.Tag, (byte)trackBar.Value);
        }

        private static void UpdateTrackBarLabel(Label label, int value1, int value2)
        {
            label.Text =
                Utilities.ByteToHexString((byte)value1) + "-" +
                Utilities.ByteToHexString((byte)value2);
        }

        private void UpdateTrackBarLabels()
        {
            ObjectAreasControl.UpdateTrackBarLabel(this.area1Label, 0, this.area1TrackBar.Value);
            ObjectAreasControl.UpdateTrackBarLabel(this.area2Label, this.area1TrackBar.Value, this.area2TrackBar.Value);
            ObjectAreasControl.UpdateTrackBarLabel(this.area3Label, this.area2TrackBar.Value, this.area3TrackBar.Value);
            ObjectAreasControl.UpdateTrackBarLabel(this.area4Label, this.area3TrackBar.Value, this.area4TrackBar.Value);
        }
    }
}
