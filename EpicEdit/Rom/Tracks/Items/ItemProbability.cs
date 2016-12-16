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
    internal class ItemProbability : INotifyPropertyChanged
    {
        /// <summary>
        /// The amount of bytes a probability takes up.
        /// </summary>
        public const int Size = 9;

        /// <summary>
        /// The total number of item probability items.
        /// </summary>
        private const int TotalCount = 32;

        public event PropertyChangedEventHandler PropertyChanged;

        private byte[] backupData;

        #region Item box display

        private ItemBoxDisplay displayedItems;
        public ItemBoxDisplay DisplayedItems
        {
            get { return this.displayedItems; }
            set
            {
                if (this.displayedItems != value)
                {
                    this.displayedItems = value;
                    this.MarkAsModified(PropertyNames.ItemProbability.DisplayedItems);
                    this.SetProbsBasedOnDisplayedItems();
                }
            }
        }

        private void MarkAsModified(string propertyName)
        {
            this.Modified = true;
            this.OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                // NOTE: Dummy property name
                // We could pass the actual property name, but it's not used, so that'd complicate the code for nothing.
                this.MarkAsModified(PropertyNames.ItemProbability.FieldValue);
            }
        }

        public int Mushroom
        {
            get { return this.values[(int)ItemType.Mushroom]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.Mushroom], value); }
        }

        public int Feather
        {
            get { return this.values[(int)ItemType.Feather]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.Feather], value); }
        }

        public int Star
        {
            get { return this.values[(int)ItemType.Star]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.Star], value); }
        }

        public int Banana
        {
            get { return this.values[(int)ItemType.Banana]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.Banana], value); }
        }

        public int GreenShell
        {
            get { return this.values[(int)ItemType.GreenShell]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.GreenShell], value); }
        }

        public int RedShell
        {
            get { return this.values[(int)ItemType.RedShell]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.RedShell], value); }
        }

        public int Ghost
        {
            get { return this.values[(int)ItemType.Ghost]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.Ghost], value); }
        }

        public int Coins
        {
            get { return this.values[(int)ItemType.Coins]; }
            set { this.SetFieldValue(ref this.values[(int)ItemType.Coins], value); }
        }

        public int Lightning
        {
            get { return ItemProbability.TotalCount - this.SubTotal; }
        }

        private int SubTotal
        {
            get
            {
                int value = 0;

                for (int i = 0; i < this.values.Length; i++)
                {
                    value += this.values[i];
                }

                return value;
            }
        }

        public int Total
        {
            get { return this.SubTotal + this.Lightning; }
        }

        #endregion Items

        public bool Modified { get; private set; }

        private readonly int[] values;

        public ItemProbability(byte[] data)
        {
            this.backupData = data;
            this.values = new int[ItemProbability.Size - 1]; // Contains all the data except displayedItems
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
            // Init everything back to default to reset the SubTotal value
            for (int i = 0; i < this.values.Length; i++)
            {
                this.values[i] = 0;
            }

            int total = 0;

            for (int i = 0; i < this.values.Length; i++)
            {
                int value = ItemProbability.GetFieldValue(data, i, ref total);
                this.SetFieldValue(ref this.values[i], value);
            }

            this.DisplayedItems = (ItemBoxDisplay)data[8];
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

            for (int i = 0; i < this.values.Length; i++)
            {
                data[index + i] = this.values[i] == 0 ? (byte)0 : (byte)(total += this.values[i]);
            }

            data[index + this.values.Length] = (byte)this.displayedItems;
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
