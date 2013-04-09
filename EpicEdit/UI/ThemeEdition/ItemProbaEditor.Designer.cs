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
    partial class ItemProbaEditor
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
            System.Windows.Forms.Label totalLabel;
            this.setComboBox = new System.Windows.Forms.ComboBox();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.totalPctLabel = new System.Windows.Forms.Label();
            this.lightningPctLabel = new System.Windows.Forms.Label();
            this.coinsPctLabel = new System.Windows.Forms.Label();
            this.ghostPctLabel = new System.Windows.Forms.Label();
            this.redPctLabel = new System.Windows.Forms.Label();
            this.greenPctLabel = new System.Windows.Forms.Label();
            this.bananaPctLabel = new System.Windows.Forms.Label();
            this.starPctLabel = new System.Windows.Forms.Label();
            this.featherPctLabel = new System.Windows.Forms.Label();
            this.mushroomPctLabel = new System.Windows.Forms.Label();
            this.lightningPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.coinsPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.ghostPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.redPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.greenPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.bananaPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.starPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.featherPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.mushroomPanel = new EpicEdit.UI.ThemeEdition.ItemIconPanel();
            this.resetButton = new System.Windows.Forms.Button();
            this.itemBoxDisplayOptionLabel = new System.Windows.Forms.Label();
            this.itemBoxDisplayOptions = new System.Windows.Forms.ComboBox();
            this.totalValue = new System.Windows.Forms.Label();
            this.lightningValue = new System.Windows.Forms.Label();
            this.coinsNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ghostNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.redNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.greenNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.starNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.bananaNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.featherNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mushroomNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lightningLabel = new System.Windows.Forms.Label();
            this.coinsLabel = new System.Windows.Forms.Label();
            this.ghostLabel = new System.Windows.Forms.Label();
            this.redLabel = new System.Windows.Forms.Label();
            this.greenLabel = new System.Windows.Forms.Label();
            this.bananaLabel = new System.Windows.Forms.Label();
            this.starLabel = new System.Windows.Forms.Label();
            this.featherLabel = new System.Windows.Forms.Label();
            this.mushroomLabel = new System.Windows.Forms.Label();
            this.lapRankComboBox = new System.Windows.Forms.ComboBox();
            this.exportProbabilitiesButton = new System.Windows.Forms.Button();
            this.importProbabilitiesButton = new System.Windows.Forms.Button();
            this.buttonToolTip = new System.Windows.Forms.ToolTip(this.components);
            totalLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.coinsNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.starNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bananaNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.featherNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mushroomNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // totalLabel
            // 
            totalLabel.AutoSize = true;
            totalLabel.Location = new System.Drawing.Point(230, 152);
            totalLabel.Name = "totalLabel";
            totalLabel.Size = new System.Drawing.Size(31, 13);
            totalLabel.TabIndex = 35;
            totalLabel.Text = "Total";
            // 
            // setComboBox
            // 
            this.setComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.setComboBox.FormattingEnabled = true;
            this.setComboBox.Location = new System.Drawing.Point(128, 6);
            this.setComboBox.Name = "setComboBox";
            this.setComboBox.Size = new System.Drawing.Size(116, 21);
            this.setComboBox.TabIndex = 1;
            this.setComboBox.SelectedIndexChanged += new System.EventHandler(this.SetComboBoxSelectedIndexChanged);
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(6, 6);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(116, 21);
            this.modeComboBox.TabIndex = 0;
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.ModeComboBoxSelectedIndexChanged);
            // 
            // totalPctLabel
            // 
            this.totalPctLabel.Location = new System.Drawing.Point(368, 152);
            this.totalPctLabel.Name = "totalPctLabel";
            this.totalPctLabel.Size = new System.Drawing.Size(50, 13);
            this.totalPctLabel.TabIndex = 59;
            this.totalPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lightningPctLabel
            // 
            this.lightningPctLabel.Location = new System.Drawing.Point(368, 126);
            this.lightningPctLabel.Name = "lightningPctLabel";
            this.lightningPctLabel.Size = new System.Drawing.Size(50, 13);
            this.lightningPctLabel.TabIndex = 58;
            this.lightningPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // coinsPctLabel
            // 
            this.coinsPctLabel.Location = new System.Drawing.Point(368, 100);
            this.coinsPctLabel.Name = "coinsPctLabel";
            this.coinsPctLabel.Size = new System.Drawing.Size(50, 13);
            this.coinsPctLabel.TabIndex = 57;
            this.coinsPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ghostPctLabel
            // 
            this.ghostPctLabel.Location = new System.Drawing.Point(368, 74);
            this.ghostPctLabel.Name = "ghostPctLabel";
            this.ghostPctLabel.Size = new System.Drawing.Size(50, 13);
            this.ghostPctLabel.TabIndex = 56;
            this.ghostPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // redPctLabel
            // 
            this.redPctLabel.Location = new System.Drawing.Point(368, 48);
            this.redPctLabel.Name = "redPctLabel";
            this.redPctLabel.Size = new System.Drawing.Size(50, 13);
            this.redPctLabel.TabIndex = 55;
            this.redPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // greenPctLabel
            // 
            this.greenPctLabel.Location = new System.Drawing.Point(149, 152);
            this.greenPctLabel.Name = "greenPctLabel";
            this.greenPctLabel.Size = new System.Drawing.Size(50, 13);
            this.greenPctLabel.TabIndex = 54;
            this.greenPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bananaPctLabel
            // 
            this.bananaPctLabel.Location = new System.Drawing.Point(149, 126);
            this.bananaPctLabel.Name = "bananaPctLabel";
            this.bananaPctLabel.Size = new System.Drawing.Size(50, 13);
            this.bananaPctLabel.TabIndex = 53;
            this.bananaPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // starPctLabel
            // 
            this.starPctLabel.Location = new System.Drawing.Point(149, 100);
            this.starPctLabel.Name = "starPctLabel";
            this.starPctLabel.Size = new System.Drawing.Size(50, 13);
            this.starPctLabel.TabIndex = 52;
            this.starPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // featherPctLabel
            // 
            this.featherPctLabel.Location = new System.Drawing.Point(149, 74);
            this.featherPctLabel.Name = "featherPctLabel";
            this.featherPctLabel.Size = new System.Drawing.Size(50, 13);
            this.featherPctLabel.TabIndex = 51;
            this.featherPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mushroomPctLabel
            // 
            this.mushroomPctLabel.Location = new System.Drawing.Point(149, 48);
            this.mushroomPctLabel.Name = "mushroomPctLabel";
            this.mushroomPctLabel.Size = new System.Drawing.Size(50, 13);
            this.mushroomPctLabel.TabIndex = 50;
            this.mushroomPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lightningPanel
            // 
            this.lightningPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Lightning;
            this.lightningPanel.Location = new System.Drawing.Point(297, 126);
            this.lightningPanel.Name = "lightningPanel";
            this.lightningPanel.Size = new System.Drawing.Size(16, 16);
            this.lightningPanel.TabIndex = 49;
            // 
            // coinsPanel
            // 
            this.coinsPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Coin;
            this.coinsPanel.Location = new System.Drawing.Point(297, 100);
            this.coinsPanel.Name = "coinsPanel";
            this.coinsPanel.Size = new System.Drawing.Size(16, 16);
            this.coinsPanel.TabIndex = 48;
            // 
            // ghostPanel
            // 
            this.ghostPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Ghost;
            this.ghostPanel.Location = new System.Drawing.Point(297, 74);
            this.ghostPanel.Name = "ghostPanel";
            this.ghostPanel.Size = new System.Drawing.Size(16, 16);
            this.ghostPanel.TabIndex = 47;
            // 
            // redPanel
            // 
            this.redPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.RedShell;
            this.redPanel.Location = new System.Drawing.Point(297, 48);
            this.redPanel.Name = "redPanel";
            this.redPanel.Size = new System.Drawing.Size(16, 16);
            this.redPanel.TabIndex = 46;
            // 
            // greenPanel
            // 
            this.greenPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.GreenShell;
            this.greenPanel.Location = new System.Drawing.Point(78, 152);
            this.greenPanel.Name = "greenPanel";
            this.greenPanel.Size = new System.Drawing.Size(16, 16);
            this.greenPanel.TabIndex = 45;
            // 
            // bananaPanel
            // 
            this.bananaPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Banana;
            this.bananaPanel.Location = new System.Drawing.Point(78, 126);
            this.bananaPanel.Name = "bananaPanel";
            this.bananaPanel.Size = new System.Drawing.Size(16, 16);
            this.bananaPanel.TabIndex = 44;
            // 
            // starPanel
            // 
            this.starPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Star;
            this.starPanel.Location = new System.Drawing.Point(78, 100);
            this.starPanel.Name = "starPanel";
            this.starPanel.Size = new System.Drawing.Size(16, 16);
            this.starPanel.TabIndex = 43;
            // 
            // featherPanel
            // 
            this.featherPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Feather;
            this.featherPanel.Location = new System.Drawing.Point(78, 74);
            this.featherPanel.Name = "featherPanel";
            this.featherPanel.Size = new System.Drawing.Size(16, 16);
            this.featherPanel.TabIndex = 42;
            // 
            // mushroomPanel
            // 
            this.mushroomPanel.ItemType = EpicEdit.Rom.Tracks.Items.ItemType.Mushroom;
            this.mushroomPanel.Location = new System.Drawing.Point(78, 48);
            this.mushroomPanel.Name = "mushroomPanel";
            this.mushroomPanel.Size = new System.Drawing.Size(16, 16);
            this.mushroomPanel.TabIndex = 41;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(6, 220);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(75, 23);
            this.resetButton.TabIndex = 40;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.ResetButtonClick);
            // 
            // itemBoxDisplayOptionLabel
            // 
            this.itemBoxDisplayOptionLabel.AutoSize = true;
            this.itemBoxDisplayOptionLabel.Location = new System.Drawing.Point(54, 191);
            this.itemBoxDisplayOptionLabel.Name = "itemBoxDisplayOptionLabel";
            this.itemBoxDisplayOptionLabel.Size = new System.Drawing.Size(114, 13);
            this.itemBoxDisplayOptionLabel.TabIndex = 39;
            this.itemBoxDisplayOptionLabel.Text = "Item box display option";
            // 
            // itemBoxDisplayOptions
            // 
            this.itemBoxDisplayOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemBoxDisplayOptions.FormattingEnabled = true;
            this.itemBoxDisplayOptions.Location = new System.Drawing.Point(194, 188);
            this.itemBoxDisplayOptions.Name = "itemBoxDisplayOptions";
            this.itemBoxDisplayOptions.Size = new System.Drawing.Size(130, 21);
            this.itemBoxDisplayOptions.TabIndex = 38;
            this.itemBoxDisplayOptions.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            this.itemBoxDisplayOptions.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.ItemComboBoxFormat);
            // 
            // totalValue
            // 
            this.totalValue.AutoSize = true;
            this.totalValue.Location = new System.Drawing.Point(330, 152);
            this.totalValue.Name = "totalValue";
            this.totalValue.Size = new System.Drawing.Size(0, 13);
            this.totalValue.TabIndex = 37;
            // 
            // lightningValue
            // 
            this.lightningValue.AutoSize = true;
            this.lightningValue.Location = new System.Drawing.Point(330, 126);
            this.lightningValue.Name = "lightningValue";
            this.lightningValue.Size = new System.Drawing.Size(0, 13);
            this.lightningValue.TabIndex = 36;
            // 
            // coinsNumericUpDown
            // 
            this.coinsNumericUpDown.Location = new System.Drawing.Point(322, 98);
            this.coinsNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.coinsNumericUpDown.Name = "coinsNumericUpDown";
            this.coinsNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.coinsNumericUpDown.TabIndex = 34;
            this.coinsNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.coinsNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // ghostNumericUpDown
            // 
            this.ghostNumericUpDown.Location = new System.Drawing.Point(322, 72);
            this.ghostNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.ghostNumericUpDown.Name = "ghostNumericUpDown";
            this.ghostNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.ghostNumericUpDown.TabIndex = 33;
            this.ghostNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ghostNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // redNumericUpDown
            // 
            this.redNumericUpDown.Location = new System.Drawing.Point(322, 46);
            this.redNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.redNumericUpDown.Name = "redNumericUpDown";
            this.redNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.redNumericUpDown.TabIndex = 32;
            this.redNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.redNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // greenNumericUpDown
            // 
            this.greenNumericUpDown.Location = new System.Drawing.Point(103, 150);
            this.greenNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.greenNumericUpDown.Name = "greenNumericUpDown";
            this.greenNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.greenNumericUpDown.TabIndex = 31;
            this.greenNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.greenNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // starNumericUpDown
            // 
            this.starNumericUpDown.Location = new System.Drawing.Point(103, 98);
            this.starNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.starNumericUpDown.Name = "starNumericUpDown";
            this.starNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.starNumericUpDown.TabIndex = 30;
            this.starNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.starNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // bananaNumericUpDown
            // 
            this.bananaNumericUpDown.Location = new System.Drawing.Point(103, 124);
            this.bananaNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.bananaNumericUpDown.Name = "bananaNumericUpDown";
            this.bananaNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.bananaNumericUpDown.TabIndex = 29;
            this.bananaNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.bananaNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // featherNumericUpDown
            // 
            this.featherNumericUpDown.Location = new System.Drawing.Point(103, 72);
            this.featherNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.featherNumericUpDown.Name = "featherNumericUpDown";
            this.featherNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.featherNumericUpDown.TabIndex = 28;
            this.featherNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.featherNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // mushroomNumericUpDown
            // 
            this.mushroomNumericUpDown.Location = new System.Drawing.Point(103, 46);
            this.mushroomNumericUpDown.Maximum = new decimal(new int[] {
                                    32,
                                    0,
                                    0,
                                    0});
            this.mushroomNumericUpDown.Name = "mushroomNumericUpDown";
            this.mushroomNumericUpDown.Size = new System.Drawing.Size(44, 20);
            this.mushroomNumericUpDown.TabIndex = 27;
            this.mushroomNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mushroomNumericUpDown.ValueChanged += new System.EventHandler(this.ValueChanged);
            // 
            // lightningLabel
            // 
            this.lightningLabel.AutoSize = true;
            this.lightningLabel.Location = new System.Drawing.Point(230, 126);
            this.lightningLabel.Name = "lightningLabel";
            this.lightningLabel.Size = new System.Drawing.Size(50, 13);
            this.lightningLabel.TabIndex = 26;
            this.lightningLabel.Text = "Lightning";
            // 
            // coinsLabel
            // 
            this.coinsLabel.AutoSize = true;
            this.coinsLabel.Location = new System.Drawing.Point(230, 100);
            this.coinsLabel.Name = "coinsLabel";
            this.coinsLabel.Size = new System.Drawing.Size(33, 13);
            this.coinsLabel.TabIndex = 25;
            this.coinsLabel.Text = "Coins";
            // 
            // ghostLabel
            // 
            this.ghostLabel.AutoSize = true;
            this.ghostLabel.Location = new System.Drawing.Point(230, 74);
            this.ghostLabel.Name = "ghostLabel";
            this.ghostLabel.Size = new System.Drawing.Size(35, 13);
            this.ghostLabel.TabIndex = 24;
            this.ghostLabel.Text = "Ghost";
            // 
            // redLabel
            // 
            this.redLabel.AutoSize = true;
            this.redLabel.Location = new System.Drawing.Point(230, 48);
            this.redLabel.Name = "redLabel";
            this.redLabel.Size = new System.Drawing.Size(51, 13);
            this.redLabel.TabIndex = 23;
            this.redLabel.Text = "Red shell";
            // 
            // greenLabel
            // 
            this.greenLabel.AutoSize = true;
            this.greenLabel.Location = new System.Drawing.Point(6, 152);
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Size = new System.Drawing.Size(60, 13);
            this.greenLabel.TabIndex = 22;
            this.greenLabel.Text = "Green shell";
            // 
            // bananaLabel
            // 
            this.bananaLabel.AutoSize = true;
            this.bananaLabel.Location = new System.Drawing.Point(6, 126);
            this.bananaLabel.Name = "bananaLabel";
            this.bananaLabel.Size = new System.Drawing.Size(44, 13);
            this.bananaLabel.TabIndex = 21;
            this.bananaLabel.Text = "Banana";
            // 
            // starLabel
            // 
            this.starLabel.AutoSize = true;
            this.starLabel.Location = new System.Drawing.Point(6, 100);
            this.starLabel.Name = "starLabel";
            this.starLabel.Size = new System.Drawing.Size(26, 13);
            this.starLabel.TabIndex = 20;
            this.starLabel.Text = "Star";
            // 
            // featherLabel
            // 
            this.featherLabel.AutoSize = true;
            this.featherLabel.Location = new System.Drawing.Point(6, 74);
            this.featherLabel.Name = "featherLabel";
            this.featherLabel.Size = new System.Drawing.Size(43, 13);
            this.featherLabel.TabIndex = 19;
            this.featherLabel.Text = "Feather";
            // 
            // mushroomLabel
            // 
            this.mushroomLabel.AutoSize = true;
            this.mushroomLabel.Location = new System.Drawing.Point(6, 48);
            this.mushroomLabel.Name = "mushroomLabel";
            this.mushroomLabel.Size = new System.Drawing.Size(56, 13);
            this.mushroomLabel.TabIndex = 18;
            this.mushroomLabel.Text = "Mushroom";
            // 
            // lapRankComboBox
            // 
            this.lapRankComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lapRankComboBox.FormattingEnabled = true;
            this.lapRankComboBox.Location = new System.Drawing.Point(250, 6);
            this.lapRankComboBox.Name = "lapRankComboBox";
            this.lapRankComboBox.Size = new System.Drawing.Size(116, 21);
            this.lapRankComboBox.TabIndex = 2;
            this.lapRankComboBox.SelectedIndexChanged += new System.EventHandler(this.LapRankComboBoxSelectedIndexChanged);
            this.lapRankComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.ItemComboBoxFormat);
            // 
            // exportProbabilitiesButton
            // 
            this.exportProbabilitiesButton.Image = global::EpicEdit.Properties.Resources.ExportButton;
            this.exportProbabilitiesButton.Location = new System.Drawing.Point(393, 220);
            this.exportProbabilitiesButton.Name = "exportProbabilitiesButton";
            this.exportProbabilitiesButton.Size = new System.Drawing.Size(24, 24);
            this.exportProbabilitiesButton.TabIndex = 61;
            this.buttonToolTip.SetToolTip(this.exportProbabilitiesButton, "Export item probabilities");
            this.exportProbabilitiesButton.UseVisualStyleBackColor = true;
            this.exportProbabilitiesButton.Click += new System.EventHandler(this.ExportProbabilitiesButtonClick);
            // 
            // importProbabilitiesButton
            // 
            this.importProbabilitiesButton.Image = global::EpicEdit.Properties.Resources.ImportButton;
            this.importProbabilitiesButton.Location = new System.Drawing.Point(363, 220);
            this.importProbabilitiesButton.Name = "importProbabilitiesButton";
            this.importProbabilitiesButton.Size = new System.Drawing.Size(24, 24);
            this.importProbabilitiesButton.TabIndex = 60;
            this.buttonToolTip.SetToolTip(this.importProbabilitiesButton, "Import item probabilities");
            this.importProbabilitiesButton.UseVisualStyleBackColor = true;
            this.importProbabilitiesButton.Click += new System.EventHandler(this.ImportProbabilitiesButtonClick);
            // 
            // ItemProbaEditor
            // 
            this.Controls.Add(this.exportProbabilitiesButton);
            this.Controls.Add(this.importProbabilitiesButton);
            this.Controls.Add(this.totalPctLabel);
            this.Controls.Add(this.lightningPctLabel);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.coinsPctLabel);
            this.Controls.Add(this.setComboBox);
            this.Controls.Add(this.ghostPctLabel);
            this.Controls.Add(this.lapRankComboBox);
            this.Controls.Add(this.redPctLabel);
            this.Controls.Add(this.mushroomLabel);
            this.Controls.Add(this.greenPctLabel);
            this.Controls.Add(this.featherLabel);
            this.Controls.Add(this.bananaPctLabel);
            this.Controls.Add(this.starLabel);
            this.Controls.Add(this.starPctLabel);
            this.Controls.Add(this.bananaLabel);
            this.Controls.Add(this.featherPctLabel);
            this.Controls.Add(this.greenLabel);
            this.Controls.Add(this.mushroomPctLabel);
            this.Controls.Add(this.redLabel);
            this.Controls.Add(this.lightningPanel);
            this.Controls.Add(this.ghostLabel);
            this.Controls.Add(this.coinsPanel);
            this.Controls.Add(this.coinsLabel);
            this.Controls.Add(this.ghostPanel);
            this.Controls.Add(this.lightningLabel);
            this.Controls.Add(this.redPanel);
            this.Controls.Add(this.mushroomNumericUpDown);
            this.Controls.Add(this.greenPanel);
            this.Controls.Add(this.featherNumericUpDown);
            this.Controls.Add(this.bananaPanel);
            this.Controls.Add(this.bananaNumericUpDown);
            this.Controls.Add(this.starPanel);
            this.Controls.Add(this.starNumericUpDown);
            this.Controls.Add(this.featherPanel);
            this.Controls.Add(this.greenNumericUpDown);
            this.Controls.Add(this.mushroomPanel);
            this.Controls.Add(this.redNumericUpDown);
            this.Controls.Add(this.ghostNumericUpDown);
            this.Controls.Add(this.itemBoxDisplayOptionLabel);
            this.Controls.Add(this.coinsNumericUpDown);
            this.Controls.Add(this.modeComboBox);
            this.Controls.Add(totalLabel);
            this.Controls.Add(this.itemBoxDisplayOptions);
            this.Controls.Add(this.lightningValue);
            this.Controls.Add(this.totalValue);
            this.Name = "ItemProbaEditor";
            this.Size = new System.Drawing.Size(420, 250);
            ((System.ComponentModel.ISupportInitialize)(this.coinsNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.starNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bananaNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.featherNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mushroomNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        private System.Windows.Forms.ToolTip buttonToolTip;
        private System.Windows.Forms.Button importProbabilitiesButton;
        private System.Windows.Forms.Button exportProbabilitiesButton;
        private System.Windows.Forms.Label mushroomPctLabel;
        private System.Windows.Forms.Label featherPctLabel;
        private System.Windows.Forms.Label starPctLabel;
        private System.Windows.Forms.Label bananaPctLabel;
        private System.Windows.Forms.Label greenPctLabel;
        private System.Windows.Forms.Label redPctLabel;
        private System.Windows.Forms.Label ghostPctLabel;
        private System.Windows.Forms.Label coinsPctLabel;
        private System.Windows.Forms.Label lightningPctLabel;
        private System.Windows.Forms.Label totalPctLabel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel featherPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel starPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel bananaPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel greenPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel redPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel ghostPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel coinsPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel lightningPanel;
        private EpicEdit.UI.ThemeEdition.ItemIconPanel mushroomPanel;
        private System.Windows.Forms.ComboBox lapRankComboBox;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.ComboBox setComboBox;
        private System.Windows.Forms.NumericUpDown coinsNumericUpDown;
        private System.Windows.Forms.NumericUpDown ghostNumericUpDown;
        private System.Windows.Forms.NumericUpDown redNumericUpDown;
        private System.Windows.Forms.NumericUpDown greenNumericUpDown;
        private System.Windows.Forms.NumericUpDown starNumericUpDown;
        private System.Windows.Forms.NumericUpDown bananaNumericUpDown;
        private System.Windows.Forms.NumericUpDown featherNumericUpDown;
        private System.Windows.Forms.NumericUpDown mushroomNumericUpDown;
        private System.Windows.Forms.Label lightningLabel;
        private System.Windows.Forms.Label coinsLabel;
        private System.Windows.Forms.Label ghostLabel;
        private System.Windows.Forms.Label redLabel;
        private System.Windows.Forms.Label greenLabel;
        private System.Windows.Forms.Label bananaLabel;
        private System.Windows.Forms.Label starLabel;
        private System.Windows.Forms.Label featherLabel;
        private System.Windows.Forms.Label mushroomLabel;
        private System.Windows.Forms.Label lightningValue;
        private System.Windows.Forms.Label totalValue;
        private System.Windows.Forms.Label itemBoxDisplayOptionLabel;
        private System.Windows.Forms.ComboBox itemBoxDisplayOptions;
        private System.Windows.Forms.Button resetButton;
    }
}
