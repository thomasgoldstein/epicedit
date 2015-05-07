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
    partial class RoadTilesetImportExportForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RadioButton tileGraphicsRadioButton;
        private System.Windows.Forms.RadioButton tileGenresRadioButtons;
        private System.Windows.Forms.RadioButton tilePalettesRadioButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button importButton;
        private System.Windows.Forms.ToolTip buttonToolTip;
        
        /// <summary>
        /// Disposes resources used by the form.
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
            this.tileGraphicsRadioButton = new System.Windows.Forms.RadioButton();
            this.tileGenresRadioButtons = new System.Windows.Forms.RadioButton();
            this.tilePalettesRadioButton = new System.Windows.Forms.RadioButton();
            this.exportButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tileGraphicsRadioButton
            // 
            this.tileGraphicsRadioButton.Location = new System.Drawing.Point(12, 12);
            this.tileGraphicsRadioButton.Name = "tileGraphicsRadioButton";
            this.tileGraphicsRadioButton.Size = new System.Drawing.Size(150, 24);
            this.tileGraphicsRadioButton.TabIndex = 0;
            this.tileGraphicsRadioButton.Text = "Tile graphics";
            this.tileGraphicsRadioButton.UseVisualStyleBackColor = true;
            this.tileGraphicsRadioButton.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // tileTypesRadioButtons
            // 
            this.tileGenresRadioButtons.Location = new System.Drawing.Point(12, 42);
            this.tileGenresRadioButtons.Name = "tileTypesRadioButtons";
            this.tileGenresRadioButtons.Size = new System.Drawing.Size(150, 24);
            this.tileGenresRadioButtons.TabIndex = 1;
            this.tileGenresRadioButtons.Text = "Tile types";
            this.tileGenresRadioButtons.UseVisualStyleBackColor = true;
            this.tileGenresRadioButtons.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // tilePalettesRadioButton
            // 
            this.tilePalettesRadioButton.Location = new System.Drawing.Point(12, 72);
            this.tilePalettesRadioButton.Name = "tilePalettesRadioButton";
            this.tilePalettesRadioButton.Size = new System.Drawing.Size(150, 24);
            this.tilePalettesRadioButton.TabIndex = 2;
            this.tilePalettesRadioButton.Text = "Tile palette associations";
            this.tilePalettesRadioButton.UseVisualStyleBackColor = true;
            this.tilePalettesRadioButton.CheckedChanged += new System.EventHandler(this.RadioButtonCheckedChanged);
            // 
            // exportButton
            // 
            this.exportButton.Image = global::EpicEdit.Properties.Resources.ExportButton;
            this.exportButton.Location = new System.Drawing.Point(248, 72);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(24, 24);
            this.exportButton.TabIndex = 4;
            this.exportButton.UseVisualStyleBackColor = true;
            this.exportButton.Click += new System.EventHandler(this.ExportButtonClick);
            // 
            // importButton
            // 
            this.importButton.Image = global::EpicEdit.Properties.Resources.ImportButton;
            this.importButton.Location = new System.Drawing.Point(218, 72);
            this.importButton.Name = "importButton";
            this.importButton.Size = new System.Drawing.Size(24, 24);
            this.importButton.TabIndex = 3;
            this.importButton.UseVisualStyleBackColor = true;
            this.importButton.Click += new System.EventHandler(this.ImportButtonClick);
            // 
            // RoadTilesetImportExportForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 112);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.importButton);
            this.Controls.Add(this.tilePalettesRadioButton);
            this.Controls.Add(this.tileGenresRadioButtons);
            this.Controls.Add(this.tileGraphicsRadioButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::EpicEdit.Properties.Resources.EpicEditIcon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RoadTilesetImportExportForm";
            this.ShowInTaskbar = false;
            this.Text = "Road tileset import / export";
            this.ResumeLayout(false);

        }
    }
}
