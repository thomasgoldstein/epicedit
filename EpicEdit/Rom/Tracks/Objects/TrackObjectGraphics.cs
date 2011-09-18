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
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// Track object graphics manager.
    /// </summary>
    public class TrackObjectGraphics
    {
        private Tile[][] tiles;

        public TrackObjectGraphics(byte[] romBuffer, Offsets offsets)
        {
            int typeCount = Enum.GetValues(typeof(ObjectType)).Length;
            int count = typeCount + 2; // + 2 for moving Match Race object and items
            this.tiles = new TrackObjectTile[count][];
            byte[] tilesetGfx;
            int[] tileIndexes;

            for (int i = 0; i < typeCount; i++)
            {
                ObjectType type = (ObjectType)i;
                int offset = TrackObjectGraphics.GetObjectGraphicsOffset(type, romBuffer, offsets);
                tilesetGfx = Codec.Decompress(romBuffer, offset);
                tileIndexes = TrackObjectGraphics.GetObjectTileIndexes(type);
                this.tiles[i] = TrackObjectGraphics.GetTiles(tilesetGfx, tileIndexes);
            }

            tileIndexes = TrackObjectGraphics.GetMatchRaceObjectTileIndexes();

            tilesetGfx = Codec.Decompress(romBuffer, offsets[Offset.MatchRaceObjectGraphics]);
            this.tiles[this.tiles.Length - 2] = TrackObjectGraphics.GetTiles(tilesetGfx, tileIndexes);

            tilesetGfx = Codec.Decompress(romBuffer, offsets[Offset.ItemGraphics]);
            this.tiles[this.tiles.Length - 1] = TrackObjectGraphics.GetTiles(tilesetGfx, tileIndexes);
        }

        private static Tile[] GetTiles(byte[] tilesetGfx, int[] tileIndexes)
        {
            Tile[] tiles = new TrackObjectTile[tileIndexes.Length];

            for (int i = 0; i < tileIndexes.Length; i++)
            {
                byte[] gfx = new byte[32];
                Buffer.BlockCopy(tilesetGfx, tileIndexes[i], gfx, 0, 32);
                tiles[i] = new TrackObjectTile(gfx);
            }

            return tiles;
        }

        private Tile[] GetObjectTiles(ObjectType tileset)
        {
            return this.tiles[(int)tileset];
        }

        private Tile[] GetMatchRaceObjectTiles()
        {
            return this.tiles[this.tiles.Length - 2];
        }

        private Tile[] GetStillMatchRaceObjectTiles()
        {
            return this.tiles[this.tiles.Length - 1];
        }

        public Bitmap GetObjectImage(GPTrack track)
        {
            Tile[] tiles = this.GetObjectTiles(track.ObjectTileset);
            Palette palette = track.ObjectPalette;

            return TrackObjectGraphics.GetObjectImage(tiles, palette);
        }

        public Bitmap GetMatchRaceObjectImage(Theme theme)
        {
            Tile[] tiles = this.GetMatchRaceObjectTiles();
            Palette palette = theme.Palettes[12];

            return TrackObjectGraphics.GetObjectImage(tiles, palette);
        }

        public Bitmap GetStillMatchRaceObjectImage(Theme theme)
        {
            Tile[] tiles = this.GetStillMatchRaceObjectTiles();
            Palette palette = theme.Palettes[14];

            return TrackObjectGraphics.GetObjectImage(tiles, palette);
        }

        private static int[] GetObjectTileIndexes(ObjectType type)
        {
            if (type == ObjectType.Plant || type == ObjectType.Fish)
            {
                return new int[] { 4 * 32, 5 * 32, 20 * 32, 21 * 32 };
            }

            return new int[] { 32 * 32, 33 * 32, 48 * 32, 49 * 32 };
        }

        private static int[] GetMatchRaceObjectTileIndexes()
        {
            return new int[] { 0, 32, 64, 96 };
        }

        private static Bitmap GetObjectImage(Tile[] tiles, Palette palette)
        {
            if (tiles[0].Palette != palette) // Assuming all tiles use the same palette
            {
                foreach (Tile tile in tiles)
                {
                    tile.Palette = palette;
                    tile.UpdateBitmap();
                }
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

        private static int GetObjectGraphicsOffset(ObjectType tileset, byte[] romBuffer, Offsets offsets)
        {
            int offsetLocation = offsets[Offset.TrackObjectGraphics];

            switch (tileset)
            {
                case ObjectType.Pipe:
                    offsetLocation += 3;
                    break;

                case ObjectType.Thwomp:
                    offsetLocation += 18;
                    break;

                case ObjectType.Mole:
                    offsetLocation += 6;
                    break;

                case ObjectType.Plant:
                    offsetLocation += 9;
                    break;

                case ObjectType.Fish:
                    offsetLocation += 15;
                    break;

                case ObjectType.RThwomp:
                    offsetLocation += 21;
                    break;
            }

            return Utilities.BytesToOffset(romBuffer[offsetLocation],
                                           romBuffer[offsetLocation + 1],
                                           romBuffer[offsetLocation + 2]);
        }
    }
}
