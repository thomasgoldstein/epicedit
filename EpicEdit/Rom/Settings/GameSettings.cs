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
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Regroups various game settings.
    /// </summary>
    internal class GameSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Offsets offsets;

        /// <summary>
        /// Gets the game mode names.
        /// </summary>
        public TextCollection ModeNames { get; private set; }

        /// <summary>
        /// Gets the cup texts displayed on the GP cup selection screen.
        /// </summary>
        public TextCollection GPCupTexts { get; private set; }

        /// <summary>
        /// Gets the cup texts displayed on the GP podium screen.
        /// </summary>
        public TextCollection GPPodiumCupTexts { get; private set; }

        /// <summary>
        /// Gets the cup and theme texts displayed in Time Trial, Match Race and Battle Mode.
        /// </summary>
        public TextCollection CupAndThemeTexts { get; private set; }

        /// <summary>
        /// Gets the cup and track name suffixes.
        /// </summary>
        public FreeTextCollection CupAndTrackNameSuffixCollection { get; private set; }

        /// <summary>
        /// Gets the driver names that appear on the GP result screen.
        /// </summary>
        public TextCollection DriverNamesGPResults { get; private set; }

        /// <summary>
        /// Gets the driver names that appear on the GP podium screen.
        /// </summary>
        public TextCollection DriverNamesGPPodium { get; private set; }

        /// <summary>
        /// Gets the driver names that appear in Time Trial.
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
                    (this.GPCupTexts != null && this.GPCupTexts.Modified) ||
                    this.GPPodiumCupTexts.Modified ||
                    this.CupAndThemeTexts.Modified ||
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
            this.Init(romBuffer, region);
            this.HandleChanges();
        }

        private void Init(byte[] romBuffer, Region region)
        {
            bool isJap = region == Region.Jap;
            int[] nameDataSizes = isJap ?
                new [] { 48, 94, 68, 144, 136, 96, 42 } :
                new [] { 66, 130, 80, 173, 134, 112, 52 };

            const char ThinSpace = '\u2009';

            this.ModeNames = new TextCollection(
                romBuffer, offsets[Offset.ModeNames], 3,
                nameDataSizes[0], true, true, false, false, 0, null, null);

            if (!isJap)
            {
                // NOTE: GP cup names loading and editing is not supported for the Japanese ROM.
                // These texts are not extensible, as the characters are not reusable.
                // This is due to the fact characters are specific and split across tiles,
                // which makes it so they can only be modified properly by editing the tile graphics.
                this.GPCupTexts = new TextCollection(
                    romBuffer, offsets[Offset.GPCupTexts], GPTrack.GroupCount,
                    nameDataSizes[1], true, false, false, true, (byte)0x80, null, null);
            }

            this.GPPodiumCupTexts = new TextCollection(
                romBuffer, offsets[Offset.GPPodiumCupTexts], GPTrack.GroupCount + 1,
                nameDataSizes[2], true, false, false, false,
                !isJap ? (byte)0x80 : (byte)0x60,
                !isJap ? new byte[] { 0xAD } : new byte[] { 0x8B, 0x8C, 0x8D, 0xFF },
                !isJap ? new [] { '\n' } : new [] { 'J', 'R', '\n', ' ' });

            this.CupAndThemeTexts = new TextCollection(
                romBuffer, offsets[Offset.CupAndThemeTexts], Track.GroupCount + Theme.Count,
                nameDataSizes[3], false, false, false, false, 0,
                new byte[] { 0x2C }, new [] { ThinSpace });

            this.CupAndTrackNameSuffixCollection = new FreeTextCollection(
                this.CupAndThemeTexts.Converter,
                SuffixedTextItem.MaxSuffixCharacterCount);

            this.DriverNamesGPResults = new TextCollection(
                romBuffer, offsets[Offset.DriverNamesGPResults], 8,
                nameDataSizes[4], true, false, isJap, false, 0, null, null);

            this.DriverNamesGPPodium = new TextCollection(
                romBuffer, offsets[Offset.DriverNamesGPPodium], 8,
                nameDataSizes[5], true, false, false, false,
                !isJap ? (byte)0x80 : (byte)0x60,
                !isJap ? new byte[] { 0xAD } : new byte[] { 0x8B, 0x8C, 0x8D, 0xFF },
                !isJap ? new [] { '\n' } : new [] { 'J', 'R', '\n', ' ' });

            this.DriverNamesTimeTrial = new TextCollection(
                romBuffer, offsets[Offset.DriverNamesTimeTrial], 8,
                nameDataSizes[6], false, false, false, false, 0,
                new byte[] { 0x2C }, new [] { ThinSpace });

            byte[] rankPointsData = Utilities.ReadBlock(romBuffer, offsets[Offset.RankPoints], RankPoints.Size);
            this.RankPoints = new RankPoints(rankPointsData);

            byte[] itemProbaData = Utilities.ReadBlock(romBuffer, offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            this.ItemProbabilities = new ItemProbabilities(itemProbaData);
        }

        private void HandleChanges()
        {
            if (this.GPCupTexts != null)
            {
                this.GPCupTexts.PropertyChanged += this.OnPropertyChanged;
            }
            this.GPPodiumCupTexts.PropertyChanged += this.OnPropertyChanged;
            this.CupAndThemeTexts.PropertyChanged += this.OnPropertyChanged;
            this.DriverNamesGPResults.PropertyChanged += this.OnPropertyChanged;
            this.DriverNamesGPPodium.PropertyChanged += this.OnPropertyChanged;
            this.DriverNamesTimeTrial.PropertyChanged += this.OnPropertyChanged;
            this.RankPoints.PropertyChanged += this.OnPropertyChanged;
            this.ItemProbabilities.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(sender, e);
        }

        public void Save(byte[] romBuffer)
        {
            if (this.GPCupTexts != null)
            {
                this.GPCupTexts.Save(romBuffer);
            }
            this.GPPodiumCupTexts.Save(romBuffer);
            this.CupAndThemeTexts.Save(romBuffer);
            this.DriverNamesGPResults.Save(romBuffer);
            this.DriverNamesGPPodium.Save(romBuffer);
            this.DriverNamesTimeTrial.Save(romBuffer);
            this.RankPoints.Save(romBuffer, this.offsets[Offset.RankPoints]);
            this.ItemProbabilities.Save(romBuffer, this.offsets[Offset.ItemProbabilities]);
        }

        public void ResetModifiedState()
        {
            if (this.GPCupTexts != null)
            {
                this.GPCupTexts.ResetModifiedState();
            }
            this.GPPodiumCupTexts.ResetModifiedState();
            this.CupAndThemeTexts.ResetModifiedState();
            this.DriverNamesGPResults.ResetModifiedState();
            this.DriverNamesGPPodium.ResetModifiedState();
            this.DriverNamesTimeTrial.ResetModifiedState();
            this.RankPoints.ResetModifiedState();
            this.ItemProbabilities.ResetModifiedState();
        }
    }
}
