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
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A NumericUpDown control that adds 1 to the displayed value, so values show up as starting at 1 rather than 0.
    /// </summary>
    internal class EpicNumericUpDown : NumericUpDown
    {
        private const int StartValue = 1;

        /// <summary>
        /// Specifices whether the text is being changed (during control initialization, or by the user).
        /// </summary>
        private bool changingText = false;

        private decimal DisplayedValue
        {
            get { return this.Value + StartValue; }
        }

        protected override void OnTextBoxTextChanged(object source, EventArgs e)
        {
            if (this.Text == this.DisplayedValue.ToString())
            {
                // Do not call the base OnTextBoxTextChanged, otherwise the UpdateEditText logic
                // will cause the Value to be updated again, causing the Text to be updated again, and so on.
                return;
            }

            this.changingText = true;
            base.OnTextBoxTextChanged(source, e);
        }

        protected override void UpdateEditText()
        {
            if (this.changingText)
            {
                this.ParseEditText();
            }

            this.Text = this.DisplayedValue.ToString();
        }

        private new void ParseEditText()
        {
            // Ideally, we'd have overridden the base ParseEditText method, but it's not marked as virtual.

            this.changingText = false;

            decimal value;
            if (decimal.TryParse(this.Text, out value))
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
    }
}
