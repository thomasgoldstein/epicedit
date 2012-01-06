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
using System.IO;

namespace EpicEditTests
{
    /// <summary>
    /// File manipulation class used for unit testing.
    /// </summary>
    internal sealed class File
    {
        public readonly static string RelativePath =
            ".." + Path.DirectorySeparatorChar +
            ".." + Path.DirectorySeparatorChar +
            "files" + Path.DirectorySeparatorChar;

        private File() { }

        public static byte[] ReadFile(string filePath)
        {
            using (FileStream fs = new FileStream(File.RelativePath + filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(fs))
            {
                int size = (int)br.BaseStream.Length;
                byte[] buffer = br.ReadBytes(size);
                return buffer;
            }
        }

        public static byte[] ReadBlock(byte[] buffer, int offset, int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(buffer, offset, bytes, 0, length);
            return bytes;
        }

        public static void WriteFile(byte[] array, string fileName)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(array);
            }
        }
    }
}
