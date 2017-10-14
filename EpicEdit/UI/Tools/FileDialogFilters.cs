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
            "SNES ROM file (*.sfc, *.smc, *.swc, *.fig, *.zip)|" +
            "*.sfc; *.smc; *.swc; *.fig; *.zip|" +
            "All files (*.*)|*.*";

        public const string Rom =
            "SNES ROM file (*.sfc, *.smc, *.swc, *.fig)|" +
            "*{0}; *.sfc; *.smc; *.swc; *.fig|" +
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
            "SNES binary file (*.sfc, *.smc, *.swc, *.fig)|" +
            "*.sfc; *.smc; *.swc; *.fig";

        public const string Binary =
            "SNES binary file (*.sfc, *.smc, *.swc, *.fig)|" +
            "*.sfc; *.smc; *.swc; *.fig|" +
            "All files (*.*)|*.*";
    }
}
