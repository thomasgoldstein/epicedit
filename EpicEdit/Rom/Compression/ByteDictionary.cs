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
using System.Collections.Generic;

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// Represents a dictionary of bytes.
    /// </summary>
    internal class ByteDictionary
    {
        private byte[] buffer;
        private Dictionary<byte, List<int>> byteDictionary;

        public ByteDictionary(byte[] buffer)
        {
            this.buffer = buffer;
            this.byteDictionary = new Dictionary<byte, List<int>>();

            for (int i = 0; i < buffer.Length; i++)
            {
                this.Add(buffer[i], i);
            }
        }

        private void Add(byte value, int offset)
        {
            if (!this.byteDictionary.ContainsKey(value))
            {
                this.byteDictionary.Add(value, new List<int>());
            }

            this.byteDictionary[value].Add(offset);
        }

        public Range GetMaxBackRange(int offset)
        {
            Range maxBackRange = Range.Empty;

            byte value = this.buffer[offset];

            foreach (int otherOffset in this.byteDictionary[value])
            {
                if (otherOffset >= offset)
                {
                    break;
                }

                int iterator = offset;
                int backIterator = otherOffset;

                do
                {
                    iterator++;
                    backIterator++;
                }
                while (iterator < this.buffer.Length &&
                       this.buffer[backIterator] == this.buffer[iterator]);

                int start = otherOffset;
                int end = backIterator;

                Range backRange = new Range(start, end);

                if (backRange.Length > Codec.SuperCommandMax)
                {
                    backRange.Length = Codec.SuperCommandMax;
                }

                if (backRange.Length >= maxBackRange.Length)
                {
                    maxBackRange = backRange;
                }
            }

            return maxBackRange;
        }

        public Range[] GetMaxBackRanges(int offset)
        {
            Range[] maxRanges = new Range[4];
            maxRanges[0] = Range.Empty; // Command 4 normal
            maxRanges[1] = Range.Empty; // Command 4 super
            maxRanges[2] = Range.Empty; // Command 6 normal
            maxRanges[3] = Range.Empty; // Command 6 super

            byte value = this.buffer[offset];

            int startPosition = offset - 0xFF;
            if (startPosition < 0)
            {
                startPosition = 0;
            }

            foreach (int otherOffset in this.byteDictionary[value])
            {
                if (otherOffset >= offset)
                {
                    break;
                }

                int iterator = offset;
                int backIterator = otherOffset;

                do
                {
                    iterator++;
                    backIterator++;
                }
                while (iterator < this.buffer.Length &&
                       this.buffer[backIterator] == this.buffer[iterator]);

                int start = otherOffset;
                int end = backIterator;

                Range backRange = new Range(start, end);

                if (backRange.Length > Codec.SuperCommandMax)
                {
                    backRange.Length = Codec.SuperCommandMax;
                }

                int rangeArrayIndex = otherOffset < startPosition ? 1 : 3;
                if (backRange.Length >= maxRanges[rangeArrayIndex].Length)
                {
                    maxRanges[rangeArrayIndex] = backRange;
                }
            }

            maxRanges[2] = maxRanges[3];
            if (maxRanges[2].Length <= Codec.NormalCommandMax)
            {
                maxRanges[3] = Range.Empty;
            }
            else
            {
                maxRanges[2].Length = Codec.NormalCommandMax;
            }

            maxRanges[0] = maxRanges[1];
            if (maxRanges[0].Length <= Codec.NormalCommandMax)
            {
                maxRanges[1] = Range.Empty;
            }
            else
            {
                maxRanges[0].Length = Codec.NormalCommandMax;
            }

            if (maxRanges[0].Length == maxRanges[2].Length)
            {
                maxRanges[0] = Range.Empty;
            }

            if (maxRanges[1].Length == maxRanges[3].Length)
            {
                maxRanges[1] = Range.Empty;
            }

            return maxRanges;
        }
    }
}
