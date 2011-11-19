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
    partial class TilesetControl
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
                if (this.components != null)
                {
                    this.components.Dispose();
                }

                if (this.tilesetDrawer != null)
                {
                    this.tilesetDrawer.Dispose();
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
            System.Windows.Forms.GroupBox selectedTileGroupBox;
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.tilesetPanel = new EpicEdit.UI.TrackEdition.TilesetControl.TilesetPanel();
            this.resetMapButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tileGenreComboBox = new System.Windows.Forms.ComboBox();
            selectedTileGroupBox = new System.Windows.Forms.GroupBox();
            selectedTileGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(3, 3);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(121, 21);
            this.themeComboBox.TabIndex = 6;
            // 
            // tilesetPanel
            // 
            this.tilesetPanel.BackColor = System.Drawing.Color.Black;
            this.tilesetPanel.Location = new System.Drawing.Point(0, 90);
            this.tilesetPanel.Name = "tilesetPanel";
            this.tilesetPanel.Size = new System.Drawing.Size(128, 512);
            this.tilesetPanel.TabIndex = 5;
            this.tilesetPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilesetPanelMouseDown);
            // 
            // resetMapButton
            // 
            this.resetMapButton.Image = global::EpicEdit.Properties.Resources.NukeButton;
            this.resetMapButton.Location = new System.Drawing.Point(103, 609);
            this.resetMapButton.Name = "resetMapButton";
            this.resetMapButton.Size = new System.Drawing.Size(24, 24);
            this.resetMapButton.TabIndex = 7;
            this.buttonToolTip.SetToolTip(this.resetMapButton, "Reset map");
            this.resetMapButton.UseVisualStyleBackColor = true;
            this.resetMapButton.Click += new System.EventHandler(this.ResetMapButtonClick);
            // 
            // tileGenreComboBox
            // 
            this.tileGenreComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tileGenreComboBox.FormattingEnabled = true;
            this.tileGenreComboBox.Location = new System.Drawing.Point(6, 19);
            this.tileGenreComboBox.Name = "tileGenreComboBox";
            this.tileGenreComboBox.Size = new System.Drawing.Size(116, 21);
            this.tileGenreComboBox.TabIndex = 0;
            this.tileGenreComboBox.SelectedIndexChanged += new System.EventHandler(this.TileGenreComboBoxSelectedIndexChanged);
            this.tileGenreComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.TileGenreComboBoxFormat);
            // 
            // selectedTileGroupBox
            // 
            selectedTileGroupBox.Controls.Add(this.tileGenreComboBox);
            selectedTileGroupBox.Location = new System.Drawing.Point(0, 30);
            selectedTileGroupBox.Name = "selectedTileGroupBox";
            selectedTileGroupBox.Size = new System.Drawing.Size(128, 50);
            selectedTileGroupBox.TabIndex = 8;
            selectedTileGroupBox.TabStop = false;
            selectedTileGroupBox.Text = "Selected tile";
            // 
            // TilesetControl
            // 
            this.Controls.Add(selectedTileGroupBox);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.resetMapButton);
            this.Controls.Add(this.tilesetPanel);
            this.Name = "TilesetControl";
            this.Size = new System.Drawing.Size(130, 640);
            selectedTileGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.ComboBox tileGenreComboBox;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button resetMapButton;
        private EpicEdit.UI.TrackEdition.TilesetControl.TilesetPanel tilesetPanel;
        private System.Windows.Forms.ComboBox themeComboBox;
    }
}