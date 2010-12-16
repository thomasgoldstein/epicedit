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
        /// <summary>
        /// Sets the Value of the passed EventArgs using the Description of the underlying Enum item.
        /// </summary>
        /// <param name="e"></param>
        public static void SetValueFromEnumDescription(ConvertEventArgs e)
        {
            if (!(e.Value is Enum))
            {
                // HACK: Do nothing in order to prevent an exception.
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
    }
}
