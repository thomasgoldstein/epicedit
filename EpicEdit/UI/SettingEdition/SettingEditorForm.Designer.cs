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
            this.gpCupTextsTab = new System.Windows.Forms.TabPage();
            this.gpCupTextsEditor = new EpicEdit.UI.SettingEdition.GPCupTextsEditor();
            this.courseSelectTextsTab = new System.Windows.Forms.TabPage();
            this.courseSelectTextsEditor = new EpicEdit.UI.SettingEdition.CourseSelectTextsEditor();
            this.courseSelectNamesTab = new System.Windows.Forms.TabPage();
            this.courseSelectNamesEditor = new EpicEdit.UI.SettingEdition.CourseSelectNamesEditor();
            this.resultsTab = new System.Windows.Forms.TabPage();
            this.resultsEditor = new EpicEdit.UI.SettingEdition.ResultsEditor();
            this.itemProbaTab = new System.Windows.Forms.TabPage();
            this.itemProbaEditor = new EpicEdit.UI.SettingEdition.ItemProbaEditor();
            this.tabImageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControl.SuspendLayout();
            this.gpCupTextsTab.SuspendLayout();
            this.courseSelectTextsTab.SuspendLayout();
            this.courseSelectNamesTab.SuspendLayout();
            this.resultsTab.SuspendLayout();
            this.itemProbaTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.gpCupTextsTab);
            this.tabControl.Controls.Add(this.courseSelectTextsTab);
            this.tabControl.Controls.Add(this.courseSelectNamesTab);
            this.tabControl.Controls.Add(this.resultsTab);
            this.tabControl.Controls.Add(this.itemProbaTab);
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
            // gpCupTextsTab
            // 
            this.gpCupTextsTab.Controls.Add(this.gpCupTextsEditor);
            this.gpCupTextsTab.ImageKey = "GPCupTextsTab";
            this.gpCupTextsTab.Location = new System.Drawing.Point(4, 23);
            this.gpCupTextsTab.Name = "gpCupTextsTab";
            this.gpCupTextsTab.Size = new System.Drawing.Size(520, 255);
            this.gpCupTextsTab.TabIndex = 0;
            this.gpCupTextsTab.ToolTipText = "GP cup texts";
            this.gpCupTextsTab.UseVisualStyleBackColor = true;
            // 
            // gpCupTextsEditor
            // 
            this.gpCupTextsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gpCupTextsEditor.Location = new System.Drawing.Point(0, 0);
            this.gpCupTextsEditor.Name = "gpCupTextsEditor";
            this.gpCupTextsEditor.Size = new System.Drawing.Size(520, 255);
            this.gpCupTextsEditor.TabIndex = 0;
            // 
            // courseSelectTextsTab
            // 
            this.courseSelectTextsTab.Controls.Add(this.courseSelectTextsEditor);
            this.courseSelectTextsTab.ImageKey = "CourseSelectTextsTab";
            this.courseSelectTextsTab.Location = new System.Drawing.Point(4, 23);
            this.courseSelectTextsTab.Name = "courseSelectTextsTab";
            this.courseSelectTextsTab.Size = new System.Drawing.Size(520, 255);
            this.courseSelectTextsTab.TabIndex = 1;
            this.courseSelectTextsTab.ToolTipText = "Course select texts";
            this.courseSelectTextsTab.UseVisualStyleBackColor = true;
            // 
            // courseSelectTextsEditor
            // 
            this.courseSelectTextsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.courseSelectTextsEditor.Location = new System.Drawing.Point(0, 0);
            this.courseSelectTextsEditor.Name = "courseSelectTextsEditor";
            this.courseSelectTextsEditor.Size = new System.Drawing.Size(520, 255);
            this.courseSelectTextsEditor.TabIndex = 0;
            // 
            // courseSelectNamesTab
            // 
            this.courseSelectNamesTab.Controls.Add(this.courseSelectNamesEditor);
            this.courseSelectNamesTab.ImageKey = "CourseSelectNamesTab";
            this.courseSelectNamesTab.Location = new System.Drawing.Point(4, 23);
            this.courseSelectNamesTab.Name = "courseSelectNamesTab";
            this.courseSelectNamesTab.Size = new System.Drawing.Size(520, 255);
            this.courseSelectNamesTab.TabIndex = 2;
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
            // resultsTab
            // 
            this.resultsTab.Controls.Add(this.resultsEditor);
            this.resultsTab.ImageKey = "ResultsTab";
            this.resultsTab.Location = new System.Drawing.Point(4, 23);
            this.resultsTab.Name = "resultsTab";
            this.resultsTab.Size = new System.Drawing.Size(520, 255);
            this.resultsTab.TabIndex = 3;
            this.resultsTab.ToolTipText = "Results";
            this.resultsTab.UseVisualStyleBackColor = true;
            // 
            // resultsEditor
            // 
            this.resultsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsEditor.Location = new System.Drawing.Point(0, 0);
            this.resultsEditor.Name = "resultsEditor";
            this.resultsEditor.Size = new System.Drawing.Size(520, 255);
            this.resultsEditor.TabIndex = 0;
            // 
            // itemProbaTab
            // 
            this.itemProbaTab.Controls.Add(this.itemProbaEditor);
            this.itemProbaTab.ImageKey = "ItemProbaButton";
            this.itemProbaTab.Location = new System.Drawing.Point(4, 23);
            this.itemProbaTab.Name = "itemProbaTab";
            this.itemProbaTab.Size = new System.Drawing.Size(520, 255);
            this.itemProbaTab.TabIndex = 4;
            this.itemProbaTab.ToolTipText = "Item probabilities";
            this.itemProbaTab.UseVisualStyleBackColor = true;
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
            this.tabImageList.Images.SetKeyName(0, "GPCupTextsTab");
            this.tabImageList.Images.SetKeyName(1, "CourseSelectTextsTab");
            this.tabImageList.Images.SetKeyName(2, "CourseSelectNamesTab");
            this.tabImageList.Images.SetKeyName(3, "ResultsTab");
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
            this.gpCupTextsTab.ResumeLayout(false);
            this.courseSelectTextsTab.ResumeLayout(false);
            this.courseSelectNamesTab.ResumeLayout(false);
            this.resultsTab.ResumeLayout(false);
            this.itemProbaTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private EpicEdit.UI.SettingEdition.GPCupTextsEditor gpCupTextsEditor;
        private System.Windows.Forms.TabPage gpCupTextsTab;
        private EpicEdit.UI.SettingEdition.ResultsEditor resultsEditor;
        private System.Windows.Forms.ImageList tabImageList;

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage itemProbaTab;
        private System.Windows.Forms.TabPage resultsTab;
        private EpicEdit.UI.SettingEdition.ItemProbaEditor itemProbaEditor;
        private System.Windows.Forms.TabPage courseSelectNamesTab;
        private EpicEdit.UI.SettingEdition.CourseSelectNamesEditor courseSelectNamesEditor;
        private System.Windows.Forms.TabPage courseSelectTextsTab;
        private CourseSelectTextsEditor courseSelectTextsEditor;
    }
}