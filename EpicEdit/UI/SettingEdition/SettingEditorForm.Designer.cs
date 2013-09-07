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
    partial class SettingEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingEditorForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.cupThemeTab = new System.Windows.Forms.TabPage();
            this.resultsTabPage = new System.Windows.Forms.TabPage();
            this.resultEditor = new EpicEdit.UI.SettingEdition.ResultEditor();
            this.itemProbaTabPage = new System.Windows.Forms.TabPage();
            this.itemProbaEditor = new EpicEdit.UI.SettingEdition.ItemProbaEditor();
            this.tabImageList = new System.Windows.Forms.ImageList(this.components);
            this.cupAndThemeNamesEditor = new EpicEdit.UI.SettingEdition.CupAndThemeNamesEditor();
            this.tabControl.SuspendLayout();
            this.cupThemeTab.SuspendLayout();
            this.resultsTabPage.SuspendLayout();
            this.itemProbaTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.cupThemeTab);
            this.tabControl.Controls.Add(this.resultsTabPage);
            this.tabControl.Controls.Add(this.itemProbaTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.tabImageList;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(528, 282);
            this.tabControl.TabIndex = 0;
            // 
            // cupThemeTab
            // 
            this.cupThemeTab.Controls.Add(this.cupAndThemeNamesEditor);
            this.cupThemeTab.ImageKey = "CupThemeTab";
            this.cupThemeTab.Location = new System.Drawing.Point(4, 23);
            this.cupThemeTab.Name = "cupThemeTab";
            this.cupThemeTab.Size = new System.Drawing.Size(520, 255);
            this.cupThemeTab.TabIndex = 2;
            this.cupThemeTab.Text = "Cups & themes";
            this.cupThemeTab.UseVisualStyleBackColor = true;
            // 
            // resultsTabPage
            // 
            this.resultsTabPage.Controls.Add(this.resultEditor);
            this.resultsTabPage.ImageKey = "ResultsTab";
            this.resultsTabPage.Location = new System.Drawing.Point(4, 23);
            this.resultsTabPage.Name = "resultsTabPage";
            this.resultsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.resultsTabPage.Size = new System.Drawing.Size(520, 255);
            this.resultsTabPage.TabIndex = 1;
            this.resultsTabPage.Text = "Results";
            this.resultsTabPage.UseVisualStyleBackColor = true;
            // 
            // resultEditor
            // 
            this.resultEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultEditor.Location = new System.Drawing.Point(3, 3);
            this.resultEditor.Name = "resultEditor";
            this.resultEditor.Size = new System.Drawing.Size(514, 249);
            this.resultEditor.TabIndex = 0;
            // 
            // itemProbaTabPage
            // 
            this.itemProbaTabPage.Controls.Add(this.itemProbaEditor);
            this.itemProbaTabPage.ImageKey = "ItemProbaButton";
            this.itemProbaTabPage.Location = new System.Drawing.Point(4, 23);
            this.itemProbaTabPage.Name = "itemProbaTabPage";
            this.itemProbaTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.itemProbaTabPage.Size = new System.Drawing.Size(520, 255);
            this.itemProbaTabPage.TabIndex = 0;
            this.itemProbaTabPage.Text = "Item probabilities";
            this.itemProbaTabPage.UseVisualStyleBackColor = true;
            // 
            // itemProbaEditor
            // 
            this.itemProbaEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemProbaEditor.Location = new System.Drawing.Point(3, 3);
            this.itemProbaEditor.Name = "itemProbaEditor";
            this.itemProbaEditor.Size = new System.Drawing.Size(514, 249);
            this.itemProbaEditor.TabIndex = 0;
            // 
            // tabImageList
            // 
            this.tabImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabImageList.ImageStream")));
            this.tabImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.tabImageList.Images.SetKeyName(0, "ResultsTab");
            this.tabImageList.Images.SetKeyName(1, "CupThemeTab");
            // 
            // cupAndThemeNamesEditor
            // 
            this.cupAndThemeNamesEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cupAndThemeNamesEditor.Location = new System.Drawing.Point(0, 0);
            this.cupAndThemeNamesEditor.Name = "cupAndThemeNamesEditor";
            this.cupAndThemeNamesEditor.Size = new System.Drawing.Size(520, 255);
            this.cupAndThemeNamesEditor.TabIndex = 0;
            // 
            // SettingEditorForm
            // 
            this.ClientSize = new System.Drawing.Size(528, 282);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::EpicEdit.Properties.Resources.EpicEditIcon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingEditorForm";
            this.ShowInTaskbar = false;
            this.Text = "Game settings";
            this.tabControl.ResumeLayout(false);
            this.cupThemeTab.ResumeLayout(false);
            this.resultsTabPage.ResumeLayout(false);
            this.itemProbaTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        private EpicEdit.UI.SettingEdition.CupAndThemeNamesEditor cupAndThemeNamesEditor;
        private System.Windows.Forms.TabPage cupThemeTab;
        private EpicEdit.UI.SettingEdition.ResultEditor resultEditor;
        private System.Windows.Forms.ImageList tabImageList;

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage itemProbaTabPage;
        private System.Windows.Forms.TabPage resultsTabPage;
        private EpicEdit.UI.SettingEdition.ItemProbaEditor itemProbaEditor;
    }
}