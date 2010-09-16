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

namespace EpicEdit.Rom.Tracks.Objects
{
	/// <summary>
	/// A collection of <see cref="TrackObject"/> zones.
	/// A track object only appears in a track if it is located within its designated zone.
	/// </summary>
	public class TrackObjectZones
	{
		public bool ReadOnly { get; set; }
		private byte[] frontZones;
		private byte[] rearZones;

		public TrackObjectZones(byte[] data)
		{
			if (data.Length == 0)
			{
				// Koopa Beach 1, which has only a single, non-editable object zone
				data = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
				this.ReadOnly = true;
			}

			this.frontZones = new byte[4];
			this.frontZones[0] = data[0];
			this.frontZones[1] = Math.Max(this.frontZones[0], data[1]);
			this.frontZones[2] = Math.Max(this.frontZones[1], data[2]);
			this.frontZones[3] = Math.Max(this.frontZones[2], data[3]);

			this.rearZones = new byte[4];
			this.rearZones[0] = data[5];
			this.rearZones[1] = Math.Max(this.rearZones[0], data[6]);
			this.rearZones[2] = Math.Max(this.rearZones[1], data[7]);
			this.rearZones[3] = Math.Max(this.rearZones[2], data[8]);
		}

		public int GetZoneIndex(bool frontZonesView, int aiElementIndex)
		{
			return frontZonesView ?
				TrackObjectZones.GetZoneIndexSub(this.frontZones, aiElementIndex) :
				TrackObjectZones.GetZoneIndexSub(this.rearZones, aiElementIndex);
		}

		private static int GetZoneIndexSub(byte[] zones, int aiElementIndex)
		{
			for (int i = 0; i < zones.Length; i++)
			{
				if (aiElementIndex < zones[i])
				{
					return i;
				}
			}

			return 0;
		}

		public byte GetZoneValue(bool frontZonesView, int zoneIndex)
		{
			if (frontZonesView)
			{
				return this.frontZones[zoneIndex];
			}
			else
			{
				return this.rearZones[zoneIndex];
			}
		}

		public void SetZoneValue(bool frontZonesView, int zoneIndex, byte value)
		{
			if (frontZonesView)
			{
				this.frontZones[zoneIndex] = value;
			}
			else
			{
				this.rearZones[zoneIndex] = value;
			}
		}

		/// <summary>
		/// Returns the TrackObjectZones data as a byte array, in the format the SMK ROM expects.
		/// </summary>
		/// <returns>The TrackObjectZones bytes.</returns>
		public byte[] GetBytes()
		{
			byte[] data =
			{
				this.frontZones[0],
				this.frontZones[1],
				this.frontZones[2],
				this.frontZones[3],
				0xFF,
				this.rearZones[0],
				this.rearZones[1],
				this.rearZones[2],
				this.rearZones[3],
				0xFF
			};

			return data;
		}
	}
}
