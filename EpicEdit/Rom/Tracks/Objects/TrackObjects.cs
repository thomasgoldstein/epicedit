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

using EpicEdit.Rom.Tracks.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// A collection of 16 <see cref="TrackObject"/> objects, and 6 <see cref="TrackObjectMatchRace"/> objects.
    /// </summary>
    internal class TrackObjects : IEnumerable<TrackObject>, INotifyPropertyChanged
    {
        public const int RegularObjectCount = 16;
        public const int MatchRaceObjectCount = 6;
        public const int ObjectCount = RegularObjectCount + MatchRaceObjectCount;
        private const int BytesPerObject = 2;
        public const int Size = ObjectCount * BytesPerObject;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TrackObject[] _objects;

        public TrackObjectAreas Areas { get; }

        public TrackObjectProperties Properties { get; }

        public TrackObjectType Tileset
        {
            get => Properties.Tileset;
            set => Properties.Tileset = value;
        }

        public TrackObjectType Interaction
        {
            get => Properties.Interaction;
            set => Properties.Interaction = value;
        }

        public TrackObjectType Routine
        {
            get => Properties.Routine;
            set => Properties.Routine = value;
        }

        public ByteArray PaletteIndexes => Properties.PaletteIndexes;

        public Palette Palette => Properties.Palette;

        public bool Flashing
        {
            get => Properties.Flashing;
            set => Properties.Flashing = value;
        }

        public TrackObjectLoading Loading => Properties.Loading;

        public TrackObjects(byte[] data, byte[] areaData, TrackAI ai, byte[] propData, GPTrack track)
        {
            _objects = new TrackObject[Size / BytesPerObject];
            SetBytes(data);

            Areas = new TrackObjectAreas(areaData, ai);
            Areas.PropertyChanged += SubPropertyChanged;

            Properties = new TrackObjectProperties(propData, track);
            Properties.PropertyChanged += SubPropertyChanged;
        }

        public void SetBytes(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException("Incorrect track object data size", nameof(data));
            }

            for (var i = 0; i < RegularObjectCount; i++)
            {
                if (_objects[i] != null)
                {
                    _objects[i].PropertyChanged -= SubPropertyChanged;
                }

                _objects[i] = new TrackObject(data, i * BytesPerObject);
                _objects[i].PropertyChanged += SubPropertyChanged;
            }

            for (var i = RegularObjectCount; i < ObjectCount; i++)
            {
                if (_objects[i] != null)
                {
                    _objects[i].PropertyChanged -= SubPropertyChanged;
                }

                _objects[i] = new TrackObjectMatchRace(data, i * BytesPerObject);
                _objects[i].PropertyChanged += SubPropertyChanged;
            }
        }

        private void SubPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public IEnumerator<TrackObject> GetEnumerator()
        {
            foreach (var tObject in _objects)
            {
                yield return tObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _objects.GetEnumerator();
        }

        public int Count => _objects.Length;

        public TrackObject this[int index] => _objects[index];

        /// <summary>
        /// Returns the TrackObjects data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The TrackObjects bytes.</returns>
        public byte[] GetBytes()
        {
            var data = new byte[_objects.Length * BytesPerObject];

            for (var i = 0; i < _objects.Length; i++)
            {
                _objects[i].GetBytes(data, i * BytesPerObject);
            }

            return data;
        }
    }
}
