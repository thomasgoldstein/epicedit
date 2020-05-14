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
    partial class GPCupTextsEditor
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
            this.gpCupSelectTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpCupSelectTextsCountLabel = new System.Windows.Forms.Label();
            this.cupTextBox1 = new System.Windows.Forms.TextBox();
            this.cupTextBox2 = new System.Windows.Forms.TextBox();
            this.cupTextBox3 = new System.Windows.Forms.TextBox();
            this.cupTextBox4 = new System.Windows.Forms.TextBox();
            this.gpResultsCupTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpResultsCupTextsCountLabel = new System.Windows.Forms.Label();
            this.resultsTextBox1 = new System.Windows.Forms.TextBox();
            this.resultsTextBox2 = new System.Windows.Forms.TextBox();
            this.resultsTextBox3 = new System.Windows.Forms.TextBox();
            this.resultsTextBox4 = new System.Windows.Forms.TextBox();
            this.gpPodiumCupTextsGroupBox = new System.Windows.Forms.GroupBox();
            this.gpPodiumCupTextsCountLabel = new System.Windows.Forms.Label();
            this.podiumTextBox5 = new System.Windows.Forms.TextBox();
            this.podiumTextBox1 = new System.Windows.Forms.TextBox();
            this.podiumTextBox2 = new System.Windows.Forms.TextBox();
            this.podiumTextBox3 = new System.Windows.Forms.TextBox();
            this.podiumTextBox4 = new System.Windows.Forms.TextBox();
            this.gpCupSelectTextsGroupBox.SuspendLayout();
            this.gpResultsCupTextsGroupBox.SuspendLayout();
            this.gpPodiumCupTextsGroupBox.SuspendLayout();
            this.SuspendLayout();
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
            this.gpCupSelectTextsGroupBox.Size = new System.Drawing.Size(164, 244);
            this.gpCupSelectTextsGroupBox.TabIndex = 0;
            this.gpCupSelectTextsGroupBox.TabStop = false;
            this.gpCupSelectTextsGroupBox.Text = "GP Cup Select texts";
            // 
            // gpCupSelectTextsCountLabel
            // 
            this.gpCupSelectTextsCountLabel.Location = new System.Drawing.Point(56, 120);
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
            this.cupTextBox1.Size = new System.Drawing.Size(148, 20);
            this.cupTextBox1.TabIndex = 0;
            this.cupTextBox1.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // cupTextBox2
            // 
            this.cupTextBox2.Location = new System.Drawing.Point(8, 45);
            this.cupTextBox2.Name = "cupTextBox2";
            this.cupTextBox2.Size = new System.Drawing.Size(148, 20);
            this.cupTextBox2.TabIndex = 1;
            this.cupTextBox2.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // cupTextBox3
            // 
            this.cupTextBox3.Location = new System.Drawing.Point(8, 71);
            this.cupTextBox3.Name = "cupTextBox3";
            this.cupTextBox3.Size = new System.Drawing.Size(148, 20);
            this.cupTextBox3.TabIndex = 2;
            this.cupTextBox3.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // cupTextBox4
            // 
            this.cupTextBox4.Location = new System.Drawing.Point(8, 97);
            this.cupTextBox4.Name = "cupTextBox4";
            this.cupTextBox4.Size = new System.Drawing.Size(148, 20);
            this.cupTextBox4.TabIndex = 3;
            this.cupTextBox4.TextChanged += new System.EventHandler(this.GPCupSelectTextsTextBoxTextChanged);
            // 
            // gpResultsCupTextsGroupBox
            // 
            this.gpResultsCupTextsGroupBox.Controls.Add(this.gpResultsCupTextsCountLabel);
            this.gpResultsCupTextsGroupBox.Controls.Add(this.resultsTextBox1);
            this.gpResultsCupTextsGroupBox.Controls.Add(this.resultsTextBox2);
            this.gpResultsCupTextsGroupBox.Controls.Add(this.resultsTextBox3);
            this.gpResultsCupTextsGroupBox.Controls.Add(this.resultsTextBox4);
            this.gpResultsCupTextsGroupBox.Location = new System.Drawing.Point(176, 6);
            this.gpResultsCupTextsGroupBox.Name = "gpResultsCupTextsGroupBox";
            this.gpResultsCupTextsGroupBox.Size = new System.Drawing.Size(164, 244);
            this.gpResultsCupTextsGroupBox.TabIndex = 1;
            this.gpResultsCupTextsGroupBox.TabStop = false;
            this.gpResultsCupTextsGroupBox.Text = "GP Results Cup texts";
            // 
            // gpResultsCupTextsCountLabel
            // 
            this.gpResultsCupTextsCountLabel.Location = new System.Drawing.Point(56, 120);
            this.gpResultsCupTextsCountLabel.Name = "gpResultsCupTextsCountLabel";
            this.gpResultsCupTextsCountLabel.Size = new System.Drawing.Size(100, 23);
            this.gpResultsCupTextsCountLabel.TabIndex = 5;
            this.gpResultsCupTextsCountLabel.Text = "...";
            this.gpResultsCupTextsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // resultsTextBox1
            // 
            this.resultsTextBox1.Location = new System.Drawing.Point(8, 19);
            this.resultsTextBox1.Name = "resultsTextBox1";
            this.resultsTextBox1.Size = new System.Drawing.Size(148, 20);
            this.resultsTextBox1.TabIndex = 0;
            this.resultsTextBox1.TextChanged += new System.EventHandler(this.GPResultsCupTextsTextBoxTextChanged);
            // 
            // resultsTextBox2
            // 
            this.resultsTextBox2.Location = new System.Drawing.Point(8, 45);
            this.resultsTextBox2.Name = "resultsTextBox2";
            this.resultsTextBox2.Size = new System.Drawing.Size(148, 20);
            this.resultsTextBox2.TabIndex = 1;
            this.resultsTextBox2.TextChanged += new System.EventHandler(this.GPResultsCupTextsTextBoxTextChanged);
            // 
            // resultsTextBox3
            // 
            this.resultsTextBox3.Location = new System.Drawing.Point(8, 71);
            this.resultsTextBox3.Name = "resultsTextBox3";
            this.resultsTextBox3.Size = new System.Drawing.Size(148, 20);
            this.resultsTextBox3.TabIndex = 2;
            this.resultsTextBox3.TextChanged += new System.EventHandler(this.GPResultsCupTextsTextBoxTextChanged);
            // 
            // resultsTextBox4
            // 
            this.resultsTextBox4.Location = new System.Drawing.Point(8, 97);
            this.resultsTextBox4.Name = "resultsTextBox4";
            this.resultsTextBox4.Size = new System.Drawing.Size(148, 20);
            this.resultsTextBox4.TabIndex = 3;
            this.resultsTextBox4.TextChanged += new System.EventHandler(this.GPResultsCupTextsTextBoxTextChanged);
            // 
            // gpPodiumCupTextsGroupBox
            // 
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.gpPodiumCupTextsCountLabel);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox5);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox1);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox2);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox3);
            this.gpPodiumCupTextsGroupBox.Controls.Add(this.podiumTextBox4);
            this.gpPodiumCupTextsGroupBox.Location = new System.Drawing.Point(346, 6);
            this.gpPodiumCupTextsGroupBox.Name = "gpPodiumCupTextsGroupBox";
            this.gpPodiumCupTextsGroupBox.Size = new System.Drawing.Size(164, 244);
            this.gpPodiumCupTextsGroupBox.TabIndex = 2;
            this.gpPodiumCupTextsGroupBox.TabStop = false;
            this.gpPodiumCupTextsGroupBox.Text = "GP Podium Cup texts";
            // 
            // gpPodiumCupTextsCountLabel
            // 
            this.gpPodiumCupTextsCountLabel.Location = new System.Drawing.Point(56, 146);
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
            this.podiumTextBox5.Size = new System.Drawing.Size(148, 20);
            this.podiumTextBox5.TabIndex = 4;
            this.podiumTextBox5.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox1
            // 
            this.podiumTextBox1.Location = new System.Drawing.Point(8, 19);
            this.podiumTextBox1.Name = "podiumTextBox1";
            this.podiumTextBox1.Size = new System.Drawing.Size(148, 20);
            this.podiumTextBox1.TabIndex = 0;
            this.podiumTextBox1.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox2
            // 
            this.podiumTextBox2.Location = new System.Drawing.Point(8, 45);
            this.podiumTextBox2.Name = "podiumTextBox2";
            this.podiumTextBox2.Size = new System.Drawing.Size(148, 20);
            this.podiumTextBox2.TabIndex = 1;
            this.podiumTextBox2.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox3
            // 
            this.podiumTextBox3.Location = new System.Drawing.Point(8, 71);
            this.podiumTextBox3.Name = "podiumTextBox3";
            this.podiumTextBox3.Size = new System.Drawing.Size(148, 20);
            this.podiumTextBox3.TabIndex = 2;
            this.podiumTextBox3.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // podiumTextBox4
            // 
            this.podiumTextBox4.Location = new System.Drawing.Point(8, 97);
            this.podiumTextBox4.Name = "podiumTextBox4";
            this.podiumTextBox4.Size = new System.Drawing.Size(148, 20);
            this.podiumTextBox4.TabIndex = 3;
            this.podiumTextBox4.TextChanged += new System.EventHandler(this.GPPodiumCupTextsTextBoxTextChanged);
            // 
            // GPCupTextsEditor
            // 
            this.Controls.Add(this.gpPodiumCupTextsGroupBox);
            this.Controls.Add(this.gpResultsCupTextsGroupBox);
            this.Controls.Add(this.gpCupSelectTextsGroupBox);
            this.Name = "GPCupTextsEditor";
            this.Size = new System.Drawing.Size(514, 250);
            this.gpCupSelectTextsGroupBox.ResumeLayout(false);
            this.gpCupSelectTextsGroupBox.PerformLayout();
            this.gpResultsCupTextsGroupBox.ResumeLayout(false);
            this.gpResultsCupTextsGroupBox.PerformLayout();
            this.gpPodiumCupTextsGroupBox.ResumeLayout(false);
            this.gpPodiumCupTextsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TextBox cupTextBox1;
        private System.Windows.Forms.TextBox cupTextBox2;
        private System.Windows.Forms.TextBox cupTextBox3;
        private System.Windows.Forms.TextBox cupTextBox4;
        private System.Windows.Forms.GroupBox gpCupSelectTextsGroupBox;
        private System.Windows.Forms.Label gpCupSelectTextsCountLabel;
        private System.Windows.Forms.GroupBox gpResultsCupTextsGroupBox;
        private System.Windows.Forms.TextBox resultsTextBox1;
        private System.Windows.Forms.TextBox resultsTextBox2;
        private System.Windows.Forms.TextBox resultsTextBox3;
        private System.Windows.Forms.TextBox resultsTextBox4;
        private System.Windows.Forms.Label gpResultsCupTextsCountLabel;
        private System.Windows.Forms.GroupBox gpPodiumCupTextsGroupBox;
        private System.Windows.Forms.Label gpPodiumCupTextsCountLabel;
        private System.Windows.Forms.TextBox podiumTextBox5;
        private System.Windows.Forms.TextBox podiumTextBox1;
        private System.Windows.Forms.TextBox podiumTextBox2;
        private System.Windows.Forms.TextBox podiumTextBox3;
        private System.Windows.Forms.TextBox podiumTextBox4;
    }
}
