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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.TrackEdition
{
	/// <summary>
	/// Represents a collection of controls to edit <see cref="TrackAI"/>.
	/// </summary>
	public partial class AIControl : UserControl
	{
		[Browsable(true)]
		public event EventHandler<EventArgs> DataChanged;

		[Browsable(true)]
		public event EventHandler<EventArgs> DeleteRequested;

		/// <summary>
		/// The AI of the current track.
		/// </summary>
		private TrackAI trackAI = null;

		/// <summary>
		/// The selected AI element.
		/// </summary>
		private TrackAIElement selectedAIElem;

		/// <summary>
		/// Gets the selected AI element.
		/// </summary>
		[Browsable(false)]
		public TrackAIElement SelectedAIElem
		{
			get
			{
				return this.selectedAIElem;
			}
		}

		[Browsable(false), DefaultValue(typeof(TrackAI), "")]
		public TrackAI TrackAI
		{
			get
			{
				return this.trackAI;
			}
			set
			{
				this.trackAI = value;

				this.SetSelectedAIElement(null);
				this.SetMaximumAIElementIndex();
			}
		}

		public AIControl()
		{
			this.InitializeComponent();

			this.aiShapeComboBox.DataSource = Enum.GetValues(typeof(Shape));
			this.aiShapeComboBox.SelectedIndexChanged += new EventHandler(this.AIShapeComboBoxSelectedIndexChanged);
		}

		private void AIShapeComboBoxFormat(object sender, ListControlConvertEventArgs e)
		{
			UITools.SetValueFromEnumDescription(e);
		}

		public void SetSelectedAIElement(TrackAIElement aiElement)
		{
			this.selectedAIElem = aiElement;

			if (this.selectedAIElem == null)
			{
				this.selectedAIElementGroupBox.Enabled = false;
			}
			else
			{
				this.selectedAIElementGroupBox.Enabled = true;

				this.aiIndexNumericUpDown.ValueChanged -= new EventHandler(this.AIIndexNumericUpDownValueChanged);
				this.aiIndexNumericUpDown.Value = this.trackAI.GetElementIndex(this.selectedAIElem);
				this.aiIndexNumericUpDown.ValueChanged += new EventHandler(this.AIIndexNumericUpDownValueChanged);

				this.aiSpeedNumericUpDown.ValueChanged -= new EventHandler(this.AISpeedNumericUpDownValueChanged);
				this.aiSpeedNumericUpDown.Value = this.selectedAIElem.Speed + 1;
				this.aiSpeedNumericUpDown.ValueChanged += new EventHandler(this.AISpeedNumericUpDownValueChanged);

				this.aiShapeComboBox.SelectedIndexChanged -= new EventHandler(this.AIShapeComboBoxSelectedIndexChanged);
				this.aiShapeComboBox.SelectedItem = this.selectedAIElem.ZoneShape;
				this.aiShapeComboBox.SelectedIndexChanged += new EventHandler(this.AIShapeComboBoxSelectedIndexChanged);
			}

			// Force controls to refresh so that the new data shows up instantly
			// NOTE: we could do this.selectedAIElementGroupBox.Refresh(); instead
			// but that would cause some minor flickering
			this.aiIndexNumericUpDown.Refresh();
			this.aiSpeedNumericUpDown.Refresh();
			this.aiShapeComboBox.Refresh();
		}

		private void AIIndexNumericUpDownValueChanged(object sender, EventArgs e)
		{
			int oldIndex = this.trackAI.GetElementIndex(this.selectedAIElem);
			int newIndex = (int)this.aiIndexNumericUpDown.Value;
			this.trackAI.ChangeElementIndex(oldIndex, newIndex);
			
			this.DataChanged(this, EventArgs.Empty);
		}

		private void AISpeedNumericUpDownValueChanged(object sender, EventArgs e)
		{
			this.selectedAIElem.Speed = (byte)(this.aiSpeedNumericUpDown.Value - 1);

			this.DataChanged(this, EventArgs.Empty);
		}

		private void AIShapeComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			Shape newShape = (Shape)this.aiShapeComboBox.SelectedValue;
			this.selectedAIElem.ChangeShape(newShape);

			this.DataChanged(this, EventArgs.Empty);
		}

		private void AIDeleteButtonClick(object sender, EventArgs e)
		{
			this.DeleteRequested(this, EventArgs.Empty);
		}

		public void SetMaximumAIElementIndex()
		{
			this.aiIndexNumericUpDown.ValueChanged -= new EventHandler(this.AIIndexNumericUpDownValueChanged);
			this.aiIndexNumericUpDown.Maximum = this.trackAI.ElementCount - 1;
			this.aiIndexNumericUpDown.ValueChanged += new EventHandler(this.AIIndexNumericUpDownValueChanged);
		}
	}
}
