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

using EpicEdit.Rom;
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Tools;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the probabilities for players to get each item.
    /// </summary>
    internal partial class ItemProbaEditor : UserControl
    {
        private ItemProbabilities _itemProbabilities;
        private ItemProbability _itemProbability;

        /// <summary>
        /// Flag to prevent events being fired in sequence from one control to another.
        /// </summary>
        private bool _fireEvents;

        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add
            {
                mushroomPanel.ColorSelected += value;
                featherPanel.ColorSelected += value;
                starPanel.ColorSelected += value;
                bananaPanel.ColorSelected += value;
                greenPanel.ColorSelected += value;
                redPanel.ColorSelected += value;
                ghostPanel.ColorSelected += value;
                coinsPanel.ColorSelected += value;
                lightningPanel.ColorSelected += value;
            }
            remove
            {
                mushroomPanel.ColorSelected -= value;
                featherPanel.ColorSelected -= value;
                starPanel.ColorSelected -= value;
                bananaPanel.ColorSelected -= value;
                greenPanel.ColorSelected -= value;
                redPanel.ColorSelected -= value;
                ghostPanel.ColorSelected -= value;
                coinsPanel.ColorSelected -= value;
                lightningPanel.ColorSelected -= value;
            }
        }

        public ItemProbaEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the Value of the Control using the Description of the underlying Enum item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemComboBoxFormat(object sender, ListControlConvertEventArgs e)
        {
            e.Value = UITools.GetDescription(e.Value);
        }

        #region Fields initialization and display

        public void Init()
        {
            _fireEvents = false;

            _itemProbabilities = Context.Game.Settings.ItemProbabilities;

            InitModeComboBox();
            InitSetComboBox();
            InitLapRankComboBox();
            InitItemBoxDisplayOptionComboBox();

            InitProbability();
            DisplayProbability();

            InitImages();

            _fireEvents = true;
        }

        private void InitModeComboBox()
        {
            modeComboBox.BeginUpdate();
            modeComboBox.Items.Clear();

            foreach (var textItem in Context.Game.Settings.ModeNames)
            {
                modeComboBox.Items.Add(textItem.FormattedValue);
            }

            modeComboBox.EndUpdate();
            modeComboBox.SelectedIndex = 0;
        }

        private void InitSetComboBox()
        {
            setComboBox.BeginUpdate();
            setComboBox.Items.Clear();

            for (var i = 0; i < ItemProbabilities.SetCount; i++)
            {
                setComboBox.Items.Add("Probability set " + (i + 1));
            }

            setComboBox.EndUpdate();
            setComboBox.SelectedIndex = 0;
        }

        private void InitLapRankComboBox()
        {
            lapRankComboBox.BeginUpdate();
            lapRankComboBox.Items.Clear();

            switch (modeComboBox.SelectedIndex)
            {
                case 0:
                    lapRankComboBox.Items.Add(ItemProbabilityGrandprixCondition.Lap1_1st);
                    lapRankComboBox.Items.Add(ItemProbabilityGrandprixCondition.Lap2To5_2ndTo4th);
                    lapRankComboBox.Items.Add(ItemProbabilityGrandprixCondition.Lap2To5_5thTo8th);
                    lapRankComboBox.SelectedIndex = 0;
                    break;

                case 1:
                    lapRankComboBox.Items.Add(ItemProbabilityMatchRaceCondition.Lap1);
                    lapRankComboBox.Items.Add(ItemProbabilityMatchRaceCondition.Lap2To5_1st);
                    lapRankComboBox.Items.Add(ItemProbabilityMatchRaceCondition.Lap2To5_2nd);
                    lapRankComboBox.SelectedIndex = 0;
                    break;
            }

            lapRankComboBox.EndUpdate();
        }

        private void InitItemBoxDisplayOptionComboBox()
        {
            itemBoxDisplayOptions.BeginUpdate();
            itemBoxDisplayOptions.Items.Clear();

            switch (modeComboBox.SelectedIndex)
            {
                case 0:
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhosts);
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhostsOrFeathers);
                    break;

                case 1:
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.AllItems);
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoFeathers);
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoCoinsOrLightnings);
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhosts);
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoGhostsOrFeathers);
                    break;

                case 2:
                    itemBoxDisplayOptions.Items.Add(ItemBoxDisplay.NoCoinsOrLightnings);
                    break;
            }

            itemBoxDisplayOptions.EndUpdate();
        }

        private void InitProbability()
        {
            switch (modeComboBox.SelectedIndex)
            {
                case 0: // GP
                    _itemProbability = _itemProbabilities.GetGrandprixProbability(setComboBox.SelectedIndex, (ItemProbabilityGrandprixCondition)lapRankComboBox.SelectedItem);
                    if (_itemProbability.DisplayedItems != ItemBoxDisplay.NoGhosts &&
                        _itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers)
                    {
                        _itemProbability.DisplayedItems = ItemBoxDisplay.NoGhosts;
                    }
                    break;

                case 1: // Match Race
                    _itemProbability = _itemProbabilities.GetMatchRaceProbability(setComboBox.SelectedIndex, (ItemProbabilityMatchRaceCondition)lapRankComboBox.SelectedItem);
                    break;

                case 2: // Battle Mode
                    _itemProbability = _itemProbabilities.GetBattleModeProbability();
                    _itemProbability.DisplayedItems = ItemBoxDisplay.NoCoinsOrLightnings;
                    break;
            }
        }

        private void DisplayProbability()
        {
            // Back up the fireEvents value to restore it at the end of the method
            var fireEventsBefore = _fireEvents;

            // Disable events being fired by updating the various fields
            _fireEvents = false;

            coinsLabel.Enabled =
                coinsPanel.LooksEnabled =
                coinsNumericUpDown.Enabled =
                coinsPctLabel.Enabled =
                _itemProbability.DisplayedItems != ItemBoxDisplay.NoCoinsOrLightnings;

            featherLabel.Enabled =
                featherPanel.LooksEnabled =
                featherNumericUpDown.Enabled =
                featherPctLabel.Enabled =
                _itemProbability.DisplayedItems != ItemBoxDisplay.NoFeathers &&
                _itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers;

            ghostLabel.Enabled =
                ghostPanel.LooksEnabled =
                ghostNumericUpDown.Enabled =
                ghostPctLabel.Enabled =
                _itemProbability.DisplayedItems != ItemBoxDisplay.NoGhosts &&
                _itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers;

            lightningLabel.Enabled =
                lightningPanel.LooksEnabled =
                lightningValue.Enabled =
                lightningPctLabel.Enabled =
                _itemProbability.DisplayedItems != ItemBoxDisplay.NoCoinsOrLightnings;

            ghostNumericUpDown.ReadOnly = !lightningLabel.Enabled;

            var ci = CultureInfo.CurrentCulture;

            var total = _itemProbability.Total;
            mushroomNumericUpDown.Value = _itemProbability.Mushroom;
            featherNumericUpDown.Value = _itemProbability.Feather;
            starNumericUpDown.Value = _itemProbability.Star;
            bananaNumericUpDown.Value = _itemProbability.Banana;
            greenNumericUpDown.Value = _itemProbability.GreenShell;
            redNumericUpDown.Value = _itemProbability.RedShell;
            ghostNumericUpDown.Value = _itemProbability.Ghost;
            coinsNumericUpDown.Value = _itemProbability.Coins;
            lightningValue.Text = _itemProbability.Lightning.ToString(ci);
            totalValue.Text = total.ToString(ci);
            itemBoxDisplayOptions.SelectedItem = _itemProbability.DisplayedItems;

            mushroomPctLabel.Text = ((float)_itemProbability.Mushroom / total).ToString("P1", ci);
            featherPctLabel.Text = ((float)_itemProbability.Feather / total).ToString("P1", ci);
            starPctLabel.Text = ((float)_itemProbability.Star / total).ToString("P1", ci);
            bananaPctLabel.Text = ((float)_itemProbability.Banana / total).ToString("P1", ci);
            greenPctLabel.Text = ((float)_itemProbability.GreenShell / total).ToString("P1", ci);
            redPctLabel.Text = ((float)_itemProbability.RedShell / total).ToString("P1", ci);
            ghostPctLabel.Text = ((float)_itemProbability.Ghost / total).ToString("P1", ci);
            coinsPctLabel.Text = ((float)_itemProbability.Coins / total).ToString("P1", ci);
            lightningPctLabel.Text = ((float)_itemProbability.Lightning / total).ToString("P1", ci);
            totalPctLabel.Text = 1.ToString("P1", ci);

            _fireEvents = fireEventsBefore;
        }

        private void InitImages()
        {
            mushroomPanel.UpdateImage();
            featherPanel.UpdateImage();
            starPanel.UpdateImage();
            bananaPanel.UpdateImage();
            greenPanel.UpdateImage();
            redPanel.UpdateImage();
            ghostPanel.UpdateImage();
            coinsPanel.UpdateImage();
            lightningPanel.UpdateImage();
        }

        #endregion Fields initialization and display

        #region Event handlers

        private void ValueChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                // This event may be fired because of DisplayProbability, skip in that case
                return;
            }

            // Set the values into the item probabilities object
            if (_itemProbability.DisplayedItems == ItemBoxDisplay.NoCoinsOrLightnings)
            {
                // Set the Ghost value to 0, then affect the Lightning value to it later.
                // This way, the Lightning value will always be 0, and the difference will
                // be affected to the Ghost value instead.
                _itemProbability.Ghost = 0;
            }
            else
            {
                _itemProbability.Ghost = (int)ghostNumericUpDown.Value;
            }

            _itemProbability.Mushroom = (int)mushroomNumericUpDown.Value;
            _itemProbability.Feather = (int)featherNumericUpDown.Value;
            _itemProbability.Star = (int)starNumericUpDown.Value;
            _itemProbability.Banana = (int)bananaNumericUpDown.Value;
            _itemProbability.GreenShell = (int)greenNumericUpDown.Value;
            _itemProbability.RedShell = (int)redNumericUpDown.Value;
            _itemProbability.Coins = (int)coinsNumericUpDown.Value;

            if (_itemProbability.DisplayedItems == ItemBoxDisplay.NoCoinsOrLightnings)
            {
                _itemProbability.Ghost = _itemProbability.Lightning;
            }

            _itemProbability.DisplayedItems = (ItemBoxDisplay)itemBoxDisplayOptions.SelectedItem;

            // Then redisplay them as validation occurs within the object
            DisplayProbability();
        }

        private void ModeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            // Disable events so they don't get fired multiple times by other controls being updated
            _fireEvents = false;

            InitLapRankComboBox();
            InitItemBoxDisplayOptionComboBox();
            lapRankComboBox.Enabled = modeComboBox.SelectedIndex != 2;
            setComboBox.Enabled = modeComboBox.SelectedIndex != 2;
            if (modeComboBox.SelectedIndex == 2)
            {
                setComboBox.SelectedItem = null;
            }
            else if (setComboBox.SelectedItem == null)
            {
                setComboBox.SelectedIndex = 0;
            }

            itemBoxDisplayOptions.Enabled = modeComboBox.SelectedIndex != 2;
            itemBoxDisplayOptionLabel.Enabled = modeComboBox.SelectedIndex != 2;

            InitProbability();
            DisplayProbability();

            _fireEvents = true;
        }

        private void LapRankComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            InitProbability();
            DisplayProbability();
        }

        private void SetComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            InitProbability();
            DisplayProbability();
        }

        private Theme _theme;

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get => _theme;
            set
            {
                if (_theme == value)
                {
                    return;
                }

                if (_theme != null)
                {
                    for (var i = 0; i < Palettes.SpritePaletteStart; i++)
                    {
                        _theme.Palettes[i].ColorGraphicsChanged -= palette_ColorsGraphicsChanged;
                        _theme.Palettes[i].ColorsGraphicsChanged -= palette_ColorsGraphicsChanged;
                    }
                }

                _theme = value;

                for (var i = 0; i < Palettes.SpritePaletteStart; i++)
                {
                    _theme.Palettes[i].ColorGraphicsChanged += palette_ColorsGraphicsChanged;
                    _theme.Palettes[i].ColorsGraphicsChanged += palette_ColorsGraphicsChanged;
                }

                UpdateIcons(_theme);
            }
        }

        private void palette_ColorsGraphicsChanged(object sender, EventArgs e)
        {
            InitImages();
        }

        public void ShowTrackData(Track track)
        {
            if (track is GPTrack gpTrack)
            {
                modeComboBox.SelectedIndex = 0;
                setComboBox.SelectedIndex = gpTrack.ItemProbabilityIndex;
                lapRankComboBox.SelectedIndex = 0;
            }
            else
            {
                modeComboBox.SelectedIndex = 2;
            }
        }

        private void UpdateIcons(Theme theme)
        {
            mushroomPanel.Theme = theme;
            featherPanel.Theme = theme;
            starPanel.Theme = theme;
            bananaPanel.Theme = theme;
            greenPanel.Theme = theme;
            redPanel.Theme = theme;
            ghostPanel.Theme = theme;
            coinsPanel.Theme = theme;
            lightningPanel.Theme = theme;
        }

        private void ResetButtonClick(object sender, EventArgs e)
        {
            _itemProbability.Reset();
            DisplayProbability();
        }

        private void ImportProbabilitiesButtonClick(object sender, EventArgs e)
        {
            if (UITools.ShowImportBinaryDataDialog(_itemProbabilities.SetBytes))
            {
                Init();
            }
        }

        private void ExportProbabilitiesButtonClick(object sender, EventArgs e)
        {
            UITools.ShowExportBinaryDataDialog(_itemProbabilities.GetBytes, "Item probabilities");
        }

        #endregion Event handlers
    }
}
