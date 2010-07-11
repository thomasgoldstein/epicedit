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
	partial class AIControl
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
			System.Windows.Forms.Label aiIndexLabel;
			System.Windows.Forms.Label aiShapeLabel;
			System.Windows.Forms.Label aiSpeedLabel;
			this.selectedAIElementGroupBox = new System.Windows.Forms.GroupBox();
			this.aiIndexNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.aiShapeComboBox = new System.Windows.Forms.ComboBox();
			this.aiDeleteButton = new System.Windows.Forms.Button();
			this.aiSpeedNumericUpDown = new System.Windows.Forms.NumericUpDown();
			aiIndexLabel = new System.Windows.Forms.Label();
			aiShapeLabel = new System.Windows.Forms.Label();
			aiSpeedLabel = new System.Windows.Forms.Label();
			this.selectedAIElementGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.aiIndexNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.aiSpeedNumericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// aiIndexLabel
			// 
			aiIndexLabel.Location = new System.Drawing.Point(6, 32);
			aiIndexLabel.Name = "aiIndexLabel";
			aiIndexLabel.Size = new System.Drawing.Size(45, 23);
			aiIndexLabel.TabIndex = 0;
			aiIndexLabel.Text = "Index";
			// 
			// aiShapeLabel
			// 
			aiShapeLabel.Location = new System.Drawing.Point(6, 101);
			aiShapeLabel.Name = "aiShapeLabel";
			aiShapeLabel.Size = new System.Drawing.Size(45, 23);
			aiShapeLabel.TabIndex = 4;
			aiShapeLabel.Text = "Shape";
			// 
			// aiSpeedLabel
			// 
			aiSpeedLabel.Location = new System.Drawing.Point(6, 65);
			aiSpeedLabel.Name = "aiSpeedLabel";
			aiSpeedLabel.Size = new System.Drawing.Size(45, 23);
			aiSpeedLabel.TabIndex = 2;
			aiSpeedLabel.Text = "Speed";
			// 
			// selectedAIElementGroupBox
			// 
			this.selectedAIElementGroupBox.Controls.Add(this.aiIndexNumericUpDown);
			this.selectedAIElementGroupBox.Controls.Add(aiIndexLabel);
			this.selectedAIElementGroupBox.Controls.Add(this.aiShapeComboBox);
			this.selectedAIElementGroupBox.Controls.Add(this.aiDeleteButton);
			this.selectedAIElementGroupBox.Controls.Add(aiShapeLabel);
			this.selectedAIElementGroupBox.Controls.Add(aiSpeedLabel);
			this.selectedAIElementGroupBox.Controls.Add(this.aiSpeedNumericUpDown);
			this.selectedAIElementGroupBox.Location = new System.Drawing.Point(2, 5);
			this.selectedAIElementGroupBox.Name = "selectedAIElementGroupBox";
			this.selectedAIElementGroupBox.Size = new System.Drawing.Size(124, 219);
			this.selectedAIElementGroupBox.TabIndex = 1;
			this.selectedAIElementGroupBox.TabStop = false;
			this.selectedAIElementGroupBox.Text = "Selected Element";
			// 
			// aiIndexNumericUpDown
			// 
			this.aiIndexNumericUpDown.Location = new System.Drawing.Point(69, 30);
			this.aiIndexNumericUpDown.Maximum = new decimal(new int[] {
									127,
									0,
									0,
									0});
			this.aiIndexNumericUpDown.Name = "aiIndexNumericUpDown";
			this.aiIndexNumericUpDown.Size = new System.Drawing.Size(45, 20);
			this.aiIndexNumericUpDown.TabIndex = 1;
			this.aiIndexNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.aiIndexNumericUpDown.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.aiIndexNumericUpDown.ValueChanged += new System.EventHandler(this.AIIndexNumericUpDownValueChanged);
			// 
			// aiShapeComboBox
			// 
			this.aiShapeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.aiShapeComboBox.FormattingEnabled = true;
			this.aiShapeComboBox.Location = new System.Drawing.Point(10, 127);
			this.aiShapeComboBox.Name = "aiShapeComboBox";
			this.aiShapeComboBox.Size = new System.Drawing.Size(104, 21);
			this.aiShapeComboBox.TabIndex = 5;
			this.aiShapeComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.AIShapeComboBoxFormat);
			// 
			// aiDeleteButton
			// 
			this.aiDeleteButton.Location = new System.Drawing.Point(25, 176);
			this.aiDeleteButton.Name = "aiDeleteButton";
			this.aiDeleteButton.Size = new System.Drawing.Size(74, 23);
			this.aiDeleteButton.TabIndex = 6;
			this.aiDeleteButton.Text = "Delete";
			this.aiDeleteButton.UseVisualStyleBackColor = true;
			this.aiDeleteButton.Click += new System.EventHandler(this.AIDeleteButtonClick);
			// 
			// aiSpeedNumericUpDown
			// 
			this.aiSpeedNumericUpDown.Location = new System.Drawing.Point(69, 63);
			this.aiSpeedNumericUpDown.Maximum = new decimal(new int[] {
									4,
									0,
									0,
									0});
			this.aiSpeedNumericUpDown.Minimum = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.aiSpeedNumericUpDown.Name = "aiSpeedNumericUpDown";
			this.aiSpeedNumericUpDown.Size = new System.Drawing.Size(45, 20);
			this.aiSpeedNumericUpDown.TabIndex = 3;
			this.aiSpeedNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.aiSpeedNumericUpDown.Value = new decimal(new int[] {
									1,
									0,
									0,
									0});
			this.aiSpeedNumericUpDown.ValueChanged += new System.EventHandler(this.AISpeedNumericUpDownValueChanged);
			// 
			// AIControl
			// 
			this.Controls.Add(this.selectedAIElementGroupBox);
			this.Name = "AIControl";
			this.Size = new System.Drawing.Size(130, 230);
			this.selectedAIElementGroupBox.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.aiIndexNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.aiSpeedNumericUpDown)).EndInit();
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.NumericUpDown aiSpeedNumericUpDown;
		private System.Windows.Forms.Button aiDeleteButton;
		private System.Windows.Forms.ComboBox aiShapeComboBox;
		private System.Windows.Forms.NumericUpDown aiIndexNumericUpDown;
		private System.Windows.Forms.GroupBox selectedAIElementGroupBox;
	}
}
