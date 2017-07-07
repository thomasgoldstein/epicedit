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

namespace EpicEdit.Rom.Compression
{
    internal interface IDecompressor
    {
        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <returns>The decompressed data.</returns>
        byte[] Decompress(byte[] buffer);

        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <returns>The decompressed data.</returns>
        byte[] Decompress(byte[] buffer, int offset);

        /// <summary>
        /// Decompresses data until a certain size is reached.
        /// Specifying the length improves performances and makes it possible to decompress corrupt data.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <param name="length">Defines the length of the returned buffer.</param>
        /// <returns>The decompressed data.</returns>
        byte[] Decompress(byte[] buffer, int offset, int length);

        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <param name="twice">Decompress the data twice.</param>
        /// <returns>The decompressed data.</returns>
        byte[] Decompress(byte[] buffer, int offset, bool twice);
    }
}
