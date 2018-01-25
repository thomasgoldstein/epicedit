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
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using System;

namespace EpicEdit.Rom.Tracks.Objects
{
    internal class TrackObjectZonesView
    {
        /// <summary>
        /// The object zone grid size (horizontally and vertically).
        /// </summary>
        public const int GridSize = TrackMap.Size / TrackAIElement.Precision;

        /// <summary>
        /// The maximum value (horizontally and vertically) within the object zone grid.
        /// </summary>
        private const int GridLimit = GridSize - 1;

        public event EventHandler<EventArgs<int>> DataChanged;

        private readonly TrackAI ai;
        public TrackAI AI => this.ai;

        private readonly byte[] zones;

        public TrackObjectZonesView(byte[] data, TrackAI ai)
        {
            this.ai = ai;
            this.zones = new byte[4];
            this.SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            for (int i = 0; i < this.zones.Length; i++)
            {
                this.SetZoneValue(i, data[i]);
            }
        }

        private byte GetZoneIndex(int aiElementIndex)
        {
            for (byte i = 0; i < this.zones.Length; i++)
            {
                if (aiElementIndex < this.zones[i])
                {
                    return i;
                }
            }

            return 0;
        }

        public byte GetZoneValue(int zoneIndex)
        {
            return this.zones[zoneIndex];
        }

        public void SetZoneValue(int zoneIndex, byte value)
        {
            if (this.zones[zoneIndex] == value)
            {
                return;
            }

            this.zones[zoneIndex] = value;
            this.OnDataChanged(zoneIndex);
        }

        private void OnDataChanged(int zoneIndex)
        {
            this.DataChanged?.Invoke(this, new EventArgs<int>(zoneIndex));
        }

        public byte[][] GetGrid()
        {
            byte[][] zones;

            if (this.ai.ElementCount == 0)
            {
                zones = new byte[GridSize][];

                for (int y = 0; y < zones.Length; y++)
                {
                    zones[y] = new byte[GridSize];
                }

                return zones;
            }

            sbyte[][] sZones = TrackObjectZonesView.InitZones();
            this.FillGridFromAI(sZones);
            zones = TrackObjectZonesView.GetGridFilledFromNearestTiles(sZones);

            return zones;
        }

        private static sbyte[][] InitZones()
        {
            sbyte[][] zones = new sbyte[GridSize][];

            for (int y = 0; y < zones.Length; y++)
            {
                zones[y] = new sbyte[GridSize];
            }

            for (int x = 0; x < zones[0].Length; x++)
            {
                zones[0][x] = -1;
            }

            for (int y = 1; y < zones.Length; y++)
            {
                Buffer.BlockCopy(zones[0], 0, zones[y], 0, zones[y].Length);
            }

            return zones;
        }

