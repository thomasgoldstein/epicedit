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

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represents an interval defined by a <see cref="Start"/> and <see cref="End"/> value.
    /// </summary>
    internal struct Range
    {
        public static readonly Range Empty;

        private int start;
        private int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// Gets or sets the range start value.
        /// </summary>
        public int Start
        {
            get { return this.start; }
            set { this.start = value; }
        }

        /// <summary>
        /// Gets or sets the range end value.
        /// </summary>
        public int End
        {
            get { return this.end; }
            set { this.end = value; }
        }

        /// <summary>
        /// Gets or sets the range length.
        /// </summary>
        public int Length
        {
            get { return this.end - this.start; }
            set { this.end = this.start + value; }
        }

        public static bool operator ==(Range left, Range right)
        {
            return left.start == right.start &&
                left.end == right.end;
        }

        public static bool operator !=(Range left, Range right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            return obj is Range && this == (Range)obj;
        }

        public override int GetHashCode()
        {
            return this.start ^ this.end;
        }

        public bool Includes(int value)
        {
            return this.start <= value && value <= this.end;
        }
    }
}