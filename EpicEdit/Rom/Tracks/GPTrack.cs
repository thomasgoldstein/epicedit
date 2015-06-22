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
using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Start;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// A Grand Prix track.
    /// </summary>
    internal class GPTrack : Track
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

        private readonly GPStartPosition startPosition;
        public GPStartPosition StartPosition
        {
            get { return this.startPosition; }
            private set { this.startPosition.SetBytes(value.GetBytes()); }
        }

        private readonly LapLine lapLine;
        public LapLine LapLine
        {
            get { return this.lapLine; }
            private set { this.lapLine.SetBytes(value.GetBytes()); }
        }

        private readonly TrackObjects objects;
        public TrackObjects Objects
        {
            get { return this.objects; }
            private set
            {
                this.objects.SetBytes(value.GetBytes());
                this.objects.Zones.SetBytes(value.Zones.GetBytes());
                this.objects.Properties.SetBytes(value.Properties.GetBytes());
            }
        }

        private int itemProbabilityIndex;
        public int ItemProbabilityIndex
        {
            get { return this.itemProbabilityIndex; }
            set
            {
                if (this.itemProbabilityIndex == value)
                {
                    return;
                }

                this.itemProbabilityIndex = value;
                this.MarkAsModified("ItemProbabilityIndex");
            }
        }

        public GPTrack(SuffixedTextItem nameItem, Theme theme,
                       byte[] map, byte[] overlayTileData,
                       byte[] aiZoneData, byte[] aiTargetData,
                       byte[] startPositionData, byte[] lapLineData,
                       byte[] objectData, byte[] objectZoneData, byte[] objectPropData,
                       OverlayTileSizes overlayTileSizes,
                       OverlayTilePatterns overlayTilePatterns,
                       int itemProbaIndex) :
            base(nameItem, theme, map, overlayTileData, aiZoneData, aiTargetData, overlayTileSizes, overlayTilePatterns)
        {
            this.startPosition = new GPStartPosition(startPositionData);
            this.StartPosition.PropertyChanged += this.StartPosition_PropertyChanged;

            this.lapLine = new LapLine(lapLineData);
            this.LapLine.DataChanged += this.LapLine_DataChanged;

            this.objects = new TrackObjects(objectData, objectZoneData, this.AI, objectPropData, theme.Palettes);
            this.Objects.DataChanged += this.Objects_DataChanged;

            this.itemProbabilityIndex = itemProbaIndex;
        }

        private void StartPosition_PropertyChanged(object sender, EventArgs e)
        {
            this.MarkAsModified("StartPosition");
        }

        private void LapLine_DataChanged(object sender, EventArgs e)
        {
            this.MarkAsModified("LapLine");
        }

        private void Objects_DataChanged(object sender, EventArgs e)
        {
            this.MarkAsModified("Objects");
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
            this.ItemProbabilityIndex = track.ItemProbabilityIndex;
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
            track.ItemProbabilityIndex = this.ItemProbabilityIndex;
        }
    }
}
