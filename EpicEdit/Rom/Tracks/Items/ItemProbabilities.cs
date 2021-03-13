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

using EpicEdit.Rom.Utility;
using System;
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Items
{
    /// <summary>
    /// Collection of <see cref="ItemProbability"/> objects.
    /// </summary>
    internal class ItemProbabilities : INotifyPropertyChanged
    {
        /// <summary>
        /// The number of probability sets.
        /// </summary>
        public const int SetCount = 7;

        /// <summary>
        /// The number of lap / rank combinations.
        /// </summary>
        private const int LapRankCount = 3;

        /// <summary>
        /// The number of modes (GP and Match Race).
        /// </summary>
        private const int ModeCount = 2;

        private const int Count = SetCount * LapRankCount * ModeCount + 1; // + 1 for Battle Mode

        public const int Size = Count * ItemProbability.Size;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ItemProbability[] _itemProbabilities;

        public ItemProbabilities(byte[] data)
        {
            _itemProbabilities = new ItemProbability[Count];

            for (var i = 0; i < Count; i++)
            {
                var itemData = GetItemData(data, i);
                _itemProbabilities[i] = new ItemProbability(itemData);
                _itemProbabilities[i].PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void SetBytes(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException($"Item probabilities data should have a size of {Size} bytes. Actual: {data.Length} bytes.", nameof(data));
            }

            for (var i = 0; i < Count; i++)
            {
                var itemData = GetItemData(data, i);
                _itemProbabilities[i].SetBytes(itemData);
            }
        }

        private static byte[] GetItemData(byte[] data, int i)
        {
            var offset = i * ItemProbability.Size;
            var itemData = new byte[ItemProbability.Size];
            return Utilities.ReadBlock(data, offset, ItemProbability.Size);
        }

        public byte[] GetBytes()
        {
            var data = new byte[Size];

            for (var i = 0; i < Count; i++)
            {
                var index = i * ItemProbability.Size;
                _itemProbabilities[i].GetBytes(data, index);
            }

            return data;
        }

        #region Get single item probability

        public ItemProbability GetGrandprixProbability(int setIndex, ItemProbabilityGrandprixCondition condition)
        {
            setIndex = ConvertSetIndex(setIndex);
            var offset = setIndex * LapRankCount + (int)condition + (LapRankCount * SetCount);
            return _itemProbabilities[offset];
        }

        public ItemProbability GetMatchRaceProbability(int setIndex, ItemProbabilityMatchRaceCondition condition)
        {
            setIndex = ConvertSetIndex(setIndex);
            var offset = setIndex * LapRankCount + (int)condition;
            return _itemProbabilities[offset];
        }

        private static int ConvertSetIndex(int value)
        {
            // Item probability indexes need to be converted to retrieve the correct data.
            int[] reorder = { 1, 0, 2, 4, 6, 5, 3 };
            return reorder[value];
        }

        public ItemProbability GetBattleModeProbability()
        {
            return _itemProbabilities[Count - 1];
        }

        #endregion Get single item probability

        public bool Modified
        {
            get
            {
                foreach (var itemProba in _itemProbabilities)
                {
                    if (itemProba.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Save(byte[] romBuffer, int offset)
        {
            if (Modified)
            {
                var data = GetBytes();
                Buffer.BlockCopy(data, 0, romBuffer, offset, Size);
            }
        }

        public void ResetModifiedState()
        {
            foreach (var itemProba in _itemProbabilities)
            {
                itemProba.ResetModifiedState();
            }
        }
    }
}
