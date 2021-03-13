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
using System.Drawing;

namespace EpicEdit.Rom.Utility
{
    internal static class Utilities
    {
        #region Read byte blocks

        /// <summary>
        /// Reads of a part of the ROM, byte by byte, until we reach a certain amount of bytes.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve data from.</param>
        /// <param name="offset">The buffer position the function starts reading from.</param>
        /// <param name="length">The amount of bytes to retrieve.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] ReadBlock(byte[] buffer, int offset, int length)
        {
            var bytes = new byte[length];
            Buffer.BlockCopy(buffer, offset, bytes, 0, length);
            return bytes;
        }

        /// <summary>
        /// Reads a part of the ROM, byte by byte, until we reach a certain value.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve data from.</param>
        /// <param name="offset">The buffer position the function starts reading from.</param>
        /// <param name="stopValues">The function keeps reading the buffer until it reaches these values. Null if there is no stop value.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] ReadBlockUntil(byte[] buffer, int offset, params byte[] stopValues)
        {
            int length;
            if (stopValues == null) // There is no stop value, we continue until the end of the buffer
            {
                length = buffer.Length - offset;
            }
            else
            {
                length = 0;
                while (!IsBlockLimitReached(buffer, offset + length, stopValues))
                {
                    length++;
                }
            }

            return ReadBlock(buffer, offset, length);
        }

