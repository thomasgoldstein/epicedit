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
    internal static class FileDialogFilters
    {
        public const string RomOrZippedRom =
            "SNES ROM file (*.sfc, *.bin, *.fig, *.smc, *.swc, *.zip)|" +
            "*.sfc; *.bin; *.fig; *.smc; *.swc; *.zip|" +
            "All files (*.*)|*.*";

        public const string Rom =
            "SNES ROM file (*.sfc, *.bin, *.fig, *.smc, *.swc)|" +
            "*{0}; *.sfc; *.bin; *.fig; *.smc; *.swc|" +
            "All files (*.*)|*.*";

        public const string Track =
            "Full track (*.smkc)|*.smkc|" +
            "Track map only (*.mkt)|*.mkt|" +
            "All files (*.*)|*.*";

        public const string ColorPalette =
            "Color palettes (*.pal)|*.pal|" +
            "All files (*.*)|*.*";

        public const string ImageOrBinary =
            "PNG (*.png)|*.png|" +
            "BMP (*.bmp)|*.bmp|" +
            "Raw binary file (*.bin)|*.bin";

        public const string Binary =
            "Raw binary file (*.bin)|*.bin|" +
            "All files (*.*)|*.*";
    }
}
