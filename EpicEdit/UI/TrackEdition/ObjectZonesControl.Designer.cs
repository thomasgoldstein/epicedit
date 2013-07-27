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
    partial class ObjectZonesControl
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
            this.zoneGroupBox = new System.Windows.Forms.GroupBox();
            this.zone4Label = new System.Windows.Forms.Label();
            this.zone3Label = new System.Windows.Forms.Label();
            this.zone2Label = new System.Windows.Forms.Label();
            this.zone1Label = new System.Windows.Forms.Label();
            this.zone4TrackBar = new System.Windows.Forms.TrackBar();
            this.zone3TrackBar = new System.Windows.Forms.TrackBar();
            this.zone2TrackBar = new System.Windows.Forms.TrackBar();
            this.zone1TrackBar = new System.Windows.Forms.TrackBar();
            this.zoneGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zone4TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zone3TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zone2TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zone1TrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // zoneGroupBox
            // 
            this.zoneGroupBox.Controls.Add(this.zone4Label);
            this.zoneGroupBox.Controls.Add(this.zone3Label);
            this.zoneGroupBox.Controls.Add(this.zone2Label);
            this.zoneGroupBox.Controls.Add(this.zone1Label);
            this.zoneGroupBox.Controls.Add(this.zone4TrackBar);
            this.zoneGroupBox.Controls.Add(this.zone3TrackBar);
            this.zoneGroupBox.Controls.Add(this.zone2TrackBar);
            this.zoneGroupBox.Controls.Add(this.zone1TrackBar);
            this.zoneGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoneGroupBox.Location = new System.Drawing.Point(0, 0);
            this.zoneGroupBox.Name = "zoneGroupBox";
            this.zoneGroupBox.Size = new System.Drawing.Size(150, 150);
            this.zoneGroupBox.TabIndex = 0;
            this.zoneGroupBox.TabStop = false;
            // 
            // zone4Label
            // 
            this.zone4Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(150)))), ((int)(((byte)(24)))));
            this.zone4Label.Location = new System.Drawing.Point(6, 109);
            this.zone4Label.Margin = new System.Windows.Forms.Padding(0);
            this.zone4Label.Name = "zone4Label";
            this.zone4Label.Size = new System.Drawing.Size(40, 25);
            this.zone4Label.TabIndex = 7;
            this.zone4Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // zone3Label
            // 
            this.zone3Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(186)))), ((int)(((byte)(64)))));
            this.zone3Label.Location = new System.Drawing.Point(6, 81);
            this.zone3Label.Margin = new System.Windows.Forms.Padding(0);
            this.zone3Label.Name = "zone3Label";
            this.zone3Label.Size = new System.Drawing.Size(40, 25);
            this.zone3Label.TabIndex = 6;
            this.zone3Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // zone2Label
            // 
            this.zone2Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(94)))), ((int)(((byte)(177)))));
            this.zone2Label.Location = new System.Drawing.Point(6, 50);
            this.zone2Label.Margin = new System.Windows.Forms.Padding(0);
            this.zone2Label.Name = "zone2Label";
            this.zone2Label.Size = new System.Drawing.Size(40, 25);
            this.zone2Label.TabIndex = 5;
            this.zone2Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // zone1Label
            // 
            this.zone1Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.zone1Label.Location = new System.Drawing.Point(6, 19);
            this.zone1Label.Margin = new System.Windows.Forms.Padding(0);
            this.zone1Label.Name = "zone1Label";
            this.zone1Label.Size = new System.Drawing.Size(40, 25);
            this.zone1Label.TabIndex = 4;
            this.zone1Label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // zone4TrackBar
            // 
            this.zone4TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.zone4TrackBar.AutoSize = false;
            this.zone4TrackBar.LargeChange = 1;
            this.zone4TrackBar.Location = new System.Drawing.Point(44, 112);
            this.zone4TrackBar.Maximum = 128;
            this.zone4TrackBar.Name = "zone4TrackBar";
            this.zone4TrackBar.Size = new System.Drawing.Size(100, 25);
            this.zone4TrackBar.TabIndex = 3;
            this.zone4TrackBar.Tag = "3";
            this.zone4TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zone4TrackBar.Scroll += new System.EventHandler(this.ZoneTrackBarScroll);
            // 
            // zone3TrackBar
            // 
            this.zone3TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.zone3TrackBar.AutoSize = false;
            this.zone3TrackBar.LargeChange = 1;
            this.zone3TrackBar.Location = new System.Drawing.Point(44, 81);
            this.zone3TrackBar.Maximum = 128;
            this.zone3TrackBar.Name = "zone3TrackBar";
            this.zone3TrackBar.Size = new System.Drawing.Size(100, 25);
            this.zone3TrackBar.TabIndex = 2;
            this.zone3TrackBar.Tag = "2";
            this.zone3TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zone3TrackBar.Scroll += new System.EventHandler(this.ZoneTrackBarScroll);
            // 
            // zone2TrackBar
            // 
            this.zone2TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.zone2TrackBar.AutoSize = false;
            this.zone2TrackBar.LargeChange = 1;
            this.zone2TrackBar.Location = new System.Drawing.Point(44, 50);
            this.zone2TrackBar.Maximum = 128;
            this.zone2TrackBar.Name = "zone2TrackBar";
            this.zone2TrackBar.Size = new System.Drawing.Size(100, 25);
            this.zone2TrackBar.TabIndex = 1;
            this.zone2TrackBar.Tag = "1";
            this.zone2TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zone2TrackBar.Scroll += new System.EventHandler(this.ZoneTrackBarScroll);
            // 
            // zone1TrackBar
            // 
            this.zone1TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.zone1TrackBar.AutoSize = false;
            this.zone1TrackBar.LargeChange = 1;
            this.zone1TrackBar.Location = new System.Drawing.Point(44, 19);
            this.zone1TrackBar.Maximum = 128;
            this.zone1TrackBar.Name = "zone1TrackBar";
            this.zone1TrackBar.Size = new System.Drawing.Size(100, 25);
            this.zone1TrackBar.TabIndex = 0;
            this.zone1TrackBar.Tag = "0";
            this.zone1TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.zone1TrackBar.Scroll += new System.EventHandler(this.ZoneTrackBarScroll);
            // 
            // ObjectZonesControl
            // 
            this.Controls.Add(this.zoneGroupBox);
            this.Name = "ObjectZonesControl";
            this.zoneGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zone4TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zone3TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zone2TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zone1TrackBar)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Label zone2Label;
        private System.Windows.Forms.Label zone3Label;
        private System.Windows.Forms.Label zone4Label;
        private System.Windows.Forms.Label zone1Label;
        private System.Windows.Forms.GroupBox zoneGroupBox;
        private System.Windows.Forms.TrackBar zone3TrackBar;
        private System.Windows.Forms.TrackBar zone4TrackBar;
        private System.Windows.Forms.TrackBar zone2TrackBar;
        private System.Windows.Forms.TrackBar zone1TrackBar;
    }
}
