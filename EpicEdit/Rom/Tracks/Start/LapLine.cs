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
    /// The lap line of a track.
    /// </summary>
    internal class LapLine
    {
        public const int Size = 6;

        /// <summary>
        /// The precision X for the lap line (16 pixels).
        /// </summary>
        private const int PrecisionX = 16;

        /// <summary>
        /// The precision (X, width and height) used for the area rectangle around the lap line.
        /// 2 tiles (or 16 pixels).
        /// </summary>
        private const int AreaPrecision = PrecisionX / Tile.Size;

        /// <summary>
        /// The precision (Y) used for the area rectangle around the lap line.
        /// 8 tiles (or 64 pixels).
        /// </summary>
        private const int AreaPrecisionY = 8;

        public event EventHandler<EventArgs> DataChanged;

        private Point _location;
        public Point Location
        {
            get => _location;
            set
            {
                // Divide x precision, so that the lap line
                // is horizontally positioned following a 2-tile (16-px) step
                var x = (value.X / PrecisionX) * PrecisionX;
                var y = value.Y;

                if (x < 0)
                {
                    x = 0;
                }
                else if (x + Length > TrackMap.Size * Tile.Size)
                {
                    x = TrackMap.Size * Tile.Size - Length;
                }

                if (y < 0)
                {
                    y = 0;
                }
                else if (y >= TrackMap.Size * Tile.Size)
                {
                    y = TrackMap.Size * Tile.Size - 1;
                }

                if (X != x || Y != y)
                {
                    _location = new Point(x, y);
                    OnDataChanged();
                }
            }
        }

        private int _length;
        public int Length
        {
            get => _length;
            private set
            {
                if (value < PrecisionX)
                {
                    _length = PrecisionX;
                }
                else
                {
                    _length = value;
                }
            }
        }

        public int X => _location.X;

        public int Y => _location.Y;

        /// <summary>
        /// Gets the x-coordinate that is the sum of X and Length property values of this LapLine.
        /// </summary>
        public int Right => X + Length;

        public LapLine(byte[] data)
        {
            SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            // In the game, the lap line data consists of a vertical value (the 2 first bytes),
            // and a rectangle (the 4 following bytes: x, y, width, height).
            // The only point of interest is where the vertical value intersects with
            // the rectangle, so, to simplify things, we only load the data as a line
            // defined by a point and a length, ignoring the 2 last bytes.

            Length = (data[4] & 0x3F) * PrecisionX;

            var y = (((data[1] & 0x03) << 8) + data[0]);
            var x = (data[2] & 0x3F) * PrecisionX;
            Location = new Point(x, y);
            // The bit mask on x is required for some of the original SMK track lap line areas
            // to work properly, as some of them have the 2 highest bits needlessly set to 1.
            // So it's necessary to only use the 6 lowest bits, like the game does.
        }

        public bool IntersectsWith(Point point)
        {
            return point.X >= X - Tile.Size &&
                point.X <= Right + (Tile.Size - 1) &&
                point.Y >= Y - Tile.Size &&
                point.Y <= Y + (Tile.Size - 1);
        }

        public ResizeHandle GetResizeHandle(Point point)
        {
            if (point.X <= X + (Tile.Size - 1))
            {
                return ResizeHandle.Left;
            }

            if (point.X >= Right - Tile.Size)
            {
                return ResizeHandle.Right;
            }

            return ResizeHandle.None;
        }

        public void Resize(ResizeHandle resizeHandle, int x)
        {
            // Divide x precision, so that the lap line
            // is horizontally positioned following a 2-tile (16-px) step
            x = (x / PrecisionX) * PrecisionX;

            if (resizeHandle == ResizeHandle.Left)
            {
                if (Right - x <= 0)
                {
                    x = Right - PrecisionX;
                }

                Length += X - x;
                _location = new Point(x, Y);
            }
            else if (resizeHandle == ResizeHandle.Right)
            {
                x += PrecisionX; // This makes the resizing behavior symmetrical

                if (x <= X)
                {
                    Length = PrecisionX;
                }
                else
                {
                    Length = x - X;
                }
            }

            OnDataChanged();
        }

        private void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        public byte[] GetBytes()
        {
            var data = new byte[Size];

            var y = Y;
            data[0] = (byte)(y & 0xFF);
            data[1] = (byte)(y >> 8);

            var areaX = X / PrecisionX;
            data[2] = (byte)areaX;

            var areaY = (int)Math.Round((float)Y / (Tile.Size * AreaPrecisionY)) - 1; // Precision: 1 = 8 tiles
            // The minus 1 is to make the rectangle start at least 8 tiles above the lap line Y value

            if (areaY < 0)
            {
                areaY = 0;
            }
            data[3] = (byte)areaY;

            var areaWidth = Length / PrecisionX;
            data[4] = (byte)areaWidth;

            var areaHeight = 16 / AreaPrecision; // 16 tiles
            const int trackHeight = TrackMap.Size / AreaPrecision;
            if (areaY + areaHeight > trackHeight)
            {
                areaHeight = trackHeight - areaY;
            }
            data[5] = (byte)areaHeight;

            return data;
        }
    }
}
