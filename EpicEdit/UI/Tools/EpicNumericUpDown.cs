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
using System.Globalization;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A NumericUpDown control that adds 1 to the displayed value, so values show up as starting at 1 rather than 0.
    /// Also allows looping from the first value to the last value, and vice versa.
    /// </summary>
    internal class EpicNumericUpDown : NumericUpDown
    {
        private const int StartValue = 1;

        private decimal DisplayedValue => this.Value + StartValue;

        #region Allow looping from the first value to the last value, and vice versa

        public EpicNumericUpDown()
        {
            this.Maximum = base.Maximum;
            this.Minimum = base.Minimum;
        }

        public new decimal Maximum
        {
            get => base.Maximum;
            set => base.Maximum = value + 1;
        }

        public new decimal Minimum
        {
            get => base.Minimum;
            set => base.Minimum = value - 1;
        }

        protected override void OnValueChanged(EventArgs e)
        {
            if (this.Value == base.Maximum)
            {
                this.Value = base.Minimum + 1;
                return;
            }
            
            if (this.Value == base.Minimum)
            {
                this.Value = base.Maximum - 1;
                return;
            }

            base.OnValueChanged(e);
        }

        #endregion Allow looping from the first value to the last value, and vice versa

        #region Add 1 to the displayed value

        protected override void UpdateEditText()
        {
            this.Text = this.DisplayedValue.ToString(CultureInfo.InvariantCulture);
        }

        protected override void ValidateEditText()
        {
            this.ParseEditText();
            this.UpdateEditText();
        }

        private new void ParseEditText()
        {
            // Ideally, we'd have overridden the base ParseEditText method, but it's not marked as virtual.
            try
            {
                if (decimal.TryParse(this.Text, out decimal value))
                {
                    value -= StartValue;

                    if (value < this.Minimum)
                    {
                        value = this.Minimum;
                    }
                    else if (value > this.Maximum)
                    {
                        value = this.Maximum;
                    }

                    this.Value = value;
                }
            }
            finally
            {
                this.UserEdit = false;
            }
        }

        #endregion Add 1 to the displayed value
    }
}
