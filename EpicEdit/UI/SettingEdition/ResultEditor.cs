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
using System.Globalization;
using System.Windows.Forms;

using EpicEdit.Rom.Settings;

namespace EpicEdit.UI.SettingEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit race result-related settings.
    /// </summary>
    public partial class ResultEditor : UserControl
    {
        private RankPoints RankPoints
        {
            get { return Context.Game.Settings.RankPoints; }
        }

        bool performEvents;

        public ResultEditor()
        {
            this.InitializeComponent();
        }

        public void Init()
        {
            this.InitRankPoints();
        }

        private void InitRankPoints()
        {
            this.performEvents = false;

            this.InitRankPoint(this.numericUpDown1);
            this.InitRankPoint(this.numericUpDown2);
            this.InitRankPoint(this.numericUpDown3);
            this.InitRankPoint(this.numericUpDown4);
            this.InitRankPoint(this.numericUpDown5);
            this.InitRankPoint(this.numericUpDown6);
            this.InitRankPoint(this.numericUpDown7);
            this.InitRankPoint(this.numericUpDown8);

            this.performEvents = true;
        }

        private void InitRankPoint(NumericUpDown control)
        {
            int rank = Convert.ToInt32(control.Tag, CultureInfo.InvariantCulture);
            control.Value = Math.Min(this.RankPoints[rank], control.Maximum);
        }

        private void NumericUpDownValueChanged(object sender, EventArgs e)
        {
            if (!this.performEvents)
            {
                return;
            }

            NumericUpDown control = sender as NumericUpDown;
            int rank = Convert.ToInt32(control.Tag, CultureInfo.InvariantCulture);
            int points = (int)control.Value;
            this.RankPoints[rank] = points;
        }
    }
}
