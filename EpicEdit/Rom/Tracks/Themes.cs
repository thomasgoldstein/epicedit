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
using System.IO;

using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks.Scenery;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents a collection of <see cref="Theme">themes</see>.
    /// </summary>
    internal sealed class Themes : IDisposable, IEnumerable<Theme>
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

            int[] paletteOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemePalettes], this.themes.Length);
            int[] roadTileGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeRoadGraphics], this.themes.Length);
            int[] bgTileGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.TrackBackgroundGraphics], this.themes.Length);
            int[] bgLayoutOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.TrackBackgroundLayouts], this.themes.Length);

            bool roadTilesetHackApplied = Themes.IsRoadTilesetHackApplied(romBuffer, offsets);
            byte[] commonRoadTilePaletteIndexes;
            byte[][] commonRoadTileGfx;

            if (roadTilesetHackApplied)
            {
                commonRoadTilePaletteIndexes = null;
                commonRoadTileGfx = null;
            }
            else
            {
                int commonRoadTileUpperByte = offsets[Offset.CommonTilesetGraphicsUpperByte];
                int commonRoadTileLowerBytes = offsets[Offset.CommonTilesetGraphicsLowerBytes];
                int commonRoadTileOffset = Utilities.BytesToOffset(
                    romBuffer[commonRoadTileLowerBytes],
                    romBuffer[commonRoadTileLowerBytes + 1],
                    romBuffer[commonRoadTileUpperByte]
                   );
                byte[] commonRoadTileData = Codec.Decompress(romBuffer, commonRoadTileOffset);
                commonRoadTilePaletteIndexes = Themes.GetPaletteIndexes(commonRoadTileData, RoadTileset.CommonTileCount);
                commonRoadTileGfx = Utilities.ReadBlockGroupUntil(commonRoadTileData, RoadTileset.TileCount, -1, 32);
            }

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
                commonRoadTileGenres = Themes.GetTileGenres(roadTileGenreData, 0, RoadTileset.CommonTileCount);
            }

            for (int i = 0; i < this.themes.Length; i++)
            {
                string name = names[reorder[i]];

                // Force the length to 512 in case the color palette data in the ROM is corrupt
                byte[] paletteData = Codec.Decompress(romBuffer, paletteOffsets[i], 512);
                Palettes palettes = new Palettes(paletteData);

                byte[] roadTileData = Codec.Decompress(romBuffer, roadTileGfxOffsets[i]);
                byte[][] roadTileGfx = Utilities.ReadBlockGroupUntil(roadTileData, RoadTileset.TileCount, -1, 32);
                byte[] allRoadTilePaletteIndexes;
                byte[][] allRoadTileGfx;

                if (roadTilesetHackApplied)
                {
                    allRoadTilePaletteIndexes = Themes.GetPaletteIndexes(roadTileData, RoadTileset.TileCount);
                    allRoadTileGfx = roadTileGfx;
                }
                else
                {
                    byte[] roadTilePaletteIndexes = Themes.GetPaletteIndexes(roadTileData, RoadTileset.ThemeTileCount);
                    allRoadTilePaletteIndexes = new byte[RoadTileset.TileCount];
                    Buffer.BlockCopy(roadTilePaletteIndexes, 0, allRoadTilePaletteIndexes, 0, RoadTileset.ThemeTileCount);
                    Buffer.BlockCopy(commonRoadTilePaletteIndexes, 0, allRoadTilePaletteIndexes, RoadTileset.ThemeTileCount, RoadTileset.CommonTileCount);
    
                    allRoadTileGfx = new byte[RoadTileset.TileCount][];
                    Array.Copy(roadTileGfx, 0, allRoadTileGfx, 0, roadTileGfx.Length);
                    Array.Copy(commonRoadTileGfx, 0, allRoadTileGfx, RoadTileset.ThemeTileCount, commonRoadTileGfx.Length);
    
                    // Set empty tile default graphics value
                    for (int j = roadTileGfx.Length; j < RoadTileset.ThemeTileCount; j++)
                    {
                        allRoadTileGfx[j] = new byte[32];
                    }
                }

                TileGenre[] allRoadTileGenres;

                if (tileGenresRelocated)
                {
                    int tileGenreOffset = offsets[Offset.TileGenresRelocated] + i * RoadTileset.TileCount;
                    allRoadTileGenres = GetTileGenres(romBuffer, tileGenreOffset, RoadTileset.TileCount);
                }
                else
                {
                    int roadTileGenreIndex = roadTileGenreIndexes[i][0] + (roadTileGenreIndexes[i][1] << 8);
                    TileGenre[] roadTileGenres = Themes.GetTileGenres(roadTileGenreData, roadTileGenreIndex, roadTileGfx.Length);
                    allRoadTileGenres = new TileGenre[RoadTileset.TileCount];
                    Array.Copy(roadTileGenres, 0, allRoadTileGenres, 0, roadTileGenres.Length);
                    Array.Copy(commonRoadTileGenres, 0, allRoadTileGenres, RoadTileset.ThemeTileCount, commonRoadTileGenres.Length);

                    // Set empty tile default genre value
                    for (int j = roadTileGfx.Length; j < RoadTileset.ThemeTileCount; j++)
                    {
                        allRoadTileGenres[j] = TileGenre.Road;
                    }
                }

                RoadTileset roadTileset = Themes.GetRoadTileset(palettes, allRoadTilePaletteIndexes, allRoadTileGfx, allRoadTileGenres);

                byte[] bgTileData = Codec.Decompress(romBuffer, bgTileGfxOffsets[i]);
                byte[][] bgTileGfx = Utilities.ReadBlockGroupUntil(bgTileData, 0, -1, 16);
                BackgroundTileset bgTileset = Themes.GetBackgroundTileset(palettes, bgTileGfx);

                byte[] bgLayoutData = Codec.Decompress(romBuffer, bgLayoutOffsets[i]);
                BackgroundLayout bgLayout = new BackgroundLayout(bgLayoutData);

                Background background = new Background(bgTileset, bgLayout);

                this.themes[i] = new Theme(name, palettes, roadTileset, background);
            }
        }

        private static bool AreTileGenresRelocated(byte[] romBuffer, int offset)
        {
            return !(romBuffer[offset++] == 0xA0 &&
                     romBuffer[offset++] == 0xBA &&
                     romBuffer[offset++] == 0xFD &&
                     romBuffer[offset] == 0xA9);
        }

        private static bool IsRoadTilesetHackApplied(byte[] romBuffer, Offsets offsets)
        {
            if (romBuffer[offsets[Offset.RoadTilesetHack1]] == 0x60 &&
                romBuffer[offsets[Offset.RoadTilesetHack2]] == 0x60 &&
                romBuffer[offsets[Offset.RoadTilesetHack3]] == 0x00 &&
                romBuffer[offsets[Offset.RoadTilesetHack4]] == 0x40)
            {
                return true;
            }

            if (romBuffer[offsets[Offset.RoadTilesetHack1]] == 0xA9 &&
                romBuffer[offsets[Offset.RoadTilesetHack2]] == 0xA0 &&
                romBuffer[offsets[Offset.RoadTilesetHack3]] == 0xC0 &&
                romBuffer[offsets[Offset.RoadTilesetHack4]] == 0x30)
            {
                return false;
            }

            throw new InvalidDataException("Error when loading road tileset data.");
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

        private static RoadTileset GetRoadTileset(Palettes palettes, byte[] tilePaletteIndexes, byte[][] tileGfx, TileGenre[] tileGenres)
        {
            RoadTile[] tiles = new RoadTile[RoadTileset.TileCount];

            for (int i = 0; i < tileGfx.Length; i++)
            {
                Palette palette = palettes[tilePaletteIndexes[i]];
                tiles[i] = new RoadTile(tileGfx[i], palette, tileGenres[i]);
            }

            return new RoadTileset(tiles);
        }

        private static BackgroundTileset GetBackgroundTileset(Palettes palettes, byte[][] tileGfx)
        {
            BackgroundTile[] tiles = new BackgroundTile[tileGfx.Length];

            for (int i = 0; i < tileGfx.Length; i++)
            {
                tiles[i] = new BackgroundTile(tileGfx[i], palettes);
            }

            return new BackgroundTileset(tiles);
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
