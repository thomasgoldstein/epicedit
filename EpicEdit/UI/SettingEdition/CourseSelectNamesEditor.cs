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
            this.InitializeComponent();
        }

        public void Init()
        {
            this.InitControls();
            this.UpdateCount();

            Context.Game.TracksReordered += delegate { this.InitControls(); };
        }

        private void InitControls()
        {
            TrackGroups trackGroups = Context.Game.TrackGroups;

            this.cup1NameControl.Init(trackGroups[0].SuffixedNameItem);
            this.cup2NameControl.Init(trackGroups[1].SuffixedNameItem);
            this.cup3NameControl.Init(trackGroups[2].SuffixedNameItem);
            this.cup4NameControl.Init(trackGroups[3].SuffixedNameItem);

            this.cup1Track1NameControl.Init(trackGroups[0][0].SuffixedNameItem);
            this.cup1Track2NameControl.Init(trackGroups[0][1].SuffixedNameItem);
            this.cup1Track3NameControl.Init(trackGroups[0][2].SuffixedNameItem);
            this.cup1Track4NameControl.Init(trackGroups[0][3].SuffixedNameItem);
            this.cup1Track5NameControl.Init(trackGroups[0][4].SuffixedNameItem);

            this.cup2Track1NameControl.Init(trackGroups[1][0].SuffixedNameItem);
            this.cup2Track2NameControl.Init(trackGroups[1][1].SuffixedNameItem);
            this.cup2Track3NameControl.Init(trackGroups[1][2].SuffixedNameItem);
            this.cup2Track4NameControl.Init(trackGroups[1][3].SuffixedNameItem);
            this.cup2Track5NameControl.Init(trackGroups[1][4].SuffixedNameItem);

            this.cup3Track1NameControl.Init(trackGroups[2][0].SuffixedNameItem);
            this.cup3Track2NameControl.Init(trackGroups[2][1].SuffixedNameItem);
            this.cup3Track3NameControl.Init(trackGroups[2][2].SuffixedNameItem);
            this.cup3Track4NameControl.Init(trackGroups[2][3].SuffixedNameItem);
            this.cup3Track5NameControl.Init(trackGroups[2][4].SuffixedNameItem);

            this.cup4Track1NameControl.Init(trackGroups[3][0].SuffixedNameItem);
            this.cup4Track2NameControl.Init(trackGroups[3][1].SuffixedNameItem);
            this.cup4Track3NameControl.Init(trackGroups[3][2].SuffixedNameItem);
            this.cup4Track4NameControl.Init(trackGroups[3][3].SuffixedNameItem);
            this.cup4Track5NameControl.Init(trackGroups[3][4].SuffixedNameItem);

            this.battleTrack1NameControl.Init(trackGroups[4][0].SuffixedNameItem);
            this.battleTrack2NameControl.Init(trackGroups[4][1].SuffixedNameItem);
            this.battleTrack3NameControl.Init(trackGroups[4][2].SuffixedNameItem);
            this.battleTrack4NameControl.Init(trackGroups[4][3].SuffixedNameItem);
        }

        private void UpdateCount()
        {
            int total = Context.Game.Settings.CupAndTrackNameSuffixCollection.TotalCharacterCount;
            int max = Context.Game.Settings.CupAndTrackNameSuffixCollection.MaxCharacterCount;
            this.countLabel.Text = $"{total}/{max}";
            this.countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void NameControlSuffixTextChanged(object sender, EventArgs e)
        {
            this.UpdateCount();
        }
    }
}
