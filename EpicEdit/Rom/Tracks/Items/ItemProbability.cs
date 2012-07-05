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
        public const int ProbabilitySize = 9;

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

        private int mushroom;
        public int Mushroom
        {
            get { return this.mushroom; }
            set
            {
                if (this.subTotal - this.mushroom + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.mushroom);
                }

                if (value != this.mushroom)
                {
                    this.Modified = true;
                    this.mushroom = value;
                }
            }
        }

        private int feather;
        public int Feather
        {
            get { return this.feather; }
            set
            {
                if (this.subTotal - this.feather + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.feather);
                }

                if (value != this.feather)
                {
                    this.Modified = true;
                    this.feather = value;
                }
            }
        }

        private int star;
        public int Star
        {
            get { return this.star; }
            set
            {
                if (this.subTotal - this.star + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.star);
                }

                if (value != this.star)
                {
                    this.Modified = true;
                    this.star = value;
                }
            }
        }

        private int banana;
        public int Banana
        {
            get { return this.banana; }
            set
            {
                if (this.subTotal - this.banana + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.banana);
                }

                if (value != this.banana)
                {
                    this.Modified = true;
                    this.banana = value;
                }
            }
        }

        private int green;
        public int Green
        {
            get { return this.green; }
            set
            {
                if (this.subTotal - this.green + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.green);
                }

                if (value != this.green)
                {
                    this.Modified = true;
                    this.green = value;
                }
            }
        }

        private int red;
        public int Red
        {
            get { return this.red; }
            set
            {
                if (this.subTotal - this.red + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.red);
                }

                if (value != this.red)
                {
                    this.Modified = true;
                    this.red = value;
                }
            }
        }

        private int ghost;
        public int Ghost
        {
            get { return this.ghost; }
            set
            {
                if (this.subTotal - this.ghost + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.ghost);
                }

                if (value != this.ghost)
                {
                    this.Modified = true;
                    this.ghost = value;
                }
            }
        }

        private int coins;
        public int Coins
        {
            get { return this.coins; }
            set
            {
                if (this.subTotal - this.coins + value > ItemProbability.TotalCount)
                {
                    value = ItemProbability.TotalCount - (this.subTotal - this.coins);
                }

                if (value != this.coins)
                {
                    this.Modified = true;
                    this.coins = value;
                }
            }
        }

        public int Lightning
        {
            get { return ItemProbability.TotalCount - this.subTotal; }
        }

        private int subTotal
        {
            get
            {
                return this.mushroom + this.feather + this.star + this.banana +
                    this.green + this.red + this.ghost + this.coins;
            }
        }

        public int Total
        {
            get { return this.subTotal + this.Lightning; }
        }

        #endregion Items

        public bool Modified { get; private set; }

        public ItemProbability(byte[] romBuffer, int offset)
        {
            this.backupData = new byte[ItemProbability.ProbabilitySize];
            Buffer.BlockCopy(romBuffer, offset, this.backupData, 0, ItemProbability.ProbabilitySize);

            this.Load(romBuffer, offset);
        }

        public void Reset()
        {
            this.Load(this.backupData, 0);
        }

        #region Reading and writing byte data

        public void Save(byte[] romBuffer, int offset)
        {
            int total = 0;

            if (this.mushroom == 0)
            {
                romBuffer[offset] = 0;
            }
            else
            {
                total += this.mushroom;
                romBuffer[offset] = (byte)total;
            }

            if (this.feather == 0)
            {
                romBuffer[offset + 1] = 0;
            }
            else
            {
                total += this.feather;
                romBuffer[offset + 1] = (byte)total;
            }

            if (this.star == 0)
            {
                romBuffer[offset + 2] = 0;
            }
            else
            {
                total += this.star;
                romBuffer[offset + 2] = (byte)total;
            }

            if (this.banana == 0)
            {
                romBuffer[offset + 3] = 0;
            }
            else
            {
                total += this.banana;
                romBuffer[offset + 3] = (byte)total;
            }

            if (this.green == 0)
            {
                romBuffer[offset + 4] = 0;
            }
            else
            {
                total += this.green;
                romBuffer[offset + 4] = (byte)total;
            }

            if (this.red == 0)
            {
                romBuffer[offset + 5] = 0;
            }
            else
            {
                total += this.red;
                romBuffer[offset + 5] = (byte)total;
            }

            if (this.ghost == 0)
            {
                romBuffer[offset + 6] = 0;
            }
            else
            {
                total += this.ghost;
                romBuffer[offset + 6] = (byte)total;
            }

            if (this.coins == 0)
            {
                romBuffer[offset + 7] = 0;
            }
            else
            {
                total += this.coins;
                romBuffer[offset + 7] = (byte)total;
            }

            romBuffer[offset + 8] = (byte)this.displayedItems;

            // Update the backup data, so that resetting the data will reload the last saved data
            Buffer.BlockCopy(romBuffer, offset, this.backupData, 0, ItemProbability.ProbabilitySize);

            this.Modified = false;
        }

        private void Load(byte[] romBuffer, int offset)
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

            this.Mushroom = romBuffer[offset];
            int total = this.mushroom;

            if (romBuffer[offset + 1] != 0)
            {
                this.Feather = romBuffer[offset + 1] - total;
                total += this.feather;
            }

            if (romBuffer[offset + 2] != 0)
            {
                this.Star = romBuffer[offset + 2] - total;
                total += this.star;
            }

            if (romBuffer[offset + 3] != 0)
            {
                this.Banana = romBuffer[offset + 3] - total;
                total += this.banana;
            }

            if (romBuffer[offset + 4] != 0)
            {
                this.Green = romBuffer[offset + 4] - total;
                total += this.green;
            }

            if (romBuffer[offset + 5] != 0)
            {
                this.Red = romBuffer[offset + 5] - total;
                total += this.red;
            }

            if (romBuffer[offset + 6] != 0)
            {
                this.Ghost = romBuffer[offset + 6] - total;
                total += this.ghost;
            }

            if (romBuffer[offset + 7] != 0)
            {
                this.Coins = romBuffer[offset + 7] - total;
            }

            this.displayedItems = (ItemBoxDisplay)romBuffer[offset + 8];

            this.SetProbsBasedOnDisplayedItems();

            this.Modified = false;
        }

        #endregion Reading and writing byte data
    }
}
