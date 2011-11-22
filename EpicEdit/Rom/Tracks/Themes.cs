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

            byte[] commonRoadTilesetData = Codec.Decompress(romBuffer, offsets[Offset.CommonTilesetGraphics]);
            byte[] commonRoadTilesetPaletteIndexes = Themes.GetPaletteIndexes(commonRoadTilesetData, Theme.CommonTileCount);
            byte[][] commonRoadTilesetGfx = Utilities.ReadBlockGroupUntil(commonRoadTilesetData, Theme.TileCount, -1, 32);

            byte[] roadTileGenreData = Codec.Decompress(romBuffer, offsets[Offset.TileGenres]);
            byte[][] roadTileGenreIndexes = Utilities.ReadBlockGroup(romBuffer, offsets[Offset.TileGenreIndexes], 2, Theme.Count * 2);
            TileGenre[] commonRoadTileGenres = Themes.GetTileGenres(roadTileGenreData, 0, Theme.CommonTileCount);

            for (int i = 0; i < this.themes.Length; i++)
            {
                // Force the length to 512 in case the color palette data in the ROM is corrupt
                byte[] colorPaletteData = Codec.Decompress(romBuffer, colorPaletteOffsets[i], 512);
                Palettes colorPalettes = new Palettes(colorPaletteData);

                byte[] roadTilesetData = Codec.Decompress(romBuffer, roadTilesetGfxOffsets[i]);
                byte[] roadTilesetPaletteIndexes = Themes.GetPaletteIndexes(roadTilesetData, Theme.ThemeTileCount);
                byte[][] roadTilesetGfx = Utilities.ReadBlockGroupUntil(roadTilesetData, Theme.TileCount, -1, 32);

                int roadTileGenreIndex = roadTileGenreIndexes[i][0] + (roadTileGenreIndexes[i][1] << 8);
                TileGenre[] roadTileGenres = Themes.GetTileGenres(roadTileGenreData, roadTileGenreIndex, roadTilesetGfx.Length);

                MapTile[] roadTileset = Themes.GetRoadTileset(colorPalettes,
                                                              roadTileGenres, roadTilesetPaletteIndexes, roadTilesetGfx,
                                                              commonRoadTileGenres, commonRoadTilesetPaletteIndexes, commonRoadTilesetGfx);

                // TODO: Add support for background tilesets
                //byte[] backgroundTilesetData = Codec.Decompress(romBuffer, backgroundTilesetGfxOffsets[i]);
                Tile[] backgroundTileset = new Tile[0];

                this.themes[i] = new Theme(names[reorder[i]], colorPalettes, roadTileset, backgroundTileset);
            }
        }

        private static TileGenre[] GetTileGenres(byte[] data, int start, int size)
        {
            TileGenre[] tileGenres = new TileGenre[size];

            for (int i = 0; i < size; i++)
            {
                tileGenres[i] = (TileGenre)data[start + i];
            }

            return tileGenres;
        }

        private static byte[] GetPaletteIndexes(byte[] tilesetData, int count)
        {
            byte[] paletteIndexes = new byte[count];
            Buffer.BlockCopy(tilesetData, 0, paletteIndexes, 0, count);
            for (int i = 0; i < count; i++)
            {
                paletteIndexes[i] = (byte)(paletteIndexes[i] >> 4);
            }

            return paletteIndexes;
        }

        private static MapTile[] GetRoadTileset(Palettes colorPalettes, TileGenre[] tileGenres, byte[] tilesetPaletteIndexes, byte[][] tilesetGfx,
                                                                        TileGenre[] commonTileGenres, byte[] commonTilesetPaletteIndexes, byte[][] commonTilesetGfx)
        {
            MapTile[] tiles = new MapTile[Theme.TileCount];

            // Get the tiles that are specific to this tileset
            Themes.SetRoadTileset(tiles, colorPalettes, tileGenres, tilesetPaletteIndexes, tilesetGfx, 0, Theme.ThemeTileCount);

            // Get the tiles that are common to all tilesets
            Themes.SetRoadTileset(tiles, colorPalettes, commonTileGenres, commonTilesetPaletteIndexes, commonTilesetGfx, Theme.ThemeTileCount, Theme.CommonTileCount);

            return tiles;
        }

        private static void SetRoadTileset(MapTile[] tiles, Palettes colorPalettes, TileGenre[] tileGenres, byte[] tilesetPaletteIndexes, byte[][] tilesetGfx, int tileIndex, int tileCount)
        {
            for (int i = 0; i < tilesetGfx.Length; i++)
            {
                Palette palette = colorPalettes[tilesetPaletteIndexes[i]];
                tiles[tileIndex + i] = new MapTile(tilesetGfx[i], palette, tileGenres[i]);
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
                    tiles[tileIndex + i] = new MapTile(image, palette, TileGenre.Road);
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
