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
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Tracks.Scenery;
using EpicEdit.Rom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents a collection of <see cref="Theme">themes</see>.
    /// </summary>
    internal sealed class Themes : IEnumerable<Theme>, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Theme[] _themes;

        public Theme this[int index] => _themes[index];

        public int Count => _themes.Length;

        public bool Modified
        {
            get
            {
                foreach (var theme in _themes)
                {
                    if (theme.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public Themes(byte[] romBuffer, Offsets offsets, TextCollection names)
        {
            _themes = new Theme[Theme.Count];
            Init(romBuffer, offsets, names);
            HandleChanges();
        }

        private void Init(byte[] romBuffer, Offsets offsets, TextCollection names)
        {
            // TODO: Retrieve order dynamically from the ROM
            int[] reorder = { 5, 4, 6, 9, 8, 10, 7, 12 }; // To reorder the themes, as they're not in the same order as the names

            var paletteOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemePalettes], _themes.Length);
            var roadTileGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeRoadGraphics], _themes.Length);
            var bgTileGfxOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeBackgroundGraphics], _themes.Length);
            var bgLayoutOffsets = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeBackgroundLayouts], _themes.Length);

            var roadTilesetHackApplied = IsRoadTilesetHackApplied(romBuffer, offsets);
            byte[] commonRoadTilePaletteIndexes;
            byte[][] commonRoadTileGfx;

            if (roadTilesetHackApplied)
            {
                commonRoadTilePaletteIndexes = null;
                commonRoadTileGfx = null;
            }
            else
            {
                var commonRoadTileUpperByte = offsets[Offset.CommonTilesetGraphicsUpperByte];
                var commonRoadTileLowerBytes = offsets[Offset.CommonTilesetGraphicsLowerBytes];
                var commonRoadTileOffset = Utilities.BytesToOffset(
                    romBuffer[commonRoadTileLowerBytes],
                    romBuffer[commonRoadTileLowerBytes + 1],
                    romBuffer[commonRoadTileUpperByte]
                   );
                var commonRoadTileData = Codec.Decompress(romBuffer, commonRoadTileOffset);
                commonRoadTilePaletteIndexes = GetPaletteIndexes(commonRoadTileData, RoadTileset.CommonTileCount);
                commonRoadTileGfx = Utilities.ReadBlockGroupUntil(commonRoadTileData, RoadTileset.TileCount, -1, 32);
            }

            var tileGenresRelocated = AreTileGenresRelocated(romBuffer, offsets[Offset.TileGenreLoad]);
            byte[] roadTileGenreData;
            byte[][] roadTileGenreIndexes;
            RoadTileGenre[] commonRoadTileGenres;

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
                commonRoadTileGenres = GetTileGenres(roadTileGenreData, 0, RoadTileset.CommonTileCount);
            }

            for (var i = 0; i < _themes.Length; i++)
            {
                var nameItem = names[reorder[i]];

                // HACK: Force the length to 512 in case the color palette data in the ROM is corrupt ("EarthBound Kart Beta" has this issue)
                var paletteData = Codec.Decompress(romBuffer, paletteOffsets[i], 512);
                var palettes = new Palettes(paletteData);

                var roadTileData = Codec.Decompress(romBuffer, roadTileGfxOffsets[i]);
                var roadTileGfx = Utilities.ReadBlockGroupUntil(roadTileData, RoadTileset.TileCount, -1, 32);
                byte[] allRoadTilePaletteIndexes;
                byte[][] allRoadTileGfx;

                if (roadTilesetHackApplied)
                {
                    allRoadTilePaletteIndexes = GetPaletteIndexes(roadTileData, RoadTileset.TileCount);
                    allRoadTileGfx = roadTileGfx;
                }
                else
                {
                    var roadTilePaletteIndexes = GetPaletteIndexes(roadTileData, RoadTileset.ThemeTileCount);
                    allRoadTilePaletteIndexes = new byte[RoadTileset.TileCount];
                    Buffer.BlockCopy(roadTilePaletteIndexes, 0, allRoadTilePaletteIndexes, 0, RoadTileset.ThemeTileCount);
                    Buffer.BlockCopy(commonRoadTilePaletteIndexes, 0, allRoadTilePaletteIndexes, RoadTileset.ThemeTileCount, RoadTileset.CommonTileCount);

                    allRoadTileGfx = new byte[RoadTileset.TileCount][];
                    Array.Copy(roadTileGfx, 0, allRoadTileGfx, 0, roadTileGfx.Length);

                    // Clone the commonRoadTileGfx jagged array,
                    // because we don't want theme-specific tile graphics update to affect other themes
                    var commonRoadTileGfxClone = new byte[commonRoadTileGfx.Length][];
                    for (var j = 0; j < commonRoadTileGfxClone.Length; j++)
                    {
                        commonRoadTileGfxClone[j] = Utilities.ReadBlock(commonRoadTileGfx[j], 0, commonRoadTileGfx[j].Length);
                    }

                    Array.Copy(commonRoadTileGfxClone, 0, allRoadTileGfx, RoadTileset.ThemeTileCount, commonRoadTileGfxClone.Length);

                    // Set empty tile default graphics value
                    for (var j = roadTileGfx.Length; j < RoadTileset.ThemeTileCount; j++)
                    {
                        allRoadTileGfx[j] = new byte[32];
                    }
                }

                RoadTileGenre[] allRoadTileGenres;

                if (tileGenresRelocated)
                {
                    var tileGenreOffset = offsets[Offset.TileGenresRelocated] + i * RoadTileset.TileCount;
                    allRoadTileGenres = GetTileGenres(romBuffer, tileGenreOffset, RoadTileset.TileCount);
                }
                else
                {
                    var roadTileGenreIndex = roadTileGenreIndexes[i][0] + (roadTileGenreIndexes[i][1] << 8);
                    var roadTileGenres = GetTileGenres(roadTileGenreData, roadTileGenreIndex, roadTileGfx.Length);
                    allRoadTileGenres = new RoadTileGenre[RoadTileset.TileCount];
                    Array.Copy(roadTileGenres, 0, allRoadTileGenres, 0, roadTileGenres.Length);
                    Array.Copy(commonRoadTileGenres, 0, allRoadTileGenres, RoadTileset.ThemeTileCount, commonRoadTileGenres.Length);

                    // Set empty tile default genre value
                    for (var j = roadTileGfx.Length; j < RoadTileset.ThemeTileCount; j++)
                    {
                        allRoadTileGenres[j] = RoadTileGenre.Road;
                    }
                }

                var roadTileset = GetRoadTileset(palettes, allRoadTilePaletteIndexes, allRoadTileGfx, allRoadTileGenres);

                var bgTileData = Codec.Decompress(romBuffer, bgTileGfxOffsets[i]);
                var bgTileGfx = Utilities.ReadBlockGroupUntil(bgTileData, 0, -1, 16);
                var bgTileset = GetBackgroundTileset(palettes, bgTileGfx);

                var bgLayoutData = Codec.Decompress(romBuffer, bgLayoutOffsets[i]);
                var bgLayout = new BackgroundLayout(bgLayoutData);

                var background = new Background(bgTileset, bgLayout);

                _themes[i] = new Theme(nameItem, palettes, roadTileset, background);
            }
        }

        private void HandleChanges()
        {
            foreach (var theme in _themes)
            {
                theme.PropertyChanged += OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
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

        private static RoadTileGenre[] GetTileGenres(byte[] data, int start, int size)
        {
            var tileGenres = new RoadTileGenre[size];

            for (var i = 0; i < size; i++)
            {
                tileGenres[i] = (RoadTileGenre)data[start + i];
            }

            return tileGenres;
        }

        private static byte[] GetPaletteIndexes(byte[] tileData, int count)
        {
            var paletteIndexes = Utilities.ReadBlock(tileData, 0, count);

            for (var i = 0; i < count; i++)
            {
                paletteIndexes[i] = (byte)(paletteIndexes[i] >> 4);
            }

            return paletteIndexes;
        }

        private static RoadTileset GetRoadTileset(Palettes palettes, byte[] tilePaletteIndexes, byte[][] tileGfx, RoadTileGenre[] tileGenres)
        {
            var tiles = new RoadTile[RoadTileset.TileCount];
            var firstPalette = palettes[0];

            for (var i = 0; i < tileGfx.Length; i++)
            {
                var palette = palettes[tilePaletteIndexes[i]];
                tiles[i] = new RoadTile(tileGfx[i], palette, tileGenres[i], firstPalette);
            }

            return new RoadTileset(tiles);
        }

        private static BackgroundTileset GetBackgroundTileset(Palettes palettes, byte[][] tileGfx)
        {
            var tiles = new BackgroundTile[BackgroundTileset.TileCount];

            for (var i = 0; i < Math.Min(tileGfx.Length, tiles.Length); i++)
            {
                tiles[i] = new BackgroundTile(tileGfx[i], palettes);
            }

            // If the tileset isn't full, fill in the rest of the tileset with empty tiles
            for (var i = tileGfx.Length; i < BackgroundTileset.TileCount; i++)
            {
                tiles[i] = new BackgroundTile(new byte[16], palettes);
            }

            return new BackgroundTileset(tiles);
        }

        public byte GetThemeId(Theme theme)
        {
            for (byte i = 0; i < _themes.Length; i++)
            {
                if (_themes[i] == theme)
                {
                    return (byte)(i << 1);
                }
            }

            throw new ArgumentException("Theme not found.", nameof(theme));
        }

        public void ResetModifiedState()
        {
            foreach (var theme in _themes)
            {
                theme.ResetModifiedState();
            }
        }

        public IEnumerator<Theme> GetEnumerator()
        {
            foreach (var theme in _themes)
            {
                yield return theme;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _themes.GetEnumerator();
        }

        public void Dispose()
        {
            foreach (var theme in _themes)
            {
                theme.Dispose();
            }

            GC.SuppressFinalize(this);
        }
    }
}
