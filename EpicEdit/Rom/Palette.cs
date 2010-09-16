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

namespace EpicEdit.Rom
{
	/// <summary>
	/// Represents a palette composed of 16 colors.
	/// </summary>
	public class Palette
	{
		private Color[] colors = new Color[16];

		public Palette(Color[] palette)
		{
			if (palette.Length != 16)
			{
				throw new ArgumentException("The palette doesn't contain 16 colors.", "palette");
			}

			this.colors = palette;
		}

		public Palette(byte[] palette)
		{
			if (palette.Length != 32)
			{
				throw new ArgumentException("The palette is not 32-byte long.", "palette");
			}

			for (int i = 0; i < 16; i++)
			{
				byte lobyte = palette[i * 2];
				byte hibyte = palette[i * 2 + 1];

				int R = (lobyte & 0x1F) << 3;
				int G = (((hibyte & 0x03) << 3) + ((lobyte & 0xE0) >> 5)) << 3;
				int B = (hibyte & 0x7C) << 1;

				this.colors[i] = Color.FromArgb(R, G, B);
			}
		}

		public Color this[int index]
		{
			get { return this.colors[index]; }
			set { this.colors[index] = value; }
		}
	}
}
