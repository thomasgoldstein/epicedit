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
using System.Globalization;
using System.Text;

namespace EpicEdit.Rom.Utility
{
    /// <summary>
    /// Supports conversions from ROM text data to string and vice versa.
    /// </summary>
    internal class TextConverter
    {
        public static readonly TextConverter Instance = new TextConverter();

        private TextConverter() { }

        private Region region;
        private Map<byte, char> dictionary;

        public void LoadCharacterSet(Region region)
        {
            if (this.dictionary == null)
            {
                this.dictionary = new Map<byte, char>();
            }
            else
            {
                if (this.region == region)
                {
                    return; // The wanted character set is already loaded
                }
                else
                {
                    this.dictionary.Clear();
                }
            }

            this.region = region;

            if (this.region == Region.Jap)
            {
                this.dictionary.Add(0x00, '0');
                this.dictionary.Add(0x01, '1');
                this.dictionary.Add(0x02, '2');
                this.dictionary.Add(0x03, '3');
                this.dictionary.Add(0x04, '4');
                this.dictionary.Add(0x05, '5');
                this.dictionary.Add(0x06, '6');
                this.dictionary.Add(0x07, '7');
                this.dictionary.Add(0x08, '8');
                this.dictionary.Add(0x09, '9');
                this.dictionary.Add(0x0A, 'A');
                this.dictionary.Add(0x0B, 'B');
                this.dictionary.Add(0x0C, 'C');
                this.dictionary.Add(0x0D, 'D');
                this.dictionary.Add(0x0E, 'E');
                this.dictionary.Add(0x0F, 'F');
                this.dictionary.Add(0x20, 'G');
                this.dictionary.Add(0x21, 'P');
                this.dictionary.Add(0x22, 'V');
                this.dictionary.Add(0x23, 'S');
                this.dictionary.Add(0x24, '?');
                this.dictionary.Add(0x25, '.');
                this.dictionary.Add(0x26, ',');
                this.dictionary.Add(0x27, '!');
                this.dictionary.Add(0x28, '\'');
                this.dictionary.Add(0x29, '"');
                this.dictionary.Add(0x2C, ' ');
                this.dictionary.Add(0x2E, '\u3099'); // Ten-ten
                this.dictionary.Add(0x2F, '\u309A'); // Maru
                this.dictionary.Add(0x30, 'あ');
                this.dictionary.Add(0x31, 'い');
                this.dictionary.Add(0x32, 'う');
                this.dictionary.Add(0x33, 'え');
                this.dictionary.Add(0x34, 'お');
                this.dictionary.Add(0x35, 'か');
                this.dictionary.Add(0x36, 'き');
                this.dictionary.Add(0x37, 'く');
                this.dictionary.Add(0x38, 'け');
                this.dictionary.Add(0x39, 'こ');
                this.dictionary.Add(0x3A, 'さ');
                this.dictionary.Add(0x3B, 'し');
                this.dictionary.Add(0x3C, 'す');
                this.dictionary.Add(0x3D, 'せ');
                this.dictionary.Add(0x3E, 'そ');
                this.dictionary.Add(0x3F, 'た');
                this.dictionary.Add(0x40, 'ち');
                this.dictionary.Add(0x41, 'つ');
                this.dictionary.Add(0x42, 'て');
                this.dictionary.Add(0x43, 'と');
                this.dictionary.Add(0x44, 'な');
                this.dictionary.Add(0x45, 'に');
                this.dictionary.Add(0x46, 'ぬ');
                this.dictionary.Add(0x47, 'ね');
                this.dictionary.Add(0x48, 'の');
                this.dictionary.Add(0x49, 'は');
                this.dictionary.Add(0x4A, 'ひ');
                this.dictionary.Add(0x4B, 'ふ');
                this.dictionary.Add(0x4C, 'へ');
                this.dictionary.Add(0x4D, 'ほ');
                this.dictionary.Add(0x4E, 'ま');
                this.dictionary.Add(0x4F, 'み');
                this.dictionary.Add(0x50, 'む');
                this.dictionary.Add(0x51, 'め');
                this.dictionary.Add(0x52, 'も');
                this.dictionary.Add(0x53, 'や');
                this.dictionary.Add(0x54, 'ゆ');
                this.dictionary.Add(0x55, 'よ');
                this.dictionary.Add(0x56, 'ら');
                this.dictionary.Add(0x57, 'り');
                this.dictionary.Add(0x58, 'る');
                this.dictionary.Add(0x59, 'れ');
                this.dictionary.Add(0x5A, 'ろ');
                this.dictionary.Add(0x5B, 'わ');
                this.dictionary.Add(0x5C, 'を');
                this.dictionary.Add(0x5D, 'ん');
                this.dictionary.Add(0x5E, '(');
                this.dictionary.Add(0x5F, '+');
                this.dictionary.Add(0x60, 'ア');
                this.dictionary.Add(0x61, 'イ');
                this.dictionary.Add(0x62, 'ウ');
                this.dictionary.Add(0x63, 'エ');
                this.dictionary.Add(0x64, 'オ');
                this.dictionary.Add(0x65, 'カ');
                this.dictionary.Add(0x66, 'キ');
                this.dictionary.Add(0x67, 'ク');
                this.dictionary.Add(0x68, 'ケ');
                this.dictionary.Add(0x69, 'コ');
                this.dictionary.Add(0x6A, 'サ');
                this.dictionary.Add(0x6B, 'シ');
                this.dictionary.Add(0x6C, 'ス');
                this.dictionary.Add(0x6D, 'セ');
                this.dictionary.Add(0x6E, 'ソ');
                this.dictionary.Add(0x6F, 'タ');
                this.dictionary.Add(0x70, 'チ');
                this.dictionary.Add(0x71, 'ツ');
                this.dictionary.Add(0x72, 'テ');
                this.dictionary.Add(0x73, 'ト');
                this.dictionary.Add(0x74, 'ナ');
                this.dictionary.Add(0x75, 'ニ');
                this.dictionary.Add(0x76, 'ヌ');
                this.dictionary.Add(0x77, 'ネ');
                this.dictionary.Add(0x78, 'ノ');
                this.dictionary.Add(0x79, 'ハ');
                this.dictionary.Add(0x7A, 'ヒ');
                this.dictionary.Add(0x7B, 'フ');
                this.dictionary.Add(0x7C, 'ヘ');
                this.dictionary.Add(0x7D, 'ホ');
                this.dictionary.Add(0x7E, 'マ');
                this.dictionary.Add(0x7F, 'ミ');
                this.dictionary.Add(0x80, 'ム');
                this.dictionary.Add(0x81, 'メ');
                this.dictionary.Add(0x82, 'モ');
                this.dictionary.Add(0x83, 'ヤ');
                this.dictionary.Add(0x84, 'ユ');
                this.dictionary.Add(0x85, 'ヨ');
                this.dictionary.Add(0x86, 'ラ');
                this.dictionary.Add(0x87, 'リ');
                this.dictionary.Add(0x88, 'ル');
                this.dictionary.Add(0x89, 'レ');
                this.dictionary.Add(0x8A, 'ロ');
                this.dictionary.Add(0x8B, 'ワ');
                this.dictionary.Add(0x8C, 'ェ');
                this.dictionary.Add(0x8D, 'ン');
                this.dictionary.Add(0x8E, '。');
                this.dictionary.Add(0x8F, 'ヽ');
                this.dictionary.Add(0x91, 'っ');
                this.dictionary.Add(0x94, 'ょ');
                this.dictionary.Add(0x97, 'ッ');
                this.dictionary.Add(0x98, 'ャ');
                this.dictionary.Add(0x99, 'ァ');
                this.dictionary.Add(0x9A, 'ョ');
                this.dictionary.Add(0x9C, 'ー');
            }
            else
            {
                this.dictionary.Add(0x00, '0');
                this.dictionary.Add(0x01, '1');
                this.dictionary.Add(0x02, '2');
                this.dictionary.Add(0x03, '3');
                this.dictionary.Add(0x04, '4');
                this.dictionary.Add(0x05, '5');
                this.dictionary.Add(0x06, '6');
                this.dictionary.Add(0x07, '7');
                this.dictionary.Add(0x08, '8');
                this.dictionary.Add(0x09, '9');
                this.dictionary.Add(0x0A, 'A');
                this.dictionary.Add(0x0B, 'B');
                this.dictionary.Add(0x0C, 'C');
                this.dictionary.Add(0x0D, 'D');
                this.dictionary.Add(0x0E, 'E');
                this.dictionary.Add(0x0F, 'F');
                this.dictionary.Add(0x10, 'G');
                this.dictionary.Add(0x11, 'H');
                this.dictionary.Add(0x12, 'I');
                this.dictionary.Add(0x13, 'J');
                this.dictionary.Add(0x14, 'K');
                this.dictionary.Add(0x15, 'L');
                this.dictionary.Add(0x16, 'M');
                this.dictionary.Add(0x17, 'N');
                this.dictionary.Add(0x18, 'O');
                this.dictionary.Add(0x19, 'P');
                this.dictionary.Add(0x1A, 'Q');
                this.dictionary.Add(0x1B, 'R');
                this.dictionary.Add(0x1C, 'S');
                this.dictionary.Add(0x1D, 'T');
                this.dictionary.Add(0x1E, 'U');
                this.dictionary.Add(0x1F, 'V');
                this.dictionary.Add(0x20, 'W');
                this.dictionary.Add(0x21, 'X');
                this.dictionary.Add(0x22, 'Y');
                this.dictionary.Add(0x23, 'Z');
                this.dictionary.Add(0x24, '?');
                this.dictionary.Add(0x25, '.');
                this.dictionary.Add(0x26, ',');
                this.dictionary.Add(0x27, '!');
                this.dictionary.Add(0x28, '\'');
                this.dictionary.Add(0x29, '"');
                this.dictionary.Add(0x2E, ':');
                this.dictionary.Add(0x2F, ' ');
                this.dictionary.Forward.Add(0x2C, ' ');
                this.dictionary.Forward.Add(0xAF, ' ');
            }
        }

