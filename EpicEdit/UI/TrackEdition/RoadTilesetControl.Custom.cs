using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EpicEdit.UI.TrackEdition
{
    internal partial class RoadTilesetControl
    {
        public void SetTheme(int number)
        {
            var ud = this.tilePaletteNumericUpDown;
            if (number >= ud.Minimum && number <= ud.Maximum)
            {
                this.tilePaletteNumericUpDown.Value = number;
            }
        }
        public void SelectFloodFillTool()
        {
            this.bucketButton.PerformClick();
        }
        public void SelectPenTool()
        {
            this.pencilButton.PerformClick();
        }
    }
}
