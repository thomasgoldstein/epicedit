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

namespace EpicEdit.Rom.Tracks.ItemProba
{
    #region Enumerations

    public enum ItemProbaThemes
    {
        MarioCircuit = 0,
        GhostValley = 1,
        DonutPlains = 2,
        BowserCastleAndRainbowRoad = 3,
        ChocoIsland = 4,
        KoopaBeach = 5,
        VanillaLake = 6
    }

    public enum GrandprixConditions
    {
        [Description("Lap 1 or 1st")]
        Lap1_1st = 1,
        [Description("Lap 2-5 / 2nd-4th")]
        Lap2To5_2ndTo4th = 0,
        [Description("Lap 2-5 / 5th-8th")]
        Lap2To5_5thTo8th = 2
    }

    public enum MatchRaceConditions
    {
        [Description("Lap 1")]
        Lap1 = 0,
        [Description("Lap 2-5 / 1st")]
        Lap2to5_1st = 1,
        [Description("Lap 2-5 / 2nd")]
        Lap2to5_2nd = 2
    }

    #endregion Enumerations

    /// <summary>
    /// Collection of <see cref="ItemProbability"/> objects.
    /// </summary>
    public class ItemProbabilities
    {
        // 7 different track types * 3 lap/rank = 21
        // 21 * 2 modes (GP and match race) = 42
        // 42 + 1 (battle mode) = 43
        private const int ProbabilityCount = 43;

        private ItemProbability[] itemProbabilities;

        public ItemProbabilities(byte[] romBuffer, int offset)
        {
            this.itemProbabilities = new ItemProbability[ItemProbabilities.ProbabilityCount];

            for (int itemProbabilityCount = 0; itemProbabilityCount < ItemProbabilities.ProbabilityCount; itemProbabilityCount++)
            {
                int address = offset + (itemProbabilityCount * ItemProbability.ProbabilityByteSize);
                this.itemProbabilities[itemProbabilityCount] = new ItemProbability(romBuffer, address);
            }
        }

        public void Save(byte[] romBuffer, int offset)
        {
            for (int itemProbabilityCount = 0; itemProbabilityCount < ItemProbabilities.ProbabilityCount; itemProbabilityCount++)
            {
                int address = offset + (itemProbabilityCount * ItemProbability.ProbabilityByteSize);
                this.itemProbabilities[itemProbabilityCount].Save(romBuffer, address);
            }
        }

        #region Get Single Item Probability

        public ItemProbability GetGrandprixProbability(ItemProbaThemes theme, GrandprixConditions condition)
        {
            int offset = (int)theme * 3 + (int)condition + (3 * 7);
            return this.itemProbabilities[offset];
        }

        public ItemProbability GetMatchRaceProbability(ItemProbaThemes theme, MatchRaceConditions condition)
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
