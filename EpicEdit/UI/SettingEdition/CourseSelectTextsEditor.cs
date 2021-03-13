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
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the text of each cup and theme.
    /// </summary>
    internal partial class CourseSelectTextsEditor : UserControl
    {
        private TextCollection _courseSelectTexts;
        private bool _fireEvents;

        public CourseSelectTextsEditor()
        {
            InitializeComponent();

            courseTextBox1.Tag = 0;
            courseTextBox2.Tag = 1;
            courseTextBox3.Tag = 2;
            courseTextBox4.Tag = 3;
            courseTextBox5.Tag = 4;
            courseTextBox6.Tag = 5;
            courseTextBox7.Tag = 6;
            courseTextBox8.Tag = 7;
            courseTextBox9.Tag = 8;
            courseTextBox10.Tag = 9;
            courseTextBox11.Tag = 10;
            courseTextBox12.Tag = 11;
            courseTextBox13.Tag = 12;
        }

        public void Init()
        {
            _courseSelectTexts = Context.Game.Settings.CourseSelectTexts;

            _fireEvents = false;

            courseTextBox1.Text = _courseSelectTexts[0].Value;
            courseTextBox2.Text = _courseSelectTexts[1].Value;
            courseTextBox3.Text = _courseSelectTexts[2].Value;
            courseTextBox4.Text = _courseSelectTexts[3].Value;
            courseTextBox5.Text = _courseSelectTexts[4].Value;
            courseTextBox6.Text = _courseSelectTexts[5].Value;
            courseTextBox7.Text = _courseSelectTexts[6].Value;
            courseTextBox8.Text = _courseSelectTexts[7].Value;
            courseTextBox9.Text = _courseSelectTexts[8].Value;
            courseTextBox10.Text = _courseSelectTexts[9].Value;
            courseTextBox11.Text = _courseSelectTexts[10].Value;
            courseTextBox12.Text = _courseSelectTexts[11].Value;
            courseTextBox13.Text = _courseSelectTexts[12].Value;

            UpdateCount(_courseSelectTexts, courseSelectTextsCountLabel);

            _fireEvents = true;
        }

        private static void UpdateCount(TextCollection textCollection, Label countLabel)
        {
            var total = textCollection.TotalCharacterCount;
            var max = textCollection.MaxCharacterCount;
            countLabel.Text = $"{total}/{max}";
            countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void CourseSelectTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            OnTextBoxTextChanged(sender, _courseSelectTexts, courseSelectTextsCountLabel);
        }

        private void OnTextBoxTextChanged(object sender, TextCollection textCollection, Label countLabel)
        {
            if (!_fireEvents)
            {
                return;
            }

            _fireEvents = false;

            var textBox = (TextBox)sender;
            var id = (int)textBox.Tag;
            var sel = textBox.SelectionStart;

            textCollection[id].Value = textBox.Text;

            textBox.Text = textCollection[id].Value; // Retrieve validated text
            textBox.SelectionStart = sel; // Restore text input position

            _fireEvents = true;

            UpdateCount(textCollection, countLabel);
        }
    }
}
