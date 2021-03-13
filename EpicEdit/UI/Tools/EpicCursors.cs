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

using EpicEdit.Properties;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    internal static class EpicCursors
    {
        private static Cursor _pencilCursor;
        public static Cursor PencilCursor => _pencilCursor ?? (_pencilCursor = GetCursor(Resources.PencilCursor, Cursors.Arrow));

        private static Cursor _bucketCursor;
        public static Cursor BucketCursor => _bucketCursor ?? (_bucketCursor = GetCursor(Resources.BucketCursor, Cursors.Cross));

        private static Cursor _colorPickerCursor;
        public static Cursor ColorPickerCursor => _colorPickerCursor ?? (_colorPickerCursor = GetCursor(Resources.ColorPickerCursor, Cursors.Hand));

        private static Cursor GetCursor(Cursor cursor, Cursor replacementCursor)
        {
            // HACK: Mitigate the effects of Mono bug #749
            // https://bugzilla.xamarin.com/show_bug.cgi?id=749
            return !cursor.Size.IsEmpty ? cursor : replacementCursor;
        }
    }
}
