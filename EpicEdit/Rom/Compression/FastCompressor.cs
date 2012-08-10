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

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// A fast compressor with a high compression rate.
    /// </summary>
    internal class FastCompressor : ICompressor
    {
        /// <summary>
        /// Compresses the data of the passed buffer.
        /// </summary>
        /// <param name="buffer">The data to compress.</param>
        /// <param name="quirksMode">Quirks mode has a lower compression rate,
        /// but which works with Japanese and European ROMs.</param>
        /// <returns>The compressed data.</returns>
        public byte[] Compress(byte[] buffer, bool quirksMode)
        {
            ByteDictionary byteDictionary = new ByteDictionary(buffer);
            byte[] compBuffer = new byte[Codec.BufferSize];

            int i = 0; // Iterator for buffer
            int j = 0; // Iterator for compBuffer

            while (i < buffer.Length)
            {
                // Gather data to call command 4 or 6 later
                Range maxBackRange = byteDictionary.GetMaxBackRange(i);

                int distance = i - maxBackRange.Start;
                // If distance > 0xFF, then command 4 is used, otherwise it's command 6
                // We need to know this to optimize compression, because using command 4 is more expensive
                // (address is encoded on one more byte, because it's absolute rather than relative)

                int k = i + 1; // Forward iterator for buffer

                if (k < buffer.Length &&
                    buffer[i] == buffer[k])
                {
                    #region Command 1
                    k++;
                    while (k < buffer.Length &&
                           buffer[i] == buffer[k])
                    {
                        k++;
                    }

                    Range range = FastCompressor.GetRange(i, k);

                    if (((distance > 0xFF && maxBackRange.Length - 1 > range.Length) ||
                         (distance <= 0xFF && maxBackRange.Length > range.Length)) &&
                         FastCompressor.IsBackCommandCallable(distance, maxBackRange, quirksMode))
                    {
                        FastCompressor.CallCommand4Or6(compBuffer, ref i, ref j, maxBackRange);
                    }
                    else
                    {
                        #region Call command 1
                        int byteCount = range.Length;

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
                        #endregion
                    }
                    #endregion
                }
                else if ((k + 1) < buffer.Length &&
                         buffer[i] == buffer[i + 2])
                {
                    #region Command 2
                    k += 2;
                    while (k < buffer.Length &&
                           buffer[k - 2] == buffer[k])
                    {
                        k++;
                    }

                    Range range = FastCompressor.GetRange(i, k);

                    if (((distance > 0xFF && maxBackRange.Length >= range.Length) ||
                         (distance <= 0xFF && maxBackRange.Length + 1 >= range.Length)) &&
                         FastCompressor.IsBackCommandCallable(distance, maxBackRange, quirksMode))
                    {
                        FastCompressor.CallCommand4Or6(compBuffer, ref i, ref j, maxBackRange);
                    }
                    else
                    {
                        #region Call command 2
                        int byteCount = range.Length;

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
                        #endregion
                    }
                    #endregion
                }
                else if (k < buffer.Length &&
                         ((buffer[i] + 1) & 0xFF) == buffer[k])
                {
                    #region Command 3
                    k++;
                    while (k < buffer.Length &&
                           ((buffer[k - 1] + 1) & 0xFF) == buffer[k])
                    {
                        k++;
                    }

                    Range range = FastCompressor.GetRange(i, k);

                    if (((distance > 0xFF && maxBackRange.Length - 1 > range.Length) ||
                         (distance <= 0xFF && maxBackRange.Length > range.Length)) &&
                         FastCompressor.IsBackCommandCallable(distance, maxBackRange, quirksMode))
                    {
                        FastCompressor.CallCommand4Or6(compBuffer, ref i, ref j, maxBackRange);
                    }
                    else
                    {
                        #region Call command 3
                        int byteCount = range.Length;

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
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region Command 0
                    if (((distance > 0xFF && maxBackRange.Length > 2) ||
                         (distance <= 0xFF && maxBackRange.Length > 1)) &&
                         FastCompressor.IsBackCommandCallable(distance, maxBackRange, quirksMode))
                    {
                        FastCompressor.CallCommand4Or6(compBuffer, ref i, ref j, maxBackRange);
                    }
                    else
                    {
                        #region Call command 0 (pre)
                        while (k < buffer.Length)
                        {
                            if
                            (
                            #region Matches command 1, 2 or 3
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
                            #endregion
                            )
                            {
                                break;
                            }

                            Range backRange = byteDictionary.GetMaxBackRange(k);
                            distance = k - backRange.Start;
                            // Matches command 4 or 6
                            if ((distance > 0xFF && backRange.Length > 4) ||
                                (distance <= 0xFF && backRange.Length > 3))
                            {
                                break;
                            }

                            k++;
                        }

                        Range range = FastCompressor.GetRange(i, k);
                        #endregion

                        #region Call command 0
                        int byteCount = range.Length;

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
                        #endregion
                    }
                    #endregion
                }
            }

            compBuffer[j++] = 0xFF;

            return Utilities.ReadBlock(compBuffer, 0, j);
        }

        private static void CallCommand4Or6(byte[] compBuffer, ref int i, ref int j, Range range)
        {
            int distance = i - range.Start;
            int byteCount = range.Length;

            if (distance <= 0xFF)
            {
                // Use a relative address (on 1 byte) to previous data
                #region Command 6
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
                #endregion
            }
            else
            {
                // Use an absolute address (on 2 bytes) to previous data
                #region Command 4
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
                #endregion
            }

            i += byteCount;
        }

        private static Range GetRange(int start, int end)
        {
            if (end - start > Codec.SuperCommandMax)
            {
                end = start + Codec.SuperCommandMax;
            }

            return new Range(start, end);
        }

        /// <summary>
        /// Checks whether it is safe to call a back command.
        /// </summary>
        /// <param name="distance">Distance between the current buffer iterator value, and the range start.</param>
        /// <param name="range">Range within already compressed data.</param>
        /// <param name="quirksMode">Whether quirks mode is enabled.</param>
        /// <returns>Whether it is safe to call a back command.</returns>
        private static bool IsBackCommandCallable(int distance, Range range, bool quirksMode)
        {
            // We should not call command 4 in "quirks mode" (for Jap and Euro ROMs),
            // because it can lead to data that the game cannot decompress properly.
            // Sometimes command 4 works, sometimes not. Since the details are not known (bound-crossing related, or so?),
            // we just never call the command (except for US ROMs, which are not affected by this issue).
            return
                !quirksMode ||
                distance <= 0xFF; // If distance <= 0xFF, then it's command 6, otherwise it's command 4
        }
    }
}
