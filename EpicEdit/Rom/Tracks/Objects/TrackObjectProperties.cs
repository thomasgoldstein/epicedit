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
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Objects
{
    internal class TrackObjectProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TrackObjectType _tileset;
        public TrackObjectType Tileset
        {
            get => _tileset;
            set
            {
                if (_tileset == value)
                {
                    return;
                }

                _tileset = value;
                OnPropertyChanged(PropertyNames.TrackObjectProperties.Tileset);
            }
        }

        private TrackObjectType _interaction;
        public TrackObjectType Interaction
        {
            get => _interaction;
            set
            {
                if (_interaction == value)
                {
                    return;
                }

                _interaction = value;
                OnPropertyChanged(PropertyNames.TrackObjectProperties.Interaction);
            }
        }

        private TrackObjectType _routine;
        public TrackObjectType Routine
        {
            get => _routine;
            set
            {
                if (_routine == value)
                {
                    return;
                }

                _routine = value;
                OnPropertyChanged(PropertyNames.TrackObjectProperties.Routine);
            }
        }

        public ByteArray PaletteIndexes { get; }

        public Palette Palette => _track.Theme.Palettes[PaletteIndexes[0] + Palettes.SpritePaletteStart];

        private bool _flashing;
        /// <summary>
        /// If true, the object sprites will loop through 4 color palettes at 60 FPS
        /// (like the Rainbow Road Thwomps do).
        /// </summary>
        public bool Flashing
        {
            get => _flashing;
            set
            {
                if (_flashing == value)
                {
                    return;
                }

                _flashing = value;
                OnPropertyChanged(PropertyNames.TrackObjectProperties.Flashing);
            }
        }

        public TrackObjectLoading Loading
        {
            get
            {
                switch (Routine)
                {
                    case TrackObjectType.Pipe:
                    case TrackObjectType.Thwomp:
                    case TrackObjectType.Mole:
                    case TrackObjectType.Plant:
                    case TrackObjectType.RThwomp:
                        return TrackObjectLoading.Regular;

                    case TrackObjectType.Fish:
                        return TrackObjectLoading.Fish;

                    case TrackObjectType.Pillar:
                        return TrackObjectLoading.Pillar;

                    default:
                        return TrackObjectLoading.None;
                }
            }
        }

        private readonly GPTrack _track;

        public TrackObjectProperties(byte[] data, GPTrack track)
        {
            PaletteIndexes = new ByteArray();
            SetBytes(data);
            PaletteIndexes.DataChanged += PaletteIndexes_DataChanged;
            _track = track;
        }

        public void SetBytes(byte[] data)
        {
            Tileset = (TrackObjectType)data[0];
            Interaction = (TrackObjectType)data[1];
            Routine = (TrackObjectType)data[2];
            PaletteIndexes.SetBytes(new[] { data[3], data[4], data[5], data[6] });
            Flashing = data[7] != 0;
        }

        public byte[] GetBytes()
        {
            return new[]
            {
                (byte)Tileset,
                (byte)Interaction,
                (byte)Routine,
                PaletteIndexes[0],
                PaletteIndexes[1],
                PaletteIndexes[2],
                PaletteIndexes[3],
                Flashing ? (byte)1 : (byte)0
            };
        }

        private void PaletteIndexes_DataChanged(object sender, EventArgs<int> e)
        {
            if (e.Value == 0)
            {
                // Raise an additional event for the default palette index, to be able to
                // differentiate it from the other palette indexes (used for flashing objects).
                OnPropertyChanged(PropertyNames.TrackObjectProperties.Palette);
            }

            OnPropertyChanged(PropertyNames.TrackObjectProperties.PaletteIndexes);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class ByteArray
    {
        public event EventHandler<EventArgs<int>> DataChanged;

        private byte[] _data;

        public void SetBytes(byte[] data)
        {
            if (_data == null)
            {
                _data = new byte[data.Length];
            }

            for (int i = 0; i < data.Length; i++)
            {
                this[i] = data[i];
            }
        }

        public byte this[int index]
        {
            get => _data[index];
            set
            {
                if (_data[index] == value)
                {
                    return;
                }

                _data[index] = value;
                OnDataChanged(index);
            }
        }

        public byte[] GetBytes()
        {
            return Clone(_data);
        }

        private void OnDataChanged(int value)
        {
            DataChanged?.Invoke(this, new EventArgs<int>(value));
        }

        private static byte[] Clone(byte[] data)
        {
            byte[] copy = new byte[data.Length];
            Buffer.BlockCopy(data, 0, copy, 0, data.Length);
            return copy;
        }
    }
}
