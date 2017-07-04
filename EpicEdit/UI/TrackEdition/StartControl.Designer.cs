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

namespace EpicEdit.UI.TrackEdition
{
    partial class StartControl
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
            System.Windows.Forms.Label secondRowLabel;
            this.precisionGroupBox = new System.Windows.Forms.GroupBox();
            this.step1pxRadioButton = new System.Windows.Forms.RadioButton();
            this.step8pxRadioButton = new System.Windows.Forms.RadioButton();
            this.step4pxRadioButton = new System.Windows.Forms.RadioButton();
            this.gpTrackGroupBox = new System.Windows.Forms.GroupBox();
            this.startBindCheckBox = new System.Windows.Forms.CheckBox();
            this.secondRowValueLabel = new System.Windows.Forms.Label();
            this.secondRowTrackBar = new System.Windows.Forms.TrackBar();
            secondRowLabel = new System.Windows.Forms.Label();
            this.precisionGroupBox.SuspendLayout();
            this.gpTrackGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.secondRowTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // secondRowLabel
            // 
            secondRowLabel.Location = new System.Drawing.Point(2, 66);
            secondRowLabel.Name = "secondRowLabel";
            secondRowLabel.Size = new System.Drawing.Size(120, 16);
            secondRowLabel.TabIndex = 1;
            secondRowLabel.Text = "2nd row offset";
            secondRowLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // precisionGroupBox
            // 
            this.precisionGroupBox.Controls.Add(this.step1pxRadioButton);
            this.precisionGroupBox.Controls.Add(this.step8pxRadioButton);
            this.precisionGroupBox.Controls.Add(this.step4pxRadioButton);
            this.precisionGroupBox.Location = new System.Drawing.Point(2, 4);
            this.precisionGroupBox.Name = "precisionGroupBox";
            this.precisionGroupBox.Size = new System.Drawing.Size(124, 120);
            this.precisionGroupBox.TabIndex = 0;
            this.precisionGroupBox.TabStop = false;
            this.precisionGroupBox.Text = "Precision";
            // 
            // step1pxRadioButton
            // 
            this.step1pxRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.step1pxRadioButton.Location = new System.Drawing.Point(47, 19);
            this.step1pxRadioButton.Name = "step1pxRadioButton";
            this.step1pxRadioButton.Size = new System.Drawing.Size(39, 24);
            this.step1pxRadioButton.TabIndex = 0;
            this.step1pxRadioButton.Text = "1px";
            this.step1pxRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.step1pxRadioButton.UseVisualStyleBackColor = true;
            this.step1pxRadioButton.CheckedChanged += new System.EventHandler(this.StepRadioButtonCheckedChanged);
            // 
            // step8pxRadioButton
            // 
            this.step8pxRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.step8pxRadioButton.Location = new System.Drawing.Point(47, 79);
            this.step8pxRadioButton.Name = "step8pxRadioButton";
            this.step8pxRadioButton.Size = new System.Drawing.Size(39, 24);
            this.step8pxRadioButton.TabIndex = 2;
            this.step8pxRadioButton.Text = "8px";
            this.step8pxRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.step8pxRadioButton.UseVisualStyleBackColor = true;
            this.step8pxRadioButton.CheckedChanged += new System.EventHandler(this.StepRadioButtonCheckedChanged);
            // 
            // step4pxRadioButton
            // 
            this.step4pxRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.step4pxRadioButton.Checked = true;
            this.step4pxRadioButton.Location = new System.Drawing.Point(47, 49);
            this.step4pxRadioButton.Name = "step4pxRadioButton";
            this.step4pxRadioButton.Size = new System.Drawing.Size(39, 24);
            this.step4pxRadioButton.TabIndex = 1;
            this.step4pxRadioButton.TabStop = true;
            this.step4pxRadioButton.Text = "4px";
            this.step4pxRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.step4pxRadioButton.UseVisualStyleBackColor = true;
            this.step4pxRadioButton.CheckedChanged += new System.EventHandler(this.StepRadioButtonCheckedChanged);
            // 
            // gpTrackGroupBox
            // 
            this.gpTrackGroupBox.Controls.Add(this.startBindCheckBox);
            this.gpTrackGroupBox.Controls.Add(this.secondRowValueLabel);
            this.gpTrackGroupBox.Controls.Add(secondRowLabel);
            this.gpTrackGroupBox.Controls.Add(this.secondRowTrackBar);
            this.gpTrackGroupBox.Location = new System.Drawing.Point(2, 137);
            this.gpTrackGroupBox.Name = "gpTrackGroupBox";
            this.gpTrackGroupBox.Size = new System.Drawing.Size(124, 124);
            this.gpTrackGroupBox.TabIndex = 1;
            this.gpTrackGroupBox.TabStop = false;
            this.gpTrackGroupBox.Text = "GP Track Settings";
            // 
            // startBindCheckBox
            // 
            this.startBindCheckBox.Checked = true;
            this.startBindCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.startBindCheckBox.Location = new System.Drawing.Point(15, 19);
            this.startBindCheckBox.Name = "startBindCheckBox";
            this.startBindCheckBox.Size = new System.Drawing.Size(103, 32);
            this.startBindCheckBox.TabIndex = 0;
            this.startBindCheckBox.Text = "Bind lap line && driver positions";
            this.startBindCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.startBindCheckBox.UseVisualStyleBackColor = true;
            // 
            // secondRowValueLabel
            // 
            this.secondRowValueLabel.Location = new System.Drawing.Point(1, 85);
            this.secondRowValueLabel.Margin = new System.Windows.Forms.Padding(0);
            this.secondRowValueLabel.Name = "secondRowValueLabel";
            this.secondRowValueLabel.Size = new System.Drawing.Size(35, 25);
            this.secondRowValueLabel.TabIndex = 2;
            this.secondRowValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // secondRowTrackBar
            // 
            this.secondRowTrackBar.AutoSize = false;
            this.secondRowTrackBar.Location = new System.Drawing.Point(30, 85);
            this.secondRowTrackBar.Maximum = 255;
            this.secondRowTrackBar.Minimum = -256;
            this.secondRowTrackBar.Name = "secondRowTrackBar";
            this.secondRowTrackBar.Size = new System.Drawing.Size(92, 25);
            this.secondRowTrackBar.TabIndex = 3;
            this.secondRowTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.secondRowTrackBar.Scroll += new System.EventHandler(this.SecondRowTrackBarScroll);
            this.secondRowTrackBar.ValueChanged += new System.EventHandler(this.SecondRowTrackBarValueChanged);
            // 
            // StartControl
            // 
            this.Controls.Add(this.gpTrackGroupBox);
            this.Controls.Add(this.precisionGroupBox);
            this.Name = "StartControl";
            this.Size = new System.Drawing.Size(130, 270);
            this.precisionGroupBox.ResumeLayout(false);
            this.gpTrackGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.secondRowTrackBar)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.GroupBox gpTrackGroupBox;
        private System.Windows.Forms.RadioButton step4pxRadioButton;
        private System.Windows.Forms.RadioButton step8pxRadioButton;
        private System.Windows.Forms.RadioButton step1pxRadioButton;
        private System.Windows.Forms.GroupBox precisionGroupBox;
        private System.Windows.Forms.CheckBox startBindCheckBox;
        private System.Windows.Forms.TrackBar secondRowTrackBar;
        private System.Windows.Forms.Label secondRowValueLabel;
    }
}