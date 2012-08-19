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

namespace EpicEdit.UI.Tools
{
    partial class CodecForm
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Disposes resources used by the form.
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
            this.Editor = new EpicEdit.UI.Tools.CodecControl();
            this.SuspendLayout();
            // 
            // Editor
            // 
            this.Editor.Location = new System.Drawing.Point(0, 0);
            this.Editor.Name = "Editor";
            this.Editor.Size = new System.Drawing.Size(170, 100);
            this.Editor.TabIndex = 0;
            // 
            // CodecForm
            // 
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(94, 72);
            this.Controls.Add(this.Editor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::EpicEdit.Properties.Resources.EpicEditIcon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CodecForm";
            this.ShowInTaskbar = false;
            this.Text = "Codec";
            this.ResumeLayout(false);
        }
        private EpicEdit.UI.Tools.CodecControl Editor;
    }
}
