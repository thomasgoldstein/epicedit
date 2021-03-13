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
        private TextCollection _gpCupSelectTexts;
        private TextCollection _gpResultsCupTexts;
        private TextCollection _gpPodiumCupTexts;
        private bool _fireEvents;

        public GPCupTextsEditor()
        {
            InitializeComponent();

            cupTextBox1.Tag = 0;
            cupTextBox2.Tag = 1;
            cupTextBox3.Tag = 2;
            cupTextBox4.Tag = 3;

            resultsTextBox1.Tag = 0;
            resultsTextBox2.Tag = 1;
            resultsTextBox3.Tag = 2;
            resultsTextBox4.Tag = 3;

            podiumTextBox1.Tag = 0;
            podiumTextBox2.Tag = 1;
            podiumTextBox3.Tag = 2;
            podiumTextBox4.Tag = 3;
            podiumTextBox5.Tag = 4;
        }

        public void Init()
        {
            _gpCupSelectTexts = Context.Game.Settings.GPCupSelectTexts;
            _gpResultsCupTexts = Context.Game.Settings.GPResultsCupTexts;
            _gpPodiumCupTexts = Context.Game.Settings.GPPodiumCupTexts;

            _fireEvents = false;

            if (_gpCupSelectTexts == null)
            {
                // NOTE: Japanese ROM, text editing not supported for GP cup names
                gpCupSelectTextsGroupBox.Enabled = false;

                cupTextBox1.Text = string.Empty;
                cupTextBox2.Text = string.Empty;
                cupTextBox3.Text = string.Empty;
                cupTextBox4.Text = string.Empty;

                gpCupSelectTextsCountLabel.Text = string.Empty;
            }
            else
            {
                gpCupSelectTextsGroupBox.Enabled = true;

                cupTextBox1.Text = _gpCupSelectTexts[0].Value;
                cupTextBox2.Text = _gpCupSelectTexts[1].Value;
                cupTextBox3.Text = _gpCupSelectTexts[2].Value;
                cupTextBox4.Text = _gpCupSelectTexts[3].Value;

                UpdateCount(_gpCupSelectTexts, gpCupSelectTextsCountLabel);
            }

            resultsTextBox1.Text = _gpResultsCupTexts[0].Value;
            resultsTextBox2.Text = _gpResultsCupTexts[1].Value;
            resultsTextBox3.Text = _gpResultsCupTexts[2].Value;
            resultsTextBox4.Text = _gpResultsCupTexts[3].Value;

            UpdateCount(_gpResultsCupTexts, gpResultsCupTextsCountLabel);

            podiumTextBox1.Text = _gpPodiumCupTexts[0].Value;
            podiumTextBox2.Text = _gpPodiumCupTexts[1].Value;
            podiumTextBox3.Text = _gpPodiumCupTexts[2].Value;
            podiumTextBox4.Text = _gpPodiumCupTexts[3].Value;
            podiumTextBox5.Text = _gpPodiumCupTexts[4].Value;

            UpdateCount(_gpPodiumCupTexts, gpPodiumCupTextsCountLabel);

            _fireEvents = true;
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
            OnTextBoxTextChanged(sender, _gpCupSelectTexts, gpCupSelectTextsCountLabel);
        }

        private void GPResultsCupTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            OnTextBoxTextChanged(sender, _gpResultsCupTexts, gpResultsCupTextsCountLabel);
        }

        private void GPPodiumCupTextsTextBoxTextChanged(object sender, EventArgs e)
        {
            OnTextBoxTextChanged(sender, _gpPodiumCupTexts, gpPodiumCupTextsCountLabel);
        }

        private void OnTextBoxTextChanged(object sender, TextCollection textCollection, Label countLabel)
        {
            if (!_fireEvents)
            {
                return;
            }

            _fireEvents = false;

            TextBox textBox = (TextBox)sender;
            int id = (int)textBox.Tag;
            int sel = textBox.SelectionStart;

            textCollection[id].Value = textBox.Text;

            textBox.Text = textCollection[id].Value; // Retrieve validated text
            textBox.SelectionStart = sel; // Restore text input position

            _fireEvents = true;

            UpdateCount(textCollection, countLabel);
        }
    }
}
