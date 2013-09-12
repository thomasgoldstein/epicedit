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
    partial class BackgroundEditor
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

                this.drawer.Dispose();
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
            System.Windows.Forms.GroupBox previewGroupBox;
            System.Windows.Forms.GroupBox frontLayerGroupBox;
            System.Windows.Forms.GroupBox backLayerGroupBox;
            System.Windows.Forms.Label backTileLabel;
            System.Windows.Forms.Label frontTileLabel;
            System.Windows.Forms.Label paletteLabel;
            this.rewindButton = new System.Windows.Forms.Button();
            this.playPauseButton = new System.Windows.Forms.Button();
            this.backgroundPreviewer = new EpicEdit.UI.ThemeEdition.BackgroundPreviewer();
            this.frontLayerPanel = new EpicEdit.UI.ThemeEdition.BackgroundPanel();
            this.backLayerPanel = new EpicEdit.UI.ThemeEdition.BackgroundPanel();
            this.tilesetPanel = new EpicEdit.UI.ThemeEdition.BackgroundTilesetPanel();
            this.backTilePanel = new EpicEdit.UI.ThemeEdition.BackgroundTilePanel();
            this.paletteNumericUpDown = new EpicEdit.UI.Tools.EpicNumericUpDown();
            this.frontTilePanel = new EpicEdit.UI.ThemeEdition.BackgroundTilePanel();
            this.themeComboBox = new EpicEdit.UI.Tools.ThemeComboBox();
            this.flipXButton = new System.Windows.Forms.CheckBox();
            this.flipYButton = new System.Windows.Forms.CheckBox();
            this.importGraphicsButton = new System.Windows.Forms.Button();
            this.exportGraphicsButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.exportLayoutButton = new System.Windows.Forms.Button();
            this.importLayoutButton = new System.Windows.Forms.Button();
            this.graphicsGroupBox = new System.Windows.Forms.GroupBox();
            this.layoutGroupBox = new System.Windows.Forms.GroupBox();
            previewGroupBox = new System.Windows.Forms.GroupBox();
            frontLayerGroupBox = new System.Windows.Forms.GroupBox();
            backLayerGroupBox = new System.Windows.Forms.GroupBox();
            backTileLabel = new System.Windows.Forms.Label();
            frontTileLabel = new System.Windows.Forms.Label();
            paletteLabel = new System.Windows.Forms.Label();
            previewGroupBox.SuspendLayout();
            frontLayerGroupBox.SuspendLayout();
            backLayerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).BeginInit();
            this.graphicsGroupBox.SuspendLayout();
            this.layoutGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // previewGroupBox
            // 
            previewGroupBox.Controls.Add(this.rewindButton);
            previewGroupBox.Controls.Add(this.playPauseButton);
            previewGroupBox.Controls.Add(this.backgroundPreviewer);
            previewGroupBox.Location = new System.Drawing.Point(6, 238);
            previewGroupBox.Name = "previewGroupBox";
            previewGroupBox.Size = new System.Drawing.Size(526, 107);
            previewGroupBox.TabIndex = 5;
            previewGroupBox.TabStop = false;
            previewGroupBox.Text = "Preview";
            // 
            // rewindButton
            // 
            this.rewindButton.Location = new System.Drawing.Point(88, 19);
            this.rewindButton.Name = "rewindButton";
            this.rewindButton.Size = new System.Drawing.Size(75, 23);
            this.rewindButton.TabIndex = 7;
            this.rewindButton.Text = "Rewind";
            this.rewindButton.UseVisualStyleBackColor = true;
            this.rewindButton.Click += new System.EventHandler(this.RewindButtonClick);
            // 
            // playPauseButton
            // 
            this.playPauseButton.Location = new System.Drawing.Point(7, 19);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(75, 23);
            this.playPauseButton.TabIndex = 6;
            this.playPauseButton.Text = "Play";
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.PlayPauseButtonClick);
            // 
            // backgroundPreviewer
            // 
            this.backgroundPreviewer.Location = new System.Drawing.Point(7, 46);
            this.backgroundPreviewer.Name = "backgroundPreviewer";
            this.backgroundPreviewer.Size = new System.Drawing.Size(512, 48);
            this.backgroundPreviewer.TabIndex = 8;
            // 
            // frontLayerGroupBox
            // 
            frontLayerGroupBox.Controls.Add(this.frontLayerPanel);
            frontLayerGroupBox.Location = new System.Drawing.Point(6, 40);
            frontLayerGroupBox.Name = "frontLayerGroupBox";
            frontLayerGroupBox.Size = new System.Drawing.Size(526, 93);
            frontLayerGroupBox.TabIndex = 1;
            frontLayerGroupBox.TabStop = false;
            frontLayerGroupBox.Text = "Front layer";
            // 
            // frontLayerPanel
            // 
            this.frontLayerPanel.AutoScroll = true;
            this.frontLayerPanel.AutoScrollMinSize = new System.Drawing.Size(2048, 48);
            this.frontLayerPanel.Front = true;
            this.frontLayerPanel.Location = new System.Drawing.Point(7, 19);
            this.frontLayerPanel.Name = "frontLayerPanel";
            this.frontLayerPanel.Size = new System.Drawing.Size(512, 48);
            this.frontLayerPanel.TabIndex = 2;
            this.frontLayerPanel.TileChanged += new System.EventHandler<System.EventArgs>(this.BackgroundLayerPanelTileChanged);
            this.frontLayerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundLayerPanelMouseDown);
            // 
            // backLayerGroupBox
            // 
            backLayerGroupBox.Controls.Add(this.backLayerPanel);
            backLayerGroupBox.Location = new System.Drawing.Point(6, 139);
            backLayerGroupBox.Name = "backLayerGroupBox";
            backLayerGroupBox.Size = new System.Drawing.Size(526, 93);
            backLayerGroupBox.TabIndex = 3;
            backLayerGroupBox.TabStop = false;
            backLayerGroupBox.Text = "Back layer";
            // 
            // backLayerPanel
            // 
            this.backLayerPanel.AutoScroll = true;
            this.backLayerPanel.AutoScrollMinSize = new System.Drawing.Size(1024, 48);
            this.backLayerPanel.Front = false;
            this.backLayerPanel.Location = new System.Drawing.Point(7, 19);
            this.backLayerPanel.Name = "backLayerPanel";
            this.backLayerPanel.Size = new System.Drawing.Size(512, 48);
            this.backLayerPanel.TabIndex = 4;
            this.backLayerPanel.TileChanged += new System.EventHandler<System.EventArgs>(this.BackgroundLayerPanelTileChanged);
            this.backLayerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundLayerPanelMouseDown);
            // 
            // backTileLabel
            // 
            backTileLabel.Location = new System.Drawing.Point(575, 40);
            backTileLabel.Name = "backTileLabel";
            backTileLabel.Size = new System.Drawing.Size(32, 23);
            backTileLabel.TabIndex = 16;
            backTileLabel.Text = "Back";
            // 
            // frontTileLabel
            // 
            frontTileLabel.Location = new System.Drawing.Point(534, 40);
            frontTileLabel.Name = "frontTileLabel";
            frontTileLabel.Size = new System.Drawing.Size(32, 23);
            frontTileLabel.TabIndex = 14;
            frontTileLabel.Text = "Front";
            // 
            // paletteLabel
            // 
            paletteLabel.Location = new System.Drawing.Point(296, 15);
            paletteLabel.Name = "paletteLabel";
            paletteLabel.Size = new System.Drawing.Size(100, 23);
            paletteLabel.TabIndex = 9;
            paletteLabel.Text = "Color palette";
            // 
            // tilesetPanel
            // 
            this.tilesetPanel.Location = new System.Drawing.Point(538, 59);
            this.tilesetPanel.Name = "tilesetPanel";
            this.tilesetPanel.Size = new System.Drawing.Size(64, 192);
            this.tilesetPanel.TabIndex = 17;
            this.tilesetPanel.Zoom = 2F;
            this.tilesetPanel.SelectedTileChanged += new System.EventHandler<System.EventArgs>(this.TilesetPanelSelectedTileChanged);
            // 
            // backTilePanel
            // 
            this.backTilePanel.Front = false;
            this.backTilePanel.Location = new System.Drawing.Point(578, 11);
            this.backTilePanel.Name = "backTilePanel";
            this.backTilePanel.Size = new System.Drawing.Size(24, 24);
            this.backTilePanel.TabIndex = 15;
            this.backTilePanel.Zoom = 3F;
            this.backTilePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundTilePanelMouseDown);
            // 
            // paletteNumericUpDown
            // 
            this.paletteNumericUpDown.Location = new System.Drawing.Point(367, 13);
            this.paletteNumericUpDown.Maximum = new decimal(new int[] {
                                    7,
                                    0,
                                    0,
                                    0});
            this.paletteNumericUpDown.Name = "paletteNumericUpDown";
            this.paletteNumericUpDown.Size = new System.Drawing.Size(37, 20);
            this.paletteNumericUpDown.TabIndex = 10;
            this.paletteNumericUpDown.ValueChanged += new System.EventHandler(this.PaletteNumericUpDownValueChanged);
            // 
            // frontTilePanel
            // 
            this.frontTilePanel.Front = true;
            this.frontTilePanel.Location = new System.Drawing.Point(538, 11);
            this.frontTilePanel.Name = "frontTilePanel";
            this.frontTilePanel.Size = new System.Drawing.Size(24, 24);
            this.frontTilePanel.TabIndex = 13;
            this.frontTilePanel.Zoom = 3F;
            this.frontTilePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundTilePanelMouseDown);
            // 
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(6, 6);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(121, 21);
            this.themeComboBox.TabIndex = 0;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // flipXButton
            // 
            this.flipXButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.flipXButton.Location = new System.Drawing.Point(419, 11);
            this.flipXButton.Name = "flipXButton";
            this.flipXButton.Size = new System.Drawing.Size(50, 23);
            this.flipXButton.TabIndex = 11;
            this.flipXButton.Text = "Flip X";
            this.flipXButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.flipXButton.UseVisualStyleBackColor = true;
            this.flipXButton.CheckedChanged += new System.EventHandler(this.FlipXButtonCheckedChanged);
            // 
            // flipYButton
            // 
            this.flipYButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.flipYButton.Location = new System.Drawing.Point(475, 11);
            this.flipYButton.Name = "flipYButton";
            this.flipYButton.Size = new System.Drawing.Size(50, 23);
            this.flipYButton.TabIndex = 12;
            this.flipYButton.Text = "Flip Y";
            this.flipYButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.flipYButton.UseVisualStyleBackColor = true;
            this.flipYButton.CheckedChanged += new System.EventHandler(this.FlipYButtonCheckedChanged);
            // 
            // importGraphicsButton
            // 
            this.importGraphicsButton.Image = global::EpicEdit.Properties.Resources.ImportButton;
            this.importGraphicsButton.Location = new System.Drawing.Point(6, 14);
            this.importGraphicsButton.Name = "importGraphicsButton";
            this.importGraphicsButton.Size = new System.Drawing.Size(24, 24);
            this.importGraphicsButton.TabIndex = 19;
            this.buttonToolTip.SetToolTip(this.importGraphicsButton, "Import graphics");
            this.importGraphicsButton.UseVisualStyleBackColor = true;
            this.importGraphicsButton.Click += new System.EventHandler(this.ImportGraphicsButtonClick);
            // 
            // exportGraphicsButton
            // 
            this.exportGraphicsButton.Image = global::EpicEdit.Properties.Resources.ExportButton;
            this.exportGraphicsButton.Location = new System.Drawing.Point(34, 14);
            this.exportGraphicsButton.Name = "exportGraphicsButton";
            this.exportGraphicsButton.Size = new System.Drawing.Size(24, 24);
            this.exportGraphicsButton.TabIndex = 20;
            this.buttonToolTip.SetToolTip(this.exportGraphicsButton, "Export graphics");
            this.exportGraphicsButton.UseVisualStyleBackColor = true;
            this.exportGraphicsButton.Click += new System.EventHandler(this.ExportGraphicsButtonClick);
            // 
            // exportLayoutButton
            // 
            this.exportLayoutButton.Image = global::EpicEdit.Properties.Resources.ExportButton;
            this.exportLayoutButton.Location = new System.Drawing.Point(34, 14);
            this.exportLayoutButton.Name = "exportLayoutButton";
            this.exportLayoutButton.Size = new System.Drawing.Size(24, 24);
            this.exportLayoutButton.TabIndex = 23;
            this.buttonToolTip.SetToolTip(this.exportLayoutButton, "Export layout");
            this.exportLayoutButton.UseVisualStyleBackColor = true;
            this.exportLayoutButton.Click += new System.EventHandler(this.ExportLayoutButtonClick);
            // 
            // importLayoutButton
            // 
            this.importLayoutButton.Image = global::EpicEdit.Properties.Resources.ImportButton;
            this.importLayoutButton.Location = new System.Drawing.Point(6, 14);
            this.importLayoutButton.Name = "importLayoutButton";
            this.importLayoutButton.Size = new System.Drawing.Size(24, 24);
            this.importLayoutButton.TabIndex = 22;
            this.buttonToolTip.SetToolTip(this.importLayoutButton, "Import layout");
            this.importLayoutButton.UseVisualStyleBackColor = true;
            this.importLayoutButton.Click += new System.EventHandler(this.ImportLayoutButtonClick);
            // 
            // graphicsGroupBox
            // 
            this.graphicsGroupBox.Controls.Add(this.importGraphicsButton);
            this.graphicsGroupBox.Controls.Add(this.exportGraphicsButton);
            this.graphicsGroupBox.Location = new System.Drawing.Point(538, 257);
            this.graphicsGroupBox.Name = "graphicsGroupBox";
            this.graphicsGroupBox.Size = new System.Drawing.Size(64, 44);
            this.graphicsGroupBox.TabIndex = 18;
            this.graphicsGroupBox.TabStop = false;
            this.graphicsGroupBox.Text = "Graphics";
            // 
            // layoutGroupBox
            // 
            this.layoutGroupBox.Controls.Add(this.importLayoutButton);
            this.layoutGroupBox.Controls.Add(this.exportLayoutButton);
            this.layoutGroupBox.Location = new System.Drawing.Point(538, 301);
            this.layoutGroupBox.Name = "layoutGroupBox";
            this.layoutGroupBox.Size = new System.Drawing.Size(64, 44);
            this.layoutGroupBox.TabIndex = 21;
            this.layoutGroupBox.TabStop = false;
            this.layoutGroupBox.Text = "Layout";
            // 
            // BackgroundEditor
            // 
            this.Controls.Add(this.layoutGroupBox);
            this.Controls.Add(this.graphicsGroupBox);
            this.Controls.Add(this.flipYButton);
            this.Controls.Add(this.flipXButton);
            this.Controls.Add(this.tilesetPanel);
            this.Controls.Add(backTileLabel);
            this.Controls.Add(backLayerGroupBox);
            this.Controls.Add(this.backTilePanel);
            this.Controls.Add(frontTileLabel);
            this.Controls.Add(frontLayerGroupBox);
            this.Controls.Add(this.frontTilePanel);
            this.Controls.Add(this.themeComboBox);
            this.Controls.Add(previewGroupBox);
            this.Controls.Add(this.paletteNumericUpDown);
            this.Controls.Add(paletteLabel);
            this.Name = "BackgroundEditor";
            this.Size = new System.Drawing.Size(610, 350);
            previewGroupBox.ResumeLayout(false);
            frontLayerGroupBox.ResumeLayout(false);
            backLayerGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).EndInit();
            this.graphicsGroupBox.ResumeLayout(false);
            this.layoutGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.GroupBox layoutGroupBox;
        private System.Windows.Forms.GroupBox graphicsGroupBox;
        private System.Windows.Forms.Button importLayoutButton;
        private System.Windows.Forms.Button exportLayoutButton;
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button exportGraphicsButton;
        private System.Windows.Forms.Button importGraphicsButton;
        private System.Windows.Forms.CheckBox flipYButton;
        private System.Windows.Forms.CheckBox flipXButton;
        private EpicEdit.UI.ThemeEdition.BackgroundTilesetPanel tilesetPanel;
        private EpicEdit.UI.ThemeEdition.BackgroundTilePanel backTilePanel;
        private EpicEdit.UI.ThemeEdition.BackgroundTilePanel frontTilePanel;
        private EpicEdit.UI.Tools.EpicNumericUpDown paletteNumericUpDown;
        private System.Windows.Forms.Button rewindButton;
        private EpicEdit.UI.ThemeEdition.BackgroundPanel backLayerPanel;
        private EpicEdit.UI.ThemeEdition.BackgroundPanel frontLayerPanel;
        private System.Windows.Forms.Button playPauseButton;
        private EpicEdit.UI.Tools.ThemeComboBox themeComboBox;
        private EpicEdit.UI.ThemeEdition.BackgroundPreviewer backgroundPreviewer;
    }
}
