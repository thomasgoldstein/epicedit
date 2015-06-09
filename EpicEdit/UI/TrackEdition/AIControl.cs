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

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Wrapper class around a generic EventArgs to make it work with the WinForms designer.
    /// </summary>
    internal class TrackAIElementEventArgs : EventArgs<TrackAIElement>
    {
        public TrackAIElementEventArgs(TrackAIElement value) : base(value) { }
    }

    /// <summary>
    /// Represents a collection of controls to edit <see cref="TrackAI"/>.
    /// </summary>
    internal partial class AIControl : UserControl
    {
        [Browsable(true)]
        public event EventHandler<EventArgs> AddElementRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ItemProbaEditorRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ElementChanged;

        [Browsable(true)]
        public event EventHandler<EventArgs> ElementAdded;

        [Browsable(true)]
        public event EventHandler<TrackAIElementEventArgs> ElementDeleted;

        [Browsable(true)]
        public event EventHandler<EventArgs> ElementsCleared;

        /// <summary>
        /// The current track.
        /// </summary>
        private Track track;

        /// <summary>
        /// The selected AI element.
        /// </summary>
        private TrackAIElement selectedElement;

        /// <summary>
        /// Gets or sets the selected AI element.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(TrackAIElement), "")]
        public TrackAIElement SelectedElement
        {
            get { return this.selectedElement; }
            set
            {
                this.selectedElement = value;

                if (this.selectedElement == null)
                {
                    this.selectedAIElementGroupBox.Enabled = false;
                }
                else
                {
                    this.selectedAIElementGroupBox.Enabled = true;

                    this.indexNumericUpDown.ValueChanged -= this.IndexNumericUpDownValueChanged;
                    this.indexNumericUpDown.Value = this.track.AI.GetElementIndex(this.selectedElement);
                    this.indexNumericUpDown.ValueChanged += this.IndexNumericUpDownValueChanged;

                    this.speedNumericUpDown.ValueChanged -= this.SpeedNumericUpDownValueChanged;
                    this.speedNumericUpDown.Value = this.selectedElement.Speed;
                    this.speedNumericUpDown.ValueChanged += this.SpeedNumericUpDownValueChanged;

                    this.shapeComboBox.SelectedIndexChanged -= this.ShapeComboBoxSelectedIndexChanged;
                    this.shapeComboBox.SelectedItem = this.selectedElement.ZoneShape;
                    this.shapeComboBox.SelectedIndexChanged += this.ShapeComboBoxSelectedIndexChanged;
                }

                // Force controls to refresh so that the new data shows up instantly.
                // NOTE: We could call this.selectedAIElementGroupBox.Refresh(); instead
                // but that would cause some minor flickering.
                this.indexNumericUpDown.Refresh();
                this.speedNumericUpDown.Refresh();
                this.shapeComboBox.Refresh();
            }
        }

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

                if (this.track != null)
                {
                    this.track.AI.DataChanged -= track_AI_DataChanged;
                    this.track.AI.ElementAdded -= this.track_AI_ElementAdded;
                    this.track.AI.ElementDeleted -= this.track_AI_ElementDeleted;
                    this.track.AI.ElementsCleared -= track_AI_ElementsCleared;
                }

                this.track = value;

                this.track.AI.DataChanged += track_AI_DataChanged;
                this.track.AI.ElementAdded += this.track_AI_ElementAdded;
                this.track.AI.ElementDeleted += this.track_AI_ElementDeleted;
                this.track.AI.ElementsCleared += track_AI_ElementsCleared;

                this.LoadItemProbabilitySet();
                this.SelectedElement = null;
                this.SetMaximumAIElementIndex();
                this.warningLabel.Visible = this.track.AI.ElementCount == 0;
            }
        }

        public AIControl()
        {
            this.InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            this.InitSetComboBox();
            this.shapeComboBox.DataSource = Enum.GetValues(typeof(Shape));
        }

        private void InitSetComboBox()
        {
            this.AddSetComboBoxItems();
            this.setComboBox.SelectedIndex = 0;
        }

        private void AddSetComboBoxItems()
        {
            this.setComboBox.BeginUpdate();

            for (int i = 0; i < ItemProbabilities.SetCount; i++)
            {
                this.setComboBox.Items.Add("Probability set " + (i + 1));
            }

            this.setComboBox.EndUpdate();
        }

        private void ResetSetComboBoxGP()
        {
            this.setComboBox.Items.Clear();
            this.AddSetComboBoxItems();
            this.setComboBox.Enabled = true;
        }

        private void ResetSetComboBoxBattle()
        {
            this.setComboBox.Items.Clear();
            TextCollection modeNames = Context.Game.Settings.ModeNames;
            this.setComboBox.Items.Add(modeNames[modeNames.Count - 1].FormattedValue);
            this.setComboBox.SelectedIndex = 0;
            this.setComboBox.Enabled = false;
        }

        private void SetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            GPTrack gpTrack = this.track as GPTrack;
            gpTrack.ItemProbabilityIndex = this.setComboBox.SelectedIndex;
        }

        private void ShapeComboBoxFormat(object sender, ListControlConvertEventArgs e)
        {
            e.Value = UITools.GetDescription(e.Value);
        }

        private void IndexNumericUpDownValueChanged(object sender, EventArgs e)
        {
            int oldIndex = this.track.AI.GetElementIndex(this.selectedElement);
            int newIndex = (int)this.indexNumericUpDown.Value;
            this.track.AI.ChangeElementIndex(oldIndex, newIndex);
        }

        private void SpeedNumericUpDownValueChanged(object sender, EventArgs e)
        {
            this.selectedElement.Speed = (byte)this.speedNumericUpDown.Value;
        }

        private void ShapeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedElement.ZoneShape = (Shape)this.shapeComboBox.SelectedValue;
        }

        private void CloneButtonClick(object sender, EventArgs e)
        {
            TrackAIElement aiElement = this.SelectedElement;
            TrackAIElement newAIElem = aiElement.Clone();

            // Shift the cloned element position, so it's not directly over the source element
            newAIElem.Location = new Point(aiElement.Location.X + TrackAIElement.Precision,
                                           aiElement.Location.Y + TrackAIElement.Precision);

            // Ensure the cloned element index is right after the source element
            int newAIElementIndex = this.track.AI.GetElementIndex(aiElement) + 1;

            this.track.AI.Insert(newAIElem, newAIElementIndex);
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            this.track.AI.Remove(this.SelectedElement);
        }

        private void LoadItemProbabilitySet()
        {
            this.setComboBox.SelectedIndexChanged -= this.SetComboBoxSelectedIndexChanged;

            GPTrack gpTrack = this.track as GPTrack;
            if (gpTrack != null)
            {
                if (!this.setComboBox.Enabled)
                {
                    this.ResetSetComboBoxGP();
                }

                this.setComboBox.SelectedIndex = gpTrack.ItemProbabilityIndex;
            }
            else
            {
                if (this.setComboBox.Enabled)
                {
                    this.ResetSetComboBoxBattle();
                }
            }

            this.setComboBox.SelectedIndexChanged += this.SetComboBoxSelectedIndexChanged;
        }

        private void ProbaEditorButtonClick(object sender, EventArgs e)
        {
            this.ItemProbaEditorRequested(this, EventArgs.Empty);
        }

        private void SetMaximumAIElementIndex()
        {
            this.indexNumericUpDown.ValueChanged -= this.IndexNumericUpDownValueChanged;
            this.indexNumericUpDown.Maximum = this.track.AI.ElementCount - 1;
            this.indexNumericUpDown.ValueChanged += this.IndexNumericUpDownValueChanged;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            this.AddElementRequested(this, EventArgs.Empty);
        }

        private void DeleteAllButtonClick(object sender, EventArgs e)
        {
            DialogResult result = UITools.ShowWarning("Do you really want to delete all AI elements?");

            if (result == DialogResult.Yes)
            {
                this.track.AI.Clear();
            }
        }

        private void track_AI_DataChanged(object sender, EventArgs e)
        {
            this.ElementChanged(this, EventArgs.Empty);
        }

        private void track_AI_ElementAdded(object sender, EventArgs<TrackAIElement> e)
        {
            this.SetMaximumAIElementIndex();
            this.SelectedElement = e.Value;

            if (this.track.AI.ElementCount > 0)
            {
                this.HideWarning();
            }

            this.ElementAdded(this, EventArgs.Empty);
        }

        private void track_AI_ElementDeleted(object sender, EventArgs<TrackAIElement> e)
        {
            this.SelectedElement = null;
            this.SetMaximumAIElementIndex();

            if (this.track.AI.ElementCount == 0)
            {
                this.ShowWarning();
            }

            this.ElementDeleted(this, new TrackAIElementEventArgs(e.Value));
        }

        private void track_AI_ElementsCleared(object sender, EventArgs e)
        {
            this.SelectedElement = null;
            this.ShowWarning();
            this.ElementsCleared(this, EventArgs.Empty);
        }

        private void ShowWarning()
        {
            this.warningLabel.Visible = true;
        }

        private void HideWarning()
        {
            this.warningLabel.Visible = false;
        }
    }
}
