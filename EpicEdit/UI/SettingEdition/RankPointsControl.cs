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
using System.Windows.Forms;
using EpicEdit.Rom.Settings;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls to edit <see cref="RankPoints"/>.
    /// </summary>
    public partial class RankPointsControl : UserControl
    {
        private static RankPoints RankPoints
        {
            get { return Context.Game.Settings.RankPoints; }
        }

        private bool performEvents;

        public RankPointsControl()
        {
            this.InitializeComponent();

            this.numericUpDown1.Tag = 0;
            this.numericUpDown2.Tag = 1;
            this.numericUpDown3.Tag = 2;
            this.numericUpDown4.Tag = 3;
            this.numericUpDown5.Tag = 4;
            this.numericUpDown6.Tag = 5;
            this.numericUpDown7.Tag = 6;
            this.numericUpDown8.Tag = 7;
        }

        public void Init()
        {
            this.performEvents = false;

            RankPointsControl.InitRankPoint(this.numericUpDown1);
            RankPointsControl.InitRankPoint(this.numericUpDown2);
            RankPointsControl.InitRankPoint(this.numericUpDown3);
            RankPointsControl.InitRankPoint(this.numericUpDown4);
            RankPointsControl.InitRankPoint(this.numericUpDown5);
            RankPointsControl.InitRankPoint(this.numericUpDown6);
            RankPointsControl.InitRankPoint(this.numericUpDown7);
            RankPointsControl.InitRankPoint(this.numericUpDown8);

            this.performEvents = true;
        }

        private static void InitRankPoint(NumericUpDown control)
        {
            int rank = (int)control.Tag;
            control.Value = Math.Min(RankPointsControl.RankPoints[rank], control.Maximum);
        }

        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!this.performEvents)
            {
                return;
            }

            NumericUpDown control = sender as NumericUpDown;
            int rank = (int)control.Tag;
            int points = (int)control.Value;
            RankPointsControl.RankPoints[rank] = points;
        }
    }
}
