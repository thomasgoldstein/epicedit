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
        private byte red;

        /// <summary>
        /// Green channel (8-bit).
        /// </summary>
        private byte green;

        /// <summary>
        /// Blue channel (8-bit).
        /// </summary>
        private byte blue;

        /// <summary>
        /// Gets the red channel (8-bit).
        /// </summary>
        public byte Red
        {
            get => this.red;
            set => this.red = value;
        }

        /// <summary>
        /// Gets the green channel (8-bit).
        /// </summary>
        public byte Green
        {
            get => this.green;
            set => this.green = value;
        }

        /// <summary>
        /// Gets the blue channel (8-bit).
        /// </summary>
        public byte Blue
        {
            get => this.blue;
            set => this.blue = value;
        }

        /// <summary>
        /// Gets the red channel (5-bit).
        /// </summary>
        public byte Red5Bit => RomColor.ConvertTo5BitColor(this.red);

        /// <summary>
        /// Gets the green channel (5-bit).
        /// </summary>
        public byte Green5Bit => RomColor.ConvertTo5BitColor(this.green);

        /// <summary>
        /// Gets the blue channel (5-bit).
        /// </summary>
        public byte Blue5Bit => RomColor.ConvertTo5BitColor(this.blue);

        /// <summary>
        /// Gets the .NET Framework Color object representation of this color.
        /// </summary>
        public Color Color => Color.FromArgb(this.red, this.green, this.blue);

        /// <summary>
        /// Gets the SMK ROM encoded bytes for this color (5-bit precise).
        /// </summary>
        /// <returns>An array of 2 bytes.</returns>
        public byte[] GetBytes()
        {
            // Encode the red, green and blue components into 2 bytes (5 bits)
            byte red = ConvertTo5BitColor(this.red);
            byte green = ConvertTo5BitColor(this.green);
            byte blue = ConvertTo5BitColor(this.blue);

            byte[] data = new byte[2];
            data[0] = (byte)(red | ((green & 0x07) << 5));
            data[1] = (byte)(((green & 0x18) >> 3) | ((blue & 0x1F) << 2));
            return data;
        }

        /// <summary>
        /// Creates a new 5-bit precise RomColor object from the color of this object.
        /// </summary>
        /// <returns>The created RomColor object.</returns>
        public RomColor To5Bit()
        {
            return From5BitRgb(ConvertTo5BitColor(this.red), ConvertTo5BitColor(this.green), ConvertTo5BitColor(this.blue));
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
            RomColor color = new RomColor();
            color.red = red;
            color.green = green;
            color.blue = blue;
            return color;
        }

        /// <summary>
        /// Creates a RomColor object from a Color object.
        /// </summary>
        /// <param name="color">The Color object.</param>
        /// <returns>The created RomColor object.</returns>
        public static RomColor FromColor(Color color)
        {
            // Extract the bytes into red, green and blue components (8 bits).
            RomColor rc = new RomColor();
            rc.red = color.R;
            rc.green = color.G;
            rc.blue = color.B;
            return rc;
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
            RomColor rc = new RomColor();
            byte lobyte = data[index];
            byte hibyte = data[index + 1];
            rc.red = ConvertTo8BitColor((byte)(lobyte & 0x1F));
            rc.green = ConvertTo8BitColor((byte)(((hibyte & 0x03) << 3) + ((lobyte & 0xE0) >> 5)));
            rc.blue = ConvertTo8BitColor((byte)((hibyte & 0x7C) >> 2));
            return rc;
        }

        /// <summary>
        /// Converts a 5-bit color channel value to 8 bit.
        /// </summary>
        /// <param name="value5bit">The 5-bit int color value (0-31).</param>
        /// <returns>The color value in 8-bit precision.</returns>
        public static byte ConvertTo8BitColor(byte value5bit)
        {
            // Simply multiplying the 5-bit color from the ROM by 8 is not a totally correct conversion to an 8-bit color.
            // A "white" in 5-bit color is 31,31,31 whereas in 8-bit it is 255,255,255. 31 * 8 = 248.
            // This calculation corrects this so that 0 = 0 and 31 = 255.
            return (byte)(value5bit * 8.25);
        }

        /// <summary>
        /// Converts an 8-bit color channel value to 5 bit.
        /// </summary>
        /// <param name="value8bit">The 8-bit int color value (0-255).</param>
        /// <returns>The color value in 5-bit precision.</returns>
        public static byte ConvertTo5BitColor(byte value8bit)
        {
            return (byte)Math.Round(value8bit / 8.25);
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
            return (color1.red == color2.red) && (color1.green == color2.green) && (color1.blue == color2.blue);
        }

        /// <summary>
        /// Compares two RomColor objects.
        /// </summary>
        public static Boolean operator !=(RomColor color1, RomColor color2)
        {
            return (color1.red != color2.red) || (color1.green != color2.green) || (color1.blue != color2.blue);
        }

        /// <summary>
        /// Enumerates colors between the current RomColor object and the specified one in a specific number of steps.
        /// </summary>
        /// <param name="target">The target color.</param>
        /// <param name="step">The number of steps.</param>
        /// <returns>A RomColor object representing one of the steps.</returns>
        public IEnumerator<RomColor> GetEnumerator(RomColor target, int step)
        {
            double stepR = ((double)target.red - (double)this.red) / (step - 1);
            double stepG = ((double)target.green - (double)this.green) / (step - 1);
            double stepB = ((double)target.blue - (double)this.blue) / (step - 1);
            for (double x = 0; x < step; x++)
            {
                yield return RomColor.From8BitRgb((byte)(this.red + (x * stepR)), (byte)(this.green + (x * stepG)), (byte)(this.blue + (x * stepB)));
            }
        }

        /// <summary>
        /// Returns the opposite color.
        /// </summary>
        /// <returns></returns>
        public RomColor Opposite()
        {
            return RomColor.From8BitRgb((byte)(255 - this.red), (byte)(255 - this.green), (byte)(255 - this.blue));
        }

        /// <summary>
        /// Returns a nicely formatted string of the current color.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture,
                                 "R: {0} ({1})" + Environment.NewLine +
                                 "G: {2} ({3})" + Environment.NewLine +
                                 "B: {4} ({5})",
                                 this.Red5Bit, this.red,
                                 this.Green5Bit, this.green,
                                 this.Blue5Bit, this.blue);
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
