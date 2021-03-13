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
        public new const int Count = GroupCount * CountPerGroup;

        /// <summary>
        /// Number of GP Groups (Cups).
        /// </summary>
        public new const int GroupCount = 4;

        /// <summary>
        /// Number of GP tracks per Group (Cup).
        /// </summary>
        public const int CountPerGroup = 5;

        private readonly GPStartPosition _startPosition;
        public GPStartPosition StartPosition
        {
            get => _startPosition;
            private set => _startPosition.SetBytes(value.GetBytes());
        }

        private readonly LapLine _lapLine;
        public LapLine LapLine
        {
            get => _lapLine;
            private set => _lapLine.SetBytes(value.GetBytes());
        }

        private readonly TrackObjects _objects;
        public TrackObjects Objects
        {
            get => _objects;
            private set
            {
                _objects.SetBytes(value.GetBytes());
                _objects.Areas.SetBytes(value.Areas.GetBytes());
                _objects.Properties.SetBytes(value.Properties.GetBytes());
            }
        }

        private int _itemProbabilityIndex;
        public int ItemProbabilityIndex
        {
            get => _itemProbabilityIndex;
            set
            {
                if (_itemProbabilityIndex == value)
                {
                    return;
                }

                _itemProbabilityIndex = value;
                MarkAsModified(PropertyNames.GPTrack.ItemProbabilityIndex);
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
            _startPosition = new GPStartPosition(startPositionData);
            StartPosition.PropertyChanged += StartPosition_PropertyChanged;

            _lapLine = new LapLine(lapLineData);
            LapLine.DataChanged += LapLine_DataChanged;

            _objects = new TrackObjects(objectData, objectAreaData, AI, objectPropData, this);
            Objects.PropertyChanged += Objects_PropertyChanged;

            _itemProbabilityIndex = itemProbaIndex;
        }

        private void StartPosition_PropertyChanged(object sender, EventArgs e)
        {
            MarkAsModified(PropertyNames.GPTrack.StartPosition);
        }

        private void LapLine_DataChanged(object sender, EventArgs e)
        {
            MarkAsModified(PropertyNames.GPTrack.LapLine);
        }

        private void Objects_PropertyChanged(object sender, EventArgs e)
        {
            MarkAsModified(PropertyNames.GPTrack.Objects);
        }

        /// <summary>
        /// Loads the GPTrack-specific items from the MakeTrack object.
        /// </summary>
        protected override void LoadDataFrom(MakeTrack track)
        {
            base.LoadDataFrom(track);

            StartPosition = track.StartPosition;
            LapLine = track.LapLine;
            Objects = track.Objects;
            ItemProbabilityIndex = track.ItemProbabilityIndex;
        }

        /// <summary>
        /// Loads the GPTrack-specific items to the MakeTrack object.
        /// </summary>
        protected override void LoadDataTo(MakeTrack track)
        {
            base.LoadDataTo(track);

            track.StartPosition = StartPosition;
            track.LapLine = LapLine;
            track.Objects = Objects;
            track.ItemProbabilityIndex = ItemProbabilityIndex;
        }
    }
}
