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
using System.Globalization;
using System.Text;

using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// A collection of texts.
    /// </summary>
    internal class TextCollection : IEnumerable<string>
    {
        /// <summary>
        /// The converter used to translate font graphics index values into .NET strings.
        /// </summary>
        private TextConverter textConverter;

        /// <summary>
        /// The texts of the collection.
        /// </summary>
        private string[] texts;

        /// <summary>
        /// The offset to the indexes that define the address of each text.
        /// </summary>
        private int indexOffset;

        /// <summary>
        /// The offset to the start of the text data.
        /// </summary>
        private int textOffset;

        /// <summary>
        /// The color palette index used by each text (using the 1st character as reference).
        /// </summary>
        private byte[] colorIndexes;

        /// <summary>
        /// The total size in bytes of the collection.
        /// </summary>
        private int totalSize;

        /// <summary>
        /// A mode for certain Japanese text collections with ten-ten and maru characters encoded separately.
        /// </summary>
        private bool japAltMode;

        /// <summary>
        /// Gets the maximum amount of characters available for the whole collection.
        /// </summary>
        public int MaxCharacterCount
        {
            get
            {
                int step = colorIndexes == null ? 1 : 2;
                return (totalSize / step) - this.texts.Length * step;
            }
        }

        public bool Modified { get; private set; }

        /// <summary>
        /// Initializes a text collection.
        /// </summary>
        /// <param name="romBuffer">The ROM buffer.</param>
        /// <param name="indexOffset">The location of the index table to each text offset.</param>
        /// <param name="count">The number of texts in the collection.</param>
        /// <param name="totalSize">The total size in bytes of the collection.</param>
        /// <param name="hasPaletteData">True if the text data is mixed with color palette indexes.</param>
        /// <param name="fixedLength">True if the length of each text is specified separately in a length table.
        /// Otherwise each text item ends with 0xFF.</param>
        /// <param name="japAltMode">True for Japanese texts with ten-ten and maru characters encoded separately.</param>
        /// <param name="shiftValue">The value to be added to each byte value to get the correct character. Usually 0.</param>
        /// <param name="keys">The byte values that need to be linked to specific characters.</param>
        /// <param name="values">The characters linked to the key byte values.</param>
        public TextCollection(byte[] romBuffer, int indexOffset, int count, int totalSize, bool hasPaletteData,
                              bool fixedLength, bool japAltMode, byte shiftValue, byte[] keys, char[] values)
        {
            this.textConverter = new TextConverter(Game.GetRegion(romBuffer), shiftValue);
            byte[][] textIndexes = Utilities.ReadBlockGroup(romBuffer, indexOffset, 2, count);

            this.texts = new string[count];
            this.indexOffset = indexOffset;
            this.totalSize = totalSize;
            this.japAltMode = japAltMode;

            if (hasPaletteData)
            {
                this.colorIndexes = new byte[count];
            }

            if (keys != null)
            {
                this.textConverter.ReplaceKeyValues(keys, values);
            }

            byte leadingOffsetByte = (byte)((indexOffset & 0xF0000) >> 16);

            this.textOffset = int.MaxValue;
            int[] offsets = new int[count];
            for (int i = 0; i < offsets.Length; i++)
            {
                // Recreates offsets from the index table loaded above
                offsets[i] = Utilities.BytesToOffset(textIndexes[i][0], textIndexes[i][1], leadingOffsetByte);

                if (offsets[i] < this.textOffset)
                {
                    this.textOffset = offsets[i];
                }
            }

            for (int i = 0; i < this.texts.Length; i++)
            {
                byte[] textBytes;

                if (!fixedLength)
                {
                    // Dynamic text length, ends at byte 0xFF
                    textBytes = Utilities.ReadBlockUntil(romBuffer, offsets[i], 0xFF);
                }
                else
                {
                    // Fixed text length, length determined in a separate table
                    int lengthOffset = indexOffset + (count + i) * 2; // 2 bytes per offset
                    int length = romBuffer[lengthOffset];

                    if (hasPaletteData)
                    {
                        length *= 2;
                    }

                    textBytes = Utilities.ReadBlock(romBuffer, offsets[i], length);
                }

                if (hasPaletteData && textBytes.Length >= 2)
                {
                    // Remember the color palette used by the text
                    // (assuming the same color palette is used by all letters, as in the original game)
                    this.colorIndexes[i] = textBytes[1];
                }

                if (japAltMode)
                {
                    // Merge the separated ten-ten and maru characters into textBytes
                    int tenMaruIndexOffset = indexOffset + (count + i) * 2; // 2 bytes per offset
                    int tenMaruOffset = Utilities.BytesToOffset(romBuffer[tenMaruIndexOffset], romBuffer[tenMaruIndexOffset + 1], leadingOffsetByte);
                    byte[] tenMaruBytes = Utilities.ReadBlockUntil(romBuffer, tenMaruOffset, 0xFF);

                    int step = !hasPaletteData ? 1 : 2;
                    int tenMaruCount = 0;

                    for (int j = 0; j < tenMaruBytes.Length; j += step)
                    {
                        if (tenMaruBytes[j] == 0x2E || // Ten-ten
                            tenMaruBytes[j] == 0x2F) // Maru
                        {
                            tenMaruCount++;
                        }
                    }

                    if (tenMaruCount > 0)
                    {
                        byte[] textBytesTemp = new byte[textBytes.Length + tenMaruCount * step];
                        int tenMaruIterator = 0;

                        for (int j = 0; j < textBytes.Length; j += step)
                        {
                            int k = j + tenMaruIterator;
                            textBytesTemp[k] = textBytes[j];

                            if (j < tenMaruBytes.Length &&
                                (tenMaruBytes[j] == 0x2E || // Ten-ten
                                 tenMaruBytes[j] == 0x2F)) // Maru
                            {
                                textBytesTemp[k + step] = tenMaruBytes[j];
                                tenMaruIterator += step;
                            }
                        }

                        textBytes = textBytesTemp;
                    }
                }

                this.texts[i] = this.textConverter.DecodeText(textBytes, hasPaletteData);
            }
        }

        public string this[int index]
        {
            get { return this.texts[index]; }
            set
            {
                this.texts[index] = this.textConverter.GetValidatedText(value);
                this.Modified = true;
            }
        }

        public string GetFormattedText(int index)
        {
            return this.textConverter.Region == Region.Jap ?
                    this.texts[index] :
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.texts[index].ToLowerInvariant());
        }

        public byte[] GetBytes(out byte[] indexes)
        {
            indexes = new byte[this.texts.Length * (!this.japAltMode ? 2 : 4)];
            byte[] data = new byte[this.totalSize];
            bool hasPaletteData = this.colorIndexes != null;
            int index = 0;

            for (int i = 0; i < this.texts.Length; i++)
            {
                byte? paletteIndex = !hasPaletteData ? null as byte? : this.colorIndexes[i];
                string text = this.texts[i];

                if (this.japAltMode)
                {
                    // Remove ten-ten and maru characters

                    // Disconnect the ten-ten and maru characters from the preceding character
                    text = text.Normalize(NormalizationForm.FormD);

                    text = text
                        .Replace("\u3099", "") // Remove ten-ten
                        .Replace("\u309A", ""); // Remove maru
                }

                byte[] offset = Utilities.OffsetToBytes(this.textOffset + index);
                indexes[i * 2] = offset[0];
                indexes[i * 2 + 1] = offset[1];

                byte[] textBytes = this.textConverter.EncodeText(text, paletteIndex);
                TextCollection.SetBytes(data, textBytes, ref index, hasPaletteData);

                if (this.japAltMode)
                {
                    // Append ten-ten and maru data separately

                    int step = !hasPaletteData ? 1 : 2;
                    int lastTenMaruIndex = 0;

                    for (int j = 0; j < this.texts[i].Length; j++)
                    {
                        // Disconnect the ten-ten and maru characters from the character
                        string chr = this.texts[i][j].ToString().Normalize(NormalizationForm.FormD);
                        int k = j * step;

                        if (chr.Length == 1)
                        {
                            // Character has neither ten-ten nor maru
                            textBytes[k] = 0x10;

                            if (hasPaletteData)
                            {
                                // Default color palette index to match the original game
                                textBytes[k + 1] = 0x21;
                            }
                        }
                        else
                        {
                            lastTenMaruIndex = k + step;
                            textBytes[k] = chr[1] == '\u3099' ?
                                (byte)0x2E : // Ten-ten
                                (byte)0x2F; // Maru
                        }
                    }

                    offset = Utilities.OffsetToBytes(this.textOffset + index);
                    indexes[(i + this.texts.Length) * 2] = offset[0];
                    indexes[(i + this.texts.Length) * 2 + 1] = offset[1];

                    byte[] tenMaruBytes = new byte[lastTenMaruIndex];
                    Buffer.BlockCopy(textBytes, 0, tenMaruBytes, 0, tenMaruBytes.Length);
                    TextCollection.SetBytes(data, tenMaruBytes, ref index, hasPaletteData);
                }
            }

            return data;
        }

        private static void SetBytes(byte[] data, byte[] textBytes, ref int index, bool hasPaletteData)
        {
            Buffer.BlockCopy(textBytes, 0, data, index, textBytes.Length);
            index += textBytes.Length;
            data[index++] = 0xFF;

            if (hasPaletteData)
            {
                data[index++] = 0xFF;
            }
        }

        public void Save(byte[] romBuffer)
        {
            if (this.Modified)
            {
                byte[] indexes;
                byte[] data = this.GetBytes(out indexes);
                Buffer.BlockCopy(indexes, 0, romBuffer, this.indexOffset, indexes.Length);
                Buffer.BlockCopy(data, 0, romBuffer, this.textOffset, data.Length);
            }
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
