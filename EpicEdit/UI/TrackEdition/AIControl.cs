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
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="TrackAI"/>.
    /// </summary>
    internal partial class AIControl : UserControl
    {
        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> AddElementRequested;

        [Browsable(true), Category("Action")]
        public event EventHandler<EventArgs> ItemProbaEditorRequested;

        private bool _fireEvents;

        /// <summary>
        /// The current track.
        /// </summary>
        private Track _track;

        /// <summary>
        /// The selected AI element.
        /// </summary>
        private TrackAIElement _selectedElement;

        /// <summary>
        /// Gets or sets the selected AI element.
        /// </summary>
        [Browsable(false), DefaultValue(typeof(TrackAIElement), "")]
        public TrackAIElement SelectedElement
        {
            get => _selectedElement;
            set
            {
                _selectedElement = value;

                if (_selectedElement == null)
                {
                    selectedAIElementGroupBox.Enabled = false;
                }
                else
                {
                    selectedAIElementGroupBox.Enabled = true;

                    SetMaximumAIElementIndex();

                    _fireEvents = false;

                    indexNumericUpDown.Value = _track.AI.GetElementIndex(_selectedElement);
                    speedNumericUpDown.Value = _selectedElement.Speed;
                    shapeComboBox.SelectedItem = _selectedElement.AreaShape;
                    isIntersectionCheckBox.Checked = _selectedElement.IsIntersection;

                    _fireEvents = true;
                }

                // Force controls to refresh so that the new data shows up instantly.
                // NOTE: We could call this.selectedAIElementGroupBox.Refresh(); instead
                // but that would cause some minor flickering.
                indexNumericUpDown.Refresh();
                speedNumericUpDown.Refresh();
                shapeComboBox.Refresh();
                isIntersectionCheckBox.Refresh();
            }
        }

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

                if (_track != null)
                {
                    _track.AI.PropertyChanged -= track_AI_PropertyChanged;
                    _track.AI.ElementAdded -= track_AI_ElementAdded;
                    _track.AI.ElementRemoved -= track_AI_ElementRemoved;
                    _track.AI.ElementsCleared -= track_AI_ElementsCleared;
                    
                    if (_track is GPTrack oldGPTrack)
                    {
                        oldGPTrack.PropertyChanged -= gpTrack_PropertyChanged;
                    }
                }

                _track = value;

                _track.AI.PropertyChanged += track_AI_PropertyChanged;
                _track.AI.ElementAdded += track_AI_ElementAdded;
                _track.AI.ElementRemoved += track_AI_ElementRemoved;
                _track.AI.ElementsCleared += track_AI_ElementsCleared;

                if (_track is GPTrack gpTrack)
                {
                    gpTrack.PropertyChanged += gpTrack_PropertyChanged;
                }

                SelectedElement = null;
                LoadItemProbabilitySet();
                SetMaximumAIElementIndex();
                warningLabel.Visible = _track.AI.ElementCount == 0;
            }
        }

        public AIControl()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitSetComboBox();
            shapeComboBox.DataSource = Enum.GetValues(typeof(TrackAIElementShape));
        }

        private void InitSetComboBox()
        {
            AddSetComboBoxItems();
            setComboBox.SelectedIndex = 0;
        }

        private void AddSetComboBoxItems()
        {
            setComboBox.BeginUpdate();

            for (var i = 0; i < ItemProbabilities.SetCount; i++)
            {
                setComboBox.Items.Add("Probability set " + (i + 1));
            }

            setComboBox.EndUpdate();
        }

        private void ResetSetComboBoxGP()
        {
            setComboBox.Items.Clear();
            AddSetComboBoxItems();
            setComboBox.Enabled = true;
        }

        private void ResetSetComboBoxBattle()
        {
            setComboBox.Items.Clear();
            var modeNames = Context.Game.Settings.ModeNames;
            setComboBox.Items.Add(modeNames[modeNames.Count - 1].FormattedValue);
            setComboBox.SelectedIndex = 0;
            setComboBox.Enabled = false;
        }

        private void SetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            var gpTrack = _track as GPTrack;
            gpTrack.ItemProbabilityIndex = setComboBox.SelectedIndex;
        }

        private void ShapeComboBoxFormat(object sender, ListControlConvertEventArgs e)
        {
            e.Value = UITools.GetDescription(e.Value);
        }

        private void IndexNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            var oldIndex = _track.AI.GetElementIndex(_selectedElement);
            var newIndex = (int)indexNumericUpDown.Value;
            _track.AI.ChangeElementIndex(oldIndex, newIndex);
        }

        private void SpeedNumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            _selectedElement.Speed = (byte)speedNumericUpDown.Value;
        }

        private void ShapeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            _selectedElement.AreaShape = (TrackAIElementShape)shapeComboBox.SelectedValue;
        }

        private void IsIntersectionCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            _selectedElement.IsIntersection = isIntersectionCheckBox.Checked;
        }

        private void CloneButtonClick(object sender, EventArgs e)
        {
            var aiElement = SelectedElement;
            var newAIElem = aiElement.Clone();

            // Shift the cloned element position, so it's not directly over the source element
            newAIElem.Location = new Point(aiElement.Location.X + TrackAIElement.Precision,
                                           aiElement.Location.Y + TrackAIElement.Precision);

            // Ensure the cloned element index is right after the source element
            var newAIElementIndex = _track.AI.GetElementIndex(aiElement) + 1;

            _track.AI.Insert(newAIElem, newAIElementIndex);
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            _track.AI.Remove(SelectedElement);
        }

        private void LoadItemProbabilitySet()
        {
            _fireEvents = false;

            if (_track is GPTrack gpTrack)
            {
                if (!setComboBox.Enabled)
                {
                    ResetSetComboBoxGP();
                }

                setComboBox.SelectedIndex = gpTrack.ItemProbabilityIndex;
            }
            else
            {
                if (setComboBox.Enabled)
                {
                    ResetSetComboBoxBattle();
                }
            }

            _fireEvents = true;
        }

        private void ProbaEditorButtonClick(object sender, EventArgs e)
        {
            ItemProbaEditorRequested(this, EventArgs.Empty);
        }

        private void SetMaximumAIElementIndex()
        {
            _fireEvents = false;
            indexNumericUpDown.Maximum = _track.AI.ElementCount - 1;
            _fireEvents = true;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            AddElementRequested(this, EventArgs.Empty);
        }

        private void DeleteAllButtonClick(object sender, EventArgs e)
        {
            var result = UITools.ShowWarning("Do you really want to delete all AI elements?");

            if (result == DialogResult.Yes)
            {
                _track.AI.Clear();
            }
        }

        private void track_AI_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var aiElement = sender as TrackAIElement;

            if (SelectedElement != aiElement)
            {
                return;
            }

            switch (e.PropertyName)
            {
                case PropertyNames.TrackAIElement.Index:
                    indexNumericUpDown.Value = _track.AI.GetElementIndex(_selectedElement);
                    break;

                case PropertyNames.TrackAIElement.Speed:
                    speedNumericUpDown.Value = aiElement.Speed;
                    break;

                case PropertyNames.TrackAIElement.AreaShape:
                    shapeComboBox.SelectedItem = aiElement.AreaShape;
                    break;

                case PropertyNames.TrackAIElement.IsIntersection:
                    isIntersectionCheckBox.Checked = aiElement.IsIntersection;
                    break;
            }
        }

        private void gpTrack_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyNames.GPTrack.ItemProbabilityIndex)
            {
                setComboBox.SelectedIndex = (_track as GPTrack).ItemProbabilityIndex;
            }
        }

        private void track_AI_ElementAdded(object sender, EventArgs<TrackAIElement> e)
        {
            SetMaximumAIElementIndex();

            if (_track.AI.ElementCount > 0)
            {
                HideWarning();
            }
        }

        private void track_AI_ElementRemoved(object sender, EventArgs<TrackAIElement> e)
        {
            SetMaximumAIElementIndex();

            if (_track.AI.ElementCount == 0)
            {
                ShowWarning();
            }
        }

        private void track_AI_ElementsCleared(object sender, EventArgs e)
        {
            ShowWarning();
        }

        private void ShowWarning()
        {
            warningLabel.Visible = true;
        }

        private void HideWarning()
        {
            warningLabel.Visible = false;
        }
    }
}
