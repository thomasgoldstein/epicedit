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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EpicEdit.Rom.Tracks.Items
{
    #region Enumerations

    public enum ItemProbaTheme
    {
        MarioCircuit = 0,
        GhostValley = 1,
        DonutPlains = 2,
        BowserCastleAndRainbowRoad = 3,
        ChocoIsland = 4,
        KoopaBeach = 5,
        VanillaLake = 6
    }

    public enum GrandprixCondition
    {
        [Description("Lap 1 or 1st")]
        Lap1_1st = 1,
        [Description("Lap 2-5 / 2nd-4th")]
        Lap2To5_2ndTo4th = 0,
        [Description("Lap 2-5 / 5th-8th")]
        Lap2To5_5thTo8th = 2
    }

    public enum MatchRaceCondition
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
    public class ItemProbabilities
    {
        // 7 different themes * 3 lap/rank = 21
        // 21 * 2 modes (GP and Match Race) = 42
        // 42 + 1 (Battle Mode) = 43
        private const int ProbabilityCount = 43;

        private ItemProbability[] itemProbabilities;

        public ItemProbabilities(byte[] romBuffer, int offset)
        {
            this.itemProbabilities = new ItemProbability[ItemProbabilities.ProbabilityCount];

            for (int i = 0; i < ItemProbabilities.ProbabilityCount; i++)
            {
                int address = offset + (i * ItemProbability.ProbabilityByteSize);
                this.itemProbabilities[i] = new ItemProbability(romBuffer, address);
            }
        }

        public void Save(byte[] romBuffer, int offset)
        {
            for (int i = 0; i < ItemProbabilities.ProbabilityCount; i++)
            {
                int address = offset + (i * ItemProbability.ProbabilityByteSize);
                this.itemProbabilities[i].Save(romBuffer, address);
            }
        }

        #region Get Single Item Probability

        public ItemProbability GetGrandprixProbability(ItemProbaTheme theme, GrandprixCondition condition)
        {
            int offset = (int)theme * 3 + (int)condition + (3 * 7);
            return this.itemProbabilities[offset];
        }

        public ItemProbability GetMatchRaceProbability(ItemProbaTheme theme, MatchRaceCondition condition)
        {
            int offset = (int)theme * 3 + (int)condition;
            return this.itemProbabilities[offset];
        }

        public ItemProbability GetBattleModeProbability()
        {
            return this.itemProbabilities[ItemProbabilities.ProbabilityCount - 1];
        }

        #endregion Get Single Item Probability

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
    }
}
