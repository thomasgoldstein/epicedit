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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represent a color originating from the SMK ROM.
    /// </summary>
    internal struct RomColor
    {
        /// <summary>
        /// The number of bytes used by a color.
        /// </summary>
        public const int Size = 2;

        /// <summary>
        /// Red channel (8-bit).
        /// </summary>
        private byte _red;

        /// <summary>
        /// Green channel (8-bit).
        /// </summary>
        private byte _green;

        /// <summary>
        /// Blue channel (8-bit).
        /// </summary>
        private byte _blue;

        /// <summary>
        /// Gets the red channel (8-bit).
        /// </summary>
        public byte Red
        {
            get => _red;
            set => _red = value;
        }

        /// <summary>
        /// Gets the green channel (8-bit).
        /// </summary>
        public byte Green
        {
            get => _green;
            set => _green = value;
        }

        /// <summary>
        /// Gets the blue channel (8-bit).
        /// </summary>
        public byte Blue
        {
            get => _blue;
            set => _blue = value;
        }

        /// <summary>
        /// Gets the red channel (5-bit).
        /// </summary>
        public byte Red5Bit => ConvertTo5BitColor(_red);

        /// <summary>
        /// Gets the green channel (5-bit).
        /// </summary>
        public byte Green5Bit => ConvertTo5BitColor(_green);

        /// <summary>
        /// Gets the blue channel (5-bit).
        /// </summary>
        public byte Blue5Bit => ConvertTo5BitColor(_blue);

        /// <summary>
        /// Gets the .NET Framework Color object representation of this color.
        /// </summary>
        public Color Color => Color.FromArgb(_red, _green, _blue);

        /// <summary>
        /// Gets the SMK ROM encoded bytes for this color (5-bit precise).
        /// </summary>
        /// <returns>An array of 2 bytes.</returns>
        public byte[] GetBytes()
        {
            // Encode the red, green and blue components into 2 bytes (5 bits)
            byte red = ConvertTo5BitColor(_red);
            byte green = ConvertTo5BitColor(_green);
            byte blue = ConvertTo5BitColor(_blue);

            return new[]
            {
                (byte)(red | ((green & 0x07) << 5)),
                (byte)(((green & 0x18) >> 3) | ((blue & 0x1F) << 2))
            };
        }

        /// <summary>
        /// Creates a new 5-bit precise RomColor object from the color of this object.
        /// </summary>
        /// <returns>The created RomColor object.</returns>
        public RomColor To5Bit()
        {
            return From5BitRgb(ConvertTo5BitColor(_red), ConvertTo5BitColor(_green), ConvertTo5BitColor(_blue));
        }

        /// <summary>
        /// Creates a new 5-bit precise RomColor object from the RGB channel values.
        /// </summary>
        /// <param name="red">Red channel (0-31).</param>
        /// <param name="green">Green channel (0-31).</param>
        /// <param name="blue">Blue channel (0-31).</param>
        /// <returns>The created RomColor object.</returns>
        public static RomColor From5BitRgb(byte red, byte green, byte blue)
        {
            return From8BitRgb(ConvertTo8BitColor(red), ConvertTo8BitColor(green), ConvertTo8BitColor(blue));
        }

        /// <summary>
        /// Creates a new 8-bit precise RomColor object from the RGB channel values.
        /// </summary>
        /// <param name="red">Red channel (0-255).</param>
        /// <param name="green">Green channel (0-255).</param>
        /// <param name="blue">Blue channel (0-255).</param>
        /// <returns>The created RomColor object.</returns>
        public static RomColor From8BitRgb(byte red, byte green, byte blue)
        {
            return new RomColor
            {
                _red = red,
                _green = green,
                _blue = blue
            };
        }

        /// <summary>
        /// Creates a RomColor object from a Color object.
        /// </summary>
        /// <param name="color">The Color object.</param>
        /// <returns>The created RomColor object.</returns>
        public static RomColor FromColor(Color color)
        {
            // Extract the bytes into red, green and blue components (8 bits).
            return new RomColor
            {
                _red = color.R,
                _green = color.G,
                _blue = color.B
            };
        }

        /// <summary>
        /// Creates a RomColor object from 5-bit encoded byte array (2 bytes).
        /// </summary>
        /// <param name="data">The bytes to decode.</param>
        /// <returns>The created RomColor object.</returns>
        public static RomColor FromBytes(byte[] data)
        {
            return FromBytes(data, 0);
        }

        /// <summary>
        /// Creates a RomColor object from 5-bit encoded byte array (2 bytes).
        /// </summary>
        /// <param name="data">The bytes to decode.</param>
        /// <param name="index">The index at which to start the decoding.</param>
        /// <returns>The created RomColor object.</returns>
        public static RomColor FromBytes(byte[] data, int index)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Color byte data cannot be null.");
            }

            if (data.Length - index < 2)
            {
                throw new ArgumentException("Invalid color byte data or index value.", nameof(data));
            }

            // Decode the bytes into red, green and blue components (8 bits)
            byte lobyte = data[index];
            byte hibyte = data[index + 1];
            return new RomColor
            {
                _red = ConvertTo8BitColor((byte)(lobyte & 0x1F)),
                _green = ConvertTo8BitColor((byte)(((hibyte & 0x03) << 3) + ((lobyte & 0xE0) >> 5))),
                _blue = ConvertTo8BitColor((byte)((hibyte & 0x7C) >> 2))
            };
        }

        /// <summary>
        /// Converts a 5-bit color channel value to 8 bit.
        /// </summary>
        /// <param name="value5Bit">The 5-bit int color value (0-31).</param>
        /// <returns>The color value in 8-bit precision.</returns>
        public static byte ConvertTo8BitColor(byte value5Bit)
        {
            // Simply multiplying the 5-bit color from the ROM by 8 is not a totally correct conversion to an 8-bit color.
            // A "white" in 5-bit color is 31,31,31 whereas in 8-bit it is 255,255,255. 31 * 8 = 248.
            // This calculation corrects this so that 0 = 0 and 31 = 255.
            return (byte)(value5Bit * 8.25);
        }

        /// <summary>
        /// Converts an 8-bit color channel value to 5 bit.
        /// </summary>
        /// <param name="value8Bit">The 8-bit int color value (0-255).</param>
        /// <returns>The color value in 5-bit precision.</returns>
        public static byte ConvertTo5BitColor(byte value8Bit)
        {
            return (byte)Math.Round(value8Bit / 8.25);
        }

        /// <summary>
        /// Implicitly converts to a RomColor object.
        /// </summary>
        public static implicit operator RomColor(Color color)
        {
            return FromColor(color);
        }

        /// <summary>
        /// Implicitly converts to a Color object.
        /// </summary>
        public static implicit operator Color(RomColor color)
        {
            return color.Color;
        }

        /// <summary>
        /// Compares two RomColor objects.
        /// </summary>
        public static Boolean operator ==(RomColor color1, RomColor color2)
        {
            return (color1._red == color2._red) && (color1._green == color2._green) && (color1._blue == color2._blue);
        }

        /// <summary>
        /// Compares two RomColor objects.
        /// </summary>
        public static Boolean operator !=(RomColor color1, RomColor color2)
        {
            return (color1._red != color2._red) || (color1._green != color2._green) || (color1._blue != color2._blue);
        }

        /// <summary>
        /// Enumerates colors between the current RomColor object and the specified one in a specific number of steps.
        /// </summary>
        /// <param name="target">The target color.</param>
        /// <param name="step">The number of steps.</param>
        /// <returns>A RomColor object representing one of the steps.</returns>
        public IEnumerator<RomColor> GetEnumerator(RomColor target, int step)
        {
            double stepR = ((double)target._red - (double)_red) / (step - 1);
            double stepG = ((double)target._green - (double)_green) / (step - 1);
            double stepB = ((double)target._blue - (double)_blue) / (step - 1);
            for (double x = 0; x < step; x++)
            {
                yield return From8BitRgb((byte)(_red + (x * stepR)), (byte)(_green + (x * stepG)), (byte)(_blue + (x * stepB)));
            }
        }

        /// <summary>
        /// Returns the opposite color.
        /// </summary>
        /// <returns></returns>
        public RomColor Opposite()
        {
            return From8BitRgb((byte)(255 - _red), (byte)(255 - _green), (byte)(255 - _blue));
        }

        /// <summary>
        /// Returns a nicely formatted string of the current color.
        /// </summary>
        /// <returns>The string representation of the RomColor.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "R: {0} ({1})" + Environment.NewLine +
                                 "G: {2} ({3})" + Environment.NewLine +
                                 "B: {4} ({5})",
                                 Red5Bit, _red,
                                 Green5Bit, _green,
                                 Blue5Bit, _blue);
        }

        /// <summary>
        /// Returns a hex-formatted string of the current color.
        /// </summary>
        /// <returns>The hex value representing the color.</returns>
        public string ToHexString()
        {
            return $"{string.Format("{0:X2}", _red)}{string.Format("{0:X2}", _green)}{string.Format("{0:X2}", _blue)}";
        }

        /// <summary>
        /// Returns a RomColor from a hex color.
        /// </summary>
        /// <param name="hex">8-bit RGB hex string from "000000" to "FFFFFF".</param>
        /// <returns>The RomColor corresponding to the hex string.</returns>
        public static RomColor FromHex(string hex)
        {
            if (hex.Length != 6)
            {
                hex = Convert.ToInt32(hex, 16).ToString("X6", CultureInfo.InvariantCulture);
            }
            var bytes = Utilities.HexStringToBytes(hex);
            return From8BitRgb(bytes[0], bytes[1], bytes[2]);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is RomColor)
            {
                RomColor color = (RomColor)obj;
                return color == this;
            }

            return base.Equals(obj);
        }
    }
}
