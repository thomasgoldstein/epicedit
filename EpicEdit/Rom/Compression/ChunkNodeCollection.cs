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
using System.Collections;
using System.Collections.Generic;

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// A collection of <see cref="ChunkNodes">chunk nodes</see>.
    /// It keeps track of the best solutions (ie: resulting in the highest compression rate) among the nodes.
    /// </summary>
    internal class ChunkNodeCollection : IEnumerable<KeyValuePair<int, ChunkNode>>
    {
        /// <summary>
        /// A dictionary of all the best <see cref="ChunkNode">nodes</see> (ie: nodes that lead to the best compression rate).
        /// The integer value represents the offset (position) in the buffer we want to compress.
        /// </summary>
        private Dictionary<int, ChunkNode> nodeDictionary;

        /// <summary>
        /// The queue of offsets to process.
        /// Using it with the <see cref="ByteDictionary"/>, we can retrieve the associated best <see cref="ChunkNode">node</see>.
        /// </summary>
        private Queue<int> offsetQueue;

        public ChunkNodeCollection()
        {
            this.nodeDictionary = new Dictionary<int, ChunkNode>();
            this.offsetQueue = new Queue<int>();

            ChunkNode rootNode = new ChunkNode(null, new byte[0]);
            this.Add(0, rootNode);
        }

        public void Add(int offset, ChunkNode node)
        {
            if (!this.nodeDictionary.ContainsKey(offset))
            {
                this.nodeDictionary.Add(offset, node);
                this.offsetQueue.Enqueue(offset);
            }
            else
            {
                if (node.CompressedBufferSize < this.nodeDictionary[offset].CompressedBufferSize)
                {
                    this.nodeDictionary[offset].SetAsNonOptimal();
                    this.nodeDictionary[offset] = node;
                    this.offsetQueue.Enqueue(offset);
                }
            }
        }

        public IEnumerator<KeyValuePair<int, ChunkNode>> GetEnumerator()
        {
            return this.nodeDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.nodeDictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets the node at a given index.
        /// </summary>
        public ChunkNode this[int offset]
        {
            get { return this.nodeDictionary[offset]; }
        }

        /// <summary>
        /// Gets the number of remaining nodes to process.
        /// </summary>
        public int Count
        {
            get { return this.offsetQueue.Count; }
        }

        /// <summary>
        /// Checks whether the next <see cref="ChunkNode">node</see> in the queue is worth processing,
        /// based on whether it's already been processed and if it's possibly in the path leading to the optimal compression rate.
        /// </summary>
        /// <returns>False if the next <see cref="ChunkNode">node</see> is known not to be worth processing, true otherwise.</returns>
        public bool IsNextNodeOptimal()
        {
            int offset = this.offsetQueue.Peek();
            ChunkNode node = this.nodeDictionary[offset];

            if (node.Processed || !node.IsOptimal)
            {
                this.offsetQueue.Dequeue();
            }

            return !node.Processed && node.IsOptimal;
        }

        /// <summary>
        /// Gets the next <see cref="ChunkNode">node</see> in the queue.
        /// </summary>
        /// <returns>The next <see cref="ChunkNode">node</see> in the queue.</returns>
        public KeyValuePair<int, ChunkNode> GetNextNode()
        {
            int offset = this.offsetQueue.Dequeue();
            ChunkNode node = this.nodeDictionary[offset];
            node.Processed = true;

            KeyValuePair<int, ChunkNode> offsetNode = new KeyValuePair<int, ChunkNode>(offset, node);
            return offsetNode;
        }
    }
}
