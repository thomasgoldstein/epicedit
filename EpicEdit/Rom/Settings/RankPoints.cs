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

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Defines how many points drivers get depending on their finishing position, from 1st to 8th.
    /// </summary>
    internal class RankPoints : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const int Count = 8;
        private const int BytesPerObject = 2;
        public const int Size = Count * BytesPerObject;

        private readonly int[] points;

        public bool Modified { get; private set; }

        public RankPoints(byte[] data)
        {
            this.points = new int[RankPoints.Count];

            for (int i = 0; i < this.points.Length; i++)
            {
                this.points[i] = data[i * RankPoints.BytesPerObject];
            }
        }

        public int this[int rank]
        {
            get => this.points[rank];
            set
            {
                this.points[rank] = value;
                this.MarkAsModified();
            }
        }

        private void MarkAsModified()
        {
            this.Modified = true;
            // NOTE: Dummy property name
            this.OnPropertyChanged(PropertyNames.RankPoints.Value);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[RankPoints.Size];

            for (int i = 0; i < this.points.Length; i++)
            {
                data[i * RankPoints.BytesPerObject] = (byte)this.points[i];
            }

            return data;
        }

        public void Save(byte[] romBuffer, int offset)
        {
            if (this.Modified)
            {
                byte[] data = this.GetBytes();
                Buffer.BlockCopy(data, 0, romBuffer, offset, RankPoints.Size);
            }
        }

        public void ResetModifiedState()
        {
            this.Modified = false;
        }
    }
}
