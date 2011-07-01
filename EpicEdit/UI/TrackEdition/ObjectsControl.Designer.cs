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
            this.propertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.routineComboBox = new System.Windows.Forms.ComboBox();
            this.routineLabel = new System.Windows.Forms.Label();
            this.interactComboBox = new System.Windows.Forms.ComboBox();
            this.interactLabel = new System.Windows.Forms.Label();
            this.tilesetComboBox = new System.Windows.Forms.ComboBox();
            this.tilesetLabel = new System.Windows.Forms.Label();
            this.zoneGroupBox = new System.Windows.Forms.GroupBox();
            this.propertiesGroupBox.SuspendLayout();
            this.zoneGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // rearObjectZonesControl
            // 
            this.rearObjectZonesControl.FrontViewZones = false;
            this.rearObjectZonesControl.Location = new System.Drawing.Point(1, 86);
            this.rearObjectZonesControl.Name = "rearObjectZonesControl";
            this.rearObjectZonesControl.Size = new System.Drawing.Size(122, 149);
            this.rearObjectZonesControl.TabIndex = 7;
            this.rearObjectZonesControl.Title = "Rear-view Zones";
            this.rearObjectZonesControl.Visible = false;
            this.rearObjectZonesControl.ValueChanged += new System.EventHandler<System.EventArgs>(this.RearObjectZonesControlValueChanged);
            // 
            // frontObjectZonesControl
            // 
            this.frontObjectZonesControl.FrontViewZones = true;
            this.frontObjectZonesControl.Location = new System.Drawing.Point(1, 86);
            this.frontObjectZonesControl.Name = "frontObjectZonesControl";
            this.frontObjectZonesControl.Size = new System.Drawing.Size(122, 149);
            this.frontObjectZonesControl.TabIndex = 6;
            this.frontObjectZonesControl.Title = "Front-view Zones";
            this.frontObjectZonesControl.ValueChanged += new System.EventHandler<System.EventArgs>(this.FrontObjectZonesControlValueChanged);
            // 
            // rearZonesRadioButton
            // 
            this.rearZonesRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.rearZonesRadioButton.Location = new System.Drawing.Point(10, 50);
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
            this.frontZonesRadioButton.Location = new System.Drawing.Point(10, 20);
            this.frontZonesRadioButton.Name = "frontZonesRadioButton";
            this.frontZonesRadioButton.Size = new System.Drawing.Size(104, 24);
            this.frontZonesRadioButton.TabIndex = 4;
            this.frontZonesRadioButton.TabStop = true;
            this.frontZonesRadioButton.Text = "Front-view Zones";
            this.frontZonesRadioButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.frontZonesRadioButton.UseVisualStyleBackColor = true;
            this.frontZonesRadioButton.CheckedChanged += new System.EventHandler(this.FrontZonesRadioButtonCheckedChanged);
            // 
            // propertiesGroupBox
            // 
            this.propertiesGroupBox.Controls.Add(this.routineComboBox);
            this.propertiesGroupBox.Controls.Add(this.routineLabel);
            this.propertiesGroupBox.Controls.Add(this.interactComboBox);
            this.propertiesGroupBox.Controls.Add(this.interactLabel);
            this.propertiesGroupBox.Controls.Add(this.tilesetComboBox);
            this.propertiesGroupBox.Controls.Add(this.tilesetLabel);
            this.propertiesGroupBox.Location = new System.Drawing.Point(2, 4);
            this.propertiesGroupBox.Name = "propertiesGroupBox";
            this.propertiesGroupBox.Size = new System.Drawing.Size(124, 171);
            this.propertiesGroupBox.TabIndex = 8;
            this.propertiesGroupBox.TabStop = false;
            this.propertiesGroupBox.Text = "Object Properties";
            // 
            // routineComboBox
            // 
            this.routineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.routineComboBox.FormattingEnabled = true;
            this.routineComboBox.Location = new System.Drawing.Point(10, 131);
            this.routineComboBox.Name = "routineComboBox";
            this.routineComboBox.Size = new System.Drawing.Size(104, 21);
            this.routineComboBox.TabIndex = 5;
            // 
            // routineLabel
            // 
            this.routineLabel.Location = new System.Drawing.Point(6, 116);
            this.routineLabel.Name = "routineLabel";
            this.routineLabel.Size = new System.Drawing.Size(100, 23);
            this.routineLabel.TabIndex = 4;
            this.routineLabel.Text = "Routine";
            // 
            // interactComboBox
            // 
            this.interactComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.interactComboBox.FormattingEnabled = true;
            this.interactComboBox.Location = new System.Drawing.Point(10, 83);
            this.interactComboBox.Name = "interactComboBox";
            this.interactComboBox.Size = new System.Drawing.Size(104, 21);
            this.interactComboBox.TabIndex = 3;
            // 
            // interactLabel
            // 
            this.interactLabel.Location = new System.Drawing.Point(6, 66);
            this.interactLabel.Name = "interactLabel";
            this.interactLabel.Size = new System.Drawing.Size(100, 23);
            this.interactLabel.TabIndex = 2;
            this.interactLabel.Text = "Interaction";
            // 
            // tilesetComboBox
            // 
            this.tilesetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tilesetComboBox.FormattingEnabled = true;
            this.tilesetComboBox.Location = new System.Drawing.Point(10, 33);
            this.tilesetComboBox.Name = "tilesetComboBox";
            this.tilesetComboBox.Size = new System.Drawing.Size(104, 21);
            this.tilesetComboBox.TabIndex = 1;
            // 
            // tilesetLabel
            // 
            this.tilesetLabel.Location = new System.Drawing.Point(6, 16);
            this.tilesetLabel.Name = "tilesetLabel";
            this.tilesetLabel.Size = new System.Drawing.Size(100, 23);
            this.tilesetLabel.TabIndex = 0;
            this.tilesetLabel.Text = "Tileset";
            // 
            // zoneGroupBox
            // 
            this.zoneGroupBox.Controls.Add(this.frontZonesRadioButton);
            this.zoneGroupBox.Controls.Add(this.rearZonesRadioButton);
            this.zoneGroupBox.Controls.Add(this.rearObjectZonesControl);
            this.zoneGroupBox.Controls.Add(this.frontObjectZonesControl);
            this.zoneGroupBox.Location = new System.Drawing.Point(2, 181);
            this.zoneGroupBox.Name = "zoneGroupBox";
            this.zoneGroupBox.Size = new System.Drawing.Size(124, 235);
            this.zoneGroupBox.TabIndex = 9;
            this.zoneGroupBox.TabStop = false;
            this.zoneGroupBox.Text = "Object Visibility Zones";
            // 
            // ObjectsControl
            // 
            this.Controls.Add(this.zoneGroupBox);
            this.Controls.Add(this.propertiesGroupBox);
            this.Name = "ObjectsControl";
            this.Size = new System.Drawing.Size(130, 420);
            this.propertiesGroupBox.ResumeLayout(false);
            this.zoneGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.GroupBox zoneGroupBox;
        private System.Windows.Forms.ComboBox interactComboBox;
        private System.Windows.Forms.Label interactLabel;
        private System.Windows.Forms.ComboBox routineComboBox;
        private System.Windows.Forms.Label routineLabel;
        private System.Windows.Forms.ComboBox tilesetComboBox;
        private System.Windows.Forms.Label tilesetLabel;
        private System.Windows.Forms.GroupBox propertiesGroupBox;
        private System.Windows.Forms.RadioButton frontZonesRadioButton;
        private System.Windows.Forms.RadioButton rearZonesRadioButton;
        private EpicEdit.UI.TrackEdition.ObjectZonesControl frontObjectZonesControl;
        private EpicEdit.UI.TrackEdition.ObjectZonesControl rearObjectZonesControl;
    }
}
