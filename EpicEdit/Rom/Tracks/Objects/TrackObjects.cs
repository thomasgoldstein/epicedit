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

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// A collection of 16 <see cref="TrackObject"/> objects, and 6 <see cref="TrackObjectMatchRace"/> objects.
    /// </summary>
    public class TrackObjects : IEnumerable<TrackObject>
    {
        private const int RegularObjectCount = 16;
        private const int MatchRaceObjectCount = 6;
        private const int ObjectCount = RegularObjectCount + MatchRaceObjectCount;
        private const int BytesPerObject = 2;
        private const int TotalBytes = ObjectCount * BytesPerObject;
        
        private TrackObject[] objects;

        public TrackObjects(byte[] data)
        {
            if (data.Length != TotalBytes)
            {
                throw new ArgumentOutOfRangeException("data");
            }

            this.objects = new TrackObject[data.Length / BytesPerObject];
            // Object coordinates are defined on 2 bytes

            for (int i = 0; i < RegularObjectCount; i++)
            {
                TrackObject trackObject = new TrackObject(data, i * BytesPerObject);
                this.objects[i] = trackObject;
            }

            for (int i = RegularObjectCount; i < ObjectCount; i++)
            {
                TrackObjectMatchRace trackObject = new TrackObjectMatchRace(data, i * BytesPerObject);
                this.objects[i] = trackObject;
            }
        }

        public IEnumerator<TrackObject> GetEnumerator()
        {
            foreach (TrackObject tObject in this.objects)
            {
                yield return tObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.objects.GetEnumerator();
        }

        public int Count
        {
            get { return this.objects.Length; }
        }

        public TrackObject this[int index]
        {
            get { return this.objects[index]; }
        }

        /// <summary>
        /// Returns the TrackObjects data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The TrackObjects bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[this.objects.Length * BytesPerObject];

            for (int i = 0; i < this.objects.Length; i++)
            {
                this.objects[i].GetBytes(data, i * BytesPerObject);
            }

            return data;
        }
    }
}
