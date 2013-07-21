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
using EpicEdit.Rom.Tracks.Items;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Regroups various game settings.
    /// </summary>
    internal class GameSettings
    {
        /// <summary>
        /// Gets the item probabilities for all the tracks and race types.
        /// </summary>
        public ItemProbabilities ItemProbabilities { get; private set; }

        /// <summary>
        /// Gets the points awarded to drivers depending on their finishing position.
        /// </summary>
        public RankPoints RankPoints { get; private set; }

        public bool Modified
        {
            get
            {
                return
                    this.ItemProbabilities.Modified ||
                    this.RankPoints.Modified;
            }
        }

        public GameSettings(byte[] romBuffer, Offsets offsets)
        {
            byte[] itemProbaData = Utilities.ReadBlock(romBuffer, offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            this.ItemProbabilities = new ItemProbabilities(itemProbaData);

            byte[] rankPointsData = Utilities.ReadBlock(romBuffer, offsets[Offset.RankPoints], RankPoints.Size);
            this.RankPoints = new RankPoints(rankPointsData);
        }

        public void ResetModifiedFlags()
        {
            this.ItemProbabilities.ResetModifiedFlag();
            this.RankPoints.ResetModifiedFlag();
        }
    }
}
