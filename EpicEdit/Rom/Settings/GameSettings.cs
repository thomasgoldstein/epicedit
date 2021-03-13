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

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.Rom.Utility;
using System.ComponentModel;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Regroups various game settings.
    /// </summary>
    internal class GameSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Offsets _offsets;

        /// <summary>
        /// Gets the game mode names.
        /// </summary>
        public TextCollection ModeNames { get; private set; }

        /// <summary>
        /// Gets the cup texts displayed on the GP cup selection screen.
        /// </summary>
        public TextCollection GPCupSelectTexts { get; private set; }

        /// <summary>
        /// Gets the cup texts displayed on the GP results screen.
        /// </summary>
        public TextCollection GPResultsCupTexts { get; private set; }

        /// <summary>
        /// Gets the cup texts displayed on the GP podium screen.
        /// </summary>
        public TextCollection GPPodiumCupTexts { get; private set; }

        /// <summary>
        /// Gets the course select texts displayed in Time Trial, Match Race and Battle Mode.
        /// </summary>
        public TextCollection CourseSelectTexts { get; private set; }

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
                    (GPCupSelectTexts != null && GPCupSelectTexts.Modified) ||
                    GPResultsCupTexts.Modified ||
                    GPPodiumCupTexts.Modified ||
                    CourseSelectTexts.Modified ||
                    DriverNamesGPResults.Modified ||
                    DriverNamesGPPodium.Modified ||
                    DriverNamesTimeTrial.Modified ||
                    RankPoints.Modified ||
                    ItemProbabilities.Modified;
            }
        }

        public GameSettings(byte[] romBuffer, Offsets offsets, Region region)
        {
            _offsets = offsets;
            Init(romBuffer, region);
            HandleChanges();
        }

        private void Init(byte[] romBuffer, Region region)
        {
            bool isJap = region == Region.Jap;
            int[] textDataSizes = isJap ?
                new[] { 48, 94, 70, 68, 144, 136, 96, 42 } :
                new[] { 66, 130, 90, 80, 173, 134, 112, 52 };

            const char thinSpace = '\u2009';

            ModeNames = new TextCollection(
                romBuffer, _offsets[Offset.ModeNames], 3,
                textDataSizes[0], true, true, false, false, 0, null, null);

            if (!isJap)
            {
                // NOTE: GP cup names loading and editing is not supported for the Japanese ROM.
                // These texts are not extensible, as the characters are not reusable.
                // This is due to the fact characters are specific and split across tiles,
                // which makes it so they can only be modified properly by editing the tile graphics.
                GPCupSelectTexts = new TextCollection(
                    romBuffer, _offsets[Offset.GPCupSelectTexts], GPTrack.GroupCount,
                    textDataSizes[1], true, false, false, true, 0x80, null, null);
            }

            GPResultsCupTexts = new TextCollection(
                romBuffer, _offsets[Offset.GPResultsCupTexts], GPTrack.GroupCount,
                textDataSizes[2], true, false, false, false, 0, null, null);

            GPPodiumCupTexts = new GPPodiumCupTextCollection(
                romBuffer, _offsets[Offset.GPPodiumCupTexts], GPTrack.GroupCount + 1,
                textDataSizes[3], true, false, false, false,
                !isJap ? (byte)0x80 : (byte)0x60,
                !isJap ? new byte[] { 0xAD } : new byte[] { 0x8B, 0x8C, 0x8D, 0xFF },
                !isJap ? new[] { '\n' } : new[] { 'J', 'R', '\n', ' ' });

            CourseSelectTexts = new TextCollection(
                romBuffer, _offsets[Offset.CourseSelectTexts], Track.GroupCount + Theme.Count,
                textDataSizes[4], false, false, false, false, 0,
                new byte[] { 0x2C }, new[] { thinSpace });

            CupAndTrackNameSuffixCollection = new FreeTextCollection(
                CourseSelectTexts.Converter,
                SuffixedTextItem.MaxSuffixCharacterCount);

            DriverNamesGPResults = new TextCollection(
                romBuffer, _offsets[Offset.DriverNamesGPResults], 8,
                textDataSizes[5], true, false, isJap, false, 0, null, null);

            DriverNamesGPPodium = new TextCollection(
                romBuffer, _offsets[Offset.DriverNamesGPPodium], 8,
                textDataSizes[6], true, false, false, false,
                !isJap ? (byte)0x80 : (byte)0x60,
                !isJap ? new byte[] { 0xAD } : new byte[] { 0x8B, 0x8C, 0x8D, 0xFF },
                !isJap ? new[] { '\n' } : new[] { 'J', 'R', '\n', ' ' });

            DriverNamesTimeTrial = new TextCollection(
                romBuffer, _offsets[Offset.DriverNamesTimeTrial], 8,
                textDataSizes[7], false, false, false, false, 0,
                new byte[] { 0x2C }, new[] { thinSpace });

            byte[] rankPointsData = Utilities.ReadBlock(romBuffer, _offsets[Offset.RankPoints], RankPoints.Size);
            RankPoints = new RankPoints(rankPointsData);

            byte[] itemProbaData = Utilities.ReadBlock(romBuffer, _offsets[Offset.ItemProbabilities], ItemProbabilities.Size);
            ItemProbabilities = new ItemProbabilities(itemProbaData);
        }

        private void HandleChanges()
        {
            if (GPCupSelectTexts != null)
            {
                GPCupSelectTexts.PropertyChanged += OnPropertyChanged;
            }
            GPResultsCupTexts.PropertyChanged += OnPropertyChanged;
            GPPodiumCupTexts.PropertyChanged += OnPropertyChanged;
            CourseSelectTexts.PropertyChanged += OnPropertyChanged;
            DriverNamesGPResults.PropertyChanged += OnPropertyChanged;
            DriverNamesGPPodium.PropertyChanged += OnPropertyChanged;
            DriverNamesTimeTrial.PropertyChanged += OnPropertyChanged;
            RankPoints.PropertyChanged += OnPropertyChanged;
            ItemProbabilities.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public void Save(byte[] romBuffer)
        {
            if (GPCupSelectTexts != null)
            {
                GPCupSelectTexts.Save(romBuffer);
            }
            GPResultsCupTexts.Save(romBuffer);
            GPPodiumCupTexts.Save(romBuffer);
            CourseSelectTexts.Save(romBuffer);
            DriverNamesGPResults.Save(romBuffer);
            DriverNamesGPPodium.Save(romBuffer);
            DriverNamesTimeTrial.Save(romBuffer);
            RankPoints.Save(romBuffer, _offsets[Offset.RankPoints]);
            ItemProbabilities.Save(romBuffer, _offsets[Offset.ItemProbabilities]);
        }

        public void ResetModifiedState()
        {
            if (GPCupSelectTexts != null)
            {
                GPCupSelectTexts.ResetModifiedState();
            }
            GPResultsCupTexts.ResetModifiedState();
            GPPodiumCupTexts.ResetModifiedState();
            CourseSelectTexts.ResetModifiedState();
            DriverNamesGPResults.ResetModifiedState();
            DriverNamesGPPodium.ResetModifiedState();
            DriverNamesTimeTrial.ResetModifiedState();
            RankPoints.ResetModifiedState();
            ItemProbabilities.ResetModifiedState();
        }
    }
}