        private void FillGridFromAI(sbyte[][] zones)
        {
            foreach (TrackAIElement aiElem in this.ai)
            {
                int aiElemIndex = this.ai.GetElementIndex(aiElem);
                sbyte zoneIndex = (sbyte)this.GetZoneIndex(aiElemIndex);
                int left = Math.Min(aiElem.Zone.X / TrackAIElement.Precision, GridSize);
                int top = Math.Min(aiElem.Zone.Y / TrackAIElement.Precision, GridSize);
                int right = Math.Min(aiElem.Zone.Right / TrackAIElement.Precision, GridSize);
                int bottom = Math.Min(aiElem.Zone.Bottom / TrackAIElement.Precision, GridSize);

                switch (aiElem.ZoneShape)
                {
                    case TrackAIElementShape.Rectangle:
                        for (int y = top; y < bottom; y++)
                        {
                            for (int x = left; x < right; x++)
                            {
                                zones[y][x] = zoneIndex;
                            }
                        }
                        break;

                    case TrackAIElementShape.TriangleTopLeft:
                        for (int y = top; y < bottom; y++)
                        {
                            for (int x = left; x < right; x++)
                            {
                                zones[y][x] = zoneIndex;
                            }
                            right--;
                        }
                        break;

                    case TrackAIElementShape.TriangleTopRight:
                        for (int y = top; y < bottom; y++)
                        {
                            for (int x = left; x < right; x++)
                            {
                                zones[y][x] = zoneIndex;
                            }
                            left++;
                        }
                        break;

                    case TrackAIElementShape.TriangleBottomRight:
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

                    case TrackAIElementShape.TriangleBottomLeft:
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
        }

        private static byte[][] GetGridFilledFromNearestTiles(sbyte[][] zones)
        {
            byte[][] newZones = new byte[zones.Length][];

            for (int y = 0; y < zones.Length; y++)
            {
                newZones[y] = new byte[zones[y].Length];
                for (int x = 0; x < zones[y].Length; x++)
                {
                    if (zones[y][x] != -1)
                    {
                        newZones[y][x] = (byte)zones[y][x];
                        continue;
                    }

                    int depth = 1;
                    sbyte zoneIndex = -1;
                    while (zoneIndex == -1)
                    {
                        sbyte matchFound;

                        matchFound = TrackObjectZonesView.GetTopRightNearestTile(zones, x, y, depth);
                        if (matchFound > zoneIndex)
                        {
                            zoneIndex = matchFound;
                        }

                        matchFound = TrackObjectZonesView.GetBottomRightNearestTile(zones, x, y, depth);
                        if (matchFound > zoneIndex)
                        {
                            zoneIndex = matchFound;
                        }

                        matchFound = TrackObjectZonesView.GetBottomLeftNearestTile(zones, x, y, depth);
                        if (matchFound > zoneIndex)
                        {
                            zoneIndex = matchFound;
                        }

                        matchFound = TrackObjectZonesView.GetTopLeftNearestTile(zones, x, y, depth);
                        if (matchFound > zoneIndex)
                        {
                            zoneIndex = matchFound;
                        }

                        depth++;
                    }

                    newZones[y][x] = (byte)zoneIndex;
                }
            }

            return newZones;
        }

        private static sbyte GetTopRightNearestTile(sbyte[][] zones, int x, int y, int depth)
        {
            sbyte matchFound = -1;

            int x2 = x;
            int y2 = y - depth;

            if (y2 < 0)
            {
                x2 -= y2;
                y2 = 0;
            }

            while (x2 <= GridLimit && y2 <= y)
            {
                if (zones[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)zones[y2][x2];
                }

                x2++;
                y2++;
            }

            return matchFound;
        }

        private static sbyte GetBottomRightNearestTile(sbyte[][] zones, int x, int y, int depth)
        {
            sbyte matchFound = -1;

            int x2 = x + depth;
            int y2 = y;

            if (x2 > GridLimit)
            {
                y2 += x2 - GridLimit;
                x2 = GridLimit;
            }

            while (x2 >= x && y2 <= GridLimit)
            {
                if (zones[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)zones[y2][x2];
                }

                x2--;
                y2++;
            }

            return matchFound;
        }

        private static sbyte GetBottomLeftNearestTile(sbyte[][] zones, int x, int y, int depth)
        {
            sbyte matchFound = -1;

            int x2 = x;
            int y2 = y + depth;

            if (y2 > GridLimit)
            {
                x2 -= y2 - GridLimit;
                y2 = GridLimit;
            }

            while (x2 >= 0 && y2 >= y)
            {
                if (zones[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)zones[y2][x2];
                }

                x2--;
                y2--;
            }

            return matchFound;
        }

        private static sbyte GetTopLeftNearestTile(sbyte[][] zones, int x, int y, int depth)
        {
            sbyte matchFound = -1;

            int x2 = x - depth;
            int y2 = y;

            if (x2 < 0)
            {
                y2 += x2;
                x2 = 0;
            }

            while (x2 <= x && y2 >= 0)
            {
                if (zones[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)zones[y2][x2];
                }

                x2++;
                y2--;
            }

            return matchFound;
        }

        /// <summary>
        /// Returns the TrackObjectZonesView data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The TrackObjectZonesView bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data =
            {
                this.zones[0],
                this.zones[1],
                this.zones[2],
                this.zones[3],
                0xFF
            };

            return data;
        }
    }
}
