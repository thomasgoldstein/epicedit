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
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;

namespace EpicEdit.Rom.Tracks
{
	/// <summary>
	/// A Grand Prix track.
	/// </summary>
	public class GPTrack : Track
	{
		public StartPosition StartPosition { get; private set; }
		public LapLine LapLine { get; private set; }
		public TrackObjects Objects { get; private set; }
		public TrackObjectZones ObjectZones { get; private set; }

		public GPTrack(string name, Theme theme,
					   byte[] map, byte[] overlayTileData,
					   byte[] aiZoneData, byte[] aiTargetData,
					   byte[] startPositionData, byte[] lapLineData,
					   byte[] objectData, byte[] objectZoneData,
					   OverlayTileSizes overlayTileSizes,
					   OverlayTilePatterns overlayTilePatterns) :
			base(name, theme, map, overlayTileData, aiZoneData, aiTargetData, overlayTileSizes, overlayTilePatterns)
		{
			this.StartPosition = new StartPosition(startPositionData);
			this.LapLine = new LapLine(lapLineData);

			if (objectZoneData != null) // null for Ghost Valley tracks, by default
			{
				this.Objects = new TrackObjects(objectData);
				this.ObjectZones = new TrackObjectZones(objectZoneData);
			}
		}

		/// <summary>
		/// Loads the GPTrack-specific items from the MakeTrack object.
		/// </summary>
		protected override void LoadDataFrom(MakeTrack track)
		{
			base.LoadDataFrom(track);

			this.StartPosition = track.StartPosition;
			this.LapLine = track.LapLine;

			if (this.Objects != null) // If the track objects are not ghost pillars
			{
				this.Objects = track.Objects;

				if (!this.ObjectZones.ReadOnly)
				{
					this.ObjectZones = track.ObjectZones;
				}
			}
		}

		/// <summary>
		/// Loads the GPTrack-specific items to the MakeTrack object.
		/// </summary>
		protected override void LoadDataTo(MakeTrack track)
		{
			base.LoadDataTo(track);

			track.StartPosition = this.StartPosition;
			track.LapLine = this.LapLine;

			if (this.Objects != null) // If the track objects are not ghost pillars
			{
				track.Objects = this.Objects;

				if (!this.ObjectZones.ReadOnly)
				{
					track.ObjectZones = this.ObjectZones;
				}
			}
		}
	}
}
