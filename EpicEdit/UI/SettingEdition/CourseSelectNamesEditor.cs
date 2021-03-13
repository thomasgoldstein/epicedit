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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the names displayed on the Course Select screen.
    /// </summary>
    internal partial class CourseSelectNamesEditor : UserControl
    {
        public CourseSelectNamesEditor()
        {
            InitializeComponent();
        }

        public void Init()
        {
            InitControls();
            UpdateCount();

            Context.Game.TracksReordered += delegate { InitControls(); };
        }

        private void InitControls()
        {
            var trackGroups = Context.Game.TrackGroups;

            cup1NameControl.Init(trackGroups[0].SuffixedNameItem);
            cup2NameControl.Init(trackGroups[1].SuffixedNameItem);
            cup3NameControl.Init(trackGroups[2].SuffixedNameItem);
            cup4NameControl.Init(trackGroups[3].SuffixedNameItem);

            cup1Track1NameControl.Init(trackGroups[0][0].SuffixedNameItem);
            cup1Track2NameControl.Init(trackGroups[0][1].SuffixedNameItem);
            cup1Track3NameControl.Init(trackGroups[0][2].SuffixedNameItem);
            cup1Track4NameControl.Init(trackGroups[0][3].SuffixedNameItem);
            cup1Track5NameControl.Init(trackGroups[0][4].SuffixedNameItem);

            cup2Track1NameControl.Init(trackGroups[1][0].SuffixedNameItem);
            cup2Track2NameControl.Init(trackGroups[1][1].SuffixedNameItem);
            cup2Track3NameControl.Init(trackGroups[1][2].SuffixedNameItem);
            cup2Track4NameControl.Init(trackGroups[1][3].SuffixedNameItem);
            cup2Track5NameControl.Init(trackGroups[1][4].SuffixedNameItem);

            cup3Track1NameControl.Init(trackGroups[2][0].SuffixedNameItem);
            cup3Track2NameControl.Init(trackGroups[2][1].SuffixedNameItem);
            cup3Track3NameControl.Init(trackGroups[2][2].SuffixedNameItem);
            cup3Track4NameControl.Init(trackGroups[2][3].SuffixedNameItem);
            cup3Track5NameControl.Init(trackGroups[2][4].SuffixedNameItem);

            cup4Track1NameControl.Init(trackGroups[3][0].SuffixedNameItem);
            cup4Track2NameControl.Init(trackGroups[3][1].SuffixedNameItem);
            cup4Track3NameControl.Init(trackGroups[3][2].SuffixedNameItem);
            cup4Track4NameControl.Init(trackGroups[3][3].SuffixedNameItem);
            cup4Track5NameControl.Init(trackGroups[3][4].SuffixedNameItem);

            battleTrack1NameControl.Init(trackGroups[4][0].SuffixedNameItem);
            battleTrack2NameControl.Init(trackGroups[4][1].SuffixedNameItem);
            battleTrack3NameControl.Init(trackGroups[4][2].SuffixedNameItem);
            battleTrack4NameControl.Init(trackGroups[4][3].SuffixedNameItem);
        }

        private void UpdateCount()
        {
            var total = Context.Game.Settings.CupAndTrackNameSuffixCollection.TotalCharacterCount;
            var max = Context.Game.Settings.CupAndTrackNameSuffixCollection.MaxCharacterCount;
            countLabel.Text = $"{total}/{max}";
            countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void NameControlSuffixTextChanged(object sender, EventArgs e)
        {
            UpdateCount();
        }
    }
}
