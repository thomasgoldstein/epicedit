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
using System.Globalization;
using System.IO;

using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Overlay;

namespace EpicEdit.Rom.Tracks
{
	public enum ResizeHandle
	{
		None,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		BottomLeft,
		Left
	}

	/// <summary>
	/// Represents the common base between a <see cref="GPTrack"/> and a <see cref="BattleTrack"/>.
	/// </summary>
	public abstract class Track
	{
		public string Name { get; private set; }
		public Theme Theme { get; set; }
		public TrackMap Map { get; private set; }
		public OverlayTiles OverlayTiles { get; private set; }
		public TrackAI AI { get; private set; }
		public bool Modified { get; set; }

		protected Track(string name, Theme theme,
						byte[] map, byte[] overlayTilesData,
						byte[] aiZoneData, byte[] aiTargetData,
						OverlayTileSizes overlayTileSizes,
						OverlayTilePatterns overlayTilePatterns)
		{
			this.Name = name;
			this.Map = new TrackMap(map);
			this.Theme = theme;
			this.AI = new TrackAI(aiZoneData, aiTargetData, this);
			this.OverlayTiles = new OverlayTiles(overlayTilesData, overlayTileSizes, overlayTilePatterns);
		}

		public Tile[] GetRoadTileset()
		{
			return this.Theme.GetRoadTileset();
		}

		public Tile GetRoadTile(int index)
		{
			return this.Theme.GetRoadTile(index);
		}

		public Tile[] GetBackgroundTileset()
		{
			return this.Theme.GetBackgroundTileset();
		}

		public Tile GetBackgroundTile(int index)
		{
			return this.Theme.GetBackgroundTile(index);
		}

		public void Import(string filePath, Themes themes, OverlayTileSizes overlayTileSizes, OverlayTilePatterns overlayTilePatterns)
		{
			string ext = Path.GetExtension(filePath).ToLower(CultureInfo.InvariantCulture);

			switch (ext)
			{
				case ".mkt":
					this.ImportMkt(filePath, themes);
					break;

				default:
				case ".smkc":
					MakeTrack make = new MakeTrack(filePath);
					this.ImportSmkc(make, themes, overlayTileSizes, overlayTilePatterns);
					break;
			}
		}

		/// <summary>
		/// Imports an MKT (Track Designer) track.
		/// </summary>
		private void ImportMkt(string filePath, Themes themes)
		{
			using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
			{
				FileInfo info = new FileInfo(filePath);
				int fileLength = (int)info.Length;

				if (fileLength != 16384 && fileLength != 16385)
				{
					throw new InvalidDataException("File \"" + Path.GetFileName(filePath) + "\"" + Environment.NewLine +
												   "isn't a valid track file and couldn't be imported!");
				}

				byte[] mapData = new byte[16384];
				reader.Read(mapData, 0, 16384);

				this.Map = new TrackMap(mapData);

				if (fileLength == 16385) // If a theme is defined
				{
					byte themeId = (byte)(reader.ReadByte() / 2);
					this.Theme = themes[themeId];
				}
			}
		}

		/// <summary>
		/// Imports an SMKC (MAKE) track.
		/// </summary>
		protected virtual void ImportSmkc(MakeTrack track, Themes themes, OverlayTileSizes overlayTileSizes, OverlayTilePatterns overlayTilePatterns)
		{
			if (track.MAP.Length != 16384 || track.GPEX.Length != 128)
			{
				throw new InvalidDataException("File \"" + Path.GetFileName(track.FilePath) + "\"" + Environment.NewLine +
											   "isn't a valid track file and couldn't be imported!");
			}

			this.Map = new TrackMap(track.MAP);
			this.Theme = themes[track.SP_REGION >> 1];

			this.OverlayTiles = new OverlayTiles(track.GPEX, overlayTileSizes, overlayTilePatterns);

			byte[] targetData, zoneData;
			track.GetAIData(out targetData, out zoneData);
			this.AI = new TrackAI(zoneData, targetData, this);
		}

		public void Export(string fileName, byte themeId)
		{
			using (BinaryWriter bw = new BinaryWriter(new FileStream(fileName, FileMode.Create, FileAccess.Write)))
			{
				bw.Write(this.Map.GetBytes());

				themeId = (byte)(themeId << 1);
				bw.Write(themeId);
			}
		}
	}
}