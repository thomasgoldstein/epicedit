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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.AI
{
    /// <summary>
    /// The Artificial Intelligence attached to a track. Basically a path that the computer follows.
    /// </summary>
    internal class TrackAI : IEnumerable<TrackAIElement>, INotifyPropertyChanged
    {
        public const int MaxElementCount = 128;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<EventArgs<TrackAIElement>> ElementAdded;
        public event EventHandler<EventArgs<TrackAIElement>> ElementRemoved;
        public event EventHandler<EventArgs> ElementsCleared;

        private readonly Track _track;
        private readonly List<TrackAIElement> _aiElements;

        public TrackAI(byte[] areaData, byte[] targetData, Track track)
        {
            _aiElements = new List<TrackAIElement>();
            _track = track;
            SetBytes(areaData, targetData);
        }

        public void SetBytes(byte[] areaData, byte[] targetData)
        {
            Clear();

            int i = 0; // i = iterator for areaData
            int j = 0; // j = iterator for targetData
            while (i < areaData.Length)
            {
                Add(new TrackAIElement(areaData, ref i, targetData, ref j));
            }
        }

        public IEnumerator<TrackAIElement> GetEnumerator()
        {
            return _aiElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _aiElements.GetEnumerator();
        }

        public int ElementCount => _aiElements.Count;

        /// <summary>
        /// Adds a new AI element to the element collection.
        /// </summary>
        /// <param name="aiElement">The new AI element.</param>
        public void Add(TrackAIElement aiElement)
        {
            if (_aiElements.Count >= MaxElementCount)
            {
                return;
            }

            _aiElements.Add(aiElement);
            aiElement.PropertyChanged += aiElement_PropertyChanged;
            OnElementAdded(aiElement);
        }

        /// <summary>
        /// Inserts a new AI element to the element collection at a specific index.
        /// </summary>
        /// <param name="aiElement">The new AI element.</param>
        /// <param name="index">The index of the new AI element.</param>
        public void Insert(TrackAIElement aiElement, int index)
        {
            if (_aiElements.Count >= MaxElementCount)
            {
                return;
            }

            _aiElements.Insert(index, aiElement);
            aiElement.PropertyChanged += aiElement_PropertyChanged;
            OnElementAdded(aiElement);
        }

        public void Remove(TrackAIElement aiElement)
        {
            aiElement.PropertyChanged -= aiElement_PropertyChanged;
            _aiElements.Remove(aiElement);
            OnElementRemoved(aiElement);
        }

        /// <summary>
        /// Removes all the AI elements from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (TrackAIElement aiElement in _aiElements)
            {
                aiElement.PropertyChanged -= aiElement_PropertyChanged;
            }

            _aiElements.Clear();
            OnElementsCleared();
        }

        private void aiElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private void OnElementAdded(TrackAIElement value)
        {
            ElementAdded?.Invoke(this, new EventArgs<TrackAIElement>(value));
        }

        private void OnElementRemoved(TrackAIElement value)
        {
            ElementRemoved?.Invoke(this, new EventArgs<TrackAIElement>(value));
        }

        private void OnElementsCleared()
        {
            ElementsCleared?.Invoke(this, EventArgs.Empty);
        }

        public int GetElementIndex(TrackAIElement aiElement)
        {
            return _aiElements.IndexOf(aiElement);
        }

        /// <summary>
        /// Changes the index of a given AI element.
        /// This increments or decrements the index of all AI elements in between.
        /// </summary>
        /// <param name="indexBefore">The index of the AI element before moving it.</param>
        /// <param name="indexAfter">The index of the AI element after having moved it.</param>
        public void ChangeElementIndex(int indexBefore, int indexAfter)
        {
            TrackAIElement aiElement = _aiElements[indexBefore];
            _aiElements.RemoveAt(indexBefore);
            _aiElements.Insert(indexAfter, aiElement);

            // HACK: The TrackAIElement doesn't raise this event itself.
            OnPropertyChanged(aiElement, new PropertyChangedEventArgs(PropertyNames.TrackAIElement.Index));
        }

        public static int GetTargetDataLength(byte[] areaData)
        {
            int areaCount = GetAreaCount(areaData);
            int aiTargetDataLength = areaCount * 3;
            return aiTargetDataLength;
        }

        private static int GetAreaCount(byte[] areaData)
        {
            int areaCount = 0;

            int i = 0;
            while (i < areaData.Length)
            {
                // Depending on whether the area is a rectangle or triangle
                i += areaData[i] == 0 ? 5 : 4;
                areaCount++;
            }

            return areaCount;
        }

        private int GetAreaDataLength()
        {
            int areaDataLength = 0;

            foreach (TrackAIElement aiElement in _aiElements)
            {
                areaDataLength += aiElement.AreaShape == TrackAIElementShape.Rectangle ? 5 : 4;
            }

            return areaDataLength;
        }

        /// <summary>
        /// Returns the AI data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The AI bytes.</returns>
        public byte[] GetBytes()
        {
            int areaDataLength = GetAreaDataLength() + 1; // + 1 for ending 0xFF
            int targetDataLength = _aiElements.Count * 3;
            byte[] data = new byte[areaDataLength + targetDataLength];

            int i = 0;

            foreach (TrackAIElement aiElement in _aiElements)
            {
                aiElement.GetAreaBytes(data, ref i);
            }
            data[i++] = 0xFF;

            foreach (TrackAIElement aiElement in _aiElements)
            {
                aiElement.GetTargetBytes(data, ref i);
            }

            return data;
        }
    }
}
