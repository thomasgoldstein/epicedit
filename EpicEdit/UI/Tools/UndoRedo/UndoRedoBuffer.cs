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
using EpicEdit.Rom.Tracks;

namespace EpicEdit.UI.Tools.UndoRedo
{
    /// <summary>
    /// Undo/redo buffer for tile changes.
    /// </summary>
    public class UndoRedoBuffer
    {
        /// <summary>
        /// Undo size limit.
        /// </summary>
        private const int Limit = 10;

        // Using LinkedLists rather than Stacks so as to be able to enforce a size limit.
        private LinkedList<TileChanges> undoBuffer;
        private LinkedList<TileChanges> redoBuffer;

        private TileChanges buffer;

        public UndoRedoBuffer()
        {
            this.undoBuffer = new LinkedList<TileChanges>();
            this.redoBuffer = new LinkedList<TileChanges>();
        }

        /// <summary>
        /// Used before making multiple calls to the Add method.
        /// Once finished, the EndAdd method must be called.
        /// This enables all the changes to count as a single Undo.
        /// </summary>
        public void BeginAdd()
        {
            this.buffer = new TileChanges();
        }

        public void Add(TileChange change)
        {
            if (this.buffer == null)
            {
                TileChanges changes = new TileChanges();
                changes.Push(change);
                this.Add(changes);
            }
            else
            {
                this.buffer.Push(change);
            }
        }

        /// <summary>
        /// Used after making multiple calls to the Add method.
        /// </summary>
        public void EndAdd()
        {
            this.Add(this.buffer);
            this.buffer = null;
        }

        public void Add(TileChanges changes)
        {
            if (this.undoBuffer.Count == Limit)
            {
                this.undoBuffer.RemoveFirst();
            }

            this.redoBuffer.Clear();
            this.undoBuffer.AddLast(changes);
        }

        public void Undo(Track track)
        {
            TileChanges undoChanges = this.undoBuffer.Last.Value;
            TileChanges redoChanges = UndoRedoBuffer.UndoRedoCommon(track, undoChanges);
            this.undoBuffer.RemoveLast();
            this.redoBuffer.AddFirst(redoChanges);
        }

        public void Redo(Track track)
        {
            TileChanges redoChanges = this.redoBuffer.First.Value;
            TileChanges undoChanges = UndoRedoBuffer.UndoRedoCommon(track, redoChanges);
            this.redoBuffer.RemoveFirst();
            this.undoBuffer.AddLast(undoChanges);
        }
        
        private static TileChanges UndoRedoCommon(Track track, TileChanges changes)
        {
            TileChanges previousChanges = new TileChanges();
            
            foreach (TileChange change in changes)
            {
                TileChange previousChange = UndoRedoBuffer.UndoRedoCommon(track, change);
                previousChanges.Push(previousChange);
            }
            
            return previousChanges;
        }

        private static TileChange UndoRedoCommon(Track track, TileChange change)
        {
            byte[][] previousData = new byte[change.Height][];

            for (int y = 0; y < change.Height; y++)
            {
                previousData[y] = new byte[change.Width];

                for (int x = 0; x < change.Width; x++)
                {
                    previousData[y][x] = track.Map[change.X + x, change.Y + y];
                    track.Map[change.X + x, change.Y + y] = change[x, y];
                }
            }

            return new TileChange(change.X, change.Y, previousData);
        }

        public void Clear()
        {
            this.undoBuffer.Clear();
            this.redoBuffer.Clear();
        }

        public bool HasUndo
        {
            get
            {
                return this.undoBuffer.Count > 0;
            }
        }

        public bool HasRedo
        {
            get
            {
                return this.redoBuffer.Count > 0;
            }
        }

        public TileChanges GetNextUndo()
        {
            return this.undoBuffer.Last.Value;
        }

        public TileChanges GetNextRedo()
        {
            return this.redoBuffer.First.Value;
        }
    }
}
