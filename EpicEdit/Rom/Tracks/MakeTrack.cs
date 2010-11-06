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
using System.Collections.Generic;
using System.IO;

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;

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
		private string filePath;

		private Track track;
		private Game game;

		public TrackMap Map
		{
			get
			{
				return new TrackMap(this.MAP);
			}
		}

		public Theme Theme
		{
			get
			{
				return this.game.Themes[this.SP_REGION[1] >> 1];
			}
		}

		public OverlayTiles OverlayTiles
		{
			get
			{
				return new OverlayTiles(this.GPEX,
										this.game.OverlayTileSizes,
										this.game.OverlayTilePatterns);
			}
		}

		public StartPosition StartPosition
		{
			get
			{
				short x, y, w;
				if (BitConverter.IsLittleEndian)
				{
					x = BitConverter.ToInt16(new byte[] { this.SP_STX[1], this.SP_STX[0] }, 0);
					y = BitConverter.ToInt16(new byte[] { this.SP_STY[1], this.SP_STY[0] }, 0);
					w = BitConverter.ToInt16(new byte[] { this.SP_STW[1], this.SP_STW[0] }, 0);
				}
				else
				{
					x = BitConverter.ToInt16(this.SP_STX, 0);
					y = BitConverter.ToInt16(this.SP_STY, 0);
					w = BitConverter.ToInt16(this.SP_STW, 0);
				}

				return new StartPosition(x, y, w);
			}
		}

		public LapLine LapLine
		{
			get
			{
				byte[] data = new byte[]
				{
					this.SP_LSLY[1],
					this.SP_LSLY[0],
					this.SP_LSPX[1],
					this.SP_LSPY[1],
					this.SP_LSPW[1],
					this.SP_LSPH[1]
				};
				return new LapLine(data);
			}
		}

		public TrackObjects Objects
		{
			get
			{
				byte[] data = new byte[44];
				Array.Copy(this.OBJ, data, data.Length);
				return new TrackObjects(data);
			}
		}

		public TrackObjectZones ObjectZones
		{
			get
			{
				return new TrackObjectZones(this.AREA_BORDER);
			}
		}

		public TrackAI AI
		{
			get
			{
				byte[] targetData, zoneData;
				this.GetAIData(out targetData, out zoneData);
				return new TrackAI(zoneData, targetData, this.track);
			}
		}

		/// <summary>
		/// Start Position X.
		/// </summary>
		private byte[] SP_STX;

		/// <summary>
		/// Start Position Y.
		/// </summary>
		private byte[] SP_STY;

		/// <summary>
		/// Start Position Width (2nd Row Offset).
		/// </summary>
		private byte[] SP_STW;

		/// <summary>
		/// Lap Line Area X.
		/// </summary>
		private byte[] SP_LSPX;

		/// <summary>
		/// Lap Line Area Y.
		/// </summary>
		private byte[] SP_LSPY;

		/// <summary>
		/// Lap Line Area Width.
		/// </summary>
		private byte[] SP_LSPW;

		/// <summary>
		/// Lap Line Area Height.
		/// </summary>
		private byte[] SP_LSPH;

		/// <summary>
		/// Lap Line Y.
		/// </summary>
		private byte[] SP_LSLY;

		/// <summary>
		/// Theme.
		/// </summary>
		private byte[] SP_REGION;

		/*/// <summary>
		/// Object behavior.
		/// </summary>
		private byte[] SP_OPN;*/

		/// <summary>
		/// Tile Map.
		/// </summary>
		private byte[] MAP;

		//private byte[] MAPMASK;

		/// <summary>
		/// Overlay Tiles.
		/// </summary>
		private byte[] GPEX;

		/// <summary>
		/// AI.
		/// </summary>
		private byte[] AREA;

		/// <summary>
		/// Objects.
		/// </summary>
		private byte[] OBJ;

		/// <summary>
		/// Object View Zones.
		/// </summary>
		private byte[] AREA_BORDER;

		public MakeTrack(string filePath, Track track, Game game)
		{
			this.track = track;
			this.game = game;

			this.InitFields();
			this.Import(filePath);
		}

		/// <summary>
		/// Set fields to default values (in case the imported file or the loaded data is incomplete).
		/// </summary>
		private void InitFields()
		{
			this.SP_STX = new byte[2];
			this.SP_STY = new byte[2];
			this.SP_STW = new byte[2];
			this.SP_LSPX = new byte[2];
			this.SP_LSPY = new byte[2];
			this.SP_LSPW = new byte[2];
			this.SP_LSPH = new byte[2];
			this.SP_LSLY = new byte[2];
			this.SP_REGION = new byte[] { 0, 2 };

			this.MAP = new byte[16384];

			this.GPEX = new byte[128];
			for (int i = 0; i < this.GPEX.Length; i++)
			{
				this.GPEX[i] = 0xFF;
			}

			this.AREA = new byte[4064];
			for (int i = 0; i < this.AREA.Length; i++)
			{
				this.AREA[i] = 0xFF;
			}

			this.OBJ = new byte[64];

			this.AREA_BORDER = new byte[10];
			for (int i = 0; i < this.AREA_BORDER.Length; i++)
			{
				this.AREA_BORDER[i] = 0xFF;
			}
		}

		/// <summary>
		/// Converts the MAKE data into the AI target and zone data into the format Epic Edit expects.
		/// </summary>
		private void GetAIData(out byte[] targetData, out byte[] zoneData)
		{
			int lineLength = 32; // Byte count per line

			List<byte> targetDataList = new List<byte>();
			List<byte> zoneDataList = new List<byte>();

			int count = this.AREA.Length / lineLength;

			for (int x = 0; x < count; x++)
			{
				if (this.AREA[x * lineLength] != 0xFF)
				{
					// Reorder the target data bytes
					targetDataList.Add(this.AREA[x * lineLength + 1]);
					targetDataList.Add(this.AREA[x * lineLength + 2]);
					targetDataList.Add(this.AREA[x * lineLength]);

					byte zoneShape = this.AREA[x * lineLength + 16];
					zoneDataList.Add(zoneShape);
					zoneDataList.Add(this.AREA[x * lineLength + 17]);
					zoneDataList.Add(this.AREA[x * lineLength + 18]);
					zoneDataList.Add(this.AREA[x * lineLength + 19]);
					if (zoneShape == 0x00) // Rectangle, the fifth byte is not needed if the shape is not a rectangle
					{
						zoneDataList.Add(this.AREA[x * lineLength + 20]);
					}
				}
			}

			targetData = targetDataList.ToArray();
			zoneData = zoneDataList.ToArray();
		}

		/// <summary>
		/// Reads the MAKE file and sets the members with data.
		/// </summary>
		private void Import(string filePath)
		{
			this.filePath = filePath;

			using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
			using (TextReader reader = new StreamReader(fs))
			{
				string line = reader.ReadLine();
				while (line != null)
				{
					if (line.StartsWith("#SP_STX ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_STX, line);
					}
					else if (line.StartsWith("#SP_STY ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_STY, line);
					}
					else if (line.StartsWith("#SP_STW ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_STW, line);
					}
					else if (line.StartsWith("#SP_LSPX ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_LSPX, line);
					}
					else if (line.StartsWith("#SP_LSPY ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_LSPY, line);
					}
					else if (line.StartsWith("#SP_LSPW ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_LSPW, line);
					}
					else if (line.StartsWith("#SP_LSPH ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_LSPH, line);
					}
					else if (line.StartsWith("#SP_LSLY ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_LSLY, line);
					}
					else if (line.StartsWith("#SP_REGION ", StringComparison.Ordinal))
					{
						MakeTrack.LoadLineData(this.SP_REGION, line);
					}
					/*else if (line.StartsWith("#SP_OPN ", StringComparison.Ordinal))
					{
						MakeTrack.LoadData(this.SP_OPN, line);
					}*/
					else if (line.Equals("#MAP", StringComparison.Ordinal))
					{
						MakeTrack.LoadBlockData(this.MAP, reader);
					}
					/*else if (line.Equals("#MAPMASK", StringComparison.Ordinal))
					{
						MakeTrack.LoadBlockData(this.MAPMASK, reader);
					}*/
					else if (line.Equals("#GPEX", StringComparison.Ordinal))
					{
						MakeTrack.LoadBlockData(this.GPEX, reader);
					}
					else if (line.Equals("#AREA", StringComparison.Ordinal))
					{
						MakeTrack.LoadBlockData(this.AREA, reader);
					}
					else if (line.Equals("#OBJ", StringComparison.Ordinal))
					{
						MakeTrack.LoadBlockData(this.OBJ, reader);
					}
					else if (line.Equals("#AREA_BORDER", StringComparison.Ordinal))
					{
						MakeTrack.LoadBlockData(this.AREA_BORDER, reader);
					}

					line = reader.ReadLine();
				}
			}
		}

		#region Extract Data

		private static void LoadLineData(byte[] field, string line)
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

			Utilities.LoadByteArrayFromString(field, line);
		}

		private static void LoadBlockData(byte[] field, TextReader reader)
		{
			int index = 0;
			string line = reader.ReadLine();
			while (!string.IsNullOrEmpty(line) && line[0] == '#')
			{
				byte[] lineBytes = Utilities.HexStringToByteArray(line.Substring(1));
				Array.Copy(lineBytes, 0, field, index, lineBytes.Length);
				line = reader.ReadLine();
				index += lineBytes.Length;
			}
		}

		#endregion Extract Data
	}
}