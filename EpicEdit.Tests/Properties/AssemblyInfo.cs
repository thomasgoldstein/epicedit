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

using NUnit.Framework;
using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// Information about this assembly is defined by the following
// attributes.
//
// change them to the information which is associated with the assembly
// you compile.
[assembly: LevelOfParallelism(8)]
[assembly: Parallelizable(ParallelScope.All)]

[assembly: CLSCompliant(true)]
// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a type to COM, use [ComVisible(true)] on that type.
[assembly: ComVisible(false)]
