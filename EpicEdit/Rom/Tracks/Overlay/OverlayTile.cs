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

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// Represents an element located over the track map. E.g.: a coin, an item block...
    /// </summary>
    internal class OverlayTile
    {
        public const byte None = 0xFF;

        public event EventHandler<EventArgs> DataChanged;

        private Point _location;
        public Point Location
        {
            get => _location;
            set
            {
                var x = value.X;
                var y = value.Y;

                if (x < 0)
                {
                    x = 0;
                }
                else if (x + Width > TrackMap.Size)
                {
                    x = TrackMap.Size - Width;
                }

                if (y < 0)
                {
                    y = 0;
                }
                else if (y + Height > TrackMap.Size)
                {
                    y = TrackMap.Size - Height;
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

        public int Width => Pattern.Width;

        public int Height => Pattern.Height;

        public OverlayTilePattern Pattern { get; set; }

        /// <summary>
        /// Initializes an OverlayTile.
        /// </summary>
        /// <param name="pattern">The pattern of the overlay tile.</param>
        /// <param name="location">The location of the overlay tile.</param>
        public OverlayTile(OverlayTilePattern pattern, Point location)
        {
            Pattern = pattern;
            _location = location;
        }

        public bool IntersectsWith(Point point)
        {
            return
                point.X >= X &&
                point.X < X + Width &&
                point.Y >= Y &&
                point.Y < Y + Height;
        }

        private void OnDataChanged()
        {
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        /// <param name="sizes">The collection of overlay tile sizes.</param>
        /// <param name="patterns">The collection of overlay tile patterns.</param>
        public void GetBytes(byte[] data, int index, OverlayTileSizes sizes, OverlayTilePatterns patterns)
        {
            var sizeIndex = sizes.IndexOf(Pattern.Size);
            var patternIndex = patterns.IndexOf(Pattern);
            data[index] = (byte)((byte)(sizeIndex << 6) | patternIndex);

            data[index + 1] = (byte)(X + ((Y << 7) & 0x80));
            data[index + 2] = (byte)(Y >> 1);
        }
    }
}