        /// <summary>
        /// Decodes a single character, from the ROM format to a char.
        /// This function assumes that the location of each character tile hasn't been changed (in the ROM font graphics), which may be wrong.
        /// </summary>
        /// <param name="charByte">The value that defines the location of the character among the font graphics.</param>
        /// <returns>The corresponding character.</returns>
        public char DecodeText(byte charByte)
        {
            if (!this.dictionary.Forward.ContainsKey(charByte))
            {
                return '?';
            }

            return this.dictionary.Forward[charByte];
        }

        /// <summary>
        /// Decodes text data, from the ROM format to a string.
        /// </summary>
        /// <param name="textBytes">Values that define the location of each character of a text string among the font graphics.</param>
        /// <param name="skipOddBytes">If true, skip every other byte. Necessary for text which contains the color palette association for each character.</param>
        /// <returns>The corresponding text string.</returns>
        public string DecodeText(byte[] textBytes, bool skipOddBytes)
        {
            return this.DecodeText(textBytes, !skipOddBytes ? 1 : 2);
        }

        private string DecodeText(byte[] textBytes, int step)
        {
            char[] textArray = new char[textBytes.Length / step];

            for (int i = 0; i < textArray.Length; i++)
            {
                textArray[i] = this.DecodeText(textBytes[i * step]);
            }

            string text = new string(textArray);

            if (this.region == Region.Jap)
            {
                // Japanese text formatting
                // (needed to connect the ten-ten and maru characters to the preceding character)
                text = text.Normalize(NormalizationForm.FormC);
            }
            else
            {
                text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
            }

            return text;
        }

        public byte[] EncodeText(string text, byte? paletteIndex)
        {
            int step;
            byte palIndex;

            if (paletteIndex == null)
            {
                step = 1;
                palIndex = 0;
            }
            else
            {
                step = 2;
                palIndex = paletteIndex.Value;
            }

            return this.EncodeText(text, step, palIndex);
        }

        private byte[] EncodeText(string text, int step, byte paletteIndex)
        {
            text = text.ToUpperInvariant();

            if (this.region == Region.Jap)
            {
                text = text.Normalize(NormalizationForm.FormD);
            }

            byte[] data = new byte[text.Length * step];

            for (int i = 0; i < text.Length; i++)
            {
                data[i * step] = this.dictionary.Reverse[text[i]];

                if (step > 1)
                {
                    data[i * step + 1] = paletteIndex;
                }
            }

            return data;
        }
    }
}
