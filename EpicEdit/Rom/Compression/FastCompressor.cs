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

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// A fast compressor with a high compression rate.
    /// </summary>
    internal class FastCompressor : ICompressor
    {
        public byte[] Compress(byte[] buffer)
        {
            var byteDictionary = new ByteDictionary(buffer);
            var compBuffer = new byte[Codec.BufferSize];

            var i = 0; // Iterator for buffer
            var j = 0; // Iterator for compBuffer

            while (i < buffer.Length)
            {
                var k = i + 1; // Forward iterator for buffer
                int command; // The compression command we consider the most efficient
                int commandCost; // How costly we consider this command

                if (k < buffer.Length &&
                    buffer[i] == buffer[k])
                {
                    // Command 1: the same byte is repeated
                    command = 1;
                    commandCost = 0;

                    do
                    {
                        k++;
                    }
                    while (k < buffer.Length &&
                           buffer[i] == buffer[k]);
                }
                else if ((k + 1) < buffer.Length &&
                         buffer[i] == buffer[i + 2])
                {
                    // Command 2: a sequence of 2 bytes is repeated
                    command = 2;
                    commandCost = 2;

                    k += 2;
                    while (k < buffer.Length &&
                           buffer[k - 2] == buffer[k])
                    {
                        k++;
                    }
                }
                else if (k < buffer.Length &&
                         ((buffer[i] + 1) & 0xFF) == buffer[k])
                {
                    // Command 3: a value keeps getting incremented by 1
                    command = 3;
                    commandCost = 0;

                    do
                    {
                        k++;
                    }
                    while (k < buffer.Length &&
                           ((buffer[k - 1] + 1) & 0xFF) == buffer[k]);
                }
                else
                {
                    // Command 0: simply stores a sequence of bytes as is
                    command = 0;
                    commandCost = 0;
                }

                var range = GetRange(i, k);
                var maxBackRange = byteDictionary.GetMaxBackRange(i);
                var distance = i - maxBackRange.Start;

                if (distance > 0xFF)
                {
                    // If distance > 0xFF, then command 4 should be used, otherwise command 6 is more efficient.
                    // We need to know this to optimize compression, because using command 4 is more expensive
                    // (the address is encoded on one more byte, because it's absolute rather than relative).
                    commandCost--; // Decreases the odds of using a back command (4 or 6)
                }

                if (maxBackRange.Length + commandCost > range.Length)
                {
                    // Command 4 or 6: repeats a previous data sequence
                    // Command 4: from an absolute address (on two bytes)
                    // Command 6: from a relative address (on one byte)
                    command = 4; // Actually 4 or 6
                }

                switch (command)
                {
                    case 0:
                        CallCommand0(buffer, compBuffer, ref i, ref j, k, byteDictionary);
                        break;

                    case 1:
                        CallCommand1(buffer, compBuffer, ref i, ref j, range.Length);
                        break;

                    case 2:
                        CallCommand2(buffer, compBuffer, ref i, ref j, range.Length);
                        break;

                    case 3:
                        CallCommand3(buffer, compBuffer, ref i, ref j, range.Length);
                        break;

                    case 4:
                        CallCommand4Or6(compBuffer, ref i, ref j, maxBackRange);
                        break;
                }
            }

            compBuffer[j++] = 0xFF;

            return Utilities.ReadBlock(compBuffer, 0, j);
        }

        private static void CallCommand0(byte[] buffer, byte[] compBuffer, ref int i, ref int j, int k, ByteDictionary byteDictionary)
        {
            while (k < buffer.Length)
            {
                if
                (
                    (
                        // Matches command 3
                        (k + 2) < buffer.Length &&
                        (buffer[k] == ((buffer[k + 1] - 1) & 0xFF) &&
                         buffer[k + 2] == ((buffer[k + 1] + 1) & 0xFF))
                    )
                    ||
                    (
                        // Matches command 1
                        (k + 2) < buffer.Length &&
                        (buffer[k] == buffer[k + 1] &&
                         buffer[k] == buffer[k + 2])
                    )
                    ||
                    (
                        // Matches command 2
                        (k + 3) < buffer.Length &&
                        (buffer[k] == buffer[k + 2] &&
                         buffer[k + 1] == buffer[k + 3])
                    )
                )
                {
                    break;
                }

                var backRange = byteDictionary.GetMaxBackRange(k);
                var distance = k - backRange.Start;
                // Matches command 4 or 6
                if ((distance > 0xFF && backRange.Length > 4) ||
                    (distance <= 0xFF && backRange.Length > 3))
                {
                    break;
                }

                k++;
            }

            var range = GetRange(i, k);

            var byteCount = range.Length;

            if (byteCount <= Codec.NormalCommandMax)
            {
                compBuffer[j++] = (byte)(byteCount - 1);
            }
            else
            {
                compBuffer[j++] = (byte)(0xE0 + ((byteCount - 1 & 0x300) >> 8));
                compBuffer[j++] = (byte)(byteCount - 1 & 0xFF);
            }

            Buffer.BlockCopy(buffer, i, compBuffer, j, byteCount);
            j += byteCount;
            i += byteCount;
        }

        private static void CallCommand1(byte[] buffer, byte[] compBuffer, ref int i, ref int j, int byteCount)
        {
            if (byteCount <= Codec.NormalCommandMax)
            {
                compBuffer[j++] = (byte)(0x20 + byteCount - 1);
            }
            else
            {
                compBuffer[j++] = (byte)(0xE4 + ((byteCount - 1 & 0x300) >> 8));
                compBuffer[j++] = (byte)(byteCount - 1 & 0xFF);
            }

            compBuffer[j++] = buffer[i];
            i += byteCount;
        }

        private static void CallCommand2(byte[] buffer, byte[] compBuffer, ref int i, ref int j, int byteCount)
        {
            if (byteCount <= Codec.NormalCommandMax)
            {
                compBuffer[j++] = (byte)(0x40 + byteCount - 1);
            }
            else
            {
                compBuffer[j++] = (byte)(0xE8 + ((byteCount - 1 & 0x300) >> 8));
                compBuffer[j++] = (byte)(byteCount - 1 & 0xFF);
            }

            compBuffer[j++] = buffer[i];
            compBuffer[j++] = buffer[i + 1];
            i += byteCount;
        }

        private static void CallCommand3(byte[] buffer, byte[] compBuffer, ref int i, ref int j, int byteCount)
        {
            if (byteCount <= Codec.NormalCommandMax)
            {
                compBuffer[j++] = (byte)(0x60 + byteCount - 1);
            }
            else
            {
                compBuffer[j++] = (byte)(0xEC + ((byteCount - 1 & 0x300) >> 8));
                compBuffer[j++] = (byte)(byteCount - 1 & 0xFF);
            }

            compBuffer[j++] = buffer[i];
            i += byteCount;
        }

        private static void CallCommand4Or6(byte[] compBuffer, ref int i, ref int j, Range range)
        {
            var distance = i - range.Start;
            var byteCount = range.Length;

            if (distance <= 0xFF)
            {
                // Command 6: uses a relative address (on 1 byte) to previous data
                if (byteCount <= Codec.NormalCommandMax)
                {
                    compBuffer[j++] = (byte)(0xC0 + byteCount - 1);
                }
                else
                {
                    compBuffer[j++] = (byte)(0xF8 + ((byteCount - 1 & 0x300) >> 8));
                    compBuffer[j++] = (byte)(byteCount - 1 & 0xFF);
                }

                compBuffer[j++] = (byte)distance;
            }
            else
            {
                // Command 4: uses an absolute address (on 2 bytes) to previous data
                if (byteCount <= Codec.NormalCommandMax)
                {
                    compBuffer[j++] = (byte)(0x80 + byteCount - 1);
                }
                else
                {
                    compBuffer[j++] = (byte)(0xF0 + ((byteCount - 1 & 0x300) >> 8));
                    compBuffer[j++] = (byte)(byteCount - 1 & 0xFF);
                }

                compBuffer[j++] = (byte)(range.Start & 0x00FF);
                compBuffer[j++] = (byte)((range.Start & 0xFF00) >> 8);
            }

            i += byteCount;
        }

        private static Range GetRange(int start, int end)
        {
            var range = new Range(start, end);
            range.Length = Codec.GetValidatedSuperCommandSize(range.Length);
            return range;
        }
    }
}
