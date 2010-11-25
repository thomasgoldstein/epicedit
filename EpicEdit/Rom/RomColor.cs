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
	public struct RomColor
	{
		/// <summary>
		/// Red channel (8 bit).
		/// </summary>
		private byte red;

		/// <summary>
		/// Green channel (8 bit).
		/// </summary>
		private byte green;

		/// <summary>
		/// Blue channel (8 bit).
		/// </summary>
		private byte blue;

		/// <summary>
		/// Gets the red channel (8 bit).
		/// </summary>
		public byte Red
		{
			get { return this.red; }
		}

		/// <summary>
		/// Gets the green channel (8 bit).
		/// </summary>
		public byte Green
		{
			get { return this.green; }
		}

		/// <summary>
		/// Gets the blue channel (8 bit).
		/// </summary>
		public byte Blue
		{
			get { return this.blue; }
		}

		/// <summary>
		/// Red channel (5 bit).
		/// </summary>
		public byte Red5Bit
		{
			get { return ConvertTo5BitColor(this.red); }
		}

		/// <summary>
		/// Green channel (5 bit).
		/// </summary>
		public byte Green5Bit
		{
			get { return ConvertTo5BitColor(this.green); }
		}

		/// <summary>
		/// Blue channel (5 bit).
		/// </summary>
		public byte Blue5Bit
		{
			get { return ConvertTo5BitColor(this.blue); }
		}

		/// <summary>
		/// Gets the .NET Framework Color object representation of this color.
		/// </summary>
		/// <returns>Returns a Color object.</returns>
		public Color GetColor()
		{
			// Generate the .NET Framework Color object
			return Color.FromArgb(this.red, this.green, this.blue);
		}

		/// <summary>
		/// Gets the SMK ROM encoded bytes for this color (5 bit precise).
		/// </summary>
		/// <returns>An array of 2 bytes.</returns>
		public byte[] GetBytes()
		{
			// Encode the red, green and blue components into the 2 bytes (5 bit)
			byte red = ConvertTo5BitColor(this.red);
			byte green = ConvertTo5BitColor(this.green);
			byte blue = ConvertTo5BitColor(this.blue);

			byte[] data = new byte[2];
			data[0] = (byte)(red | ((green & 0x07) << 5));
			data[1] = (byte)(((green & 0x18) >> 3) | ((blue & 0x1F) << 2));
			return data;
		}

		/// <summary>
		/// Creates a new 5 bit precise RomColor object from the color of this object.
		/// </summary>
		/// <returns>The created RomColor object.</returns>
		public RomColor To5Bit()
		{
			return From5BitRgb(ConvertTo5BitColor(this.red), ConvertTo5BitColor(this.green), ConvertTo5BitColor(this.blue));
		}

		/// <summary>
		/// Creates a new 5 bit precise RomColor object from the rgb channel values.
		/// </summary>
		/// <param name="red">Red channel (0-31).</param>
		/// <param name="green">Green channel (0-31).</param>
		/// <param name="blue">Blue channel (0-31).</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor From5BitRgb(int red, int green, int blue)
		{
			return From8BitRgb(ConvertTo8BitColor(red), ConvertTo8BitColor(green), ConvertTo8BitColor(blue));
		}

		/// <summary>
		/// Creates a new 8 bit precise RomColor object from the rgb channel values.
		/// </summary>
		/// <param name="red">Red channel (0-255).</param>
		/// <param name="green">Green channel (0-255).</param>
		/// <param name="blue">Blue channel (0-255).</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor From8BitRgb(int red, int green, int blue)
		{
			RomColor color = new RomColor();
			color.red = (byte)red;
			color.green = (byte)green;
			color.blue = (byte)blue;
			return color;
		}

		/// <summary>
		/// Creates a RomColor object from a Color object.
		/// </summary>
		/// <param name="color">The Color object.</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor FromColor(Color color)
		{
			// Extract the bytes into red, green and blue components (8 bit).
			RomColor rc = new RomColor();
			rc.red = color.R;
			rc.green = color.G;
			rc.blue = color.B;
			return rc;
		}

		/// <summary>
		/// Creates a RomColor object from 5 bit encoded byte array (2 bytes).
		/// </summary>
		/// <param name="data">The bytes to decode.</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor FromBytes(byte[] data)
		{
			return FromBytes(data, 0);
		}

		/// <summary>
		/// Creates a RomColor object from 5 bit encoded byte array (2 bytes).
		/// </summary>
		/// <param name="data">The bytes to decode.</param>
		/// <param name="index">The index at which to start the decoding.</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor FromBytes(byte[] data, int index)
		{
			if (data == null || data.Length - index < 2)
			{
				throw new ArgumentException("Invalid color byte data");
			}

			if (index > data.Length - 2)
			{
				throw new ArgumentException("Invalid position");
			}

			// Decode the bytes into red, green and blue components (8 bit)
			RomColor rc = new RomColor();
			byte lobyte = data[index];
			byte hibyte = data[index + 1];
			rc.red = ConvertTo8BitColor(lobyte & 0x1F);
			rc.green = ConvertTo8BitColor(((hibyte & 0x03) << 3) + ((lobyte & 0xE0) >> 5));
			rc.blue = ConvertTo8BitColor((hibyte & 0x7C) >> 2);
			return rc;
		}

		/// <summary>
		/// Simply multiplying the 5 bit color from the ROM by 8 is not a totally correct conversion to an 8 bit color.
		/// A "white" in 5 bit color is 31,31,31 whereas in 8 bit it is 255,255,255. 31 * 8 = 248.
		/// This calculation corrects this so that 0 = 0 and 31 = 255.
		/// </summary>
		/// <param name="value5bit">The 5 bit int color value (0-31).</param>
		/// <returns>The color value in 8 bit precision.</returns>
		private static byte ConvertTo8BitColor(int value5bit)
		{
			return (byte)(value5bit * 8.25);
		}

		/// <summary>
		/// Takes care of finding nearest color.
		/// </summary>
		/// <param name="value8bit">The 8 bit int color value (0-255).</param>
		/// <returns>The color value in 5 bit precision.</returns>
		private static byte ConvertTo5BitColor(int value8bit)
		{
			return (byte)Math.Round((double)value8bit / (255d / 31d), 0, MidpointRounding.ToEven);
		}

		/// <summary>
		/// Implicitly convert to a RomColor object.
		/// </summary>
		public static implicit operator RomColor(Color color)
		{
			return FromColor(color);
		}

		/// <summary>
		/// Implicitly convert to a Color object.
		/// </summary>
		public static implicit operator Color(RomColor color)
		{
			return color.GetColor();
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
		/// This function is used by the color picker to generate.
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
				yield return RomColor.From8BitRgb((int)(this.red + (x * stepR)), (int)(this.green + (x * stepG)), (int)(this.blue + (x * stepB)));
			}
		}

		/// <summary>
		/// Returns the opposite color.
		/// </summary>
		/// <returns></returns>
		public RomColor Opposite()
		{
			return RomColor.From8BitRgb(255 - this.red, 255 - this.green, 255 - this.blue);
		}

		/// <summary>
		/// Returns a nicely formatted string of the current color.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture,
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
			else
			{
				return base.Equals(obj);
			}
		}
	}
}
