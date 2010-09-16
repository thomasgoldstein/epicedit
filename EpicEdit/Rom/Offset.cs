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

namespace EpicEdit.Rom
{
	/// <summary>
	/// Represents a ROM address.
	/// </summary>
	internal struct Offset
	{
		private int value;

		public int Value
		{
			get { return this.value; }
		}

		public static implicit operator int(Offset offset)
		{
			return offset.value;
		}

		/// <summary>
		/// Returns the Offset data as a byte array, in the format the SMK ROM expects.
		/// </summary>
		/// <returns>The Offset bytes.</returns>
		public byte[] GetBytes()
		{
			byte[] data =
			{
				(byte)(this.value & 0xFF),
				(byte)((this.value & 0xFF00) >> 8),
				(byte)(0xC0 + ((this.value & 0xF0000) >> 16))
			};
			return data;
		}

		public Offset(byte[] data)
		{
			this.value = ((data[2] & 0xF) << 16) + (data[1] << 8) + data[0];
		}

		public Offset(int offset)
		{
			if (offset > 0xFFFFF)
			{
				throw new ArgumentOutOfRangeException("offset", "The offset value is too high.");
			}

			this.value = offset;
		}
	}
}
