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
        private byte[][] graphics;

        public TrackObjectGraphics(byte[] romBuffer, Offsets offsets)
        {
            int typeCount = Enum.GetValues(typeof(ObjectType)).Length;
            int count = typeCount + 2; // + 2 for moving Match Race object and items
            this.graphics = new byte[count][];

            for (int i = 0; i < typeCount; i++)
            {
                int offset = TrackObjectGraphics.GetObjectGraphicsOffset((ObjectType)i, romBuffer, offsets);
                this.graphics[i] = Codec.Decompress(romBuffer, offset);
            }

            this.graphics[this.graphics.Length - 2] = Codec.Decompress(romBuffer, offsets[Offset.MatchRaceObjectGraphics]);
            this.graphics[this.graphics.Length - 1] = Codec.Decompress(romBuffer, offsets[Offset.ItemGraphics]);
        }

        private byte[] GetObjectGraphics(ObjectType tileset)
        {
            return this.graphics[(int)tileset];
        }

        private byte[] GetMatchRaceObjectGraphics()
        {
            return this.graphics[this.graphics.Length - 2];
        }

        private byte[] GetItemGraphics()
        {
            return this.graphics[this.graphics.Length - 1];
        }

        public Bitmap GetObjectImage(GPTrack track)
        {
            byte[] gfx = this.GetObjectGraphics(track.ObjectTileset);
            Palette palette = track.ObjectPalette;
            int[] tileIndexes = TrackObjectGraphics.GetObjectTileIndexes(track.ObjectTileset);

            return TrackObjectGraphics.GetObjectImage(gfx, tileIndexes, palette);
        }

        public Bitmap GetMatchRaceObjectImage(Theme theme)
        {
            byte[] gfx = this.GetMatchRaceObjectGraphics();
            Palette palette = theme.Palettes[12];
            int[] tileIndexes = { 0, 32, 64, 96 };

            return TrackObjectGraphics.GetObjectImage(gfx, tileIndexes, palette);
        }

        public Bitmap GetStillMatchRaceObjectImage(Theme theme)
        {
            byte[] gfx = this.GetItemGraphics();
            Palette palette = theme.Palettes[14];
            int[] tileIndexes = { 0, 32, 64, 96 };

            return TrackObjectGraphics.GetObjectImage(gfx, tileIndexes, palette);
        }

        private static int[] GetObjectTileIndexes(ObjectType type)
        {
            if (type == ObjectType.Plant || type == ObjectType.Fish)
            {
                return new int[] { 4 * 32, 5 * 32, 20 * 32, 21 * 32 };
            }

            return new int[] { 32 * 32, 33 * 32, 48 * 32, 49 * 32 };
        }

        private static Bitmap GetObjectImage(byte[] gfx, int[] tileIndexes, Palette palette)
        {
            Bitmap bitmap = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                GraphicsConverter.SetBitmapFrom4bppPlanarComposite(bitmap, gfx, tileIndexes[0], palette, 0, 0);
                GraphicsConverter.SetBitmapFrom4bppPlanarComposite(bitmap, gfx, tileIndexes[1], palette, 8, 0);
                GraphicsConverter.SetBitmapFrom4bppPlanarComposite(bitmap, gfx, tileIndexes[2], palette, 0, 8);
                GraphicsConverter.SetBitmapFrom4bppPlanarComposite(bitmap, gfx, tileIndexes[3], palette, 8, 8);
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
