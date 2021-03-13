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

using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Represents a background previewer.
    /// </summary>
    internal class BackgroundPreviewer : EpicPanel
    {
        private BackgroundDrawer _drawer;

        [Browsable(false), DefaultValue(typeof(BackgroundDrawer), "")]
        public BackgroundDrawer Drawer
        {
            //get => this.drawer;
            set => _drawer = value;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_drawer == null)
            {
                return;
            }

            _drawer.DrawBackgroundPreview(e.Graphics);
        }

        public void SetFrame(int value)
        {
            if (_drawer == null)
            {
                return;
            }

            _drawer.PreviewFrame = value;
            Invalidate();
        }
    }
}
