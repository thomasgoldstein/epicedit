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

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// A collection of track groups, each of which contains several tracks.
    /// </summary>
    internal class TrackGroups : IEnumerable<TrackGroup>, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TrackGroup[] trackGroups;

        public TrackGroups()
        {
            this.trackGroups = new TrackGroup[Track.GroupCount];
        }

        public bool Modified
        {
            get
            {
                foreach (TrackGroup trackGroup in this.trackGroups)
                {
                    if (trackGroup.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void ResetModifiedState()
        {
            foreach (TrackGroup trackGroup in this.trackGroups)
            {
                trackGroup.ResetModifiedState();
            }
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
            set
            {
                // NOTE: Only meant to be called once per item, so no need to detach event handlers
                this.trackGroups[index] = value;
                this.trackGroups[index].PropertyChanged += this.OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(sender, e);
        }

        public int Count
        {
            get { return this.trackGroups.Length; }
        }
    }
}
