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
using System.Windows.Forms;
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A <see cref="ComboBox"/> that contains the track themes.
    /// </summary>
    internal class ThemeComboBox : ComboBox
    {
        public void Init()
        {
            this.DataSource = Context.Game.Themes.GetList();
        }

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme SelectedTheme
        {
            get { return this.SelectedItem as Theme; }
            set { this.SelectedItem = value; }
        }
    }
}
