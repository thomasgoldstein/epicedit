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
    internal partial class GPCupTextsEditor : UserControl
    {
        private TextCollection gpCupSelectTexts;
        private TextCollection gpResultsCupTexts;
        private TextCollection gpPodiumCupTexts;
        private bool fireEvents;

        public GPCupTextsEditor()
        {
            this.InitializeComponent();

            this.cupTextBox1.Tag = 0;
            this.cupTextBox2.Tag = 1;
            this.cupTextBox3.Tag = 2;
            this.cupTextBox4.Tag = 3;

            this.resultsTextBox1.Tag = 0;
            this.resultsTextBox2.Tag = 1;
            this.resultsTextBox3.Tag = 2;
            this.resultsTextBox4.Tag = 3;

            this.podiumTextBox1.Tag = 0;
            this.podiumTextBox2.Tag = 1;
            this.podiumTextBox3.Tag = 2;
            this.podiumTextBox4.Tag = 3;
            this.podiumTextBox5.Tag = 4;
        }

        public void Init()
        {
            this.gpCupSelectTexts = Context.Game.Settings.GPCupSelectTexts;
            this.gpResultsCupTexts = Context.Game.Settings.GPResultsCupTexts;
            this.gpPodiumCupTexts = Context.Game.Settings.GPPodiumCupTexts;

            this.fireEvents = false;

            if (this.gpCupSelectTexts == null)
            {
                // NOTE: Japanese ROM, text editing not supported for GP cup names
                this.gpCupSelectTextsGroupBox.Enabled = false;

                this.cupTextBox1.Text = string.Empty;
                this.cupTextBox2.Text = string.Empty;
                this.cupTextBox3.Text = string.Empty;
                this.cupTextBox4.Text = string.Empty;

                this.gpCupSelectTextsCountLabel.Text = string.Empty;
            }
            else
            {
                this.gpCupSelectTextsGroupBox.Enabled = true;

                this.cupTextBox1.Text = this.gpCupSelectTexts[0].Value;
                this.cupTextBox2.Text = this.gpCupSelectTexts[1].Value;
                this.cupTextBox3.Text = this.gpCupSelectTexts[2].Value;
                this.cupTextBox4.Text = this.gpCupSelectTexts[3].Value;

                UpdateCount(this.gpCupSelectTexts, this.gpCupSelectTextsCountLabel);
            }

            this.resultsTextBox1.Text = this.gpResultsCupTexts[0].Value;
            this.resultsTextBox2.Text = this.gpResultsCupTexts[1].Value;
            this.resultsTextBox3.Text = this.gpResultsCupTexts[2].Value;
            this.resultsTextBox4.Text = this.gpResultsCupTexts[3].Value;

            UpdateCount(this.gpResultsCupTexts, this.gpResultsCupTextsCountLabel);

            this.podiumTextBox1.Text = this.gpPodiumCupTexts[0].Value;
            this.podiumTextBox2.Text = this.gpPodiumCupTexts[1].Value;
            this.podiumTextBox3.Text = this.gpPodiumCupTexts[2].Value;
            this.podiumTextBox4.Text = this.gpPodiumCupTexts[3].Value;
            this.podiumTextBox5.Text = this.gpPodiumCupTexts[4].Value;

            UpdateCount(this.gpPodiumCupTexts, this.gpPodiumCupTextsCountLabel);

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

        private void GPResultsCupTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.gpResultsCupTexts, this.gpResultsCupTextsCountLabel);
        }

        private void GPPodiumCupTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.gpPodiumCupTexts, this.gpPodiumCupTextsCountLabel);
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
