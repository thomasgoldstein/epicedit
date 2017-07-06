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
    internal partial class CupAndThemeTextsEditor : UserControl
    {
        private TextCollection gpCupSelectTexts;
        private TextCollection gpPodiumCupTexts;
        private TextCollection cupAndThemeTexts;
        private bool fireEvents;

        public CupAndThemeTextsEditor()
        {
            this.InitializeComponent();

            this.csTextBox1.Tag = 0;
            this.csTextBox2.Tag = 1;
            this.csTextBox3.Tag = 2;
            this.csTextBox4.Tag = 3;

            this.pcTextBox1.Tag = 0;
            this.pcTextBox2.Tag = 1;
            this.pcTextBox3.Tag = 2;
            this.pcTextBox4.Tag = 3;
            this.pcTextBox5.Tag = 4;

            this.ctTextBox1.Tag = 0;
            this.ctTextBox2.Tag = 1;
            this.ctTextBox3.Tag = 2;
            this.ctTextBox4.Tag = 3;
            this.ctTextBox5.Tag = 4;
            this.ctTextBox6.Tag = 5;
            this.ctTextBox7.Tag = 6;
            this.ctTextBox8.Tag = 7;
            this.ctTextBox9.Tag = 8;
            this.ctTextBox10.Tag = 9;
            this.ctTextBox11.Tag = 10;
            this.ctTextBox12.Tag = 11;
            this.ctTextBox13.Tag = 12;
        }

        public void Init()
        {
            this.gpCupSelectTexts = Context.Game.Settings.GPCupSelectTexts;
            this.gpPodiumCupTexts = Context.Game.Settings.GPPodiumCupTexts;
            this.cupAndThemeTexts = Context.Game.Settings.CupAndThemeTexts;

            this.fireEvents = false;

            if (this.gpCupSelectTexts == null)
            {
                // NOTE: Japanese ROM, text editing not supported for GP cup names
                this.gpCupSelectTextsGroupBox.Enabled = false;

                this.csTextBox1.Text = string.Empty;
                this.csTextBox2.Text = string.Empty;
                this.csTextBox3.Text = string.Empty;
                this.csTextBox4.Text = string.Empty;

                this.gpCupSelectTextsCountLabel.Text = string.Empty;
            }
            else
            {
                this.gpCupSelectTextsGroupBox.Enabled = true;

                this.csTextBox1.Text = this.gpCupSelectTexts[0].Value;
                this.csTextBox2.Text = this.gpCupSelectTexts[1].Value;
                this.csTextBox3.Text = this.gpCupSelectTexts[2].Value;
                this.csTextBox4.Text = this.gpCupSelectTexts[3].Value;

                UpdateCount(this.gpCupSelectTexts, this.gpCupSelectTextsCountLabel);
            }

            this.pcTextBox1.Text = this.gpPodiumCupTexts[0].Value;
            this.pcTextBox2.Text = this.gpPodiumCupTexts[1].Value;
            this.pcTextBox3.Text = this.gpPodiumCupTexts[2].Value;
            this.pcTextBox4.Text = this.gpPodiumCupTexts[3].Value;
            this.pcTextBox5.Text = this.gpPodiumCupTexts[4].Value;

            UpdateCount(this.gpPodiumCupTexts, this.gpPodiumCupTextsCountLabel);

            this.ctTextBox1.Text = this.cupAndThemeTexts[0].Value;
            this.ctTextBox2.Text = this.cupAndThemeTexts[1].Value;
            this.ctTextBox3.Text = this.cupAndThemeTexts[2].Value;
            this.ctTextBox4.Text = this.cupAndThemeTexts[3].Value;
            this.ctTextBox5.Text = this.cupAndThemeTexts[4].Value;
            this.ctTextBox6.Text = this.cupAndThemeTexts[5].Value;
            this.ctTextBox7.Text = this.cupAndThemeTexts[6].Value;
            this.ctTextBox8.Text = this.cupAndThemeTexts[7].Value;
            this.ctTextBox9.Text = this.cupAndThemeTexts[8].Value;
            this.ctTextBox10.Text = this.cupAndThemeTexts[9].Value;
            this.ctTextBox11.Text = this.cupAndThemeTexts[10].Value;
            this.ctTextBox12.Text = this.cupAndThemeTexts[11].Value;
            this.ctTextBox13.Text = this.cupAndThemeTexts[12].Value;

            UpdateCount(this.cupAndThemeTexts, this.cupAndThemeTextsCountLabel);

            this.fireEvents = true;
        }

        private static void UpdateCount(TextCollection textCollection, Label countLabel)
        {
            int total = textCollection.TotalCharacterCount;
            int max = textCollection.MaxCharacterCount;
            countLabel.Text = $"{total}/{max}";
            countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void GPCupSelectTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.gpCupSelectTexts, this.gpCupSelectTextsCountLabel);
        }

        private void GPPodiumCupTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.gpPodiumCupTexts, this.gpPodiumCupTextsCountLabel);
        }

        private void CupAndThemeTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.cupAndThemeTexts, this.cupAndThemeTextsCountLabel);
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
