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
    /// A chunk node is the result of a compression command.
    /// </summary>
    internal class ChunkNode
    {
        public int CompressedBufferSize { get; }

        /// <summary>
        /// Gets or sets the value determining whether the node has already been processed.
        /// </summary>
        public bool Processed { get; set; }

        /// <summary>
        /// Gets a value indicidating whether the node is among the optimal solutions.
        /// </summary>
        public bool IsOptimal { get; private set; }

        private readonly ChunkNode _parent;
        private List<ChunkNode> _children;
        private readonly int _command;
        private readonly int _sourceOffset;
        private readonly int _byteCount;
        private readonly int _compressedChunkSize;

        private int CompressedBufferOffset => CompressedBufferSize - _compressedChunkSize;

        public ChunkNode(ChunkNode parent, int command, int sourceOffset, int byteCount)
        {
            IsOptimal = true;
            _command = command;
            _sourceOffset = sourceOffset;
            _byteCount = byteCount;
            _compressedChunkSize = GetCompressedSize();
            CompressedBufferSize = _compressedChunkSize;

            if (parent != null)
            {
                _parent = parent;
                _parent.AddChild(this);
                CompressedBufferSize += _parent.CompressedBufferSize;
            }
        }

        private void AddChild(ChunkNode child)
        {
            if (_children == null)
            {
                _children = new List<ChunkNode>();
            }

            _children.Add(child);
        }

        /// <summary>
        /// Marks the node and all of its descendants as non-optimal.
        /// </summary>
        public void SetAsNonOptimal()
        {
            IsOptimal = false;

            if (_children != null)
            {
                foreach (var child in _children)
                {
                    child.SetAsNonOptimal();
                }
            }
        }

        private int GetCompressedSize()
        {
            var length = _byteCount <= Codec.NormalCommandMax ? 1 : 2;

            switch (_command)
            {
                case 0: // Reads an amount of bytes from ROM in sequence stores them in the same sequence.
                    length += _byteCount;
                    break;
                case 1: // Reads one byte from ROM and continuously stores the same byte in sequence.
                case 3: // Reads one byte from ROM and continously stores it, but the byte to write is incremented after every write.
                case 6: // Reads one byte, which is then subtracted from the current writing address to create a pointer to a previously written address. Bytes are read in sequence from this address and are stored in sequence.
                    length++;
                    break;
                case 2: // Reads two bytes from ROM and continously stores the same two bytes in sequence. 
                case 4: // Reads two bytes consisting of a pointer to a previously written address. Bytes are sequentially read from the supplied reading address and stored in sequence to the target address.
                case 5: // Identical to 4, although every byte read is inverted (EOR #$FF) before it is stored. This command doesn't see much use.
                    length += 2;
                    break;
                default: // Root ChunkNode
                    length = 0;
                    break;
            }

            return length;
        }

        /// <summary>
        /// Returns all the data compressed up to this node.
        /// </summary>
        /// <param name="sourceBuffer">The data to be compressed.</param>
        /// <returns>The compressed data.</returns>
        public byte[] GetCompressedBuffer(byte[] sourceBuffer)
        {
            var compressedBuffer = new byte[CompressedBufferSize + 1];
            // + 1 for the ending 0xFF command

            CopyChunk(sourceBuffer, compressedBuffer);
            compressedBuffer[CompressedBufferSize] = 0xFF;

            return compressedBuffer;
        }

        private void CopyChunk(byte[] sourceBuffer, byte[] compressedBuffer)
        {
            if (_parent != null)
            {
                _parent.CopyChunk(sourceBuffer, compressedBuffer);
            }

            var destOffset = CompressedBufferOffset;

            switch (_command)
            {
                case 0:
                    if (_byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xE0 + ((_byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1 & 0xFF);
                    }

                    Buffer.BlockCopy(sourceBuffer, _sourceOffset, compressedBuffer, destOffset, _byteCount);
                    break;

                case 1:
                    if (_byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x20 + _byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xE4 + ((_byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset] = sourceBuffer[_sourceOffset];
                    break;

                case 2:
                    if (_byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x40 + _byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xE8 + ((_byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset++] = sourceBuffer[_sourceOffset];
                    compressedBuffer[destOffset] = sourceBuffer[_sourceOffset + 1];
                    break;

                case 3:
                    if (_byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x60 + _byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xEC + ((_byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset] = sourceBuffer[_sourceOffset];
                    break;

                case 4:
                    if (_byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x80 + _byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xF0 + ((_byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset++] = (byte)(_sourceOffset & 0x00FF);
                    compressedBuffer[destOffset] = (byte)((_sourceOffset & 0xFF00) >> 8);
                    break;

                case 6:
                    if (_byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0xC0 + _byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xF8 + ((_byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(_byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset] = (byte)(_sourceOffset);
                    break;
            }
        }
    }
}
