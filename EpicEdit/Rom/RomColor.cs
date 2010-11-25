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
		public byte Red;

		/// <summary>
		/// Green channel (8 bit).
		/// </summary>
		public byte Green;

		/// <summary>
		/// Blue channel (8 bit).
		/// </summary>
		public byte Blue;

		/// <summary>
		/// Red channel (5 bit).
		/// </summary>
		public byte Red5Bit
		{
			get { return ConvertTo5BitColor(this.Red); }
		}

		/// <summary>
		/// Green channel (5 bit).
		/// </summary>
		public byte Green5Bit
		{
			get { return ConvertTo5BitColor(this.Green); }
		}

		/// <summary>
		/// Blue channel (5 bit).
		/// </summary>
		public byte Blue5Bit
		{
			get { return ConvertTo5BitColor(this.Blue); }
		}

		/// <summary>
		/// Gets the .NET Framework Color object representation of this color.
		/// </summary>
		/// <returns>Returns a Color object.</returns>
		public Color GetColor()
		{
			// Generate the .NET Framework Color object
			return Color.FromArgb(this.Red, this.Green, this.Blue);
		}

		/// <summary>
		/// Gets the SMK ROM encoded bytes for this color (5 bit precise).
		/// </summary>
		/// <returns>An array of 2 bytes.</returns>
		public byte[] GetBytes()
		{
			// Encode the red, green and blue components into the 2 bytes (5 bit)
			byte red = ConvertTo5BitColor(this.Red);
			byte green = ConvertTo5BitColor(this.Green);
			byte blue = ConvertTo5BitColor(this.Blue);

			byte[] bytes = new byte[2];
			bytes[0] = (byte)(red | ((green & 0x07) << 5));
			bytes[1] = (byte)(((green & 0x18) >> 3) | ((blue & 0x1F) << 2));
			return bytes;
		}

		/// <summary>
		/// Creates a new 5 bit precise RomColor object from the color of this object.
		/// </summary>
		/// <returns>The created RomColor object.</returns>
		public RomColor To5Bit()
		{
			return From5BitRgb(ConvertTo5BitColor(this.Red), ConvertTo5BitColor(this.Green), ConvertTo5BitColor(this.Blue));
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
			color.Red = (byte)red;
			color.Green = (byte)green;
			color.Blue = (byte)blue;
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
			rc.Red = color.R;
			rc.Green = color.G;
			rc.Blue = color.B;
			return rc;
		}

		/// <summary>
		/// Creates a RomColor object from 5 bit encoded byte array (2 bytes).
		/// </summary>
		/// <param name="bytes">The bytes to decode.</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor FromBytes(byte[] bytes)
		{
			return FromBytes(bytes, 0);
		}

		/// <summary>
		/// Creates a RomColor object from 5 bit encoded byte array (2 bytes).
		/// </summary>
		/// <param name="bytes">The bytes to decode.</param>
		/// <param name="index">The index at which to start the decoding.</param>
		/// <returns>The created RomColor object.</returns>
		public static RomColor FromBytes(byte[] bytes, int index)
		{
			if (bytes == null || bytes.Length - index < 2)
			{
				throw new Exception("Invalid color byte data");
			}

			if (index > bytes.Length - 2)
			{
				throw new Exception("Invalid position");
			}

			// Decode the bytes into red, green and blue components (8 bit)
			RomColor rc = new RomColor();
			byte lobyte = bytes[index];
			byte hibyte = bytes[index + 1];
			rc.Red = ConvertTo8BitColor(lobyte & 0x1F);
			rc.Green = ConvertTo8BitColor(((hibyte & 0x03) << 3) + ((lobyte & 0xE0) >> 5));
			rc.Blue = ConvertTo8BitColor((hibyte & 0x7C) >> 2);
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
			return (color1.Red == color2.Red) && (color1.Green == color2.Green) && (color1.Blue == color2.Blue);
		}

		/// <summary>
		/// Compares two RomColor objects.
		/// </summary>
		public static Boolean operator !=(RomColor color1, RomColor color2)
		{
			return (color1.Red != color2.Red) || (color1.Green != color2.Green) || (color1.Blue != color2.Blue);
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
			double stepR = ((double)target.Red - (double)this.Red) / (step - 1);
			double stepG = ((double)target.Green - (double)this.Green) / (step - 1);
			double stepB = ((double)target.Blue - (double)this.Blue) / (step - 1);
			for (double x = 0; x < step; x++)
			{
				yield return RomColor.From8BitRgb((int)(this.Red + (x * stepR)), (int)(this.Green + (x * stepG)), (int)(this.Blue + (x * stepB)));
			}
		}

		/// <summary>
		/// Returns the opposite color.
		/// </summary>
		/// <returns></returns>
		public RomColor Opposite()
		{
			return RomColor.From8BitRgb(255 - this.Red, 255 - this.Green, 255 - this.Blue);
		}

		/// <summary>
		/// Returns a nicely formatted string of the current color.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("R: {0} ({1})" + Environment.NewLine +
								 "G: {2} ({3})" + Environment.NewLine +
								 "B: {4} ({5})",
								 this.Red5Bit, this.Red,
								 this.Green5Bit, this.Green,
								 this.Blue5Bit, this.Blue);
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
