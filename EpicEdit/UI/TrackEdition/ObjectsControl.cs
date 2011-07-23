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
        public event EventHandler<EventArgs> DataChanged;

        [Browsable(true)]
        public event EventHandler<EventArgs> DataChangedNoRepaint;
        
        [Browsable(true)]
        public event EventHandler<EventArgs> ViewChanged;

        /// <summary>
        /// Used to get the index of the paletteNumericUpDown control.
        /// </summary>
        private Control[] palettePickers;

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

                this.tilesetComboBox.SelectedIndexChanged -= this.TilesetComboBoxSelectedIndexChanged;
                this.interactComboBox.SelectedIndexChanged -= this.InteractComboBoxSelectedIndexChanged;
                this.routineComboBox.SelectedIndexChanged -= this.RoutineComboBoxSelectedIndexChanged;
                this.palette1NumericUpDown.ValueChanged -= this.PaletteNumericUpDownValueChanged;
                this.palette2NumericUpDown.ValueChanged -= this.PaletteNumericUpDownValueChanged;
                this.palette3NumericUpDown.ValueChanged -= this.PaletteNumericUpDownValueChanged;
                this.palette4NumericUpDown.ValueChanged -= this.PaletteNumericUpDownValueChanged;
                this.flashingCheckBox.CheckedChanged -= this.FlashingCheckBoxCheckedChanged;

                this.tilesetComboBox.SelectedItem = value.ObjectTileset;
                this.interactComboBox.SelectedItem = value.ObjectInteraction;
                this.routineComboBox.SelectedItem = value.ObjectRoutine;
                this.palette1NumericUpDown.Value = value.ObjectPaletteIndexes[0] + 1;
                this.palette2NumericUpDown.Value = value.ObjectPaletteIndexes[1] + 1;
                this.palette3NumericUpDown.Value = value.ObjectPaletteIndexes[2] + 1;
                this.palette4NumericUpDown.Value = value.ObjectPaletteIndexes[3] + 1;
                this.flashingCheckBox.Checked = value.ObjectFlashing;

                this.tilesetComboBox.SelectedIndexChanged += this.TilesetComboBoxSelectedIndexChanged;
                this.interactComboBox.SelectedIndexChanged += this.InteractComboBoxSelectedIndexChanged;
                this.routineComboBox.SelectedIndexChanged += this.RoutineComboBoxSelectedIndexChanged;
                this.palette1NumericUpDown.ValueChanged += this.PaletteNumericUpDownValueChanged;
                this.palette2NumericUpDown.ValueChanged += this.PaletteNumericUpDownValueChanged;
                this.palette3NumericUpDown.ValueChanged += this.PaletteNumericUpDownValueChanged;
                this.palette4NumericUpDown.ValueChanged += this.PaletteNumericUpDownValueChanged;
                this.flashingCheckBox.CheckedChanged += this.FlashingCheckBoxCheckedChanged;

                this.ToggleZoneGroupBox();
                this.ToggleAlternatePalettes();
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
            this.ViewChanged(this, EventArgs.Empty);

            this.frontObjectZonesControl.Visible = this.frontZonesRadioButton.Checked;
            this.rearObjectZonesControl.Visible = this.rearZonesRadioButton.Checked;
        }

        private void FrontObjectZonesControlValueChanged(object sender, EventArgs e)
        {
            this.DataChanged(this, EventArgs.Empty);
        }

        private void RearObjectZonesControlValueChanged(object sender, EventArgs e)
        {
            this.DataChanged(this, EventArgs.Empty);
        }
        
        private void TilesetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.Track.ObjectTileset = (ObjectType)this.tilesetComboBox.SelectedItem;
            this.DataChanged(this, EventArgs.Empty);
        }
        
        private void InteractComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.Track.ObjectInteraction = (ObjectType)this.interactComboBox.SelectedItem;
            this.DataChangedNoRepaint(this, EventArgs.Empty);
        }
        
        private void RoutineComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.Track.ObjectRoutine = (ObjectType)this.routineComboBox.SelectedItem;
            this.ToggleZoneGroupBox();
            this.DataChanged(this, EventArgs.Empty);
        }

        private void ToggleZoneGroupBox()
        {
            this.zoneGroupBox.Enabled = this.Track.ObjectRoutine != ObjectType.Pillar;
        }
        
        private void PaletteNumericUpDownValueChanged(object sender, EventArgs e)
        {
            NumericUpDown control = sender as NumericUpDown;

            int index;
            for (index = 0; index < this.palettePickers.Length; index++)
            {
                if (control == palettePickers[index])
                {
                    break;
                }
            }

            this.Track.ObjectPaletteIndexes[index] = (byte)(control.Value - 1);

            if (index == 0)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
            else
            {
                // No visible change
                this.DataChangedNoRepaint(this, EventArgs.Empty);
            }
        }
        
        private void FlashingCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this.Track.ObjectFlashing = this.flashingCheckBox.Checked;
            this.ToggleAlternatePalettes();
            this.DataChangedNoRepaint(this, EventArgs.Empty);
        }

        private void ToggleAlternatePalettes()
        {
            bool enable = this.Track.ObjectFlashing;
            this.palettesLabel.Enabled = enable;
            this.palette2NumericUpDown.Enabled = enable;
            this.palette3NumericUpDown.Enabled = enable;
            this.palette4NumericUpDown.Enabled = enable;
        }
    }
}
