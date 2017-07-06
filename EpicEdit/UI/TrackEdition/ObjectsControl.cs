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

        private bool fireEvents;

        /// <summary>
        /// Used to get the index of the paletteNumericUpDown control.
        /// </summary>
        private readonly Control[] palettePickers;

        public ObjectsControl()
        {
            this.InitializeComponent();

            this.tilesetComboBox.DataSource = Enum.GetValues(typeof(ObjectType));
            this.interactComboBox.DataSource = Enum.GetValues(typeof(ObjectType));
            this.routineComboBox.DataSource = Enum.GetValues(typeof(ObjectType));

            this.palettePickers = new Control[]
            {
                this.palette1NumericUpDown,
                this.palette2NumericUpDown,
                this.palette3NumericUpDown,
                this.palette4NumericUpDown
            };
        }

        private GPTrack track;

        [Browsable(false), DefaultValue(typeof(GPTrack), "")]
        public GPTrack Track
        {
            get { return this.track; }
            set
            {
                if (this.track == value)
                {
                    return;
                }

                if (this.track != null)
                {
                    this.track.Objects.PropertyChanged -= this.track_Objects_PropertyChanged;
                }

                this.track = value;

                if (this.track == null) // BattleTrack
                {
                    this.Enabled = false;
                    return;
                }

                this.Enabled = true;

                this.track.Objects.PropertyChanged += this.track_Objects_PropertyChanged;

                this.frontObjectZonesControl.ZonesView = this.track.Objects.Zones.FrontView;
                this.rearObjectZonesControl.ZonesView = this.track.Objects.Zones.RearView;

                TrackObjects objects = this.track.Objects;

                this.fireEvents = false;

                this.tilesetComboBox.SelectedItem = objects.Tileset;
                this.interactComboBox.SelectedItem = objects.Interaction;
                this.routineComboBox.SelectedItem = objects.Routine;
                this.palette1NumericUpDown.Value = objects.PaletteIndexes[0];
                this.palette2NumericUpDown.Value = objects.PaletteIndexes[1];
                this.palette3NumericUpDown.Value = objects.PaletteIndexes[2];
                this.palette4NumericUpDown.Value = objects.PaletteIndexes[3];
                this.flashingCheckBox.Checked = objects.Flashing;

                this.fireEvents = true;

                this.ToggleZoneGroupBox();
                this.ToggleAlternatePalettes();
            }
        }

        private void track_Objects_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case PropertyNames.TrackObjectProperties.Tileset:
                    this.tilesetComboBox.SelectedItem = this.Track.Objects.Tileset;
                    break;

                case PropertyNames.TrackObjectProperties.Interaction:
                    this.interactComboBox.SelectedItem = this.Track.Objects.Interaction;
                    break;

                case PropertyNames.TrackObjectProperties.Routine:
                    this.routineComboBox.SelectedItem = this.Track.Objects.Routine;
                    break;

                case PropertyNames.TrackObjectProperties.PaletteIndexes:
                    this.palette1NumericUpDown.Value = this.Track.Objects.PaletteIndexes[0];
                    this.palette2NumericUpDown.Value = this.Track.Objects.PaletteIndexes[1];
                    this.palette3NumericUpDown.Value = this.Track.Objects.PaletteIndexes[2];
                    this.palette4NumericUpDown.Value = this.Track.Objects.PaletteIndexes[3];
                    break;

                case PropertyNames.TrackObjectProperties.Flashing:
                    this.flashingCheckBox.Checked = this.Track.Objects.Flashing;
                    break;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current view is the front-zones one.
        /// </summary>
        public bool FrontZonesView
        {
            get { return this.frontZonesRadioButton.Checked; }
        }

        private void FrontZonesRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            this.ViewChanged(this, EventArgs.Empty);

            this.frontObjectZonesControl.Visible = this.frontZonesRadioButton.Checked;
            this.rearObjectZonesControl.Visible = this.rearZonesRadioButton.Checked;
        }

        private void TilesetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.Track.Objects.Tileset = (ObjectType)this.tilesetComboBox.SelectedItem;
        }

        private void InteractComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.Track.Objects.Interaction = (ObjectType)this.interactComboBox.SelectedItem;
        }

        private void RoutineComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.Track.Objects.Routine = (ObjectType)this.routineComboBox.SelectedItem;
            this.ToggleZoneGroupBox();
        }

        private void ToggleZoneGroupBox()
        {
            this.zoneGroupBox.Enabled = this.Track.Objects.Routine != ObjectType.Pillar;
        }

        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            NumericUpDown control = sender as NumericUpDown;

            int index;
            for (index = 0; index < this.palettePickers.Length; index++)
            {
                if (control == this.palettePickers[index])
                {
                    break;
                }
            }

            this.Track.Objects.PaletteIndexes[index] = (byte)control.Value;
        }

        private void FlashingCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.Track.Objects.Flashing = this.flashingCheckBox.Checked;
            this.ToggleAlternatePalettes();
        }

        private void ToggleAlternatePalettes()
        {
            bool enable = this.Track.Objects.Flashing;
            this.palettesLabel.Enabled = enable;
            this.palette2NumericUpDown.Enabled = enable;
            this.palette3NumericUpDown.Enabled = enable;
            this.palette4NumericUpDown.Enabled = enable;
        }
    }
}
