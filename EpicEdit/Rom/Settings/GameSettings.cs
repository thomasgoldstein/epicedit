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
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Regroups various game settings.
    /// </summary>
    internal class GameSettings
    {
        /// <summary>
        /// Gets the cup and theme names.
        /// </summary>
        public TextCollection CupAndThemeNames { get; private set; }

        /// <summary>
        /// Gets the game mode names.
        /// </summary>
        public TextCollection ModeNames { get; private set; }

        /// <summary>
        /// Gets the points awarded to drivers depending on their finishing position.
        /// </summary>
        public RankPoints RankPoints { get; private set; }

        /// <summary>
        /// Gets the item probabilities for all the tracks and race types.
        /// </summary>
        public ItemProbabilities ItemProbabilities { get; private set; }

        public bool Modified
        {
            get
            {
                return
                    this.CupAndThemeNames.Modified ||
                    this.ModeNames.Modified ||
                    this.RankPoints.Modified ||
                    this.ItemProbabilities.Modified;
            }
        }

        public GameSettings(byte[] romBuffer, Offsets offsets)
        {
            this.CupAndThemeNames = new TextCollection(romBuffer, offsets[Offset.CupAndThemeNames], Track.GroupCount + Theme.Count, 173, false);
            this.ModeNames = new TextCollection(romBuffer, offsets[Offset.ModeNames], offsets[Offset.ModeNames] + 6, 3, 66, true);

            byte[] rankPointsData = Utilities.ReadBlock(romBuffer, offsets[Offset.RankPoints], RankPoints.Size);
            this.RankPoints = new RankPoints(rankPointsData);

            byte[] itemProbaData = Utilities.ReadBlock(romBuffer, offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            this.ItemProbabilities = new ItemProbabilities(itemProbaData);
        }

        public void ResetModifiedState()
        {
            this.CupAndThemeNames.ResetModifiedState();
            this.ModeNames.ResetModifiedState();
            this.RankPoints.ResetModifiedState();
            this.ItemProbabilities.ResetModifiedState();
        }
    }
}
