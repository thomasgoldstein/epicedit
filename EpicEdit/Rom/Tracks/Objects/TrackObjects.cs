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
        private TrackObject[] objects;

        public TrackObjects(byte[] data)
        {
            if (data.Length != 44)
            {
                throw new ArgumentOutOfRangeException("data");
            }

            this.objects = new TrackObject[data.Length / 2];
            // Object coordinates are defined on 2 bytes

            for (int i = 0; i < 16; i++)
            {
                TrackObject trackObject = new TrackObject(data, i * 2);
                this.objects[i] = trackObject;
            }

            for (int i = 16; i < 22; i++)
            {
                TrackObjectMatchRace trackObject = new TrackObjectMatchRace(data, i * 2);
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
            byte[] data = new byte[this.objects.Length * 2];

            for (int i = 0; i < this.objects.Length; i++)
            {
                this.objects[i].GetBytes(data, i * 2);
            }

            return data;
        }
    }
}
