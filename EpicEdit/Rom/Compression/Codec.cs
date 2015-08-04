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
using EpicEdit.Rom.Utility;

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
        internal const int BufferSize = 131072;

        /// <summary>
        /// The maximum number of bytes stored by regular compression commands.
        /// </summary>
        internal const int NormalCommandMax = 32;

        /// <summary>
        /// The maximum number of bytes stored by super compression commands.
        /// </summary>
        internal const int SuperCommandMax = 1024;

        private static bool QuirksMode;

        public static void SetRegion(Region region)
        {
            Codec.QuirksMode = region != Region.US;
        }

        public static int GetValidatedSuperCommandSize(int byteCount)
        {
            if (byteCount > Codec.SuperCommandMax)
            {
                byteCount = Codec.SuperCommandMax;
            }

            if (Codec.QuirksMode && (byteCount % 256) == 0)
            {
                // Japanese and European ROMs do not support command sizes that are a multiple of 256
                // (at least for the second compression of double compressed data),
                // so subtract one from the byte count.
                byteCount--;
            }

            return byteCount;
        }

        private static ICompressor compressor;

        private static ICompressor Compressor
        {
            get
            {
                if (Codec.compressor == null)
                {
                    Codec.compressor = new FastCompressor();
                }

                return Codec.compressor;
            }
        }

        private static ICompressor optimalCompressor;

        private static ICompressor OptimalCompressor
        {
            get
            {
                if (Codec.optimalCompressor == null)
                {
                    Codec.optimalCompressor = new OptimalCompressor();
                }

                return Codec.optimalCompressor;
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
            byte[] destBuffer = new byte[Codec.BufferSize];
            int destPosition = 0;
            byte value;

            try
            {
                while ((value = buffer[offset++]) != 0xFF)
                {
                    Codec.Decompress(buffer, destBuffer, value, true, ref offset, ref destPosition);
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
        public static byte[] Decompress(byte[] buffer, int offset, int length)
        {
            byte[] destBuffer = new byte[length];
            int destPosition = 0;

            try
            {
                while (destPosition < length)
                {
                    byte value = buffer[offset++];
                    Codec.Decompress(buffer, destBuffer, value, false, ref offset, ref destPosition);
                }
            }
            catch (IndexOutOfRangeException) { }

            return destBuffer;
        }

        private static void Decompress(byte[] srcBuffer, byte[] destBuffer, byte value, bool throwOnInvalidData, ref int srcOffset, ref int destOffset)
        {
            byte command = (byte)((value & 0xE0) >> 5);
            int i = 0;
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
                        int j = 0;
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
                        int prevOffset = srcBuffer[srcOffset++] + (srcBuffer[srcOffset++] << 8);
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

        public static byte[] Decompress(byte[] buffer, int offset, bool twice)
        {
            byte[] data = Codec.Decompress(buffer, offset);

            if (twice)
            {
                data = Codec.Decompress(data);
            }

            return data;
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
            byte value;

            while ((value = buffer[offset++]) != 0xFF)
            {
                byte command = (byte)((value & 0xE0) >> 5);
                int count;

                if (command == 7) // This special command extends the byte count.
                {
                    command = (byte)((value & 0x1C) >> 2);
                    count = ((value & 3) << 8) + buffer[offset++];
                }
                else
                {
                    count = value & 0x1F;
                }
                count++;

                switch (command)
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
        /// <param name="optimize">Optimize compression rate (slower).</param>
        /// <returns>The compressed data.</returns>
        public static byte[] Compress(byte[] buffer, bool optimize)
        {
            ICompressor comp = !optimize ? Codec.Compressor : Codec.OptimalCompressor;
            return comp.Compress(buffer);
        }

        /// <summary>
        /// Compresses the data of a passed buffer into a destination buffer,
        /// starting at the offset value.
        /// </summary>
        /// <param name="bufferToCompress">The data to compress.</param>
        /// <param name="destinationBuffer">The buffer where the compressed data will be saved.</param>
        /// <param name="offset">Location where the data will be saved.</param>
        public static void Compress(byte[] bufferToCompress, byte[] destinationBuffer, int offset)
        {
            byte[] compBuffer = Codec.Compress(bufferToCompress);
            Buffer.BlockCopy(compBuffer, 0, destinationBuffer, offset, compBuffer.Length);
        }

        /// <summary>
        /// Compresses the data of the passed buffer twice.
        /// </summary>
        /// <param name="buffer">The data to compress.</param>
        /// <param name="twice">Compress the data twice.</param>
        /// <param name="optimize">Optimize compression rate (slower).</param>
        /// <returns>The double-compressed data.</returns>
        public static byte[] Compress(byte[] buffer, bool twice, bool optimize)
        {
            buffer = Codec.Compress(buffer, optimize);

            if (twice)
            {
                // NOTE: No need to optimize the second compression,
                // it's quite slow and doesn't really improve the compression rate.
                buffer = Codec.Compress(buffer);
            }

            return buffer;
        }
    }
}
