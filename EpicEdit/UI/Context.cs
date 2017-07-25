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

using EpicEdit.Rom;
using System.Windows.Forms;

namespace EpicEdit.UI
{
    /// <summary>
    /// The current context.
    /// </summary>
    internal static class Context
    {
        /// <summary>
        /// The Super Mario Kart Game.
        /// </summary>
        internal static Game Game;

        /// <summary>
        /// Gets or sets the control that, when visible, enables the color picker mode.
        /// </summary>
        internal static Control ColorPickerControl { get; set; }

        /// <summary>
        /// Gets value that determines if the color picker mode is currently enabled.
        /// </summary>
        internal static bool ColorPickerMode => Context.ColorPickerControl != null && Context.ColorPickerControl.Visible;
    }
}
