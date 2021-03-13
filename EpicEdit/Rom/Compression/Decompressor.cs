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

using EpicEdit.Rom.Utility;
using System;
using System.IO;

namespace EpicEdit.Rom.Compression
{
    internal class Decompressor : IDecompressor
    {
        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <returns>The decompressed data.</returns>
        public byte[] Decompress(byte[] buffer)
        {
            return Decompress(buffer, 0);
        }

        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <returns>The decompressed data.</returns>
        public byte[] Decompress(byte[] buffer, int offset)
        {
            var destBuffer = new byte[Codec.BufferSize];
            var destPosition = 0;
            byte value;

            try
            {
                while ((value = buffer[offset++]) != 0xFF)
                {
                    Decompress(buffer, destBuffer, value, true, ref offset, ref destPosition);
                }
            }
            catch (IndexOutOfRangeException)
            {
                destPosition--; // Buffer limit reached
            }

            return Utilities.ReadBlock(destBuffer, 0, destPosition);
        }

        /// <summary>
        /// Decompresses data until a certain size is reached.
        /// Specifying the length improves performances and makes it possible to decompress corrupt data.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <param name="length">Defines the length of the returned buffer.</param>
        /// <returns>The decompressed data.</returns>
        public byte[] Decompress(byte[] buffer, int offset, int length)
        {
            var destBuffer = new byte[length];
            var destPosition = 0;

            try
            {
                while (destPosition < length)
                {
                    var value = buffer[offset++];
                    Decompress(buffer, destBuffer, value, false, ref offset, ref destPosition);
                }
            }
            catch (IndexOutOfRangeException) { }

            return destBuffer;
        }

        private void Decompress(byte[] srcBuffer, byte[] destBuffer, byte value, bool throwOnInvalidData, ref int srcOffset, ref int destOffset)
        {
            var command = (byte)((value & 0xE0) >> 5);
            var i = 0;
            int count;
            byte xor = 0x00;

            if (command == 7) // This special command extends the byte count.
            {
                command = (byte)((value & 0x1C) >> 2);
                count = ((value & 3) << 8) + srcBuffer[srcOffset++];
            }
            else
            {
                count = value & 0x1F;
            }
            count++;

            try
            {
                switch (command)
                {
                    case 0: // Reads an amount of bytes from ROM in sequence stores them in the same sequence.
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = srcBuffer[srcOffset++];
                        }
                        break;

                    case 1: // Reads one byte from ROM and continuously stores the same byte in sequence.
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = srcBuffer[srcOffset];
                        }
                        srcOffset++;
                        break;

                    case 2: // Reads two bytes from ROM and continously stores the same two bytes in sequence. 
                        var j = 0;
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = srcBuffer[srcOffset + j];
                            j = 1 - j;
                        }
                        srcOffset += 2;
                        break;

                    case 3: // Reads one byte from ROM and continously stores it, but the byte to write is incremented after every write.
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = (byte)((srcBuffer[srcOffset] + i) & 0xFF);
                        }
                        srcOffset++;
                        break;

                    case 4: // Reads two bytes consisting of a pointer to a previously written address. Bytes are sequentially read from the supplied reading address and stored in sequence to the target address.
                        var prevOffset = srcBuffer[srcOffset++] + (srcBuffer[srcOffset++] << 8);
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = destBuffer[prevOffset + i];
                        }
                        break;

                    case 5: // Identical to 4, although every byte read is inverted (EOR #$FF) before it is stored. This command doesn't see much use.
                        xor = 0xFF;
                        prevOffset = srcBuffer[srcOffset++] + (srcBuffer[srcOffset++] << 8);
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = (byte)(destBuffer[prevOffset + i] ^ xor);
                        }
                        xor = 0x00;
                        break;

                    case 6: // Reads one byte, which is then subtracted from the current writing address to create a pointer to a previously written address. Bytes are read in sequence from this address and are stored in sequence.
                        prevOffset = destOffset - srcBuffer[srcOffset++];
                        for (i = 0; i < count; i++)
                        {
                            destBuffer[destOffset++] = destBuffer[prevOffset++];
                        }
                        break;

                    default:
                        if (throwOnInvalidData)
                        {
                            throw new InvalidDataException("The data can't be decompressed.");
                        }
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                destOffset--;
                for (; i < count; i++)
                {
                    destBuffer[destOffset++] = (byte)(0 ^ xor);
                }
            }
        }
    }
}
