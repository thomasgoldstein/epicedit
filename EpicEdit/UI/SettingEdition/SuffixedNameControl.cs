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
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    internal partial class SuffixedNameControl : UserControl
    {
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs> SuffixTextChanged;

        private SuffixedTextItem _textItem;
        private bool _fireEvents;

        public SuffixedNameControl()
        {
            InitializeComponent();
        }

        public void Init(SuffixedTextItem textItem)
        {
            _fireEvents = false;

            if (_textItem != null)
            {
                _textItem.Suffix.PropertyChanged -= textItem_Suffix_PropertyChanged;
            }

            _textItem = textItem;
            _textItem.Suffix.PropertyChanged += textItem_Suffix_PropertyChanged;

            nameComboBox.Init(Context.Game.Settings.CourseSelectTexts);
            nameComboBox.SelectedItem = _textItem.TextItem;
            suffixTextBox.Text = _textItem.Suffix.Value;

            _fireEvents = true;
        }

        private TextItem SelectedTextItem => nameComboBox.SelectedItem as TextItem;

        private void NameComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            _textItem.TextItem = SelectedTextItem;
        }

        private void SuffixTextBoxTextChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            _fireEvents = false;

            int sel = suffixTextBox.SelectionStart;
            _textItem.Suffix.Value = suffixTextBox.Text;
            suffixTextBox.Text = _textItem.Suffix.Value; // Retrieve validated text
            suffixTextBox.SelectionStart = sel; // Restore text input position

            _fireEvents = true;
        }

        private void textItem_Suffix_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SuffixTextChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
