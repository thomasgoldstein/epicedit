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
                this.playerTrackBar.Dispose();
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
            System.Windows.Forms.Label previewSpeedLabel;
            System.Windows.Forms.GroupBox frontLayerGroupBox;
            System.Windows.Forms.GroupBox backLayerGroupBox;
            System.Windows.Forms.Label backTileLabel;
            System.Windows.Forms.Label frontTileLabel;
            System.Windows.Forms.Label paletteLabel;
            this.previewSpeedNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.playerTrackBar = new EpicEdit.UI.Tools.PlayerTrackBar();
            this.backgroundPreviewer = new EpicEdit.UI.ThemeEdition.BackgroundPreviewer();
            this.playPauseButton = new System.Windows.Forms.Button();
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
            previewSpeedLabel = new System.Windows.Forms.Label();
            frontLayerGroupBox = new System.Windows.Forms.GroupBox();
            backLayerGroupBox = new System.Windows.Forms.GroupBox();
            backTileLabel = new System.Windows.Forms.Label();
            frontTileLabel = new System.Windows.Forms.Label();
            paletteLabel = new System.Windows.Forms.Label();
            previewGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewSpeedNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerTrackBar)).BeginInit();
            frontLayerGroupBox.SuspendLayout();
            backLayerGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.paletteNumericUpDown)).BeginInit();
            this.graphicsGroupBox.SuspendLayout();
            this.layoutGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // previewGroupBox
            // 
            previewGroupBox.Controls.Add(previewSpeedLabel);
            previewGroupBox.Controls.Add(this.previewSpeedNumericUpDown);
            previewGroupBox.Controls.Add(this.playerTrackBar);
            previewGroupBox.Controls.Add(this.backgroundPreviewer);
            previewGroupBox.Controls.Add(this.playPauseButton);
            previewGroupBox.Location = new System.Drawing.Point(6, 238);
            previewGroupBox.Name = "previewGroupBox";
            previewGroupBox.Size = new System.Drawing.Size(526, 107);
            previewGroupBox.TabIndex = 11;
            previewGroupBox.TabStop = false;
            previewGroupBox.Text = "Preview";
            // 
            // previewSpeedLabel
            // 
            previewSpeedLabel.Location = new System.Drawing.Point(431, 80);
            previewSpeedLabel.Name = "previewSpeedLabel";
            previewSpeedLabel.Size = new System.Drawing.Size(43, 23);
            previewSpeedLabel.TabIndex = 3;
            previewSpeedLabel.Text = "Speed";
            // 
            // previewSpeedNumericUpDown
            // 
            this.previewSpeedNumericUpDown.Location = new System.Drawing.Point(480, 77);
            this.previewSpeedNumericUpDown.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.previewSpeedNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.previewSpeedNumericUpDown.Name = "previewSpeedNumericUpDown";
            this.previewSpeedNumericUpDown.Size = new System.Drawing.Size(40, 20);
            this.previewSpeedNumericUpDown.TabIndex = 4;
            this.previewSpeedNumericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.previewSpeedNumericUpDown.ValueChanged += new System.EventHandler(this.PreviewSpeedNumericUpDownValueChanged);
            // 
            // playerTrackBar
            // 
            this.playerTrackBar.LargeChange = 0;
            this.playerTrackBar.Location = new System.Drawing.Point(88, 78);
            this.playerTrackBar.Maximum = 511;
            this.playerTrackBar.Name = "playerTrackBar";
            this.playerTrackBar.Size = new System.Drawing.Size(337, 45);
            this.playerTrackBar.SmallChange = 0;
            this.playerTrackBar.TabIndex = 2;
            this.playerTrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.playerTrackBar.ValueChanged += new System.EventHandler(this.PlayerTrackBarValueChanged);
            // 
            // backgroundPreviewer
            // 
            this.backgroundPreviewer.Location = new System.Drawing.Point(7, 19);
            this.backgroundPreviewer.Name = "backgroundPreviewer";
            this.backgroundPreviewer.Size = new System.Drawing.Size(512, 48);
            this.backgroundPreviewer.TabIndex = 0;
            // 
            // playPauseButton
            // 
            this.playPauseButton.Image = global::EpicEdit.Properties.Resources.PlayButton;
            this.playPauseButton.Location = new System.Drawing.Point(7, 77);
            this.playPauseButton.Name = "playPauseButton";
            this.playPauseButton.Size = new System.Drawing.Size(75, 24);
            this.playPauseButton.TabIndex = 1;
            this.playPauseButton.Text = "Play";
            this.playPauseButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.playPauseButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.playPauseButton.UseVisualStyleBackColor = true;
            this.playPauseButton.Click += new System.EventHandler(this.PlayPauseButtonClick);
            // 
            // frontLayerGroupBox
            // 
            frontLayerGroupBox.Controls.Add(this.frontLayerPanel);
            frontLayerGroupBox.Location = new System.Drawing.Point(6, 40);
            frontLayerGroupBox.Name = "frontLayerGroupBox";
            frontLayerGroupBox.Size = new System.Drawing.Size(526, 93);
            frontLayerGroupBox.TabIndex = 9;
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
            this.frontLayerPanel.TabIndex = 0;
            this.frontLayerPanel.TileChanged += new System.EventHandler<System.EventArgs>(this.BackgroundLayerPanelTileChanged);
            this.frontLayerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundLayerPanelMouseDown);
            // 
            // backLayerGroupBox
            // 
            backLayerGroupBox.Controls.Add(this.backLayerPanel);
            backLayerGroupBox.Location = new System.Drawing.Point(6, 139);
            backLayerGroupBox.Name = "backLayerGroupBox";
            backLayerGroupBox.Size = new System.Drawing.Size(526, 93);
            backLayerGroupBox.TabIndex = 10;
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
            this.backLayerPanel.TabIndex = 0;
            this.backLayerPanel.TileChanged += new System.EventHandler<System.EventArgs>(this.BackgroundLayerPanelTileChanged);
            this.backLayerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundLayerPanelMouseDown);
            // 
            // backTileLabel
            // 
            backTileLabel.Location = new System.Drawing.Point(575, 40);
            backTileLabel.Name = "backTileLabel";
            backTileLabel.Size = new System.Drawing.Size(32, 23);
            backTileLabel.TabIndex = 8;
            backTileLabel.Text = "Back";
            // 
            // frontTileLabel
            // 
            frontTileLabel.Location = new System.Drawing.Point(534, 40);
            frontTileLabel.Name = "frontTileLabel";
            frontTileLabel.Size = new System.Drawing.Size(32, 23);
            frontTileLabel.TabIndex = 7;
            frontTileLabel.Text = "Front";
            // 
            // paletteLabel
            // 
            paletteLabel.Location = new System.Drawing.Point(296, 15);
            paletteLabel.Name = "paletteLabel";
            paletteLabel.Size = new System.Drawing.Size(100, 23);
            paletteLabel.TabIndex = 1;
            paletteLabel.Text = "Color palette";
            // 
            // tilesetPanel
            // 
            this.tilesetPanel.Location = new System.Drawing.Point(538, 59);
            this.tilesetPanel.Name = "tilesetPanel";
            this.tilesetPanel.Size = new System.Drawing.Size(64, 192);
            this.tilesetPanel.TabIndex = 12;
            this.tilesetPanel.Zoom = 2F;
            this.tilesetPanel.SelectedTileChanged += new System.EventHandler<System.EventArgs>(this.TilesetPanelSelectedTileChanged);
            // 
            // backTilePanel
            // 
            this.backTilePanel.Front = false;
            this.backTilePanel.Location = new System.Drawing.Point(578, 11);
            this.backTilePanel.Name = "backTilePanel";
            this.backTilePanel.Size = new System.Drawing.Size(24, 24);
            this.backTilePanel.TabIndex = 6;
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
            this.paletteNumericUpDown.TabIndex = 2;
            this.paletteNumericUpDown.ValueChanged += new System.EventHandler(this.PaletteNumericUpDownValueChanged);
            // 
            // frontTilePanel
            // 
            this.frontTilePanel.Front = true;
            this.frontTilePanel.Location = new System.Drawing.Point(538, 11);
            this.frontTilePanel.Name = "frontTilePanel";
            this.frontTilePanel.Size = new System.Drawing.Size(24, 24);
            this.frontTilePanel.TabIndex = 5;
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
            this.flipXButton.TabIndex = 3;
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
            this.flipYButton.TabIndex = 4;
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
            this.importGraphicsButton.TabIndex = 0;
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
            this.exportGraphicsButton.TabIndex = 1;
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
            this.exportLayoutButton.TabIndex = 1;
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
            this.importLayoutButton.TabIndex = 0;
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
            this.graphicsGroupBox.TabIndex = 13;
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
            this.layoutGroupBox.TabIndex = 14;
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
            previewGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewSpeedNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.playerTrackBar)).EndInit();
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
        private EpicEdit.UI.ThemeEdition.BackgroundPanel backLayerPanel;
        private EpicEdit.UI.ThemeEdition.BackgroundPanel frontLayerPanel;
        private System.Windows.Forms.Button playPauseButton;
        private EpicEdit.UI.Tools.ThemeComboBox themeComboBox;
        private EpicEdit.UI.ThemeEdition.BackgroundPreviewer backgroundPreviewer;
        private EpicEdit.UI.Tools.PlayerTrackBar playerTrackBar;
        private System.Windows.Forms.NumericUpDown previewSpeedNumericUpDown;
    }
}
