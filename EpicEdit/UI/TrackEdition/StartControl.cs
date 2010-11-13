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
using System.Globalization;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.TrackEdition
{
	/// <summary>
	/// Represents a collection of controls to edit <see cref="LapLine"/> and <see cref="StartPosition"/> objects.
	/// </summary>
	public partial class StartControl : UserControl
	{
		[Browsable(true)]
		public event EventHandler<EventArgs> DataChanged;

		/// <summary>
		/// The current track.
		/// </summary>
		private Track track = null;

		/// <summary>
		/// Gets or sets the current track.
		/// </summary>
		[Browsable(false), DefaultValue(typeof(Track), "")]
		public Track Track
		{
			get
			{
				return this.track;
			}
			set
			{
				this.track = value;

				if (this.track is GPTrack)
				{
					GPTrack gpTrack = this.track as GPTrack;
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
		}

		public int Precision
		{
			get
			{
				int precision;

				if (this.step1pxRadioButton.Checked)
				{
					precision = 1;
				}
				else if (this.step4pxRadioButton.Checked)
				{
					precision = 4;
				}
				else //if (this.step8pxRadioButton.Checked)
				{
					precision = 8;
				}

				return precision;
			}
		}

		public bool LapLineAndDriverPositionsBound
		{
			get
			{
				return this.startBindCheckBox.Checked;
			}
		}

		private void SecondRowTrackBarScroll(object sender, EventArgs e)
		{
			GPTrack gpTrack = this.track as GPTrack;

			int valueBefore = gpTrack.StartPosition.SecondRowOffset;
			int step = this.Precision;
			gpTrack.StartPosition.SecondRowOffset = (this.secondRowTrackBar.Value / step) * step;
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
				this.secondRowValueLabel.Text = gpTrack.StartPosition.SecondRowOffset.ToString(CultureInfo.InvariantCulture);
			}
		}
	}
}
