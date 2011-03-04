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
using System.Collections.Generic;

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// A slow compressor with a very high compression rate.
    /// </summary>
    internal class OptimalCompressor : ICompressor
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
            if (quirksMode == true)
            {
                throw new NotImplementedException();
            }

            ByteDictionary byteDictionary = new ByteDictionary(buffer);
            ChunkNodeCollection nodeCollection = new ChunkNodeCollection();

            while (nodeCollection.Count > 0)
            {
                if (nodeCollection.IsNextNodeOptimal())
                {
                    KeyValuePair<int, ChunkNode> parentNode = nodeCollection.GetNextNode();

                    if (parentNode.Key < buffer.Length)
                    {
                        OptimalCompressor.CreateChildNodes(buffer, nodeCollection, parentNode.Key, parentNode.Value, byteDictionary);
                    }
                }
            }

            ChunkNode bestNode = nodeCollection[buffer.Length];
            return bestNode.GetCompressedBuffer();
        }

        private static void CreateChildNodes(byte[] buffer, ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode, ByteDictionary byteDictionary)
        {
            // NOTE: Command 5 (ie: the same as command 4 except it inverts each byte)
            // is not implemented, because it's almost never used.
            // Implementing it would complicate the code and slow down the compression for little to no benefit.

            // NOTE: Checking out command 0 everytime (ie: putting it out of the if conditions)
            // can improve compression a tiny bit (like just one byte) in some rare cases,
            // but it's not worth the huge hit on compression speed.

            OptimalCompressor.CreateNodesFromBackCommands(nodeCollection, i, parentNode, byteDictionary);

            if ((i + 1) < buffer.Length &&
                buffer[i] == buffer[i + 1])
            {
                OptimalCompressor.CreateNodesFromCommand(OptimalCompressor.CallCommand1, buffer, nodeCollection, i, parentNode);
            }
            else if ((i + 2) < buffer.Length &&
                     buffer[i] == buffer[i + 2])
            {
                OptimalCompressor.CreateNodesFromCommand(OptimalCompressor.CallCommand2, buffer, nodeCollection, i, parentNode);
            }
            else if ((i + 1) < buffer.Length &&
                     ((buffer[i] + 1) & 0xFF) == buffer[i + 1])
            {
                OptimalCompressor.CreateNodesFromCommand(OptimalCompressor.CallCommand3, buffer, nodeCollection, i, parentNode);
            }
            else
            {
                OptimalCompressor.CreateNodesFromCommand0(buffer, nodeCollection, i, parentNode);
            }
        }

        private static void CreateNodesFromCommand(CommandCall commandCall, byte[] buffer, ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode)
        {
            byte[] chunk;
            byte[] chunkS;

            int byteCount;
            int byteCountS;

            ChunkNode node;

            commandCall(buffer, i, out chunk, out byteCount, out chunkS, out byteCountS);
            node = new ChunkNode(parentNode, chunk);
            nodeCollection.Add(i + byteCount, node);

            if (chunkS != null)
            {
                node = new ChunkNode(parentNode, chunkS);
                nodeCollection.Add(i + byteCountS, node);
            }
        }
        private static void CreateNodesFromBackCommands(ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode, ByteDictionary byteDictionary)
        {
            Range[] ranges = byteDictionary.GetMaxBackRanges(i);
            byte[] chunk;
            byte[] chunkS;

            int byteCount;
            int byteCountS;

            ChunkNode node;

            OptimalCompressor.CallCommand4(out chunk, out byteCount, out chunkS, out byteCountS, ranges[0], ranges[1]);
            if (chunk != null)
            {
                node = new ChunkNode(parentNode, chunk);
                nodeCollection.Add(i + byteCount, node);
            }
            if (chunkS != null)
            {
                node = new ChunkNode(parentNode, chunkS);
                nodeCollection.Add(i + byteCountS, node);
            }

            OptimalCompressor.CallCommand6(i, out chunk, out byteCount, out chunkS, out byteCountS, ranges[2], ranges[3]);
            if (chunk != null)
            {
                node = new ChunkNode(parentNode, chunk);
                nodeCollection.Add(i + byteCount, node);
            }
            if (chunkS != null)
            {
                node = new ChunkNode(parentNode, chunkS);
                nodeCollection.Add(i + byteCountS, node);
            }
        }
        private static void CreateNodesFromCommand0(byte[] buffer, ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode)
        {
            byte[] chunk;
            int byteCount;
            ChunkNode node;

            OptimalCompressor.CallCommand0(buffer, i, out chunk, out byteCount);
            node = new ChunkNode(parentNode, chunk);
            nodeCollection.Add(i + byteCount, node);
        }

        private delegate void CommandCall(byte[] buffer, int i, out byte[] chunk, out int byteCount, out byte[] chunkS, out int byteCountS);

        private static void CallCommand0(byte[] buffer, int i, out byte[] chunk, out int byteCount)
        {
            Range range = OptimalCompressor.GetCommand0Range(buffer, i);
            byteCount = range.Length;

            int k = 0; // Iterator for compressed chunk

            if (byteCount <= Codec.NormalCommandMax)
            {
                chunk = new byte[byteCount + 1];
                chunk[k++] = (byte)(byteCount - 1);
            }
            else
            {
                if (byteCount > Codec.SuperCommandMax)
                {
                    byteCount = Codec.SuperCommandMax;
                }

                chunk = new byte[byteCount + 2];
                chunk[k++] = (byte)(0xE0 + ((byteCount - 1 & 0x300) >> 8));
                chunk[k++] = (byte)(byteCount - 1 & 0xFF);
            }

            Array.Copy(buffer, i, chunk, k, byteCount);
        }
        private static Range GetCommand0Range(byte[] buffer, int i)
        {
            int j = i + 1; // Forward iterator for buffer

            while (j < buffer.Length)
            {
                if (
                #region A command is coming up
(
                    // Matches command 3
                        (j + 2) < buffer.Length &&
                        (buffer[j] == ((buffer[j + 1] - 1) & 0xFF) &&
                         buffer[j + 2] == ((buffer[j + 1] + 1) & 0xFF))
                    )
                    ||
                    (
                    // Matches command 1
                        (j + 2) < buffer.Length &&
                        (buffer[j] == buffer[j + 1] &&
                         buffer[j] == buffer[j + 2])
                    )
                    ||
                    (
                    // Matches command 2
                         (j + 3) < buffer.Length &&
                         (buffer[j] == buffer[j + 2] &&
                          buffer[j + 1] == buffer[j + 3])
                    )
                    ||
                    // Matches command 4 or 6
                        OptimalCompressor.IsBackCommandComing(buffer, j)
                #endregion
)
                {
                    break;
                }

                j++;
            }

            Range range = new Range(i, j);

            return range;
        }

        private static void CallCommand1(byte[] buffer, int i, out byte[] chunk, out int byteCount, out byte[] chunkS, out int byteCountS)
        {
            int j = i + 2; // Forward iterator for buffer

            while (j < buffer.Length &&
                   buffer[i] == buffer[j])
            {
                j++;
            }

            Range range = new Range(i, j);
            byteCount = range.Length;
            byteCountS = byteCount;

            OptimalCompressor.CallCommand1Normal(buffer, i, out chunk, ref byteCount);
            OptimalCompressor.CallCommand1Super(buffer, i, out chunkS, ref byteCountS);
        }
        private static void CallCommand1Normal(byte[] buffer, int i, out byte[] chunk, ref int byteCount)
        {
            if (byteCount > Codec.NormalCommandMax)
            {
                byteCount = Codec.NormalCommandMax;
            }

            chunk = new byte[2];
            chunk[0] = (byte)(0x20 + byteCount - 1);
            chunk[1] = buffer[i];
        }
        private static void CallCommand1Super(byte[] buffer, int i, out byte[] chunk, ref int byteCount)
        {
            if (byteCount <= Codec.NormalCommandMax)
            {
                chunk = null;
                return;
            }

            if (byteCount > Codec.SuperCommandMax)
            {
                byteCount = Codec.SuperCommandMax;
            }

            chunk = new byte[3];
            chunk[0] = (byte)(0xE4 + ((byteCount - 1 & 0x300) >> 8));
            chunk[1] = (byte)(byteCount - 1 & 0xFF);
            chunk[2] = buffer[i];
        }

        private static void CallCommand2(byte[] buffer, int i, out byte[] chunk, out int byteCount, out byte[] chunkS, out int byteCountS)
        {
            int j = i + 3; // Forward iterator for buffer

            while (j < buffer.Length &&
                   buffer[j - 2] == buffer[j])
            {
                j++;
            }

            Range range = new Range(i, j);
            byteCount = range.Length;
            byteCountS = byteCount;

            OptimalCompressor.CallCommand2Normal(buffer, i, out chunk, ref byteCount);
            OptimalCompressor.CallCommand2Super(buffer, i, out chunkS, ref byteCountS);
        }
        private static void CallCommand2Normal(byte[] buffer, int i, out byte[] chunk, ref int byteCount)
        {
            if (byteCount > Codec.NormalCommandMax)
            {
                byteCount = Codec.NormalCommandMax;
            }

            chunk = new byte[3];
            chunk[0] = (byte)(0x40 + byteCount - 1);
            chunk[1] = buffer[i];
            chunk[2] = buffer[i + 1];
        }
        private static void CallCommand2Super(byte[] buffer, int i, out byte[] chunk, ref int byteCount)
        {
            if (byteCount <= Codec.NormalCommandMax)
            {
                chunk = null;
                return;
            }

            if (byteCount > Codec.SuperCommandMax)
            {
                byteCount = Codec.SuperCommandMax;
            }

            chunk = new byte[4];
            chunk[0] = (byte)(0xE8 + ((byteCount - 1 & 0x300) >> 8));
            chunk[1] = (byte)(byteCount - 1 & 0xFF);
            chunk[2] = buffer[i];
            chunk[3] = buffer[i + 1];
        }

        private static void CallCommand3(byte[] buffer, int i, out byte[] chunk, out int byteCount, out byte[] chunkS, out int byteCountS)
        {
            int j = i + 2; // Forward iterator for buffer

            while (j < buffer.Length &&
                   ((buffer[j - 1] + 1) & 0xFF) == buffer[j])
            {
                j++;
            }

            Range range = new Range(i, j);
            byteCount = range.Length;
            byteCountS = byteCount;

            OptimalCompressor.CallCommand3Normal(buffer, i, out chunk, ref byteCount);
            OptimalCompressor.CallCommand3Super(buffer, i, out chunkS, ref byteCountS);
        }
        private static void CallCommand3Normal(byte[] buffer, int i, out byte[] chunk, ref int byteCount)
        {
            if (byteCount > Codec.NormalCommandMax)
            {
                byteCount = Codec.NormalCommandMax;
            }

            chunk = new byte[2];
            chunk[0] = (byte)(0x60 + byteCount - 1);
            chunk[1] = buffer[i];
        }
        private static void CallCommand3Super(byte[] buffer, int i, out byte[] chunk, ref int byteCount)
        {
            if (byteCount <= Codec.NormalCommandMax)
            {
                chunk = null;
                return;
            }

            if (byteCount > Codec.SuperCommandMax)
            {
                byteCount = Codec.SuperCommandMax;
            }

            chunk = new byte[3];
            chunk[0] = (byte)(0xEC + ((byteCount - 1 & 0x300) >> 8));
            chunk[1] = (byte)(byteCount - 1 & 0xFF);
            chunk[2] = buffer[i];
        }

        private static void CallCommand4(out byte[] chunk, out int byteCount, out byte[] chunkS, out int byteCountS, Range range, Range rangeS)
        {
            OptimalCompressor.CallCommand4Normal(out chunk, out byteCount, range);
            OptimalCompressor.CallCommand4Super(out chunkS, out byteCountS, rangeS);
        }
        private static void CallCommand4Normal(out byte[] chunk, out int byteCount, Range range)
        {
            byteCount = range.Length;
            if (byteCount == 0)
            {
                chunk = null;
                return;
            }

            chunk = new byte[3];
            chunk[0] = (byte)(0x80 + byteCount - 1);
            chunk[1] = (byte)(range.Start & 0x00FF);
            chunk[2] = (byte)((range.Start & 0xFF00) >> 8);
        }
        private static void CallCommand4Super(out byte[] chunk, out int byteCount, Range range)
        {
            byteCount = range.Length;
            if (byteCount == 0)
            {
                chunk = null;
                return;
            }

            chunk = new byte[4];
            chunk[0] = (byte)(0xF0 + ((byteCount - 1 & 0x300) >> 8));
            chunk[1] = (byte)(byteCount - 1 & 0xFF);
            chunk[2] = (byte)(range.Start & 0x00FF);
            chunk[3] = (byte)((range.Start & 0xFF00) >> 8);
        }

        private static void CallCommand6(int i, out byte[] chunk, out int byteCount, out byte[] chunkS, out int byteCountS, Range range, Range rangeS)
        {
            OptimalCompressor.CallCommand6Normal(i, out chunk, out byteCount, range);
            OptimalCompressor.CallCommand6Super(i, out chunkS, out byteCountS, rangeS);
        }
        private static void CallCommand6Normal(int i, out byte[] chunk, out int byteCount, Range range)
        {
            byteCount = range.Length;
            if (byteCount == 0)
            {
                chunk = null;
                return;
            }

            int distance = i - range.Start;

            chunk = new byte[2];
            chunk[0] = (byte)(0xC0 + byteCount - 1);
            chunk[1] = (byte)distance;
        }
        private static void CallCommand6Super(int i, out byte[] chunk, out int byteCount, Range range)
        {
            byteCount = range.Length;
            if (byteCount == 0)
            {
                chunk = null;
                return;
            }

            int distance = i - range.Start;

            chunk = new byte[3];
            chunk[0] = (byte)(0xF8 + ((byteCount - 1 & 0x300) >> 8));
            chunk[1] = (byte)(byteCount - 1 & 0xFF);
            chunk[2] = (byte)distance;
        }

        private static bool IsBackCommandComing(byte[] buffer, int position)
        {
            for (int backRangeStart = 0; backRangeStart < position; backRangeStart++)
            {
                if (buffer[position] == buffer[backRangeStart])
                {
                    int backRangeIterator = backRangeStart;
                    int rangeIterator = position;

                    while (rangeIterator < buffer.Length &&
                           buffer[rangeIterator] == buffer[backRangeIterator])
                    {
                        backRangeIterator++;
                        rangeIterator++;
                    }

                    Range backRange = new Range(backRangeStart, backRangeIterator);
                    if (backRange.Length > 4 ||
                        position - backRangeStart <= 0xFF && backRange.Length > 3)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
