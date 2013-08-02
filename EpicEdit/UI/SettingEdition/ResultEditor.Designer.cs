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
    partial class ResultEditor
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
            System.Windows.Forms.GroupBox pointsGroupBox;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            this.numericUpDown8 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown7 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            pointsGroupBox = new System.Windows.Forms.GroupBox();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            pointsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // pointsGroupBox
            // 
            pointsGroupBox.Controls.Add(this.numericUpDown8);
            pointsGroupBox.Controls.Add(label8);
            pointsGroupBox.Controls.Add(this.numericUpDown7);
            pointsGroupBox.Controls.Add(label7);
            pointsGroupBox.Controls.Add(this.numericUpDown6);
            pointsGroupBox.Controls.Add(label6);
            pointsGroupBox.Controls.Add(this.numericUpDown5);
            pointsGroupBox.Controls.Add(label5);
            pointsGroupBox.Controls.Add(this.numericUpDown4);
            pointsGroupBox.Controls.Add(label4);
            pointsGroupBox.Controls.Add(this.numericUpDown3);
            pointsGroupBox.Controls.Add(label3);
            pointsGroupBox.Controls.Add(this.numericUpDown2);
            pointsGroupBox.Controls.Add(label2);
            pointsGroupBox.Controls.Add(this.numericUpDown1);
            pointsGroupBox.Controls.Add(label1);
            pointsGroupBox.Location = new System.Drawing.Point(3, 3);
            pointsGroupBox.Name = "pointsGroupBox";
            pointsGroupBox.Size = new System.Drawing.Size(122, 244);
            pointsGroupBox.TabIndex = 0;
            pointsGroupBox.TabStop = false;
            pointsGroupBox.Text = "Ranking points";
            // 
            // numericUpDown8
            // 
            this.numericUpDown8.Location = new System.Drawing.Point(50, 206);
            this.numericUpDown8.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown8.Name = "numericUpDown8";
            this.numericUpDown8.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown8.TabIndex = 15;
            this.numericUpDown8.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(23, 208);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(22, 13);
            label8.TabIndex = 14;
            label8.Text = "8th";
            // 
            // numericUpDown7
            // 
            this.numericUpDown7.Location = new System.Drawing.Point(50, 180);
            this.numericUpDown7.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown7.Name = "numericUpDown7";
            this.numericUpDown7.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown7.TabIndex = 13;
            this.numericUpDown7.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(23, 182);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(22, 13);
            label7.TabIndex = 12;
            label7.Text = "7th";
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.Location = new System.Drawing.Point(50, 154);
            this.numericUpDown6.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown6.TabIndex = 11;
            this.numericUpDown6.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(23, 156);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(22, 13);
            label6.TabIndex = 10;
            label6.Text = "6th";
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.Location = new System.Drawing.Point(50, 128);
            this.numericUpDown5.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown5.TabIndex = 9;
            this.numericUpDown5.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(23, 130);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(22, 13);
            label5.TabIndex = 8;
            label5.Text = "5th";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Location = new System.Drawing.Point(50, 102);
            this.numericUpDown4.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown4.TabIndex = 7;
            this.numericUpDown4.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(23, 104);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(22, 13);
            label4.TabIndex = 6;
            label4.Text = "4th";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Location = new System.Drawing.Point(50, 76);
            this.numericUpDown3.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown3.TabIndex = 5;
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(23, 78);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(22, 13);
            label3.TabIndex = 4;
            label3.Text = "3rd";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(50, 50);
            this.numericUpDown2.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown2.TabIndex = 3;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(23, 52);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(25, 13);
            label2.TabIndex = 2;
            label2.Text = "2nd";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(50, 24);
            this.numericUpDown1.Maximum = new decimal(new int[] {
                                    9,
                                    0,
                                    0,
                                    0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(40, 20);
            this.numericUpDown1.TabIndex = 1;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDownValueChanged);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(23, 26);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(21, 13);
            label1.TabIndex = 0;
            label1.Text = "1st";
            // 
            // ResultEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(pointsGroupBox);
            this.Name = "ResultEditor";
            this.Size = new System.Drawing.Size(420, 250);
            pointsGroupBox.ResumeLayout(false);
            pointsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.NumericUpDown numericUpDown4;
        private System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.NumericUpDown numericUpDown6;
        private System.Windows.Forms.NumericUpDown numericUpDown7;
        private System.Windows.Forms.NumericUpDown numericUpDown8;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
    }
}
