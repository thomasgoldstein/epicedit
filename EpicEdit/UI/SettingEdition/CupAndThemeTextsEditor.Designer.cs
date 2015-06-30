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

namespace EpicEdit.UI.SettingEdition
{
    partial class CupAndThemeTextsEditor
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.GroupBox cupAndThemeTextsGroupBox;
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.cupAndThemeTextsCountLabel = new System.Windows.Forms.Label();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.gpCupTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpCupTextsCountLabel = new System.Windows.Forms.Label();
            this.gpTextBox1 = new System.Windows.Forms.TextBox();
            this.gpTextBox2 = new System.Windows.Forms.TextBox();
            this.gpTextBox3 = new System.Windows.Forms.TextBox();
            this.gpTextBox4 = new System.Windows.Forms.TextBox();
            cupAndThemeTextsGroupBox = new System.Windows.Forms.GroupBox();
            cupAndThemeTextsGroupBox.SuspendLayout();
            this.gpCupTextsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cupAndThemeTextsGroupBox
            // 
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox1);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox2);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox3);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox4);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox5);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox6);
            cupAndThemeTextsGroupBox.Controls.Add(this.cupAndThemeTextsCountLabel);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox7);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox13);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox8);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox12);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox9);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox11);
            cupAndThemeTextsGroupBox.Controls.Add(this.textBox10);
            cupAndThemeTextsGroupBox.Location = new System.Drawing.Point(161, 6);
            cupAndThemeTextsGroupBox.Name = "cupAndThemeTextsGroupBox";
            cupAndThemeTextsGroupBox.Size = new System.Drawing.Size(353, 244);
            cupAndThemeTextsGroupBox.TabIndex = 1;
            cupAndThemeTextsGroupBox.TabStop = false;
            cupAndThemeTextsGroupBox.Text = "Cup and Theme texts";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(108, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(9, 45);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(108, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(9, 71);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(108, 20);
            this.textBox3.TabIndex = 2;
            this.textBox3.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(9, 97);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(108, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(122, 19);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(108, 20);
            this.textBox5.TabIndex = 4;
            this.textBox5.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(122, 45);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(108, 20);
            this.textBox6.TabIndex = 5;
            this.textBox6.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // cupAndThemeTextsCountLabel
            // 
            this.cupAndThemeTextsCountLabel.Location = new System.Drawing.Point(244, 123);
            this.cupAndThemeTextsCountLabel.Name = "cupAndThemeTextsCountLabel";
            this.cupAndThemeTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.cupAndThemeTextsCountLabel.TabIndex = 13;
            this.cupAndThemeTextsCountLabel.Text = "...";
            this.cupAndThemeTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(122, 71);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(108, 20);
            this.textBox7.TabIndex = 6;
            this.textBox7.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(236, 97);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(108, 20);
            this.textBox13.TabIndex = 12;
            this.textBox13.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(122, 97);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(108, 20);
            this.textBox8.TabIndex = 7;
            this.textBox8.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(236, 71);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(108, 20);
            this.textBox12.TabIndex = 11;
            this.textBox12.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(122, 123);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(108, 20);
            this.textBox9.TabIndex = 8;
            this.textBox9.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(236, 45);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(108, 20);
            this.textBox11.TabIndex = 10;
            this.textBox11.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(236, 19);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(108, 20);
            this.textBox10.TabIndex = 9;
            this.textBox10.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // gpCupTextsGroupBox
            // 
            this.gpCupTextsGroupBox.Controls.Add(this.gpCupTextsCountLabel);
            this.gpCupTextsGroupBox.Controls.Add(this.gpTextBox1);
            this.gpCupTextsGroupBox.Controls.Add(this.gpTextBox2);
            this.gpCupTextsGroupBox.Controls.Add(this.gpTextBox3);
            this.gpCupTextsGroupBox.Controls.Add(this.gpTextBox4);
            this.gpCupTextsGroupBox.Location = new System.Drawing.Point(6, 6);
            this.gpCupTextsGroupBox.Name = "gpCupTextsGroupBox";
            this.gpCupTextsGroupBox.Size = new System.Drawing.Size(152, 244);
            this.gpCupTextsGroupBox.TabIndex = 0;
            this.gpCupTextsGroupBox.TabStop = false;
            this.gpCupTextsGroupBox.Text = "GP Cup texts";
            // 
            // gpCupTextsCountLabel
            // 
            this.gpCupTextsCountLabel.Location = new System.Drawing.Point(44, 123);
            this.gpCupTextsCountLabel.Name = "gpCupTextsCountLabel";
            this.gpCupTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.gpCupTextsCountLabel.TabIndex = 4;
            this.gpCupTextsCountLabel.Text = "...";
            this.gpCupTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gpTextBox1
            // 
            this.gpTextBox1.Location = new System.Drawing.Point(8, 19);
            this.gpTextBox1.Name = "gpTextBox1";
            this.gpTextBox1.Size = new System.Drawing.Size(136, 20);
            this.gpTextBox1.TabIndex = 0;
            this.gpTextBox1.TextChanged += new System.EventHandler(this.GPCupTextsTextBoxTextChanged);
            // 
            // gpTextBox2
            // 
            this.gpTextBox2.Location = new System.Drawing.Point(8, 45);
            this.gpTextBox2.Name = "gpTextBox2";
            this.gpTextBox2.Size = new System.Drawing.Size(136, 20);
            this.gpTextBox2.TabIndex = 1;
            this.gpTextBox2.TextChanged += new System.EventHandler(this.GPCupTextsTextBoxTextChanged);
            // 
            // gpTextBox3
            // 
            this.gpTextBox3.Location = new System.Drawing.Point(8, 71);
            this.gpTextBox3.Name = "gpTextBox3";
            this.gpTextBox3.Size = new System.Drawing.Size(136, 20);
            this.gpTextBox3.TabIndex = 2;
            this.gpTextBox3.TextChanged += new System.EventHandler(this.GPCupTextsTextBoxTextChanged);
            // 
            // gpTextBox4
            // 
            this.gpTextBox4.Location = new System.Drawing.Point(8, 97);
            this.gpTextBox4.Name = "gpTextBox4";
            this.gpTextBox4.Size = new System.Drawing.Size(136, 20);
            this.gpTextBox4.TabIndex = 3;
            this.gpTextBox4.TextChanged += new System.EventHandler(this.GPCupTextsTextBoxTextChanged);
            // 
            // CupAndThemeTextsEditor
            // 
            this.Controls.Add(this.gpCupTextsGroupBox);
            this.Controls.Add(cupAndThemeTextsGroupBox);
            this.Name = "CupAndThemeTextsEditor";
            this.Size = new System.Drawing.Size(514, 250);
            cupAndThemeTextsGroupBox.ResumeLayout(false);
            cupAndThemeTextsGroupBox.PerformLayout();
            this.gpCupTextsGroupBox.ResumeLayout(false);
            this.gpCupTextsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label cupAndThemeTextsCountLabel;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox gpTextBox1;
        private System.Windows.Forms.TextBox gpTextBox2;
        private System.Windows.Forms.TextBox gpTextBox3;
        private System.Windows.Forms.TextBox gpTextBox4;
        private System.Windows.Forms.GroupBox gpCupTextsGroupBox;
        private System.Windows.Forms.Label gpCupTextsCountLabel;
    }
}
