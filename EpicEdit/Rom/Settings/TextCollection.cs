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

using EpicEdit.Rom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// A collection of texts.
    /// </summary>
    internal class TextCollection : IEnumerable<TextItem>, ITextCollection, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the converter used to translate font graphics index values into .NET strings.
        /// </summary>
        public TextConverter Converter { get; }

        public Region Region => Converter.Region;

        /// <summary>
        /// The texts of the collection.
        /// </summary>
        private readonly TextItem[] _texts;

        /// <summary>
        /// The offset to the indexes that define the address of each text.
        /// </summary>
        private readonly int _indexOffset;

        /// <summary>
        /// The offset to the start of the text data.
        /// </summary>
        private readonly int _textOffset;

        /// <summary>
        /// The color palette index used by each text (using the 1st character as reference).
        /// </summary>
        private readonly byte[] _colorIndexes;

        /// <summary>
        /// The total size in bytes of the collection.
        /// </summary>
        private readonly int _totalSize;

        /// <summary>
        /// A mode for certain Japanese text collections with ten-ten and maru characters encoded separately.
        /// </summary>
        private readonly bool _japAltMode;

        /// <summary>
        /// True if characters are tall (two 8x8 vertically-laid tiles), false otherwise (one 8x8 tile per character).
        /// </summary>
        private readonly bool _tallCharacters;

        /// <summary>
        /// Gets the maximum amount of characters available for the whole collection.
        /// </summary>
        public int MaxCharacterCount { get; }

        /// <summary>
        /// Gets the total amount of characters used by the whole collection.
        /// </summary>
        public int TotalCharacterCount
        {
            get
            {
                GetBytes(out var indexes, out var length);
                return GetCharacterCount(length);
            }
        }

        private int GetCharacterCount(int byteCount)
        {
            var step = _colorIndexes == null ? 1 : 2;
            return (byteCount / step) - _texts.Length;
        }

        public bool Modified { get; private set; }

        /// <summary>
        /// Initializes a text collection.
        /// </summary>
        /// <param name="romBuffer">The ROM buffer.</param>
        /// <param name="indexOffset">The location of the index table to each text offset.</param>
        /// <param name="count">The number of texts in the collection.</param>
        /// <param name="totalSize">The total size in bytes of the collection.</param>
        /// <param name="hasPaletteData">True if the text data is mixed with color palette indexes (ie: each letter followed by the index of its associated palette).</param>
        /// <param name="fixedLength">True if the length of each text is specified separately in a length table.
        /// Otherwise each text item ends with 0xFF.</param>
        /// <param name="japAltMode">True for Japanese texts with ten-ten and maru characters encoded separately.</param>
        /// <param name="tallCharacters">True if characters are tall (two 8x8 vertically-laid tiles), false otherwise (one 8x8 tile per character).</param>
        /// <param name="shiftValue">The value to be added to each byte value to get the correct character. Usually 0.</param>
        /// <param name="keys">The byte values that need to be linked to specific characters.</param>
        /// <param name="values">The characters linked to the key byte values.</param>
        public TextCollection(byte[] romBuffer, int indexOffset, int count, int totalSize, bool hasPaletteData,
                              bool fixedLength, bool japAltMode, bool tallCharacters, byte shiftValue, byte[] keys, char[] values)
        {
            Converter = new TextConverter(Game.GetRegion(romBuffer), tallCharacters, shiftValue);
            var textIndexes = Utilities.ReadBlockGroup(romBuffer, indexOffset, 2, count);

            _texts = new TextItem[count];
            _indexOffset = indexOffset;
            _totalSize = totalSize;
            _japAltMode = japAltMode;
            _tallCharacters = tallCharacters;

            if (hasPaletteData)
            {
                _colorIndexes = new byte[count];
            }

            MaxCharacterCount = GetCharacterCount(_totalSize);

            if (keys != null)
            {
                Converter.ReplaceKeyValues(keys, values);
            }

            var leadingOffsetByte = (byte)((indexOffset & 0xF0000) >> 16);

            _textOffset = int.MaxValue;
            var offsets = new int[count];
            for (var i = 0; i < offsets.Length; i++)
            {
                // Recreates offsets from the index table loaded above
                offsets[i] = Utilities.BytesToOffset(textIndexes[i][0], textIndexes[i][1], leadingOffsetByte);

                if (offsets[i] < _textOffset)
                {
                    _textOffset = offsets[i];
                }
            }

            for (var i = 0; i < _texts.Length; i++)
            {
                byte[] textBytes;

                if (!fixedLength)
                {
                    // Dynamic text length, ends at bytes 0xFF or 0xFFFF
                    var stopValues = !hasPaletteData ? new byte[] { 0xFF } : new byte[] { 0xFF, 0xFF };
                    textBytes = Utilities.ReadBlockUntil(romBuffer, offsets[i], stopValues);
                }
                else
                {
                    // Fixed text length, length determined in a separate table
                    var lengthOffset = indexOffset + (count + i) * 2; // 2 bytes per offset
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
                    _colorIndexes[i] = textBytes[1];
                }

                if (japAltMode)
                {
                    // Merge the separated ten-ten and maru characters into textBytes
                    var tenMaruIndexOffset = indexOffset + (count + i) * 2; // 2 bytes per offset
                    var tenMaruOffset = Utilities.BytesToOffset(romBuffer[tenMaruIndexOffset], romBuffer[tenMaruIndexOffset + 1], leadingOffsetByte);
                    var tenMaruBytes = Utilities.ReadBlockUntil(romBuffer, tenMaruOffset, 0xFF);

                    var step = !hasPaletteData ? 1 : 2;
                    var tenMaruCount = 0;

                    for (var j = 0; j < tenMaruBytes.Length; j += step)
                    {
                        if (tenMaruBytes[j] == 0x2E || // Ten-ten
                            tenMaruBytes[j] == 0x2F) // Maru
                        {
                            tenMaruCount++;
                        }
                    }

                    if (tenMaruCount > 0)
                    {
                        var textBytesTemp = new byte[textBytes.Length + tenMaruCount * step];
                        var tenMaruIterator = 0;

                        for (var j = 0; j < textBytes.Length; j += step)
                        {
                            var k = j + tenMaruIterator;
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

                _texts[i] = new TextItem(this, Converter.DecodeText(textBytes, hasPaletteData));
                _texts[i].PropertyChanged += TextItem_PropertyChanged;
            }
        }

        private void TextItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Modified = true;
            OnPropertyChanged(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public TextItem this[int index] => _texts[index];

        public int IndexOf(TextItem item)
        {
            for (var i = 0; i < _texts.Length; i++)
            {
                if (_texts[i] == item)
                {
                    return i;
                }
            }

            return -1;
        }

        public byte[] GetBytes(out byte[] indexes)
        {
            return GetBytes(out indexes, out var length);
        }

        protected virtual byte[] GetBytes(out byte[] indexes, out int length)
        {
            indexes = new byte[_texts.Length * (!_japAltMode ? 2 : 4)];
            var data = new byte[_totalSize];
            var hasPaletteData = _colorIndexes != null;
            var index = 0;

            for (var i = 0; i < _texts.Length; i++)
            {
                var paletteIndex = !hasPaletteData ? null as byte? : _colorIndexes[i];
                var text = _texts[i].Value;

                if (_japAltMode)
                {
                    // Remove ten-ten and maru characters

                    // Disconnect the ten-ten and maru characters from the preceding character
                    text = text.Normalize(NormalizationForm.FormD);

                    text = text
                        .Replace("\u3099", "") // Remove ten-ten
                        .Replace("\u309A", ""); // Remove maru
                }

                var offset = Utilities.OffsetToBytes(_textOffset + index);
                indexes[i * 2] = offset[0];
                indexes[i * 2 + 1] = offset[1];

                var textBytes = Converter.EncodeText(text, paletteIndex);
                SetBytes(data, textBytes, ref index, hasPaletteData);

                if (_japAltMode)
                {
                    // Append ten-ten and maru data separately

                    var step = !hasPaletteData ? 1 : 2;
                    var lastTenMaruIndex = 0;

                    for (var j = 0; j < _texts[i].Value.Length; j++)
                    {
                        // Disconnect the ten-ten and maru characters from the character
                        var chr = _texts[i].Value[j].ToString().Normalize(NormalizationForm.FormD);
                        var k = j * step;

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

                    offset = Utilities.OffsetToBytes(_textOffset + index);
                    indexes[(i + _texts.Length) * 2] = offset[0];
                    indexes[(i + _texts.Length) * 2 + 1] = offset[1];

                    var tenMaruBytes = new byte[lastTenMaruIndex];
                    Buffer.BlockCopy(textBytes, 0, tenMaruBytes, 0, tenMaruBytes.Length);
                    SetBytes(data, tenMaruBytes, ref index, hasPaletteData);
                }
            }

            length = index;

            if (_tallCharacters)
            {
                // Fix space character attributes (removing palette association)

                for (var i = 0; i < length; i += 2)
                {
                    if (data[i] == 0xE5)
                    {
                        data[i + 1] = 0x00;
                    }
                }
            }

            for (var i = length; i < _totalSize; i++)
            {
                // Fill the unused end of the data with 0xFF
                data[i] = 0xFF;
            }

            return data;
        }

        private static void SetBytes(byte[] data, byte[] textBytes, ref int index, bool hasPaletteData)
        {
            var length = index + textBytes.Length + (!hasPaletteData ? 1 : 2);
            if (data.Length < length)
            {
                // The text data is too big to fit into the data array.
                // This can only happen when the GetBytes method is called from TotalCharacterCount,
                // because otherwise all texts are guaranteed to fit within the collection.
                index = length;
                return;
            }

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
            if (Modified)
            {
                var data = GetBytes(out var indexes);
                Buffer.BlockCopy(indexes, 0, romBuffer, _indexOffset, indexes.Length);
                Buffer.BlockCopy(data, 0, romBuffer, _textOffset, data.Length);
            }
        }

        public void ResetModifiedState()
        {
            Modified = false;
        }

        public IEnumerator<TextItem> GetEnumerator()
        {
            foreach (var text in _texts)
            {
                yield return text;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _texts.GetEnumerator();
        }

        public int Count => _texts.Length;
    }
}
