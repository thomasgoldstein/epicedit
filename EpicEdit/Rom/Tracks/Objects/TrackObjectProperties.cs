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

        private TrackObjectType tileset;
        public TrackObjectType Tileset
        {
            get => this.tileset;
            set
            {
                if (this.tileset == value)
                {
                    return;
                }

                this.tileset = value;
                this.OnPropertyChanged(PropertyNames.TrackObjectProperties.Tileset);
            }
        }

        private TrackObjectType interaction;
        public TrackObjectType Interaction
        {
            get => this.interaction;
            set
            {
                if (this.interaction == value)
                {
                    return;
                }

                this.interaction = value;
                this.OnPropertyChanged(PropertyNames.TrackObjectProperties.Interaction);
            }
        }

        private TrackObjectType routine;
        public TrackObjectType Routine
        {
            get => this.routine;
            set
            {
                if (this.routine == value)
                {
                    return;
                }

                this.routine = value;
                this.OnPropertyChanged(PropertyNames.TrackObjectProperties.Routine);
            }
        }

        private readonly ByteArray paletteIndexes;
        public ByteArray PaletteIndexes => this.paletteIndexes;

        public Palette Palette => this.track.Theme.Palettes[this.PaletteIndexes[0] + Palettes.SpritePaletteStart];

        private bool flashing;
        /// <summary>
        /// If true, the object sprites will loop through 4 color palettes at 60 FPS
        /// (like the Rainbow Road Thwomps do).
        /// </summary>
        public bool Flashing
        {
            get => this.flashing;
            set
            {
                if (this.flashing == value)
                {
                    return;
                }

                this.flashing = value;
                this.OnPropertyChanged(PropertyNames.TrackObjectProperties.Flashing);
            }
        }

        public TrackObjectLoading Loading
        {
            get
            {
                switch (this.Routine)
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

        private readonly GPTrack track;

        public TrackObjectProperties(byte[] data, GPTrack track)
        {
            this.paletteIndexes = new ByteArray();
            this.SetBytes(data);
            this.PaletteIndexes.DataChanged += this.PaletteIndexes_DataChanged;
            this.track = track;
        }

        public void SetBytes(byte[] data)
        {
            this.Tileset = (TrackObjectType)data[0];
            this.Interaction = (TrackObjectType)data[1];
            this.Routine = (TrackObjectType)data[2];
            this.PaletteIndexes.SetBytes(new[] { data[3], data[4], data[5], data[6] });
            this.Flashing = data[7] != 0;
        }

        public byte[] GetBytes()
        {
            return new[]
            {
                (byte)this.Tileset,
                (byte)this.Interaction,
                (byte)this.Routine,
                this.PaletteIndexes[0],
                this.PaletteIndexes[1],
                this.PaletteIndexes[2],
                this.PaletteIndexes[3],
                Flashing ? (byte)1 : (byte)0
            };
        }

        private void PaletteIndexes_DataChanged(object sender, EventArgs<int> e)
        {
            if (e.Value == 0)
            {
                // Raise an additional event for the default palette index, to be able to
                // differentiate it from the other palette indexes (used for flashing objects).
                this.OnPropertyChanged(PropertyNames.TrackObjectProperties.Palette);
            }

            this.OnPropertyChanged(PropertyNames.TrackObjectProperties.PaletteIndexes);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class ByteArray
    {
        public event EventHandler<EventArgs<int>> DataChanged;

        private byte[] data;

        public void SetBytes(byte[] data)
        {
            if (this.data == null)
            {
                this.data = new byte[data.Length];
            }

            for (int i = 0; i < data.Length; i++)
            {
                this[i] = data[i];
            }
        }

        public byte this[int index]
        {
            get => this.data[index];
            set
            {
                if (this.data[index] == value)
                {
                    return;
                }

                this.data[index] = value;
                this.OnDataChanged(index);
            }
        }

        public byte[] GetBytes()
        {
            return Clone(this.data);
        }

        private void OnDataChanged(int value)
        {
            this.DataChanged?.Invoke(this, new EventArgs<int>(value));
        }

        private static byte[] Clone(byte[] data)
        {
            byte[] copy = new byte[data.Length];
            Buffer.BlockCopy(data, 0, copy, 0, data.Length);
            return copy;
        }
    }
}
