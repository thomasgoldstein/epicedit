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
using System.Drawing;
using System.Drawing.Imaging;

using EpicEdit.Rom.Compression;

namespace EpicEdit.Rom.Tracks.Items
{
    /// <summary>
    /// Item icon graphics manager.
    /// </summary>
    internal sealed class ItemIconGraphics : IDisposable
    {
        private Tile[][] tiles;

        public ItemIconGraphics(byte[] romBuffer, Offsets offsets)
        {
            byte[] itemGfx = Codec.Decompress(romBuffer, offsets[Offset.ItemIconGraphics]);
            int itemCount = Enum.GetValues(typeof(ItemType)).Length;
            int paletteOffsetStart = offsets[Offset.ItemIconTilesPalettes];
            this.tiles = new ItemIconTile[itemCount][];

            for (int i = 0; i < itemCount; i++)
            {
                int paletteOffset = paletteOffsetStart + i * 2;
                this.tiles[i] = ItemIconGraphics.GetTiles(romBuffer, paletteOffset, itemGfx);
            }
        }

        private static Tile[] GetTiles(byte[] romBuffer, int paletteOffset, byte[] itemGfx)
        {
            int tileIndex = romBuffer[paletteOffset] & 0x7F;
            byte globalPalIndex = romBuffer[paletteOffset + 1];
            int palIndex = globalPalIndex / 16;
            int subPalIndex = globalPalIndex % 16;

            Tile[] tiles = new ItemIconTile[4];
            int bytesPerTile = 16;

            for (int i = 0; i < tiles.Length; i++)
            {
                byte[] gfx = new byte[bytesPerTile];
                Buffer.BlockCopy(itemGfx, (tileIndex + i) * bytesPerTile, gfx, 0, bytesPerTile);
                tiles[i] = new ItemIconTile(gfx, palIndex, subPalIndex);
            }

            return tiles;
        }

        private Tile[] GetTiles(ItemType type)
        {
            return this.tiles[(int)type];
        }

        public Bitmap GetImage(ItemType type, Palettes palettes)
        {
            Tile[] tiles = this.GetTiles(type);

            return ItemIconGraphics.GetImage(tiles, palettes);
        }

        private static Bitmap GetImage(Tile[] tiles, Palettes palettes)
        {
            foreach (ItemIconTile tile in tiles)
            {
                tile.SetPalette(palettes);
            }

            Bitmap bitmap = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
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
            Tile[] tiles = this.tiles[(int)type];
            int subIndex = (y * 2) + x;
            return tiles[subIndex];
        }

        public void UpdateTiles(Palette palette)
        {
            foreach (Tile[] tiles in this.tiles)
            {
                foreach (Tile tile in tiles)
                {
                    if (tile.Palette == palette)
                    {
                        tile.UpdateBitmap();
                    }
                }
            }
        }

        public void Dispose()
        {
            foreach (Tile[] tiles in this.tiles)
            {
                foreach (Tile tile in tiles)
                {
                    tile.Dispose();
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
