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
    /// Represents a collection of controls to edit <see cref="EpicEdit.Rom.Tracks.Start.LapLine"/> and <see cref="EpicEdit.Rom.Tracks.Start.GPStartPosition"/> objects.
    /// </summary>
    internal partial class StartControl : UserControl
    {
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> DataChanged;

        /// <summary>
        /// The current track.
        /// </summary>
        private Track track;

        /// <summary>
        /// Gets or sets the current track.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track
        {
            get { return this.track; }
            set
            {
                if (this.track == value)
                {
                    return;
                }

                this.track = value;
                GPTrack gpTrack = this.track as GPTrack;

                if (gpTrack != null)
                {
                    this.gpTrackGroupBox.Enabled = true;
                    this.secondRowTrackBar.Value = gpTrack.StartPosition.SecondRowOffset;
                }
                else
                {
                    this.gpTrackGroupBox.Enabled = false;
                }
            }
        }

        public StartControl()
        {
            this.InitializeComponent();
            this.SetPrecision();
        }

        public int Precision { get; private set; }

        public bool LapLineAndDriverPositionsBound
        {
            get { return this.startBindCheckBox.Checked; }
        }

        private void SecondRowTrackBarScroll(object sender, EventArgs e)
        {
            GPTrack gpTrack = this.track as GPTrack;

            int valueBefore = gpTrack.StartPosition.SecondRowOffset;
            gpTrack.StartPosition.SecondRowOffset = (this.secondRowTrackBar.Value / this.Precision) * this.Precision;
            int valueAfter = gpTrack.StartPosition.SecondRowOffset;

            if (valueBefore != valueAfter)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }

        private void SecondRowTrackBarValueChanged(object sender, EventArgs e)
        {
            GPTrack gpTrack = this.track as GPTrack;

            if (this.secondRowTrackBar.Value != gpTrack.StartPosition.SecondRowOffset)
            {
                this.secondRowTrackBar.Value = gpTrack.StartPosition.SecondRowOffset;
            }
            else
            {
                this.secondRowValueLabel.Text = gpTrack.StartPosition.SecondRowOffset.ToString(CultureInfo.CurrentCulture);
            }
        }
        
        private void StepRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;

            // Avoid calling the method twice (once for the button that was previously checked, then the one newly checked)
            if (button.Checked)
            {
                this.SetPrecision();
            }
        }

        private void SetPrecision()
        {
            if (this.step1pxRadioButton.Checked)
            {
                this.Precision = 1;
            }
            else if (this.step4pxRadioButton.Checked)
            {
                this.Precision = 4;
            }
            else //if (this.step8pxRadioButton.Checked)
            {
                this.Precision = 8;
            }
            
            this.secondRowTrackBar.SmallChange = this.Precision;
            this.secondRowTrackBar.LargeChange = this.Precision * 5;
        }
    }
}
