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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

using EpicEdit.Rom.Compression;
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents a collection of <see cref="Theme">themes</see>.
    /// </summary>
    public sealed class Themes : IDisposable, IEnumerable<Theme>
    {
        private Theme[] themes;

        public Theme this[int index]
        {
            get { return this.themes[index]; }
        }

        public int Count
        {
            get { return this.themes.Length; }
        }

        public Themes(byte[] romBuffer, Offsets offsets, string[] names)
        {
            this.LoadThemes(romBuffer, offsets, names);
        }

        private void LoadThemes(byte[] romBuffer, Offsets offsets, string[] names)
        {
            this.themes = new Theme[Theme.Count];

            // TODO: Retrieve order dynamically from the ROM
            int[] reorder = { 5, 4, 6, 9, 8, 10, 7, 12 }; // To reorder the themes, as they're not in the same order as the names

            int[] colorPaletteOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeColorPalettes], this.themes.Length);
            int[] roadTilesetGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeRoadGraphics], this.themes.Length);
            //int[] backgroundTilesetGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.TrackBackgroundGraphics], this.themes.Length);

            byte[] roadCommonTilesetData = Codec.Decompress(romBuffer, offsets[Offset.CommonTilesetGraphics]);
            byte[] roadCommonTilesetPaletteIndexes = Themes.GetPaletteIndexes(roadCommonTilesetData);
            byte[][] roadCommonTilesetGfx = Utilities.ReadBlockGroupUntil(roadCommonTilesetData, 0x100, -1, 32);

            for (int i = 0; i < this.themes.Length; i++)
            {
                // Force the length to 512 in case the color palette data in the ROM is corrupt
                byte[] colorPaletteData = Codec.Decompress(romBuffer, colorPaletteOffsets[i], 512);
                Palettes colorPalettes = new Palettes(colorPaletteData);

                byte[] roadTilesetData = Codec.Decompress(romBuffer, roadTilesetGfxOffsets[i]);
                byte[] roadTilesetPaletteIndexes = Themes.GetPaletteIndexes(roadTilesetData);
                byte[][] roadTilesetGfx = Utilities.ReadBlockGroupUntil(roadTilesetData, 0x100, -1, 32);

                Tile[] roadTileset = Themes.GetRoadTileset(colorPalettes,
                                                           roadTilesetPaletteIndexes, roadTilesetGfx,
                                                           roadCommonTilesetPaletteIndexes, roadCommonTilesetGfx);

                // TODO: Add support for background tilesets
                //byte[] backgroundTilesetData = Codec.Decompress(romBuffer, backgroundTilesetGfxOffsets[i]);
                Tile[] backgroundTileset = new Tile[0];

                this.themes[i] = new Theme(names[reorder[i]], colorPalettes, roadTileset, backgroundTileset);
            }
        }

        private static byte[] GetPaletteIndexes(byte[] tilesetData)
        {
            byte[] paletteIndexes = new byte[0x100];
            Buffer.BlockCopy(tilesetData, 0, paletteIndexes, 0, paletteIndexes.Length);
            for (int i = 0; i < paletteIndexes.Length; i++)
            {
                paletteIndexes[i] = (byte)(paletteIndexes[i] >> 4);
            }

            return paletteIndexes;
        }

        private static Tile[] GetRoadTileset(Palettes colorPalettes, byte[] tilesetPaletteIndexes, byte[][] tilesetGfx, byte[] commonTilesetPaletteIndexes, byte[][] commonTilesetGfx)
        {
            Tile[] tiles = new Tile[256];

            // Get the tiles that are specific to this tileset
            Themes.SetRoadTileset(tiles, colorPalettes, tilesetPaletteIndexes, tilesetGfx, 0, 192);

            // Get the tiles that are common to all tilesets
            Themes.SetRoadTileset(tiles, colorPalettes, commonTilesetPaletteIndexes, commonTilesetGfx, 192, 64);

            return tiles;
        }

        private static void SetRoadTileset(Tile[] tiles, Palettes colorPalettes, byte[] tilesetPaletteIndexes, byte[][] tilesetGfx, int tileIndex, int tileCount)
        {
            for (int i = 0; i < tilesetGfx.Length; i++)
            {
                Palette palette = colorPalettes[tilesetPaletteIndexes[i]];
                tiles[tileIndex + i] = new StillTile(palette, tilesetGfx[i], TileGenre.Road);
            }
            
            if (tilesetGfx.Length < tileCount) // The tileset isn't full, there are missing tiles
            {
                Palette palette = colorPalettes[0];

                Bitmap emptyTile = new Bitmap(Tile.Size, Tile.Size, PixelFormat.Format32bppPArgb);
                // Turns bitmap black
                FastBitmap fBitmap = new FastBitmap(emptyTile);
                fBitmap.Release();

                Rectangle tileRectangle = new Rectangle(0, 0, Tile.Size, Tile.Size);

                for (int i = tilesetGfx.Length; i < tileCount; i++)
                {
                    // Fill in the rest of the tileset with empty (black) tiles
                    Bitmap image = emptyTile.Clone(tileRectangle, emptyTile.PixelFormat);
                    tiles[tileIndex + i] = new StillTile(palette, image, TileGenre.Road);
                }
            }
        }

        public byte GetThemeId(Theme theme)
        {
            for (byte i = 0; i < this.themes.Length; i++)
            {
                if (this.themes[i] == theme)
                {
                    return (byte)(i << 1);
                }
            }

            throw new MissingMemberException("Theme not found.");
        }

        public IEnumerator<Theme> GetEnumerator()
        {
            foreach (Theme theme in this.themes)
            {
                yield return theme;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.themes.GetEnumerator();
        }

        public void Dispose()
        {
            foreach (Theme theme in this.themes)
            {
                theme.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
