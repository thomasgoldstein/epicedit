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

        private int width;
        public int Width
        {
            get => this.width;
            set => this.SetSize(value, this.height);
        }

        private int height;
        public int Height
        {
            get => this.height;
            set => this.SetSize(this.width, value);
        }

        public bool Modified { get; private set; }

        public OverlayTileSize(byte[] data)
        {
            if (data.Length != OverlayTileSize.Size)
            {
                throw new ArgumentException("Incorrect overlay tile size data size", nameof(data));
            }

            this.width = data[0];
            this.height = data[1];
            this.Modified = false;
        }

        public void SetSize(int width, int height)
        {
            int area = width * height;

            // From the documentation, only 32 bytes are loaded into VRAM per overlay tile
            if (width > 0 && height > 0 && area > 0 && area <= 32)
            {
                if (this.width != width || this.height != height)
                {
                    this.width = width;
                    this.height = height;
                    this.Modified = true;
                }
            }
            else
            {
                throw new ArgumentException("Invalid overlay tile size.");
            }
            this.Modified = false;
        }

        public void GetBytes(byte[] data, int index)
        {
            data[index] = (byte)this.width;
            data[index + 1] = (byte)this.height;
        }
    }
}
