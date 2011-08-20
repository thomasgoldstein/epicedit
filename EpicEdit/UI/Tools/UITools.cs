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
using System.Reflection;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// Represents a UI helper class.
    /// </summary>
    public static class UITools
    {
        public static DialogResult ShowWarning(string message, MessageBoxButtons buttons)
        {
            return
                MessageBox.Show(
                    message,
                    Application.ProductName,
                    buttons,
                    MessageBoxIcon.Warning);
        }

        public static DialogResult ShowWarning(string message)
        {
            return UITools.ShowWarning(message, MessageBoxButtons.YesNo);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(
                message,
                Application.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Sets the Value of the passed EventArgs using the Description of the underlying Enum item.
        /// </summary>
        /// <param name="e"></param>
        public static void SetValueFromEnumDescription(ConvertEventArgs e)
        {
            if (!(e.Value is Enum))
            {
                // HACK: Do nothing in order to avoid an exception.
                // Workaround for Mono bug #620326
                // https://bugzilla.novell.com/show_bug.cgi?id=620326
                return;
            }
            
            bool foundDescription = false;
            Enum en = (Enum)e.Value;
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    foundDescription = true;
                    e.Value = ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            if (!foundDescription)
            {
                e.Value = en.ToString();
            }
        }

        /// <summary>
        /// Applies a workaround for a Microsoft bug which makes it so a tooltip
        /// no longer pops up after it has timed out once. Affects Windows XP but not Vista.
        /// </summary>
        public static void FixToolTip(Control control, ToolTip toolTip)
        {
            // HACK: See method summary. For more details, see:
            // http://stackoverflow.com/questions/559707/winforms-tooltip-will-not-re-appear-after-first-use
            control.MouseEnter += (s, ea) => { toolTip.Active = false; toolTip.Active = true; };
        }
    }
}
