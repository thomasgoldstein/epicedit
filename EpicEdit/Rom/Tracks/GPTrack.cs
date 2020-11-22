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

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Start;
using System;

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
            get => this.startPosition;
            private set => this.startPosition.SetBytes(value.GetBytes());
        }

        private readonly LapLine lapLine;
        public LapLine LapLine
        {
            get => this.lapLine;
            private set => this.lapLine.SetBytes(value.GetBytes());
        }

        private readonly TrackObjects objects;
        public TrackObjects Objects
        {
            get => this.objects;
            private set
            {
                this.objects.SetBytes(value.GetBytes());
                this.objects.Areas.SetBytes(value.Areas.GetBytes());
                this.objects.Properties.SetBytes(value.Properties.GetBytes());
            }
        }

        private int itemProbabilityIndex;
        public int ItemProbabilityIndex
        {
            get => this.itemProbabilityIndex;
            set
            {
                if (this.itemProbabilityIndex == value)
                {
                    return;
                }

                this.itemProbabilityIndex = value;
                this.MarkAsModified(PropertyNames.GPTrack.ItemProbabilityIndex);
            }
        }

        public GPTrack(SuffixedTextItem nameItem, Theme theme,
                       byte[] map, byte[] overlayTileData,
                       byte[] aiAreaData, byte[] aiTargetData,
                       byte[] startPositionData, byte[] lapLineData,
                       byte[] objectData, byte[] objectAreaData, byte[] objectPropData,
                       OverlayTileSizes overlayTileSizes,
                       OverlayTilePatterns overlayTilePatterns,
                       int itemProbaIndex) :
            base(nameItem, theme, map, overlayTileData, aiAreaData, aiTargetData, overlayTileSizes, overlayTilePatterns)
        {
            this.startPosition = new GPStartPosition(startPositionData);
            this.StartPosition.PropertyChanged += this.StartPosition_PropertyChanged;

            this.lapLine = new LapLine(lapLineData);
            this.LapLine.DataChanged += this.LapLine_DataChanged;

            this.objects = new TrackObjects(objectData, objectAreaData, this.AI, objectPropData, this);
            this.Objects.PropertyChanged += this.Objects_PropertyChanged;

            this.itemProbabilityIndex = itemProbaIndex;
        }

        private void StartPosition_PropertyChanged(object sender, EventArgs e)
        {
            this.MarkAsModified(PropertyNames.GPTrack.StartPosition);
        }

        private void LapLine_DataChanged(object sender, EventArgs e)
        {
            this.MarkAsModified(PropertyNames.GPTrack.LapLine);
        }

        private void Objects_PropertyChanged(object sender, EventArgs e)
        {
            this.MarkAsModified(PropertyNames.GPTrack.Objects);
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
