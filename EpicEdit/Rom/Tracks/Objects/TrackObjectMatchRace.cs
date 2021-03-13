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

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// A type of track object that only appears in Match Race mode.
    /// </summary>
    internal class TrackObjectMatchRace : TrackObject
    {
        private TrackObjectDirection _direction;

        /// <summary>
        /// Gets or sets the object direction.
        /// </summary>
        public TrackObjectDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction == value)
                {
                    return;
                }

                _direction = value;
                OnPropertyChanged(PropertyNames.TrackObjectMatchRace.Direction);
            }
        }

        /// <summary>
        /// Initializes a TrackObjectMatchRace.
        /// </summary>
        /// <param name="data">The byte array to get the data from.</param>
        /// <param name="index">The index to use in the byte array.</param>
        public TrackObjectMatchRace(byte[] data, int index)
            : base(data, index)
        {
            if ((data[index + 1] & 0x80) == 0)
            {
                _direction = (data[index + 1] & 0x40) == 0 ?
                    TrackObjectDirection.Horizontal : TrackObjectDirection.Vertical;
            }
            else
            {
                _direction = TrackObjectDirection.None;
            }
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        public override void GetBytes(byte[] data, int index)
        {
            base.GetBytes(data, index);

            if (_direction == TrackObjectDirection.Vertical)
            {
                data[index + 1] += 0x40;
            }
            else if (_direction == TrackObjectDirection.None)
            {
                data[index + 1] += 0x80;
            }
        }
    }
}
