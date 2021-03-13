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
using System.Collections.Generic;

namespace EpicEdit.Rom.Compression
{
    /// <summary>
    /// Represents a dictionary of bytes.
    /// </summary>
    internal class ByteDictionary
    {
        private readonly byte[] _buffer;
        private readonly Dictionary<byte, List<int>> _byteDictionary;

        public ByteDictionary(byte[] buffer)
        {
            _buffer = buffer;
            _byteDictionary = new Dictionary<byte, List<int>>();

            for (var i = 0; i < buffer.Length; i++)
            {
                Add(buffer[i], i);
            }
        }

        private void Add(byte value, int offset)
        {
            if (!_byteDictionary.TryGetValue(value, out var list))
            {
                list = new List<int>();
                _byteDictionary.Add(value, list);
            }

            list.Add(offset);
        }

        public Range GetMaxBackRange(int offset)
        {
            var maxBackRange = Range.Empty;

            var value = _buffer[offset];

            foreach (var otherOffset in _byteDictionary[value])
            {
                if (otherOffset >= offset)
                {
                    break;
                }

                var iterator = offset;
                var backIterator = otherOffset;

                do
                {
                    iterator++;
                    backIterator++;
                }
                while (iterator < _buffer.Length &&
                       _buffer[backIterator] == _buffer[iterator]);

                var start = otherOffset;
                var end = backIterator;

                var backRange = new Range(start, end);
                backRange.Length = Codec.GetValidatedSuperCommandSize(backRange.Length);

                if (backRange.Length >= maxBackRange.Length)
                {
                    maxBackRange = backRange;
                }
            }

            return maxBackRange;
        }

        public Range[] GetMaxBackRanges(int offset)
        {
            var maxRange4n = Range.Empty; // Command 4 normal
            var maxRange4s = Range.Empty; // Command 4 super
            var maxRange6n = Range.Empty; // Command 6 normal
            var maxRange6s = Range.Empty; // Command 6 super

            var value = _buffer[offset];

            var startPosition = offset - 0xFF;
            if (startPosition < 0)
            {
                startPosition = 0;
            }

            foreach (var otherOffset in _byteDictionary[value])
            {
                if (otherOffset >= offset)
                {
                    break;
                }

                var iterator = offset;
                var backIterator = otherOffset;

                do
                {
                    iterator++;
                    backIterator++;
                }
                while (iterator < _buffer.Length &&
                       _buffer[backIterator] == _buffer[iterator]);

                var start = otherOffset;
                var end = backIterator;

                var backRange = new Range(start, end);
                backRange.Length = Codec.GetValidatedSuperCommandSize(backRange.Length);

                if (otherOffset < startPosition)
                {
                    if (backRange.Length >= maxRange4s.Length)
                    {
                        maxRange4s = backRange;
                    }
                }
                else
                {
                    if (backRange.Length >= maxRange6s.Length)
                    {
                        maxRange6s = backRange;
                    }
                }
            }

            maxRange6n = maxRange6s;
            if (maxRange6n.Length <= Codec.NormalCommandMax)
            {
                maxRange6s = Range.Empty;
            }
            else
            {
                maxRange6n.Length = Codec.NormalCommandMax;
            }

            maxRange4n = maxRange4s;
            if (maxRange4n.Length <= Codec.NormalCommandMax)
            {
                maxRange4s = Range.Empty;
            }
            else
            {
                maxRange4n.Length = Codec.NormalCommandMax;
            }

            if (maxRange4n.Length == maxRange6n.Length)
            {
                maxRange4n = Range.Empty;
            }

            if (maxRange4s.Length == maxRange6s.Length)
            {
                maxRange4s = Range.Empty;
            }

            return new[] { maxRange4n, maxRange4s, maxRange6n, maxRange6s };
        }
    }
}
