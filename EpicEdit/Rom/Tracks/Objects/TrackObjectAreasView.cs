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
    internal class TrackObjectAreasView
    {
        /// <summary>
        /// The object area grid size (horizontally and vertically).
        /// </summary>
        public const int GridSize = TrackMap.Size / TrackAIElement.Precision;

        /// <summary>
        /// The maximum value (horizontally and vertically) within the object area grid.
        /// </summary>
        private const int GridLimit = GridSize - 1;

        public event EventHandler<EventArgs<int>> DataChanged;

        private readonly TrackAI ai;
        public TrackAI AI => this.ai;

        private readonly byte[] areas;

        public TrackObjectAreasView(byte[] data, TrackAI ai)
        {
            this.ai = ai;
            this.areas = new byte[4];
            this.SetBytes(data);
        }

        public void SetBytes(byte[] data)
        {
            for (int i = 0; i < this.areas.Length; i++)
            {
                this.SetAreaValue(i, data[i]);
            }
        }

        private byte GetAreaIndex(int aiElementIndex)
        {
            for (byte i = 0; i < this.areas.Length; i++)
            {
                if (aiElementIndex < this.areas[i])
                {
                    return i;
                }
            }

            return 0;
        }

        public byte GetAreaValue(int areaIndex)
        {
            return this.areas[areaIndex];
        }

        public void SetAreaValue(int areaIndex, byte value)
        {
            if (this.areas[areaIndex] == value)
            {
                return;
            }

            this.areas[areaIndex] = value;
            this.OnDataChanged(areaIndex);
        }

        private void OnDataChanged(int areaIndex)
        {
            this.DataChanged?.Invoke(this, new EventArgs<int>(areaIndex));
        }

        public byte[][] GetGrid()
        {
            byte[][] areas;

            if (this.ai.ElementCount == 0)
            {
                areas = new byte[GridSize][];

                for (int y = 0; y < areas.Length; y++)
                {
                    areas[y] = new byte[GridSize];
                }

                return areas;
            }

            sbyte[][] sAreas = TrackObjectAreasView.InitAreas();
            this.FillGridFromAI(sAreas);
            areas = TrackObjectAreasView.GetGridFilledFromNearestTiles(sAreas);

            return areas;
        }

        private static sbyte[][] InitAreas()
        {
            sbyte[][] areas = new sbyte[GridSize][];

            for (int y = 0; y < areas.Length; y++)
            {
                areas[y] = new sbyte[GridSize];
            }

            for (int x = 0; x < areas[0].Length; x++)
            {
                areas[0][x] = -1;
            }

            for (int y = 1; y < areas.Length; y++)
            {
                Buffer.BlockCopy(areas[0], 0, areas[y], 0, areas[y].Length);
            }

            return areas;
        }

        private void FillGridFromAI(sbyte[][] areas)
        {
            foreach (TrackAIElement aiElem in this.ai)
            {
                int aiElemIndex = this.ai.GetElementIndex(aiElem);
                sbyte areaIndex = (sbyte)this.GetAreaIndex(aiElemIndex);
                int left = Math.Min(aiElem.Area.X / TrackAIElement.Precision, GridSize);
                int top = Math.Min(aiElem.Area.Y / TrackAIElement.Precision, GridSize);
                int right = Math.Min(aiElem.Area.Right / TrackAIElement.Precision, GridSize);
                int bottom = Math.Min(aiElem.Area.Bottom / TrackAIElement.Precision, GridSize);

                switch (aiElem.AreaShape)
                {
                    case TrackAIElementShape.Rectangle:
                        for (int y = top; y < bottom; y++)
                        {
                            for (int x = left; x < right; x++)
                            {
                                areas[y][x] = areaIndex;
                            }
                        }
                        break;

                    case TrackAIElementShape.TriangleTopLeft:
                        for (int y = top; y < bottom; y++)
                        {
                            for (int x = left; x < right; x++)
                            {
                                areas[y][x] = areaIndex;
                            }
                            right--;
                        }
                        break;

                    case TrackAIElementShape.TriangleTopRight:
                        for (int y = top; y < bottom; y++)
                        {
                            for (int x = left; x < right; x++)
                            {
                                areas[y][x] = areaIndex;
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
                                areas[y][x] = areaIndex;
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
                                areas[y][x] = areaIndex;
                            }
                            right++;
                        }
                        break;
                }
            }
        }

        private static byte[][] GetGridFilledFromNearestTiles(sbyte[][] areas)
        {
            byte[][] newAreas = new byte[areas.Length][];

            for (int y = 0; y < areas.Length; y++)
            {
                newAreas[y] = new byte[areas[y].Length];
                for (int x = 0; x < areas[y].Length; x++)
                {
                    if (areas[y][x] != -1)
                    {
                        newAreas[y][x] = (byte)areas[y][x];
                        continue;
                    }

                    int depth = 1;
                    sbyte areaIndex = -1;
                    while (areaIndex == -1)
                    {
                        sbyte matchFound;

                        matchFound = TrackObjectAreasView.GetTopRightNearestTile(areas, x, y, depth);
                        if (matchFound > areaIndex)
                        {
                            areaIndex = matchFound;
                        }

                        matchFound = TrackObjectAreasView.GetBottomRightNearestTile(areas, x, y, depth);
                        if (matchFound > areaIndex)
                        {
                            areaIndex = matchFound;
                        }

                        matchFound = TrackObjectAreasView.GetBottomLeftNearestTile(areas, x, y, depth);
                        if (matchFound > areaIndex)
                        {
                            areaIndex = matchFound;
                        }

                        matchFound = TrackObjectAreasView.GetTopLeftNearestTile(areas, x, y, depth);
                        if (matchFound > areaIndex)
                        {
                            areaIndex = matchFound;
                        }

                        depth++;
                    }

                    newAreas[y][x] = (byte)areaIndex;
                }
            }

            return newAreas;
        }

        private static sbyte GetTopRightNearestTile(sbyte[][] areas, int x, int y, int depth)
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
                if (areas[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)areas[y2][x2];
                }

                x2++;
                y2++;
            }

            return matchFound;
        }

        private static sbyte GetBottomRightNearestTile(sbyte[][] areas, int x, int y, int depth)
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
                if (areas[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)areas[y2][x2];
                }

                x2--;
                y2++;
            }

            return matchFound;
        }

        private static sbyte GetBottomLeftNearestTile(sbyte[][] areas, int x, int y, int depth)
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
                if (areas[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)areas[y2][x2];
                }

                x2--;
                y2--;
            }

            return matchFound;
        }

        private static sbyte GetTopLeftNearestTile(sbyte[][] areas, int x, int y, int depth)
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
                if (areas[y2][x2] > matchFound)
                {
                    matchFound = (sbyte)areas[y2][x2];
                }

                x2++;
                y2--;
            }

            return matchFound;
        }

        /// <summary>
        /// Returns the <see cref="TrackObjectAreasView"/> data as a byte array, in the format the SMK ROM expects.
        /// </summary>
        /// <returns>The <see cref="TrackObjectAreasView"/> bytes.</returns>
        public byte[] GetBytes()
        {
            byte[] data =
            {
                this.areas[0],
                this.areas[1],
                this.areas[2],
                this.areas[3],
                0xFF
            };

            return data;
        }
    }
}
