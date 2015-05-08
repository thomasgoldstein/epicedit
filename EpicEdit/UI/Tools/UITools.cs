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
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Utility;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// Represents a UI helper class.
    /// </summary>
    internal static class UITools
    {
        public delegate T Func<T>(); // No Func in .NET 2.0, so define it

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
            return UITools.ShowWarning(message, MessageBoxButtons.YesNo);
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

            Type type = item.GetType();
            MemberInfo[] memInfo = type.GetMember(item.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
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
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string pattern = "[" + Regex.Escape(invalidChars) + "]*";
            fileName = Regex.Replace(fileName, pattern, string.Empty);
            return fileName;
        }

        /// <summary>
        /// Applies a workaround for a Microsoft bug which makes it so a tooltip
        /// no longer pops up after it has timed out once. Affects Windows XP but not Vista.
        /// </summary>
        public static void FixToolTip(Control control, ToolTip toolTip)
        {
            // HACK: See method summary. For more details, see:
            // http://stackoverflow.com/questions/559707/winforms-tooltip-will-not-re-appear-after-first-use
            control.MouseEnter += (s, ea) => { toolTip.Active = false; toolTip.Active = true; };
        }

        public static bool ShowImportTilesetGraphicsDialog(Tile[] tileset)
        {
            string filter =
                "PNG (*.png)|*.png|" +
                "BMP (*.bmp)|*.bmp|" +
                "Raw binary file (*.bin)|*.bin";

            return UITools.ShowImportDataDialog(filePath => UITools.ImportTilesetGraphics(filePath, tileset), filter);
        }

        private static void ImportTilesetGraphics(string filePath, Tile[] tileset)
        {
            if (filePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                filePath.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
            {
                // Import image
                using (Bitmap image = new Bitmap(filePath))
                {
                    UITools.ImportTilesetGraphics(image, tileset);
                }
            }
            else
            {
                // Import raw binary graphics
                byte[] data = File.ReadAllBytes(filePath);
                UITools.ImportTilesetGraphics(data, tileset);
            }
        }

        private static void ImportTilesetGraphics(Bitmap image, Tile[] tileset)
        {
            int width = image.Width;
            int height = image.Height;

            if (width % Tile.Size != 0 ||
                height % Tile.Size != 0 ||
                (width * height) != (tileset.Length * Tile.Size * Tile.Size))
            {
                throw new ArgumentException("Invalid tileset image size.", "image");
            }

            int yTileCount = height / Tile.Size;
            int xTileCount = width / Tile.Size;

            for (int y = 0; y < yTileCount; y++)
            {
                for (int x = 0; x < xTileCount; x++)
                {
                    Bitmap tileImage = image.Clone(
                        new Rectangle(x * Tile.Size,
                                      y * Tile.Size,
                                      Tile.Size,
                                      Tile.Size),
                        PixelFormat.Format32bppPArgb);

                    Tile tile = tileset[y * xTileCount + x];
                    tile.Bitmap = tileImage;
                }
            }
        }

        private static void ImportTilesetGraphics(byte[] data, Tile[] tileset)
        {
            int tileBpp = tileset[0] is Tile2bpp ? 2 : 4;
            int tileLength = (Tile.Size * Tile.Size) / (8 / tileBpp);

            if (data.Length != tileset.Length * tileLength)
            {
                throw new ArgumentException("Invalid tileset data size.", "data");
            }

            byte[][] tileData = Utilities.ReadBlockGroup(data, 0, tileLength, tileset.Length);

            for (int i = 0; i < tileset.Length; i++)
            {
                Tile tile = tileset[i];
                tile.Graphics = tileData[i];
            }
        }

        public static bool ShowImportBinaryDataDialog(Action<byte[]> setDataMethod)
        {
            string filter =
                "Raw binary file (*.bin)|*.bin|" +
                "All files (*.*)|*.*";

            return UITools.ShowImportBinaryDataDialog(setDataMethod, filter);
        }

        public static bool ShowImportBinaryDataDialog(Action<byte[]> setDataMethod, string filter)
        {
            return UITools.ShowImportDataDialog(filePath => setDataMethod(File.ReadAllBytes(filePath)), filter);
        }

        public static bool ShowImportDataDialog(Action<string> setDataMethod, string filter)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = filter;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    UITools.ImportData(setDataMethod, ofd.FileName);
                    return true;
                }

                return false;
            }
        }

        public static void ImportData(Action<string> setDataMethod, string filePath)
        {
            try
            {
                setDataMethod(filePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (ArgumentException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (InvalidDataException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }

        public static void ShowExportTilesetGraphicsDialog(Image image, Tile[] tileset, string fileName)
        {
            string filter =
                "PNG (*.png)|*.png|" +
                "BMP (*.bmp)|*.bmp|" +
                "Raw binary file (*.bin)|*.bin";

            UITools.ShowExportDataDialog(acceptedFileName => UITools.ExportTilesetGraphics(image, tileset, acceptedFileName), fileName, filter);
        }

        private static void ExportTilesetGraphics(Image image, Tile[] tileset, string fileName)
        {
            switch (Path.GetExtension(fileName).ToUpperInvariant())
            {
                case ".PNG":
                    image.Save(fileName, ImageFormat.Png);
                    break;

                case ".BMP":
                    image.Save(fileName, ImageFormat.Bmp);
                    break;

                default:
                    File.WriteAllBytes(fileName, UITools.GetTilesetBytes(tileset));
                    break;
            }
        }

        private static byte[] GetTilesetBytes(Tile[] tileset)
        {
            int tileBpp = tileset[0] is Tile2bpp ? 2 : 4;
            int tileLength = (Tile.Size * Tile.Size) / (8 / tileBpp);

            byte[] data = new byte[tileset.Length * tileLength];

            for (int i = 0; i < tileset.Length; i++)
            {
                Tile tile = tileset[i];
                Buffer.BlockCopy(tile.Graphics, 0, data, i * tileLength, tile.Graphics.Length);
            }

            return data;
        }

        public static void ShowExportBinaryDataDialog(Func<byte[]> getDataMethod, string fileName)
        {
            string filter =
                "Raw binary file (*.bin)|*.bin|" +
                "All files (*.*)|*.*";

            UITools.ShowExportBinaryDataDialog(getDataMethod, fileName, filter);
        }

        public static void ShowExportBinaryDataDialog(Func<byte[]> getDataMethod, string fileName, string filter)
        {
            UITools.ShowExportDataDialog(acceptedFileName => File.WriteAllBytes(acceptedFileName, getDataMethod()), fileName, filter);
        }

        public static void ShowExportDataDialog(Action<string> exportMethod, string fileName, string filter)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = filter;

                sfd.FileName = UITools.SanitizeFileName(fileName);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        UITools.ExportData(exportMethod, sfd.FileName);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        UITools.ShowError(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        UITools.ShowError(ex.Message);
                    }
                }
            }
        }

        public static void ExportData(Action<string> exportMethod, string fileName)
        {
            try
            {
                exportMethod(fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                UITools.ShowError(ex.Message);
            }
            catch (IOException ex)
            {
                UITools.ShowError(ex.Message);
            }
        }
    }
}
