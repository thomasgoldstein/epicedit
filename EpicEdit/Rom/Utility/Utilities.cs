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
using System.Globalization;
using System.Text;

namespace EpicEdit.Rom.Utility
{
    internal class Utilities
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
            byte[] bytes = new byte[length];
            Buffer.BlockCopy(buffer, offset, bytes, 0, length);
            return bytes;
        }

        /// <summary>
        /// Reads a part of the ROM, byte by byte, until we reach a certain value.
        /// </summary>
        /// <param name="buffer">The buffer to retrieve data from.</param>
        /// <param name="offset">The buffer position the function starts reading from.</param>
        /// <param name="stopValue">The function keeps reading the buffer until it reaches this value. -1 means there is no stop value.</param>
        /// <returns>An array of bytes.</returns>
        public static byte[] ReadBlockUntil(byte[] buffer, int offset, int stopValue)
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

            return Utilities.ReadBlock(buffer, offset, length);
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
            byte[][] group = new byte[groupCount][];

            for (int i = 0; i < groupCount; i++)
            {
                group[i] = Utilities.ReadBlock(buffer, offset + (i * groupSize), groupSize);
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

            byte[][] group = new byte[length][];

            for (int i = 0; i < length; i++)
            {
                group[i] = Utilities.ReadBlock(buffer, offset + (i * groupSize), groupSize);
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
            int offsetSize = 3;
            int[] offsetGroup = new int[offsetCount];

            for (int i = 0; i < offsetCount; i++)
            {
                byte[] address = Utilities.ReadBlock(buffer, offset + (i * offsetSize), offsetSize);
                offsetGroup[i] = Utilities.BytesToOffset(address);
            }

            return offsetGroup;
        }

        #endregion Read byte blocks

        #region Bytes <-> Offset conversion

        public static int BytesToOffset(byte[] data)
        {
            return Utilities.BytesToOffset(data, 0);
        }

        public static int BytesToOffset(byte[] data, int start)
        {
            return Utilities.BytesToOffset(data[start], data[start + 1], data[start + 2]);
        }

        public static int BytesToOffset(byte val1, byte val2, byte val3)
        {
            return ((val3 & 0xF) << 16) + (val2 << 8) + val1;
        }

        public static byte[] OffsetToBytes(int data)
        {
            if (data > 0xFFFFF)
            {
                throw new ArgumentOutOfRangeException("data", "The offset value is too high.");
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
            byte[] bytes = new byte[data.Length / 2];
            Utilities.LoadBytesFromHexString(bytes, data);
            return bytes;
        }

        public static void LoadBytesFromHexString(byte[] bytes, string hex)
        {
            int bl = bytes.Length;
            for (int i = 0; i < bl; ++i)
            {
                bytes[i] = (byte)((hex[2 * i] > 'F' ? hex[2 * i] - 0x57 : hex[2 * i] > '9' ? hex[2 * i] - 0x37 : hex[2 * i] - 0x30) << 4);
                bytes[i] |= (byte)(hex[2 * i + 1] > 'F' ? hex[2 * i + 1] - 0x57 : hex[2 * i + 1] > '9' ? hex[2 * i + 1] - 0x37 : hex[2 * i + 1] - 0x30);
            }
        }

        public static string BytesToHexString(byte[] data)
        {
            byte b;
            int i, j, k;
            int l = data.Length;
            char[] r = new char[l * 2];
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
            char[] r = new char[2];
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
            RomColor[] colors = new RomColor[Palette.ColorCount];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = palette[i];
            }

            return Utilities.GetColorIndex(color, colors);
        }

        public static int GetColorIndex(RomColor color, RomColor[] colors)
        {
            int colorIndex = -1;

            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == color)
                {
                    colorIndex = i;
                    break;
                }
            }

            if (colorIndex == -1)
            {
                colorIndex = Utilities.GetClosestColorIndex(color, colors);
            }

            return colorIndex;
        }

        public static int GetClosestColorIndex(Color color, RomColor[] colors)
        {
            float value = Utilities.GetColorHslValue(color);
            float diff = float.MaxValue;
            int match = -1;

            for (int i = 0; i < colors.Length; i++)
            {
                Color col = colors[i];
                float value2 = Utilities.GetColorHslValue(col);
                float diff2 = Math.Abs(value - value2);

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
            const float HueWeight = 0.8f;
            const float SaturationWeight = 0.1f;
            const float BrightnessWeight = 0.1f;

            return (color.GetHue() / 360.0f) * HueWeight +
                color.GetSaturation() * SaturationWeight +
                color.GetBrightness() * BrightnessWeight;
        }

        #endregion Color handling
    }
}
