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
        /// The maximum number of undos, beyond which the oldest undos are deleted.
        /// </summary>
        private const int Limit = 10;

        /// <summary>
        /// The maximum number of changes accumulated in the buffer before consolidating them.
        /// </summary>
        private const int ChangeLimit = 50;

        /// <summary>
        /// The bound track.
        /// </summary>
        private Track track;

        // Using LinkedLists rather than Stacks so as to be able to enforce a size limit.
        private LinkedList<TileChange> undoBuffer;
        private LinkedList<TileChange> redoBuffer;

        private Stack<TileChange> buffer;

        public UndoRedoBuffer(Track track)
        {
            this.track = track;
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
            this.buffer = new Stack<TileChange>();
        }

        public void Add(TileChange change)
        {
            if (this.buffer == null)
            {
                var changes = new Stack<TileChange>();
                changes.Push(change);
                this.Add(changes);
            }
            else
            {
                this.buffer.Push(change);
                if (this.buffer.Count == ChangeLimit)
                {
                    TileChange tileChange = this.ConsolidateChanges(this.buffer);
                    this.buffer.Clear();
                    this.buffer.Push(tileChange);
                }
            }
        }

        /// <summary>
        /// Consolidates changes, uniting them all into a single one, so as to improve performances.
        /// </summary>
        private TileChange ConsolidateChanges(IEnumerable<TileChange> changes)
        {
            int xStart = TrackMap.Size;
            int yStart = TrackMap.Size;
            int xEnd = 0;
            int yEnd = 0;

            foreach (TileChange change in changes)
            {
                if (xStart > change.X)
                {
                    xStart = change.X;
                }

                int right = change.X + change.Width;
                if (xEnd < right)
                {
                    xEnd = right;
                }

                if (yStart > change.Y)
                {
                    yStart = change.Y;
                }

                int bottom = change.Y + change.Height;
                if (yEnd < bottom)
                {
                    yEnd = bottom;
                }
            }

            int width = xEnd - xStart;
            int height = yEnd - yStart;

            TrackMap map = this.track.Map;
            byte[][] data = new byte[height][];
            for (int y = 0; y < height; y++)
            {
                data[y] = new byte[width];
                for (int x = 0; x < width; x++)
                {
                    data[y][x] = map[xStart + x, yStart + y];
                }
            }

            foreach (TileChange change in changes)
            {
                int offsetY = change.Y - yStart;
                int offsetX = change.X - xStart;
                for (int y = 0; y < change.Height; y++)
                {
                    for (int x = 0; x < change.Width; x++)
                    {
                        data[offsetY + y][offsetX + x] = change[x, y];
                    }
                }
            }

            return new TileChange(xStart, yStart, data);
        }

        /// <summary>
        /// Used after making multiple calls to the Add method.
        /// </summary>
        public void EndAdd()
        {
            this.Add(this.buffer);
            this.buffer = null;
        }

        public void Add(IEnumerable<TileChange> changes)
        {
            if (this.undoBuffer.Count == Limit)
            {
                this.undoBuffer.RemoveFirst();
            }

            this.redoBuffer.Clear();

            TileChange change = this.ConsolidateChanges(changes);
            this.undoBuffer.AddLast(change);
        }

        public void Undo()
        {
            TileChange undoChange = this.undoBuffer.Last.Value;
            TileChange redoChange = this.UndoRedoCommon(undoChange);
            this.undoBuffer.RemoveLast();
            this.redoBuffer.AddFirst(redoChange);
        }

        public void Redo()
        {
            TileChange redoChange = this.redoBuffer.First.Value;
            TileChange undoChange = this.UndoRedoCommon(redoChange);
            this.redoBuffer.RemoveFirst();
            this.undoBuffer.AddLast(undoChange);
        }

        private TileChange UndoRedoCommon(TileChange change)
        {
            byte[][] previousData = new byte[change.Height][];

            for (int y = 0; y < change.Height; y++)
            {
                previousData[y] = new byte[change.Width];

                for (int x = 0; x < change.Width; x++)
                {
                    previousData[y][x] = this.track.Map[change.X + x, change.Y + y];
                    this.track.Map[change.X + x, change.Y + y] = change[x, y];
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
            get { return this.undoBuffer.Count > 0; }
        }

        public bool HasRedo
        {
            get { return this.redoBuffer.Count > 0; }
        }

        public TileChange NextUndo
        {
            get { return this.undoBuffer.Last.Value; }
        }

        public TileChange NextRedo
        {
            get { return this.redoBuffer.First.Value; }
        }
    }
}
