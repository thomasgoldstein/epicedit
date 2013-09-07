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
using System.Windows.Forms;

using EpicEdit.Rom.Settings;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the name of each cup and theme.
    /// </summary>
    internal partial class CupAndThemeNamesEditor : UserControl
    {
        private TextCollection names;
        private bool fireEvents;

        public CupAndThemeNamesEditor()
        {
            this.InitializeComponent();

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
            this.names = Context.Game.Settings.CupAndThemeNames;

            this.fireEvents = false;

            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    control.Text = this.names[(int)control.Tag];
                }
            }

            this.fireEvents = true;

            this.UpdateCount();
        }

        private void UpdateCount()
        {
            int total = this.names.TotalCharacterCount;
            int max = this.names.MaxCharacterCount;
            this.countLabel.Text = string.Format("{0}/{1}", total, max);
            this.countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (!this.fireEvents)
            {
                return;
            }

            this.fireEvents = false;

            TextBox textBox = sender as TextBox;
            int id = (int)textBox.Tag;
            int sel = textBox.SelectionStart;
            this.names[id] = textBox.Text;
            textBox.Text = this.names[id];
            textBox.SelectionStart = sel;

            this.fireEvents = true;

            this.UpdateCount();
        }
    }
}
