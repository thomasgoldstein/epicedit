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
            System.Windows.Forms.Label redLabel;
            System.Windows.Forms.Label greenLabel;
            System.Windows.Forms.Label blueLabel;
            System.Windows.Forms.Label bitsLabel;
            this.basicColorsPictureBox = new System.Windows.Forms.PictureBox();
            this.shadesPictureBox = new System.Windows.Forms.PictureBox();
            this.red5NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.green5NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.blue5NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.newColorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.oldColorToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.red8NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.green8NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.blue8NumericUpDown = new System.Windows.Forms.NumericUpDown();
            redLabel = new System.Windows.Forms.Label();
            greenLabel = new System.Windows.Forms.Label();
            blueLabel = new System.Windows.Forms.Label();
            bitsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.basicColorsPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shadesPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.red5NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.green5NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blue5NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.red8NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.green8NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blue8NumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // redLabel
            // 
            redLabel.AutoSize = true;
            redLabel.Location = new System.Drawing.Point(133, 22);
            redLabel.Name = "redLabel";
            redLabel.Size = new System.Drawing.Size(27, 13);
            redLabel.TabIndex = 0;
            redLabel.Text = "Red";
            // 
            // greenLabel
            // 
            greenLabel.AutoSize = true;
            greenLabel.Location = new System.Drawing.Point(133, 69);
            greenLabel.Name = "greenLabel";
            greenLabel.Size = new System.Drawing.Size(36, 13);
            greenLabel.TabIndex = 3;
            greenLabel.Text = "Green";
            // 
            // blueLabel
            // 
            blueLabel.AutoSize = true;
            blueLabel.Location = new System.Drawing.Point(133, 116);
            blueLabel.Name = "blueLabel";
            blueLabel.Size = new System.Drawing.Size(28, 13);
            blueLabel.TabIndex = 5;
            blueLabel.Text = "Blue";
            // 
            // bitsLabel
            // 
            bitsLabel.AutoSize = true;
            bitsLabel.Location = new System.Drawing.Point(166, 22);
            bitsLabel.Name = "bitsLabel";
            bitsLabel.Size = new System.Drawing.Size(44, 13);
            bitsLabel.TabIndex = 1;
            bitsLabel.Text = "5 / 8-bit";
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
            this.basicColorsPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BasicColorsPictureBoxMouseDown);
            this.basicColorsPictureBox.MouseLeave += new System.EventHandler(this.BasicColorsPictureBoxMouseLeave);
            this.basicColorsPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BasicColorsPictureBoxMouseMove);
            this.basicColorsPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BasicColorsPictureBoxMouseUp);
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
            this.shadesPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ShadesPictureBoxMouseDown);
            this.shadesPictureBox.MouseLeave += new System.EventHandler(this.ShadesPictureBoxMouseLeave);
            this.shadesPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ShadesPictureBoxMouseMove);
            this.shadesPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ShadesPictureBoxMouseUp);
            // 
            // red5NumericUpDown
            // 
            this.red5NumericUpDown.Location = new System.Drawing.Point(136, 38);
            this.red5NumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.red5NumericUpDown.Name = "red5NumericUpDown";
            this.red5NumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.red5NumericUpDown.TabIndex = 2;
            this.red5NumericUpDown.ValueChanged += new System.EventHandler(this.Color5BitNumericUpDownValueChanged);
            this.red5NumericUpDown.Enter += new System.EventHandler(this.NumericUpDownEnter);
            // 
            // green5NumericUpDown
            // 
            this.green5NumericUpDown.Location = new System.Drawing.Point(136, 85);
            this.green5NumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.green5NumericUpDown.Name = "green5NumericUpDown";
            this.green5NumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.green5NumericUpDown.TabIndex = 4;
            this.green5NumericUpDown.ValueChanged += new System.EventHandler(this.Color5BitNumericUpDownValueChanged);
            this.green5NumericUpDown.Enter += new System.EventHandler(this.NumericUpDownEnter);
            // 
            // blue5NumericUpDown
            // 
            this.blue5NumericUpDown.Location = new System.Drawing.Point(136, 132);
            this.blue5NumericUpDown.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.blue5NumericUpDown.Name = "blue5NumericUpDown";
            this.blue5NumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.blue5NumericUpDown.TabIndex = 6;
            this.blue5NumericUpDown.ValueChanged += new System.EventHandler(this.Color5BitNumericUpDownValueChanged);
            this.blue5NumericUpDown.Enter += new System.EventHandler(this.NumericUpDownEnter);
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
            // red8NumericUpDown
            // 
            this.red8NumericUpDown.Location = new System.Drawing.Point(186, 38);
            this.red8NumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.red8NumericUpDown.Name = "red8NumericUpDown";
            this.red8NumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.red8NumericUpDown.TabIndex = 7;
            this.red8NumericUpDown.ValueChanged += new System.EventHandler(this.Color8BitNumericUpDownValueChanged);
            this.red8NumericUpDown.Enter += new System.EventHandler(this.NumericUpDownEnter);
            // 
            // green8NumericUpDown
            // 
            this.green8NumericUpDown.Location = new System.Drawing.Point(186, 85);
            this.green8NumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.green8NumericUpDown.Name = "green8NumericUpDown";
            this.green8NumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.green8NumericUpDown.TabIndex = 8;
            this.green8NumericUpDown.ValueChanged += new System.EventHandler(this.Color8BitNumericUpDownValueChanged);
            this.green8NumericUpDown.Enter += new System.EventHandler(this.NumericUpDownEnter);
            // 
            // blue8NumericUpDown
            // 
            this.blue8NumericUpDown.Location = new System.Drawing.Point(186, 132);
            this.blue8NumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.blue8NumericUpDown.Name = "blue8NumericUpDown";
            this.blue8NumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.blue8NumericUpDown.TabIndex = 9;
            this.blue8NumericUpDown.ValueChanged += new System.EventHandler(this.Color8BitNumericUpDownValueChanged);
            this.blue8NumericUpDown.Enter += new System.EventHandler(this.NumericUpDownEnter);
            // 
            // ColorPicker
            // 
            this.Controls.Add(bitsLabel);
            this.Controls.Add(this.blue8NumericUpDown);
            this.Controls.Add(this.green8NumericUpDown);
            this.Controls.Add(this.red8NumericUpDown);
            this.Controls.Add(blueLabel);
            this.Controls.Add(greenLabel);
            this.Controls.Add(redLabel);
            this.Controls.Add(this.blue5NumericUpDown);
            this.Controls.Add(this.green5NumericUpDown);
            this.Controls.Add(this.red5NumericUpDown);
            this.Controls.Add(this.shadesPictureBox);
            this.Controls.Add(this.basicColorsPictureBox);
            this.Name = "ColorPicker";
            this.Size = new System.Drawing.Size(230, 152);
            ((System.ComponentModel.ISupportInitialize)(this.basicColorsPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shadesPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.red5NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.green5NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blue5NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.red8NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.green8NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blue8NumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.NumericUpDown blue8NumericUpDown;
        private System.Windows.Forms.NumericUpDown green8NumericUpDown;
        private System.Windows.Forms.NumericUpDown red8NumericUpDown;
        private System.Windows.Forms.ToolTip newColorToolTip;
        private System.Windows.Forms.ToolTip oldColorToolTip;

        #endregion

        private System.Windows.Forms.PictureBox basicColorsPictureBox;
        private System.Windows.Forms.PictureBox shadesPictureBox;
        private System.Windows.Forms.NumericUpDown red5NumericUpDown;
        private System.Windows.Forms.NumericUpDown green5NumericUpDown;
        private System.Windows.Forms.NumericUpDown blue5NumericUpDown;

    }
}
