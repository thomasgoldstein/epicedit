﻿#region GPL statement
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

namespace EpicEdit.Rom
{
	internal class Utilities
	{
		#region Read Blocks

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
			Array.Copy(buffer, offset, bytes, 0, length);
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

			byte[] bytes = new byte[length];
			Array.Copy(buffer, offset, bytes, 0, length);

			return bytes;
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
				group[i] = new byte[groupSize];
				Array.Copy(buffer, offset + (i * groupSize), group[i], 0, groupSize);
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
				group[i] = new byte[groupSize];
				Array.Copy(buffer, offset + (i * groupSize), group[i], 0, groupSize);
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
		public static Offset[] ReadBlockOffset(byte[] buffer, int offset, int offsetCount)
		{
			int offsetSize = 3;
			Offset[] offsetGroup = new Offset[offsetCount];

			for (int i = 0; i < offsetCount; i++)
			{
				byte[] address = new byte[offsetSize];
				Array.Copy(buffer, offset + (i * offsetSize), address, 0, offsetSize);
				offsetGroup[i] = new Offset(address);
			}

			return offsetGroup;
		}

		#endregion Read Blocks

		#region Decrypt ROM Text

		/// <summary>
		/// Decrypts a single character of text data, for a Japanese game.
		/// This function assumes that the location of each character tile hasn't been changed (in the ROM font graphics), which may be wrong.
		/// </summary>
		/// <param name="hexChar">The value that defines the location of the character among the font graphics.</param>
		/// <returns>The corresponding character.</returns>
		public static char DecryptRomTextJapanese(byte hexChar)
		{
			switch (hexChar)
			{
				case 0x00: return '0';
				case 0x01: return '1';
				case 0x02: return '2';
				case 0x03: return '3';
				case 0x04: return '4';
				case 0x05: return '5';
				case 0x06: return '6';
				case 0x07: return '7';
				case 0x08: return '8';
				case 0x09: return '9';
				case 0x0a: return 'A';
				case 0x0b: return 'B';
				case 0x0c: return 'C';
				case 0x0d: return 'D';
				case 0x0e: return 'E';
				case 0x0f: return 'F';
				case 0x20: return 'G';
				case 0x21: return 'P';
				case 0x22: return 'V';
				case 0x23: return 'S';
				case 0x24: return '?';
				case 0x25: return '.';
				case 0x26: return ',';
				case 0x27: return '!';
				case 0x28: return '\'';
				case 0x29: return '"';
				case 0x2c: return ' ';
				case 0x2e: return '\u3099'; // Ten-ten
				case 0x2f: return '\u309A'; // Maru
				case 0x30: return 'あ';
				case 0x31: return 'い';
				case 0x32: return 'う';
				case 0x33: return 'え';
				case 0x34: return 'お';
				case 0x35: return 'か';
				case 0x36: return 'き';
				case 0x37: return 'く';
				case 0x38: return 'け';
				case 0x39: return 'こ';
				case 0x3a: return 'さ';
				case 0x3b: return 'し';
				case 0x3c: return 'す';
				case 0x3d: return 'せ';
				case 0x3e: return 'そ';
				case 0x3f: return 'た';
				case 0x40: return 'ち';
				case 0x41: return 'つ';
				case 0x42: return 'て';
				case 0x43: return 'と';
				case 0x44: return 'な';
				case 0x45: return 'に';
				case 0x46: return 'ぬ';
				case 0x47: return 'ね';
				case 0x48: return 'の';
				case 0x49: return 'は';
				case 0x4a: return 'ひ';
				case 0x4b: return 'ふ';
				case 0x4c: return 'へ';
				case 0x4d: return 'ほ';
				case 0x4e: return 'ま';
				case 0x4f: return 'み';
				case 0x50: return 'む';
				case 0x51: return 'め';
				case 0x52: return 'も';
				case 0x53: return 'や';
				case 0x54: return 'ゆ';
				case 0x55: return 'よ';
				case 0x56: return 'ら';
				case 0x57: return 'り';
				case 0x58: return 'る';
				case 0x59: return 'れ';
				case 0x5a: return 'ろ';
				case 0x5b: return 'わ';
				case 0x5c: return 'を';
				case 0x5d: return 'ん';
				case 0x5e: return '(';
				case 0x5f: return '+';
				case 0x60: return 'ア';
				case 0x61: return 'イ';
				case 0x62: return 'ウ';
				case 0x63: return 'エ';
				case 0x64: return 'オ';
				case 0x65: return 'カ';
				case 0x66: return 'キ';
				case 0x67: return 'ク';
				case 0x68: return 'ケ';
				case 0x69: return 'コ';
				case 0x6a: return 'サ';
				case 0x6b: return 'シ';
				case 0x6c: return 'ス';
				case 0x6d: return 'セ';
				case 0x6e: return 'ソ';
				case 0x6f: return 'タ';
				case 0x70: return 'チ';
				case 0x71: return 'ツ';
				case 0x72: return 'テ';
				case 0x73: return 'ト';
				case 0x74: return 'ナ';
				case 0x75: return 'ニ';
				case 0x76: return 'ヌ';
				case 0x77: return 'ネ';
				case 0x78: return 'ノ';
				case 0x79: return 'ハ';
				case 0x7a: return 'ヒ';
				case 0x7b: return 'フ';
				case 0x7c: return 'ヘ';
				case 0x7d: return 'ホ';
				case 0x7e: return 'マ';
				case 0x7f: return 'ミ';
				case 0x80: return 'ム';
				case 0x81: return 'メ';
				case 0x82: return 'モ';
				case 0x83: return 'ヤ';
				case 0x84: return 'ユ';
				case 0x85: return 'ヨ';
				case 0x86: return 'ラ';
				case 0x87: return 'リ';
				case 0x88: return 'ル';
				case 0x89: return 'レ';
				case 0x8a: return 'ロ';
				case 0x8b: return 'ワ';
				case 0x8c: return 'ェ';
				case 0x8d: return 'ン';
				case 0x8e: return '。';
				case 0x8f: return 'ヽ';
				case 0x91: return 'っ';
				case 0x94: return 'ょ';
				case 0x97: return 'ッ';
				case 0x98: return 'ャ';
				case 0x99: return 'ァ';
				case 0x9a: return 'ョ';
				case 0x9c: return 'ー';
				default: return '?';
			}
		}

