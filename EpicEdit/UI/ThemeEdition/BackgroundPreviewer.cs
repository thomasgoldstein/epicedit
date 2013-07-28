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
using EpicEdit.Rom.Tracks;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Represents a background previewer.
    /// </summary>
    internal partial class BackgroundPreviewer : EpicPanel
    {
        private BackgroundDrawer drawer;

        [Browsable(false), DefaultValue(typeof(BackgroundDrawer), "")]
        public BackgroundDrawer Drawer
        {
            //get { return this.drawer; }
            set
            {
                this.drawer = value;

                this.repaintTimer = new Timer();
                this.repaintTimer.Interval = 30;
                this.repaintTimer.Tick += delegate
                {
                    this.Invalidate();
                    this.drawer.IncrementPreviewFrame();
                };
            }
        }

        private Timer repaintTimer;

        public BackgroundPreviewer()
        {
            this.InitializeComponent();
        }

        public bool Paused
        {
            get { return !this.repaintTimer.Enabled; }
        }

        public void Play()
        {
            this.repaintTimer.Start();
        }

        public void Pause()
        {
            this.repaintTimer.Stop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.drawer == null)
            {
                return;
            }

            this.drawer.DrawBackgroundPreview(e.Graphics);
        }
    }
}
