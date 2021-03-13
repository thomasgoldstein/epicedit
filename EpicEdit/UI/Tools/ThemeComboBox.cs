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

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A <see cref="ComboBox"/> that contains the track themes.
    /// </summary>
    internal class ThemeComboBox : ComboBox
    {
        private readonly Dictionary<TextItem, int> _indexDictionary;

        public ThemeComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            _indexDictionary = new Dictionary<TextItem, int>();
        }

        public void Init()
        {
            _indexDictionary.Clear();

            BeginUpdate();
            Items.Clear();
            int index = 0;

            foreach (Theme theme in Context.Game.Themes)
            {
                _indexDictionary.Add(theme.NameItem, index++);
                theme.NameItem.PropertyChanged += themeNameItem_PropertyChanged;
                Items.Add(theme);
            }

            EndUpdate();
        }

        private void themeNameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = _indexDictionary[sender as TextItem];
            Items[index] = Context.Game.Themes[index];
        }

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme SelectedTheme
        {
            get => SelectedItem as Theme;
            set => SelectedItem = value;
        }
    }
}
