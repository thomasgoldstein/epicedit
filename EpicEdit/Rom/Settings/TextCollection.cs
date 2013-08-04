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

        public TextCollection(byte[] romBuffer, int indexOffset, int count, int totalSize, bool skipOddBytes)
        {
            byte[][] textIndexes = Utilities.ReadBlockGroup(romBuffer, indexOffset, 2, count);

            this.texts = new string[count];

            if (skipOddBytes)
            {
                this.colorIndexes = new byte[count];
            }

            byte leadingOffsetByte = (byte)((indexOffset & 0xF0000) >> 16);

            for (int i = 0; i < this.texts.Length; i++)
            {
                int offset = Utilities.BytesToOffset(textIndexes[i][0], textIndexes[i][1], leadingOffsetByte); // Recreates offsets from the index table loaded above
                byte[] textBytes = Utilities.ReadBlockUntil(romBuffer, offset, 0xFF);
                this.texts[i] = TextConverter.Instance.DecodeText(textBytes, skipOddBytes);

                if (skipOddBytes)
                {
                    this.colorIndexes[i] = TextCollection.GetColorIndex(textBytes);
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

        /// <summary>
        /// Gets the first character color index that is different from 0.
        /// </summary>
        private static byte GetColorIndex(byte[] textBytes)
        {
            byte colorIndex = 0;

            for (int i = 1; i < textBytes.Length; i += 2)
            {
                if (textBytes[i] != 0)
                {
                    colorIndex = textBytes[i];
                    break;
                }
            }

            return colorIndex;
        }

        public byte[] GetBytes()
        {
            byte[] data = new byte[this.totalSize];
            bool hasPaletteData = this.colorIndexes != null;
            int index = 0;

            for (int i = 0; i < this.texts.Length; i++)
            {
                byte? paletteIndex = !hasPaletteData ? null as byte? : this.colorIndexes[i];
                byte[] textBytes = TextConverter.Instance.EncodeText(this.texts[i], paletteIndex);

                Buffer.BlockCopy(textBytes, 0, data, index, textBytes.Length);
                index += textBytes.Length;
                textBytes[index++] = 0xFF;

                if (hasPaletteData)
                {
                    textBytes[index++] = 0xFF;
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