        private static bool IsBlockLimitReached(byte[] buffer, int offset, params byte[] stopValues)
        {
            for (var i = 0; i < stopValues.Length; i++)
            {
                if (buffer[offset + i] != stopValues[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Reads a part of the ROM, dividing the result by groups of a certain size, until we reach a certain amount of groups.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve data from.</param>
        /// <param name="offset">The buffer position the function starts reading from.</param>
        /// <param name="groupSize">Defines the number of bytes per group.</param>
        /// <param name="groupCount">Defines the number of groups to return.</param>
        /// <returns>A jagged array that contains byte groups.</returns>
        public static byte[][] ReadBlockGroup(byte[] buffer, int offset, int groupSize, int groupCount)
        {
            var group = new byte[groupCount][];

            for (var i = 0; i < groupCount; i++)
            {
                group[i] = ReadBlock(buffer, offset + (i * groupSize), groupSize);
            }

            return group;
        }

        /// <summary>
        /// Reads a part of the ROM, dividing the result by groups of a certain size, until we reach a certain value.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve data from.</param>
        /// <param name="offset">The buffer position the function starts reading from.</param>
        /// <param name="stopValue">The function keeps reading the buffer until it reaches this value. -1 means there is no stop value.</param>
        /// <param name="groupSize">Defines the number of bytes per group.</param>
        /// <returns>A jagged array that contains byte groups.</returns>
        public static byte[][] ReadBlockGroupUntil(byte[] buffer, int offset, int stopValue, int groupSize)
        {
            int length;
            if (stopValue == -1) // Means there is no stop value, we continue until the end of the buffer
            {
                length = buffer.Length - offset;
            }
            else
            {
                length = 0;
                while (buffer[offset + length] != stopValue)
                {
                    length++;
                }
            }

            length /= groupSize;

            var group = new byte[length][];

            for (var i = 0; i < length; i++)
            {
                group[i] = ReadBlock(buffer, offset + (i * groupSize), groupSize);
            }

            return group;
        }

        /// <summary>
        /// Retrieves offsets (ROM addresses) from a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve data from.</param>
        /// <param name="offset">The buffer position the function starts reading from.</param>
        /// <param name="offsetCount">The number of offsets to retrieve.</param>
        /// <returns>An array of offsets.</returns>
        public static int[] ReadBlockOffset(byte[] buffer, int offset, int offsetCount)
        {
            const int offsetSize = 3;
            var offsetGroup = new int[offsetCount];

            for (var i = 0; i < offsetCount; i++)
            {
                offsetGroup[i] = BytesToOffset(buffer, offset + (i * offsetSize));
            }

            return offsetGroup;
        }

        #endregion Read byte blocks

        #region Bytes <-> Offset conversion

        public static int BytesToOffset(byte[] data, int start)
        {
            return BytesToOffset(data[start], data[start + 1], data[start + 2]);
        }

        public static int BytesToOffset(byte val1, byte val2, byte val3)
        {
            return ((val3 & 0xF) << 16) + (val2 << 8) + val1;
        }

        public static byte[] OffsetToBytes(int data)
        {
            if (data > 0xFFFFF)
            {
                throw new ArgumentOutOfRangeException(nameof(data), "The offset value is too high.");
            }

            return new byte[]
            {
                (byte)(data & 0xFF),
                (byte)((data & 0xFF00) >> 8),
                (byte)(0xC0 + ((data & 0xF0000) >> 16))
            };
        }

        #endregion Bytes <-> Offset conversion

        #region Bytes <-> String conversion

        public static byte[] HexStringToBytes(string data)
        {
            var bytes = new byte[data.Length / 2];
            LoadBytesFromHexString(bytes, data);
            return bytes;
        }

        public static void LoadBytesFromHexString(byte[] bytes, string hex)
        {
            var bl = bytes.Length;
            for (var i = 0; i < bl; ++i)
            {
                bytes[i] = (byte)((hex[2 * i] > 'F' ? hex[2 * i] - 0x57 : hex[2 * i] > '9' ? hex[2 * i] - 0x37 : hex[2 * i] - 0x30) << 4);
                bytes[i] |= (byte)(hex[2 * i + 1] > 'F' ? hex[2 * i + 1] - 0x57 : hex[2 * i + 1] > '9' ? hex[2 * i + 1] - 0x37 : hex[2 * i + 1] - 0x30);
            }
        }

        public static string BytesToHexString(byte[] data)
        {
            byte b;
            int i, j, k;
            var l = data.Length;
            var r = new char[l * 2];
            for (i = 0, j = 0; i < l; ++i)
            {
                b = data[i];
                k = b >> 4;
                r[j++] = (char)(k > 9 ? k + 0x37 : k + 0x30);
                k = b & 15;
                r[j++] = (char)(k > 9 ? k + 0x37 : k + 0x30);
            }
            return new string(r);
        }

        public static string ByteToHexString(byte data)
        {
            byte b;
            int k;
            var r = new char[2];
            b = data;
            k = b >> 4;
            r[0] = (char)(k > 9 ? k + 0x37 : k + 0x30);
            k = b & 15;
            r[1] = (char)(k > 9 ? k + 0x37 : k + 0x30);
            return new string(r);
        }

        #endregion Bytes <-> String conversion

        #region Color handling

        public static int GetColorIndex(Color color, Palette palette)
        {
            var colors = new RomColor[Palette.ColorCount];

            for (var i = 0; i < colors.Length; i++)
            {
                colors[i] = palette[i];
            }

            return GetColorIndex(color, colors);
        }

        public static int GetColorIndex(RomColor color, RomColor[] colors)
        {
            var colorIndex = -1;

            for (var i = 0; i < colors.Length; i++)
            {
                if (colors[i] == color)
                {
                    colorIndex = i;
                    break;
                }
            }

            if (colorIndex == -1)
            {
                colorIndex = GetClosestColorIndex(color, colors);
            }

            return colorIndex;
        }

        public static int GetClosestColorIndex(Color color, RomColor[] colors)
        {
            var value = GetColorHslValue(color);
            var diff = float.MaxValue;
            var match = -1;

            for (var i = 0; i < colors.Length; i++)
            {
                Color col = colors[i];
                var value2 = GetColorHslValue(col);
                var diff2 = Math.Abs(value - value2);

                if (diff2 < diff)
                {
                    diff = diff2;
                    match = i;
                }
            }

            return match;
        }

        private static float GetColorHslValue(Color color)
        {
            const float hueWeight = 0.8f;
            const float saturationWeight = 0.1f;
            const float brightnessWeight = 0.1f;

            return (color.GetHue() / 360.0f) * hueWeight +
                color.GetSaturation() * saturationWeight +
                color.GetBrightness() * brightnessWeight;
        }

        #endregion Color handling
    }
}
