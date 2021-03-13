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
        private RadioButton _selectedRadioButton;

        public RoadTilesetImportExportAction Action { get; private set; }

        public RoadTilesetImportExportType Type { get; private set; }

        public RoadTilesetImportExportForm()
        {
            InitializeComponent();
            tileGraphicsRadioButton.Select();
        }

        private void RadioButtonCheckedChanged(object sender, System.EventArgs e)
        {
            var rb = (RadioButton)sender;
            if (rb.Checked)
            {
                _selectedRadioButton = rb;
                SetButtonToolTips();
            }
        }

        private void SetButtonToolTips()
        {
            var label = _selectedRadioButton.Text;
            buttonToolTip.SetToolTip(importButton, "Import " + label);
            buttonToolTip.SetToolTip(exportButton, "Export " + label);
        }

        private void ImportButtonClick(object sender, System.EventArgs e)
        {
            Action = RoadTilesetImportExportAction.Import;
            SetResultAndClose();
        }

        private void ExportButtonClick(object sender, System.EventArgs e)
        {
            Action = RoadTilesetImportExportAction.Export;
            SetResultAndClose();
        }

        private void SetResultAndClose()
        {
            DialogResult = DialogResult.OK;
            Type = GetSelectedMode();
            Close();
        }

        private RoadTilesetImportExportType GetSelectedMode()
        {
            return
                _selectedRadioButton == tileGraphicsRadioButton ?
                RoadTilesetImportExportType.Graphics :
                _selectedRadioButton == tileGenresRadioButtons ?
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
