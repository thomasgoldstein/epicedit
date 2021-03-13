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
using System.ComponentModel;
using System.Drawing;

namespace EpicEdit.Rom.Tracks.Start
{
    /// <summary>
    /// The starting position of the drivers on a GP track.
    /// </summary>
    internal class GPStartPosition : INotifyPropertyChanged
    {
        public const int Size = 6;
        public const int Height = 168;
        private const int PixelLimit = TrackMap.Limit * Tile.Size;
        private const int SecondRowMin = -256;
        private const int SecondRowMax = 255;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _enableBoundsChecks;

        private Point _location;
        public Point Location
        {
            get => _location;
            set
            {
                int x = value.X;
                int y = value.Y;

                if (_enableBoundsChecks)
                {
                    if (SecondRowOffset > 0)
                    {
                        if (x < Tile.Size)
                        {
                            x = Tile.Size;
                        }
                        else if (x + SecondRowOffset > PixelLimit)
                        {
                            x = PixelLimit - SecondRowOffset;
                        }
                    }
                    else
                    {
                        if (x + SecondRowOffset < Tile.Size)
                        {
                            x = Tile.Size - SecondRowOffset;
                        }
                        else if (x > PixelLimit)
                        {
                            x = PixelLimit;
                        }
                    }

                    if (y < Tile.Size)
                    {
                        y = Tile.Size;
                    }
                    else if (y > PixelLimit - Height)
                    {
                        y = PixelLimit - Height;
                    }
                }

                if (X != x || Y != y)
                {
                    _location = new Point(x, y);
                    OnPropertyChanged(PropertyNames.GPStartPosition.Location);
                }
            }
        }

        private int _secondRowOffset;
        public int SecondRowOffset
        {
            get => _secondRowOffset;
            set
            {
                if (_enableBoundsChecks)
                {
                    if (X + value < Tile.Size)
                    {
                        value = Tile.Size - X;
                    }
                    else if (X + value > PixelLimit)
                    {
                        value = PixelLimit - X;
                    }
                    else if (value < SecondRowMin)
                    {
                        value = SecondRowMin;
                    }
                    else if (value > SecondRowMax)
                    {
                        value = SecondRowMax;
                    }
                }

                if (_secondRowOffset != value)
                {
                    _secondRowOffset = value;
                    OnPropertyChanged(PropertyNames.GPStartPosition.SecondRowOffset);
                }
            }
        }

        public int X => _location.X;

        public int Y => _location.Y;

        /// <summary>
        /// Gets the left bound of the GPStartPosition, depending on the <see cref="SecondRowOffset"/>.
        /// </summary>
        public int Left => X + Math.Min(0, SecondRowOffset);

        /// <summary>
        /// Gets the right bound of the GPStartPosition, depending on the <see cref="SecondRowOffset"/>.
        /// </summary>
        public int Right => X + Math.Max(0, SecondRowOffset);

        public GPStartPosition(short x, short y, short secondRowOffset)
        {
            _enableBoundsChecks = false;

            Location = new Point(x, y);
            SecondRowOffset = secondRowOffset;

            _enableBoundsChecks = true;
        }

        public GPStartPosition(byte[] data)
        {
            SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            _enableBoundsChecks = false;

            int x = (data[1] << 8) + data[0];
            int y = (data[3] << 8) + data[2];
            Location = new Point(x, y);

            int rowOffset = data[4];
            if (data[5] != 0x00)
            {
                // All original tracks have either has 0x00 or 0xFF for the 6th byte,
                // but we would need something more flexible to match the game behavior
                // if the value is neither 0x00 nor 0xFF (something which shouldn't happen).
                rowOffset -= 256;
            }

            SecondRowOffset = rowOffset;

            _enableBoundsChecks = true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns the GPStartPosition data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The GPStartPosition bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[Size];
            data[0] = (byte)(X & 0xFF);
            data[1] = (byte)((X & 0xFF00) >> 8);
            data[2] = (byte)(Y & 0xFF);
            data[3] = (byte)((Y & 0xFF00) >> 8);

            if (SecondRowOffset < 0)
            {
                data[4] = (byte)(SecondRowOffset + 256);
                data[5] = 0xFF;
            }
            else
            {
                data[4] = (byte)SecondRowOffset;
                data[5] = 0x00;
            }

            return data;
        }

        public bool IntersectsWith(Point point)
        {
            return point.X >= Left - Tile.Size &&
                point.X <= Right + (Tile.Size - 1) &&
                point.Y >= Y - Tile.Size &&
                point.Y <= Y + Height + (Tile.Size - 1);
        }
    }
}