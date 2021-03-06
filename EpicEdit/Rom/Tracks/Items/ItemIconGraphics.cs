﻿#region GPL statement
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

using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Utility;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.Rom.Tracks.Items
{
    /// <summary>
    /// Item icon graphics manager.
    /// </summary>
    internal sealed class ItemIconGraphics : IDisposable
    {
        private readonly Tile2bpp _topBorder;
        private readonly Tile[][] _tiles;

        public ItemIconGraphics(byte[] romBuffer, Offsets offsets)
        {
            var itemGfx = Codec.Decompress(romBuffer, offsets[Offset.ItemIconGraphics]);
            var itemCount = Enum.GetValues(typeof(ItemType)).Length;
            var startOffset = offsets[Offset.ItemIconTileLayout];
            _tiles = new Tile2bpp[itemCount][];

            for (var i = 0; i < itemCount; i++)
            {
                var offset = startOffset + i * 2;
                _tiles[i] = GetTiles(romBuffer, offset, itemGfx);
            }

            var topBorderOffset = offsets[Offset.TopBorderTileLayout];
            var tileIndex = (byte)(romBuffer[topBorderOffset] & 0x7F);
            var properties = romBuffer[topBorderOffset + 1];
            _topBorder = GetTile(tileIndex, properties, itemGfx);
        }

        private static Tile[] GetTiles(byte[] romBuffer, int offset, byte[] itemGfx)
        {
            var tileIndex = (byte)(romBuffer[offset] & 0x7F);
            var properties = romBuffer[offset + 1];

            Tile[] tiles = new Tile2bpp[4];

            for (var i = 0; i < tiles.Length; i++)
            {
                tiles[i] = GetTile((byte)(tileIndex + i), properties, itemGfx);
            }

            return tiles;
        }

        private static Tile2bpp GetTile(byte tileIndex, byte properties, byte[] itemGfx)
        {
            const int bytesPerTile = 16;
            var gfx = Utilities.ReadBlock(itemGfx, tileIndex * bytesPerTile, bytesPerTile);
            return new Tile2bpp(gfx, properties);
        }

        private Tile[] GetTiles(ItemType type)
        {
            return _tiles[(int)type];
        }

        public Bitmap GetImage(ItemType type, Palettes palettes)
        {
            var tiles = GetTiles(type);

            return GetImage(tiles, palettes);
        }

        private static Bitmap GetImage(Tile[] tiles, Palettes palettes)
        {
            foreach (Tile2bpp tile in tiles)
            {
                tile.Palettes = palettes;
            }

            var bitmap = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(tiles[0].Bitmap, 0, 0);
                g.DrawImage(tiles[1].Bitmap, 8, 0);
                g.DrawImage(tiles[2].Bitmap, 0, 8);
                g.DrawImage(tiles[3].Bitmap, 8, 8);
            }

            return bitmap;
        }

        public Tile GetTile(ItemType type, int x, int y)
        {
            var tiles = _tiles[(int)type];
            var subIndex = (y * 2) + x;
            return tiles[subIndex];
        }

        public Bitmap GetTopBorder(Palettes palettes)
        {
            _topBorder.Palettes = palettes;
            return _topBorder.Bitmap;
        }

        public void Dispose()
        {
            foreach (var tiles in _tiles)
            {
                foreach (var tile in tiles)
                {
                    tile.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
