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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A <see cref="ComboBox"/> that contains text items.
    /// </summary>
    internal class TextItemComboBox : ComboBox
    {
        private readonly Dictionary<TextItem, int> _indexDictionary;
        private TextCollection _textCollection;

        public TextItemComboBox()
        {
            DropDownStyle = ComboBoxStyle.DropDownList;
            _indexDictionary = new Dictionary<TextItem, int>();
        }

        public void Init(TextCollection textCollection)
        {
            _textCollection = textCollection;
            _indexDictionary.Clear();

            BeginUpdate();
            Items.Clear();
            int index = 0;

            foreach (TextItem textItem in _textCollection)
            {
                _indexDictionary.Add(textItem, index++);
                textItem.PropertyChanged += textItem_PropertyChanged;
                Items.Add(textItem);
            }

            EndUpdate();
        }

        private void textItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = _indexDictionary[sender as TextItem];
            Items[index] = _textCollection[index];
        }
    }
}
