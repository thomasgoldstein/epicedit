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

namespace EpicEdit
{
    /// <summary>
    /// Returns information about the OS and the framework run by the user.
    /// </summary>
    internal static class Platform
    {
        /// <summary>
        /// Checks whether the framework used is Mono.
        /// </summary>
        /// <returns>True (Mono) or false (not Mono).</returns>
        public static bool IsMono
        {
            get { return Type.GetType("Mono.Runtime") != null; }
        }

        /// <summary>
        /// Checks whether the application is run on Windows.
        /// </summary>
        /// <returns>True (Windows) or false (not Windows).</returns>
        public static bool IsWindows
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT ||
                    Environment.OSVersion.Platform == PlatformID.Win32Windows;
            }
        }

        /// <summary>
        /// Checks whether the application is run on Linux, Unix or Mac.
        /// </summary>
        /// <returns>True (Unix) or false (not Unix).</returns>
        public static bool IsLinuxOrMac
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return p == 4 || p == 128;
            }
        }
    }
}
