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
                byte[] fullRoadTilesetPaletteIndexes = new byte[Theme.TileCount];
                Buffer.BlockCopy(roadTilesetPaletteIndexes, 0, fullRoadTilesetPaletteIndexes, 0, Theme.ThemeTileCount);
                Buffer.BlockCopy(commonRoadTilesetPaletteIndexes, 0, fullRoadTilesetPaletteIndexes, Theme.ThemeTileCount, Theme.CommonTileCount);

                byte[][] roadTilesetGfx = Utilities.ReadBlockGroupUntil(roadTilesetData, Theme.TileCount, -1, 32);
                byte[][] fullRoadTilesetGfx = new byte[Theme.TileCount][];
                Array.Copy(roadTilesetGfx, 0, fullRoadTilesetGfx, 0, roadTilesetGfx.Length);
                Array.Copy(commonRoadTilesetGfx, 0, fullRoadTilesetGfx, Theme.ThemeTileCount, commonRoadTilesetGfx.Length);

                // Set empty tile default graphics value
                for (int j = roadTilesetGfx.Length; j < Theme.ThemeTileCount; j++)
                {
                    fullRoadTilesetGfx[j] = new byte[32];
                }

                int roadTileGenreIndex = roadTileGenreIndexes[i][0] + (roadTileGenreIndexes[i][1] << 8);
                TileGenre[] roadTileGenres = Themes.GetTileGenres(roadTileGenreData, roadTileGenreIndex, roadTilesetGfx.Length);
                TileGenre[] fullRoadTileGenres = new TileGenre[Theme.TileCount];
                Array.Copy(roadTileGenres, 0, fullRoadTileGenres, 0, roadTileGenres.Length);
                Array.Copy(commonRoadTileGenres, 0, fullRoadTileGenres, Theme.ThemeTileCount, commonRoadTileGenres.Length);

                // Set empty tile default genre value
                for (int j = roadTilesetGfx.Length; j < Theme.ThemeTileCount; j++)
                {
                    fullRoadTileGenres[j] = TileGenre.Road;
                }

                MapTile[] roadTileset = Themes.GetRoadTileset(colorPalettes, fullRoadTilesetPaletteIndexes, fullRoadTilesetGfx, fullRoadTileGenres);

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

        private static MapTile[] GetRoadTileset(Palettes colorPalettes, byte[] tilesetPaletteIndexes, byte[][] tilesetGfx, TileGenre[] tileGenres)
        {
            MapTile[] tiles = new MapTile[Theme.TileCount];

            for (int i = 0; i < tilesetGfx.Length; i++)
            {
                Palette palette = colorPalettes[tilesetPaletteIndexes[i]];
                tiles[i] = new MapTile(tilesetGfx[i], palette, tileGenres[i]);
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
