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
            if (disposing)
            {
                if (components != null)
                {
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
            this.ctTextBox1 = new System.Windows.Forms.TextBox();
            this.cupAndThemeTextsCountLabel = new System.Windows.Forms.Label();
            this.ctTextBox10 = new System.Windows.Forms.TextBox();
            this.ctTextBox2 = new System.Windows.Forms.TextBox();
            this.ctTextBox11 = new System.Windows.Forms.TextBox();
            this.ctTextBox3 = new System.Windows.Forms.TextBox();
            this.ctTextBox9 = new System.Windows.Forms.TextBox();
            this.ctTextBox4 = new System.Windows.Forms.TextBox();
            this.ctTextBox12 = new System.Windows.Forms.TextBox();
            this.ctTextBox5 = new System.Windows.Forms.TextBox();
            this.ctTextBox8 = new System.Windows.Forms.TextBox();
            this.ctTextBox6 = new System.Windows.Forms.TextBox();
            this.ctTextBox13 = new System.Windows.Forms.TextBox();
            this.ctTextBox7 = new System.Windows.Forms.TextBox();
            this.gpCupSelectTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpCupSelectTextsCountLabel = new System.Windows.Forms.Label();
            this.csTextBox1 = new System.Windows.Forms.TextBox();
            this.csTextBox2 = new System.Windows.Forms.TextBox();
            this.csTextBox3 = new System.Windows.Forms.TextBox();
            this.csTextBox4 = new System.Windows.Forms.TextBox();
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
            this.gpCupSelectTextsGroupBox.SuspendLayout();
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
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox1);
            this.cupAndThemeTextsPanel.Controls.Add(this.cupAndThemeTextsCountLabel);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox10);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox2);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox11);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox3);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox9);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox4);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox12);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox5);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox8);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox6);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox13);
            this.cupAndThemeTextsPanel.Controls.Add(this.ctTextBox7);
            this.cupAndThemeTextsPanel.Location = new System.Drawing.Point(5, 16);
            this.cupAndThemeTextsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.cupAndThemeTextsPanel.Name = "cupAndThemeTextsPanel";
            this.cupAndThemeTextsPanel.Size = new System.Drawing.Size(181, 210);
            this.cupAndThemeTextsPanel.TabIndex = 0;
            // 
            // ctTextBox1
            // 
            this.ctTextBox1.Location = new System.Drawing.Point(3, 3);
            this.ctTextBox1.Name = "ctTextBox1";
            this.ctTextBox1.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox1.TabIndex = 0;
            this.ctTextBox1.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
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
            // ctTextBox10
            // 
            this.ctTextBox10.Location = new System.Drawing.Point(3, 237);
            this.ctTextBox10.Name = "ctTextBox10";
            this.ctTextBox10.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox10.TabIndex = 9;
            this.ctTextBox10.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox2
            // 
            this.ctTextBox2.Location = new System.Drawing.Point(3, 29);
            this.ctTextBox2.Name = "ctTextBox2";
            this.ctTextBox2.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox2.TabIndex = 1;
            this.ctTextBox2.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox11
            // 
            this.ctTextBox11.Location = new System.Drawing.Point(3, 263);
            this.ctTextBox11.Name = "ctTextBox11";
            this.ctTextBox11.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox11.TabIndex = 10;
            this.ctTextBox11.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox3
            // 
            this.ctTextBox3.Location = new System.Drawing.Point(3, 55);
            this.ctTextBox3.Name = "ctTextBox3";
            this.ctTextBox3.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox3.TabIndex = 2;
            this.ctTextBox3.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox9
            // 
            this.ctTextBox9.Location = new System.Drawing.Point(3, 211);
            this.ctTextBox9.Name = "ctTextBox9";
            this.ctTextBox9.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox9.TabIndex = 8;
            this.ctTextBox9.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox4
            // 
            this.ctTextBox4.Location = new System.Drawing.Point(3, 81);
            this.ctTextBox4.Name = "ctTextBox4";
            this.ctTextBox4.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox4.TabIndex = 3;
            this.ctTextBox4.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox12
            // 
            this.ctTextBox12.Location = new System.Drawing.Point(3, 289);
            this.ctTextBox12.Name = "ctTextBox12";
            this.ctTextBox12.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox12.TabIndex = 11;
            this.ctTextBox12.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox5
            // 
            this.ctTextBox5.Location = new System.Drawing.Point(3, 107);
            this.ctTextBox5.Name = "ctTextBox5";
            this.ctTextBox5.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox5.TabIndex = 4;
            this.ctTextBox5.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox8
            // 
            this.ctTextBox8.Location = new System.Drawing.Point(3, 185);
            this.ctTextBox8.Name = "ctTextBox8";
            this.ctTextBox8.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox8.TabIndex = 7;
            this.ctTextBox8.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox6
            // 
            this.ctTextBox6.Location = new System.Drawing.Point(3, 133);
            this.ctTextBox6.Name = "ctTextBox6";
            this.ctTextBox6.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox6.TabIndex = 5;
            this.ctTextBox6.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox13
            // 
            this.ctTextBox13.Location = new System.Drawing.Point(3, 315);
            this.ctTextBox13.Name = "ctTextBox13";
            this.ctTextBox13.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox13.TabIndex = 12;
            this.ctTextBox13.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // ctTextBox7
            // 
            this.ctTextBox7.Location = new System.Drawing.Point(3, 159);
            this.ctTextBox7.Name = "ctTextBox7";
            this.ctTextBox7.Size = new System.Drawing.Size(136, 20);
            this.ctTextBox7.TabIndex = 6;
            this.ctTextBox7.TextChanged += new System.EventHandler(this.CupAndThemeTextsTextBoxTextChanged);
            // 
            // gpCupSelectTextsGroupBox
            // 
            this.gpCupSelectTextsGroupBox.Controls.Add(this.gpCupSelectTextsCountLabel);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.csTextBox1);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.csTextBox2);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.csTextBox3);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.csTextBox4);
            this.gpCupSelectTextsGroupBox.Location = new System.Drawing.Point(6, 6);
            this.gpCupSelectTextsGroupBox.Name = "gpCupSelectTextsGroupBox";
            this.gpCupSelectTextsGroupBox.Size = new System.Drawing.Size(152, 244);
            this.gpCupSelectTextsGroupBox.TabIndex = 0;
            this.gpCupSelectTextsGroupBox.TabStop = false;
            this.gpCupSelectTextsGroupBox.Text = "GP Cup Select texts";
            // 
            // gpCupSelectTextsCountLabel
            // 
            this.gpCupSelectTextsCountLabel.Location = new System.Drawing.Point(44, 123);
            this.gpCupSelectTextsCountLabel.Name = "gpCupSelectTextsCountLabel";
            this.gpCupSelectTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.gpCupSelectTextsCountLabel.TabIndex = 4;
            this.gpCupSelectTextsCountLabel.Text = "...";
            this.gpCupSelectTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // csTextBox1
            // 
            this.csTextBox1.Location = new System.Drawing.Point(8, 19);
            this.csTextBox1.Name = "csTextBox1";
            this.csTextBox1.Size = new System.Drawing.Size(136, 20);
            this.csTextBox1.TabIndex = 0;
            this.csTextBox1.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // csTextBox2
            // 
            this.csTextBox2.Location = new System.Drawing.Point(8, 45);
            this.csTextBox2.Name = "csTextBox2";
            this.csTextBox2.Size = new System.Drawing.Size(136, 20);
            this.csTextBox2.TabIndex = 1;
            this.csTextBox2.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // csTextBox3
            // 
            this.csTextBox3.Location = new System.Drawing.Point(8, 71);
            this.csTextBox3.Name = "csTextBox3";
            this.csTextBox3.Size = new System.Drawing.Size(136, 20);
            this.csTextBox3.TabIndex = 2;
            this.csTextBox3.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // csTextBox4
            // 
            this.csTextBox4.Location = new System.Drawing.Point(8, 97);
            this.csTextBox4.Name = "csTextBox4";
            this.csTextBox4.Size = new System.Drawing.Size(136, 20);
            this.csTextBox4.TabIndex = 3;
            this.csTextBox4.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
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
            this.pcTextBox5.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // pcTextBox1
            // 
            this.pcTextBox1.Location = new System.Drawing.Point(8, 19);
            this.pcTextBox1.Name = "pcTextBox1";
            this.pcTextBox1.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox1.TabIndex = 0;
            this.pcTextBox1.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // pcTextBox2
            // 
            this.pcTextBox2.Location = new System.Drawing.Point(8, 45);
            this.pcTextBox2.Name = "pcTextBox2";
            this.pcTextBox2.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox2.TabIndex = 1;
            this.pcTextBox2.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // pcTextBox3
            // 
            this.pcTextBox3.Location = new System.Drawing.Point(8, 71);
            this.pcTextBox3.Name = "pcTextBox3";
            this.pcTextBox3.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox3.TabIndex = 2;
            this.pcTextBox3.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // pcTextBox4
            // 
            this.pcTextBox4.Location = new System.Drawing.Point(8, 97);
            this.pcTextBox4.Name = "pcTextBox4";
            this.pcTextBox4.Size = new System.Drawing.Size(136, 20);
            this.pcTextBox4.TabIndex = 3;
            this.pcTextBox4.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // CupAndThemeTextsEditor
            // 
            this.Controls.Add(this.gpPodiumCupTextsGroupBox);
            this.Controls.Add(this.gpCupSelectTextsGroupBox);
            this.Controls.Add(cupAndThemeTextsGroupBox);
            this.Name = "CupAndThemeTextsEditor";
            this.Size = new System.Drawing.Size(514, 250);
            cupAndThemeTextsGroupBox.ResumeLayout(false);
            this.cupAndThemeTextsPanel.ResumeLayout(false);
            this.cupAndThemeTextsPanel.PerformLayout();
            this.gpCupSelectTextsGroupBox.ResumeLayout(false);
            this.gpCupSelectTextsGroupBox.PerformLayout();
            this.gpPodiumCupTextsGroupBox.ResumeLayout(false);
            this.gpPodiumCupTextsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label cupAndThemeTextsCountLabel;
        private System.Windows.Forms.TextBox ctTextBox10;
        private System.Windows.Forms.TextBox ctTextBox11;
        private System.Windows.Forms.TextBox ctTextBox12;
        private System.Windows.Forms.TextBox ctTextBox13;
        private System.Windows.Forms.TextBox ctTextBox9;
        private System.Windows.Forms.TextBox ctTextBox5;
        private System.Windows.Forms.TextBox ctTextBox6;
        private System.Windows.Forms.TextBox ctTextBox7;
        private System.Windows.Forms.TextBox ctTextBox8;
        private System.Windows.Forms.TextBox ctTextBox4;
        private System.Windows.Forms.TextBox ctTextBox3;
        private System.Windows.Forms.TextBox ctTextBox2;
        private System.Windows.Forms.TextBox ctTextBox1;
        private System.Windows.Forms.TextBox csTextBox1;
        private System.Windows.Forms.TextBox csTextBox2;
        private System.Windows.Forms.TextBox csTextBox3;
        private System.Windows.Forms.TextBox csTextBox4;
        private System.Windows.Forms.GroupBox gpCupSelectTextsGroupBox;
        private System.Windows.Forms.Label gpCupSelectTextsCountLabel;
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
