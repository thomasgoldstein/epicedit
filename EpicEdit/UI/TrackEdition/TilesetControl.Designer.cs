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
            System.Windows.Forms.Label paletteLabel;
            this.tilePaletteNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tileGenreComboBox = new System.Windows.Forms.ComboBox();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.tilesetPanel = new EpicEdit.UI.TrackEdition.TilesetControl.TilesetPanel();
            this.resetMapButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            selectedTileGroupBox = new System.Windows.Forms.GroupBox();
            paletteLabel = new System.Windows.Forms.Label();
            selectedTileGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tilePaletteNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // selectedTileGroupBox
            // 
            selectedTileGroupBox.Controls.Add(this.tilePaletteNumericUpDown);
            selectedTileGroupBox.Controls.Add(paletteLabel);
            selectedTileGroupBox.Controls.Add(this.tileGenreComboBox);
            selectedTileGroupBox.Location = new System.Drawing.Point(2, 30);
            selectedTileGroupBox.Name = "selectedTileGroupBox";
            selectedTileGroupBox.Size = new System.Drawing.Size(124, 76);
            selectedTileGroupBox.TabIndex = 8;
            selectedTileGroupBox.TabStop = false;
            selectedTileGroupBox.Text = "Selected tile";
            // 
            // tilePaletteNumericUpDown
            // 
            this.tilePaletteNumericUpDown.Location = new System.Drawing.Point(81, 46);
            this.tilePaletteNumericUpDown.Maximum = new decimal(new int[] {
                                    8,
                                    0,
                                    0,
                                    0});
            this.tilePaletteNumericUpDown.Minimum = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.tilePaletteNumericUpDown.Name = "tilePaletteNumericUpDown";
            this.tilePaletteNumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.tilePaletteNumericUpDown.TabIndex = 2;
            this.tilePaletteNumericUpDown.Value = new decimal(new int[] {
                                    1,
                                    0,
                                    0,
                                    0});
            this.tilePaletteNumericUpDown.ValueChanged += new System.EventHandler(this.TilePaletteNumericUpDownValueChanged);
            // 
            // paletteLabel
            // 
            paletteLabel.Location = new System.Drawing.Point(6, 48);
            paletteLabel.Name = "paletteLabel";
            paletteLabel.Size = new System.Drawing.Size(100, 23);
            paletteLabel.TabIndex = 1;
            paletteLabel.Text = "Color palette";
            // 
            // tileGenreComboBox
            // 
            this.tileGenreComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tileGenreComboBox.FormattingEnabled = true;
            this.tileGenreComboBox.Location = new System.Drawing.Point(6, 19);
            this.tileGenreComboBox.Name = "tileGenreComboBox";
            this.tileGenreComboBox.Size = new System.Drawing.Size(112, 21);
            this.tileGenreComboBox.TabIndex = 0;
            this.tileGenreComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.TileGenreComboBoxFormat);
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
            this.tilesetPanel.Location = new System.Drawing.Point(0, 112);
            this.tilesetPanel.Name = "tilesetPanel";
            this.tilesetPanel.Size = new System.Drawing.Size(128, 512);
            this.tilesetPanel.TabIndex = 5;
            this.tilesetPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilesetPanelMouseDown);
            // 
            // resetMapButton
            // 
            this.resetMapButton.Image = global::EpicEdit.Properties.Resources.NukeButton;
            this.resetMapButton.Location = new System.Drawing.Point(103, 631);
            this.resetMapButton.Name = "resetMapButton";
            this.resetMapButton.Size = new System.Drawing.Size(24, 24);
            this.resetMapButton.TabIndex = 7;
            this.buttonToolTip.SetToolTip(this.resetMapButton, "Reset map");
            this.resetMapButton.UseVisualStyleBackColor = true;
            this.resetMapButton.Click += new System.EventHandler(this.ResetMapButtonClick);
            // 
            // TilesetControl
            // 
            this.Controls.Add(selectedTileGroupBox);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(this.resetMapButton);
            this.Controls.Add(this.tilesetPanel);
            this.Name = "TilesetControl";
            this.Size = new System.Drawing.Size(130, 660);
            selectedTileGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tilePaletteNumericUpDown)).EndInit();
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.NumericUpDown tilePaletteNumericUpDown;
        private System.Windows.Forms.ComboBox tileGenreComboBox;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button resetMapButton;
        private EpicEdit.UI.TrackEdition.TilesetControl.TilesetPanel tilesetPanel;
        private System.Windows.Forms.ComboBox themeComboBox;
    }
}