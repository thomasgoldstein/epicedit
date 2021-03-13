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

using EpicEdit.Rom.Tracks.Road;
using System;
using System.Drawing;

namespace EpicEdit.Rom.Tracks.Start
{
    /// <summary>
    /// The starting position of the drivers on a battle track.
    /// </summary>
    internal class BattleStartPosition
    {
        private const int PixelLimit = TrackMap.Limit * Tile.Size;

        public event EventHandler<EventArgs> DataChanged;

        private Point _location;
        public Point Location
        {
            get => _location;
            set
            {
                var x = value.X;
                var y = value.Y;

                if (x < Tile.Size)
                {
                    x = Tile.Size;
                }
                else if (x > PixelLimit)
                {
                    x = PixelLimit;
                }

                if (y < Tile.Size)
                {
                    y = Tile.Size;
                }
                else if (y > PixelLimit)
                {
                    y = PixelLimit;
                }

                if (X != x || Y != y)
                {
                    _location = new Point(x, y);
                    OnDataChanged();
                }
            }
        }

        public int X => _location.X;

        public int Y => _location.Y;

        public BattleStartPosition(byte[] data)
        {
            SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            var x = (data[1] << 8) + data[0];
            var y = (data[3] << 8) + data[2];
            _location = new Point(x, y);
        }

        public static implicit operator Point(BattleStartPosition position)
        {
            return position._location;
        }

        private void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns the BattleStartPosition data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The BattleStartPosition bytes.</returns>
        public byte[] GetBytes()
        {
            var data = new byte[4];
            data[0] = (byte)(X & 0xFF);
            data[1] = (byte)((X >> 8) & 0xFF);
            data[2] = (byte)(Y & 0xFF);
            data[3] = (byte)((Y >> 8) & 0xFF);
            return data;
        }

        public bool IntersectsWith(Point point)
        {
            return point.X >= X - Tile.Size &&
                point.X <= X + (Tile.Size - 1) &&
                point.Y >= Y - Tile.Size &&
                point.Y <= Y + (Tile.Size - 1);
        }
    }
}
