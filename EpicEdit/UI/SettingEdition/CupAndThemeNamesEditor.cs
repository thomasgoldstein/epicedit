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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using EpicEdit.Rom.Settings;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the name of each cup and theme.
    /// </summary>
    internal partial class CupAndThemeNamesEditor : UserControl
    {
        private TextCollection gpCupNames;
        private TextCollection cupAndThemeNames;
        private bool fireEvents;

        public CupAndThemeNamesEditor()
        {
            this.InitializeComponent();

            this.gpTextBox1.Tag = 0;
            this.gpTextBox2.Tag = 1;
            this.gpTextBox3.Tag = 2;
            this.gpTextBox4.Tag = 3;

            this.textBox1.Tag = 0;
            this.textBox2.Tag = 1;
            this.textBox3.Tag = 2;
            this.textBox4.Tag = 3;
            this.textBox5.Tag = 4;
            this.textBox6.Tag = 5;
            this.textBox7.Tag = 6;
            this.textBox8.Tag = 7;
            this.textBox9.Tag = 8;
            this.textBox10.Tag = 9;
            this.textBox11.Tag = 10;
            this.textBox12.Tag = 11;
            this.textBox13.Tag = 12;
        }

        public void Init()
        {
            this.gpCupNames = Context.Game.Settings.GPCupNames;
            this.cupAndThemeNames = Context.Game.Settings.CupAndThemeNames;

            this.fireEvents = false;

            if (this.gpCupNames == null)
            {
                // NOTE: Japanese ROM, text editing not supported for GP cup names
                this.gpCupNamesGroupBox.Enabled = false;

                this.gpTextBox1.Text = string.Empty;
                this.gpTextBox2.Text = string.Empty;
                this.gpTextBox3.Text = string.Empty;
                this.gpTextBox4.Text = string.Empty;

                this.gpCupNamesCountLabel.Text = string.Empty;
            }
            else
            {
                this.gpCupNamesGroupBox.Enabled = true;

                this.gpTextBox1.Text = this.gpCupNames[0].Value;
                this.gpTextBox2.Text = this.gpCupNames[1].Value;
                this.gpTextBox3.Text = this.gpCupNames[2].Value;
                this.gpTextBox4.Text = this.gpCupNames[3].Value;

                UpdateCount(this.gpCupNames, this.gpCupNamesCountLabel);
            }

            this.textBox1.Text = this.cupAndThemeNames[0].Value;
            this.textBox2.Text = this.cupAndThemeNames[1].Value;
            this.textBox3.Text = this.cupAndThemeNames[2].Value;
            this.textBox4.Text = this.cupAndThemeNames[3].Value;
            this.textBox5.Text = this.cupAndThemeNames[4].Value;
            this.textBox6.Text = this.cupAndThemeNames[5].Value;
            this.textBox7.Text = this.cupAndThemeNames[6].Value;
            this.textBox8.Text = this.cupAndThemeNames[7].Value;
            this.textBox9.Text = this.cupAndThemeNames[8].Value;
            this.textBox10.Text = this.cupAndThemeNames[9].Value;
            this.textBox11.Text = this.cupAndThemeNames[10].Value;
            this.textBox12.Text = this.cupAndThemeNames[11].Value;
            this.textBox13.Text = this.cupAndThemeNames[12].Value;

            UpdateCount(this.cupAndThemeNames, this.cupAndThemeNamesCountLabel);

            this.fireEvents = true;
        }

        private static void UpdateCount(TextCollection textCollection, Label countLabel)
        {
            int total = textCollection.TotalCharacterCount;
            int max = textCollection.MaxCharacterCount;
            countLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}", total, max);
            countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void GPCupNamesTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.gpCupNames, this.gpCupNamesCountLabel);
        }

        private void CupAndThemeNamesTextBoxTextChanged(object sender, EventArgs e)
        {
            this.OnTextBoxTextChanged(sender, this.cupAndThemeNames, this.cupAndThemeNamesCountLabel);
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

            textCollection.SetValue(id, textBox.Text);

            textBox.Text = textCollection[id].Value; // Retrieve validated text
            textBox.SelectionStart = sel; // Restore text input position

            this.fireEvents = true;

            UpdateCount(textCollection, countLabel);
        }
    }
}
