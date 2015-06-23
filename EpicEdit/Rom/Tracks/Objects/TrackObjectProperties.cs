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
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Objects
{
    internal class TrackObjectProperties : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObjectType tileset;
        public ObjectType Tileset
        {
            get { return this.tileset; }
            set
            {
                if (this.tileset == value)
                {
                    return;
                }

                this.tileset = value;
                this.OnPropertyChanged("Tileset");
            }
        }

        private ObjectType interaction;
        public ObjectType Interaction
        {
            get { return this.interaction; }
            set
            {
                if (this.interaction == value)
                {
                    return;
                }

                this.interaction = value;
                this.OnPropertyChanged("Interaction");
            }
        }

        private ObjectType routine;
        public ObjectType Routine
        {
            get { return this.routine; }
            set
            {
                if (this.routine == value)
                {
                    return;
                }

                this.routine = value;
                this.OnPropertyChanged("Routine");
            }
        }

        public ByteArray PaletteIndexes { get; private set; }

        public Palette Palette
        {
            get { return this.palettes[this.PaletteIndexes[0] + Palettes.SpritePaletteStart]; }
        }

        private bool flashing;
        /// <summary>
        /// If true, the object sprites will loop through 4 color palettes at 60 FPS
        /// (like the Rainbow Road Thwomps do).
        /// </summary>
        public bool Flashing
        {
            get { return this.flashing; }
            set
            {
                if (this.flashing == value)
                {
                    return;
                }

                this.flashing = value;
                this.OnPropertyChanged("Flashing");
            }
        }

        public ObjectLoading Loading
        {
            get
            {
                switch (this.Routine)
                {
                    case ObjectType.Pipe:
                    case ObjectType.Thwomp:
                    case ObjectType.Mole:
                    case ObjectType.Plant:
                    case ObjectType.RThwomp:
                        return ObjectLoading.Regular;

                    case ObjectType.Fish:
                        return ObjectLoading.Fish;

                    case ObjectType.Pillar:
                        return ObjectLoading.Pillar;

                    default:
                        return ObjectLoading.None;
                }
            }
        }

        private readonly Palettes palettes;

        public TrackObjectProperties(byte[] data, Palettes palettes)
        {
            this.SetBytes(data);
            this.palettes = palettes;
        }

        public void SetBytes(byte[] data)
        {
            this.Tileset = (ObjectType)data[0];
            this.Interaction = (ObjectType)data[1];
            this.Routine = (ObjectType)data[2];
            this.PaletteIndexes = new ByteArray(new[] { data[3], data[4], data[5], data[6] });
            this.PaletteIndexes.DataChanged += this.PaletteIndexes_DataChanged;
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

        private void PaletteIndexes_DataChanged(object sender, EventArgs e)
        {
            this.OnPropertyChanged("PaletteIndexes");
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    internal class ByteArray
    {
        public event EventHandler<EventArgs> DataChanged;

        private readonly byte[] data;

        public ByteArray(byte[] data)
        {
            this.data = Clone(data);
        }

        public byte this[int index]
        {
            get { return this.data[index]; }
            set
            {
                if (this.data[index] == value)
                {
                    return;
                }

                this.data[index] = value;
                this.OnDataChanged();
            }
        }

        public byte[] GetBytes()
        {
            return Clone(this.data);
        }

        private void OnDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }

        private static byte[] Clone(byte[] data)
        {
            byte[] copy = new byte[data.Length];
            Buffer.BlockCopy(data, 0, copy, 0, data.Length);
            return copy;
        }
    }
}
