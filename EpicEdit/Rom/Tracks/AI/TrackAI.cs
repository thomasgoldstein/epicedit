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
using System.ComponentModel;
using System.Drawing;

using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Utility;

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

        private readonly Track track;
        private readonly List<TrackAIElement> aiElements;

        public TrackAI(byte[] zoneData, byte[] targetData, Track track)
        {
            this.aiElements = new List<TrackAIElement>();
            this.track = track;
            this.SetBytes(zoneData, targetData);
        }

        public void SetBytes(byte[] zoneData, byte[] targetData)
        {
            this.Clear();

            int i = 0; // i = iterator for zoneData
            int j = 0; // j = iterator for targetData
            while (i < zoneData.Length)
            {
                if (zoneData[i] == 0xFF)
                {
                    // HACK: Tolerate invalid data, and consider it's the end of the AI data.
                    // Without this, some of the tracks shipped with MAKE trigger an exception
                    // due to the fact the last AI element has an invalid zone shape value.
                    break;
                }

                this.Add(new TrackAIElement(zoneData, ref i, targetData, ref j));
            }
        }

        public IEnumerator<TrackAIElement> GetEnumerator()
        {
            return this.aiElements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.aiElements.GetEnumerator();
        }

        public int ElementCount
        {
            get { return this.aiElements.Count; }
        }

        /// <summary>
        /// Adds a new AI element to the element collection.
        /// </summary>
        /// <param name="aiElement">The new AI element.</param>
        public void Add(TrackAIElement aiElement)
        {
            if (this.aiElements.Count >= TrackAI.MaxElementCount)
            {
                return;
            }

            this.aiElements.Add(aiElement);
            aiElement.PropertyChanged += this.aiElement_PropertyChanged;
            this.OnElementAdded(aiElement);
        }

        /// <summary>
        /// Inserts a new AI element to the element collection at a specific index.
        /// </summary>
        /// <param name="aiElement">The new AI element.</param>
        /// <param name="index">The index of the new AI element.</param>
        public void Insert(TrackAIElement aiElement, int index)
        {
            if (this.aiElements.Count >= TrackAI.MaxElementCount)
            {
                return;
            }

            this.aiElements.Insert(index, aiElement);
            aiElement.PropertyChanged += aiElement_PropertyChanged;
            this.OnElementAdded(aiElement);
        }

        public void Remove(TrackAIElement aiElement)
        {
            aiElement.PropertyChanged -= this.aiElement_PropertyChanged;
            this.aiElements.Remove(aiElement);
            this.OnElementRemoved(aiElement);
        }

        /// <summary>
        /// Removes all the AI elements from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (TrackAIElement aiElement in this.aiElements)
            {
                aiElement.PropertyChanged -= this.aiElement_PropertyChanged;
            }

            this.aiElements.Clear();
            this.OnElementsCleared();
        }

        private void aiElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(sender, e);
            }
        }

        private void OnElementAdded(TrackAIElement value)
        {
            if (this.ElementAdded != null)
            {
                this.ElementAdded(this, new EventArgs<TrackAIElement>(value));
            }
        }

        private void OnElementRemoved(TrackAIElement value)
        {
            if (this.ElementRemoved != null)
            {
                this.ElementRemoved(this, new EventArgs<TrackAIElement>(value));
            }
        }

        private void OnElementsCleared()
        {
            if (this.ElementsCleared != null)
            {
                this.ElementsCleared(this, EventArgs.Empty);
            }
        }

        public int GetElementIndex(TrackAIElement aiElement)
        {
            return this.aiElements.IndexOf(aiElement);
        }

        /// <summary>
        /// Changes the index of a given AI element.
        /// This increments or decrements the index of all AI elements in between.
        /// </summary>
        /// <param name="indexBefore">The index of the AI element before moving it.</param>
        /// <param name="indexAfter">The index of the AI element after having moved it.</param>
        public void ChangeElementIndex(int indexBefore, int indexAfter)
        {
            TrackAIElement aiElement = this.aiElements[indexBefore];
            this.aiElements.RemoveAt(indexBefore);
            this.aiElements.Insert(indexAfter, aiElement);
            this.OnPropertyChanged(aiElement, new PropertyChangedEventArgs("Index"));
        }

        public static int ComputeTargetDataLength(byte[] zoneData)
        {
            int zoneCount = TrackAI.ComputeZoneCount(zoneData);
            int aiTargetDataLength = zoneCount * 3;
            return aiTargetDataLength;
        }

        private static int ComputeZoneCount(byte[] zoneData)
        {
            int zoneCount = 0;

            int i = 0;
            while (i < zoneData.Length)
            {
                // Depending on whether the zone is a rectangle or triangle
                i += zoneData[i] == 0 ? 5 : 4;
                zoneCount++;
            }

            return zoneCount;
        }

        private int ComputeZoneDataLength()
        {
            int zoneDataLength = 0;

            foreach (TrackAIElement aiElement in this.aiElements)
            {
                zoneDataLength += aiElement.ZoneShape == Shape.Rectangle ? 5 : 4;
            }

            return zoneDataLength;
        }

        /// <summary>
        /// Returns the AI data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The AI bytes.</returns>
        public byte[] GetBytes()
        {
            int zoneDataLength = this.ComputeZoneDataLength() + 1; // + 1 for ending 0xFF
            int targetDataLength = this.aiElements.Count * 3;
            byte[] data = new byte[zoneDataLength + targetDataLength];

            int i = 0;

            foreach (TrackAIElement aiElement in this.aiElements)
            {
                aiElement.GetZoneBytes(data, ref i);
            }
            data[i++] = 0xFF;

            foreach (TrackAIElement aiElement in this.aiElements)
            {
                aiElement.GetTargetBytes(data, ref i);
            }

            List<TrackAIElement> crossedAIElements = this.GetCrossedAIElements();
            foreach (TrackAIElement aiElement in crossedAIElements)
            {
                int elementIndex = this.aiElements.IndexOf(aiElement);

                // Tack 0x80 on the byte in order to avoid object visibility issues.
                // Without this, you'd enter another object zone as you cross the path,
                // potentially causing momentarily disappearing objects.
                // In the original game, this would happen in Mario Circuit 2: the
                // 4 pipes in front of you would briefly disappear while jumping
                // above the crossed path.
                data[zoneDataLength + (elementIndex * 3) + 2] += 0x80;
            }

            return data;
        }

        /// <summary>
        /// Get the list of AI elements that are part of a path that is crossed by another one.
        /// In the original game, this only happens with one element in Mario Circuit 2.
        /// The returned list only includes elements that belong to a different object zone
        /// than the element(s) from the path crossing them.
        /// </summary>
        /// <returns>The list of AI elements that are part of a crossed path.</returns>
        private List<TrackAIElement> GetCrossedAIElements()
        {
            List<TrackAIElement> crossedAIElements = new List<TrackAIElement>();

            GPTrack gpTrack = this.track as GPTrack;

            if (gpTrack == null || gpTrack.Objects.Routine == ObjectType.Pillar)
            {
                return crossedAIElements;
            }

            for (int aiElementIndex = 0; aiElementIndex < this.aiElements.Count; aiElementIndex++)
            {
                TrackAIElement aiElement = this.aiElements[aiElementIndex];
                Rectangle aiElementZone = aiElement.Zone;

                // Trick to make it easy to know whether 2 AI rectangles are side by side.
                aiElementZone.Inflate(1, 1);

                bool elementIsNextToAnother = true;
                // Test all forward AI elements, testing for intersections with previous elements is pointless
                // as it was already tested by the previous aiElementIndex
                for (int indexNext = aiElementIndex + 1; indexNext < this.aiElements.Count; indexNext++)
                {
                    TrackAIElement nextAIElement = this.aiElements[indexNext];

                    if (aiElementZone.IntersectsWith(nextAIElement.Zone))
                    {
                        elementIsNextToAnother = true;
                        break;
                    }

                    elementIsNextToAnother = false;
                }

                if (!elementIsNextToAnother)
                {
                    // This AI element isn't followed by another one right next to it.
                    // Let's check if there is an element in between this element and its target.

                    int x = Math.Min(aiElement.Zone.X, aiElement.Target.X);
                    int y = Math.Min(aiElement.Zone.Y, aiElement.Target.Y);
                    int width = Math.Max(aiElement.Zone.Right, aiElement.Target.X) - x;
                    int height = Math.Max(aiElement.Zone.Bottom, aiElement.Target.Y) - y;

                    Rectangle gap = new Rectangle(x, y, width, height);

                    // Only test elements preceding aiElementIndex, testing later elements causes a problem between 
                    // Bowser Castle 3 elements 27 and 28 which include a gap in the original SMK map.
                    // This would be very bad track design anyways as this would create the possibility of a shortcut.
                    for (int otherAIElementIndex = 0; otherAIElementIndex < aiElementIndex; otherAIElementIndex++)
                    {

                        if (otherAIElementIndex == aiElementIndex)
                        {
                            continue;
                        }

                        TrackAIElement otherAIElement = this.aiElements[otherAIElementIndex];

                        if (otherAIElement.Zone.IntersectsWith(gap) && !aiElementZone.IntersectsWith(otherAIElement.Zone))
                        {
                            // Only add elements that intersects with the gap and not the original AI element zone.
                            // This fixes a display issue of track objects when switching zones and also tells
                            // the AI to ignore the intersected AI zone.
                            crossedAIElements.Add(otherAIElement);
                        }
                    }
                }
            }

            return crossedAIElements;
        }
    }
}
