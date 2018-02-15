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

        private readonly ChunkNode parent;
        private List<ChunkNode> children;
        private readonly int command;
        private readonly int sourceOffset;
        private readonly int byteCount;
        private readonly int compressedChunkSize;

        private int CompressedBufferOffset => this.CompressedBufferSize - this.compressedChunkSize;

        public ChunkNode(ChunkNode parent, int command, int sourceOffset, int byteCount)
        {
            this.IsOptimal = true;
            this.command = command;
            this.sourceOffset = sourceOffset;
            this.byteCount = byteCount;
            this.compressedChunkSize = this.GetCompressedSize();
            this.CompressedBufferSize = this.compressedChunkSize;

            if (parent != null)
            {
                this.parent = parent;
                this.parent.AddChild(this);
                this.CompressedBufferSize += this.parent.CompressedBufferSize;
            }
        }

        private void AddChild(ChunkNode child)
        {
            if (this.children == null)
            {
                this.children = new List<ChunkNode>();
            }

            this.children.Add(child);
        }

        /// <summary>
        /// Marks the node and all of its descendants as non-optimal.
        /// </summary>
        public void SetAsNonOptimal()
        {
            this.IsOptimal = false;

            if (this.children != null)
            {
                foreach (ChunkNode child in this.children)
                {
                    child.SetAsNonOptimal();
                }
            }
        }

        private int GetCompressedSize()
        {
            int length = this.byteCount <= Codec.NormalCommandMax ? 1 : 2;

            switch (this.command)
            {
                case 0: // Reads an amount of bytes from ROM in sequence stores them in the same sequence.
                    length += this.byteCount;
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
            byte[] compressedBuffer = new byte[this.CompressedBufferSize + 1];
            // + 1 for the ending 0xFF command

            this.CopyChunk(sourceBuffer, compressedBuffer);
            compressedBuffer[this.CompressedBufferSize] = 0xFF;

            return compressedBuffer;
        }

        private void CopyChunk(byte[] sourceBuffer, byte[] compressedBuffer)
        {
            if (this.parent != null)
            {
                this.parent.CopyChunk(sourceBuffer, compressedBuffer);
            }

            int destOffset = this.CompressedBufferOffset;

            switch (this.command)
            {
                case 0:
                    if (byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xE0 + ((byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1 & 0xFF);
                    }

                    Buffer.BlockCopy(sourceBuffer, this.sourceOffset, compressedBuffer, destOffset, byteCount);
                    break;

                case 1:
                    if (byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x20 + byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xE4 + ((byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset] = sourceBuffer[this.sourceOffset];
                    break;

                case 2:
                    if (byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x40 + byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xE8 + ((byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset++] = sourceBuffer[this.sourceOffset];
                    compressedBuffer[destOffset] = sourceBuffer[this.sourceOffset + 1];
                    break;

                case 3:
                    if (byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x60 + byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xEC + ((byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset] = sourceBuffer[this.sourceOffset];
                    break;

                case 4:
                    if (byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0x80 + byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xF0 + ((byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset++] = (byte)(this.sourceOffset & 0x00FF);
                    compressedBuffer[destOffset] = (byte)((this.sourceOffset & 0xFF00) >> 8);
                    break;

                case 6:
                    if (byteCount <= Codec.NormalCommandMax)
                    {
                        compressedBuffer[destOffset++] = (byte)(0xC0 + byteCount - 1);
                    }
                    else
                    {
                        compressedBuffer[destOffset++] = (byte)(0xF8 + ((byteCount - 1 & 0x300) >> 8));
                        compressedBuffer[destOffset++] = (byte)(byteCount - 1 & 0xFF);
                    }
                    compressedBuffer[destOffset] = (byte)(this.sourceOffset);
                    break;
            }
        }
    }
}
