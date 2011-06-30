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
    partial class ObjectsControl
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
            this.rearObjectZonesControl = new EpicEdit.UI.TrackEdition.ObjectZonesControl();
            this.frontObjectZonesControl = new EpicEdit.UI.TrackEdition.ObjectZonesControl();
            this.rearZonesRadioButton = new System.Windows.Forms.RadioButton();
            this.frontZonesRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // rearObjectZonesControl
            // 
            this.rearObjectZonesControl.FrontViewZones = false;
            this.rearObjectZonesControl.Location = new System.Drawing.Point(3, 75);
            this.rearObjectZonesControl.Name = "rearObjectZonesControl";
            this.rearObjectZonesControl.Size = new System.Drawing.Size(124, 149);
            this.rearObjectZonesControl.TabIndex = 7;
            this.rearObjectZonesControl.Title = "Rear-view Zones";
            this.rearObjectZonesControl.Visible = false;
            this.rearObjectZonesControl.ValueChanged += new System.EventHandler<System.EventArgs>(this.RearObjectZonesControlValueChanged);
            // 
            // frontObjectZonesControl
            // 
            this.frontObjectZonesControl.FrontViewZones = true;
            this.frontObjectZonesControl.Location = new System.Drawing.Point(3, 75);
            this.frontObjectZonesControl.Name = "frontObjectZonesControl";
            this.frontObjectZonesControl.Size = new System.Drawing.Size(124, 149);
            this.frontObjectZonesControl.TabIndex = 6;
            this.frontObjectZonesControl.Title = "Front-view Zones";
            this.frontObjectZonesControl.ValueChanged += new System.EventHandler<System.EventArgs>(this.FrontObjectZonesControlValueChanged);
            // 
            // rearZonesRadioButton
            // 
            this.rearZonesRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.rearZonesRadioButton.Location = new System.Drawing.Point(12, 39);
            this.rearZonesRadioButton.Name = "rearZonesRadioButton";
            this.rearZonesRadioButton.Size = new System.Drawing.Size(104, 24);
            this.rearZonesRadioButton.TabIndex = 5;
            this.rearZonesRadioButton.TabStop = true;
            this.rearZonesRadioButton.Text = "Rear-view Zones";
            this.rearZonesRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rearZonesRadioButton.UseVisualStyleBackColor = true;
            // 
            // frontZonesRadioButton
            // 
            this.frontZonesRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.frontZonesRadioButton.Checked = true;
            this.frontZonesRadioButton.Location = new System.Drawing.Point(12, 9);
            this.frontZonesRadioButton.Name = "frontZonesRadioButton";
            this.frontZonesRadioButton.Size = new System.Drawing.Size(104, 24);
            this.frontZonesRadioButton.TabIndex = 4;
            this.frontZonesRadioButton.TabStop = true;
            this.frontZonesRadioButton.Text = "Front-view Zones";
            this.frontZonesRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.frontZonesRadioButton.UseVisualStyleBackColor = true;
            this.frontZonesRadioButton.CheckedChanged += new System.EventHandler(this.FrontZonesRadioButtonCheckedChanged);
            // 
            // ObjectsControl
            // 
            this.Controls.Add(this.rearObjectZonesControl);
            this.Controls.Add(this.frontObjectZonesControl);
            this.Controls.Add(this.rearZonesRadioButton);
            this.Controls.Add(this.frontZonesRadioButton);
            this.Name = "ObjectsControl";
            this.Size = new System.Drawing.Size(130, 400);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.RadioButton frontZonesRadioButton;
        private System.Windows.Forms.RadioButton rearZonesRadioButton;
        private EpicEdit.UI.TrackEdition.ObjectZonesControl frontObjectZonesControl;
        private EpicEdit.UI.TrackEdition.ObjectZonesControl rearObjectZonesControl;
    }
}
