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
using System.Drawing;

using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks
{
    public enum TileGenre : byte
    {
        /// <summary>
        /// Jump bar.
        /// </summary>
        [Description("Jump bar")]
        JumpBar = 0x10,

        /// <summary>
        /// Choco Island jump bar.
        /// </summary>
        [Description("Jump bar 2")]
        JumpBar2 = 0x12,

        /// <summary>
        /// Part of "?" block.
        /// </summary>
        [Description("Item block")]
        ItemBlock = 0x14,

        /// <summary>
        /// Zipper.
        /// </summary>
        [Description("Zipper")]
        Zipper = 0x16,

        /// <summary>
        /// Oil slick.
        /// </summary>
        [Description("Oil slick")]
        OilSlick = 0x18,

        /// <summary>
        /// Coin.
        /// </summary>
        [Description("Coin")]
        Coin = 0x1A,

        /// <summary>
        /// Choco Island jump bar #2 (less severe speed impact seemingly).
        /// </summary>
        [Description("Jump bar 3")]
        JumpBar3 = 0x1C,

        /// <summary>
        /// Ghost Valley bumpy planks of wood where drivers line up.
        /// </summary>
        [Description("Wood plank")]
        WoodPlank = 0x1E,

        /// <summary>
        /// Ghost Valley / Rainbow Road void. Tile that you fall through, making Lakitu fish you out.
        /// </summary>
        [Description("Empty")]
        Empty = 0x20,

        /// <summary>
        /// Deep water.
        /// </summary>
        [Description("Water")]
        Water = 0x22,

        /// <summary>
        /// Bowser Castle lava.
        /// </summary>
        [Description("Lava")]
        Lava = 0x24,

        /// <summary>
        /// Lakitu will pick you up if you touch the tile.
        /// </summary>
        [Description("Out of bounds")]
        OutOfBounds = 0x26,

        /// <summary>
        /// Empty border that delimits the road and the void, on Rainbow Road.
        /// </summary>
        [Description("Empty border")]
        EmptyBorder = 0x28,

        /// <summary>
        /// Mario Circuit road (also used for other "good handling" surfaces).
        /// </summary>
        [Description("Road")]
        Road = 0x40,

        /// <summary>
        /// Ghost Valley road.
        /// </summary>
        [Description("Road 2")]
        Road2 = 0x42,

        /// <summary>
        /// Bowser Castle road.
        /// </summary>
        [Description("Road 3")]
        Road3 = 0x44,

        /// <summary>
        /// Donut Plains road.
        /// </summary>
        [Description("Road 4")]
        Road4 = 0x46,

        /// <summary>
        /// Koopa Beach wet sand.
        /// </summary>
        [Description("Wet sand")]
        WetSand = 0x48,

        /// <summary>
        /// Koopa Beach road.
        /// </summary>
        [Description("Road 5")]
        Road5 = 0x4A,

        /// <summary>
        /// Choco Island road.
        /// </summary>
        [Description("Road 6")]
        Road6 = 0x4C,

        /// <summary>
        /// Vanilla Lake road / Ghost Valley road / Bowser Castle road / Rainbow Road road.
        /// </summary>
        [Description("Road 7")]
        Road7 = 0x4E,

        /// <summary>
        /// Donut Plains bridge.
        /// </summary>
        [Description("Bridge")]
        Bridge = 0x50,

        /// <summary>
        /// "Slippery" sections of track.
        /// </summary>
        [Description("Slippery")]
        Slippery = 0x52,

        /// <summary>
        /// Mario Circuit sand.
        /// </summary>
        [Description("Sand")]
        Sand = 0x54,

        /// <summary>
        /// Choco Island offroad.
        /// </summary>
        [Description("Offroad")]
        OffRoad = 0x56,

        /// <summary>
        /// Vanilla Lake snow / Choco Island sand.
        /// </summary>
        [Description("Snow")]
        Snow = 0x58,

        /// <summary>
        /// Drivable grass.
        /// </summary>
        [Description("Grass")]
        Grass = 0x5A,

        /// <summary>
        /// Koopa Beach shallow water.
        /// </summary>
        [Description("Shallow water")]
        ShallowWater = 0x5C,

        /// <summary>
        /// Choco Island mud.
        /// </summary>
        [Description("Mud")]
        Mud = 0x5E,

        /// <summary>
        /// Solid block (like colored bricks).
        /// </summary>
        [Description("Solid block")]
        SolidBlock = 0x80,

        /// <summary>
        /// Ghost Valley boundary block that disappears when hit.
        /// </summary>
        [Description("Breakable block")]
        BreakableBlock = 0x82,

        /// <summary>
        /// Vanilla Lake breakable ice block.
        /// </summary>
        [Description("Ice block")]
        IceBlock = 0x84
    };

    /// <summary>
    /// Represents a non-animated track map tile.
    /// </summary>
    public class MapTile : Tile
    {
        private Bitmap image;

        private TileGenre genre = TileGenre.Road;
        public TileGenre Genre
        {
            get { return this.genre; }
            set
            {
                if (!Enum.IsDefined(typeof(TileGenre), value))
                {
                    throw new ArgumentException("Invalid tile type value: " + value.ToString("X") + ".", "value");
                }

                this.genre = value;
            }
        }

        public override Bitmap Bitmap
        {
            get { return this.image; }
        }

        public MapTile(byte[] gfx, Palette palette, TileGenre genre)
        {
            this.Palette = palette;
            this.graphics = gfx;
            this.Genre = genre;
            this.GenerateBitmap();
        }

        public MapTile(Bitmap image, Palette palette, TileGenre genre)
        {
            this.Palette = palette;
            this.image = image;
            this.Genre = genre;
        }

        public override void UpdateBitmap()
        {
            if (this.graphics == null)
            {
                // This is an empty tile, contains no data
                return;
            }

            this.image.Dispose();
            this.GenerateBitmap();
        }

        private void GenerateBitmap()
        {
            this.image = GraphicsConverter.GetBitmapFrom4bppLinearReversed(this.graphics, this.Palette);
        }

        public override int GetColorIndexAt(int x, int y)
        {
            if (this.graphics == null)
            {
                // Empty tile, no data
                return -1;
            }

            int xSub = x % 2;
            x /= 2;
            byte px = this.graphics[y * 4 + x];
            int index = xSub == 0 ?
                px & 0x0F : (px & 0xF0) >> 4;

            return index;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.image.Dispose();
            }
        }
    }
}
