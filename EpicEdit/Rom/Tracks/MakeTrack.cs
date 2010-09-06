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
using System.IO;

namespace EpicEdit.Rom
{
	/// <summary>
	/// Represents the data found in a MAKE exported track file.
	/// </summary>
	public class MakeTrack
	{
		/// <summary>
		/// Path to the imported file.
		/// </summary>
		public string FilePath { get; private set; }

		/// <summary>
		/// Start Position X.
		/// </summary>
		public byte[] SP_STX;

		/// <summary>
		/// Converted Start Position X.
		/// </summary>
		public short StartPositionX
		{
			get
			{
				if (BitConverter.IsLittleEndian)
				{
					return BitConverter.ToInt16(new byte[] { this.SP_STX[1], this.SP_STX[0] }, 0);
				}
				else
				{
					return BitConverter.ToInt16(this.SP_STX, 0);
				}
			}
		}

		/// <summary>
		/// Start Position Y.
		/// </summary>
		public byte[] SP_STY;

		/// <summary>
		/// Converted Start Position Y.
		/// </summary>
		public short StartPositionY
		{
			get
			{
				if (BitConverter.IsLittleEndian)
				{
					return BitConverter.ToInt16(new byte[] { this.SP_STY[1], this.SP_STY[0] }, 0);
				}
				else
				{
					return BitConverter.ToInt16(this.SP_STY, 0);
				}
			}
		}

		/// <summary>
		/// Start Position Width (2nd Row Offset).
		/// </summary>
		public byte[] SP_STW;

		/// <summary>
		/// Converted Start Position Width (2nd Row Offset).
		/// </summary>
		public short StartPositionW
		{
			get
			{
				if (BitConverter.IsLittleEndian)
				{
					return BitConverter.ToInt16(new byte[] { this.SP_STW[1], this.SP_STW[0] }, 0);
				}
				else
				{
					return BitConverter.ToInt16(this.SP_STW, 0);
				}
			}
		}

		/// <summary>
		/// Lap Line Area X.
		/// </summary>
		public byte[] SP_LSPX;

		/// <summary>
		/// Lap Line Area Y.
		/// </summary>
		public byte[] SP_LSPY;

		/// <summary>
		/// Lap Line Area Width.
		/// </summary>
		public byte[] SP_LSPW;

		/// <summary>
		/// Lap Line Area Height.
		/// </summary>
		public byte[] SP_LSPH;

		/// <summary>
		/// Lap Line Y.
		/// </summary>
		public byte[] SP_LSLY;

		/// <summary>
		/// Theme.
		/// </summary>
		public byte SP_REGION;

		public byte[] SP_OPN;

		/// <summary>
		/// Tile Map.
		/// </summary>
		public byte[] MAP;
		public byte[] MAPMASK;

		/// <summary>
		/// Overlay Tiles.
		/// </summary>
		public byte[] GPEX;

		/// <summary>
		/// AI.
		/// </summary>
		public byte[] AREA;

		/// <summary>
		/// Objects.
		/// </summary>
		public byte[] OBJ;

		/// <summary>
		/// Object View Zones.
		/// </summary>
		public byte[] AREA_BORDER;

		public MakeTrack(string filePath)
		{
			this.Import(filePath);
		}

		/// <summary>
		/// Converts the MAKE data into the AI target and zone data into the format Epic Edit expects.
		/// </summary>
		public void GetAIData(out byte[] targetData, out byte[] zoneData)
		{
			List<byte> targetDataList = new List<byte>();
			List<byte> zoneDataList = new List<byte>();

			int count = this.AREA.Length / 32;

			for (int x = 0; x < count; x++)
			{
				if (this.AREA[x * 32] != 0xFF)
				{
					// Reorder the target data bytes
					targetDataList.Add(this.AREA[x * 32 + 1]);
					targetDataList.Add(this.AREA[x * 32 + 2]);
					targetDataList.Add(this.AREA[x * 32]);

					byte zoneShape = this.AREA[x * 32 + 16];
					zoneDataList.Add(zoneShape);
					zoneDataList.Add(this.AREA[x * 32 + 17]);
					zoneDataList.Add(this.AREA[x * 32 + 18]);
					zoneDataList.Add(this.AREA[x * 32 + 19]);
					if (zoneShape == 0x00) // Rectangle, the fifth byte is not needed if the shape is not a rectangle
					{
						zoneDataList.Add(this.AREA[x * 32 + 20]);
					}
				}
			}

			targetData = targetDataList.ToArray();
			zoneData = zoneDataList.ToArray();
		}

