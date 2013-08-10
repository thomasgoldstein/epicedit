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
        private Region region;
        private Map<byte, char> dictionary;

        public TextConverter(Region region, byte shiftValue)
        {
            this.region = region;
            this.LoadCharacterSet(shiftValue);
        }

        /// <summary>
        /// Loads the character set for the current region.
        /// This function assumes that the location of each character tile hasn't been changed
        /// (in the ROM font graphics), which may be wrong.
        /// </summary>
        private void LoadCharacterSet(byte shiftValue)
        {
            char[] chars = this.region == Region.Jap ?
                CharacterSetJap.Get() :
                CharacterSetUS.Get();

            this.dictionary = new Map<byte, char>();

            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] != char.MinValue)
                {
                    this.dictionary.Add((byte)(i + shiftValue), chars[i]);
                }
            }
        }

        public void ReplaceKeyValues(byte[] keys, char[] values)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (this.dictionary.Forward.ContainsKey(keys[i]))
                {
                    this.dictionary.Remove(keys[i]);
                }

                if (this.dictionary.Reverse.ContainsKey(values[i]))
                {
                    byte key = this.dictionary.Reverse[values[i]];
                    this.dictionary.Remove(key);
                }

                this.dictionary.Add(keys[i], values[i]);
            }
        }

        /// <summary>
        /// Decodes a single character, from the ROM format to a char.
        /// </summary>
        /// <param name="charByte">The value that defines the location of the character among the font graphics.</param>
        /// <returns>The corresponding character.</returns>
        public char DecodeText(byte charByte)
        {
            return !this.dictionary.Forward.ContainsKey(charByte) ?
                '?' : this.dictionary.Forward[charByte];
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
                // Japanese text formatting
                // (needed to disconnect the ten-ten and maru characters from the preceding character)
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
