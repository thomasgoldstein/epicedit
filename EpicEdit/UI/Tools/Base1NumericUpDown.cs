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

using System.Globalization;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A NumericUpDown control that adds 1 to the displayed value, so values show up as starting at 1 rather than 0.
    /// Also allows looping from the first value to the last value, and vice versa.
    /// </summary>
    internal class Base1NumericUpDown : NumericUpDown
    {
        private const int StartValue = 1;

        private decimal DisplayedValue => Value + StartValue;

        #region Allow looping from the first value to the last value, and vice versa

        public Base1NumericUpDown()
        {
            Maximum = Maximum;
            Minimum = Minimum;
        }
        public override void UpButton()
        {
            if (Value < Maximum)
            {
                Value++;
            }
            else
            {
                Value = Minimum;
            }
        }
        public override void DownButton()
        {
            if (Value > Minimum)
            {
                Value--;
            }
            else
            {
                Value = Maximum;
            }
        }

        #endregion Allow looping from the first value to the last value, and vice versa

        #region Add 1 to the displayed value

        protected override void UpdateEditText()
        {
            Text = DisplayedValue.ToString(CultureInfo.InvariantCulture);
        }

        protected override void ValidateEditText()
        {
            ParseEditText();
            UpdateEditText();
        }

        private new void ParseEditText()
        {
            // Ideally, we'd have overridden the base ParseEditText method, but it's not marked as virtual.
            try
            {
                if (decimal.TryParse(Text, out var value))
                {
                    value -= StartValue;

                    if (value < Minimum)
                    {
                        value = Minimum;
                    }
                    else if (value > Maximum)
                    {
                        value = Maximum;
                    }

                    Value = value;
                }
            }
            finally
            {
                UserEdit = false;
            }
        }

        #endregion Add 1 to the displayed value
    }
}
