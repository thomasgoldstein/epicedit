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
    partial class ObjectAreasControl
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
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.area4Label = new System.Windows.Forms.Label();
            this.area3Label = new System.Windows.Forms.Label();
            this.area2Label = new System.Windows.Forms.Label();
            this.area1Label = new System.Windows.Forms.Label();
            this.area4TrackBar = new System.Windows.Forms.TrackBar();
            this.area3TrackBar = new System.Windows.Forms.TrackBar();
            this.area2TrackBar = new System.Windows.Forms.TrackBar();
            this.area1TrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.area4TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.area3TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.area2TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.area1TrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.area4Label);
            this.groupBox.Controls.Add(this.area3Label);
            this.groupBox.Controls.Add(this.area2Label);
            this.groupBox.Controls.Add(this.area1Label);
            this.groupBox.Controls.Add(this.area4TrackBar);
            this.groupBox.Controls.Add(this.area3TrackBar);
            this.groupBox.Controls.Add(this.area2TrackBar);
            this.groupBox.Controls.Add(this.area1TrackBar);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(150, 150);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            // 
            // area4Label
            // 
            this.area4Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(150)))), ((int)(((byte)(24)))));
            this.area4Label.Location = new System.Drawing.Point(6, 109);
            this.area4Label.Margin = new System.Windows.Forms.Padding(0);
            this.area4Label.Name = "area4Label";
            this.area4Label.Size = new System.Drawing.Size(40, 25);
            this.area4Label.TabIndex = 6;
            this.area4Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // area3Label
            // 
            this.area3Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(186)))), ((int)(((byte)(64)))));
            this.area3Label.Location = new System.Drawing.Point(6, 81);
            this.area3Label.Margin = new System.Windows.Forms.Padding(0);
            this.area3Label.Name = "area3Label";
            this.area3Label.Size = new System.Drawing.Size(40, 25);
            this.area3Label.TabIndex = 4;
            this.area3Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // area2Label
            // 
            this.area2Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(94)))), ((int)(((byte)(177)))));
            this.area2Label.Location = new System.Drawing.Point(6, 50);
            this.area2Label.Margin = new System.Windows.Forms.Padding(0);
            this.area2Label.Name = "area2Label";
            this.area2Label.Size = new System.Drawing.Size(40, 25);
            this.area2Label.TabIndex = 2;
            this.area2Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // area1Label
            // 
            this.area1Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.area1Label.Location = new System.Drawing.Point(6, 19);
            this.area1Label.Margin = new System.Windows.Forms.Padding(0);
            this.area1Label.Name = "area1Label";
            this.area1Label.Size = new System.Drawing.Size(40, 25);
            this.area1Label.TabIndex = 0;
            this.area1Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // area4TrackBar
            // 
            this.area4TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.area4TrackBar.AutoSize = false;
            this.area4TrackBar.LargeChange = 1;
            this.area4TrackBar.Location = new System.Drawing.Point(44, 112);
            this.area4TrackBar.Maximum = 128;
            this.area4TrackBar.Name = "area4TrackBar";
            this.area4TrackBar.Size = new System.Drawing.Size(100, 25);
            this.area4TrackBar.TabIndex = 7;
            this.area4TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.area4TrackBar.ValueChanged += new System.EventHandler(this.Area4TrackBarValueChanged);
            // 
            // area3TrackBar
            // 
            this.area3TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.area3TrackBar.AutoSize = false;
            this.area3TrackBar.LargeChange = 1;
            this.area3TrackBar.Location = new System.Drawing.Point(44, 81);
            this.area3TrackBar.Maximum = 128;
            this.area3TrackBar.Name = "area3TrackBar";
            this.area3TrackBar.Size = new System.Drawing.Size(100, 25);
            this.area3TrackBar.TabIndex = 5;
            this.area3TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.area3TrackBar.ValueChanged += new System.EventHandler(this.Area3TrackBarValueChanged);
            // 
            // area2TrackBar
            // 
            this.area2TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.area2TrackBar.AutoSize = false;
            this.area2TrackBar.LargeChange = 1;
            this.area2TrackBar.Location = new System.Drawing.Point(44, 50);
            this.area2TrackBar.Maximum = 128;
            this.area2TrackBar.Name = "area2TrackBar";
            this.area2TrackBar.Size = new System.Drawing.Size(100, 25);
            this.area2TrackBar.TabIndex = 3;
            this.area2TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.area2TrackBar.ValueChanged += new System.EventHandler(this.Area2TrackBarValueChanged);
            // 
            // area1TrackBar
            // 
            this.area1TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.area1TrackBar.AutoSize = false;
            this.area1TrackBar.LargeChange = 1;
            this.area1TrackBar.Location = new System.Drawing.Point(44, 19);
            this.area1TrackBar.Maximum = 128;
            this.area1TrackBar.Name = "area1TrackBar";
            this.area1TrackBar.Size = new System.Drawing.Size(100, 25);
            this.area1TrackBar.TabIndex = 1;
            this.area1TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.area1TrackBar.ValueChanged += new System.EventHandler(this.Area1TrackBarValueChanged);
            // 
            // ObjectAreasControl
            // 
            this.Controls.Add(this.groupBox);
            this.Name = "ObjectAreasControl";
            this.groupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.area4TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.area3TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.area2TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.area1TrackBar)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Label area2Label;
        private System.Windows.Forms.Label area3Label;
        private System.Windows.Forms.Label area4Label;
        private System.Windows.Forms.Label area1Label;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.TrackBar area3TrackBar;
        private System.Windows.Forms.TrackBar area4TrackBar;
        private System.Windows.Forms.TrackBar area2TrackBar;
        private System.Windows.Forms.TrackBar area1TrackBar;
    }
}
