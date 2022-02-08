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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Utility;
using EpicEdit.UI.Gfx;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// Represents a UI helper class.
    /// </summary>
    internal static class UITools
    {
        public delegate T Func<T>(); // No Func in .NET 2.0, so define it
        public delegate void BulkImportDataAction(int fileIndex, string fileName);

        public static DialogResult ShowWarning(string message, MessageBoxButtons buttons)
        {
            return
                MessageBox.Show(
                    message,
                    Application.ProductName,
                    buttons,
                    MessageBoxIcon.Warning);
        }

        public static DialogResult ShowWarning(string message)
        {
            return ShowWarning(message, MessageBoxButtons.YesNo);
        }

        public static DialogResult ShowInfo(string message, MessageBoxButtons buttons)
        {
            return
                MessageBox.Show(
                    message,
                    Application.ProductName,
                    buttons,
                    MessageBoxIcon.Information);
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(
                message,
                Application.ProductName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        /// <summary>
        /// Gets the passed object description attribute value.
        /// </summary>
        /// <param name="item">Object instance.</param>
        public static string GetDescription(object item)
        {
            string desc = null;

            var type = item.GetType();
            var memInfo = type.GetMember(item.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    desc = (attrs[0] as DescriptionAttribute).Description;
                }
            }

            if (desc == null) // Description not found
            {
                desc = item.ToString();
            }

            return desc;
        }

        /// <summary>
        /// Removes invalid file name characters.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>Sanitized file name.</returns>
        public static string SanitizeFileName(string fileName)
        {
            var invalidChars = new string(Path.GetInvalidFileNameChars());
            var pattern = "[" + Regex.Escape(invalidChars) + "]*";
            fileName = Regex.Replace(fileName, pattern, string.Empty);
            return fileName;
        }

        public static bool ShowImportTilesetGraphicsDialog(ITileset tileset)
        {
            return ShowImportDataDialog(filePath => ImportTilesetGraphics(filePath, tileset), FileDialogFilters.ImageOrBinary);
        }

        private static void ImportTilesetGraphics(string filePath, ITileset tileset)
        {
            if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                // Import image
                using (var image = new Bitmap(filePath))
                {
                    ImportTilesetGraphics(image, tileset);
                }
            }
            else
            {
                // Import raw binary graphics
                var data = File.ReadAllBytes(filePath);
                ImportTilesetGraphics(data, tileset);
            }
        }

        private static void ImportTilesetGraphics(Bitmap image, ITileset tileset)
        {
            var width = image.Width;
            var height = image.Height;

            if (width % Tile.Size != 0 ||
                height % Tile.Size != 0 ||
                (width * height) != (tileset.Length * Tile.Size * Tile.Size))
            {
                throw new ArgumentException("Invalid tileset image size.", nameof(image));
            }

            var yTileCount = height / Tile.Size;
            var xTileCount = width / Tile.Size;
            var pixelFormat = image.PixelFormat;

            for (var y = 0; y < yTileCount; y++)
            {
                for (var x = 0; x < xTileCount; x++)
                {
                    var tileImage = image.Clone(
                        new Rectangle(x * Tile.Size,
                                      y * Tile.Size,
                                      Tile.Size,
                                      Tile.Size),
                        pixelFormat);

                    var tile = tileset[y * xTileCount + x];
                    tile.Bitmap = tileImage;
                }
            }

            if (tileset.BitsPerPixel == 4 && pixelFormat == PixelFormat.Format8bppIndexed)
            {
                UpdatePalettesFromIndexedImage(tileset, image.Palette);
            }
        }

        private static void UpdatePalettesFromIndexedImage(ITileset tileset, ColorPalette bmpPalette)
        {
            var palettes = tileset[0].Palette.Collection;
            var paletteBytes = new byte[Palette.Size * palettes.Count];
            var paletteBytesIndex = 0;

            foreach (var color in bmpPalette.Entries)
            {
                var colorBytes = ((RomColor)color).GetBytes();
                paletteBytes[paletteBytesIndex++] = colorBytes[0];
                paletteBytes[paletteBytesIndex++] = colorBytes[1];
            }

            palettes.SetBytes(paletteBytes);
        }

        private static void ImportTilesetGraphics(byte[] data, ITileset tileset)
        {
            var tileLength = Tile.Size * tileset.BitsPerPixel;

            if (data.Length != tileset.Length * tileLength)
            {
                throw new ArgumentException("Invalid tileset data size.", nameof(data));
            }

            var tileData = Utilities.ReadBlockGroup(data, 0, tileLength, tileset.Length);

            for (var i = 0; i < tileset.Length; i++)
            {
                var tile = tileset[i];
                tile.Graphics = tileData[i];
            }
        }

        public static bool ShowImportBinaryDataDialog(Action<byte[]> setDataMethod)
        {
            return ShowImportBinaryDataDialog(setDataMethod, FileDialogFilters.Binary);
        }

        public static bool ShowImportBinaryDataDialog(Action<byte[]> setDataMethod, string filter)
        {
            return ShowImportDataDialog(filePath => setDataMethod(File.ReadAllBytes(filePath)), filter);
        }

        public static bool ShowImportDataDialog(Action<string> setDataMethod, string filter)
        {
            return ShowImportDataDialog((index, filePath) => setDataMethod(filePath), filter, 1);
        }

        public static bool ShowImportDataDialog(BulkImportDataAction setDataMethod, string filter, int maxFileCount)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;
                ofd.Multiselect = maxFileCount > 1;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return false;
                }

                var filePaths = ofd.FileNames;
                if (filePaths.Length > maxFileCount)
                {
                    Array.Resize(ref filePaths, maxFileCount);
                }

                ImportData(setDataMethod, filePaths);

                return true;
            }
        }

        public static void ImportData(Action<string> setDataMethod, params string[] filePaths)
        {
            ImportData((index, filePath) => setDataMethod(filePath), filePaths);
        }

        public static void ImportData(BulkImportDataAction setDataMethod, params string[] filePaths)
        {
            try
            {
                for (var i = 0; i < filePaths.Length; i++)
                {
                    setDataMethod(i, filePaths[i]);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                ShowError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                ShowError(ex.Message);
            }
        }

        public static void ShowExportTilesetGraphicsDialog(ITileset tileset, Size imageSize, string fileName)
        {
            ShowExportDataDialog(filePath => ExportTilesetGraphics(tileset, imageSize, filePath), fileName, FileDialogFilters.ImageOrBinary);
        }

        private static void ExportTilesetGraphics(ITileset tileset, Size imageSize, string fileName)
        {
            switch (Path.GetExtension(fileName).ToUpperInvariant())
            {
                case ".PNG":
                    SaveImage(tileset, imageSize, fileName, ImageFormat.Png);
                    break;

                case ".BMP":
                    SaveImage(tileset, imageSize, fileName, ImageFormat.Bmp);
                    break;

                default:
                    File.WriteAllBytes(fileName, GetTilesetBytes(tileset));
                    break;
            }
        }

        private static void SaveImage(ITileset tileset, Size imageSize, string fileName, ImageFormat imageFormat)
        {
            using (var indexedImage = IndexedImageFactory.Create(tileset, imageSize.Width, imageSize.Height))
            {
                indexedImage.Save(fileName, imageFormat);
            }
        }

        private static byte[] GetTilesetBytes(ITileset tileset)
        {
            var tileLength = Tile.Size * tileset.BitsPerPixel;

            var data = new byte[tileset.Length * tileLength];

            for (var i = 0; i < tileset.Length; i++)
            {
                var tile = tileset[i];
                Buffer.BlockCopy(tile.Graphics, 0, data, i * tileLength, tile.Graphics.Length);
            }

            return data;
        }

        public static void ShowExportBinaryDataDialog(Func<byte[]> getDataMethod, string fileName)
        {
            ShowExportBinaryDataDialog(getDataMethod, fileName, FileDialogFilters.Binary);
        }

        public static void ShowExportBinaryDataDialog(Func<byte[]> getDataMethod, string fileName, string filter)
        {
            ShowExportDataDialog(filePath => File.WriteAllBytes(filePath, getDataMethod()), fileName, filter);
        }

        public static void ShowExportDataDialog(Action<string> exportMethod, string fileName, string filter)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = filter;

                sfd.FileName = SanitizeFileName(fileName);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportData(exportMethod, sfd.FileName);
                }
            }
        }

        public static void ExportData(Action<string> exportMethod, string filePath)
        {
            try
            {
                exportMethod(filePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                ShowError(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                ShowError(ex.Message);
            }
        }
    }
}
