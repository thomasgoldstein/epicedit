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

namespace EpicEdit.UI.ThemeEdition
{
    partial class ColorPicker
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }

                if (this.basicColorsBitmap != null)
                {
                    this.basicColorsBitmap.Dispose();
                }

                if (this.basicColorsCache != null)
                {
                    this.basicColorsCache.Dispose();
                }

                if (this.shadesBitmap != null)
                {
                    this.shadesBitmap.Dispose();
                }

                if (this.shadesCache != null)
                {
                    this.shadesCache.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.basicColorsPictureBox = new System.Windows.Forms.PictureBox();
            this.shadesPictureBox = new System.Windows.Forms.PictureBox();
            this.redNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.greenNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.blueNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.redLabel = new System.Windows.Forms.Label();
            this.greenLabel = new System.Windows.Forms.Label();
            this.blueLabel = new System.Windows.Forms.Label();
            this.newColorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.oldColorToolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.basicColorsPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shadesPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // basicColorsPictureBox
            // 
            this.basicColorsPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.basicColorsPictureBox.Location = new System.Drawing.Point(0, 0);
            this.basicColorsPictureBox.Name = "basicColorsPictureBox";
            this.basicColorsPictureBox.Size = new System.Drawing.Size(188, 17);
            this.basicColorsPictureBox.TabIndex = 8;
            this.basicColorsPictureBox.TabStop = false;
            this.basicColorsPictureBox.Click += new System.EventHandler(this.BasicColorsPictureBoxClick);
            this.basicColorsPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.BasicColorsPictureBoxPaint);
            this.basicColorsPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BasicColorsPictureBoxMouseMove);
            // 
            // shadesPictureBox
            // 
            this.shadesPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shadesPictureBox.Location = new System.Drawing.Point(0, 22);
            this.shadesPictureBox.Name = "shadesPictureBox";
            this.shadesPictureBox.Size = new System.Drawing.Size(130, 130);
            this.shadesPictureBox.TabIndex = 9;
            this.shadesPictureBox.TabStop = false;
            this.shadesPictureBox.Click += new System.EventHandler(this.ShadesPictureBoxClick);
            this.shadesPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.ShadesPictureBoxPaint);
            this.shadesPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ShadesPictureBoxMouseMove);
            // 
            // redNumericUpDown
            // 
            this.redNumericUpDown.Location = new System.Drawing.Point(136, 38);
            this.redNumericUpDown.Maximum = new decimal(new int[] {
                                    31,
                                    0,
                                    0,
                                    0});
            this.redNumericUpDown.Name = "redNumericUpDown";
            this.redNumericUpDown.Size = new System.Drawing.Size(53, 20);
            this.redNumericUpDown.TabIndex = 10;
            this.redNumericUpDown.ValueChanged += new System.EventHandler(this.RgbValueChanged);
            // 
            // greenNumericUpDown
            // 
            this.greenNumericUpDown.Location = new System.Drawing.Point(136, 85);
            this.greenNumericUpDown.Maximum = new decimal(new int[] {
                                    31,
                                    0,
                                    0,
                                    0});
            this.greenNumericUpDown.Name = "greenNumericUpDown";
            this.greenNumericUpDown.Size = new System.Drawing.Size(53, 20);
            this.greenNumericUpDown.TabIndex = 11;
            this.greenNumericUpDown.ValueChanged += new System.EventHandler(this.RgbValueChanged);
            // 
            // blueNumericUpDown
            // 
            this.blueNumericUpDown.Location = new System.Drawing.Point(136, 132);
            this.blueNumericUpDown.Maximum = new decimal(new int[] {
                                    31,
                                    0,
                                    0,
                                    0});
            this.blueNumericUpDown.Name = "blueNumericUpDown";
            this.blueNumericUpDown.Size = new System.Drawing.Size(53, 20);
            this.blueNumericUpDown.TabIndex = 12;
            this.blueNumericUpDown.ValueChanged += new System.EventHandler(this.RgbValueChanged);
            // 
            // redLabel
            // 
            this.redLabel.AutoSize = true;
            this.redLabel.Location = new System.Drawing.Point(133, 22);
            this.redLabel.Name = "redLabel";
            this.redLabel.Size = new System.Drawing.Size(27, 13);
            this.redLabel.TabIndex = 5;
            this.redLabel.Text = "Red";
            // 
            // greenLabel
            // 
            this.greenLabel.AutoSize = true;
            this.greenLabel.Location = new System.Drawing.Point(133, 69);
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Size = new System.Drawing.Size(36, 13);
            this.greenLabel.TabIndex = 14;
            this.greenLabel.Text = "Green";
            // 
            // blueLabel
            // 
            this.blueLabel.AutoSize = true;
            this.blueLabel.Location = new System.Drawing.Point(133, 116);
            this.blueLabel.Name = "blueLabel";
            this.blueLabel.Size = new System.Drawing.Size(28, 13);
            this.blueLabel.TabIndex = 15;
            this.blueLabel.Text = "Blue";
            // 
            // newColorToolTip
            // 
            this.newColorToolTip.AutoPopDelay = 5000;
            this.newColorToolTip.InitialDelay = 1;
            this.newColorToolTip.ReshowDelay = 1;
            // 
            // oldColorToolTip
            // 
            this.oldColorToolTip.AutoPopDelay = 5000;
            this.oldColorToolTip.InitialDelay = 1;
            this.oldColorToolTip.ReshowDelay = 1;
            // 
            // ColorPicker
            // 
            this.Controls.Add(this.blueLabel);
            this.Controls.Add(this.greenLabel);
            this.Controls.Add(this.redLabel);
            this.Controls.Add(this.blueNumericUpDown);
            this.Controls.Add(this.greenNumericUpDown);
            this.Controls.Add(this.redNumericUpDown);
            this.Controls.Add(this.shadesPictureBox);
            this.Controls.Add(this.basicColorsPictureBox);
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(189, 152);
            ((System.ComponentModel.ISupportInitialize)(this.basicColorsPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shadesPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.ToolTip newColorToolTip;
        private System.Windows.Forms.ToolTip oldColorToolTip;

        #endregion

        private System.Windows.Forms.PictureBox basicColorsPictureBox;
        private System.Windows.Forms.PictureBox shadesPictureBox;
        private System.Windows.Forms.NumericUpDown redNumericUpDown;
        private System.Windows.Forms.NumericUpDown greenNumericUpDown;
        private System.Windows.Forms.NumericUpDown blueNumericUpDown;
        private System.Windows.Forms.Label redLabel;
        private System.Windows.Forms.Label greenLabel;
        private System.Windows.Forms.Label blueLabel;

    }
}
