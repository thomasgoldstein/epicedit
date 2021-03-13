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

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// Represents the dimensions of an overlay tile.
    /// </summary>
    internal class OverlayTileSize
    {
        public const int Size = 2;

        private int _width;
        public int Width
        {
            get => _width;
            set => SetSize(value, _height);
        }

        private int _height;
        public int Height
        {
            get => _height;
            set => SetSize(_width, value);
        }

        public bool Modified { get; private set; }

        public OverlayTileSize(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException("Incorrect overlay tile size data size", nameof(data));
            }

            _width = data[0];
            _height = data[1];
            Modified = false;
        }

        public void SetSize(int width, int height)
        {
            var area = width * height;

            // From the documentation, only 32 bytes are loaded into VRAM per overlay tile
            if (width > 0 && height > 0 && area > 0 && area <= 32)
            {
                if (_width != width || _height != height)
                {
                    _width = width;
                    _height = height;
                    Modified = true;
                }
            }
            else
            {
                throw new ArgumentException("Invalid overlay tile size.");
            }
            Modified = false;
        }

        public void GetBytes(byte[] data, int index)
        {
            data[index] = (byte)_width;
            data[index + 1] = (byte)_height;
        }
    }
}
