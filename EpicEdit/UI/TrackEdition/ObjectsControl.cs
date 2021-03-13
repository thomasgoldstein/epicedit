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
using EpicEdit.Rom.Tracks.Objects;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="TrackObjects"/>.
    /// </summary>
    internal partial class ObjectsControl : UserControl
    {
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> ViewChanged;

        private bool _fireEvents;

        /// <summary>
        /// Used to get the index of the paletteNumericUpDown control.
        /// </summary>
        private readonly Control[] _palettePickers;

        public ObjectsControl()
        {
            InitializeComponent();

            tilesetComboBox.DataSource = Enum.GetValues(typeof(TrackObjectType));
            interactComboBox.DataSource = Enum.GetValues(typeof(TrackObjectType));
            routineComboBox.DataSource = Enum.GetValues(typeof(TrackObjectType));

            _palettePickers = new Control[]
            {
                palette1NumericUpDown,
                palette2NumericUpDown,
                palette3NumericUpDown,
                palette4NumericUpDown
            };
        }

        private GPTrack _track;

        [Browsable(false), DefaultValue(typeof(GPTrack), "")]
        public GPTrack Track
        {
            get => _track;
            set
            {
                if (_track == value)
                {
                    return;
                }

                if (_track != null)
                {
                    _track.Objects.PropertyChanged -= track_Objects_PropertyChanged;
                }

                _track = value;

                if (_track == null) // BattleTrack
                {
                    Enabled = false;
                    return;
                }

                Enabled = true;

                _track.Objects.PropertyChanged += track_Objects_PropertyChanged;

                frontObjectAreasControl.AreasView = _track.Objects.Areas.FrontView;
                rearObjectAreasControl.AreasView = _track.Objects.Areas.RearView;

                var objects = _track.Objects;

                _fireEvents = false;

                tilesetComboBox.SelectedItem = objects.Tileset;
                interactComboBox.SelectedItem = objects.Interaction;
                routineComboBox.SelectedItem = objects.Routine;
                palette1NumericUpDown.Value = objects.PaletteIndexes[0];
                palette2NumericUpDown.Value = objects.PaletteIndexes[1];
                palette3NumericUpDown.Value = objects.PaletteIndexes[2];
                palette4NumericUpDown.Value = objects.PaletteIndexes[3];
                flashingCheckBox.Checked = objects.Flashing;

                _fireEvents = true;

                ToggleAreaGroupBox();
                ToggleAlternatePalettes();
            }
        }

        private void track_Objects_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case PropertyNames.TrackObjectProperties.Tileset:
                    tilesetComboBox.SelectedItem = Track.Objects.Tileset;
                    break;

                case PropertyNames.TrackObjectProperties.Interaction:
                    interactComboBox.SelectedItem = Track.Objects.Interaction;
                    break;

                case PropertyNames.TrackObjectProperties.Routine:
                    routineComboBox.SelectedItem = Track.Objects.Routine;
                    break;

                case PropertyNames.TrackObjectProperties.PaletteIndexes:
                    palette1NumericUpDown.Value = Track.Objects.PaletteIndexes[0];
                    palette2NumericUpDown.Value = Track.Objects.PaletteIndexes[1];
                    palette3NumericUpDown.Value = Track.Objects.PaletteIndexes[2];
                    palette4NumericUpDown.Value = Track.Objects.PaletteIndexes[3];
                    break;

                case PropertyNames.TrackObjectProperties.Flashing:
                    flashingCheckBox.Checked = Track.Objects.Flashing;
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current view is the front-areas one.
        /// </summary>
        public bool FrontAreasView => frontAreasRadioButton.Checked;

        private void FrontAreasRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            ViewChanged(this, EventArgs.Empty);

            frontObjectAreasControl.Visible = frontAreasRadioButton.Checked;
            rearObjectAreasControl.Visible = rearAreasRadioButton.Checked;
        }

        private void TilesetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            Track.Objects.Tileset = (TrackObjectType)tilesetComboBox.SelectedItem;
        }

        private void InteractComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            Track.Objects.Interaction = (TrackObjectType)interactComboBox.SelectedItem;
        }

        private void RoutineComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            Track.Objects.Routine = (TrackObjectType)routineComboBox.SelectedItem;
            ToggleAreaGroupBox();
        }

        private void ToggleAreaGroupBox()
        {
            areaGroupBox.Enabled = Track.Objects.Routine != TrackObjectType.Pillar;
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            var control = (NumericUpDown)sender;

            int index;
            for (index = 0; index < _palettePickers.Length; index++)
            {
                if (control == _palettePickers[index])
                {
                    break;
                }
            }

            Track.Objects.PaletteIndexes[index] = (byte)control.Value;
        }

        private void FlashingCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            Track.Objects.Flashing = flashingCheckBox.Checked;
            ToggleAlternatePalettes();
        }

        private void ToggleAlternatePalettes()
        {
            var enable = Track.Objects.Flashing;
            palettesLabel.Enabled = enable;
            palette2NumericUpDown.Enabled = enable;
            palette3NumericUpDown.Enabled = enable;
            palette4NumericUpDown.Enabled = enable;
        }
    }
}
