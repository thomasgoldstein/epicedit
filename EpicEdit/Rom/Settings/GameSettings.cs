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
        private Offsets offsets;

        /// <summary>
        /// Gets the game mode names.
        /// </summary>
        public TextCollection ModeNames { get; private set; }

        /// <summary>
        /// Gets the cup names displayed on the GP cup selection screen.
        /// </summary>
        public TextCollection GPCupNames { get; private set; }

        /// <summary>
        /// Gets the cup and theme names displayed in Time Trial.
        /// </summary>
        public TextCollection CupAndThemeNames { get; private set; }

        /// <summary>
        /// Driver names that appear on the GP result screen.
        /// </summary>
        public TextCollection DriverNamesGPResults { get; private set; }

        /// <summary>
        /// Driver names that appear on the GP podium screen.
        /// </summary>
        public TextCollection DriverNamesGPPodium { get; private set; }

        /// <summary>
        /// Driver names that appear in Time Trial.
        /// </summary>
        public TextCollection DriverNamesTimeTrial { get; private set; }

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
                    (this.GPCupNames != null && this.GPCupNames.Modified) ||
                    this.CupAndThemeNames.Modified ||
                    this.DriverNamesGPResults.Modified ||
                    this.DriverNamesGPPodium.Modified ||
                    this.DriverNamesTimeTrial.Modified ||
                    this.RankPoints.Modified ||
                    this.ItemProbabilities.Modified;
            }
        }

        public GameSettings(byte[] romBuffer, Offsets offsets, Region region)
        {
            this.offsets = offsets;

            bool isJap = region == Region.Jap;
            int[] nameDataSizes = isJap ?
                new int[] { 48, 94, 144, 136, 96, 42 } :
                new int[] { 66, 130, 173, 134, 112, 52 };

            char thinSpace = '\u2009';

            this.ModeNames = new TextCollection(
                romBuffer, offsets[Offset.ModeNames], 3,
                nameDataSizes[0], true, true, false, false, 0, null, null);

            if (!isJap)
            {
                // NOTE: GP cup names loading and editing is not supported for the Japanese ROM.
                // These texts are not extensible, as the characters are not reusable.
                // This is due to the fact characters are specific and split across tiles,
                // which makes it so they can only be modified properly by editing the tile graphics.
                this.GPCupNames = new TextCollection(
                    romBuffer, offsets[Offset.GPCupNames], 4,
                    nameDataSizes[1], true, false, false, true, (byte)0x80, null, null);
            }

            this.CupAndThemeNames = new TextCollection(
                romBuffer, offsets[Offset.CupAndThemeNames], Track.GroupCount + Theme.Count,
                nameDataSizes[2], false, false, false, false, 0,
                new byte[] { 0x2C }, new char[] { thinSpace });

            this.DriverNamesGPResults = new TextCollection(
                romBuffer, offsets[Offset.DriverNamesGPResults], 8,
                nameDataSizes[3], true, false, isJap, false, 0, null, null);

            this.DriverNamesGPPodium = new TextCollection(
                romBuffer, offsets[Offset.DriverNamesGPPodium], 8,
                nameDataSizes[4], true, false, false, false, (isJap ? (byte)0x60 : (byte)0x80),
                !isJap ? null : new byte[] { 0x8B, 0x8C }, !isJap ? null : new char[] { 'J', 'R' });

            this.DriverNamesTimeTrial = new TextCollection(
                romBuffer, offsets[Offset.DriverNamesTimeTrial], 8,
                nameDataSizes[5], false, false, false, false, 0,
                new byte[] { 0x2C }, new char[] { thinSpace });

            byte[] rankPointsData = Utilities.ReadBlock(romBuffer, offsets[Offset.RankPoints], RankPoints.Size);
            this.RankPoints = new RankPoints(rankPointsData);

            byte[] itemProbaData = Utilities.ReadBlock(romBuffer, offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            this.ItemProbabilities = new ItemProbabilities(itemProbaData);
        }

        public void Save(byte[] romBuffer)
        {
            if (this.GPCupNames != null)
            {
                this.GPCupNames.Save(romBuffer);
            }
            this.CupAndThemeNames.Save(romBuffer);
            this.DriverNamesGPResults.Save(romBuffer);
            this.DriverNamesGPPodium.Save(romBuffer);
            this.DriverNamesTimeTrial.Save(romBuffer);
            this.SaveRankPoints(romBuffer);
            this.SaveItemProbabilities(romBuffer);
        }

        private void SaveRankPoints(byte[] romBuffer)
        {
            if (this.RankPoints.Modified)
            {
                byte[] data = this.RankPoints.GetBytes();
                Buffer.BlockCopy(data, 0, romBuffer, this.offsets[Offset.RankPoints], RankPoints.Size);
            }
        }

        private void SaveItemProbabilities(byte[] romBuffer)
        {
            if (this.ItemProbabilities.Modified)
            {
                byte[] data = this.ItemProbabilities.GetBytes();
                Buffer.BlockCopy(data, 0, romBuffer, this.offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            }
        }

        public void ResetModifiedState()
        {
            this.CupAndThemeNames.ResetModifiedState();
            this.DriverNamesGPResults.ResetModifiedState();
            this.DriverNamesGPPodium.ResetModifiedState();
            this.DriverNamesTimeTrial.ResetModifiedState();
            this.RankPoints.ResetModifiedState();
            this.ItemProbabilities.ResetModifiedState();
        }
    }
}
