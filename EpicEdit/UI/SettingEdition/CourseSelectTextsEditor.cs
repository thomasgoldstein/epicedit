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
        private TextCollection courseSelectTexts;
        private bool fireEvents;

        public CourseSelectTextsEditor()
        {
            this.InitializeComponent();

            this.courseTextBox1.Tag = 0;
            this.courseTextBox2.Tag = 1;
            this.courseTextBox3.Tag = 2;
            this.courseTextBox4.Tag = 3;
            this.courseTextBox5.Tag = 4;
            this.courseTextBox6.Tag = 5;
            this.courseTextBox7.Tag = 6;
            this.courseTextBox8.Tag = 7;
            this.courseTextBox9.Tag = 8;
            this.courseTextBox10.Tag = 9;
            this.courseTextBox11.Tag = 10;
            this.courseTextBox12.Tag = 11;
            this.courseTextBox13.Tag = 12;
        }

        public void Init()
        {
            this.courseSelectTexts = Context.Game.Settings.CourseSelectTexts;

            this.fireEvents = false;

            this.courseTextBox1.Text = this.courseSelectTexts[0].Value;
            this.courseTextBox2.Text = this.courseSelectTexts[1].Value;
            this.courseTextBox3.Text = this.courseSelectTexts[2].Value;
            this.courseTextBox4.Text = this.courseSelectTexts[3].Value;
            this.courseTextBox5.Text = this.courseSelectTexts[4].Value;
            this.courseTextBox6.Text = this.courseSelectTexts[5].Value;
            this.courseTextBox7.Text = this.courseSelectTexts[6].Value;
            this.courseTextBox8.Text = this.courseSelectTexts[7].Value;
            this.courseTextBox9.Text = this.courseSelectTexts[8].Value;
            this.courseTextBox10.Text = this.courseSelectTexts[9].Value;
            this.courseTextBox11.Text = this.courseSelectTexts[10].Value;
            this.courseTextBox12.Text = this.courseSelectTexts[11].Value;
            this.courseTextBox13.Text = this.courseSelectTexts[12].Value;

            UpdateCount(this.courseSelectTexts, this.courseSelectTextsCountLabel);

            this.fireEvents = true;
        }

        private static void UpdateCount(TextCollection textCollection, Label countLabel)
        {
            int total = textCollection.TotalCharacterCount;
            int max = textCollection.MaxCharacterCount;
            countLabel.Text = $"{total}/{max}";
            countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void CourseSelectTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.courseSelectTexts, this.courseSelectTextsCountLabel);
        }

        private void OnTextBoxTextChanged(object sender, TextCollection textCollection, Label countLabel)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.fireEvents = false;

            TextBox textBox = sender as TextBox;
            int id = (int)textBox.Tag;
            int sel = textBox.SelectionStart;

            textCollection[id].Value = textBox.Text;

            textBox.Text = textCollection[id].Value; // Retrieve validated text
            textBox.SelectionStart = sel; // Restore text input position

            this.fireEvents = true;

            UpdateCount(textCollection, countLabel);
        }
    }
}
