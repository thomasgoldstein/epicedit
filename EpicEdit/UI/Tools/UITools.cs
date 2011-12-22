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
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
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
        /// Gets the passed object description attribute value.
        /// </summary>
        /// <param name="item">Object instance.</param>
        public static string GetDescription(object item)
        {
            string desc = null;

            Type type = item.GetType();
            MemberInfo[] memInfo = type.GetMember(item.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    desc = (attrs[0] as DescriptionAttribute).Description;
                }
            }

            if (desc == null) // Description not found
            {
                desc = item.ToString();
            }

            return desc;
        }

        /// <summary>
        /// Removes invalid file name characters.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Sanitized file name.</returns>
        public static string SanitizeFileName(string fileName)
        {
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            invalidChars = "[" + Regex.Escape(invalidChars) + "]*";
            fileName = Regex.Replace(fileName, invalidChars, string.Empty);
            return fileName;
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
