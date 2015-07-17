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

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// A TrackBar that directly jumps to the clicked location.
    /// </summary>
    internal class PlayerTrackBar : TrackBar
    {
        /// <summary>
        /// The horizontal distance between the track bar line edges and the control bounds.
        /// </summary>
        private const int LineHorizontalOffset = 10;

        public PlayerTrackBar()
        {
            this.SmallChange = 0;
            this.LargeChange = 0;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.JumpToValue(e.X);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                this.JumpToValue(e.X);
            }
        }

        private void JumpToValue(int x)
        {
            double xPercent = ((double)x / this.Width);
            int range = this.Maximum - this.Minimum;
            int lineOffsetCompensation = (int)(-LineHorizontalOffset * (0.5d - xPercent) * 4);
            int value = (int)(xPercent * range) + lineOffsetCompensation;

            if (value < this.Minimum)
            {
                value = this.Minimum;
            }
            else if (value > this.Maximum)
            {
                value = this.Maximum;
            }

            this.Value = value;
        }
    }
}
