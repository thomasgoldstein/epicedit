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
using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// A collection of texts.
    /// </summary>
    internal class TextCollection : IEnumerable<string>
    {
        private TextConverter textConverter;
        private string[] texts;
        private byte[] colorIndexes;
        private int totalSize;

        public int TotalCharacterCount
        {
            get
            {
                int step = colorIndexes == null ? 1 : 2;
                return (totalSize / step) - this.texts.Length * step;
            }
        }

        public bool Modified { get; private set; }

        public TextCollection(byte[] romBuffer, int indexOffset, int count, int totalSize, bool skipOddBytes,
                              bool fixedLength, byte shiftValue, byte[] keys, char[] values)
        {
            this.textConverter = new TextConverter(Game.GetRegion(romBuffer), shiftValue);
            byte[][] textIndexes = Utilities.ReadBlockGroup(romBuffer, indexOffset, 2, count);

            this.texts = new string[count];

            if (skipOddBytes)
            {
                this.colorIndexes = new byte[count];
            }

            if (keys != null)
            {
                this.textConverter.ReplaceKeyValues(keys, values);
            }

            byte leadingOffsetByte = (byte)((indexOffset & 0xF0000) >> 16);

            for (int i = 0; i < this.texts.Length; i++)
            {
                // Recreates offsets from the index table loaded above
                int offset = Utilities.BytesToOffset(textIndexes[i][0], textIndexes[i][1], leadingOffsetByte);
                byte[] textBytes;

                if (!fixedLength)
                {
                    // Dynamic text length, ends at byte 0xFF
                    textBytes = Utilities.ReadBlockUntil(romBuffer, offset, 0xFF);
                }
                else
                {
                    // Fixed text length, length determined in a separate table
                    int lengthOffset = indexOffset + (count + i) * 2; // 2 bytes per offset
                    int length = romBuffer[lengthOffset];

                    if (skipOddBytes)
                    {
                        length *= 2;
                    }

                    textBytes = Utilities.ReadBlock(romBuffer, offset, length);
                }

                this.texts[i] = this.textConverter.DecodeText(textBytes, skipOddBytes);

                if (skipOddBytes && textBytes.Length >= 2)
                {
                    this.colorIndexes[i] = textBytes[1];
                }
            }

            this.totalSize = totalSize;
        }

        public string this[int index]
        {
            get { return this.texts[index]; }
            set
            {
                this.texts[index] = value;
                this.Modified = true;
            }
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[this.totalSize];
            bool hasPaletteData = this.colorIndexes != null;
            int index = 0;

            for (int i = 0; i < this.texts.Length; i++)
            {
                byte? paletteIndex = !hasPaletteData ? null as byte? : this.colorIndexes[i];
                byte[] textBytes = this.textConverter.EncodeText(this.texts[i], paletteIndex);
                Buffer.BlockCopy(textBytes, 0, data, index, textBytes.Length);
                index += textBytes.Length;
                data[index++] = 0xFF;

                if (hasPaletteData)
                {
                    data[index++] = 0xFF;
                }
            }

            return data;
        }

        public void ResetModifiedState()
        {
            this.Modified = false;
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (string text in this.texts)
            {
                yield return text;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.texts.GetEnumerator();
        }

        public int Count
        {
            get { return this.texts.Length; }
        }
    }
}
