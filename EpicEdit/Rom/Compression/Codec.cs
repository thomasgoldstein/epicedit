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

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// Provides methods to handle data compression and decompression.
    /// </summary>
    internal static class Codec
    {
        /// <summary>
        /// The size of the buffer to handle data compression and decompression.
        /// </summary>
        internal const int BufferSize = 16384;

        /// <summary>
        /// The maximum number of bytes stored by regular compression commands.
        /// </summary>
        internal const int NormalCommandMax = 32;

        /// <summary>
        /// The maximum number of bytes stored by super compression commands.
        /// </summary>
        internal const int SuperCommandMax = 1024;

        /// <summary>
        /// Gets or sets value that specifies whether the compression rate should be optimal (slower).
        /// </summary>
        public static bool Optimal { get; set; }

        private static ICompressor compressor;

        private static ICompressor Compressor
        {
            get
            {
                if (Codec.Optimal)
                {
                    if (!(Codec.compressor is OptimalCompressor))
                    {
                        Codec.compressor = new OptimalCompressor();
                    }
                }
                else
                {
                    if (!(Codec.compressor is FastCompressor))
                    {
                        Codec.compressor = new FastCompressor();
                    }
                }

                return Codec.compressor;
            }
        }

        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <returns>The decompressed data.</returns>
        public static byte[] Decompress(byte[] buffer)
        {
            return Codec.Decompress(buffer, 0);
        }

        /// <summary>
        /// Decompresses data until a stop (0xFF) command is found.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <returns>The decompressed data.</returns>
        public static byte[] Decompress(byte[] buffer, int offset)
        {
            byte[] decBuffer = new byte[Codec.BufferSize];

            int destPosition = 0;
            byte cmd;

            try
            {
                while ((cmd = buffer[offset++]) != 0xFF)
                {
                    byte ctrl = (byte)((cmd & 0xE0) >> 5);
                    int i = 0;
                    int count;
                    byte xor = 0x00;

                    if (ctrl == 7) // This special command extends the byte count.
                    {
                        ctrl = (byte)((cmd & 0x1C) >> 2);
                        count = ((cmd & 3) << 8) + buffer[offset++];
                    }
                    else
                    {
                        count = cmd & 0x1F;
                    }
                    count++;

                    try
                    {
                        switch (ctrl)
                        {
                            case 0: // Reads an amount of bytes from ROM in sequence stores them in the same sequence.
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = buffer[offset++];
                                }
                                break;

                            case 1: // Reads one byte from ROM and continuously stores the same byte in sequence.
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = buffer[offset];
                                }
                                offset++;
                                break;

                            case 2: // Reads two bytes from ROM and continously stores the same two bytes in sequence. 
                                int j = 0;
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = buffer[offset + j];
                                    j = 1 - j;
                                }
                                offset += 2;
                                break;

                            case 3: // Reads one byte from ROM and continously stores it, but the byte to write is incremented after every write.
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = (byte)((buffer[offset] + i) % 256);
                                }
                                offset++;
                                break;

                            case 4: // Reads two bytes consisting of a pointer to a previously written address. Bytes are sequentially read from the supplied reading address and stored in sequence to the target address.
                                int srcPosition = buffer[offset++] + buffer[offset++] * 256;
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = decBuffer[srcPosition + i];
                                }
                                break;

                            case 5: // Identical to 4, although every byte read is inverted (EOR #$FF) before it is stored. This command doesn't see much use.
                                xor = 0xFF;
                                srcPosition = buffer[offset++] + buffer[offset++] * 256;
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = (byte)(decBuffer[srcPosition + i] ^ xor);
                                }
                                xor = 0x00;
                                break;

                            case 6: // Reads one byte, which is then subtracted from the current writing address to create a pointer to a previously written address. Bytes are read in sequence from this address and are stored in sequence.
                                srcPosition = destPosition - buffer[offset++];
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = decBuffer[srcPosition++];
                                }
                                break;

                            default:
                                throw new InvalidDataException("The data can't be decompressed.");
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        destPosition--;
                        for (; i < count; i++)
                        {
                            decBuffer[destPosition++] = (byte)(0 ^ xor);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                destPosition--; // Buffer limit reached
            }

            return Utilities.ReadBlock(decBuffer, 0, destPosition);
        }

        /// <summary>
        /// Decompresses data until a certain size is reached.
        /// Specifying the length improves performances and makes it possible to decompress corrupt data.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <param name="length">Defines the length of the returned buffer.</param>
        /// <returns>The decompressed data.</returns>
        public static byte[] Decompress(byte[] buffer, int offset, int length)
        {
            byte[] decBuffer = new byte[length];
            int destPosition = 0;

            try
            {
                while (destPosition < length)
                {
                    byte cmd = buffer[offset++];
                    byte ctrl = (byte)((cmd & 0xE0) >> 5);
                    int i = 0;
                    int count;
                    byte xor = 0x00;

                    if (ctrl == 7) // This special command extends the byte count.
                    {
                        ctrl = (byte)((cmd & 0x1C) >> 2);
                        count = ((cmd & 3) << 8) + buffer[offset++];
                    }
                    else
                    {
                        count = cmd & 0x1F;
                    }
                    count++;

                    try
                    {
                        switch (ctrl)
                        {
                            case 0: // Reads an amount of bytes from ROM in sequence stores them in the same sequence.
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = buffer[offset++];
                                }
                                break;

                            case 1: // Reads one byte from ROM and continuously stores the same byte in sequence.
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = buffer[offset];
                                }
                                offset++;
                                break;

                            case 2: // Reads two bytes from ROM and continously stores the same two bytes in sequence. 
                                int j = 0;
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = buffer[offset + j];
                                    j = 1 - j;
                                }
                                offset += 2;
                                break;

                            case 3: // Reads one byte from ROM and continously stores it, but the byte to write is incremented after every write.
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = (byte)((buffer[offset] + i) % 256);
                                }
                                offset++;
                                break;

                            case 4: // Reads two bytes consisting of a pointer to a previously written address. Bytes are sequentially read from the supplied reading address and stored in sequence to the target address.
                                int srcPosition = buffer[offset++] + buffer[offset++] * 256;
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = decBuffer[srcPosition + i];
                                }
                                break;

                            case 5: // Identical to 4, although every byte read is inverted (EOR #$FF) before it is stored. This command doesn't see much use.
                                xor = 0xFF;
                                srcPosition = buffer[offset++] + buffer[offset++] * 256;
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = (byte)(decBuffer[srcPosition + i] ^ xor);
                                }
                                xor = 0x00;
                                break;

                            case 6: // Reads one byte, which is then subtracted from the current writing address to create a pointer to a previously written address. Bytes are read in sequence from this address and are stored in sequence.
                                srcPosition = destPosition - buffer[offset++];
                                for (i = 0; i < count; i++)
                                {
                                    decBuffer[destPosition++] = decBuffer[srcPosition++];
                                }
                                break;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        destPosition--;
                        for (; i < count; i++)
                        {
                            decBuffer[destPosition++] = (byte)(0 ^ xor);
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException) { }

            return decBuffer;
        }

        /// <summary>
        /// Computes the length of a compressed data block.
        /// </summary>
        /// <param name="buffer">The compressed data block.</param>
        /// <returns>The length of the compressed block.</returns>
        public static int GetLength(byte[] buffer)
        {
            return Codec.GetLength(buffer, 0);
        }

        /// <summary>
        /// Computes the length of a compressed data block.
        /// </summary>
        /// <param name="buffer">The buffer to decompress data from.</param>
        /// <param name="offset">The buffer position to start from.</param>
        /// <returns>The length of the compressed block.</returns>
        public static int GetLength(byte[] buffer, int offset)
        {
            int startingOffset = offset;
            byte cmd;

            while ((cmd = buffer[offset++]) != 0xFF)
            {
                byte ctrl = (byte)((cmd & 0xE0) >> 5);
                int count;

                if (ctrl == 7) // This special command extends the byte count.
                {
                    ctrl = (byte)((cmd & 0x1C) >> 2);
                    count = ((cmd & 3) << 8) + buffer[offset++];
                }
                else
                {
                    count = cmd & 0x1F;
                }
                count++;

                switch (ctrl)
                {
                    case 0: // Reads an amount of bytes from ROM in sequence stores them in the same sequence.
                        offset += count;
                        break;
                    case 1: // Reads one byte from ROM and continuously stores the same byte in sequence.
                    case 3: // Reads one byte from ROM and continously stores it, but the byte to write is incremented after every write.
                    case 6: // Reads one byte, which is then subtracted from the current writing address to create a pointer to a previously written address. Bytes are read in sequence from this address and are stored in sequence.
                        offset++;
                        break;
                    case 2: // Reads two bytes from ROM and continously stores the same two bytes in sequence. 
                    case 4: // Reads two bytes consisting of a pointer to a previously written address. Bytes are sequentially read from the supplied reading address and stored in sequence to the target address.
                    case 5: // Identical to 4, although every byte read is inverted (EOR #$FF) before it is stored. This command doesn't see much use.
                        offset += 2;
                        break;
                    default:
                        throw new InvalidDataException("The data can't be decompressed.");
                }
            }

            return offset - startingOffset;
        }

        public static byte[] GetCompressedChunk(byte[] buffer, int offset)
        {
            int compressedChunkLength = Codec.GetLength(buffer, offset);
            return Utilities.ReadBlock(buffer, offset, compressedChunkLength);
        }

        /// <summary>
        /// Compresses the data of the passed buffer.
        /// </summary>
        /// <param name="buffer">The data to compress.</param>
        /// <returns>The compressed data.</returns>
        public static byte[] Compress(byte[] buffer)
        {
            return Codec.Compress(buffer, false);
        }

        /// <summary>
        /// Compresses the data of the passed buffer.
        /// </summary>
        /// <param name="buffer">The data to compress.</param>
        /// <param name="quirksMode">Quirks mode has a lower compression rate,
        /// but which works with Japanese and European ROMs.</param>
        /// <returns>The compressed data.</returns>
        public static byte[] Compress(byte[] buffer, bool quirksMode)
        {
            return Codec.Compressor.Compress(buffer, quirksMode);
        }

        /// <summary>
        /// Compresses the data of a passed buffer into a destination buffer,
        /// starting at the offset value.
        /// </summary>
        /// <param name="bufferToCompress">The data to compress.</param>
        /// <param name="destinationBuffer">The buffer where the compressed data will be saved.</param>
        /// <param name="offset">Location where the data will be saved.</param>
        /// <param name="quirksMode">Quirks mode has a lower compression rate,
        /// but which works with Japanese and European ROMs.</param>
        public static void Compress(byte[] bufferToCompress, byte[] destinationBuffer, int offset, bool quirksMode)
        {
            byte[] compBuffer = Codec.Compress(bufferToCompress, quirksMode);
            Buffer.BlockCopy(compBuffer, 0, destinationBuffer, offset, compBuffer.Length);
        }
    }
}