		/// <summary>
		/// Converts the MAKE data into the lap line data Epic Edit expects.
		/// </summary>
		public byte[] GetLapLineData()
		{
			byte[] ret = new byte[6];
			ret[0] = this.SP_LSLY[1];
			ret[1] = this.SP_LSLY[0];
			ret[2] = this.SP_LSPX[1];
			ret[3] = this.SP_LSPY[1];
			ret[4] = this.SP_LSPW[1];
			ret[5] = this.SP_LSPH[1];
			return ret;
		}

		/// <summary>
		/// Epic Edit expects 44 bytes for the object data, this method removes anything over 44 bytes.
		/// </summary>
		public byte[] GetObjectData()
		{
			if (this.OBJ.Length > 44)
			{
				List<byte> ret = new List<byte>();
				ret.AddRange(this.OBJ);
				ret.RemoveRange(44, ret.Count - 44);
				return ret.ToArray();
			}
			else
			{
				return this.OBJ;
			}
		}

		/// <summary>
		/// Reads the MAKE file and sets the members with data.
		/// </summary>
		private void Import(string filePath)
		{
			this.FilePath = filePath;

			using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
			{
				using (TextReader reader = new StreamReader(fs))
				{

					String line = reader.ReadLine();
					while (line != null)
					{
						if (line.StartsWith("#SP_STX ", StringComparison.Ordinal))
						{
							this.SP_STX = GetLineData(line);
						}
						else if (line.StartsWith("#SP_STY ", StringComparison.Ordinal))
						{
							this.SP_STY = GetLineData(line);
						}
						else if (line.StartsWith("#SP_STW ", StringComparison.Ordinal))
						{
							this.SP_STW = GetLineData(line);
						}
						else if (line.StartsWith("#SP_LSPX ", StringComparison.Ordinal))
						{
							this.SP_LSPX = GetLineData(line);
						}
						else if (line.StartsWith("#SP_LSPY ", StringComparison.Ordinal))
						{
							this.SP_LSPY = GetLineData(line);
						}
						else if (line.StartsWith("#SP_LSPW ", StringComparison.Ordinal))
						{
							this.SP_LSPW = GetLineData(line);
						}
						else if (line.StartsWith("#SP_LSPH ", StringComparison.Ordinal))
						{
							this.SP_LSPH = GetLineData(line);
						}
						else if (line.StartsWith("#SP_LSLY ", StringComparison.Ordinal))
						{
							this.SP_LSLY = GetLineData(line);
						}
						else if (line.StartsWith("#SP_REGION ", StringComparison.Ordinal))
						{
							this.SP_REGION = GetLineData(line)[1];
						}
						else if (line.StartsWith("#SP_OPN ", StringComparison.Ordinal))
						{
							this.SP_OPN = GetLineData(line);
						}
						else if (line.Equals("#MAP", StringComparison.Ordinal))
						{
							this.MAP = GetBlockData(reader);
						}
						else if (line.Equals("#MAPMASK", StringComparison.Ordinal))
						{
							this.MAPMASK = GetBlockData(reader);
						}
						else if (line.Equals("#GPEX", StringComparison.Ordinal))
						{
							this.GPEX = GetBlockData(reader);
						}
						else if (line.Equals("#AREA", StringComparison.Ordinal))
						{
							this.AREA = GetBlockData(reader);
						}
						else if (line.Equals("#OBJ", StringComparison.Ordinal))
						{
							this.OBJ = GetBlockData(reader);
						}
						else if (line.Equals("#AREA_BORDER", StringComparison.Ordinal))
						{
							this.AREA_BORDER = GetBlockData(reader);
						}

						line = reader.ReadLine();
					}
				}
			}
		}

		#region Extract Data

		private byte[] GetLineData(String line)
		{
			int space = line.IndexOf(' ');
			if (space == -1)
			{
				throw new InvalidDataException("Unable to find space character");
			}

			line = line.Substring(space).Trim();
			if (line.Length % 2 != 0)
			{
				throw new InvalidDataException("Invalid data length");
			}

			return Utilities.HexStringToByteArray(line);
		}

		private byte[] GetBlockData(TextReader reader)
		{
			List<byte> ret = new List<byte>();

			String line = reader.ReadLine();
			while (line != null)
			{
				if (line.StartsWith("#"))
				{
					ret.AddRange(Utilities.HexStringToByteArray(line.Substring(1)));
				}
				else
				{
					break;
				}

				line = reader.ReadLine();
			}

			return ret.ToArray();
		}

		#endregion Extract Data
	}
}