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

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// A theme editor.
    /// </summary>
    public partial class ThemeEditor : UserControl
    {
        public ThemeEditor()
        {
            this.InitializeComponent();
        }

        public void InitOnFirstRomLoad()
        {
            this.InitOnRomLoad();
            this.Enabled = true;
        }

        public void InitOnRomLoad()
        {
            this.paletteEditor.InitOnRomLoad();
            this.itemProbaControl.InitOnRomLoad();
        }
    }
}
