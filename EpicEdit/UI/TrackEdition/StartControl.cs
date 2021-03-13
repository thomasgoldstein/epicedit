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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using EpicEdit.Rom.Tracks.Start;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="EpicEdit.Rom.Tracks.Start.LapLine"/> and <see cref="GPStartPosition"/> objects.
    /// </summary>
    internal partial class StartControl : UserControl
    {
        /// <summary>
        /// The current track.
        /// </summary>
        private Track _track;

        /// <summary>
        /// Gets or sets the current track.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track
        {
            get => _track;
            set
            {
                if (_track == value)
                {
                    return;
                }

                if (_track is GPTrack oldGPTrack)
                {
                    oldGPTrack.StartPosition.PropertyChanged -= gpTrack_StartPosition_PropertyChanged;
                }

                _track = value;

                if (!(_track is GPTrack gpTrack))
                {
                    gpTrackGroupBox.Enabled = false;
                }
                else
                {
                    gpTrackGroupBox.Enabled = true;

                    // NOTE: Temporarily detach the secondRowNumericUpDown.ValueChanged event handler
                    // so that the current precision does not alter the second row offset on track load.
                    secondRowNumericUpDown.ValueChanged -= SecondRowValueLabelNumericUpDownValueChanged;
                    secondRowNumericUpDown.Value = gpTrack.StartPosition.SecondRowOffset;
                    secondRowNumericUpDown.ValueChanged += SecondRowValueLabelNumericUpDownValueChanged;

                    secondRowTrackBar.Value = gpTrack.StartPosition.SecondRowOffset;
                    gpTrack.StartPosition.PropertyChanged += gpTrack_StartPosition_PropertyChanged;
                }
            }
        }

        public StartControl()
        {
            InitializeComponent();
            SetPrecision();
        }

        private void gpTrack_StartPosition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.GPStartPosition.SecondRowOffset)
            {
                GPTrack gpTrack = _track as GPTrack;
                secondRowNumericUpDown.Value = gpTrack.StartPosition.SecondRowOffset;
                secondRowTrackBar.Value = gpTrack.StartPosition.SecondRowOffset;
            }
        }

        public int Precision { get; private set; }

        public bool LapLineAndDriverPositionsBound => startBindCheckBox.Checked;

        private void SecondRowValueLabelNumericUpDownValueChanged(object sender, EventArgs e)
        {
            GPTrack gpTrack = _track as GPTrack;
            gpTrack.StartPosition.SecondRowOffset = GetPrecisionValue((int)secondRowNumericUpDown.Value);

            // Make sure the UI reflects the validated SecondRowOffset value
            secondRowNumericUpDown.Value = gpTrack.StartPosition.SecondRowOffset;
        }

        private void SecondRowTrackBarScroll(object sender, EventArgs e)
        {
            GPTrack gpTrack = _track as GPTrack;
            gpTrack.StartPosition.SecondRowOffset = GetPrecisionValue(secondRowTrackBar.Value);
        }

        private void SecondRowTrackBarValueChanged(object sender, EventArgs e)
        {
            GPTrack gpTrack = _track as GPTrack;
            // Make sure the UI reflects the validated SecondRowOffset value
            secondRowTrackBar.Value = gpTrack.StartPosition.SecondRowOffset;
        }

        private void StepRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = sender as RadioButton;

            // Avoid calling the method twice (once for the button that was previously checked, then the one newly checked)
            if (button.Checked)
            {
                SetPrecision();
            }
        }

        private void SetPrecision()
        {
            Precision =
                step1pxRadioButton.Checked ? 1 :
                step4pxRadioButton.Checked ? 4 :
                8;

            secondRowNumericUpDown.Increment = Precision;
            secondRowTrackBar.SmallChange = Precision;
            secondRowTrackBar.LargeChange = Precision * 5;
        }

        private int GetPrecisionValue(int value)
        {
            return (value / Precision) * Precision;
        }
    }
}
