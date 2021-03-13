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
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the name of each driver.
    /// </summary>
    internal partial class DriverNamesControl : UserControl
    {
        private TextCollection _names;
        private bool _fireEvents;

        public DriverNamesControl()
        {
            InitializeComponent();

            textBox1.Tag = 0;
            textBox2.Tag = 1;
            textBox3.Tag = 2;
            textBox4.Tag = 3;
            textBox5.Tag = 4;
            textBox6.Tag = 5;
            textBox7.Tag = 6;
            textBox8.Tag = 7;
        }

        public void Init(TextCollection names)
        {
            _names = names;

            _fireEvents = false;

            textBox1.Text = names[0].Value;
            textBox2.Text = names[1].Value;
            textBox3.Text = names[2].Value;
            textBox4.Text = names[3].Value;
            textBox5.Text = names[4].Value;
            textBox6.Text = names[5].Value;
            textBox7.Text = names[6].Value;
            textBox8.Text = names[7].Value;

            _fireEvents = true;

            UpdateCount();
        }

        private void UpdateCount()
        {
            int total = _names.TotalCharacterCount;
            int max = _names.MaxCharacterCount;
            countLabel.Text = $"{total}/{max}";
            countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        [Category("Appearance")]
        public string Title
        {
            get => groupBox.Text;
            set => groupBox.Text = value;
        }

        private void TextBoxTextChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            _fireEvents = false;

            TextBox textBox = sender as TextBox;
            int id = (int)textBox.Tag;
            int sel = textBox.SelectionStart;
            _names[id].Value = textBox.Text;
            textBox.Text = _names[id].Value; // Retrieve validated text
            textBox.SelectionStart = sel; // Restore text input position

            _fireEvents = true;

            UpdateCount();
        }
    }
}
