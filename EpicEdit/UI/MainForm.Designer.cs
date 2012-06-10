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

namespace EpicEdit.UI
{
    partial class MainForm
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                if (Context.Game != null)
                {
                    Context.Game.Dispose();
                }

                EpicEdit.UI.Gfx.TilesetHelper.Instance.Dispose();
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
            this.trackEditor = new EpicEdit.UI.TrackEdition.TrackEditor();
            this.SuspendLayout();
            // 
            // trackEditor
            // 
            this.trackEditor.AllowDrop = true;
            this.trackEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackEditor.Location = new System.Drawing.Point(0, 0);
            this.trackEditor.Name = "trackEditor";
            this.trackEditor.Size = new System.Drawing.Size(854, 716);
            this.trackEditor.TabIndex = 0;
            this.trackEditor.FileDragged += new System.EventHandler<EpicEdit.UI.Tools.EventArgs<string>>(this.TrackEditorFileDragged);
            this.trackEditor.OpenRomDialogRequested += new System.EventHandler<System.EventArgs>(this.TrackEditorOpenRomDialogRequested);
            this.trackEditor.SaveRomDialogRequested += new System.EventHandler<System.EventArgs>(this.TrackEditorSaveRomDialogRequested);
            this.trackEditor.ToggleScreenModeRequested += new System.EventHandler<System.EventArgs>(this.TrackEditorToggleScreenModeRequested);
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(854, 716);
            this.Controls.Add(this.trackEditor);
            this.Icon = global::EpicEdit.Properties.Resources.EpicEditIcon;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.ResumeLayout(false);
        }
        private EpicEdit.UI.TrackEdition.TrackEditor trackEditor;
    }
}
