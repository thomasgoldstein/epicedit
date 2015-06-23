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
            System.Windows.Forms.Label routineLabel;
            System.Windows.Forms.Label interactLabel;
            System.Windows.Forms.Label tilesetLabel;
            System.Windows.Forms.Label paletteLabel;
            this.palettesLabel = new System.Windows.Forms.Label();
            this.rearObjectZonesControl = new EpicEdit.UI.TrackEdition.ObjectZonesControl();
            this.frontObjectZonesControl = new EpicEdit.UI.TrackEdition.ObjectZonesControl();
            this.rearZonesRadioButton = new System.Windows.Forms.RadioButton();
            this.frontZonesRadioButton = new System.Windows.Forms.RadioButton();
            this.propertiesGroupBox = new System.Windows.Forms.GroupBox();
            this.palette4NumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.palette3NumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.palette2NumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.flashingCheckBox = new System.Windows.Forms.CheckBox();
            this.palette1NumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.routineComboBox = new System.Windows.Forms.ComboBox();
            this.interactComboBox = new System.Windows.Forms.ComboBox();
            this.tilesetComboBox = new System.Windows.Forms.ComboBox();
            this.zoneGroupBox = new System.Windows.Forms.GroupBox();
            routineLabel = new System.Windows.Forms.Label();
            interactLabel = new System.Windows.Forms.Label();
            tilesetLabel = new System.Windows.Forms.Label();
            paletteLabel = new System.Windows.Forms.Label();
            this.propertiesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.palette4NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette3NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette2NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette1NumericUpDown)).BeginInit();
            this.zoneGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // routineLabel
            // 
            routineLabel.Location = new System.Drawing.Point(6, 16);
            routineLabel.Name = "routineLabel";
            routineLabel.Size = new System.Drawing.Size(100, 23);
            routineLabel.TabIndex = 0;
            routineLabel.Text = "Routine";
            // 
            // interactLabel
            // 
            interactLabel.Location = new System.Drawing.Point(6, 66);
            interactLabel.Name = "interactLabel";
            interactLabel.Size = new System.Drawing.Size(100, 23);
            interactLabel.TabIndex = 2;
            interactLabel.Text = "Interaction";
            // 
            // tilesetLabel
            // 
            tilesetLabel.Location = new System.Drawing.Point(6, 116);
            tilesetLabel.Name = "tilesetLabel";
            tilesetLabel.Size = new System.Drawing.Size(100, 23);
            tilesetLabel.TabIndex = 4;
            tilesetLabel.Text = "Tileset";
            // 
            // paletteLabel
            // 
            paletteLabel.Location = new System.Drawing.Point(6, 168);
            paletteLabel.Name = "paletteLabel";
            paletteLabel.Size = new System.Drawing.Size(100, 23);
            paletteLabel.TabIndex = 6;
            paletteLabel.Text = "Color palette";
            // 
            // palettesLabel
            // 
            this.palettesLabel.Location = new System.Drawing.Point(10, 227);
            this.palettesLabel.Name = "palettesLabel";
            this.palettesLabel.Size = new System.Drawing.Size(61, 30);
            this.palettesLabel.TabIndex = 9;
            this.palettesLabel.Text = "Flashing palettes";
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
            this.rearObjectZonesControl.ValueChanged += new System.EventHandler<System.EventArgs>(this.ObjectZonesControlValueChanged);
            // 
            // frontObjectZonesControl
            // 
            this.frontObjectZonesControl.FrontViewZones = true;
            this.frontObjectZonesControl.Location = new System.Drawing.Point(1, 86);
            this.frontObjectZonesControl.Name = "frontObjectZonesControl";
            this.frontObjectZonesControl.Size = new System.Drawing.Size(122, 149);
            this.frontObjectZonesControl.TabIndex = 6;
            this.frontObjectZonesControl.Title = "Front-view Zones";
            this.frontObjectZonesControl.ValueChanged += new System.EventHandler<System.EventArgs>(this.ObjectZonesControlValueChanged);
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
            this.propertiesGroupBox.Controls.Add(this.palette4NumericUpDown);
            this.propertiesGroupBox.Controls.Add(this.palette3NumericUpDown);
            this.propertiesGroupBox.Controls.Add(this.palette2NumericUpDown);
            this.propertiesGroupBox.Controls.Add(this.palettesLabel);
            this.propertiesGroupBox.Controls.Add(this.flashingCheckBox);
            this.propertiesGroupBox.Controls.Add(this.palette1NumericUpDown);
            this.propertiesGroupBox.Controls.Add(paletteLabel);
            this.propertiesGroupBox.Controls.Add(this.routineComboBox);
            this.propertiesGroupBox.Controls.Add(routineLabel);
            this.propertiesGroupBox.Controls.Add(this.interactComboBox);
            this.propertiesGroupBox.Controls.Add(interactLabel);
            this.propertiesGroupBox.Controls.Add(this.tilesetComboBox);
            this.propertiesGroupBox.Controls.Add(tilesetLabel);
            this.propertiesGroupBox.Location = new System.Drawing.Point(2, 4);
            this.propertiesGroupBox.Name = "propertiesGroupBox";
            this.propertiesGroupBox.Size = new System.Drawing.Size(124, 293);
            this.propertiesGroupBox.TabIndex = 8;
            this.propertiesGroupBox.TabStop = false;
            this.propertiesGroupBox.Text = "Object Properties";
            // 
            // palette4NumericUpDown
            // 
            this.palette4NumericUpDown.Location = new System.Drawing.Point(77, 257);
            this.palette4NumericUpDown.Maximum = new decimal(new int[] {
                                    7,
                                    0,
                                    0,
                                    0});
            this.palette4NumericUpDown.Name = "palette4NumericUpDown";
            this.palette4NumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.palette4NumericUpDown.TabIndex = 12;
            // 
            // palette3NumericUpDown
            // 
            this.palette3NumericUpDown.Location = new System.Drawing.Point(77, 237);
            this.palette3NumericUpDown.Maximum = new decimal(new int[] {
                                    7,
                                    0,
                                    0,
                                    0});
            this.palette3NumericUpDown.Name = "palette3NumericUpDown";
            this.palette3NumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.palette3NumericUpDown.TabIndex = 11;
            // 
            // palette2NumericUpDown
            // 
            this.palette2NumericUpDown.Location = new System.Drawing.Point(77, 217);
            this.palette2NumericUpDown.Maximum = new decimal(new int[] {
                                    7,
                                    0,
                                    0,
                                    0});
            this.palette2NumericUpDown.Name = "palette2NumericUpDown";
            this.palette2NumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.palette2NumericUpDown.TabIndex = 10;
            // 
            // flashingCheckBox
            // 
            this.flashingCheckBox.Location = new System.Drawing.Point(10, 192);
            this.flashingCheckBox.Name = "flashingCheckBox";
            this.flashingCheckBox.Size = new System.Drawing.Size(104, 24);
            this.flashingCheckBox.TabIndex = 8;
            this.flashingCheckBox.Text = "Enable flashing";
            this.flashingCheckBox.UseVisualStyleBackColor = true;
            // 
            // palette1NumericUpDown
            // 
            this.palette1NumericUpDown.Location = new System.Drawing.Point(77, 166);
            this.palette1NumericUpDown.Maximum = new decimal(new int[] {
                                    7,
                                    0,
                                    0,
                                    0});
            this.palette1NumericUpDown.Name = "palette1NumericUpDown";
            this.palette1NumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.palette1NumericUpDown.TabIndex = 7;
            // 
            // routineComboBox
            // 
            this.routineComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.routineComboBox.FormattingEnabled = true;
            this.routineComboBox.Location = new System.Drawing.Point(10, 33);
            this.routineComboBox.Name = "routineComboBox";
            this.routineComboBox.Size = new System.Drawing.Size(104, 21);
            this.routineComboBox.TabIndex = 1;
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
            // tilesetComboBox
            // 
            this.tilesetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tilesetComboBox.FormattingEnabled = true;
            this.tilesetComboBox.Location = new System.Drawing.Point(10, 133);
            this.tilesetComboBox.Name = "tilesetComboBox";
            this.tilesetComboBox.Size = new System.Drawing.Size(104, 21);
            this.tilesetComboBox.TabIndex = 5;
            // 
            // zoneGroupBox
            // 
            this.zoneGroupBox.Controls.Add(this.frontZonesRadioButton);
            this.zoneGroupBox.Controls.Add(this.rearZonesRadioButton);
            this.zoneGroupBox.Controls.Add(this.rearObjectZonesControl);
            this.zoneGroupBox.Controls.Add(this.frontObjectZonesControl);
            this.zoneGroupBox.Location = new System.Drawing.Point(2, 303);
            this.zoneGroupBox.Name = "zoneGroupBox";
            this.zoneGroupBox.Size = new System.Drawing.Size(124, 235);
            this.zoneGroupBox.TabIndex = 9;
            this.zoneGroupBox.TabStop = false;
            this.zoneGroupBox.Text = "Object Visibility";
            // 
            // ObjectsControl
            // 
            this.Controls.Add(this.zoneGroupBox);
            this.Controls.Add(this.propertiesGroupBox);
            this.Name = "ObjectsControl";
            this.Size = new System.Drawing.Size(130, 550);
            this.propertiesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.palette4NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette3NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette2NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.palette1NumericUpDown)).EndInit();
            this.zoneGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.Label palettesLabel;
        private EpicEdit.UI.Tools.EpicNumericUpDown palette1NumericUpDown;
        private EpicEdit.UI.Tools.EpicNumericUpDown palette4NumericUpDown;
        private EpicEdit.UI.Tools.EpicNumericUpDown palette3NumericUpDown;
        private EpicEdit.UI.Tools.EpicNumericUpDown palette2NumericUpDown;
        private System.Windows.Forms.CheckBox flashingCheckBox;
        private System.Windows.Forms.GroupBox zoneGroupBox;
        private System.Windows.Forms.ComboBox interactComboBox;
        private System.Windows.Forms.ComboBox routineComboBox;
        private System.Windows.Forms.ComboBox tilesetComboBox;
        private System.Windows.Forms.GroupBox propertiesGroupBox;
        private System.Windows.Forms.RadioButton frontZonesRadioButton;
        private System.Windows.Forms.RadioButton rearZonesRadioButton;
        private EpicEdit.UI.TrackEdition.ObjectZonesControl frontObjectZonesControl;
        private EpicEdit.UI.TrackEdition.ObjectZonesControl rearObjectZonesControl;
    }
}
