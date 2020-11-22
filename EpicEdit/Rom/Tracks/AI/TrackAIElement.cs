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

using EpicEdit.Rom.Tracks.Road;
using System;
using System.ComponentModel;
using System.Drawing;

namespace EpicEdit.Rom.Tracks.AI
{
    /// <summary>
    /// Represents an element of the <see cref="TrackAI"/>.
    /// </summary>
    internal class TrackAIElement : INotifyPropertyChanged
    {
        /// <summary>
        /// The precision for AI elements: 2 tiles (16 pixels).
        /// </summary>
        public const int Precision = 2;

        public event PropertyChangedEventHandler PropertyChanged;

        private TrackAIElementShape areaShape;
        /// <summary>
        /// Gets or sets the area shape.
        /// </summary>
        public TrackAIElementShape AreaShape
        {
            get => this.areaShape;
            set
            {
                if (this.areaShape == value)
                {
                    return;
                }

                if (this.areaShape == TrackAIElementShape.Rectangle &&
                    value != TrackAIElementShape.Rectangle)
                {
                    if (this.area.Width > this.area.Height)
                    {
                        this.area.Width = this.area.Height;
                    }
                    else if (this.area.Height > this.area.Width)
                    {
                        this.area.Height = this.area.Width;
                    }
                }

                this.areaShape = value;
                this.OnPropertyChanged(PropertyNames.TrackAIElement.AreaShape);
            }
        }

        private Rectangle area;
        /// <summary>
        /// Gets the area.
        /// </summary>
        public Rectangle Area => this.area;

        private Point target;
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public Point Target
        {
            get => this.target;
            set => this.MoveTargetTo(value.X, value.Y);
        }

        private byte speed;
        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public byte Speed
        {
            get => this.speed;
            set
            {
                if (this.speed == value)
                {
                    return;
                }

                this.speed = value;
                this.OnPropertyChanged(PropertyNames.TrackAIElement.Speed);
            }
        }

