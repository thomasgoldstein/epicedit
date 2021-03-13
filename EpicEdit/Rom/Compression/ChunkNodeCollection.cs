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

using System.Collections;
using System.Collections.Generic;

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// A collection of <see cref="ChunkNode">chunk nodes</see>.
    /// It keeps track of the best solutions (ie: resulting in the highest compression rate) among the nodes.
    /// </summary>
    internal class ChunkNodeCollection : IEnumerable<KeyValuePair<int, ChunkNode>>
    {
        /// <summary>
        /// A dictionary of all the best <see cref="ChunkNode">nodes</see> (ie: nodes that lead to the best compression rate).
        /// The integer value represents the offset (position) in the buffer we want to compress.
        /// </summary>
        private readonly Dictionary<int, ChunkNode> _nodeDictionary;

        /// <summary>
        /// The queue of offsets to process.
        /// Using it with the <see cref="ByteDictionary"/>, we can retrieve the associated best <see cref="ChunkNode">node</see>.
        /// </summary>
        private readonly Queue<int> _offsetQueue;

        public ChunkNodeCollection()
        {
            _nodeDictionary = new Dictionary<int, ChunkNode>();
            _offsetQueue = new Queue<int>();

            ChunkNode rootNode = new ChunkNode(null, -1, 0, 0);
            Add(0, rootNode);
        }

        public void Add(int offset, ChunkNode node)
        {
            if (!_nodeDictionary.TryGetValue(offset, out ChunkNode storedNode))
            {
                _nodeDictionary.Add(offset, node);
                _offsetQueue.Enqueue(offset);
            }
            else
            {
                if (node.CompressedBufferSize < storedNode.CompressedBufferSize)
                {
                    storedNode.SetAsNonOptimal();
                    _nodeDictionary[offset] = node;
                    _offsetQueue.Enqueue(offset);
                }
            }
        }

        public IEnumerator<KeyValuePair<int, ChunkNode>> GetEnumerator()
        {
            return _nodeDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _nodeDictionary.GetEnumerator();
        }

        /// <summary>
        /// Gets the node at a given index.
        /// </summary>
        public ChunkNode this[int offset] => _nodeDictionary[offset];

        /// <summary>
        /// Gets the number of remaining nodes to process.
        /// </summary>
        public int Count => _offsetQueue.Count;

        /// <summary>
        /// Checks whether the next <see cref="ChunkNode">node</see> in the queue is worth processing,
        /// based on whether it's already been processed and if it's possibly in the path leading to the optimal compression rate.
        /// </summary>
        /// <returns>False if the next <see cref="ChunkNode">node</see> is known not to be worth processing, true otherwise.</returns>
        public bool IsNextNodeOptimal()
        {
            int offset = _offsetQueue.Peek();
            ChunkNode node = _nodeDictionary[offset];

            if (node.Processed || !node.IsOptimal)
            {
                _offsetQueue.Dequeue();
            }

            return !node.Processed && node.IsOptimal;
        }

        /// <summary>
        /// Gets the next <see cref="ChunkNode">node</see> in the queue.
        /// </summary>
        /// <returns>The next <see cref="ChunkNode">node</see> in the queue.</returns>
        public KeyValuePair<int, ChunkNode> GetNextNode()
        {
            int offset = _offsetQueue.Dequeue();
            ChunkNode node = _nodeDictionary[offset];
            node.Processed = true;

            KeyValuePair<int, ChunkNode> offsetNode = new KeyValuePair<int, ChunkNode>(offset, node);
            return offsetNode;
        }
    }
}
