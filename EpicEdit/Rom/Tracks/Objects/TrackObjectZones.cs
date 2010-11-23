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
		private GPTrack track;

		public bool ReadOnly { get; set; }
		private byte[] frontZones;
		private byte[] rearZones;

		public TrackObjectZones(byte[] data, GPTrack track)
		{
			this.track = track;

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

		private byte GetZoneIndex(bool frontZonesView, int aiElementIndex)
		{
			return frontZonesView ?
				TrackObjectZones.GetZoneIndexSub(this.frontZones, aiElementIndex) :
				TrackObjectZones.GetZoneIndexSub(this.rearZones, aiElementIndex);
		}

		private static byte GetZoneIndexSub(byte[] zones, int aiElementIndex)
		{
			for (byte i = 0; i < zones.Length; i++)
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

		public byte[][] GetGrid(bool frontZonesView)
		{
			byte[][] zones = new byte[64][];

			for (int y = 0; y < zones.Length; y++)
			{
				zones[y] = new byte[64];
				
				for (int x = 0; x < zones[y].Length; x++)
				{
					zones[y][x] = 0xFF;
				}
			}

			foreach (TrackAIElement aiElem in this.track.AI)
			{
				int aiElemIndex = this.track.AI.GetElementIndex(aiElem);
				byte zoneIndex = this.GetZoneIndex(frontZonesView, aiElemIndex);
				int left = aiElem.Zone.X / 2;
				int top = aiElem.Zone.Y / 2;
				int right = aiElem.Zone.Right / 2;
				int bottom = aiElem.Zone.Bottom / 2;

				switch (aiElem.ZoneShape)
				{
					case Shape.Rectangle:
						for (int y = top; y < bottom; y++)
						{
							for (int x = left; x < right; x++)
							{
								zones[y][x] = zoneIndex;
							}
						}
						break;

					case Shape.TriangleTopLeft:
						for (int y = top; y < bottom; y++)
						{
							for (int x = left; x < right; x++)
							{
								zones[y][x] = zoneIndex;
							}
							right--;
						}
						break;

					case Shape.TriangleTopRight:
						for (int y = top; y < bottom; y++)
						{
							for (int x = left; x < right; x++)
							{
								zones[y][x] = zoneIndex;
							}
							left++;
						}
						break;

					case Shape.TriangleBottomRight:
						left = right - 1;
						for (int y = top; y < bottom; y++)
						{
							for (int x = left; x < right; x++)
							{
								zones[y][x] = zoneIndex;
							}
							left--;
						}
						break;

					case Shape.TriangleBottomLeft:
						right = left + 1;
						for (int y = top; y < bottom; y++)
						{
							for (int x = left; x < right; x++)
							{
								zones[y][x] = zoneIndex;
							}
							right++;
						}
						break;
				}
			}

			return zones;
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
