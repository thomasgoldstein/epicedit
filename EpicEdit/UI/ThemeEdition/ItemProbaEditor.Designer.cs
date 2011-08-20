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
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.modeComboBox = new System.Windows.Forms.ComboBox();
            this.itemGroupBox = new System.Windows.Forms.GroupBox();
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
            this.lightningPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.coinsPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.ghostPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.redPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.greenPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.bananaPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.starPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.featherPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.mushroomPictureBox = new EpicEdit.UI.Tools.EpicPictureBox();
            this.resetButton = new System.Windows.Forms.Button();
            this.itemBoxDisplayOptionLabel = new System.Windows.Forms.Label();
            this.itemBoxDisplayOption = new System.Windows.Forms.ComboBox();
            this.totalValue = new System.Windows.Forms.Label();
            this.lightningValue = new System.Windows.Forms.Label();
            this.totalLabel = new System.Windows.Forms.Label();
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
            this.itemGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightningPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinsPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.redPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bananaPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.starPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.featherPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mushroomPictureBox)).BeginInit();
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
            // themeComboBox
            // 
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.FormattingEnabled = true;
            this.themeComboBox.Location = new System.Drawing.Point(124, 20);
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.Size = new System.Drawing.Size(182, 21);
            this.themeComboBox.TabIndex = 1;
            this.themeComboBox.SelectedIndexChanged += new System.EventHandler(this.ThemeComboBoxSelectedIndexChanged);
            // 
            // modeComboBox
            // 
            this.modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.modeComboBox.FormattingEnabled = true;
            this.modeComboBox.Location = new System.Drawing.Point(11, 20);
            this.modeComboBox.Name = "modeComboBox";
            this.modeComboBox.Size = new System.Drawing.Size(107, 21);
            this.modeComboBox.TabIndex = 0;
            this.modeComboBox.SelectedIndexChanged += new System.EventHandler(this.ModeComboBoxSelectedIndexChanged);
            // 
            // itemGroupBox
            // 
            this.itemGroupBox.Controls.Add(this.totalPctLabel);
            this.itemGroupBox.Controls.Add(this.lightningPctLabel);
            this.itemGroupBox.Controls.Add(this.coinsPctLabel);
            this.itemGroupBox.Controls.Add(this.ghostPctLabel);
            this.itemGroupBox.Controls.Add(this.redPctLabel);
            this.itemGroupBox.Controls.Add(this.greenPctLabel);
            this.itemGroupBox.Controls.Add(this.bananaPctLabel);
            this.itemGroupBox.Controls.Add(this.starPctLabel);
            this.itemGroupBox.Controls.Add(this.featherPctLabel);
            this.itemGroupBox.Controls.Add(this.mushroomPctLabel);
            this.itemGroupBox.Controls.Add(this.lightningPictureBox);
            this.itemGroupBox.Controls.Add(this.coinsPictureBox);
            this.itemGroupBox.Controls.Add(this.ghostPictureBox);
            this.itemGroupBox.Controls.Add(this.redPictureBox);
            this.itemGroupBox.Controls.Add(this.greenPictureBox);
            this.itemGroupBox.Controls.Add(this.bananaPictureBox);
            this.itemGroupBox.Controls.Add(this.starPictureBox);
            this.itemGroupBox.Controls.Add(this.featherPictureBox);
            this.itemGroupBox.Controls.Add(this.mushroomPictureBox);
            this.itemGroupBox.Controls.Add(this.resetButton);
            this.itemGroupBox.Controls.Add(this.itemBoxDisplayOptionLabel);
            this.itemGroupBox.Controls.Add(this.modeComboBox);
            this.itemGroupBox.Controls.Add(this.itemBoxDisplayOption);
            this.itemGroupBox.Controls.Add(this.totalValue);
            this.itemGroupBox.Controls.Add(this.lightningValue);
            this.itemGroupBox.Controls.Add(this.totalLabel);
            this.itemGroupBox.Controls.Add(this.coinsNumericUpDown);
            this.itemGroupBox.Controls.Add(this.ghostNumericUpDown);
            this.itemGroupBox.Controls.Add(this.redNumericUpDown);
            this.itemGroupBox.Controls.Add(this.greenNumericUpDown);
            this.itemGroupBox.Controls.Add(this.starNumericUpDown);
            this.itemGroupBox.Controls.Add(this.bananaNumericUpDown);
            this.itemGroupBox.Controls.Add(this.featherNumericUpDown);
            this.itemGroupBox.Controls.Add(this.mushroomNumericUpDown);
            this.itemGroupBox.Controls.Add(this.lightningLabel);
            this.itemGroupBox.Controls.Add(this.coinsLabel);
            this.itemGroupBox.Controls.Add(this.ghostLabel);
            this.itemGroupBox.Controls.Add(this.redLabel);
            this.itemGroupBox.Controls.Add(this.greenLabel);
            this.itemGroupBox.Controls.Add(this.bananaLabel);
            this.itemGroupBox.Controls.Add(this.starLabel);
            this.itemGroupBox.Controls.Add(this.featherLabel);
            this.itemGroupBox.Controls.Add(this.mushroomLabel);
            this.itemGroupBox.Controls.Add(this.lapRankComboBox);
            this.itemGroupBox.Controls.Add(this.themeComboBox);
            this.itemGroupBox.Location = new System.Drawing.Point(0, 0);
            this.itemGroupBox.Name = "itemGroupBox";
            this.itemGroupBox.Size = new System.Drawing.Size(430, 270);
            this.itemGroupBox.TabIndex = 3;
            this.itemGroupBox.TabStop = false;
            this.itemGroupBox.Text = "Item probabilities";
            // 
            // totalPctLabel
            // 
            this.totalPctLabel.Location = new System.Drawing.Point(373, 166);
            this.totalPctLabel.Name = "totalPctLabel";
            this.totalPctLabel.Size = new System.Drawing.Size(50, 13);
            this.totalPctLabel.TabIndex = 59;
            this.totalPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lightningPctLabel
            // 
            this.lightningPctLabel.Location = new System.Drawing.Point(373, 140);
            this.lightningPctLabel.Name = "lightningPctLabel";
            this.lightningPctLabel.Size = new System.Drawing.Size(50, 13);
            this.lightningPctLabel.TabIndex = 58;
            this.lightningPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // coinsPctLabel
            // 
            this.coinsPctLabel.Location = new System.Drawing.Point(373, 114);
            this.coinsPctLabel.Name = "coinsPctLabel";
            this.coinsPctLabel.Size = new System.Drawing.Size(50, 13);
            this.coinsPctLabel.TabIndex = 57;
            this.coinsPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ghostPctLabel
            // 
            this.ghostPctLabel.Location = new System.Drawing.Point(373, 88);
            this.ghostPctLabel.Name = "ghostPctLabel";
            this.ghostPctLabel.Size = new System.Drawing.Size(50, 13);
            this.ghostPctLabel.TabIndex = 56;
            this.ghostPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // redPctLabel
            // 
            this.redPctLabel.Location = new System.Drawing.Point(373, 62);
            this.redPctLabel.Name = "redPctLabel";
            this.redPctLabel.Size = new System.Drawing.Size(50, 13);
            this.redPctLabel.TabIndex = 55;
            this.redPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // greenPctLabel
            // 
            this.greenPctLabel.Location = new System.Drawing.Point(154, 166);
            this.greenPctLabel.Name = "greenPctLabel";
            this.greenPctLabel.Size = new System.Drawing.Size(50, 13);
            this.greenPctLabel.TabIndex = 54;
            this.greenPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // bananaPctLabel
            // 
            this.bananaPctLabel.Location = new System.Drawing.Point(154, 140);
            this.bananaPctLabel.Name = "bananaPctLabel";
            this.bananaPctLabel.Size = new System.Drawing.Size(50, 13);
            this.bananaPctLabel.TabIndex = 53;
            this.bananaPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // starPctLabel
            // 
            this.starPctLabel.Location = new System.Drawing.Point(154, 114);
            this.starPctLabel.Name = "starPctLabel";
            this.starPctLabel.Size = new System.Drawing.Size(50, 13);
            this.starPctLabel.TabIndex = 52;
            this.starPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // featherPctLabel
            // 
            this.featherPctLabel.Location = new System.Drawing.Point(154, 88);
            this.featherPctLabel.Name = "featherPctLabel";
            this.featherPctLabel.Size = new System.Drawing.Size(50, 13);
            this.featherPctLabel.TabIndex = 51;
            this.featherPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mushroomPctLabel
            // 
            this.mushroomPctLabel.Location = new System.Drawing.Point(154, 62);
            this.mushroomPctLabel.Name = "mushroomPctLabel";
            this.mushroomPctLabel.Size = new System.Drawing.Size(50, 13);
            this.mushroomPctLabel.TabIndex = 50;
            this.mushroomPctLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lightningPictureBox
            // 
            this.lightningPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.lightningPictureBox.Image = null;
            this.lightningPictureBox.Location = new System.Drawing.Point(302, 140);
            this.lightningPictureBox.Name = "lightningPictureBox";
            this.lightningPictureBox.Size = new System.Drawing.Size(16, 16);
            this.lightningPictureBox.TabIndex = 49;
            this.lightningPictureBox.TabStop = false;
            // 
            // coinsPictureBox
            // 
            this.coinsPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.coinsPictureBox.Image = null;
            this.coinsPictureBox.Location = new System.Drawing.Point(302, 114);
            this.coinsPictureBox.Name = "coinsPictureBox";
            this.coinsPictureBox.Size = new System.Drawing.Size(16, 16);
            this.coinsPictureBox.TabIndex = 48;
            this.coinsPictureBox.TabStop = false;
            // 
            // ghostPictureBox
            // 
            this.ghostPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.ghostPictureBox.Image = null;
            this.ghostPictureBox.Location = new System.Drawing.Point(302, 88);
            this.ghostPictureBox.Name = "ghostPictureBox";
            this.ghostPictureBox.Size = new System.Drawing.Size(16, 16);
            this.ghostPictureBox.TabIndex = 47;
            this.ghostPictureBox.TabStop = false;
            // 
            // redPictureBox
            // 
            this.redPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.redPictureBox.Image = null;
            this.redPictureBox.Location = new System.Drawing.Point(302, 62);
            this.redPictureBox.Name = "redPictureBox";
            this.redPictureBox.Size = new System.Drawing.Size(16, 16);
            this.redPictureBox.TabIndex = 46;
            this.redPictureBox.TabStop = false;
            // 
            // greenPictureBox
            // 
            this.greenPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.greenPictureBox.Image = null;
            this.greenPictureBox.Location = new System.Drawing.Point(83, 166);
            this.greenPictureBox.Name = "greenPictureBox";
            this.greenPictureBox.Size = new System.Drawing.Size(16, 16);
            this.greenPictureBox.TabIndex = 45;
            this.greenPictureBox.TabStop = false;
            // 
            // bananaPictureBox
            // 
            this.bananaPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.bananaPictureBox.Image = null;
            this.bananaPictureBox.Location = new System.Drawing.Point(83, 140);
            this.bananaPictureBox.Name = "bananaPictureBox";
            this.bananaPictureBox.Size = new System.Drawing.Size(16, 16);
            this.bananaPictureBox.TabIndex = 44;
            this.bananaPictureBox.TabStop = false;
            // 
            // starPictureBox
            // 
            this.starPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.starPictureBox.Image = null;
            this.starPictureBox.Location = new System.Drawing.Point(83, 114);
            this.starPictureBox.Name = "starPictureBox";
            this.starPictureBox.Size = new System.Drawing.Size(16, 16);
            this.starPictureBox.TabIndex = 43;
            this.starPictureBox.TabStop = false;
            // 
            // featherPictureBox
            // 
            this.featherPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.featherPictureBox.Image = null;
            this.featherPictureBox.Location = new System.Drawing.Point(83, 88);
            this.featherPictureBox.Name = "featherPictureBox";
            this.featherPictureBox.Size = new System.Drawing.Size(16, 16);
            this.featherPictureBox.TabIndex = 42;
            this.featherPictureBox.TabStop = false;
            // 
            // mushroomPictureBox
            // 
            this.mushroomPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.mushroomPictureBox.Image = null;
            this.mushroomPictureBox.Location = new System.Drawing.Point(83, 62);
            this.mushroomPictureBox.Name = "mushroomPictureBox";
            this.mushroomPictureBox.Size = new System.Drawing.Size(16, 16);
            this.mushroomPictureBox.TabIndex = 41;
            this.mushroomPictureBox.TabStop = false;
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(11, 234);
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
            this.itemBoxDisplayOptionLabel.Location = new System.Drawing.Point(59, 205);
            this.itemBoxDisplayOptionLabel.Name = "itemBoxDisplayOptionLabel";
            this.itemBoxDisplayOptionLabel.Size = new System.Drawing.Size(114, 13);
            this.itemBoxDisplayOptionLabel.TabIndex = 39;
            this.itemBoxDisplayOptionLabel.Text = "Item box display option";
            // 
            // itemBoxDisplayOption
            // 
            this.itemBoxDisplayOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.itemBoxDisplayOption.FormattingEnabled = true;
            this.itemBoxDisplayOption.Location = new System.Drawing.Point(199, 202);
            this.itemBoxDisplayOption.Name = "itemBoxDisplayOption";
            this.itemBoxDisplayOption.Size = new System.Drawing.Size(130, 21);
            this.itemBoxDisplayOption.TabIndex = 38;
            this.itemBoxDisplayOption.SelectedIndexChanged += new System.EventHandler(this.ValueChanged);
            this.itemBoxDisplayOption.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.SetValueFromDescription);
            // 
            // totalValue
            // 
            this.totalValue.AutoSize = true;
            this.totalValue.Location = new System.Drawing.Point(335, 166);
            this.totalValue.Name = "totalValue";
            this.totalValue.Size = new System.Drawing.Size(0, 13);
            this.totalValue.TabIndex = 37;
            // 
            // lightningValue
            // 
            this.lightningValue.AutoSize = true;
            this.lightningValue.Location = new System.Drawing.Point(335, 140);
            this.lightningValue.Name = "lightningValue";
            this.lightningValue.Size = new System.Drawing.Size(0, 13);
            this.lightningValue.TabIndex = 36;
            // 
            // totalLabel
            // 
            this.totalLabel.AutoSize = true;
            this.totalLabel.Location = new System.Drawing.Point(235, 166);
            this.totalLabel.Name = "totalLabel";
            this.totalLabel.Size = new System.Drawing.Size(31, 13);
            this.totalLabel.TabIndex = 35;
            this.totalLabel.Text = "Total";
            // 
            // coinsNumericUpDown
            // 
            this.coinsNumericUpDown.Location = new System.Drawing.Point(327, 112);
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
            this.ghostNumericUpDown.Location = new System.Drawing.Point(327, 86);
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
            this.redNumericUpDown.Location = new System.Drawing.Point(327, 60);
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
            this.greenNumericUpDown.Location = new System.Drawing.Point(108, 164);
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
            this.starNumericUpDown.Location = new System.Drawing.Point(108, 112);
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
            this.bananaNumericUpDown.Location = new System.Drawing.Point(108, 138);
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
            this.featherNumericUpDown.Location = new System.Drawing.Point(108, 86);
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
            this.mushroomNumericUpDown.Location = new System.Drawing.Point(108, 60);
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
            this.lightningLabel.Location = new System.Drawing.Point(235, 140);
            this.lightningLabel.Name = "lightningLabel";
            this.lightningLabel.Size = new System.Drawing.Size(50, 13);
            this.lightningLabel.TabIndex = 26;
            this.lightningLabel.Text = "Lightning";
            // 
            // coinsLabel
            // 
            this.coinsLabel.AutoSize = true;
            this.coinsLabel.Location = new System.Drawing.Point(235, 114);
            this.coinsLabel.Name = "coinsLabel";
            this.coinsLabel.Size = new System.Drawing.Size(33, 13);
            this.coinsLabel.TabIndex = 25;
            this.coinsLabel.Text = "Coins";
            // 
            // ghostLabel
            // 
            this.ghostLabel.AutoSize = true;
            this.ghostLabel.Location = new System.Drawing.Point(235, 88);
            this.ghostLabel.Name = "ghostLabel";
            this.ghostLabel.Size = new System.Drawing.Size(35, 13);
            this.ghostLabel.TabIndex = 24;
            this.ghostLabel.Text = "Ghost";
            // 
            // redLabel
            // 
            this.redLabel.AutoSize = true;
            this.redLabel.Location = new System.Drawing.Point(235, 62);
            this.redLabel.Name = "redLabel";
            this.redLabel.Size = new System.Drawing.Size(51, 13);
            this.redLabel.TabIndex = 23;
            this.redLabel.Text = "Red shell";
            // 
            // greenLabel
            // 
            this.greenLabel.AutoSize = true;
            this.greenLabel.Location = new System.Drawing.Point(11, 166);
            this.greenLabel.Name = "greenLabel";
            this.greenLabel.Size = new System.Drawing.Size(60, 13);
            this.greenLabel.TabIndex = 22;
            this.greenLabel.Text = "Green shell";
            // 
            // bananaLabel
            // 
            this.bananaLabel.AutoSize = true;
            this.bananaLabel.Location = new System.Drawing.Point(11, 140);
            this.bananaLabel.Name = "bananaLabel";
            this.bananaLabel.Size = new System.Drawing.Size(44, 13);
            this.bananaLabel.TabIndex = 21;
            this.bananaLabel.Text = "Banana";
            // 
            // starLabel
            // 
            this.starLabel.AutoSize = true;
            this.starLabel.Location = new System.Drawing.Point(11, 114);
            this.starLabel.Name = "starLabel";
            this.starLabel.Size = new System.Drawing.Size(26, 13);
            this.starLabel.TabIndex = 20;
            this.starLabel.Text = "Star";
            // 
            // featherLabel
            // 
            this.featherLabel.AutoSize = true;
            this.featherLabel.Location = new System.Drawing.Point(11, 88);
            this.featherLabel.Name = "featherLabel";
            this.featherLabel.Size = new System.Drawing.Size(43, 13);
            this.featherLabel.TabIndex = 19;
            this.featherLabel.Text = "Feather";
            // 
            // mushroomLabel
            // 
            this.mushroomLabel.AutoSize = true;
            this.mushroomLabel.Location = new System.Drawing.Point(11, 62);
            this.mushroomLabel.Name = "mushroomLabel";
            this.mushroomLabel.Size = new System.Drawing.Size(56, 13);
            this.mushroomLabel.TabIndex = 18;
            this.mushroomLabel.Text = "Mushroom";
            // 
            // lapRankComboBox
            // 
            this.lapRankComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lapRankComboBox.FormattingEnabled = true;
            this.lapRankComboBox.Location = new System.Drawing.Point(312, 20);
            this.lapRankComboBox.Name = "lapRankComboBox";
            this.lapRankComboBox.Size = new System.Drawing.Size(107, 21);
            this.lapRankComboBox.TabIndex = 2;
            this.lapRankComboBox.SelectedIndexChanged += new System.EventHandler(this.LapRankComboBoxSelectedIndexChanged);
            this.lapRankComboBox.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.SetValueFromDescription);
            // 
            // ItemProbaEditor
            // 
            this.Controls.Add(this.itemGroupBox);
            this.Name = "ItemProbaEditor";
            this.Size = new System.Drawing.Size(430, 270);
            this.itemGroupBox.ResumeLayout(false);
            this.itemGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightningPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinsPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bananaPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.starPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.featherPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mushroomPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coinsNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ghostNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.redNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.starNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bananaNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.featherNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mushroomNumericUpDown)).EndInit();
            this.ResumeLayout(false);
        }
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
        private EpicEdit.UI.Tools.EpicPictureBox featherPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox starPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox bananaPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox greenPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox redPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox ghostPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox coinsPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox lightningPictureBox;
        private EpicEdit.UI.Tools.EpicPictureBox mushroomPictureBox;
        private System.Windows.Forms.ComboBox lapRankComboBox;
        private System.Windows.Forms.GroupBox itemGroupBox;
        private System.Windows.Forms.ComboBox modeComboBox;
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.Label totalLabel;
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
        private System.Windows.Forms.ComboBox itemBoxDisplayOption;
        private System.Windows.Forms.Button resetButton;
    }
}
