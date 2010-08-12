﻿#region GPL statement
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
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.ItemProba;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
	/// <summary>
	/// Represents a collection of controls used to edit the probabilities for players to get each item.
	/// </summary>
	public partial class ItemProbaControl : UserControl
	{
		private ItemProbabilities itemProbabilities;
		private ItemProbability itemProbability;

		/// <summary>
		/// Flag to prevent events being fired in sequence from one control to another.
		/// </summary>
		private bool performEvents = true;

		public ItemProbaControl()
		{
			this.InitializeComponent();
		}

		/// <summary>
		/// Gets the description attribute text of an enum that was passed as an item into a ComboBox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GetDescription(object sender, ListControlConvertEventArgs e)
		{
			UITools.SetValueFromEnumDescription(e);
		}

		#region Fields Initialization and Display

		public void InitOnRomLoad()
		{
			this.performEvents = false;

			this.itemProbabilities = MainForm.SmkGame.ItemProbabilities;

			this.InitializeModeList();
			this.InitializeThemeList();
			this.InitializeLapRankList();
			this.InitializeItemBoxDisplayOptionList();

			this.InitializeProbability();
			this.DisplayProbability();
			this.DisplayItemIcons();

			this.performEvents = true;
		}

		private void InitializeModeList()
		{
			this.modeComboBox.BeginUpdate();
			this.modeComboBox.Items.Clear();

			string[] modeNames = MainForm.SmkGame.GetModeNames();
			foreach (string modeName in modeNames)
			{
				this.modeComboBox.Items.Add(modeName);
			}

			this.modeComboBox.EndUpdate();
			this.modeComboBox.SelectedIndex = 0;
		}

		private void InitializeThemeList()
		{
			this.themeComboBox.BeginUpdate();
			this.themeComboBox.Items.Clear();

			Themes themes = MainForm.SmkGame.Themes;
			for (int i = 0; i < themes.Count - 2; i++)
			{
				this.themeComboBox.Items.Add(themes[i].Name);
			}

			this.themeComboBox.Items.Add(themes[themes.Count - 2].Name + "/ " + themes[themes.Count - 1].Name);
			this.themeComboBox.EndUpdate();
			this.themeComboBox.SelectedIndex = 0;
		}

		private void InitializeLapRankList()
		{
			this.lapRankComboBox.BeginUpdate();
			this.lapRankComboBox.Items.Clear();

			switch (this.modeComboBox.SelectedIndex)
			{
				case 0:
					this.lapRankComboBox.Items.Add(GrandprixConditions.Lap1_1st);
					this.lapRankComboBox.Items.Add(GrandprixConditions.Lap2To5_2ndTo4th);
					this.lapRankComboBox.Items.Add(GrandprixConditions.Lap2To5_5thTo8th);
					this.lapRankComboBox.SelectedIndex = 0;
					break;

				case 1:
					this.lapRankComboBox.Items.Add(MatchRaceConditions.Lap1);
					this.lapRankComboBox.Items.Add(MatchRaceConditions.Lap2to5_1st);
					this.lapRankComboBox.Items.Add(MatchRaceConditions.Lap2to5_2nd);
					this.lapRankComboBox.SelectedIndex = 0;
					break;
			}

			this.lapRankComboBox.EndUpdate();
		}

		private void InitializeItemBoxDisplayOptionList()
		{
			this.itemBoxDisplayOption.BeginUpdate();
			this.itemBoxDisplayOption.Items.Clear();

			switch (this.modeComboBox.SelectedIndex)
			{
				case 0:
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoGhosts);
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoGhostsOrFeathers);
					break;

				case 1:
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.AllItems);
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoFeathers);
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoCoinsOrLightnings);
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoGhosts);
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoGhostsOrFeathers);
					break;

				case 2:
					this.itemBoxDisplayOption.Items.Add(ItemBoxDisplay.NoCoinsOrLightnings);
					break;
			}

			this.itemBoxDisplayOption.EndUpdate();
		}

		private void InitializeProbability()
		{
			switch (this.modeComboBox.SelectedIndex)
			{
				case 0: // GP
					this.itemProbability = this.itemProbabilities.GetGrandprixProbability(this.GetTheme(), (GrandprixConditions)this.lapRankComboBox.SelectedItem);
					if (this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhosts &&
						this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers)
					{
						this.itemProbability.DisplayedItems = ItemBoxDisplay.NoGhosts;
					}
					break;

				case 1: // Match Race
					this.itemProbability = this.itemProbabilities.GetMatchRaceProbability(this.GetTheme(), (MatchRaceConditions)this.lapRankComboBox.SelectedItem);
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
				this.coinsPictureBox.Enabled =
				this.coinsNumericUpDown.Enabled =
				this.coinsPctLabel.Enabled =
				this.itemProbability.DisplayedItems != ItemBoxDisplay.NoCoinsOrLightnings;

			this.featherLabel.Enabled =
				this.featherPictureBox.Enabled =
				this.featherNumericUpDown.Enabled =
				this.featherPctLabel.Enabled =
				this.itemProbability.DisplayedItems != ItemBoxDisplay.NoFeathers &&
				this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers;

			this.ghostLabel.Enabled =
				this.ghostPictureBox.Enabled =
				this.ghostNumericUpDown.Enabled =
				this.ghostPctLabel.Enabled =
				this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhosts &&
				this.itemProbability.DisplayedItems != ItemBoxDisplay.NoGhostsOrFeathers;

			this.lightningLabel.Enabled =
				this.lightningPictureBox.Enabled =
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
			this.itemBoxDisplayOption.SelectedItem = this.itemProbability.DisplayedItems;

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

		private void DisplayItemIcons()
		{
			this.mushroomPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Mushroom);
			this.featherPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Feather);
			this.starPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Star);
			this.bananaPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Banana);
			this.greenPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.GreenShell);
			this.redPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.RedShell);
			this.ghostPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Ghost);
			this.coinsPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Coin);
			this.lightningPictureBox.Image = MainForm.SmkGame.GetItemIcon(ItemType.Lightning);
		}

		#endregion Fields Initialization and Display

		#region Getters / Converters for the ComboBoxes

		private ItemProbaThemes GetTheme()
		{
			ItemProbaThemes ret;

			switch (this.themeComboBox.SelectedIndex)
			{
				case 0:
					ret = ItemProbaThemes.GhostValley;
					break;

				default:
				case 1:
					ret = ItemProbaThemes.MarioCircuit;
					break;

				case 2:
					ret = ItemProbaThemes.DonutPlains;
					break;

				case 3:
					ret = ItemProbaThemes.ChocoIsland;
					break;

				case 4:
					ret = ItemProbaThemes.VanillaLake;
					break;

				case 5:
					ret = ItemProbaThemes.KoopaBeach;
					break;

				case 6:
					ret = ItemProbaThemes.BowserCastleAndRainbowRoad;
					break;
			}

			return ret;
		}

		#endregion Getters / Converters for the ComboBoxes

		#region Events

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

			this.itemProbability.DisplayedItems = (ItemBoxDisplay)this.itemBoxDisplayOption.SelectedItem;

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

			this.InitializeLapRankList();
			this.InitializeItemBoxDisplayOptionList();
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

			this.itemBoxDisplayOption.Enabled = this.modeComboBox.SelectedIndex != 2;
			this.itemBoxDisplayOptionLabel.Enabled = this.modeComboBox.SelectedIndex != 2;

			this.InitializeProbability();
			this.DisplayProbability();

			this.performEvents = true;
		}

		private void lapRankComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.performEvents)
			{
				this.InitializeProbability();
				this.DisplayProbability();
			}
		}

		private void themeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.performEvents)
			{
				this.InitializeProbability();
				this.DisplayProbability();
			}
		}

		private void resetButton_Click(object sender, EventArgs e)
		{
			this.itemProbability.Reset();
			this.DisplayProbability();
		}

		#endregion Events
	}
}
