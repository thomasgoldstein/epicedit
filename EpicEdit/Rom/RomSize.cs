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
    /// The possible SNES ROM sizes.
    /// </summary>
    public static class RomSize
    {
        /// <summary>
        /// The smallest SNES ROM size possible: 256 KiB (2 megabits), and the step value between each possible ROM sizes.
        /// </summary>
        internal const int Size256 = 256 * 1024;

        /// <summary>
        /// The size of the original Super Mario Kart ROM: 512 KiB (4 megabits).
        /// </summary>
        internal const int Size512 = 512 * 1024;

        /// <summary>
        /// 768 KiB (6 megabits).
        /// </summary>
        internal const int Size768 = 768 * 1024;

        /// <summary>
        /// The limit up to which Epic Edit can write data: 1024 KiB (8 megabits).
        /// Epic Edit doesn't save data beyond 1024 KiB because offsets don't go beyond 0xFFFFF.
        /// </summary>
        internal const int Size1024 = 1024 * 1024;

        /// <summary>
        /// 16 megabits.
        /// </summary>
        internal const int Size2048 = 2048 * 1024;

        /// <summary>
        /// 32 megabits.
        /// </summary>
        internal const int Size4096 = 4096 * 1024;

        /*/// <summary>
        /// 48 megabits.
        /// </summary>
        internal const int Size6144 = 6144 * 1024;*/

        /// <summary>
        /// 64 megabits.
        /// </summary>
        internal const int Size8192 = 8192 * 1024;
    }
}
