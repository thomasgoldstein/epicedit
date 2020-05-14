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
            System.Windows.Forms.GroupBox courseSelectTextsGroupBox;
            this.courseSelectTextsPanel = new System.Windows.Forms.Panel();
            this.courseTextBox1 = new System.Windows.Forms.TextBox();
            this.courseSelectTextsCountLabel = new System.Windows.Forms.Label();
            this.courseTextBox10 = new System.Windows.Forms.TextBox();
            this.courseTextBox2 = new System.Windows.Forms.TextBox();
            this.courseTextBox11 = new System.Windows.Forms.TextBox();
            this.courseTextBox3 = new System.Windows.Forms.TextBox();
            this.courseTextBox9 = new System.Windows.Forms.TextBox();
            this.courseTextBox4 = new System.Windows.Forms.TextBox();
            this.courseTextBox12 = new System.Windows.Forms.TextBox();
            this.courseTextBox5 = new System.Windows.Forms.TextBox();
            this.courseTextBox8 = new System.Windows.Forms.TextBox();
            this.courseTextBox6 = new System.Windows.Forms.TextBox();
            this.courseTextBox13 = new System.Windows.Forms.TextBox();
            this.courseTextBox7 = new System.Windows.Forms.TextBox();
            this.gpCupSelectTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpCupSelectTextsCountLabel = new System.Windows.Forms.Label();
            this.cupTextBox1 = new System.Windows.Forms.TextBox();
            this.cupTextBox2 = new System.Windows.Forms.TextBox();
            this.cupTextBox3 = new System.Windows.Forms.TextBox();
            this.cupTextBox4 = new System.Windows.Forms.TextBox();
            this.gpPodiumCupTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpPodiumCupTextsCountLabel = new System.Windows.Forms.Label();
            this.podiumTextBox5 = new System.Windows.Forms.TextBox();
            this.podiumTextBox1 = new System.Windows.Forms.TextBox();
            this.podiumTextBox2 = new System.Windows.Forms.TextBox();
            this.podiumTextBox3 = new System.Windows.Forms.TextBox();
            this.podiumTextBox4 = new System.Windows.Forms.TextBox();
            courseSelectTextsGroupBox = new System.Windows.Forms.GroupBox();
            courseSelectTextsGroupBox.SuspendLayout();
            this.courseSelectTextsPanel.SuspendLayout();
            this.gpCupSelectTextsGroupBox.SuspendLayout();
            this.gpPodiumCupTextsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // courseSelectTextsGroupBox
            // 
            courseSelectTextsGroupBox.Controls.Add(this.courseSelectTextsPanel);
            courseSelectTextsGroupBox.Location = new System.Drawing.Point(322, 6);
            courseSelectTextsGroupBox.Name = "courseSelectTextsGroupBox";
            courseSelectTextsGroupBox.Padding = new System.Windows.Forms.Padding(0, 0, 3, 3);
            courseSelectTextsGroupBox.Size = new System.Drawing.Size(189, 400);
            courseSelectTextsGroupBox.TabIndex = 2;
            courseSelectTextsGroupBox.TabStop = false;
            courseSelectTextsGroupBox.Text = "Course Select texts";
            // 
            // courseSelectTextsPanel
            // 
            this.courseSelectTextsPanel.AutoScroll = true;
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox1);
            this.courseSelectTextsPanel.Controls.Add(this.courseSelectTextsCountLabel);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox10);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox2);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox11);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox3);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox9);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox4);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox12);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox5);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox8);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox6);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox13);
            this.courseSelectTextsPanel.Controls.Add(this.courseTextBox7);
            this.courseSelectTextsPanel.Location = new System.Drawing.Point(5, 16);
            this.courseSelectTextsPanel.Margin = new System.Windows.Forms.Padding(0);
            this.courseSelectTextsPanel.Name = "courseSelectTextsPanel";
            this.courseSelectTextsPanel.Size = new System.Drawing.Size(181, 210);
            this.courseSelectTextsPanel.TabIndex = 0;
            // 
            // courseTextBox1
            // 
            this.courseTextBox1.Location = new System.Drawing.Point(3, 3);
            this.courseTextBox1.Name = "courseTextBox1";
            this.courseTextBox1.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox1.TabIndex = 0;
            this.courseTextBox1.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseSelectTextsCountLabel
            // 
            this.courseSelectTextsCountLabel.Location = new System.Drawing.Point(39, 338);
            this.courseSelectTextsCountLabel.Name = "courseSelectTextsCountLabel";
            this.courseSelectTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.courseSelectTextsCountLabel.TabIndex = 13;
            this.courseSelectTextsCountLabel.Text = "...";
            this.courseSelectTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // courseTextBox10
            // 
            this.courseTextBox10.Location = new System.Drawing.Point(3, 237);
            this.courseTextBox10.Name = "courseTextBox10";
            this.courseTextBox10.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox10.TabIndex = 9;
            this.courseTextBox10.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox2
            // 
            this.courseTextBox2.Location = new System.Drawing.Point(3, 29);
            this.courseTextBox2.Name = "courseTextBox2";
            this.courseTextBox2.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox2.TabIndex = 1;
            this.courseTextBox2.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox11
            // 
            this.courseTextBox11.Location = new System.Drawing.Point(3, 263);
            this.courseTextBox11.Name = "courseTextBox11";
            this.courseTextBox11.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox11.TabIndex = 10;
            this.courseTextBox11.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox3
            // 
            this.courseTextBox3.Location = new System.Drawing.Point(3, 55);
            this.courseTextBox3.Name = "courseTextBox3";
            this.courseTextBox3.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox3.TabIndex = 2;
            this.courseTextBox3.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox9
            // 
            this.courseTextBox9.Location = new System.Drawing.Point(3, 211);
            this.courseTextBox9.Name = "courseTextBox9";
            this.courseTextBox9.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox9.TabIndex = 8;
            this.courseTextBox9.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox4
            // 
            this.courseTextBox4.Location = new System.Drawing.Point(3, 81);
            this.courseTextBox4.Name = "courseTextBox4";
            this.courseTextBox4.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox4.TabIndex = 3;
            this.courseTextBox4.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox12
            // 
            this.courseTextBox12.Location = new System.Drawing.Point(3, 289);
            this.courseTextBox12.Name = "courseTextBox12";
            this.courseTextBox12.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox12.TabIndex = 11;
            this.courseTextBox12.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox5
            // 
            this.courseTextBox5.Location = new System.Drawing.Point(3, 107);
            this.courseTextBox5.Name = "courseTextBox5";
            this.courseTextBox5.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox5.TabIndex = 4;
            this.courseTextBox5.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox8
            // 
            this.courseTextBox8.Location = new System.Drawing.Point(3, 185);
            this.courseTextBox8.Name = "courseTextBox8";
            this.courseTextBox8.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox8.TabIndex = 7;
            this.courseTextBox8.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox6
            // 
            this.courseTextBox6.Location = new System.Drawing.Point(3, 133);
            this.courseTextBox6.Name = "courseTextBox6";
            this.courseTextBox6.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox6.TabIndex = 5;
            this.courseTextBox6.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox13
            // 
            this.courseTextBox13.Location = new System.Drawing.Point(3, 315);
            this.courseTextBox13.Name = "courseTextBox13";
            this.courseTextBox13.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox13.TabIndex = 12;
            this.courseTextBox13.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // courseTextBox7
            // 
            this.courseTextBox7.Location = new System.Drawing.Point(3, 159);
            this.courseTextBox7.Name = "courseTextBox7";
            this.courseTextBox7.Size = new System.Drawing.Size(136, 20);
            this.courseTextBox7.TabIndex = 6;
            this.courseTextBox7.TextChanged += new System.EventHandler(this.CourseSelectTextsTextBoxTextChanged);
            // 
            // gpCupSelectTextsGroupBox
            // 
            this.gpCupSelectTextsGroupBox.Controls.Add(this.gpCupSelectTextsCountLabel);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.cupTextBox1);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.cupTextBox2);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.cupTextBox3);
            this.gpCupSelectTextsGroupBox.Controls.Add(this.cupTextBox4);
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
            // cupTextBox1
            // 
            this.cupTextBox1.Location = new System.Drawing.Point(8, 19);
            this.cupTextBox1.Name = "cupTextBox1";
            this.cupTextBox1.Size = new System.Drawing.Size(136, 20);
            this.cupTextBox1.TabIndex = 0;
            this.cupTextBox1.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // cupTextBox2
            // 
            this.cupTextBox2.Location = new System.Drawing.Point(8, 45);
            this.cupTextBox2.Name = "cupTextBox2";
            this.cupTextBox2.Size = new System.Drawing.Size(136, 20);
            this.cupTextBox2.TabIndex = 1;
            this.cupTextBox2.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // cupTextBox3
            // 
            this.cupTextBox3.Location = new System.Drawing.Point(8, 71);
            this.cupTextBox3.Name = "cupTextBox3";
            this.cupTextBox3.Size = new System.Drawing.Size(136, 20);
            this.cupTextBox3.TabIndex = 2;
            this.cupTextBox3.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // cupTextBox4
            // 
            this.cupTextBox4.Location = new System.Drawing.Point(8, 97);
            this.cupTextBox4.Name = "cupTextBox4";
            this.cupTextBox4.Size = new System.Drawing.Size(136, 20);
            this.cupTextBox4.TabIndex = 3;
            this.cupTextBox4.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // gpPodiumCupTextsGroupBox
            // 
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.gpPodiumCupTextsCountLabel);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox5);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox1);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox2);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox3);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox4);
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
            // podiumTextBox5
            // 
            this.podiumTextBox5.Location = new System.Drawing.Point(8, 123);
            this.podiumTextBox5.Name = "podiumTextBox5";
            this.podiumTextBox5.Size = new System.Drawing.Size(136, 20);
            this.podiumTextBox5.TabIndex = 4;
            this.podiumTextBox5.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox1
            // 
            this.podiumTextBox1.Location = new System.Drawing.Point(8, 19);
            this.podiumTextBox1.Name = "podiumTextBox1";
            this.podiumTextBox1.Size = new System.Drawing.Size(136, 20);
            this.podiumTextBox1.TabIndex = 0;
            this.podiumTextBox1.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox2
            // 
            this.podiumTextBox2.Location = new System.Drawing.Point(8, 45);
            this.podiumTextBox2.Name = "podiumTextBox2";
            this.podiumTextBox2.Size = new System.Drawing.Size(136, 20);
            this.podiumTextBox2.TabIndex = 1;
            this.podiumTextBox2.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox3
            // 
            this.podiumTextBox3.Location = new System.Drawing.Point(8, 71);
            this.podiumTextBox3.Name = "podiumTextBox3";
            this.podiumTextBox3.Size = new System.Drawing.Size(136, 20);
            this.podiumTextBox3.TabIndex = 2;
            this.podiumTextBox3.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox4
            // 
            this.podiumTextBox4.Location = new System.Drawing.Point(8, 97);
            this.podiumTextBox4.Name = "podiumTextBox4";
            this.podiumTextBox4.Size = new System.Drawing.Size(136, 20);
            this.podiumTextBox4.TabIndex = 3;
            this.podiumTextBox4.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // CupAndThemeTextsEditor
            // 
            this.Controls.Add(this.gpPodiumCupTextsGroupBox);
            this.Controls.Add(this.gpCupSelectTextsGroupBox);
            this.Controls.Add(courseSelectTextsGroupBox);
            this.Name = "CupAndThemeTextsEditor";
            this.Size = new System.Drawing.Size(514, 250);
            courseSelectTextsGroupBox.ResumeLayout(false);
            this.courseSelectTextsPanel.ResumeLayout(false);
            this.courseSelectTextsPanel.PerformLayout();
            this.gpCupSelectTextsGroupBox.ResumeLayout(false);
            this.gpCupSelectTextsGroupBox.PerformLayout();
            this.gpPodiumCupTextsGroupBox.ResumeLayout(false);
            this.gpPodiumCupTextsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label courseSelectTextsCountLabel;
        private System.Windows.Forms.TextBox courseTextBox10;
        private System.Windows.Forms.TextBox courseTextBox11;
        private System.Windows.Forms.TextBox courseTextBox12;
        private System.Windows.Forms.TextBox courseTextBox13;
        private System.Windows.Forms.TextBox courseTextBox9;
        private System.Windows.Forms.TextBox courseTextBox5;
        private System.Windows.Forms.TextBox courseTextBox6;
        private System.Windows.Forms.TextBox courseTextBox7;
        private System.Windows.Forms.TextBox courseTextBox8;
        private System.Windows.Forms.TextBox courseTextBox4;
        private System.Windows.Forms.TextBox courseTextBox3;
        private System.Windows.Forms.TextBox courseTextBox2;
        private System.Windows.Forms.TextBox courseTextBox1;
        private System.Windows.Forms.TextBox cupTextBox1;
        private System.Windows.Forms.TextBox cupTextBox2;
        private System.Windows.Forms.TextBox cupTextBox3;
        private System.Windows.Forms.TextBox cupTextBox4;
        private System.Windows.Forms.GroupBox gpCupSelectTextsGroupBox;
        private System.Windows.Forms.Label gpCupSelectTextsCountLabel;
        private System.Windows.Forms.GroupBox gpPodiumCupTextsGroupBox;
        private System.Windows.Forms.TextBox podiumTextBox1;
        private System.Windows.Forms.TextBox podiumTextBox2;
        private System.Windows.Forms.TextBox podiumTextBox3;
        private System.Windows.Forms.TextBox podiumTextBox4;
        private System.Windows.Forms.Label gpPodiumCupTextsCountLabel;
        private System.Windows.Forms.TextBox podiumTextBox5;
        private System.Windows.Forms.Panel courseSelectTextsPanel;
    }
}
