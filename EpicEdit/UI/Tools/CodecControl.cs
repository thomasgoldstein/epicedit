﻿#region GPL statement
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
using EpicEdit.Rom.Compression;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// The UI to compress and decompress ROM data.
    /// </summary>
    internal partial class CodecControl : UserControl
    {
        public CodecControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            var min = Context.Game.HeaderSize;
            offsetTextBox.Minimum = min;

            // Data cannot be recompressed from 512 KiB to 1024 KiB, because that range is reserved for Epic Edit.
            // Therefore, set the maximum to 512 Kib if the ROM is no bigger than 1024 Kib.
            var max = Context.Game.Size > RomSize.Size1024 ? Context.Game.Size : RomSize.Size512;
            offsetTextBox.Maximum = max;

            offsetTextBox.Value = min;
        }

        private void BrowseButtonClick(object sender, EventArgs e)
        {
            var offset = (int)offsetTextBox.Value;
            var twice = twiceCheckBox.Checked;

            if (compressRadioButton.Checked)
            {
                byte[] data = null;
                if (UITools.ShowImportDataDialog(fileName => data = File.ReadAllBytes(fileName), FileDialogFilters.Binary))
                {
                    var limit = (int)offsetTextBox.Maximum;
                    Compress(data, offset, twice, limit);
                }
            }
            else
            {
                UITools.ShowExportDataDialog(fileName => File.WriteAllBytes(fileName, Context.Game.Decompress(offset, twice)), string.Empty, FileDialogFilters.Binary);
            }
        }

        private static void Compress(byte[] data, int offset, bool twice, int limit)
        {
            var compData = Codec.Compress(data, twice, true);

            var info = FormatCompressedChunkInfo("New data", offset, compData.Length, data.Length);

            string oldInfo;
            try
            {
                var oldCompSize = Context.Game.GetCompressedChunkLength(offset);
                var oldUncompSize = Context.Game.Decompress(offset, twice).Length;
                oldInfo = FormatCompressedChunkInfo("Old data", offset, oldCompSize, oldUncompSize);
            }
            catch (InvalidDataException)
            {
                oldInfo = $"Cannot decompress data at {offset:X}." + Environment.NewLine;
            }

            info = oldInfo + Environment.NewLine + info;

            if (offset + compData.Length > limit)
            {
                info += Environment.NewLine + "Not enough room to fit the new data. Operation cancelled.";
                UITools.ShowError(info);
            }
            else if (UITools.ShowInfo(info, MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Context.Game.InsertData(compData, offset);
            }
        }

        private static string FormatCompressedChunkInfo(string label, int offset, int compSize, int uncompSize)
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "{0}: from {1:X} to {2:X}" + Environment.NewLine +
                                 "Compressed size: {3} bytes" + Environment.NewLine +
                                 "Uncompressed size: {4} bytes" + Environment.NewLine,
                                 label, offset, (offset + compSize - 1),
                                 compSize,
                                 uncompSize);
        }
    }
}
