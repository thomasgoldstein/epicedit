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
using System.IO;
using System.Windows.Forms;

using EpicEdit.Rom;

namespace EpicEdit.UI.Tools
{
    /// <summary>
    /// The UI to compress and decompress ROM data.
    /// </summary>
    public partial class CodecControl : UserControl
    {
        public CodecControl()
        {
            this.InitializeComponent();
        }

        public void Init()
        {
            int min = Context.Game.HeaderSize;
            this.offsetNumericUpDown.Minimum = min;

            // Data cannot be recompressed from 512 KiB to 1024 KiB, because that range is reserved for Epic Edit.
            // Therefore, set the maximum to 512 Kib if the ROM is no bigger than 1024 Kib.
            int max = Context.Game.Size > RomSize.Size1024 ? Context.Game.Size : RomSize.Size512;
            this.offsetNumericUpDown.Maximum = max;

            this.offsetNumericUpDown.Value = min;
        }
        
        private void BrowseButtonClick(object sender, EventArgs e)
        {
            int offset = (int)this.offsetNumericUpDown.Value;
            bool twice = this.twiceCheckBox.Checked;

            if (this.compressRadioButton.Checked)
            {
                CodecControl.Compress(offset, twice);
            }
            else
            {
                CodecControl.Decompress(offset, twice);
            }
        }

        private static void Compress(int offset, bool twice)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Raw binary file (*.bin)|*.bin|" +
                             "All files (*.*)|*.*";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    byte[] data;

                    try
                    {
                        data = File.ReadAllBytes(ofd.FileName);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        UITools.ShowError(ex.Message);
                        return;
                    }
                    catch (IOException ex)
                    {
                        UITools.ShowError(ex.Message);
                        return;
                    }
                    catch (InvalidDataException ex)
                    {
                        UITools.ShowError(ex.Message);
                        return;
                    }

                    byte[] compData = Context.Game.Compress(data, twice);

                    string info = CodecControl.FormatCompressedChunkInfo("New data", offset, compData.Length, data.Length);

                    string oldInfo;
                    try
                    {
                        int oldCompSize = Context.Game.GetCompressedChunkLength(offset, twice);
                        int oldUncompSize = Context.Game.Decompress(offset, twice).Length;
                        oldInfo = CodecControl.FormatCompressedChunkInfo("Old data", offset, oldCompSize, oldUncompSize);
                    }
                    catch
                    {
                        oldInfo = "Cannot decompress data at " + offset.ToString("X") + "." + Environment.NewLine;
                    }

                    info = oldInfo + Environment.NewLine + info;

                    if (UITools.ShowInfo(info, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        Context.Game.InsertData(compData, offset);
                    }
                }
            }
        }

        private static string FormatCompressedChunkInfo(string label, int offset, int compSize, int uncompSize)
        {
            return string.Format("{0}: from {1} to {2}" + Environment.NewLine +
                                 "Compressed size: {3}" + Environment.NewLine +
                                 "Uncompressed size: {4}" + Environment.NewLine,
                                 label,
                                 offset.ToString("X"),
                                 (offset + compSize).ToString("X"),
                                 compSize.ToString("X"),
                                 uncompSize.ToString("X"));
        }

        private static void Decompress(int offset, bool twice)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Raw binary file (*.bin)|*.bin|" +
                             "All files (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    byte[] data = Context.Game.Decompress(offset, twice);

                    try
                    {
                        File.WriteAllBytes(sfd.FileName, data);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        UITools.ShowError(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        UITools.ShowError(ex.Message);
                    }
                    catch (InvalidDataException ex)
                    {
                        UITools.ShowError(ex.Message);
                    }
                }
            }
        }
    }
}
