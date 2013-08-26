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
using System.Reflection;
using System.Windows.Forms;

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="TrackAI"/>.
    /// </summary>
    internal partial class AIControl : UserControl
    {
        [Browsable(true)]
        public event EventHandler<EventArgs> DataChanged;

        [Browsable(true)]
        public event EventHandler<EventArgs> DataChangedNoRepaint;

        [Browsable(true)]
        public event EventHandler<EventArgs> DeleteRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> AddRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> DeleteAllRequested;

        [Browsable(true)]
        public event EventHandler<EventArgs> ItemProbaEditorRequested;

        /// <summary>
        /// The current track.
        /// </summary>
        private Track track = null;

        /// <summary>
        /// The selected AI element.
        /// </summary>
        private TrackAIElement selectedElement = null;

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
                this.track = value;

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
            this.setComboBox.Items.Add(modeNames.GetFormattedText(modeNames.Count - 1));
            this.setComboBox.SelectedIndex = 0;
            this.setComboBox.Enabled = false;
        }

        private void SetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            GPTrack gpTrack = this.track as GPTrack;
            gpTrack.ItemProbabilityIndex = this.setComboBox.SelectedIndex;

            this.DataChangedNoRepaint(this, EventArgs.Empty);
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

            this.DataChanged(this, EventArgs.Empty);
        }

        private void SpeedNumericUpDownValueChanged(object sender, EventArgs e)
        {
            this.selectedElement.Speed = (byte)this.speedNumericUpDown.Value;

            this.DataChanged(this, EventArgs.Empty);
        }

        private void ShapeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            Shape newShape = (Shape)this.shapeComboBox.SelectedValue;
            this.selectedElement.ChangeShape(newShape);

            this.DataChanged(this, EventArgs.Empty);
        }

        private void DeleteButtonClick(object sender, EventArgs e)
        {
            this.DeleteRequested(this, EventArgs.Empty);
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

        public void SetMaximumAIElementIndex()
        {
            this.indexNumericUpDown.ValueChanged -= this.IndexNumericUpDownValueChanged;
            this.indexNumericUpDown.Maximum = this.track.AI.ElementCount - 1;
            this.indexNumericUpDown.ValueChanged += this.IndexNumericUpDownValueChanged;
        }

        private void AddButtonClick(object sender, EventArgs e)
        {
            this.AddRequested(this, EventArgs.Empty);
        }

        private void DeleteAllButtonClick(object sender, EventArgs e)
        {
            DialogResult result = UITools.ShowWarning("Do you really want to delete all AI elements?");

            if (result == DialogResult.Yes)
            {
                this.DeleteAllRequested(this, EventArgs.Empty);
            }
        }

        public void ShowWarning()
        {
            this.warningLabel.Visible = true;
        }

        public void HideWarning()
        {
            this.warningLabel.Visible = false;
        }
    }
}
