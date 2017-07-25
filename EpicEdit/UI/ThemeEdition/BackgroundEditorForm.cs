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

using EpicEdit.Rom;
using EpicEdit.Rom.Utility;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// The Form that contains the <see cref="BackgroundEditor">background editor</see>.
    /// </summary>
    internal partial class BackgroundEditorForm : Form
    {
        /// <summary>
        /// Raised when a pixel color has been selected.
        /// </summary>
        [Browsable(true), Category("Behavior")]
        public event EventHandler<EventArgs<Palette, int>> ColorSelected
        {
            add => this.Editor.ColorSelected += value;
            remove => this.Editor.ColorSelected -= value;
        }

        public BackgroundEditorForm()
        {
            this.InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (!this.Visible)
            {
                this.Editor.PausePreview();
            }

            base.OnVisibleChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            this.Editor.ResetScrollPositions();
            e.Cancel = true;
        }

        public void Init()
        {
            this.Editor.Init();
        }
    }
}
