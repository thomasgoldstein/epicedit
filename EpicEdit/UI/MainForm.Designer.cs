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

                if (MainForm.SmkGame != null)
                {
                    MainForm.SmkGame.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.trackTabPage = new System.Windows.Forms.TabPage();
            this.trackEditor = new EpicEdit.UI.TrackEdition.TrackEditor();
            this.themeTabPage = new System.Windows.Forms.TabPage();
            this.themeEditor = new EpicEdit.UI.ThemeEdition.ThemeEditor();
            this.tabControl.SuspendLayout();
            this.trackTabPage.SuspendLayout();
            this.themeTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.trackTabPage);
            this.tabControl.Controls.Add(this.themeTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(48, 3);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(854, 661);
            this.tabControl.TabIndex = 0;
            // 
            // trackTabPage
            // 
            this.trackTabPage.Controls.Add(this.trackEditor);
            this.trackTabPage.Location = new System.Drawing.Point(4, 22);
            this.trackTabPage.Name = "trackTabPage";
            this.trackTabPage.Size = new System.Drawing.Size(846, 635);
            this.trackTabPage.TabIndex = 0;
            this.trackTabPage.Text = "Track editor";
            this.trackTabPage.UseVisualStyleBackColor = true;
            // 
            // trackEditor
            // 
            this.trackEditor.AllowDrop = true;
            this.trackEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackEditor.Location = new System.Drawing.Point(0, 0);
            this.trackEditor.Name = "trackEditor";
            this.trackEditor.Size = new System.Drawing.Size(846, 635);
            this.trackEditor.TabIndex = 0;
            this.trackEditor.OpenRomDialogRequested += new System.EventHandler<System.EventArgs>(this.TrackEditorOpenRomDialogRequested);
            this.trackEditor.FileDragged += new System.EventHandler<EpicEdit.UI.Tools.EventArgs<string>>(this.TrackEditorFileDragged);
            this.trackEditor.ToggleScreenModeRequested += new System.EventHandler<System.EventArgs>(this.TrackEditorToggleScreenModeRequested);
            this.trackEditor.SaveRomDialogRequested += new System.EventHandler<System.EventArgs>(this.TrackEditorSaveRomDialogRequested);
            // 
            // themeTabPage
            // 
            this.themeTabPage.Controls.Add(this.themeEditor);
            this.themeTabPage.Location = new System.Drawing.Point(4, 22);
            this.themeTabPage.Name = "themeTabPage";
            this.themeTabPage.Size = new System.Drawing.Size(846, 635);
            this.themeTabPage.TabIndex = 1;
            this.themeTabPage.Text = "Theme editor";
            this.themeTabPage.UseVisualStyleBackColor = true;
            // 
            // themeEditor
            // 
            this.themeEditor.AllowDrop = true;
            this.themeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.themeEditor.Enabled = false;
            this.themeEditor.Location = new System.Drawing.Point(0, 0);
            this.themeEditor.Name = "themeEditor";
            this.themeEditor.Size = new System.Drawing.Size(846, 635);
            this.themeEditor.TabIndex = 2;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(854, 661);
            this.Controls.Add(this.tabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormFormClosing);
            this.tabControl.ResumeLayout(false);
            this.trackTabPage.ResumeLayout(false);
            this.themeTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.TabPage themeTabPage;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage trackTabPage;
        private EpicEdit.UI.TrackEdition.TrackEditor trackEditor;
        private EpicEdit.UI.ThemeEdition.ThemeEditor themeEditor;
    }
}
