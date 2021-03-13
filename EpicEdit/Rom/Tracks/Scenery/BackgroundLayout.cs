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

namespace EpicEdit.Rom.Tracks.Scenery
{
    /// <summary>
    /// Represents the way background tiles are laid.
    /// </summary>
    internal class BackgroundLayout : INotifyPropertyChanged
    {
        /// <summary>
        /// The number of visible tile rows that compose a background layer.
        /// </summary>
        public const int RowCount = ActualRowCount - YStart;

        /// <summary>
        /// The number of tiles that compose the front layer row.
        /// </summary>
        public const int FrontLayerWidth = FrontLayerRowSize / TileLength;

        /// <summary>
        /// The number of tiles that compose the back layer row.
        /// </summary>
        public const int BackLayerWidth = BackLayerRowSize / TileLength;

        /// <summary>
        /// The number of tile rows that compose a background layer.
        /// </summary>
        private const int ActualRowCount = 4;

        /// <summary>
        /// Vertical index for the first visible tile row (ie: the top tile row is not visible in the game).
        /// </summary>
        private const int YStart = 1;

        /// <summary>
        /// How many bytes a background tile takes up.
        /// </summary>
        private const int TileLength = 2;

        /// <summary>
        /// The size of byte blocks, following the data is structured in the original ROM.
        /// </summary>
        private const int BlockSize = 64;

        /// <summary>
        /// The size of a group of 4 byte blocks, one for each layer row.
        /// </summary>
        private const int BlockGroupSize = BlockSize * ActualRowCount;

        /// <summary>
        /// How many bytes the front layer takes up.
        /// </summary>
        private const int FrontLayerSize = 1024;

        /// <summary>
        /// How many bytes a row of the front layer takes up.
        /// </summary>
        private const int FrontLayerRowSize = FrontLayerSize / ActualRowCount;

        /// <summary>
        /// How many bytes the back layer takes up.
        /// </summary>
        private const int BackLayerSize = 512;

        /// <summary>
        /// How many bytes a row of the back layer takes up.
        /// </summary>
        private const int BackLayerRowSize = BackLayerSize / ActualRowCount;

        /// <summary>
        /// How many bytes a whole background layout takes up.
        /// </summary>
        public const int Size = FrontLayerSize + BackLayerSize;

        public event PropertyChangedEventHandler PropertyChanged;

        private byte[][] _frontLayer;
        private byte[][] _backLayer;

        public bool Modified { get; private set; }

        public BackgroundLayout(byte[] data)
        {
            SetBytesInternal(data);
        }

        private void SetBytesInternal(byte[] data)
        {
            if (data.Length != Size)
            {
                throw new ArgumentException($"The background layout must have a length of {Size}.", nameof(data));
            }

            _frontLayer = GetLayer(data, 0, FrontLayerRowSize);
            _backLayer = GetLayer(data, FrontLayerSize, BackLayerRowSize);
        }

        public void SetBytes(byte[] data)
        {
            SetBytesInternal(data);
            MarkAsModified();
        }

        private static byte[][] GetLayer(byte[] data, int start, int rowSize)
        {
            byte[][] layer = new byte[ActualRowCount][];

            for (int y = 0; y < ActualRowCount; y++)
            {
                layer[y] = new byte[rowSize];
                int src = start + (y * BlockSize);

                for (int x = 0; x < rowSize; x += BlockSize)
                {
                    Buffer.BlockCopy(data, src, layer[y], x, BlockSize);
                    src += BlockGroupSize;
                }
            }

            return layer;
        }

        public void GetTileData(int x, int y, bool front, out byte tileId, out byte properties)
        {
            byte[][] layer = front ? _frontLayer : _backLayer;
            tileId = layer[YStart + y][x * 2];
            properties = (byte)(layer[YStart + y][x * 2 + 1] & 0xDF);
        }

        public void SetTileData(int x, int y, bool front, byte tileId, byte properties)
        {
            byte[][] layer = front ? _frontLayer : _backLayer;
            layer[YStart + y][x * 2] = tileId;
            layer[YStart + y][x * 2 + 1] = properties;

            MarkAsModified();
        }

        private void MarkAsModified()
        {
            Modified = true;
            OnPropertyChanged(PropertyNames.Background.Data);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[Size];
            SetBytes(data, _frontLayer, 0);
            SetBytes(data, _backLayer, FrontLayerSize);
            return data;
        }

        public void ResetModifiedState()
        {
            Modified = false;
        }

        private static void SetBytes(byte[] data, byte[][] layer, int start)
        {
            int rowSize = layer[0].Length;

            for (int y = 0; y < ActualRowCount; y++)
            {
                int dest = start + (y * BlockSize);
                for (int x = 0; x < rowSize; x += BlockSize)
                {
                    Buffer.BlockCopy(layer[y], x, data, dest, BlockSize);
                    dest += BlockGroupSize;
                }
            }
        }
    }
}
