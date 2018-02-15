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

using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Utility;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// Track object graphics manager.
    /// </summary>
    internal sealed class TrackObjectGraphics : IDisposable
    {
        private readonly Tile[][] tiles;

        public TrackObjectGraphics(byte[] romBuffer, Offsets offsets)
        {
            int typeCount = Enum.GetValues(typeof(ObjectType)).Length;
            int count = typeCount + 2; // + 2 to account for moving Match Race object and items
            this.tiles = new TrackObjectTile[count][];
            int offsetLocation = offsets[Offset.TrackObjectGraphics];
            byte[] tilesetGfx;
            int[] tileIndexes;

            for (int i = 0; i < typeCount; i++)
            {
                ObjectType type = (ObjectType)i;
                int offset = TrackObjectGraphics.GetGraphicsOffset(type, romBuffer, offsetLocation);
                tilesetGfx = Codec.Decompress(romBuffer, offset);
                tileIndexes = TrackObjectGraphics.GetTileIndexes(type);
                this.tiles[i] = TrackObjectGraphics.GetTiles(tilesetGfx, tileIndexes);
            }

            tileIndexes = TrackObjectGraphics.GetMatchRaceTileIndexes();

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
                byte[] gfx = Utilities.ReadBlock(tilesetGfx, tileIndexes[i], 32);
                tiles[i] = new TrackObjectTile(gfx);
            }

            return tiles;
        }

        private Tile[] GetTiles(ObjectType tileset)
        {
            return this.tiles[(int)tileset];
        }

        public Bitmap GetImage(GPTrack track)
        {
            Tile[] tiles = this.GetTiles(track.Objects.Tileset);
            Palette palette = track.Objects.Palette;

            return TrackObjectGraphics.GetImage(tiles, palette);
        }

        /// <summary>
        /// Gets the Match Race object image.
        /// </summary>
        /// <param name="theme">The track theme.</param>
        /// <param name="moving">True for moving Match Race object, false for still Match Race object.</param>
        /// <returns></returns>
        public Bitmap GetMatchRaceObjectImage(Theme theme, bool moving)
        {
            Tile[] tiles = this.tiles[this.GetMatchRaceTileIndex(moving)];
            Palette palette = moving ? theme.Palettes[12] : theme.Palettes[14];

            return TrackObjectGraphics.GetImage(tiles, palette);
        }

        private int GetMatchRaceTileIndex(bool moving)
        {
            return moving ? this.tiles.Length - 2 : this.tiles.Length - 1;
        }

        private static int[] GetTileIndexes(ObjectType type)
        {
            if (type == ObjectType.Plant || type == ObjectType.Fish)
            {
                return new[] { 4 * 32, 5 * 32, 20 * 32, 21 * 32 };
            }

            return new[] { 32 * 32, 33 * 32, 48 * 32, 49 * 32 };
        }

        private static int[] GetMatchRaceTileIndexes()
        {
            return new[] { 0, 32, 64, 96 };
        }

        private static Bitmap GetImage(Tile[] tiles, Palette palette)
        {
            if (tiles[0].Palette != palette) // Assuming all tiles use the same palette
            {
                foreach (Tile tile in tiles)
                {
                    tile.Palette = palette;
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

        public Tile GetTile(GPTrack track, TrackObject trackObject, int x, int y)
        {
            int index;

            if (!(trackObject is TrackObjectMatchRace matchRaceObject))
            {
                index = (int)track.Objects.Tileset;
            }
            else
            {
                bool moving = matchRaceObject.Direction != TrackObjectDirection.None;
                index = this.GetMatchRaceTileIndex(moving);
            }

            Tile[] tiles = this.tiles[index];
            int subIndex = (y * 2) + x;
            return tiles[subIndex];
        }

        private static int GetGraphicsOffset(ObjectType tileset, byte[] romBuffer, int offsetLocation)
        {
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

            return Utilities.BytesToOffset(romBuffer, offsetLocation);
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
