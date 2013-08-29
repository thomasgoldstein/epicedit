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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

using EpicEdit.Rom.Settings;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the name of each driver.
    /// </summary>
    internal partial class DriverNamesControl : UserControl
    {
        private bool fireEvents;

        public DriverNamesControl()
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
        }

        private TextCollection names;

        public void Init(TextCollection names)
        {
            this.names = names;

            this.fireEvents = false;

            this.textBox1.Text = names[0];
            this.textBox2.Text = names[1];
            this.textBox3.Text = names[2];
            this.textBox4.Text = names[3];
            this.textBox5.Text = names[4];
            this.textBox6.Text = names[5];
            this.textBox7.Text = names[6];
            this.textBox8.Text = names[7];

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

        [Category("Appearance")]
        public string Title
        {
            get { return this.groupBox.Text; }
            set { this.groupBox.Text = value; }
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
