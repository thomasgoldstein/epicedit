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
        protected override void UpdateEditText()
        {
            this.Text = (this.Value + 1).ToString();
        }
        
        protected override void OnTextBoxTextChanged(object source, EventArgs e)
        {
            // Do nothing, we don't want the UpdateEditText logic to cause the Value to be updated again,
            // causing the Text to be updated again, and so on.
        }
    }
}
