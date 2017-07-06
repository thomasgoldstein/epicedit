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
        private readonly TrackMap trackMap;

        // Using LinkedLists rather than Stacks so as to be able to enforce a size limit.
        private readonly LinkedList<TileChange> undoBuffer;
        private readonly LinkedList<TileChange> redoBuffer;

        private Stack<TileChange> buffer;

        public UndoRedoBuffer(TrackMap trackMap)
        {
            this.trackMap = trackMap;
            this.undoBuffer = new LinkedList<TileChange>();
            this.redoBuffer = new LinkedList<TileChange>();
        }

        /// <summary>
        /// Used before making multiple calls to the Add method.
        /// Once finished, the EndAdd method must be called.
        /// This enables all the changes to count as a single Undo.
        /// </summary>
        public void BeginAdd()
        {
            if (this.buffer == null)
            {
                this.buffer = new Stack<TileChange>();
            }
        }

        public void Add(TileChange change)
        {
            if (this.buffer == null)
            {
                this.BeginAdd();
                this.Add(change);
                this.EndAdd();
            }
            else
            {
                this.buffer.Push(change);
                if (this.buffer.Count == ChangeLimit)
                {
                    // Consolidate tile changes to improve performances
                    TileChange tileChange = new TileChange(this.buffer, this.trackMap);
                    this.buffer.Clear();
                    this.buffer.Push(tileChange);
                }
            }
        }

        /// <summary>
        /// Used after making multiple calls to the Add method.
        /// </summary>
        public void EndAdd()
        {
            if (this.buffer == null)
            {
                return;
            }

            if (this.buffer.Count > 0)
            {
                this.Add(this.buffer);
            }

            this.buffer = null;
        }

        private void Add(IEnumerable<TileChange> changes)
        {
            if (this.undoBuffer.Count == Limit)
            {
                this.undoBuffer.RemoveFirst();
            }

            this.redoBuffer.Clear();

            // Consolidate tile changes into a single one
            TileChange change = new TileChange(changes, this.trackMap);
            this.undoBuffer.AddLast(change);
        }

        /// <summary>
        /// Cancels the previous change.
        /// </summary>
        /// <returns>The change that has been reapplied.</returns>
        public TileChange Undo()
        {
            if (!this.HasUndo || this.buffer != null)
            {
                // Nothing to undo, or a change is already ongoing
                return null;
            }

            TileChange undoChange = this.undoBuffer.Last.Value;
            TileChange redoChange = this.ApplyChange(undoChange);
            this.undoBuffer.RemoveLast();
            this.redoBuffer.AddFirst(redoChange);

            return undoChange;
        }

        /// <summary>
        /// Reapplies the last undone change.
        /// </summary>
        /// <returns>The change that has been reapplied.</returns>
        public TileChange Redo()
        {
            if (!this.HasRedo || this.buffer != null)
            {
                // Nothing to redo, or a change is already ongoing
                return null;
            }

            TileChange redoChange = this.redoBuffer.First.Value;
            TileChange undoChange = this.ApplyChange(redoChange);
            this.redoBuffer.RemoveFirst();
            this.undoBuffer.AddLast(undoChange);

            return redoChange;
        }

        private TileChange ApplyChange(TileChange change)
        {
            TileChange undoChange = new TileChange(change.X, change.Y, change.Width, change.Height, this.trackMap);
            this.trackMap.SetTiles(change.X, change.Y, change);
            return undoChange;
        }

        public void Clear()
        {
            this.undoBuffer.Clear();
            this.redoBuffer.Clear();
        }

        public bool HasUndo
        {
            get { return this.undoBuffer.Count > 0; }
        }

        public bool HasRedo
        {
            get { return this.redoBuffer.Count > 0; }
        }
    }
}
