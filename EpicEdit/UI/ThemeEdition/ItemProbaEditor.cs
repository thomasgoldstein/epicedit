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

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the probabilities for players to get each item.
    /// </summary>
    internal partial class ItemProbaEditor : UserControl
    {
        private ItemProbabilities itemProbabilities;
        private ItemProbability itemProbability;

        /// <summary>
        /// Flag to prevent events being fired in sequence from one control to another.
        /// </summary>
        private bool performEvents = true;

        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true)]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add
            {
                this.mushroomPanel.ColorSelected += value;
                this.featherPanel.ColorSelected += value;
                this.starPanel.ColorSelected += value;
                this.bananaPanel.ColorSelected += value;
                this.greenPanel.ColorSelected += value;
                this.redPanel.ColorSelected += value;
                this.ghostPanel.ColorSelected += value;
                this.coinsPanel.ColorSelected += value;
                this.lightningPanel.ColorSelected += value;
            }
            remove
            {
                this.mushroomPanel.ColorSelected -= value;
                this.featherPanel.ColorSelected -= value;
                this.starPanel.ColorSelected -= value;
                this.bananaPanel.ColorSelected -= value;
                this.greenPanel.ColorSelected -= value;
                this.redPanel.ColorSelected -= value;
                this.ghostPanel.ColorSelected -= value;
                this.coinsPanel.ColorSelected -= value;
                this.lightningPanel.ColorSelected -= value;
            }
        }

        public ItemProbaEditor()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the Value of the Control using the Description of the underlying Enum item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetValueFromDescription(object sender, ListControlConvertEventArgs e)
        {
            e.Value = UITools.GetDescription(e.Value);
        }

        #region Fields initialization and display

        public void Init()
        {
            this.performEvents = false;

            this.itemProbabilities = Context.Game.ItemProbabilities;

            this.InitModeComboBox();
            this.InitThemeComboBox();
            this.InitLapRankComboBox();
            this.InitItemBoxDisplayOptionComboBox();

            this.InitProbability();
            this.DisplayProbability();

            this.InitImages();

            this.performEvents = true;
        }

        private void InitModeComboBox()
        {
            this.modeComboBox.BeginUpdate();
            this.modeComboBox.Items.Clear();

            string[] modeNames = Context.Game.GetModeNames();
            foreach (string modeName in modeNames)
            {
                this.modeComboBox.Items.Add(modeName);
            }

            this.modeComboBox.EndUpdate();
            this.modeComboBox.SelectedIndex = 0;
        }

        private void InitThemeComboBox()
        {
            this.themeComboBox.BeginUpdate();
            this.themeComboBox.Items.Clear();

            Themes themes = Context.Game.Themes;
            for (int i = 0; i < themes.Count - 2; i++)
            {
                this.themeComboBox.Items.Add(themes[i].Name);
            }

            this.themeComboBox.Items.Add(themes[themes.Count - 2].Name + "/ " + themes[themes.Count - 1].Name);
            this.themeComboBox.EndUpdate();
            this.themeComboBox.SelectedIndex = 0;
        }

        private void InitLapRankComboBox()
        {
            this.lapRankComboBox.BeginUpdate();
            this.lapRankComboBox.Items.Clear();

            switch (this.modeComboBox.SelectedIndex)
            {
                case 0:
                    this.lapRankComboBox.Items.Add(GrandprixCondition.Lap1_1st);
                    this.lapRankComboBox.Items.Add(GrandprixCondition.Lap2To5_2ndTo4th);
                    this.lapRankComboBox.Items.Add(GrandprixCondition.Lap2To5_5thTo8th);
                    this.lapRankComboBox.SelectedIndex = 0;
                    break;

                case 1:
                    this.lapRankComboBox.Items.Add(MatchRaceCondition.Lap1);
                    this.lapRankComboBox.Items.Add(MatchRaceCondition.Lap2To5_1st);
                    this.lapRankComboBox.Items.Add(MatchRaceCondition.Lap2To5_2nd);
                    this.lapRankComboBox.SelectedIndex = 0;
                    break;
            }

            this.lapRankComboBox.EndUpdate();
        }

        private void InitItemBoxDisplayOptionComboBox()
        {
            this.itemBoxDisplayOptions.BeginUpdate();
            this.itemBoxDisplayOptions.Items.Clear();

            switch (this.modeComboBox.SelectedIndex)
            {
                case 0:
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhosts);
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhostsOrFeathers);
                    break;

                case 1:
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.AllItems);
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoFeathers);
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoCoinsOrLightnings);
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhosts);
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhostsOrFeathers);
                    break;

                case 2:
                    this.itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoCoinsOrLightnings);
                    break;
            }

            this.itemBoxDisplayOptions.EndUpdate();
        }

        private void InitProbability()
        {
            switch (this.modeComboBox.SelectedIndex)
            {
                case 0: // GP
                    this.itemProbability = this.itemProbabilities.GetGrandprixProbability(this.GetTheme(), (GrandprixCondition)this.lapRankComboBox.SelectedItem);
                    if (this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhosts &&
                        this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers)
                    {
                        this.itemProbability.DisplayedItems = ItemBoxDisplay.NoGhosts;
                    }
                    break;

                case 1: // Match Race
                    this.itemProbability = this.itemProbabilities.GetMatchRaceProbability(this.GetTheme(), (MatchRaceCondition)this.lapRankComboBox.SelectedItem);
                    break;

                case 2: // Battle Mode
                    this.itemProbability = this.itemProbabilities.GetBattleModeProbability();
                    this.itemProbability.DisplayedItems = ItemBoxDisplay.NoCoinsOrLightnings;
                    break;
            }
        }

        private bool ignoreChange = false;

        private void DisplayProbability()
        {
            // Disable events being fired by updating the various fields
            this.ignoreChange = true;

            this.coinsLabel.Enabled =
                this.coinsPanel.LooksEnabled =
                this.coinsNumericUpDown.Enabled =
                this.coinsPctLabel.Enabled =
                this.itemProbability.DisplayedItems != ItemBoxDisplay.NoCoinsOrLightnings;

            this.featherLabel.Enabled =
                this.featherPanel.LooksEnabled =
                this.featherNumericUpDown.Enabled =
                this.featherPctLabel.Enabled =
                this.itemProbability.DisplayedItems != ItemBoxDisplay.NoFeathers &&
                this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers;

            this.ghostLabel.Enabled =
                this.ghostPanel.LooksEnabled =
                this.ghostNumericUpDown.Enabled =
                this.ghostPctLabel.Enabled =
                this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhosts &&
                this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers;

            this.lightningLabel.Enabled =
                this.lightningPanel.LooksEnabled =
                this.lightningValue.Enabled =
                this.lightningPctLabel.Enabled =
                this.itemProbability.DisplayedItems != ItemBoxDisplay.NoCoinsOrLightnings;

            this.ghostNumericUpDown.ReadOnly = !this.lightningLabel.Enabled;

            CultureInfo ic = CultureInfo.InvariantCulture;

            int total = this.itemProbability.Total;
            this.mushroomNumericUpDown.Value = this.itemProbability.Mushroom;
            this.featherNumericUpDown.Value = this.itemProbability.Feather;
            this.starNumericUpDown.Value = this.itemProbability.Star;
            this.bananaNumericUpDown.Value = this.itemProbability.Banana;
            this.greenNumericUpDown.Value = this.itemProbability.Green;
            this.redNumericUpDown.Value = this.itemProbability.Red;
            this.ghostNumericUpDown.Value = this.itemProbability.Ghost;
            this.coinsNumericUpDown.Value = this.itemProbability.Coins;
            this.lightningValue.Text = this.itemProbability.Lightning.ToString(ic);
            this.totalValue.Text = total.ToString(ic);
            this.itemBoxDisplayOptions.SelectedItem = this.itemProbability.DisplayedItems;

            this.mushroomPctLabel.Text = ((float)this.itemProbability.Mushroom / total).ToString("P1", ic);
            this.featherPctLabel.Text = ((float)this.itemProbability.Feather / total).ToString("P1", ic);
            this.starPctLabel.Text = ((float)this.itemProbability.Star / total).ToString("P1", ic);
            this.bananaPctLabel.Text = ((float)this.itemProbability.Banana / total).ToString("P1", ic);
            this.greenPctLabel.Text = ((float)this.itemProbability.Green / total).ToString("P1", ic);
            this.redPctLabel.Text = ((float)this.itemProbability.Red / total).ToString("P1", ic);
            this.ghostPctLabel.Text = ((float)this.itemProbability.Ghost / total).ToString("P1", ic);
            this.coinsPctLabel.Text = ((float)this.itemProbability.Coins / total).ToString("P1", ic);
            this.lightningPctLabel.Text = ((float)this.itemProbability.Lightning / total).ToString("P1", ic);
            this.totalPctLabel.Text = 1.ToString("P1", ic);

            this.ignoreChange = false;
        }

        private void InitImages()
        {
            this.mushroomPanel.UpdateImage();
            this.featherPanel.UpdateImage();
            this.starPanel.UpdateImage();
            this.bananaPanel.UpdateImage();
            this.greenPanel.UpdateImage();
            this.redPanel.UpdateImage();
            this.ghostPanel.UpdateImage();
            this.coinsPanel.UpdateImage();
            this.lightningPanel.UpdateImage();
        }

        public void UpdateImages(Palette palette)
        {
            Context.Game.ItemIconGraphics.UpdateTiles(palette);
            this.InitImages();
        }

        #endregion Fields initialization and display

        #region Getters / converters for the ComboBoxes

        private ItemProbaTheme GetTheme()
        {
            ItemProbaTheme ret;

            switch (this.themeComboBox.SelectedIndex)
            {
                case 0:
                    ret = ItemProbaTheme.GhostValley;
                    break;

                default:
                case 1:
                    ret = ItemProbaTheme.MarioCircuit;
                    break;

                case 2:
                    ret = ItemProbaTheme.DonutPlains;
                    break;

                case 3:
                    ret = ItemProbaTheme.ChocoIsland;
                    break;

                case 4:
                    ret = ItemProbaTheme.VanillaLake;
                    break;

                case 5:
                    ret = ItemProbaTheme.KoopaBeach;
                    break;

                case 6:
                    ret = ItemProbaTheme.BowserCastleAndRainbowRoad;
                    break;
            }

            return ret;
        }

        #endregion Getters / converters for the ComboBoxes

        #region Events handlers

        private void ValueChanged(object sender, EventArgs e)
        {
            if (this.ignoreChange)
            {
                // This event may be fired because of DisplayProbability, skip in that case
                return;
            }

            // Set the values into the item probabilities object
            if (this.itemProbability.DisplayedItems == ItemBoxDisplay.NoCoinsOrLightnings)
            {
                // Set the Ghost value to 0, then affect the Lightning value to it later.
                // This way, the Lightning value will always be 0, and the difference will
                // be affected to the Ghost value instead.
                this.itemProbability.Ghost = 0;
            }
            else
            {
                this.itemProbability.Ghost = (int)this.ghostNumericUpDown.Value;
            }

            this.itemProbability.Mushroom = (int)this.mushroomNumericUpDown.Value;
            this.itemProbability.Feather = (int)this.featherNumericUpDown.Value;
            this.itemProbability.Star = (int)this.starNumericUpDown.Value;
            this.itemProbability.Banana = (int)this.bananaNumericUpDown.Value;
            this.itemProbability.Green = (int)this.greenNumericUpDown.Value;
            this.itemProbability.Red = (int)this.redNumericUpDown.Value;
            this.itemProbability.Coins = (int)this.coinsNumericUpDown.Value;

            if (this.itemProbability.DisplayedItems == ItemBoxDisplay.NoCoinsOrLightnings)
            {
                this.itemProbability.Ghost = this.itemProbability.Lightning;
            }

            this.itemProbability.DisplayedItems = (ItemBoxDisplay)this.itemBoxDisplayOptions.SelectedItem;

            // Then redisplay them as validation occurs within the object
            this.DisplayProbability();
        }

        private void ModeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.performEvents)
            {
                return;
            }

            // Disable events so they don't get fired multiple times by other controls being updated
            this.performEvents = false;

            this.InitLapRankComboBox();
            this.InitItemBoxDisplayOptionComboBox();
            this.lapRankComboBox.Enabled = this.modeComboBox.SelectedIndex != 2;
            this.themeComboBox.Enabled = this.modeComboBox.SelectedIndex != 2;
            if (this.modeComboBox.SelectedIndex == 2)
            {
                this.themeComboBox.SelectedItem = null;
            }
            else if (this.themeComboBox.SelectedItem == null)
            {
                this.themeComboBox.SelectedIndex = 0;
            }

            this.itemBoxDisplayOptions.Enabled = this.modeComboBox.SelectedIndex != 2;
            this.itemBoxDisplayOptionLabel.Enabled = this.modeComboBox.SelectedIndex != 2;

            this.InitProbability();
            this.DisplayProbability();

            this.performEvents = true;
        }

        private void LapRankComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.performEvents)
            {
                this.InitProbability();
                this.DisplayProbability();
            }
        }

        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.performEvents)
            {
                this.InitProbability();
                this.DisplayProbability();
                this.InitImages();
            }
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            this.itemProbability.Reset();
            this.DisplayProbability();
        }

        #endregion Events handlers

        #region class ItemIconPanel
        private sealed class ItemIconPanel : TilePanel
        {
            public ItemIconPanel()
            {
                if (this.DesignMode)
                {
                    // Avoid exceptions in design mode
                    this.image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
                    this.currentImage = this.image;
                }
            }

            private new bool DesignMode
            {
                get
                {
                    if (base.DesignMode)
                    {
                        return true;
                    }

                    return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
                }
            }

            /// <summary>
            /// Reference to the current image (enabled or disabled).
            /// </summary>
            private Image currentImage;

            private Image image;
            private Image disabledImage;

            private Image Image
            {
                //get { return this.image; }
                set
                {
                    if (this.image != null)
                    {
                        this.image.Dispose();
                    }

                    this.image = value;

                    if (this.disabledImage != null)
                    {
                        this.disabledImage.Dispose();
                        this.disabledImage = null;
                    }

                    this.SetCurrentImage();
                }
            }

            private bool looksEnabled = true;

            /// <summary>
            /// Gets or sets value that specifies whether the control looks enabled or not.
            /// This enables mouse events (color picking) even if the control looks disabled.
            /// </summary>
            [Browsable(true), DefaultValue(typeof(bool), "true")]
            public bool LooksEnabled
            {
                //get { return this.looksEnabled; }
                set
                {
                    this.looksEnabled = value;
                    this.SetCurrentImage();
                }
            }

            private ItemType itemType;
            public ItemType ItemType
            {
                //get { return this.itemType; }
                set { this.itemType = value; }
            }

            private void CreateImage()
            {
                int index = (this.Parent as ItemProbaEditor).themeComboBox.SelectedIndex;
                if (index == -1)
                {
                    // No theme selected, default to first theme
                    index = 0;
                }
                Palettes palettes = Context.Game.Themes[index].Palettes;
                this.Image = Context.Game.ItemIconGraphics.GetImage(this.itemType, palettes);
            }

            public void UpdateImage()
            {
                this.CreateImage();
                this.Refresh();
            }

            private void SetCurrentImage()
            {
                if (this.image == null)
                {
                    return;
                }

                if (this.looksEnabled)
                {
                    this.currentImage = this.image;
                }
                else
                {
                    if (this.disabledImage == null)
                    {
                        this.disabledImage = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
                        using (Graphics g = Graphics.FromImage(this.disabledImage))
                        using (Image image = ToolStripRenderer.CreateDisabledImage(this.image))
                        {
                            g.Clear(SystemColors.Control);
                            g.DrawImage(image, 0, 0);
                        }
                    }
                    this.currentImage = this.disabledImage;
                }

                this.Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.DrawImage(this.currentImage, 0, 0);
            }

            protected override Tile GetTileAt(int x, int y)
            {
                // Convert from pixel precision to tile precision
                x /= Tile.Size;
                y /= Tile.Size;

                return Context.Game.ItemIconGraphics.GetTile(this.itemType, x, y);
            }

            protected override void Dispose(bool disposing)
            {
                if (this.image != null)
                {
                    this.image.Dispose();
                }

                if (this.disabledImage != null)
                {
                    this.disabledImage.Dispose();
                }

                base.Dispose(disposing);
            }
        }
        #endregion class ItemIconPanel
    }
}
