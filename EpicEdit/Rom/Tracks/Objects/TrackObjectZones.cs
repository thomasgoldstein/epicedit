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
    /// A collection of <see cref="TrackObject"/> zones.
    /// A track object only appears in a track if it is located within its designated zone.
    /// </summary>
    internal class TrackObjectZones : INotifyPropertyChanged
    {
        public const int Size = 10;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly TrackObjectZonesView frontView;
        public TrackObjectZonesView FrontView => this.frontView;

        private readonly TrackObjectZonesView rearView;
        public TrackObjectZonesView RearView => this.rearView;

        public TrackObjectZones(byte[] data, TrackAI ai)
        {
            this.frontView = new TrackObjectZonesView(new[] { data[0], data[1], data[2], data[3] }, ai);
            this.rearView = new TrackObjectZonesView(new[] { data[5], data[6], data[7], data[8] }, ai);

            this.frontView.DataChanged += this.frontView_DataChanged;
            this.rearView.DataChanged += this.rearView_DataChanged;
        }

        public void SetBytes(byte[] data)
        {
            this.frontView.SetBytes(new[] { data[0], data[1], data[2], data[3] });
            this.rearView.SetBytes(new[] { data[5], data[6], data[7], data[8] });
        }

        private void frontView_DataChanged(object sender, EventArgs<int> e)
        {
            this.OnPropertyChanged(PropertyNames.TrackObjectZones.FrontView);
        }

        private void rearView_DataChanged(object sender, EventArgs<int> e)
        {
            this.OnPropertyChanged(PropertyNames.TrackObjectZones.RearView);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Returns the TrackObjectZones data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The TrackObjectZones bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data = new byte[Size];
            byte[] frontData = this.frontView.GetBytes();
            byte[] rearData = this.rearView.GetBytes();

            Buffer.BlockCopy(frontData, 0, data, 0, frontData.Length);
            Buffer.BlockCopy(rearData, 0, data, frontData.Length, rearData.Length);

            return data;
        }
    }
}
