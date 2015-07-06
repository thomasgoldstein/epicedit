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

using System;
using System.Windows.Forms;
using EpicEdit.Properties;

namespace EpicEdit.UI.Tools
{
    internal static class EpicCursors
    {
        public static readonly Cursor PencilCursor;
        public static readonly Cursor BucketCursor;
        public static readonly Cursor ColorPickerCursor;

        static EpicCursors()
        {
            PencilCursor = GetCursor(Resources.PencilCursor, Cursors.Arrow);
            BucketCursor = GetCursor(Resources.BucketCursor, Cursors.Cross);
            ColorPickerCursor = GetCursor(Resources.ColorPickerCursor, Cursors.Hand);
        }

        private static Cursor GetCursor(Cursor cursor, Cursor replacementCursor)
        {
            // HACK: Mitigate the effects of Mono bug #749
            // https://bugzilla.xamarin.com/show_bug.cgi?id=749
            return !cursor.Size.IsEmpty ? cursor : replacementCursor;
        }
    }
}
