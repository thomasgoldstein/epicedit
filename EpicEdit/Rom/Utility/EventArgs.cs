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

namespace EpicEdit.Rom.Utility
{
    /// <summary>
    /// EventArgs that take one generic parameter.
    /// </summary>
    internal class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            this.Value = value;
        }

        public T Value { get; }
    }

    /// <summary>
    /// EventArgs that take two generic parameters.
    /// </summary>
    internal class EventArgs<T, U> : EventArgs
    {
        public EventArgs(T value1, U value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }

        public T Value1 { get; }

        public U Value2 { get; }
    }
}
