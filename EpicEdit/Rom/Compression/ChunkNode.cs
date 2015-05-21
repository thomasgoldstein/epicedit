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
        public int CompressedBufferSize { get; private set; }

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
        private readonly byte[] compressedChunk;

        public ChunkNode(ChunkNode parent, byte[] compressedChunk)
        {
            if (compressedChunk == null)
            {
                throw new ArgumentNullException("compressedChunk", "The compressed chunk can't be null.");
            }

            this.IsOptimal = true;
            this.compressedChunk = compressedChunk;
            this.CompressedBufferSize = this.compressedChunk.Length;

            if (parent != null)
            {
                this.parent = parent;
                this.parent.AddChild(this);
                this.CompressedBufferSize += this.parent.CompressedBufferSize;
            }
        }

        /// <summary>
        /// Returns all the data compressed up to this node.
        /// </summary>
        /// <returns>The compressed data.</returns>
        public byte[] GetCompressedBuffer()
        {
            byte[] compressedBuffer = new byte[this.CompressedBufferSize + 1];
            // + 1 for the ending 0xFF command

            if (this.parent != null)
            {
                this.parent.CopyChunk(compressedBuffer);
            }

            this.CopyChunk(compressedBuffer);
            compressedBuffer[this.CompressedBufferSize] = 0xFF;

            return compressedBuffer;
        }

        private void CopyChunk(byte[] compressedBuffer)
        {
            if (this.parent != null)
            {
                this.parent.CopyChunk(compressedBuffer);
            }

            Buffer.BlockCopy(this.compressedChunk, 0, compressedBuffer, this.CompressedBufferSize - this.compressedChunk.Length, this.compressedChunk.Length);
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
    }
}
