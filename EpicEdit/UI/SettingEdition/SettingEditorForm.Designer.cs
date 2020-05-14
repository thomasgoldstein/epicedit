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
            this.cupAndThemeTextsTab = new System.Windows.Forms.TabPage();
            this.cupAndThemeTextsEditor = new EpicEdit.UI.SettingEdition.CupAndThemeTextsEditor();
            this.courseSelectNamesTab = new System.Windows.Forms.TabPage();
            this.courseSelectNamesEditor = new EpicEdit.UI.SettingEdition.CourseSelectNamesEditor();
            this.resultsTabPage = new System.Windows.Forms.TabPage();
            this.resultEditor = new EpicEdit.UI.SettingEdition.ResultEditor();
            this.itemProbaTabPage = new System.Windows.Forms.TabPage();
            this.itemProbaEditor = new EpicEdit.UI.SettingEdition.ItemProbaEditor();
            this.tabImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl.SuspendLayout();
            this.cupAndThemeTextsTab.SuspendLayout();
            this.courseSelectNamesTab.SuspendLayout();
            this.resultsTabPage.SuspendLayout();
            this.itemProbaTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.cupAndThemeTextsTab);
            this.tabControl.Controls.Add(this.courseSelectNamesTab);
            this.tabControl.Controls.Add(this.resultsTabPage);
            this.tabControl.Controls.Add(this.itemProbaTabPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ImageList = this.tabImageList;
            this.tabControl.ItemSize = new System.Drawing.Size(28, 19);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(0, 3);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowToolTips = true;
            this.tabControl.Size = new System.Drawing.Size(528, 282);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            // 
            // cupAndThemeTextsTab
            // 
            this.cupAndThemeTextsTab.Controls.Add(this.cupAndThemeTextsEditor);
            this.cupAndThemeTextsTab.ImageKey = "CupAndThemeTextsTab";
            this.cupAndThemeTextsTab.Location = new System.Drawing.Point(4, 23);
            this.cupAndThemeTextsTab.Name = "cupAndThemeTextsTab";
            this.cupAndThemeTextsTab.Size = new System.Drawing.Size(520, 255);
            this.cupAndThemeTextsTab.TabIndex = 2;
            this.cupAndThemeTextsTab.ToolTipText = "Cups &&& themes";
            this.cupAndThemeTextsTab.UseVisualStyleBackColor = true;
            // 
            // cupAndThemeTextsEditor
            // 
            this.cupAndThemeTextsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cupAndThemeTextsEditor.Location = new System.Drawing.Point(0, 0);
            this.cupAndThemeTextsEditor.Name = "cupAndThemeTextsEditor";
            this.cupAndThemeTextsEditor.Size = new System.Drawing.Size(520, 255);
            this.cupAndThemeTextsEditor.TabIndex = 0;
            // 
            // courseSelectNamesTab
            // 
            this.courseSelectNamesTab.Controls.Add(this.courseSelectNamesEditor);
            this.courseSelectNamesTab.ImageKey = "CourseSelectNamesTab";
            this.courseSelectNamesTab.Location = new System.Drawing.Point(4, 23);
            this.courseSelectNamesTab.Name = "courseSelectNamesTab";
            this.courseSelectNamesTab.Size = new System.Drawing.Size(520, 255);
            this.courseSelectNamesTab.TabIndex = 3;
            this.courseSelectNamesTab.ToolTipText = "Course select names";
            this.courseSelectNamesTab.UseVisualStyleBackColor = true;
            // 
            // courseSelectNamesEditor
            // 
            this.courseSelectNamesEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.courseSelectNamesEditor.Location = new System.Drawing.Point(0, 0);
            this.courseSelectNamesEditor.Name = "courseSelectNamesEditor";
            this.courseSelectNamesEditor.Size = new System.Drawing.Size(520, 255);
            this.courseSelectNamesEditor.TabIndex = 0;
            // 
            // resultsTabPage
            // 
            this.resultsTabPage.Controls.Add(this.resultEditor);
            this.resultsTabPage.ImageKey = "ResultsTab";
            this.resultsTabPage.Location = new System.Drawing.Point(4, 23);
            this.resultsTabPage.Name = "resultsTabPage";
            this.resultsTabPage.Size = new System.Drawing.Size(520, 255);
            this.resultsTabPage.TabIndex = 1;
            this.resultsTabPage.ToolTipText = "Results";
            this.resultsTabPage.UseVisualStyleBackColor = true;
            // 
            // resultEditor
            // 
            this.resultEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultEditor.Location = new System.Drawing.Point(0, 0);
            this.resultEditor.Name = "resultEditor";
            this.resultEditor.Size = new System.Drawing.Size(520, 255);
            this.resultEditor.TabIndex = 0;
            // 
            // itemProbaTabPage
            // 
            this.itemProbaTabPage.Controls.Add(this.itemProbaEditor);
            this.itemProbaTabPage.ImageKey = "ItemProbaButton";
            this.itemProbaTabPage.Location = new System.Drawing.Point(4, 23);
            this.itemProbaTabPage.Name = "itemProbaTabPage";
            this.itemProbaTabPage.Size = new System.Drawing.Size(520, 255);
            this.itemProbaTabPage.TabIndex = 0;
            this.itemProbaTabPage.ToolTipText = "Item probabilities";
            this.itemProbaTabPage.UseVisualStyleBackColor = true;
            // 
            // itemProbaEditor
            // 
            this.itemProbaEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemProbaEditor.Location = new System.Drawing.Point(0, 0);
            this.itemProbaEditor.Name = "itemProbaEditor";
            this.itemProbaEditor.Size = new System.Drawing.Size(520, 255);
            this.itemProbaEditor.TabIndex = 0;
            // 
            // tabImageList
            // 
            this.tabImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabImageList.ImageStream")));
            this.tabImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.tabImageList.Images.SetKeyName(0, "CupAndThemeTextsTab");
            this.tabImageList.Images.SetKeyName(1, "CourseSelectNamesTab");
            this.tabImageList.Images.SetKeyName(2, "ResultsTab");
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
            this.cupAndThemeTextsTab.ResumeLayout(false);
            this.courseSelectNamesTab.ResumeLayout(false);
            this.resultsTabPage.ResumeLayout(false);
            this.itemProbaTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private EpicEdit.UI.SettingEdition.CupAndThemeTextsEditor cupAndThemeTextsEditor;
        private System.Windows.Forms.TabPage cupAndThemeTextsTab;
        private EpicEdit.UI.SettingEdition.ResultEditor resultEditor;
        private System.Windows.Forms.ImageList tabImageList;

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage itemProbaTabPage;
        private System.Windows.Forms.TabPage resultsTabPage;
        private EpicEdit.UI.SettingEdition.ItemProbaEditor itemProbaEditor;
        private System.Windows.Forms.TabPage courseSelectNamesTab;
        private EpicEdit.UI.SettingEdition.CourseSelectNamesEditor courseSelectNamesEditor;
    }
}