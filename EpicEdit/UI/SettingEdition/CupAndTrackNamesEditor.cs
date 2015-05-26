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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit the name of each cup and track.
    /// </summary>
    internal partial class CupAndTrackNamesEditor : UserControl
    {
        private Dictionary<SuffixedNameControl, SuffixedTextItem> controlDictionary;

        public CupAndTrackNamesEditor()
        {
            this.InitializeComponent();
            this.controlDictionary = new Dictionary<SuffixedNameControl, SuffixedTextItem>();
        }

        public void Init()
        {
            this.InitControls();
            this.UpdateCount();

            Context.Game.TracksReordered += delegate { this.InitControls(); };
        }

        private void InitControls()
        {
            this.controlDictionary.Clear();
            TrackGroups trackGroups = Context.Game.TrackGroups;

            this.controlDictionary.Add(this.cup1NameControl, trackGroups[0].SuffixedNameItem);
            this.controlDictionary.Add(this.cup2NameControl, trackGroups[1].SuffixedNameItem);
            this.controlDictionary.Add(this.cup3NameControl, trackGroups[2].SuffixedNameItem);
            this.controlDictionary.Add(this.cup4NameControl, trackGroups[3].SuffixedNameItem);

            this.controlDictionary.Add(this.cup1Track1NameControl, trackGroups[0][0].SuffixedNameItem);
            this.controlDictionary.Add(this.cup1Track2NameControl, trackGroups[0][1].SuffixedNameItem);
            this.controlDictionary.Add(this.cup1Track3NameControl, trackGroups[0][2].SuffixedNameItem);
            this.controlDictionary.Add(this.cup1Track4NameControl, trackGroups[0][3].SuffixedNameItem);
            this.controlDictionary.Add(this.cup1Track5NameControl, trackGroups[0][4].SuffixedNameItem);

            this.controlDictionary.Add(this.cup2Track1NameControl, trackGroups[1][0].SuffixedNameItem);
            this.controlDictionary.Add(this.cup2Track2NameControl, trackGroups[1][1].SuffixedNameItem);
            this.controlDictionary.Add(this.cup2Track3NameControl, trackGroups[1][2].SuffixedNameItem);
            this.controlDictionary.Add(this.cup2Track4NameControl, trackGroups[1][3].SuffixedNameItem);
            this.controlDictionary.Add(this.cup2Track5NameControl, trackGroups[1][4].SuffixedNameItem);

            this.controlDictionary.Add(this.cup3Track1NameControl, trackGroups[2][0].SuffixedNameItem);
            this.controlDictionary.Add(this.cup3Track2NameControl, trackGroups[2][1].SuffixedNameItem);
            this.controlDictionary.Add(this.cup3Track3NameControl, trackGroups[2][2].SuffixedNameItem);
            this.controlDictionary.Add(this.cup3Track4NameControl, trackGroups[2][3].SuffixedNameItem);
            this.controlDictionary.Add(this.cup3Track5NameControl, trackGroups[2][4].SuffixedNameItem);

            this.controlDictionary.Add(this.cup4Track1NameControl, trackGroups[3][0].SuffixedNameItem);
            this.controlDictionary.Add(this.cup4Track2NameControl, trackGroups[3][1].SuffixedNameItem);
            this.controlDictionary.Add(this.cup4Track3NameControl, trackGroups[3][2].SuffixedNameItem);
            this.controlDictionary.Add(this.cup4Track4NameControl, trackGroups[3][3].SuffixedNameItem);
            this.controlDictionary.Add(this.cup4Track5NameControl, trackGroups[3][4].SuffixedNameItem);

            this.controlDictionary.Add(this.battleTrack1NameControl, trackGroups[4][0].SuffixedNameItem);
            this.controlDictionary.Add(this.battleTrack2NameControl, trackGroups[4][1].SuffixedNameItem);
            this.controlDictionary.Add(this.battleTrack3NameControl, trackGroups[4][2].SuffixedNameItem);
            this.controlDictionary.Add(this.battleTrack4NameControl, trackGroups[4][3].SuffixedNameItem);

            foreach (KeyValuePair<SuffixedNameControl, SuffixedTextItem> kvp in this.controlDictionary)
            {
                kvp.Key.Init(kvp.Value);
            }
        }

        private void UpdateCount()
        {
            int total = Context.Game.Settings.CupAndTrackNameSuffixCollection.TotalCharacterCount;
            int max = Context.Game.Settings.CupAndTrackNameSuffixCollection.MaxCharacterCount;
            this.countLabel.Text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}", total, max);
            this.countLabel.ForeColor = total >= max ? Color.Red : SystemColors.ControlText;
        }

        private void NameControlSelectedNameChanged(object sender, EventArgs e)
        {
            SuffixedNameControl control = sender as SuffixedNameControl;
            this.controlDictionary[control].TextItem = control.SelectedTextItem;
        }

        private void NameControlSuffixTextChanged(object sender, EventArgs e)
        {
            this.UpdateCount();
        }
    }
}
