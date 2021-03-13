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

using EpicEdit.Rom.Settings;
using System;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="RankPoints"/>.
    /// </summary>
    internal partial class RankPointsControl : UserControl
    {
        private static RankPoints RankPoints => Context.Game.Settings.RankPoints;

        private bool _fireEvents;

        public RankPointsControl()
        {
            InitializeComponent();

            numericUpDown1.Tag = 0;
            numericUpDown2.Tag = 1;
            numericUpDown3.Tag = 2;
            numericUpDown4.Tag = 3;
            numericUpDown5.Tag = 4;
            numericUpDown6.Tag = 5;
            numericUpDown7.Tag = 6;
            numericUpDown8.Tag = 7;
        }

        public void Init()
        {
            _fireEvents = false;

            InitRankPoint(numericUpDown1);
            InitRankPoint(numericUpDown2);
            InitRankPoint(numericUpDown3);
            InitRankPoint(numericUpDown4);
            InitRankPoint(numericUpDown5);
            InitRankPoint(numericUpDown6);
            InitRankPoint(numericUpDown7);
            InitRankPoint(numericUpDown8);

            _fireEvents = true;
        }

        private static void InitRankPoint(NumericUpDown control)
        {
            int rank = (int)control.Tag;
            control.Value = Math.Min(RankPoints[rank], control.Maximum);
        }

        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!_fireEvents)
            {
                return;
            }

            NumericUpDown control = (NumericUpDown)sender;
            int rank = (int)control.Tag;
            int points = (int)control.Value;
            RankPoints[rank] = points;
        }
    }
}
