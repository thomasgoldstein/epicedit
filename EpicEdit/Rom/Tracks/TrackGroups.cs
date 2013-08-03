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

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// A collection of track groups, each of which contains several tracks.
    /// </summary>
    internal class TrackGroups : IEnumerable<TrackGroup>
    {
        private TrackGroup[] trackGroups;

        public TrackGroups()
        {
            this.trackGroups = new TrackGroup[Track.GroupCount];
        }

        public IEnumerator<TrackGroup> GetEnumerator()
        {
            foreach (TrackGroup trackGroup in this.trackGroups)
            {
                yield return trackGroup;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.trackGroups.GetEnumerator();
        }

        public TrackGroup this[int index]
        {
            get { return this.trackGroups[index]; }
            set { this.trackGroups[index] = value; }
        }

        public int Count
        {
            get { return this.trackGroups.Length; }
        }
    }
}
