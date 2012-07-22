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
    #region Enumerations

    internal enum ItemProbaTheme
    {
        MarioCircuit = 0,
        GhostValley = 1,
        DonutPlains = 2,
        BowserCastleAndRainbowRoad = 3,
        ChocoIsland = 4,
        KoopaBeach = 5,
        VanillaLake = 6
    }

    internal enum GrandprixCondition
    {
        [Description("Lap 1 or 1st")]
        Lap1_1st = 1,
        [Description("Lap 2-5 / 2nd-4th")]
        Lap2To5_2ndTo4th = 0,
        [Description("Lap 2-5 / 5th-8th")]
        Lap2To5_5thTo8th = 2
    }

    internal enum MatchRaceCondition
    {
        [Description("Lap 1")]
        Lap1 = 0,
        [Description("Lap 2-5 / 1st")]
        Lap2To5_1st = 1,
        [Description("Lap 2-5 / 2nd")]
        Lap2To5_2nd = 2
    }

    #endregion Enumerations

    /// <summary>
    /// Collection of <see cref="ItemProbability"/> objects.
    /// </summary>
    internal class ItemProbabilities
    {
        // The number of themes (Bowser Castle and Rainbow Road being mixed).
        private const int ThemeCount = 7;

        // The number of lap / rank combinations.
        private const int LapRankCount = 3;

        // The number of modes (GP and Match Race).
        private const int ModeCount = 2;

        private const int Count = ThemeCount * LapRankCount * ModeCount + 1; // + 1 for Battle Mode

        public const int Size = Count * ItemProbability.Size;

        private ItemProbability[] itemProbabilities;

        public ItemProbabilities(byte[] data)
        {
            this.itemProbabilities = new ItemProbability[ItemProbabilities.Count];

            for (int i = 0; i < ItemProbabilities.Count; i++)
            {
                byte[] itemData = ItemProbabilities.GetItemData(data, i);
                this.itemProbabilities[i] = new ItemProbability(itemData);
            }
        }

        public void SetBytes(byte[] data)
        {
            if (data.Length != ItemProbabilities.Size)
            {
                throw new ArgumentOutOfRangeException("data", "Item probabilities data should have a size of " + Size + " bytes. Actual: " + data.Length + " bytes.");
            }

            for (int i = 0; i < ItemProbabilities.Count; i++)
            {
                byte[] itemData = ItemProbabilities.GetItemData(data, i);
                this.itemProbabilities[i].SetBytes(itemData);
            }
        }

        private static byte[] GetItemData(byte[] data, int i)
        {
            int offset = i * ItemProbability.Size;
            byte[] itemData = new byte[ItemProbability.Size];
            return Utilities.ReadBlock(data, offset, ItemProbability.Size);
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[ItemProbabilities.Size];

            for (int i = 0; i < ItemProbabilities.Count; i++)
            {
                int index = i * ItemProbability.Size;
                this.itemProbabilities[i].GetBytes(data, index);
            }

            return data;
        }

        #region Get single item probability

        public ItemProbability GetGrandprixProbability(ItemProbaTheme theme, GrandprixCondition condition)
        {
            int offset = (int)theme * LapRankCount + (int)condition + (LapRankCount * ThemeCount);
            return this.itemProbabilities[offset];
        }

        public ItemProbability GetMatchRaceProbability(ItemProbaTheme theme, MatchRaceCondition condition)
        {
            int offset = (int)theme * LapRankCount + (int)condition;
            return this.itemProbabilities[offset];
        }

        public ItemProbability GetBattleModeProbability()
        {
            return this.itemProbabilities[ItemProbabilities.Count - 1];
        }

        #endregion Get single item probability

        public bool Modified
        {
            get
            {
                foreach (ItemProbability itemProba in this.itemProbabilities)
                {
                    if (itemProba.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void ResetModifiedFlag()
        {
            foreach (ItemProbability itemProba in this.itemProbabilities)
            {
                itemProba.ResetModifiedFlag();
            }
        }
    }
}
