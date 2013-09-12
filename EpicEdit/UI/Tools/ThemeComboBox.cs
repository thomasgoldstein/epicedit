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
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A <see cref="ComboBox"/> that contains the track themes.
    /// </summary>
    public class ThemeComboBox : ComboBox
    {
        public void Init()
        {
            this.BeginUpdate();
            this.Items.Clear();

            foreach (Theme theme in Context.Game.Themes)
            {
                this.Items.Add(theme);
            }

            this.EndUpdate();
        }
    }
}
