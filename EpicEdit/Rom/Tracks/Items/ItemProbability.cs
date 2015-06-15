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

namespace EpicEdit.Rom.Tracks.Items
{
    internal enum ItemBoxDisplay : byte
    {
        [Description("All items")]
        AllItems = 0x80,
        [Description("No feathers")]
        NoFeathers = 0x81,
        [Description("No coins or lightnings")]
        NoCoinsOrLightnings = 0x82,
        [Description("No ghosts")]
        NoGhosts = 0x83,
        [Description("No ghosts or feathers")]
        NoGhostsOrFeathers = 0x84
    }

    /// <summary>
    /// Represents the probability for players to get each item.
    /// </summary>
    internal class ItemProbability
    {
        /// <summary>
        /// The amount of bytes a probability takes up.
        /// </summary>
        public const int Size = 9;

        /// <summary>
        /// The total number of item probability items.
        /// </summary>
        private const int TotalCount = 32;

        private byte[] backupData;

        #region Item box display

        private ItemBoxDisplay displayedItems;
        public ItemBoxDisplay DisplayedItems
        {
            get { return this.displayedItems; }
            set
            {
                if (value != this.displayedItems)
                {
                    this.Modified = true;
                    this.displayedItems = value;
                    this.SetProbsBasedOnDisplayedItems();
                }
            }
        }

        private void SetProbsBasedOnDisplayedItems()
        {
            switch (this.displayedItems)
            {
                case ItemBoxDisplay.NoCoinsOrLightnings:
                    this.Coins = 0;
                    // Arbitrarily affect Lightning value to Ghost, so that Lightning = 0
                    this.Ghost += this.Lightning;
                    break;
                case ItemBoxDisplay.NoFeathers:
                    this.Feather = 0;
                    break;
                case ItemBoxDisplay.NoGhosts:
                    this.Ghost = 0;
                    break;
                case ItemBoxDisplay.NoGhostsOrFeathers:
                    this.Ghost = 0;
                    this.Feather = 0;
                    break;
                default:
                case ItemBoxDisplay.AllItems:
                    break;
            }
        }

        #endregion Item box display

        #region Items

        private void SetFieldValue(ref int field, int value)
        {
            if (this.SubTotal - field + value > ItemProbability.TotalCount)
            {
                value = ItemProbability.TotalCount - (this.SubTotal - field);
            }

            if (field != value)
            {
                field = value;
                this.Modified = true;
            }
        }

        private int mushroom;
        public int Mushroom
        {
            get { return this.mushroom; }
            set { this.SetFieldValue(ref this.mushroom, value); }
        }

        private int feather;
        public int Feather
        {
            get { return this.feather; }
            set { this.SetFieldValue(ref this.feather, value); }
        }

        private int star;
        public int Star
        {
            get { return this.star; }
            set { this.SetFieldValue(ref this.star, value); }
        }

        private int banana;
        public int Banana
        {
            get { return this.banana; }
            set { this.SetFieldValue(ref this.banana, value); }
        }

        private int green;
        public int Green
        {
            get { return this.green; }
            set { this.SetFieldValue(ref this.green, value); }
        }

        private int red;
        public int Red
        {
            get { return this.red; }
            set { this.SetFieldValue(ref this.red, value); }
        }

        private int ghost;
        public int Ghost
        {
            get { return this.ghost; }
            set { this.SetFieldValue(ref this.ghost, value); }
        }

        private int coins;
        public int Coins
        {
            get { return this.coins; }
            set { this.SetFieldValue(ref this.coins, value); }
        }

        public int Lightning
        {
            get { return ItemProbability.TotalCount - this.SubTotal; }
        }

        private int SubTotal
        {
            get
            {
                return this.mushroom + this.feather + this.star + this.banana +
                    this.green + this.red + this.ghost + this.coins;
            }
        }

        public int Total
        {
            get { return this.SubTotal + this.Lightning; }
        }

        #endregion Items

        public bool Modified { get; private set; }

        public ItemProbability(byte[] data)
        {
            this.backupData = data;
            this.SetBytes(data);
            this.Modified = false;
        }

        public void Reset()
        {
            this.SetBytes(this.backupData);
            this.Modified = false;
        }

        #region Reading and writing byte data

        public void SetBytes(byte[] data)
        {
            // Init everything back to default, this will help when calling reset
            this.mushroom = 0;
            this.feather = 0;
            this.star = 0;
            this.banana = 0;
            this.green = 0;
            this.red = 0;
            this.ghost = 0;
            this.coins = 0;
            this.displayedItems = ItemBoxDisplay.AllItems;

            int total = 0;

            this.Mushroom = ItemProbability.GetFieldValue(data, 0, ref total);
            this.Feather = ItemProbability.GetFieldValue(data, 1, ref total);
            this.Star = ItemProbability.GetFieldValue(data, 2, ref total);
            this.Banana = ItemProbability.GetFieldValue(data, 3, ref total);
            this.Green = ItemProbability.GetFieldValue(data, 4, ref total);
            this.Red = ItemProbability.GetFieldValue(data, 5, ref total);
            this.Ghost = ItemProbability.GetFieldValue(data, 6, ref total);
            this.Coins = ItemProbability.GetFieldValue(data, 7, ref total);

            this.displayedItems = (ItemBoxDisplay)data[8];

            this.SetProbsBasedOnDisplayedItems();

            this.Modified = true;
        }

        private static int GetFieldValue(byte[] data, int index, ref int total)
        {
            if (data[index] == 0)
            {
                return 0;
            }

            int value = data[index] - total;
            total += value;
            return value;
        }

        private byte[] GetBytes()
        {
            byte[] data = new byte[ItemProbability.Size];
            this.GetBytes(data, 0);
            return data;
        }

        public void GetBytes(byte[] data, int index)
        {
            int total = 0;
            data[index] = this.mushroom == 0 ? (byte)0 : (byte)(total += this.mushroom);
            data[index + 1] = this.feather == 0 ? (byte)0 : (byte)(total += this.feather);
            data[index + 2] = this.star == 0 ? (byte)0 : (byte)(total += this.star);
            data[index + 3] = this.banana == 0 ? (byte)0 : (byte)(total += this.banana);
            data[index + 4] = this.green == 0 ? (byte)0 : (byte)(total += this.green);
            data[index + 5] = this.red == 0 ? (byte)0 : (byte)(total += this.red);
            data[index + 6] = this.ghost == 0 ? (byte)0 : (byte)(total += this.ghost);
            data[index + 7] = this.coins == 0 ? (byte)0 : (byte)(total += this.coins);
            data[index + 8] = (byte)this.displayedItems;
        }

        public void ResetModifiedState()
        {
            // Update the backup data, so that resetting the data will reload the last saved data
            this.backupData = this.GetBytes();
            this.Modified = false;
        }

        #endregion Reading and writing byte data
    }
}
