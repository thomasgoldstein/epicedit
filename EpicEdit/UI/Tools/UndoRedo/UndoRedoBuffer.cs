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

using EpicEdit.Rom.Tracks.Road;
using System.Collections.Generic;

namespace EpicEdit.UI.Tools.UndoRedo
{
    /// <summary>
    /// Undo/redo buffer for tile changes.
    /// </summary>
    internal class UndoRedoBuffer
    {
        /// <summary>
        /// The maximum number of undos, beyond which the oldest undos are deleted.
        /// </summary>
        private const int Limit = 10;

        /// <summary>
        /// The maximum number of changes accumulated in the buffer before consolidating them.
        /// </summary>
        private const int ChangeLimit = 50;

        /// <summary>
        /// The bound track map.
        /// </summary>
        private readonly TrackMap _trackMap;

        // Using LinkedLists rather than Stacks so as to be able to enforce a size limit.
        private readonly LinkedList<TileChange> _undoBuffer;
        private readonly LinkedList<TileChange> _redoBuffer;

        private Stack<TileChange> _buffer;

        public UndoRedoBuffer(TrackMap trackMap)
        {
            _trackMap = trackMap;
            _undoBuffer = new LinkedList<TileChange>();
            _redoBuffer = new LinkedList<TileChange>();
        }

        /// <summary>
        /// Used before making multiple calls to the Add method.
        /// Once finished, the EndAdd method must be called.
        /// This enables all the changes to count as a single Undo.
        /// </summary>
        public void BeginAdd()
        {
            if (_buffer == null)
            {
                _buffer = new Stack<TileChange>();
            }
        }

        public void Add(TileChange change)
        {
            if (_buffer == null)
            {
                BeginAdd();
                Add(change);
                EndAdd();
            }
            else
            {
                _buffer.Push(change);
                if (_buffer.Count == ChangeLimit)
                {
                    // Consolidate tile changes to improve performances
                    TileChange tileChange = new TileChange(_buffer, _trackMap);
                    _buffer.Clear();
                    _buffer.Push(tileChange);
                }
            }
        }

        /// <summary>
        /// Used after making multiple calls to the Add method.
        /// </summary>
        public void EndAdd()
        {
            if (_buffer == null)
            {
                return;
            }

            if (_buffer.Count > 0)
            {
                Add(_buffer);
            }

            _buffer = null;
        }

        private void Add(IEnumerable<TileChange> changes)
        {
            if (_undoBuffer.Count == Limit)
            {
                _undoBuffer.RemoveFirst();
            }

            _redoBuffer.Clear();

            // Consolidate tile changes into a single one
            TileChange change = new TileChange(changes, _trackMap);
            _undoBuffer.AddLast(change);
        }

        /// <summary>
        /// Cancels the previous change.
        /// </summary>
        /// <returns>The change that has been reapplied.</returns>
        public TileChange Undo()
        {
            if (!HasUndo || _buffer != null)
            {
                // Nothing to undo, or a change is already ongoing
                return null;
            }

            TileChange undoChange = _undoBuffer.Last.Value;
            TileChange redoChange = ApplyChange(undoChange);
            _undoBuffer.RemoveLast();
            _redoBuffer.AddFirst(redoChange);

            return undoChange;
        }

        /// <summary>
        /// Reapplies the last undone change.
        /// </summary>
        /// <returns>The change that has been reapplied.</returns>
        public TileChange Redo()
        {
            if (!HasRedo || _buffer != null)
            {
                // Nothing to redo, or a change is already ongoing
                return null;
            }

            TileChange redoChange = _redoBuffer.First.Value;
            TileChange undoChange = ApplyChange(redoChange);
            _redoBuffer.RemoveFirst();
            _undoBuffer.AddLast(undoChange);

            return redoChange;
        }

        private TileChange ApplyChange(TileChange change)
        {
            TileChange undoChange = new TileChange(change.X, change.Y, change.Width, change.Height, _trackMap);
            _trackMap.SetTiles(change.X, change.Y, change);
            return undoChange;
        }

        public void Clear()
        {
            _undoBuffer.Clear();
            _redoBuffer.Clear();
        }

        public bool HasUndo => _undoBuffer.Count > 0;

        public bool HasRedo => _redoBuffer.Count > 0;
    }
}
