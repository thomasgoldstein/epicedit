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
        private bool _fireEvents;

        private TrackObjectAreasView _areasView;

        [Category("Data"), Browsable(false), DefaultValue(typeof(GPTrack), "")]
        public TrackObjectAreasView AreasView
        {
            get => _areasView;
            set
            {
                if (_areasView != null)
                {
                    _areasView.DataChanged -= areasView_DataChanged;
                    _areasView.AI.ElementAdded -= areasView_AI_CollectionChanged;
                    _areasView.AI.ElementRemoved -= areasView_AI_CollectionChanged;
                }

                _areasView = value;

                _areasView.DataChanged += areasView_DataChanged;
                _areasView.AI.ElementAdded += areasView_AI_CollectionChanged;
                _areasView.AI.ElementRemoved += areasView_AI_CollectionChanged;

                var max = _areasView.AI.ElementCount;
                Maximum = max;

                var area1Value = _areasView.GetAreaValue(0);
                var area2Value = _areasView.GetAreaValue(1);
                var area3Value = _areasView.GetAreaValue(2);
                var area4Value = _areasView.GetAreaValue(3);

                _fireEvents = false;

                area1TrackBar.Value = Math.Min(area1Value, max);
                area2TrackBar.Value = Math.Min(area2Value, max);
                area3TrackBar.Value = Math.Min(area3Value, max);
                area4TrackBar.Value = Math.Min(area4Value, max);

                _fireEvents = true;

                UpdateTrackBarLabels();
            }
        }

        [Category("Appearance")]
        public string Title
        {
            get => groupBox.Text;
            set => groupBox.Text = value;
        }

        private int Maximum
        {
            set
            {
                area1TrackBar.Maximum = value;
                area2TrackBar.Maximum = value;
                area3TrackBar.Maximum = value;
                area4TrackBar.Maximum = value;
            }
        }

        public ObjectAreasControl()
        {
            InitializeComponent();

            area1TrackBar.Tag = 0;
            area2TrackBar.Tag = 1;
            area3TrackBar.Tag = 2;
            area4TrackBar.Tag = 3;
        }

        private void areasView_DataChanged(object sender, EventArgs<int> e)
        {
            var trackBar =
                e.Value == 0 ? area1TrackBar :
                e.Value == 1 ? area2TrackBar :
                e.Value == 2 ? area3TrackBar :
                area4TrackBar;

            _fireEvents = false;

            trackBar.Value = Math.Min(_areasView.GetAreaValue(e.Value), _areasView.AI.ElementCount);

            _fireEvents = true;

            UpdateTrackBarLabels();
        }

        private void areasView_AI_CollectionChanged(object sender, EventArgs<TrackAIElement> e)
        {
            Maximum = _areasView.AI.ElementCount;
        }

        private void Area1TrackBarValueChanged(object sender, EventArgs e)
        {
            AreaTrackBarValueChanged(null,
                                          area1TrackBar, area1Label,
                                          area2TrackBar, area2Label);
        }

        private void Area2TrackBarValueChanged(object sender, EventArgs e)
        {
            AreaTrackBarValueChanged(area1TrackBar,
                                          area2TrackBar, area2Label,
                                          area3TrackBar, area3Label);
        }

        private void Area3TrackBarValueChanged(object sender, EventArgs e)
        {
            AreaTrackBarValueChanged(area2TrackBar,
                                          area3TrackBar, area3Label,
                                          area4TrackBar, area4Label);
        }

        private void Area4TrackBarValueChanged(object sender, EventArgs e)
        {
            AreaTrackBarValueChanged(area3TrackBar,
                                          area4TrackBar, area4Label,
                                          null, null);
        }

        private void AreaTrackBarValueChanged(TrackBar prevTrackBar, TrackBar trackBar, Label label, TrackBar nextTrackBar, Label nextLabel)
        {
            if (!_fireEvents)
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

            UpdateTrackBarLabel(label, prevTrackBar == null ? 0 : prevTrackBar.Value, trackBar.Value);

            if (nextTrackBar != null)
            {
                UpdateTrackBarLabel(nextLabel, trackBar.Value, nextTrackBar.Value);
            }

            _areasView.SetAreaValue((int)trackBar.Tag, (byte)trackBar.Value);
        }

        private static void UpdateTrackBarLabel(Label label, int value1, int value2)
        {
            label.Text =
                Utilities.ByteToHexString((byte)value1) + "-" +
                Utilities.ByteToHexString((byte)value2);
        }

        private void UpdateTrackBarLabels()
        {
            UpdateTrackBarLabel(area1Label, 0, area1TrackBar.Value);
            UpdateTrackBarLabel(area2Label, area1TrackBar.Value, area2TrackBar.Value);
            UpdateTrackBarLabel(area3Label, area2TrackBar.Value, area3TrackBar.Value);
            UpdateTrackBarLabel(area4Label, area3TrackBar.Value, area4TrackBar.Value);
        }
    }
}
