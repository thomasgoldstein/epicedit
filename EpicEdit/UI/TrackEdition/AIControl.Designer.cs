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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label aiIndexLabel;
            System.Windows.Forms.Label shapeLabel;
            System.Windows.Forms.Label speedLabel;
            this.selectedAIElementGroupBox = new System.Windows.Forms.GroupBox();
            this.cloneButton = new System.Windows.Forms.Button();
            this.indexNumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.deleteButton = new System.Windows.Forms.Button();
            this.shapeComboBox = new System.Windows.Forms.ComboBox();
            this.speedNumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.deleteAllButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.addButton = new System.Windows.Forms.Button();
            this.probaEditorButton = new System.Windows.Forms.Button();
            this.warningLabel = new System.Windows.Forms.Label();
            this.itemProbaGroupBox = new System.Windows.Forms.GroupBox();
            this.setComboBox = new System.Windows.Forms.ComboBox();
            aiIndexLabel = new System.Windows.Forms.Label();
            shapeLabel = new System.Windows.Forms.Label();
            speedLabel = new System.Windows.Forms.Label();
            this.selectedAIElementGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedNumericUpDown)).BeginInit();
            this.itemProbaGroupBox.SuspendLayout();
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
            // shapeLabel
            // 
            shapeLabel.Location = new System.Drawing.Point(6, 101);
            shapeLabel.Name = "shapeLabel";
            shapeLabel.Size = new System.Drawing.Size(45, 23);
            shapeLabel.TabIndex = 4;
            shapeLabel.Text = "Shape";
            // 
            // speedLabel
            // 
            speedLabel.Location = new System.Drawing.Point(6, 65);
            speedLabel.Name = "speedLabel";
            speedLabel.Size = new System.Drawing.Size(45, 23);
            speedLabel.TabIndex = 2;
            speedLabel.Text = "Speed";
            // 
            // selectedAIElementGroupBox
            // 
            this.selectedAIElementGroupBox.Controls.Add(this.cloneButton);
            this.selectedAIElementGroupBox.Controls.Add(this.indexNumericUpDown);
            this.selectedAIElementGroupBox.Controls.Add(aiIndexLabel);
            this.selectedAIElementGroupBox.Controls.Add(this.deleteButton);
            this.selectedAIElementGroupBox.Controls.Add(this.shapeComboBox);
            this.selectedAIElementGroupBox.Controls.Add(shapeLabel);
            this.selectedAIElementGroupBox.Controls.Add(speedLabel);
            this.selectedAIElementGroupBox.Controls.Add(this.speedNumericUpDown);
            this.selectedAIElementGroupBox.Location = new System.Drawing.Point(2, 120);
            this.selectedAIElementGroupBox.Name = "selectedAIElementGroupBox";
            this.selectedAIElementGroupBox.Size = new System.Drawing.Size(124, 199);
            this.selectedAIElementGroupBox.TabIndex = 1;
            this.selectedAIElementGroupBox.TabStop = false;
            this.selectedAIElementGroupBox.Text = "Selected Element";
            // 
            // cloneButton
            // 
            this.cloneButton.Image = global::EpicEdit.Properties.Resources.CopyButton;
            this.cloneButton.Location = new System.Drawing.Point(60, 168);
            this.cloneButton.Name = "cloneButton";
            this.cloneButton.Size = new System.Drawing.Size(24, 24);
            this.cloneButton.TabIndex = 6;
            this.buttonToolTip.SetToolTip(this.cloneButton, "Clone element");
            this.cloneButton.UseVisualStyleBackColor = true;
            this.cloneButton.Click += new System.EventHandler(this.CloneButtonClick);
            // 
            // indexNumericUpDown
            // 
            this.indexNumericUpDown.Location = new System.Drawing.Point(69, 30);
            this.indexNumericUpDown.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.indexNumericUpDown.Name = "indexNumericUpDown";
            this.indexNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.indexNumericUpDown.TabIndex = 1;
            this.indexNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.indexNumericUpDown.ValueChanged += new System.EventHandler(this.IndexNumericUpDownValueChanged);
            // 
            // deleteButton
            // 
            this.deleteButton.Image = global::EpicEdit.Properties.Resources.DeleteButton;
            this.deleteButton.Location = new System.Drawing.Point(90, 168);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(24, 24);
            this.deleteButton.TabIndex = 7;
            this.buttonToolTip.SetToolTip(this.deleteButton, "Delete element");
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // shapeComboBox
            // 
            this.shapeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.shapeComboBox.FormattingEnabled = true;
            this.shapeComboBox.Location = new System.Drawing.Point(10, 127);
            this.shapeComboBox.Name = "shapeComboBox";
            this.shapeComboBox.Size = new System.Drawing.Size(104, 21);
            this.shapeComboBox.TabIndex = 5;
            this.shapeComboBox.SelectedIndexChanged += new System.EventHandler(this.ShapeComboBoxSelectedIndexChanged);
            this.shapeComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.ShapeComboBoxFormat);
            // 
            // speedNumericUpDown
            // 
            this.speedNumericUpDown.Location = new System.Drawing.Point(69, 63);
            this.speedNumericUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.speedNumericUpDown.Name = "speedNumericUpDown";
            this.speedNumericUpDown.Size = new System.Drawing.Size(45, 20);
            this.speedNumericUpDown.TabIndex = 3;
            this.speedNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.speedNumericUpDown.ValueChanged += new System.EventHandler(this.SpeedNumericUpDownValueChanged);
            // 
            // deleteAllButton
            // 
            this.deleteAllButton.Image = global::EpicEdit.Properties.Resources.NukeButton;
            this.deleteAllButton.Location = new System.Drawing.Point(92, 325);
            this.deleteAllButton.Name = "deleteAllButton";
            this.deleteAllButton.Size = new System.Drawing.Size(24, 24);
            this.deleteAllButton.TabIndex = 3;
            this.buttonToolTip.SetToolTip(this.deleteAllButton, "Delete all");
            this.deleteAllButton.UseVisualStyleBackColor = true;
            this.deleteAllButton.Click += new System.EventHandler(this.DeleteAllButtonClick);
            // 
            // addButton
            // 
            this.addButton.Image = global::EpicEdit.Properties.Resources.AddButton;
            this.addButton.Location = new System.Drawing.Point(63, 325);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(24, 24);
            this.addButton.TabIndex = 2;
            this.buttonToolTip.SetToolTip(this.addButton, "Add new element");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButtonClick);
            // 
            // probaEditorButton
            // 
            this.probaEditorButton.Image = global::EpicEdit.Properties.Resources.ItemProbaButton;
            this.probaEditorButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.probaEditorButton.Location = new System.Drawing.Point(29, 68);
            this.probaEditorButton.Name = "probaEditorButton";
            this.probaEditorButton.Size = new System.Drawing.Size(66, 24);
            this.probaEditorButton.TabIndex = 1;
            this.probaEditorButton.Text = "View";
            this.probaEditorButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.probaEditorButton.UseVisualStyleBackColor = true;
            this.probaEditorButton.Click += new System.EventHandler(this.ProbaEditorButtonClick);
            // 
            // warningLabel
            // 
            this.warningLabel.Location = new System.Drawing.Point(4, 358);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(122, 44);
            this.warningLabel.TabIndex = 4;
            this.warningLabel.Text = "Warning: a track needs at least one element in order to work.";
            // 
            // itemProbaGroupBox
            // 
            this.itemProbaGroupBox.Controls.Add(this.probaEditorButton);
            this.itemProbaGroupBox.Controls.Add(this.setComboBox);
            this.itemProbaGroupBox.Location = new System.Drawing.Point(2, 4);
            this.itemProbaGroupBox.Name = "itemProbaGroupBox";
            this.itemProbaGroupBox.Size = new System.Drawing.Size(124, 110);
            this.itemProbaGroupBox.TabIndex = 0;
            this.itemProbaGroupBox.TabStop = false;
            this.itemProbaGroupBox.Text = "Item probabilities";
            // 
            // setComboBox
            // 
            this.setComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.setComboBox.FormattingEnabled = true;
            this.setComboBox.Location = new System.Drawing.Point(10, 30);
            this.setComboBox.Name = "setComboBox";
            this.setComboBox.Size = new System.Drawing.Size(104, 21);
            this.setComboBox.TabIndex = 0;
            this.setComboBox.SelectedIndexChanged += new System.EventHandler(this.SetComboBoxSelectedIndexChanged);
            // 
            // AIControl
            // 
            this.Controls.Add(this.itemProbaGroupBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.warningLabel);
            this.Controls.Add(this.deleteAllButton);
            this.Controls.Add(this.selectedAIElementGroupBox);
            this.Name = "AIControl";
            this.Size = new System.Drawing.Size(130, 410);
            this.selectedAIElementGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.indexNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.speedNumericUpDown)).EndInit();
            this.itemProbaGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Button cloneButton;
        private System.Windows.Forms.Button probaEditorButton;
        private System.Windows.Forms.ComboBox setComboBox;
        private System.Windows.Forms.GroupBox itemProbaGroupBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button deleteAllButton;
        private EpicEdit.UI.Tools.EpicNumericUpDown indexNumericUpDown;
        private System.Windows.Forms.ComboBox shapeComboBox;
        private System.Windows.Forms.Button deleteButton;
        private EpicEdit.UI.Tools.EpicNumericUpDown speedNumericUpDown;
        private System.Windows.Forms.GroupBox selectedAIElementGroupBox;
    }
}
