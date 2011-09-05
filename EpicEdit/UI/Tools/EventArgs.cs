﻿#region GPL statement
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

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// Generic EventArgs.
    /// </summary>
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            this.value = value;
        }

        private T value;
        public T Value
        {
            get { return this.value; }
        }
    }
}