		/// <summary>
		/// Decrypts a single character of text data, for an occidental game.
		/// This function assumes that the location of each character tile hasn't been changed (in the ROM font graphics), which may be wrong.
		/// </summary>
		/// <param name="hexChar">The value that defines the location of the character among the font graphics.</param>
		/// <returns>The corresponding character.</returns>
		public static char DecryptRomTextOccidental(byte hexChar)
		{
			switch (hexChar)
			{
				case 0x00: return '0';
				case 0x01: return '1';
				case 0x02: return '2';
				case 0x03: return '3';
				case 0x04: return '4';
				case 0x05: return '5';
				case 0x06: return '6';
				case 0x07: return '7';
				case 0x08: return '8';
				case 0x09: return '9';
				case 0x0a: return 'a';
				case 0x0b: return 'b';
				case 0x0c: return 'c';
				case 0x0d: return 'd';
				case 0x0e: return 'e';
				case 0x0f: return 'f';
				case 0x10: return 'g';
				case 0x11: return 'h';
				case 0x12: return 'i';
				case 0x13: return 'j';
				case 0x14: return 'k';
				case 0x15: return 'l';
				case 0x16: return 'm';
				case 0x17: return 'n';
				case 0x18: return 'o';
				case 0x19: return 'p';
				case 0x1a: return 'q';
				case 0x1b: return 'r';
				case 0x1c: return 's';
				case 0x1d: return 't';
				case 0x1e: return 'u';
				case 0x1f: return 'v';
				case 0x20: return 'w';
				case 0x21: return 'x';
				case 0x22: return 'y';
				case 0x23: return 'z';
				case 0x24: return '?';
				case 0x25: return '.';
				case 0x26: return ',';
				case 0x27: return '!';
				case 0x28: return '\'';
				case 0x29: return '"';
				case 0x2c: return ' ';
				case 0x2f: return ' ';
				default: return '?';
			}
		}

		/// <summary>
		/// Decrypts a string of text data.
		/// </summary>
		/// <param name="hexText">Values that define the location of each character of a text string among the font graphics.</param>
		/// <returns>The corresponding text string.</returns>
		public static string DecryptRomText(byte[] hexText, Regions region)
		{
			return DecryptRomText(hexText, 1, region);
		}

		/// <summary>
		/// Decrypts a string of text data, skipping every other byte.
		/// </summary>
		/// <param name="hexText">Values that define the location of each character of a text string among the font graphics.</param>
		/// <returns>The corresponding text string.</returns>
		public static string DecryptRomTextOdd(byte[] hexText, Regions region)
		{
			return DecryptRomText(hexText, 2, region);
		}

		public static string DecryptRomText(byte[] hexText, int step, Regions region)
		{
			char[] textArray = new char[hexText.Length / step];
			string text;

			if (region == Regions.Jap)
			{
				for (int i = 0; i < textArray.Length; i++)
				{
					textArray[i] = Utilities.DecryptRomTextJapanese(hexText[i * step]);
				}

				text = new string(textArray);

				// Japanese text formatting
				// (needed to connect the ten-ten and maru characters to the preceding character)
				text = text.Normalize(System.Text.NormalizationForm.FormC);
			}
			else
			{
				for (int i = 0; i < textArray.Length; i++)
				{
					textArray[i] = DecryptRomTextOccidental(hexText[i * step]);
				}

				text = new string(textArray);

				text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text);
			}

			return text;
		}

		#endregion Decrypt ROM Text

		public static byte[] HexStringToByteArray(string input)
		{
			byte[] bytes = new byte[(input.Length) / 2];
			Utilities.LoadByteArrayFromHexString(bytes, input);
			return bytes;
		}

		public static void LoadByteArrayFromHexString(byte[] bytes, string input)
		{
			for (int x = 0; x < bytes.Length; x++)
			{
				bytes[x] = Convert.ToByte(input.Substring(x * 2, 2), 16);
			}
		}

		public static string ByteArrayToHexString(byte[] data)
		{
			char[] lookup = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
			int i = 0, p = 0, l = data.Length;
			char[] c = new char[l * 2];
			byte d;
			while (i < l)
			{
				d = data[i++];
				c[p++] = lookup[d / 0x10];
				c[p++] = lookup[d % 0x10];
			}
			return new string(c, 0, c.Length);
		}
	}
}
