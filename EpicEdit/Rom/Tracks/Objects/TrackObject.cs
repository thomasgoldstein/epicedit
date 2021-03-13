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

using System.ComponentModel;
using System.Drawing;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// A track object. E.g.: a pipe, a Cheep-Cheep, a Thwomp...
    /// </summary>
    internal class TrackObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Point _location;
        public Point Location
        {
            get => _location;
            set
            {
                if (_location == value)
                {
                    return;
                }

                _location = value;
                OnPropertyChanged(PropertyNames.TrackObject.Location);
            }
        }

        /// <summary>
        /// Initializes a TrackObject.
        /// </summary>
        /// <param name="data">The byte array to get the data from.</param>
        /// <param name="index">The index to use in the byte array.</param>
        public TrackObject(byte[] data, int index)
        {
            var x = (data[index] & 0x7F);
            var y = ((data[index + 1] & 0x3F) << 1) + ((data[index] & 0x80) >> 7);
            Location = new Point(x, y);
        }

        public int X => Location.X;

        public int Y => Location.Y;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        public virtual void GetBytes(byte[] data, int index)
        {
            data[index] = (byte)(X + ((Y << 7) & 0x80));
            data[index + 1] = (byte)(Y >> 1);
        }
    }
}
