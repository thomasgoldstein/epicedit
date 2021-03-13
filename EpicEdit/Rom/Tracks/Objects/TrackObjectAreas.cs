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

using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Utility;
using System;
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks.Objects
{
    /// <summary>
    /// A collection of <see cref="TrackObject"/> areas.
    /// A track object only appears in a track if it is located within its designated area.
    /// </summary>
    internal class TrackObjectAreas : INotifyPropertyChanged
    {
        public const int Size = 10;

        public event PropertyChangedEventHandler PropertyChanged;
        public TrackObjectAreasView FrontView { get; }
        public TrackObjectAreasView RearView { get; }

        public TrackObjectAreas(byte[] data, TrackAI ai)
        {
            FrontView = new TrackObjectAreasView(new[] { data[0], data[1], data[2], data[3] }, ai);
            RearView = new TrackObjectAreasView(new[] { data[5], data[6], data[7], data[8] }, ai);

            FrontView.DataChanged += frontView_DataChanged;
            RearView.DataChanged += rearView_DataChanged;
        }

        public void SetBytes(byte[] data)
        {
            FrontView.SetBytes(new[] { data[0], data[1], data[2], data[3] });
            RearView.SetBytes(new[] { data[5], data[6], data[7], data[8] });
        }

        private void frontView_DataChanged(object sender, EventArgs<int> e)
        {
            OnPropertyChanged(PropertyNames.TrackObjectAreas.FrontView);
        }

        private void rearView_DataChanged(object sender, EventArgs<int> e)
        {
            OnPropertyChanged(PropertyNames.TrackObjectAreas.RearView);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns the <see cref="TrackObjectAreas"/> data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The <see cref="TrackObjectAreas"/> bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[Size];
            byte[] frontData = FrontView.GetBytes();
            byte[] rearData = RearView.GetBytes();

            Buffer.BlockCopy(frontData, 0, data, 0, frontData.Length);
            Buffer.BlockCopy(rearData, 0, data, frontData.Length, rearData.Length);

            return data;
        }
    }
}
