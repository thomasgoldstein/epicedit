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
            this.cupAndThemeTextsPanel = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cupAndThemeTextsCountLabel = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.gpCupTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpCupTextsCountLabel = new System.Windows.Forms.Label();
            this.gpTextBox1 = new System.Windows.Forms.TextBox();
            this.gpTextBox2 = new System.Windows.Forms.TextBox();
            this.gpTextBox3 = new System.Windows.Forms.TextBox();
            this.gpTextBox4 = new System.Windows.Forms.TextBox();
            this.gpPodiumCupTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpPodiumCupTextsCountLabel = new System.Windows.Forms.Label();
            this.pcTextBox5 = new System.Windows.Forms.TextBox();
            this.pcTextBox1 = new System.Windows.Forms.TextBox();
            this.pcTextBox2 = new System.Windows.Forms.TextBox();
            this.pcTextBox3 = new System.Windows.Forms.TextBox();
            this.pcTextBox4 = new System.Windows.Forms.TextBox();
            cupAndThemeTextsGroupBox = new System.Windows.Forms.GroupBox();
            cupAndThemeTextsGroupBox.SuspendLayout();
            this.cupAndThemeTextsPanel.SuspendLayout();
            this.gpCupTextsGroupBox.SuspendLayout();
            this.gpPodiumCupTextsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cupAndThemeTextsGroupBox
            // 
            cupAndThemeTextsGroupBox.Controls.Add(this.cupAndThemeTextsPanel);
            cupAndThemeTextsGroupBox.Location = new System.Drawing.Point(322, 6);
            cupAndThemeTextsGroupBox.Name = "cupAndThemeTextsGroupBox";
            cupAndThemeTextsGroupBox.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
            cupAndThemeTextsGroupBox.Size = new System.Drawing.Size(189, 400);
            cupAndThemeTextsGroupBox.TabIndex = 2;
            cupAndThemeTextsGroupBox.TabStop = false;
            cupAndThemeTextsGroupBox.Text = "Cup and Theme texts";
            // 
            // cupAndThemeTextsPanel
            // 
            this.cupAndThemeTextsPanel.AutoScroll = true;
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox1);
            this.cupAndThemeTextsPanel.Controls.Add(this.cupAndThemeTextsCountLabel);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox10);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox2);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox11);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox3);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox9);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox4);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox12);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox5);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox8);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox6);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox13);
            this.cupAndThemeTextsPanel.Controls.Add(this.textBox7);
            this.cupAndThemeTextsPanel.Location = new System.Drawing.Point(5, 16);
            this.cupAndThemeTextsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.cupAndThemeTextsPanel.Name = "cupAndThemeTextsPanel";
            this.cupAndThemeTextsPanel.Size = new System.Drawing.Size(181, 210);
            this.cupAndThemeTextsPanel.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(136, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // cupAndThemeTextsCountLabel
            // 
            this.cupAndThemeTextsCountLabel.Location = new System.Drawing.Point(39, 338);
            this.cupAndThemeTextsCountLabel.Name = "cupAndThemeTextsCountLabel";
            this.cupAndThemeTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.cupAndThemeTextsCountLabel.TabIndex = 13;
            this.cupAndThemeTextsCountLabel.Text = "...";
            this.cupAndThemeTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(3, 237);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(136, 20);
            this.textBox10.TabIndex = 9;
            this.textBox10.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(3, 29);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(136, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(3, 263);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(136, 20);
            this.textBox11.TabIndex = 10;
            this.textBox11.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(3, 55);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(136, 20);
            this.textBox3.TabIndex = 2;
            this.textBox3.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(3, 211);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(136, 20);
            this.textBox9.TabIndex = 8;
            this.textBox9.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(3, 81);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(136, 20);
            this.textBox4.TabIndex = 3;
            this.textBox4.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(3, 289);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(136, 20);
            this.textBox12.TabIndex = 11;
            this.textBox12.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(3, 107);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(136, 20);
            this.textBox5.TabIndex = 4;
            this.textBox5.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(3, 185);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(136, 20);
            this.textBox8.TabIndex = 7;
            this.textBox8.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(3, 133);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(136, 20);
            this.textBox6.TabIndex = 5;
            this.textBox6.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(3, 315);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(136, 20);
            this.textBox13.TabIndex = 12;
            this.textBox13.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(3, 159);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(136, 20);
            this.textBox7.TabIndex = 6;
            this.textBox7.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
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
            this.gpCupTextsGroupBox.Text = "GP Cup Select texts";
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
            // gpPodiumCupTextsGroupBox
            // 
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.gpPodiumCupTextsCountLabel);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.pcTextBox5);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.pcTextBox1);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.pcTextBox2);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.pcTextBox3);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.pcTextBox4);
            this.gpPodiumCupTextsGroupBox.Location = new System.Drawing.Point(164, 6);
            this.gpPodiumCupTextsGroupBox.Name = "gpPodiumCupTextsGroupBox";
            this.gpPodiumCupTextsGroupBox.Size = new System.Drawing.Size(152, 244);
            this.gpPodiumCupTextsGroupBox.TabIndex = 1;
            this.gpPodiumCupTextsGroupBox.TabStop = false;
            this.gpPodiumCupTextsGroupBox.Text = "GP Podium Cup texts";
            // 
            // gpPodiumCupTextsCountLabel
            // 
            this.gpPodiumCupTextsCountLabel.Location = new System.Drawing.Point(44, 149);
            this.gpPodiumCupTextsCountLabel.Name = "gpPodiumCupTextsCountLabel";
            this.gpPodiumCupTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.gpPodiumCupTextsCountLabel.TabIndex = 5;
            this.gpPodiumCupTextsCountLabel.Text = "...";
            this.gpPodiumCupTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pcTextBox5
            // 
            this.pcTextBox5.Location = new System.Drawing.Point(8, 123);
            this.pcTextBox5.Name = "pcTextBox5";
            this.pcTextBox5.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox5.TabIndex = 4;
            this.pcTextBox5.TextChanged += new System.EventHandler(this.GPPodiumCupTextBoxTextChanged);
            // 
            // pcTextBox1
            // 
            this.pcTextBox1.Location = new System.Drawing.Point(8, 19);
            this.pcTextBox1.Name = "pcTextBox1";
            this.pcTextBox1.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox1.TabIndex = 0;
            this.pcTextBox1.TextChanged += new System.EventHandler(this.GPPodiumCupTextBoxTextChanged);
            // 
            // pcTextBox2
            // 
            this.pcTextBox2.Location = new System.Drawing.Point(8, 45);
            this.pcTextBox2.Name = "pcTextBox2";
            this.pcTextBox2.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox2.TabIndex = 1;
            this.pcTextBox2.TextChanged += new System.EventHandler(this.GPPodiumCupTextBoxTextChanged);
            // 
            // pcTextBox3
            // 
            this.pcTextBox3.Location = new System.Drawing.Point(8, 71);
            this.pcTextBox3.Name = "pcTextBox3";
            this.pcTextBox3.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox3.TabIndex = 2;
            this.pcTextBox3.TextChanged += new System.EventHandler(this.GPPodiumCupTextBoxTextChanged);
            // 
            // pcTextBox4
            // 
            this.pcTextBox4.Location = new System.Drawing.Point(8, 97);
            this.pcTextBox4.Name = "pcTextBox4";
            this.pcTextBox4.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox4.TabIndex = 3;
            this.pcTextBox4.TextChanged += new System.EventHandler(this.GPPodiumCupTextBoxTextChanged);
            // 
            // CupAndThemeTextsEditor
            // 
            this.Controls.Add(this.gpPodiumCupTextsGroupBox);
            this.Controls.Add(this.gpCupTextsGroupBox);
            this.Controls.Add(cupAndThemeTextsGroupBox);
            this.Name = "CupAndThemeTextsEditor";
            this.Size = new System.Drawing.Size(514, 250);
            cupAndThemeTextsGroupBox.ResumeLayout(false);
            this.cupAndThemeTextsPanel.ResumeLayout(false);
            this.cupAndThemeTextsPanel.PerformLayout();
            this.gpCupTextsGroupBox.ResumeLayout(false);
            this.gpCupTextsGroupBox.PerformLayout();
            this.gpPodiumCupTextsGroupBox.ResumeLayout(false);
            this.gpPodiumCupTextsGroupBox.PerformLayout();
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
        private System.Windows.Forms.GroupBox gpPodiumCupTextsGroupBox;
        private System.Windows.Forms.TextBox pcTextBox1;
        private System.Windows.Forms.TextBox pcTextBox2;
        private System.Windows.Forms.TextBox pcTextBox3;
        private System.Windows.Forms.TextBox pcTextBox4;
        private System.Windows.Forms.Label gpPodiumCupTextsCountLabel;
        private System.Windows.Forms.TextBox pcTextBox5;
        private System.Windows.Forms.Panel cupAndThemeTextsPanel;
    }
}
