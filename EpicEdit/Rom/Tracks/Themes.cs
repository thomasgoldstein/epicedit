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

using EpicEdit.Rom.Compression;
using EpicEdit.UI.Gfx;

namespace EpicEdit.Rom.Tracks
{
    public sealed class Themes : IDisposable, IEnumerable<Theme>
    {
        private Theme[] themes;

        public Theme this[int index]
        {
            get { return themes[index]; }
        }

        public int Count
        {
            get { return themes.Length; }
        }

        public Themes(byte[] romBuffer, Offsets offsets, string[] names)
        {
            LoadThemes(romBuffer, offsets, names);
        }

        private void LoadThemes(byte[] romBuffer, Offsets offsets, string[] names)
        {
            this.themes = new Theme[Game.ThemeCount];

            int[] reorder = { 5, 4, 6, 9, 8, 10, 7, 12 }; // To reorder the themes, as they're not in the same order as the names
            // TODO: Retrieve order dynamically from the ROM

            int[] themeColorPaletteAddresses = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeColorPalettes], themes.Length);
            int[] roadTilesetGfxAddresses = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.ThemeRoadGraphics], themes.Length);
            //int[] backgroundTilesetGfxAddresses = Utilities.ReadBlockOffset(romBuffer, offsets[Offset.TrackBackgroundGraphics], themes.Length);

            byte[] roadCommonTilesetData = Codec.Decompress(romBuffer, offsets[Offset.CommonTilesetGraphics]);
            byte[] roadCommonTilesetPaletteIndexes = Themes.GetPaletteIndexes(roadCommonTilesetData);
            byte[][] roadCommonTilesetGfx = Utilities.ReadBlockGroupUntil(roadCommonTilesetData, 0x100, -1, 32);

            for (int i = 0; i < themes.Length; i++)
            {
                byte[] colorPaletteData = Codec.Decompress(romBuffer, themeColorPaletteAddresses[i], 512);
                // Force the length to 512 in case the color palette data in the ROM is corrupt

                Palette[] colorPalettes = new Palette[16];

                for (int j = 0; j < colorPalettes.Length; j++)
                {
                    byte[] paletteBytes = new byte[32];
                    Array.Copy(colorPaletteData, j * 32, paletteBytes, 0, 32);
                    colorPalettes[j] = new Palette(paletteBytes);
                }

                byte[] roadTilesetData = Codec.Decompress(romBuffer, roadTilesetGfxAddresses[i]);
                byte[] roadTilesetPaletteIndexes = Themes.GetPaletteIndexes(roadTilesetData);
                byte[][] roadTilesetGfx = Utilities.ReadBlockGroupUntil(roadTilesetData, 0x100, -1, 32);

                Bitmap[] roadBitmaps = Themes.GetRoadTilesetBitmaps(colorPalettes,
                                                                    roadTilesetPaletteIndexes, roadTilesetGfx,
                                                                    roadCommonTilesetPaletteIndexes, roadCommonTilesetGfx);

                Tile[] roadTileset = new Tile[roadBitmaps.Length];

                for (int tileId = 0; tileId < roadBitmaps.Length; tileId++)
                {
                    roadTileset[tileId] = new StillTile(roadBitmaps[tileId], TileGenre.Road);
                }

                // TODO: Add support for background tilesets
                //byte[] backgroundTilesetData = Codec.Decompress(this.romBuffer, backgroundTilesetGfxAddresses[i]);
                Tile[] backgroundTileset = new Tile[0];
                Music music = new Music();

                themes[i] = new Theme(names[reorder[i]], colorPalettes, roadTileset, backgroundTileset, music);
            }
        }

        private static byte[] GetPaletteIndexes(byte[] tilesetData)
        {
            byte[] paletteIndexes = new byte[0x100];
            Array.Copy(tilesetData, 0, paletteIndexes, 0, paletteIndexes.Length);
            for (int i = 0; i < paletteIndexes.Length; i++)
            {
                paletteIndexes[i] = (byte)(paletteIndexes[i] >> 4);
            }

            return paletteIndexes;
        }

        private static Bitmap[] GetRoadTilesetBitmaps(Palette[] colorPalettes, byte[] tilesetPaletteIndexes, byte[][] tilesetGfx, byte[] commonTilesetPaletteIndexes, byte[][] commonTilesetGfx)
        {
            Bitmap[] tileBitmaps = new Bitmap[256];

            // Get the tiles that are specific to this tileset
            GraphicsConverter.GetBitmapFrom4bppLinearReversed(tileBitmaps, colorPalettes, tilesetPaletteIndexes, tilesetGfx, 0, 192);

            // Get the tiles that are common to all tilesets
            GraphicsConverter.GetBitmapFrom4bppLinearReversed(tileBitmaps, colorPalettes, commonTilesetPaletteIndexes, commonTilesetGfx, 192, 64);

            return tileBitmaps;
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