        private bool isIntersection;
        /// <summary>
        /// Gets or sets a value that determines if the element is at an intersection.
        /// When an AI element is flagged as an intersection, this tells the AI to ignore
        /// the intersected AI area, and avoids track object display issues when switching areas.
        /// </summary>
        public bool IsIntersection
        {
            get => this.isIntersection;
            set
            {
                if (this.isIntersection == value)
                {
                    return;
                }

                this.isIntersection = value;
                this.OnPropertyChanged(PropertyNames.TrackAIElement.IsIntersection);
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Point Location
        {
            get => this.area.Location;
            set => this.MoveTo(value.X, value.Y);
        }

        /// <summary>
        /// Initializes a TrackAIElement.
        /// </summary>
        /// <param name="areaData">The byte array to get the area data from.</param>
        /// <param name="areaDataIndex">The index to use in the area byte array.</param>
        /// <param name="targetData">The byte array to get the target data from.</param>
        /// <param name="targetDataIndex">The index to use in the target byte array.</param>
        public TrackAIElement(byte[] areaData, ref int areaDataIndex, byte[] targetData, ref int targetDataIndex)
        {
            this.AreaShape = (TrackAIElementShape)areaData[areaDataIndex++];
            int areaX = areaData[areaDataIndex++] * Precision;
            int areaY = areaData[areaDataIndex++] * Precision;

            if (this.AreaShape == TrackAIElementShape.Rectangle)
            {
                int areaWidth = areaData[areaDataIndex++] * Precision;
                int areaHeight = areaData[areaDataIndex++] * Precision;

                this.area = new Rectangle(areaX, areaY, areaWidth, areaHeight);
            }
            else
            {
                int areaSize = areaData[areaDataIndex++] * Precision;

                // In the ROM, the X and Y values of a triangle
                // determine the location of its right angle.
                // We don't follow this logic, and change X and Y
                // to make them always determine the top left corner.
                switch (this.AreaShape)
                {
                    case TrackAIElementShape.TriangleTopRight:
                        areaX -= areaSize - Precision;
                        break;

                    case TrackAIElementShape.TriangleBottomRight:
                        areaX -= areaSize - Precision;
                        areaY -= areaSize - Precision;
                        break;

                    case TrackAIElementShape.TriangleBottomLeft:
                        areaY -= areaSize - Precision;
                        break;
                }

                this.area = new Rectangle(areaX, areaY, areaSize, areaSize);
            }

            this.target = new Point(targetData[targetDataIndex++], targetData[targetDataIndex++]);
            byte speedAndIntersection = targetData[targetDataIndex++];
            this.speed = (byte)(speedAndIntersection & 0x03);
            this.isIntersection = (speedAndIntersection & 0x80) == 0x80;
        }

        public TrackAIElement(Point position)
        {
            const int Size = 16;
            // Halve precision, so that areas are positioned following a 2-tile (16-px) step
            int areaX = ((position.X - (Size / 2)) / Precision) * Precision;
            int areaY = ((position.Y - (Size / 2)) / Precision) * Precision;

            // Ensure the element isn't out of the track bounds
            areaX = areaX < 0 ? 0 :
                (areaX + Size) > TrackMap.Size ? TrackMap.Size - Size :
                areaX;

            areaY = areaY < 0 ? 0 :
                (areaY + Size) > TrackMap.Size ? TrackMap.Size - Size :
                areaY;

            Rectangle area = new Rectangle(areaX, areaY, Size, Size);

            this.area = area;

            int x = area.X + area.Width / Precision;
            int y = area.Y + area.Height / Precision;
            this.target = new Point(x, y);
            this.speed = 0;
        }

        private TrackAIElement() { }

        public bool IntersectsWith(Point point)
        {
            if (this.AreaShape == TrackAIElementShape.Rectangle)
            {
                return this.IntersectsWithRectangle(point);
            }

            return this.IntersectsWithTriangle(point);
        }

        private bool IntersectsWithRectangle(Point point)
        {
            return
                point.X >= this.area.Left &&
                point.X < this.area.Right &&
                point.Y >= this.area.Top &&
                point.Y < this.area.Bottom;
        }

        private bool IntersectsWithTriangle(Point point)
        {
            if (!this.IntersectsWithRectangle(point))
            {
                return false;
            }

            // Divide precision by 2
            point = new Point((point.X / Precision) * Precision, (point.Y / Precision) * Precision);
            int x = point.X - this.area.X; // X coordinate relative to the triangle top-left corner
            int y = point.Y - this.area.Y; // Y coordinate relative to the triangle top-left corner

            switch (this.AreaShape)
            {
                case TrackAIElementShape.TriangleTopLeft:
                    return x + y <= this.area.Width - Precision;

                case TrackAIElementShape.TriangleTopRight:
                    return x >= y;

                case TrackAIElementShape.TriangleBottomRight:
                    return x + y >= this.area.Width - Precision;

                case TrackAIElementShape.TriangleBottomLeft:
                    return x <= y;

                default:
                    throw new InvalidOperationException();
            }
        }

        public ResizeHandle GetResizeHandle(Point point)
        {
            if (this.AreaShape == TrackAIElementShape.Rectangle)
            {
                return this.GetResizeHandleRectangle(point);
            }

            return this.GetResizeHandleTriangle(point);
        }

        private ResizeHandle GetResizeHandleRectangle(Point point)
        {
            ResizeHandle resizeHandle;

            if (point.X > this.area.Left &&
                point.X < this.area.Right - 1 &&
                point.Y > this.area.Top &&
                point.Y < this.area.Bottom - 1)
            {
                resizeHandle = ResizeHandle.None;
            }
            else
            {
                if (point.X == this.area.Left)
                {
                    if (point.Y == this.area.Top)
                    {
                        resizeHandle = ResizeHandle.TopLeft;
                    }
                    else if (point.Y == this.area.Bottom - 1)
                    {
                        resizeHandle = ResizeHandle.BottomLeft;
                    }
                    else
                    {
                        resizeHandle = ResizeHandle.Left;
                    }
                }
                else if (point.X == this.area.Right - 1)
                {
                    if (point.Y == this.area.Top)
                    {
                        resizeHandle = ResizeHandle.TopRight;
                    }
                    else if (point.Y == this.area.Bottom - 1)
                    {
                        resizeHandle = ResizeHandle.BottomRight;
                    }
                    else
                    {
                        resizeHandle = ResizeHandle.Right;
                    }
                }
                else
                {
                    if (point.Y == this.area.Top)
                    {
                        resizeHandle = ResizeHandle.Top;
                    }
                    else
                    {
                        resizeHandle = ResizeHandle.Bottom;
                    }
                }
            }

            return resizeHandle;
        }

        private ResizeHandle GetResizeHandleTriangle(Point point)
        {
            int diagonal;

            switch (this.AreaShape)
            {
                case TrackAIElementShape.TriangleTopLeft:
                    #region
                    diagonal = (point.X - this.area.X) + (point.Y - this.area.Y);
                    if (diagonal >= this.area.Width - Precision && diagonal <= this.area.Width)
                    {
                        return ResizeHandle.BottomRight;
                    }

                    if (point.X == this.area.Left)
                    {
                        return ResizeHandle.Left;
                    }

                    if (point.Y == this.area.Top)
                    {
                        return ResizeHandle.Top;
                    }
                    #endregion
                    break;

                case TrackAIElementShape.TriangleTopRight:
                    #region
                    diagonal = (point.X - this.area.X) - (point.Y - this.area.Y);
                    if (diagonal >= -Precision && diagonal <= 0)
                    {
                        return ResizeHandle.BottomLeft;
                    }

                    if (point.X == this.area.Right - 1)
                    {
                        return ResizeHandle.Right;
                    }

                    if (point.Y == this.area.Top)
                    {
                        return ResizeHandle.Top;
                    }
                    #endregion
                    break;

                case TrackAIElementShape.TriangleBottomRight:
                    #region
                    diagonal = (point.X - this.area.X) + (point.Y - this.area.Y);
                    if (diagonal >= this.area.Width - Precision && diagonal <= this.area.Width)
                    {
                        return ResizeHandle.TopLeft;
                    }

                    if (point.X == this.area.Right - 1)
                    {
                        return ResizeHandle.Right;
                    }

                    if (point.Y == this.area.Bottom - 1)
                    {
                        return ResizeHandle.Bottom;
                    }
                    #endregion
                    break;

                case TrackAIElementShape.TriangleBottomLeft:
                    #region
                    diagonal = (point.X - this.area.X) - (point.Y - this.area.Y);
                    if (diagonal >= 0 && diagonal <= Precision)
                    {
                        return ResizeHandle.TopRight;
                    }

                    if (point.X == this.area.Left)
                    {
                        return ResizeHandle.Left;
                    }

                    if (point.Y == this.area.Bottom - 1)
                    {
                        return ResizeHandle.Bottom;
                    }
                    #endregion
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return ResizeHandle.None;
        }

        public Point[] GetTriangle()
        {
            Point[] points = new Point[this.area.Width + 3];

            int x;
            int y;
            int xStep;
            int yStep;
            Point rightAngle;

            switch (this.AreaShape)
            {
                case TrackAIElementShape.TriangleTopLeft:
                    x = this.area.X;
                    y = this.area.Y + this.area.Height;
                    xStep = Precision;
                    yStep = -Precision;
                    rightAngle = this.area.Location;
                    break;

                case TrackAIElementShape.TriangleTopRight:
                    x = this.area.X + this.area.Width;
                    y = this.area.Y + this.area.Height;
                    xStep = -Precision;
                    yStep = -Precision;
                    rightAngle = new Point(x, this.area.Y);
                    break;

                case TrackAIElementShape.TriangleBottomRight:
                    x = this.area.X + this.area.Width;
                    y = this.area.Y;
                    xStep = -Precision;
                    yStep = Precision;
                    rightAngle = new Point(x, this.area.Y + this.area.Height);
                    break;

                case TrackAIElementShape.TriangleBottomLeft:
                    x = this.area.X;
                    y = this.area.Y;
                    xStep = Precision;
                    yStep = Precision;
                    rightAngle = new Point(x, this.area.Y + this.area.Height);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            int i = 0;
            bool even = true;
            while (i < points.Length - Precision)
            {
                points[i++] = new Point(x, y);
                if (even)
                {
                    x += xStep;
                }
                else
                {
                    y += yStep;
                }
                even = !even;
            }

            points[i++] = rightAngle;
            points[i] = points[0];

            return points;
        }

        private void MoveTo(int x, int y)
        {
            // Halve precision, so that areas are positioned following a 2-tile (16-px) step
            x = (x / Precision) * Precision;
            y = (y / Precision) * Precision;

            if (x < 0)
            {
                x = 0;
            }
            else if (x + this.area.Width > TrackMap.Size)
            {
                x = TrackMap.Size - this.area.Width;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y + this.area.Height > TrackMap.Size)
            {
                y = TrackMap.Size - this.area.Height;
            }

            int targetX = x - (this.area.X - this.target.X);
            int targetY = y - (this.area.Y - this.target.Y);

            if (this.area.X != x || this.area.Y != y)
            {
                this.area.Location = new Point(x, y);
                this.OnPropertyChanged(PropertyNames.TrackAIElement.Location);

                this.MoveTargetTo(targetX, targetY);
            }
        }

        private void MoveTargetTo(int x, int y)
        {
            if (x < 0)
            {
                x = 0;
            }
            else if (x > TrackMap.Limit)
            {
                x = TrackMap.Limit;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y > TrackMap.Limit)
            {
                y = TrackMap.Limit;
            }

            if (this.target.X != x || this.target.Y != y)
            {
                this.target = new Point(x, y);
                this.OnPropertyChanged(PropertyNames.TrackAIElement.Target);
            }
        }

        public void Resize(ResizeHandle resizeHandle, int x, int y)
        {
            // Halve precision, so that areas are positioned following a 2-tile (16-px) step
            x = (x / Precision) * Precision;
            y = (y / Precision) * Precision;

            if (this.AreaShape == TrackAIElementShape.Rectangle)
            {
                this.ResizeRectangle(resizeHandle, x, y);
            }
            else
            {
                this.ResizeTriangle(resizeHandle, x, y);
            }

            this.OnPropertyChanged(PropertyNames.TrackAIElement.Area);
        }

        private void ResizeRectangle(ResizeHandle resizeHandle, int x, int y)
        {
            int areaX;
            int areaY;
            int width;
            int height;

            switch (resizeHandle)
            {
                case ResizeHandle.TopLeft:
                    #region
                    if (x >= this.area.Right)
                    {
                        x = this.area.Right - Precision;
                    }

                    if (y >= this.area.Bottom)
                    {
                        y = this.area.Bottom - Precision;
                    }

                    areaX = x;
                    areaY = y;
                    width = this.area.Right - x;
                    height = this.area.Bottom - y;
                    #endregion
                    break;

                case ResizeHandle.Top:
                    #region
                    if (y >= this.area.Bottom)
                    {
                        y = this.area.Bottom - Precision;
                    }

                    areaX = this.area.X;
                    areaY = y;
                    width = this.area.Width;
                    height = this.area.Bottom - y;
                    #endregion
                    break;

                case ResizeHandle.TopRight:
                    #region
                    if (x < this.area.Left)
                    {
                        x = this.area.Left;
                    }

                    if (y >= this.area.Bottom)
                    {
                        y = this.area.Bottom - Precision;
                    }

                    areaX = this.area.X;
                    areaY = y;
                    width = x - this.area.Left + Precision;
                    height = this.area.Bottom - y;
                    #endregion
                    break;

                case ResizeHandle.Right:
                    #region
                    if (x < this.area.Left)
                    {
                        x = this.area.Left;
                    }

                    areaX = this.area.X;
                    areaY = this.area.Y;
                    width = x - this.area.Left + Precision;
                    height = this.area.Height;
                    #endregion
                    break;

                case ResizeHandle.BottomRight:
                    #region
                    if (x < this.area.Left)
                    {
                        x = this.area.Left;
                    }

                    if (y < this.area.Top)
                    {
                        y = this.area.Top;
                    }

                    areaX = this.area.X;
                    areaY = this.area.Y;
                    width = x - this.area.Left + Precision;
                    height = y - this.area.Top + Precision;
                    #endregion
                    break;

                case ResizeHandle.Bottom:
                    #region
                    if (y < this.area.Top)
                    {
                        y = this.area.Top;
                    }

                    areaX = this.area.X;
                    areaY = this.area.Y;
                    width = this.area.Width;
                    height = y - this.area.Top + Precision;
                    #endregion
                    break;

                case ResizeHandle.BottomLeft:
                    #region
                    if (x >= this.area.Right)
                    {
                        x = this.area.Right - Precision;
                    }

                    if (y < this.area.Top)
                    {
                        y = this.area.Top;
                    }

                    areaX = x;
                    areaY = this.area.Y;
                    width = this.area.Right - x;
                    height = y - this.area.Top + Precision;
                    #endregion
                    break;

                case ResizeHandle.Left:
                    #region
                    if (x >= this.area.Right)
                    {
                        x = this.area.Right - Precision;
                    }

                    areaX = x;
                    areaY = this.area.Y;
                    width = this.area.Right - x;
                    height = this.area.Height;
                    #endregion
                    break;

                default:
                    throw new InvalidOperationException();
            }

            this.area = new Rectangle(areaX, areaY, width, height);
        }

        private void ResizeTriangle(ResizeHandle resizeHandle, int x, int y)
        {
            int areaX;
            int areaY;
            int length;

            switch (resizeHandle)
            {
                case ResizeHandle.TopLeft:
                    #region
                    length = (this.area.Right - x) + (this.area.Bottom - y);

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(length - this.area.Right, length - this.area.Bottom);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length

                    areaX = this.area.Right - length;
                    areaY = this.area.Bottom - length;
                    #endregion
                    break;

                case ResizeHandle.Top:
                    #region
                    length = this.area.Bottom - y;

                    if (this.AreaShape == TrackAIElementShape.TriangleTopLeft)
                    {
                        areaX = this.area.Left;

                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = this.area.X + length - TrackMap.Size;
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaY = this.area.Bottom - length;
                    }
                    else //if (this.AreaShape == Shape.TriangleTopRight)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = length - this.area.Right;
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaX = this.area.Right - length;
                        areaY = this.area.Bottom - length;
                    }
                    #endregion
                    break;

                case ResizeHandle.TopRight:
                    #region
                    length = (x - this.area.X) + (this.area.Bottom - y);
                    areaX = this.area.X;

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(areaX + length - TrackMap.Size, length - this.area.Bottom);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length

                    areaY = this.area.Bottom - length;
                    #endregion
                    break;

                case ResizeHandle.Right:
                    #region
                    length = x - this.area.X + Precision;
                    areaX = this.area.X;

                    if (this.AreaShape == TrackAIElementShape.TriangleTopRight)
                    {
                        areaY = this.area.Y;

                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(areaX + length - TrackMap.Size, areaY + length - TrackMap.Size);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length
                    }
                    else //if (this.AreaShape == Shape.TriangleBottomRight)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(areaX + length - TrackMap.Size, length - this.area.Bottom);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaY = this.area.Bottom - length;
                    }
                    #endregion
                    break;

                case ResizeHandle.BottomRight:
                    #region
                    length = (x - this.area.X) + (y - this.area.Y);
                    areaX = this.area.X;
                    areaY = this.area.Y;

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(areaX + length - TrackMap.Size, areaY + length - TrackMap.Size);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length
                    #endregion
                    break;

                case ResizeHandle.Bottom:
                    #region
                    length = y - this.area.Y + Precision;
                    areaY = this.area.Y;

                    if (this.AreaShape == TrackAIElementShape.TriangleBottomRight)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(length - this.area.Right, areaY + length - TrackMap.Size);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaX = this.area.Right - length;
                    }
                    else //if (this.AreaShape == Shape.TriangleBottomLeft)
                    {
                        areaX = this.area.X;

                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(areaX + length - TrackMap.Size, areaY + length - TrackMap.Size);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length
                    }
                    #endregion
                    break;

                case ResizeHandle.BottomLeft:
                    #region
                    length = (this.area.Right - x) + (y - this.area.Y);
                    areaY = this.area.Y;

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(length - this.area.Right, areaY + length - TrackMap.Size);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length

                    areaX = this.area.Right - length;
                    #endregion
                    break;

                case ResizeHandle.Left:
                    #region
                    length = this.area.Right - x;

                    if (this.AreaShape == TrackAIElementShape.TriangleTopLeft)
                    {
                        areaY = this.area.Y;

                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(length - this.area.Right, areaY + length - TrackMap.Size);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length
                    }
                    else //if (this.AreaShape == Shape.TriangleBottomLeft)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(length - this.area.Right, length - this.area.Bottom);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaY = this.area.Bottom - length;
                    }

                    areaX = this.area.Right - length;
                    #endregion
                    break;

                default:
                    throw new InvalidOperationException();
            }

            this.area = new Rectangle(areaX, areaY, length, length);
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        public void GetTargetBytes(byte[] data, ref int index)
        {
            data[index++] = (byte)this.target.X;
            data[index++] = (byte)this.target.Y;
            data[index++] = (byte)(this.speed + (!this.isIntersection ? 0x00 : 0x80));
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        public void GetAreaBytes(byte[] data, ref int index)
        {
            data[index++] = (byte)this.AreaShape;

            if (this.AreaShape == TrackAIElementShape.Rectangle)
            {
                data[index++] = (byte)(this.area.X / Precision);
                data[index++] = (byte)(this.area.Y / Precision);
                data[index++] = (byte)(this.area.Width / Precision);
                data[index++] = (byte)(this.area.Height / Precision);
            }
            else
            {
                int size = this.area.Width / Precision;

                switch (this.AreaShape)
                {
                    case TrackAIElementShape.TriangleTopLeft:
                        data[index++] = (byte)(this.area.X / Precision);
                        data[index++] = (byte)(this.area.Y / Precision);
                        break;

                    case TrackAIElementShape.TriangleTopRight:
                        data[index++] = (byte)(this.area.X / Precision + size - 1);
                        data[index++] = (byte)(this.area.Y / Precision);
                        break;

                    case TrackAIElementShape.TriangleBottomRight:
                        data[index++] = (byte)(this.area.X / Precision + size - 1);
                        data[index++] = (byte)(this.area.Y / Precision + size - 1);
                        break;

                    case TrackAIElementShape.TriangleBottomLeft:
                        data[index++] = (byte)(this.area.X / Precision);
                        data[index++] = (byte)(this.area.Y / Precision + size - 1);
                        break;
                }

                data[index++] = (byte)size;
            }
        }

        public TrackAIElement Clone()
        {
            return new TrackAIElement
            {
                target = this.target,
                speed = this.speed,
                isIntersection = this.isIntersection,
                area = this.area,
                areaShape = this.areaShape
            };
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
