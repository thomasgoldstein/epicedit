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

using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using System;
using System.Drawing;
using System.Drawing.Imaging;

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
        private readonly Palette _firstPalette;

        private RoadTileGenre _genre = RoadTileGenre.Road;
        public RoadTileGenre Genre
        {
            get => _genre;
            set
            {
                if (_genre == value)
                {
                    return;
                }

                if (!Enum.IsDefined(typeof(RoadTileGenre), value))
                {
                    throw new ArgumentException($"Invalid tile type value: {value:X}.", nameof(value));
                }

                _genre = value;
                OnPropertyChanged(PropertyNames.RoadTile.Genre);
            }
        }

        public RoadTile(byte[] gfx, Palette palette, RoadTileGenre genre, Palette firstPalette)
        {
            _firstPalette = firstPalette;
            Graphics = gfx;
            Palette = palette;
            Genre = genre;
        }

        protected override void GenerateBitmap()
        {
            // Format: 4bpp linear, reverse-order
            // Each tile is made up of 8x8 pixels, coded on 32 bytes (4 bits per pixel)

            var bitmap = new Bitmap(Size, Size, PixelFormat.Format32bppPArgb);
            var fBitmap = new FastBitmap(bitmap);

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var colorIndex = GetColorIndexAt(x, y);
                    var color = TilePalette[colorIndex];
                    fBitmap.SetPixel(x, y, color);
                }
            }

            fBitmap.Release();
            InternalBitmap = bitmap;
        }

        protected override void GenerateGraphics()
        {
            var fBitmap = new FastBitmap(InternalBitmap);

            var palette = TilePalette;
            var pixelIndex = 0;
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size / 2; x++)
                {
                    RomColor color1 = fBitmap.GetPixel(x * 2, y);
                    RomColor color2 = fBitmap.GetPixel(x * 2 + 1, y);
                    var colorIndex1 = Utilities.GetColorIndex(color1, palette);
                    var colorIndex2 = Utilities.GetColorIndex(color2, palette);

                    Graphics[pixelIndex++] = (byte)(colorIndex1 + (colorIndex2 << 4));
                }
            }

            fBitmap.Release();

            // Regenerate the bitmap, in case the new image contained colors
            // not present in the palettes
            UpdateBitmap();
        }

        public override int GetColorIndexAt(int x, int y)
        {
            var xSub = x % 2;
            x /= 2;
            var px = Graphics[y * 4 + x];
            var index = xSub == 0 ?
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
                if (Palette[0] == _firstPalette[0])
                {
                    // The first color of the palette matches the first color of the first palette.
                    // Optimization, avoid creating a new palette.
                    return Palette;
                }

                // When a tile uses the first color of the palette, the color actually applied
                // is the first color of the first palette of the collection.
                // The first color of the other palettes are ignored / never displayed.
                var palette = new Palette(Palette.Collection, -1, Palette.GetBytes());
                palette[0] = _firstPalette[0];
                return palette;
            }
        }

        /// <summary>
        /// Gets or sets the tile palette association byte, the way it's stored in the ROM.
        /// </summary>
        public byte PaletteByte
        {
            get => (byte)(Palette.Index << 4);
            set => Palette = Palette.Collection[(value >> 4)];
        }
    }
}
