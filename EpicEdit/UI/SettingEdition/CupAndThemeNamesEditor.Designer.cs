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
    partial class CupAndThemeNamesEditor
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
            System.Windows.Forms.GroupBox cupNamesGroupBox;
            System.Windows.Forms.GroupBox themeNamesGroupBox;
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.countLabel = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            cupNamesGroupBox = new System.Windows.Forms.GroupBox();
            themeNamesGroupBox = new System.Windows.Forms.GroupBox();
            cupNamesGroupBox.SuspendLayout();
            themeNamesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cupNamesGroupBox
            // 
            cupNamesGroupBox.Controls.Add(this.textBox1);
            cupNamesGroupBox.Controls.Add(this.textBox2);
            cupNamesGroupBox.Controls.Add(this.textBox3);
            cupNamesGroupBox.Controls.Add(this.textBox4);
            cupNamesGroupBox.Location = new System.Drawing.Point(64, 3);
            cupNamesGroupBox.Name = "cupNamesGroupBox";
            cupNamesGroupBox.Size = new System.Drawing.Size(130, 244);
            cupNamesGroupBox.TabIndex = 15;
            cupNamesGroupBox.TabStop = false;
            cupNamesGroupBox.Text = "Cup names";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(10, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(108, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(10, 45);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(108, 20);
            this.textBox2.TabIndex = 2;
            this.textBox2.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(10, 71);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(108, 20);
            this.textBox3.TabIndex = 3;
            this.textBox3.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(10, 97);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(108, 20);
            this.textBox4.TabIndex = 4;
            this.textBox4.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // themeNamesGroupBox
            // 
            themeNamesGroupBox.Controls.Add(this.textBox8);
            themeNamesGroupBox.Controls.Add(this.textBox7);
            themeNamesGroupBox.Controls.Add(this.countLabel);
            themeNamesGroupBox.Controls.Add(this.textBox6);
            themeNamesGroupBox.Controls.Add(this.textBox10);
            themeNamesGroupBox.Controls.Add(this.textBox5);
            themeNamesGroupBox.Controls.Add(this.textBox11);
            themeNamesGroupBox.Controls.Add(this.textBox9);
            themeNamesGroupBox.Controls.Add(this.textBox12);
            themeNamesGroupBox.Controls.Add(this.textBox13);
            themeNamesGroupBox.Location = new System.Drawing.Point(200, 3);
            themeNamesGroupBox.Name = "themeNamesGroupBox";
            themeNamesGroupBox.Size = new System.Drawing.Size(244, 244);
            themeNamesGroupBox.TabIndex = 16;
            themeNamesGroupBox.TabStop = false;
            themeNamesGroupBox.Text = "Theme names&";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(10, 19);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(108, 20);
            this.textBox8.TabIndex = 5;
            this.textBox8.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(10, 45);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(108, 20);
            this.textBox7.TabIndex = 6;
            this.textBox7.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // countLabel
            // 
            this.countLabel.Location = new System.Drawing.Point(132, 123);
            this.countLabel.Name = "countLabel";
            this.countLabel.Size = new System.Drawing.Size(100, 23);
            this.countLabel.TabIndex = 14;
            this.countLabel.Text = "...";
            this.countLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(10, 71);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(108, 20);
            this.textBox6.TabIndex = 7;
            this.textBox6.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(124, 97);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(108, 20);
            this.textBox10.TabIndex = 13;
            this.textBox10.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(10, 97);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(108, 20);
            this.textBox5.TabIndex = 8;
            this.textBox5.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(124, 71);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(108, 20);
            this.textBox11.TabIndex = 12;
            this.textBox11.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(10, 123);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(108, 20);
            this.textBox9.TabIndex = 9;
            this.textBox9.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(124, 45);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(108, 20);
            this.textBox12.TabIndex = 11;
            this.textBox12.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // textBox13
            // 
            this.textBox13.Location = new System.Drawing.Point(124, 19);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(108, 20);
            this.textBox13.TabIndex = 10;
            this.textBox13.TextChanged += new System.EventHandler(this.TextBoxTextChanged);
            // 
            // CupAndThemeNamesEditor
            // 
            this.Controls.Add(themeNamesGroupBox);
            this.Controls.Add(cupNamesGroupBox);
            this.Name = "CupAndThemeNamesEditor";
            this.Size = new System.Drawing.Size(514, 250);
            cupNamesGroupBox.ResumeLayout(false);
            cupNamesGroupBox.PerformLayout();
            themeNamesGroupBox.ResumeLayout(false);
            themeNamesGroupBox.PerformLayout();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Label countLabel;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
    }
}
