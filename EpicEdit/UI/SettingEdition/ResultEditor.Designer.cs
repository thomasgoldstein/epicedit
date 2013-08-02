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

namespace EpicEdit.UI.SettingEdition
{
    partial class ResultEditor
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the control.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.rankPointsControl = new EpicEdit.UI.SettingEdition.RankPointsControl();
            this.SuspendLayout();
            // 
            // rankPointsControl
            // 
            this.rankPointsControl.Location = new System.Drawing.Point(3, 3);
            this.rankPointsControl.Name = "rankPointsControl";
            this.rankPointsControl.Size = new System.Drawing.Size(122, 244);
            this.rankPointsControl.TabIndex = 0;
            // 
            // ResultEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rankPointsControl);
            this.Name = "ResultEditor";
            this.Size = new System.Drawing.Size(420, 250);
            this.ResumeLayout(false);
        }
        private EpicEdit.UI.SettingEdition.RankPointsControl rankPointsControl;
    }
}
