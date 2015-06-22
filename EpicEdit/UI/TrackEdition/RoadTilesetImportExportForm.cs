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

using System.Windows.Forms;

namespace EpicEdit.UI.TrackEdition
{
    /// <summary>
    /// Form which contains road tileset export options.
    /// </summary>
    internal partial class RoadTilesetImportExportForm : Form
    {
        private RadioButton selectedRadioButton;

        public RoadTilesetImportExportAction Action { get; private set; }

        public RoadTilesetImportExportType Type { get; private set; }

        public RoadTilesetImportExportForm()
        {
            this.InitializeComponent();
            this.tileGraphicsRadioButton.Select();
        }

        private void RadioButtonCheckedChanged(object sender, System.EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                this.selectedRadioButton = rb;
                this.SetButtonToolTips();
            }
        }

        private void SetButtonToolTips()
        {
            string label = this.selectedRadioButton.Text;
            this.buttonToolTip.SetToolTip(this.importButton, "Import " + label);
            this.buttonToolTip.SetToolTip(this.exportButton, "Export " + label);
        }

        private void ImportButtonClick(object sender, System.EventArgs e)
        {
            this.Action = RoadTilesetImportExportAction.Import;
            this.SetResultAndClose();
        }

        private void ExportButtonClick(object sender, System.EventArgs e)
        {
            this.Action = RoadTilesetImportExportAction.Export;
            this.SetResultAndClose();
        }

        private void SetResultAndClose()
        {
            this.DialogResult = DialogResult.OK;
            this.Type = this.GetSelectedMode();
            this.Close();
        }

        private RoadTilesetImportExportType GetSelectedMode()
        {
            return
                this.selectedRadioButton == this.tileGraphicsRadioButton ?
                RoadTilesetImportExportType.Graphics :
                this.selectedRadioButton == this.tileGenresRadioButtons ?
                RoadTilesetImportExportType.Genres :
                RoadTilesetImportExportType.Palettes;
        }
    }

    internal enum RoadTilesetImportExportAction
    {
        Import,
        Export
    }

    internal enum RoadTilesetImportExportType
    {
        Graphics,
        Genres,
        Palettes
    }
}
