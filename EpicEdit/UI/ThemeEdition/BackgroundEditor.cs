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
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.ThemeEdition
{
    /// <summary>
    /// Represents a collection of controls used to edit backgrounds.
    /// </summary>
    internal partial class BackgroundEditor : UserControl
    {
        private Theme Theme
        {
            get { return this.themeComboBox.SelectedItem as Theme; }
        }

        public BackgroundEditor()
        {
            this.InitializeComponent();
        }

        public void Init()
        {
            this.themeComboBox.BeginUpdate();
            this.themeComboBox.Items.Clear();
            foreach (Theme theme in Context.Game.Themes)
            {
                this.themeComboBox.Items.Add(theme);
            }
            this.themeComboBox.EndUpdate();
            this.themeComboBox.SelectedIndex = 0;
        }
        
        private void ThemeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            this.backgroundPreviewer.LoadBackground(this.Theme.Background);
        }
        
        private void PlayPauseButtonClick(object sender, EventArgs e)
        {
            if (this.backgroundPreviewer.Paused)
            {
                this.backgroundPreviewer.Play();
                this.playPauseButton.Text = "Pause";
            }
            else
            {                
                this.backgroundPreviewer.Pause();
                this.playPauseButton.Text = "Play";
            }
        }
    }
}
