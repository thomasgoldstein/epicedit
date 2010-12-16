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
    /// A group of tracks.
    /// </summary>
    public class TrackGroup : IEnumerable<Track>
    {
        private string name;
        private Track[] tracks;

        public TrackGroup(string name, Track[] tracks)
        {
            this.name = name;
            this.tracks = tracks;
        }

        public string Name
        {
            get { return this.name; }
        }

        public int Size()
        {
            return this.tracks.Length;
        }

        public IEnumerator<Track> GetEnumerator()
        {
            foreach (Track track in this.tracks)
            {
                yield return track;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.tracks.GetEnumerator();
        }

        public Track[] GetTracks()
        {
            return this.tracks;
        }

        public Track this[int index]
        {
            get { return this.tracks[index]; }
        }
    }
}
