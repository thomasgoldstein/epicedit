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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A <see cref="ComboBox"/> that contains the track themes.
    /// </summary>
    internal class ThemeComboBox : ComboBox
    {
        private Dictionary<TextItem, int> indexDictionary;

        public ThemeComboBox()
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.indexDictionary = new Dictionary<TextItem, int>();
        }

        public void Init()
        {
            this.indexDictionary.Clear();

            this.BeginUpdate();
            this.Items.Clear();
            int index = 0;

            foreach (Theme theme in Context.Game.Themes)
            {
                this.indexDictionary.Add(theme.NameItem, index++);
                theme.NameItem.PropertyChanged += this.themeNameItem_PropertyChanged;
                this.Items.Add(theme);
            }

            this.EndUpdate();
        }

        private void themeNameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = this.indexDictionary[sender as TextItem];
            this.Items[index] = Context.Game.Themes[index];
        }

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme SelectedTheme
        {
            get { return this.SelectedItem as Theme; }
            set { this.SelectedItem = value; }
        }
    }
}
