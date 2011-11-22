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
using System.IO;

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
            int[] roadTileGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeRoadGraphics], this.themes.Length);
            //int[] backgroundTileGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.TrackBackgroundGraphics], this.themes.Length);

            byte[] commonRoadTileData = Codec.Decompress(romBuffer, offsets[Offset.CommonTilesetGraphics]);
            byte[] commonRoadTilePaletteIndexes = Themes.GetPaletteIndexes(commonRoadTileData, Theme.CommonTileCount);
            byte[][] commonRoadTileGfx = Utilities.ReadBlockGroupUntil(commonRoadTileData, Theme.TileCount, -1, 32);

            bool tileGenresRelocated = Themes.AreTileGenresRelocated(romBuffer, offsets[Offset.TileGenreLoad]);
            byte[] roadTileGenreData;
            byte[][] roadTileGenreIndexes;
            TileGenre[] commonRoadTileGenres;

            if (tileGenresRelocated)
            {
                roadTileGenreData = null;
                roadTileGenreIndexes = null;
                commonRoadTileGenres = null;
            }
            else
            {
                roadTileGenreData = Codec.Decompress(romBuffer, offsets[Offset.TileGenres]);
                roadTileGenreIndexes = Utilities.ReadBlockGroup(romBuffer, offsets[Offset.TileGenreIndexes], 2, Theme.Count * 2);
                commonRoadTileGenres = Themes.GetTileGenres(roadTileGenreData, 0, Theme.CommonTileCount);
            }

            for (int i = 0; i < this.themes.Length; i++)
            {
                // Force the length to 512 in case the color palette data in the ROM is corrupt
                byte[] colorPaletteData = Codec.Decompress(romBuffer, colorPaletteOffsets[i], 512);
                Palettes colorPalettes = new Palettes(colorPaletteData);

                byte[] roadTileData = Codec.Decompress(romBuffer, roadTileGfxOffsets[i]);

                byte[] roadTilePaletteIndexes = Themes.GetPaletteIndexes(roadTileData, Theme.ThemeTileCount);
                byte[] allRoadTilePaletteIndexes = new byte[Theme.TileCount];
                Buffer.BlockCopy(roadTilePaletteIndexes, 0, allRoadTilePaletteIndexes, 0, Theme.ThemeTileCount);
                Buffer.BlockCopy(commonRoadTilePaletteIndexes, 0, allRoadTilePaletteIndexes, Theme.ThemeTileCount, Theme.CommonTileCount);

                byte[][] roadTileGfx = Utilities.ReadBlockGroupUntil(roadTileData, Theme.TileCount, -1, 32);
                byte[][] allRoadTileGfx = new byte[Theme.TileCount][];
                Array.Copy(roadTileGfx, 0, allRoadTileGfx, 0, roadTileGfx.Length);
                Array.Copy(commonRoadTileGfx, 0, allRoadTileGfx, Theme.ThemeTileCount, commonRoadTileGfx.Length);

                // Set empty tile default graphics value
                for (int j = roadTileGfx.Length; j < Theme.ThemeTileCount; j++)
                {
                    allRoadTileGfx[j] = new byte[32];
                }

                TileGenre[] allRoadTileGenres;

                if (!tileGenresRelocated)
                {
                    int roadTileGenreIndex = roadTileGenreIndexes[i][0] + (roadTileGenreIndexes[i][1] << 8);
                    TileGenre[] roadTileGenres = Themes.GetTileGenres(roadTileGenreData, roadTileGenreIndex, roadTileGfx.Length);
                    allRoadTileGenres = new TileGenre[Theme.TileCount];
                    Array.Copy(roadTileGenres, 0, allRoadTileGenres, 0, roadTileGenres.Length);
                    Array.Copy(commonRoadTileGenres, 0, allRoadTileGenres, Theme.ThemeTileCount, commonRoadTileGenres.Length);

                    // Set empty tile default genre value
                    for (int j = roadTileGfx.Length; j < Theme.ThemeTileCount; j++)
                    {
                        allRoadTileGenres[j] = TileGenre.Road;
                    }
                }
                else
                {
                    int tileGenreOffset = offsets[Offset.TileGenres2] + i * Theme.TileCount;
                    allRoadTileGenres = GetTileGenres(romBuffer, tileGenreOffset, Theme.TileCount);
                }

                MapTile[] roadTileset = Themes.GetRoadTileset(colorPalettes, allRoadTilePaletteIndexes, allRoadTileGfx, allRoadTileGenres);

                // TODO: Add support for background tilesets
                //byte[] backgroundTileData = Codec.Decompress(romBuffer, backgroundTileGfxOffsets[i]);
                Tile[] backgroundTileset = new Tile[0];

                this.themes[i] = new Theme(names[reorder[i]], colorPalettes, roadTileset, backgroundTileset);
            }
        }

        private static bool AreTileGenresRelocated(byte[] romBuffer, int offset)
        {
            if (romBuffer[offset] == 0x5C &&
                romBuffer[offset + 1] == 0xF4 &&
                romBuffer[offset + 2] == 0x5E &&
                romBuffer[offset + 3] == 0xC8)
            {
                return true;
            }

            if (romBuffer[offset] == 0xA0 &&
                romBuffer[offset + 1] == 0xBA &&
                romBuffer[offset + 2] == 0xFD &&
                romBuffer[offset + 3] == 0xA9)
            {
                return false;
            }

            throw new InvalidDataException("Error when loading tile types.");
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

        private static byte[] GetPaletteIndexes(byte[] tileData, int count)
        {
            byte[] paletteIndexes = new byte[count];
            Buffer.BlockCopy(tileData, 0, paletteIndexes, 0, count);
            for (int i = 0; i < count; i++)
            {
                paletteIndexes[i] = (byte)(paletteIndexes[i] >> 4);
            }

            return paletteIndexes;
        }

        private static MapTile[] GetRoadTileset(Palettes colorPalettes, byte[] tilePaletteIndexes, byte[][] tileGfx, TileGenre[] tileGenres)
        {
            MapTile[] tiles = new MapTile[Theme.TileCount];

            for (int i = 0; i < tileGfx.Length; i++)
            {
                Palette palette = colorPalettes[tilePaletteIndexes[i]];
                tiles[i] = new MapTile(tileGfx[i], palette, tileGenres[i]);
            }

            return tiles;
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
