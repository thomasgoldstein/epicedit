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

using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Items
{
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

        private byte[] _backupData;

        #region Item box display

        private ItemBoxDisplay _displayedItems;
        public ItemBoxDisplay DisplayedItems
        {
            get => _displayedItems;
            set
            {
                if (_displayedItems != value)
                {
                    _displayedItems = value;
                    MarkAsModified(PropertyNames.ItemProbability.DisplayedItems);
                    SetProbsBasedOnDisplayedItems();
                }
            }
        }

        private void MarkAsModified(string propertyName)
        {
            Modified = true;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetProbsBasedOnDisplayedItems()
        {
            switch (_displayedItems)
            {
                case ItemBoxDisplay.NoCoinsOrLightnings:
                    Coins = 0;
                    // Arbitrarily affect Lightning value to Ghost, so that Lightning = 0
                    Ghost += Lightning;
                    break;
                case ItemBoxDisplay.NoFeathers:
                    Feather = 0;
                    break;
                case ItemBoxDisplay.NoGhosts:
                    Ghost = 0;
                    break;
                case ItemBoxDisplay.NoGhostsOrFeathers:
                    Ghost = 0;
                    Feather = 0;
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
            if (SubTotal - field + value > TotalCount)
            {
                value = TotalCount - (SubTotal - field);
            }

            if (field != value)
            {
                field = value;
                // We could pass the actual property name, but it's not used, so that'd complicate the code for nothing.
                MarkAsModified(PropertyNames.ItemProbability.FieldValue);
            }
        }

        public int Mushroom
        {
            get => _values[(int)ItemType.Mushroom];
            set => SetFieldValue(ref _values[(int)ItemType.Mushroom], value);
        }

        public int Feather
        {
            get => _values[(int)ItemType.Feather];
            set => SetFieldValue(ref _values[(int)ItemType.Feather], value);
        }

        public int Star
        {
            get => _values[(int)ItemType.Star];
            set => SetFieldValue(ref _values[(int)ItemType.Star], value);
        }

        public int Banana
        {
            get => _values[(int)ItemType.Banana];
            set => SetFieldValue(ref _values[(int)ItemType.Banana], value);
        }

        public int GreenShell
        {
            get => _values[(int)ItemType.GreenShell];
            set => SetFieldValue(ref _values[(int)ItemType.GreenShell], value);
        }

        public int RedShell
        {
            get => _values[(int)ItemType.RedShell];
            set => SetFieldValue(ref _values[(int)ItemType.RedShell], value);
        }

        public int Ghost
        {
            get => _values[(int)ItemType.Ghost];
            set => SetFieldValue(ref _values[(int)ItemType.Ghost], value);
        }

        public int Coins
        {
            get => _values[(int)ItemType.Coins];
            set => SetFieldValue(ref _values[(int)ItemType.Coins], value);
        }

        public int Lightning => TotalCount - SubTotal;

        private int SubTotal
        {
            get
            {
                var value = 0;

                for (var i = 0; i < _values.Length; i++)
                {
                    value += _values[i];
                }

                return value;
            }
        }

        public int Total => SubTotal + Lightning;

        #endregion Items

        public bool Modified { get; private set; }

        private readonly int[] _values;

        public ItemProbability(byte[] data)
        {
            _backupData = data;
            _values = new int[Size - 1]; // Contains all the data except displayedItems
            SetBytes(data);
            Modified = false;
        }

        public void Reset()
        {
            SetBytes(_backupData);
            Modified = false;
        }

        #region Reading and writing byte data

        public void SetBytes(byte[] data)
        {
            // Init everything back to default to reset the SubTotal value
            for (var i = 0; i < _values.Length; i++)
            {
                _values[i] = 0;
            }

            var total = 0;

            for (var i = 0; i < _values.Length; i++)
            {
                var value = GetFieldValue(data, i, ref total);
                SetFieldValue(ref _values[i], value);
            }

            DisplayedItems = (ItemBoxDisplay)data[8];
        }

        private static int GetFieldValue(byte[] data, int index, ref int total)
        {
            if (data[index] == 0)
            {
                return 0;
            }

            var value = data[index] - total;
            total += value;
            return value;
        }

        private byte[] GetBytes()
        {
            var data = new byte[Size];
            GetBytes(data, 0);
            return data;
        }

        public void GetBytes(byte[] data, int index)
        {
            var total = 0;

            for (var i = 0; i < _values.Length; i++)
            {
                data[index + i] = _values[i] == 0 ? (byte)0 : (byte)(total += _values[i]);
            }

            data[index + _values.Length] = (byte)_displayedItems;
        }

        public void ResetModifiedState()
        {
            // Update the backup data, so that resetting the data will reload the last saved data
            _backupData = GetBytes();
            Modified = false;
        }

        #endregion Reading and writing byte data
    }
}
