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
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks.Road
{
    /// <summary>
    /// Represents a non-animated track road tile.
    /// </summary>
    internal sealed class RoadTile : Tile
    {
        /// <summary>
        /// The first palette of the concerned palette collection.
        /// Needed to retrieve the back color (first color of the first palette).
        /// </summary>
        private readonly Palette firstPalette;

        private RoadTileGenre genre = RoadTileGenre.Road;
        public RoadTileGenre Genre
        {
            get { return this.genre; }
            set
            {
                if (this.genre == value)
                {
                    return;
                }

                if (!Enum.IsDefined(typeof(RoadTileGenre), value))
                {
                    throw new ArgumentException("Invalid tile type value: " + value.ToString("X") + ".", "value");
                }

                this.genre = value;
                this.OnPropertyChange("Genre");
            }
        }

        public RoadTile(byte[] gfx, Palette palette, RoadTileGenre genre, Palette firstPalette)
        {
            this.firstPalette = firstPalette;
            this.Graphics = gfx;
            this.Palette = palette;
            this.Genre = genre;
        }

        protected override void GenerateBitmap()
        {
            this.bitmap = GraphicsConverter.GetBitmapFrom4bppLinearReversed(this.Graphics, this.TilePalette);
        }

        protected override void GenerateGraphics()
        {
            FastBitmap fBitmap = new FastBitmap(this.bitmap);

            Palette palette = this.TilePalette;
            int pixelIndex = 0;
            for (int y = 0; y < Tile.Size; y++)
            {
                for (int x = 0; x < Tile.Size / 2; x++)
                {
                    RomColor color1 = (RomColor)fBitmap.GetPixel(x * 2, y);
                    RomColor color2 = (RomColor)fBitmap.GetPixel(x * 2 + 1, y);
                    int colorIndex1 = Utilities.GetColorIndex(color1, palette);
                    int colorIndex2 = Utilities.GetColorIndex(color2, palette);

                    this.Graphics[pixelIndex++] = (byte)(colorIndex1 + (colorIndex2 << 4));
                }
            }

            fBitmap.Release();

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            this.UpdateBitmap();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            int xSub = x % 2;
            x /= 2;
            byte px = this.Graphics[y * 4 + x];
            int index = xSub == 0 ?
                px & 0x0F : (px & 0xF0) >> 4;

            return index;
        }

        /// <summary>
        /// Gets the actual palette to be applied on the tile.
        /// </summary>
        private Palette TilePalette
        {
            get
            {
                if (this.Palette[0] == this.firstPalette[0])
                {
                    // The first color of the palette matches the first color of the first palette.
                    // Optimization, avoid creating a new palette.
                    return this.Palette;
                }

                // When a tile uses the first color of the palette, the color actually applied
                // is the first color of the first palette of the collection.
                // The first color of the other palettes are ignored / never displayed.
                Palette palette = new Palette(this.Palette.Collection, this.Palette.GetBytes());
                palette[0] = this.firstPalette[0];
                return palette;
            }
        }

        /// <summary>
        /// Gets or sets the tile palette association byte, the way it's stored in the ROM.
        /// </summary>
        public byte PaletteByte
        {
            get { return (byte)(this.Palette.Index << 4); }
            set { this.Palette = this.Palette.Collection[(value >> 4)]; }
        }
    }
}
