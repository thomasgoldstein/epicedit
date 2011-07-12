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
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// A Grand Prix track.
    /// </summary>
    public class GPTrack : Track
    {
        /// <summary>
        /// Number of GP tracks.
        /// </summary>
        public new const int Count = GPTrack.GroupCount * GPTrack.CountPerGroup;

        /// <summary>
        /// Number of GP Groups (Cups).
        /// </summary>
        public new const int GroupCount = 4;

        /// <summary>
        /// Number of GP tracks per Group (Cup).
        /// </summary>
        public const int CountPerGroup = 5;

        public GPStartPosition StartPosition { get; private set; }
        public LapLine LapLine { get; private set; }
        public TrackObjects Objects { get; private set; }
        public TrackObjectZones ObjectZones { get; private set; }

        public ObjectType ObjectTileset { get; set; }
        public ObjectType ObjectInteraction { get; set; }
        public ObjectType ObjectRoutine { get; set; }
        public int ObjectPaletteIndex { get; set; }
        public Palette ObjectPalette
        {
            get
            {
                return this.Theme.Palettes[this.ObjectPaletteIndex];
            }
        }

        public ObjectLoading ObjectLoading
        {
            get
            {
                switch (this.ObjectRoutine)
                {
                    case ObjectType.Pipe:
                    case ObjectType.Thwomp:
                    case ObjectType.Mole:
                    case ObjectType.Plant:
                    case ObjectType.RThwomp:
                        return ObjectLoading.Regular;

                    case ObjectType.Fish:
                        return ObjectLoading.Fish;

                    case ObjectType.Pillar:
                        return ObjectLoading.Pillar;

                    default:
                        return ObjectLoading.None;
                }
            }
        }

        public GPTrack(string name, Theme theme,
                       byte[] map, byte[] overlayTileData,
                       byte[] aiZoneData, byte[] aiTargetData,
                       byte[] startPositionData, byte[] lapLineData,
                       byte[] objectData, byte[] objectZoneData,
                       OverlayTileSizes overlayTileSizes,
                       OverlayTilePatterns overlayTilePatterns) :
            base(name, theme, map, overlayTileData, aiZoneData, aiTargetData, overlayTileSizes, overlayTilePatterns)
        {
            this.StartPosition = new GPStartPosition(startPositionData);
            this.LapLine = new LapLine(lapLineData);
            this.Objects = new TrackObjects(objectData);
            this.ObjectZones = new TrackObjectZones(objectZoneData, this);
        }

        /// <summary>
        /// Loads the GPTrack-specific items from the MakeTrack object.
        /// </summary>
        protected override void LoadDataFrom(MakeTrack track)
        {
            base.LoadDataFrom(track);

            this.StartPosition = track.StartPosition;
            this.LapLine = track.LapLine;
            this.Objects = track.Objects;
            this.ObjectZones = track.ObjectZones;
            this.ObjectTileset = track.ObjectTileset;
            this.ObjectInteraction = track.ObjectInteraction;
            this.ObjectRoutine = track.ObjectRoutine;
        }

        /// <summary>
        /// Loads the GPTrack-specific items to the MakeTrack object.
        /// </summary>
        protected override void LoadDataTo(MakeTrack track)
        {
            base.LoadDataTo(track);

            track.StartPosition = this.StartPosition;
            track.LapLine = this.LapLine;
            track.Objects = this.Objects;
            track.ObjectZones = this.ObjectZones;
            track.ObjectTileset = this.ObjectTileset;
            track.ObjectInteraction = this.ObjectInteraction;
            track.ObjectRoutine = this.ObjectRoutine;
        }
    }
}
