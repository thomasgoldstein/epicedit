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

        private readonly TrackGroup[] _trackGroups;

        public TrackGroups()
        {
            _trackGroups = new TrackGroup[Track.GroupCount];
        }

        public bool Modified
        {
            get
            {
                foreach (TrackGroup trackGroup in _trackGroups)
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
            foreach (TrackGroup trackGroup in _trackGroups)
            {
                trackGroup.ResetModifiedState();
            }
        }

        public IEnumerator<TrackGroup> GetEnumerator()
        {
            foreach (TrackGroup trackGroup in _trackGroups)
            {
                yield return trackGroup;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _trackGroups.GetEnumerator();
        }

        public TrackGroup this[int index]
        {
            get => _trackGroups[index];
            set
            {
                // NOTE: Only meant to be called once per item, so no need to detach event handlers
                _trackGroups[index] = value;
                _trackGroups[index].PropertyChanged += OnPropertyChanged;
            }
        }

        public Track GetTrack(int index)
        {
            int iterator = 0;
            foreach (TrackGroup trackGroup in _trackGroups)
            {
                foreach (Track track in trackGroup)
                {
                    if (index == iterator++)
                    {
                        return track;
                    }
                }
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public int Count => _trackGroups.Length;
    }
}
