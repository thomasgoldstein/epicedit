using System;
using System.Globalization;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    internal class HexTextBox : NumericUpDown
    {
        public int HexValuePadding { get; set; }

        public HexTextBox()
        {
            Hexadecimal = true;

            // Remove the up/down arrows
            Controls.RemoveAt(0);
        }

        protected override void OnTextBoxResize(object source, EventArgs e)
        {
            // Adapt width to make up for the removed up/down arrows
            Controls[0].Width = Width - (SystemInformation.Border3DSize.Width * 2);
        }

        protected override void UpdateEditText()
        {
            // Pad the hex value as configured
            Text = ((int)Value).ToString("X" + HexValuePadding, CultureInfo.InvariantCulture);
        }
    }
}
