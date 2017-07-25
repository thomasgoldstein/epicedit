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
        /// The precision (X, width and height) used for the zone rectangle around the lap line.
        /// 2 tiles (or 16 pixels).
        /// </summary>
        private const int ZonePrecision = LapLine.PrecisionX / Tile.Size;

        /// <summary>
        /// The precision (Y) used for the zone rectangle around the lap line.
        /// 8 tiles (or 64 pixels).
        /// </summary>
        private const int ZonePrecisionY = 8;

        public event EventHandler<EventArgs> DataChanged;

        private Point location;
        public Point Location
        {
            get => this.location;
            set
            {
                // Divide x precision, so that the lap line
                // is horizontally positioned following a 2-tile (16-px) step
                int x = (value.X / LapLine.PrecisionX) * LapLine.PrecisionX;
                int y = value.Y;

                if (x < 0)
                {
                    x = 0;
                }
                else if (x + this.Length > TrackMap.Size * Tile.Size)
                {
                    x = TrackMap.Size * Tile.Size - this.Length;
                }

                if (y < 0)
                {
                    y = 0;
                }
                else if (y >= TrackMap.Size * Tile.Size)
                {
                    y = TrackMap.Size * Tile.Size - 1;
                }

                if (this.X != x || this.Y != y)
                {
                    this.location = new Point(x, y);
                    this.OnDataChanged();
                }
            }
        }

        private int length;
        public int Length
        {
            get => this.length;
            private set
            {
                if (value < LapLine.PrecisionX)
                {
                    this.length = LapLine.PrecisionX;
                }
                else
                {
                    this.length = value;
                }
            }
        }

        public int X => this.location.X;

        public int Y => this.location.Y;

        /// <summary>
        /// Gets the x-coordinate that is the sum of X and Length property values of this LapLine.
        /// </summary>
        public int Right => this.X + this.Length;

        public LapLine(byte[] data)
        {
            this.SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            // In the game, the lap line data consists of a vertical value (the 2 first bytes),
            // and a rectangle (the 4 following bytes: x, y, width, height).
            // The only point of interest is where the vertical value intersects with
            // the rectangle, so, to simplify things, we only load the data as a line
            // defined by a point and a length, ignoring the 2 last bytes.

            this.Length = (data[4] & 0x3F) * LapLine.PrecisionX;

            int y = (((data[1] & 0x03) << 8) + data[0]);
            int x = (data[2] & 0x3F) * LapLine.PrecisionX;
            this.Location = new Point(x, y);
            // The bit mask on x is required for some of the original SMK track lap line zones
            // to work properly, as some of them have the 2 highest bits needlessly set to 1.
            // So it's necessary to only use the 6 lowest bits, like the game does.
        }

        public bool IntersectsWith(Point point)
        {
            return point.X >= this.X - Tile.Size &&
                point.X <= this.Right + (Tile.Size - 1) &&
                point.Y >= this.Y - Tile.Size &&
                point.Y <= this.Y + (Tile.Size - 1);
        }

        public ResizeHandle GetResizeHandle(Point point)
        {
            if (point.X <= this.X + (Tile.Size - 1))
            {
                return ResizeHandle.Left;
            }

            if (point.X >= this.Right - Tile.Size)
            {
                return ResizeHandle.Right;
            }

            return ResizeHandle.None;
        }

        public void Resize(ResizeHandle resizeHandle, int x)
        {
            // Divide x precision, so that the lap line
            // is horizontally positioned following a 2-tile (16-px) step
            x = (x / LapLine.PrecisionX) * LapLine.PrecisionX;

            if (resizeHandle == ResizeHandle.Left)
            {
                if (this.Right - x <= 0)
                {
                    x = this.Right - LapLine.PrecisionX;
                }

                this.Length += this.X - x;
                this.location = new Point(x, this.Y);
            }
            else if (resizeHandle == ResizeHandle.Right)
            {
                x += LapLine.PrecisionX; // This makes the resizing behavior symmetrical

                if (x <= this.X)
                {
                    this.Length = LapLine.PrecisionX;
                }
                else
                {
                    this.Length = x - this.X;
                }
            }

            this.OnDataChanged();
        }

        private void OnDataChanged()
        {
            this.DataChanged?.Invoke(this, EventArgs.Empty);
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[LapLine.Size];

            int y = this.Y;
            data[0] = (byte)(y & 0xFF);
            data[1] = (byte)(y >> 8);

            int zoneX = this.X / LapLine.PrecisionX;
            data[2] = (byte)zoneX;

            int zoneY = (int)Math.Round((float)this.Y / (Tile.Size * LapLine.ZonePrecisionY)) - 1; // Precision: 1 = 8 tiles
            // The minus 1 is to make the rectangle start at least 8 tiles above the lap line Y value

            if (zoneY < 0)
            {
                zoneY = 0;
            }
            data[3] = (byte)zoneY;

            int zoneWidth = this.Length / LapLine.PrecisionX;
            data[4] = (byte)zoneWidth;

            int zoneHeight = 16 / LapLine.ZonePrecision; // 16 tiles
            const int TrackHeight = TrackMap.Size / LapLine.ZonePrecision;
            if (zoneY + zoneHeight > TrackHeight)
            {
                zoneHeight = TrackHeight - zoneY;
            }
            data[5] = (byte)zoneHeight;

            return data;
        }
    }
}
