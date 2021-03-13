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
using System.Collections.Generic;

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// A slow compressor with a very high compression rate.
    /// </summary>
    internal class OptimalCompressor : ICompressor
    {
        public byte[] Compress(byte[] buffer)
        {
            ByteDictionary byteDictionary = new ByteDictionary(buffer);
            ChunkNodeCollection nodeCollection = new ChunkNodeCollection();

            while (nodeCollection.Count > 0)
            {
                if (nodeCollection.IsNextNodeOptimal())
                {
                    KeyValuePair<int, ChunkNode> parentNode = nodeCollection.GetNextNode();

                    if (parentNode.Key < buffer.Length)
                    {
                        CreateChildNodes(buffer, nodeCollection, parentNode.Key, parentNode.Value, byteDictionary);
                    }
                }
            }

            ChunkNode bestNode = nodeCollection[buffer.Length];
            return bestNode.GetCompressedBuffer(buffer);
        }

        private static void CreateChildNodes(byte[] buffer, ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode, ByteDictionary byteDictionary)
        {
            // NOTE: Command 5 (ie: the same as command 4 except it inverts each byte)
            // is not implemented, because it's almost never used.
            // Implementing it would complicate the code and slow down the compression for little to no benefit.

            // NOTE: Checking out command 0 every time (ie: putting it out of the if conditions)
            // can improve compression a tiny bit (like just one byte) in some rare cases,
            // but it's not worth the huge hit on compression speed.

            CreateNodesFromBackCommands(nodeCollection, i, parentNode, byteDictionary);

            int command;
            int byteCount;

            if ((i + 1) < buffer.Length &&
                buffer[i] == buffer[i + 1])
            {
                command = 1;
                byteCount = GetCommand1ByteCount(buffer, i);
            }
            else if ((i + 2) < buffer.Length &&
                     buffer[i] == buffer[i + 2])
            {
                command = 2;
                byteCount = GetCommand2ByteCount(buffer, i);
            }
            else if ((i + 1) < buffer.Length &&
                     ((buffer[i] + 1) & 0xFF) == buffer[i + 1])
            {
                command = 3;
                byteCount = GetCommand3ByteCount(buffer, i);
            }
            else
            {
                command = 0;
                byteCount = GetCommand0ByteCount(buffer, i);
            }

            CreateNodesFromCommand(command, byteCount, nodeCollection, i, parentNode);
        }

        private static void CreateNodesFromCommand(int command, int byteCount, ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode)
        {
            byteCount = Codec.GetValidatedSuperCommandSize(byteCount);

            if (byteCount > Codec.NormalCommandMax)
            {
                int reducedByteCount = Math.Min(byteCount, Codec.NormalCommandMax);
                nodeCollection.Add(i + reducedByteCount, new ChunkNode(parentNode, command, i, reducedByteCount));
            }

            nodeCollection.Add(i + byteCount, new ChunkNode(parentNode, command, i, byteCount));
        }

        private static void CreateNodesFromBackCommands(ChunkNodeCollection nodeCollection, int i, ChunkNode parentNode, ByteDictionary byteDictionary)
        {
            Range[] ranges = byteDictionary.GetMaxBackRanges(i);

            if (ranges[0].Length > 0)
            {
                nodeCollection.Add(i + ranges[0].Length, new ChunkNode(parentNode, 4, ranges[0].Start, ranges[0].Length));
            }
            if (ranges[1].Length > 0)
            {
                nodeCollection.Add(i + ranges[1].Length, new ChunkNode(parentNode, 4, ranges[1].Start, ranges[1].Length));
            }
            
            if (ranges[2].Length > 0)
            {
                nodeCollection.Add(i + ranges[2].Length, new ChunkNode(parentNode, 6, i - ranges[2].Start, ranges[2].Length));
            }
            if (ranges[3].Length > 0)
            {
                nodeCollection.Add(i + ranges[3].Length, new ChunkNode(parentNode, 6, i - ranges[3].Start, ranges[3].Length));
            }
        }

        private static int GetCommand0ByteCount(byte[] buffer, int i)
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
                        IsBackCommandComing(buffer, j)
                #endregion
                )
                {
                    break;
                }

                j++;
            }

            return j - i;
        }

        private static int GetCommand1ByteCount(byte[] buffer, int i)
        {
            int j = i + 2; // Forward iterator for buffer

            while (j < buffer.Length &&
                   buffer[i] == buffer[j])
            {
                j++;
            }

            return j - i;
        }

        private static int GetCommand2ByteCount(byte[] buffer, int i)
        {
            int j = i + 3; // Forward iterator for buffer

            while (j < buffer.Length &&
                   buffer[j - 2] == buffer[j])
            {
                j++;
            }

            return j - i;
        }

        private static int GetCommand3ByteCount(byte[] buffer, int i)
        {
            int j = i + 2; // Forward iterator for buffer

            while (j < buffer.Length &&
                   ((buffer[j - 1] + 1) & 0xFF) == buffer[j])
            {
                j++;
            }

            return j - i;
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
