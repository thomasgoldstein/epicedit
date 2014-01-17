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
using System.ComponentModel;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// The Form that contains the <see cref="PaletteEditor">palette editor</see>.
    /// </summary>
    internal partial class PaletteEditorForm : Form
    {
        [Browsable(true)]
        public event EventHandler<EventArgs> ColorChanged
        {
            add { this.Editor.ColorChanged += value; }
            remove { this.Editor.ColorChanged -= value; }
        }

        [Browsable(true)]
        public event EventHandler<EventArgs> ColorsChanged
        {
            add { this.Editor.ColorsChanged += value; }
            remove { this.Editor.ColorsChanged -= value; }
        }

        [Browsable(true)]
        public event EventHandler<EventArgs> PalettesChanged
        {
            add { this.Editor.PalettesChanged += value; }
            remove { this.Editor.PalettesChanged -= value; }
        }

        [Browsable(true)]
        public event EventHandler<EventArgs> ThemeBackColorChanged
        {
            add { this.Editor.ThemeBackColorChanged += value; }
            remove { this.Editor.ThemeBackColorChanged -= value; }
        }

        public PaletteEditorForm()
        {
            this.InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        public void Init()
        {
            this.Editor.Init();
        }
    }
}
