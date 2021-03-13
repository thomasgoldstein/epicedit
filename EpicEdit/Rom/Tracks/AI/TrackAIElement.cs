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

        private TrackAIElementShape _areaShape;
        /// <summary>
        /// Gets or sets the area shape.
        /// </summary>
        public TrackAIElementShape AreaShape
        {
            get => _areaShape;
            set
            {
                if (_areaShape == value)
                {
                    return;
                }

                if (_areaShape == TrackAIElementShape.Rectangle &&
                    value != TrackAIElementShape.Rectangle)
                {
                    if (_area.Width > _area.Height)
                    {
                        _area.Width = _area.Height;
                    }
                    else if (_area.Height > _area.Width)
                    {
                        _area.Height = _area.Width;
                    }
                }

                _areaShape = value;
                OnPropertyChanged(PropertyNames.TrackAIElement.AreaShape);
            }
        }

        private Rectangle _area;
        /// <summary>
        /// Gets the area.
        /// </summary>
        public Rectangle Area => _area;

        private Point _target;
        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        public Point Target
        {
            get => _target;
            set => MoveTargetTo(value.X, value.Y);
        }

        private byte _speed;
        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        public byte Speed
        {
            get => _speed;
            set
            {
                if (_speed == value)
                {
                    return;
                }

                _speed = value;
                OnPropertyChanged(PropertyNames.TrackAIElement.Speed);
            }
        }

        private bool _isIntersection;
        /// <summary>
        /// Gets or sets a value that determines if the element is at an intersection.
        /// When an AI element is flagged as an intersection, this tells the AI to ignore
        /// the intersected AI area, and avoids track object display issues when switching areas.
        /// </summary>
        public bool IsIntersection
        {
            get => _isIntersection;
            set
            {
                if (_isIntersection == value)
                {
                    return;
                }

                _isIntersection = value;
                OnPropertyChanged(PropertyNames.TrackAIElement.IsIntersection);
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public Point Location
        {
            get => _area.Location;
            set => MoveTo(value.X, value.Y);
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
            AreaShape = (TrackAIElementShape)areaData[areaDataIndex++];
            int areaX = areaData[areaDataIndex++] * Precision;
            int areaY = areaData[areaDataIndex++] * Precision;

            if (AreaShape == TrackAIElementShape.Rectangle)
            {
                int areaWidth = areaData[areaDataIndex++] * Precision;
                int areaHeight = areaData[areaDataIndex++] * Precision;

                _area = new Rectangle(areaX, areaY, areaWidth, areaHeight);
            }
            else
            {
                int areaSize = areaData[areaDataIndex++] * Precision;

                // In the ROM, the X and Y values of a triangle
                // determine the location of its right angle.
                // We don't follow this logic, and change X and Y
                // to make them always determine the top left corner.
                switch (AreaShape)
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

                _area = new Rectangle(areaX, areaY, areaSize, areaSize);
            }

            _target = new Point(targetData[targetDataIndex++], targetData[targetDataIndex++]);
            byte speedAndIntersection = targetData[targetDataIndex++];
            _speed = (byte)(speedAndIntersection & 0x03);
            _isIntersection = (speedAndIntersection & 0x80) == 0x80;
        }

        public TrackAIElement(Point position)
        {
            const int size = 16;
            // Halve precision, so that areas are positioned following a 2-tile (16-px) step
            int areaX = ((position.X - (size / 2)) / Precision) * Precision;
            int areaY = ((position.Y - (size / 2)) / Precision) * Precision;

            // Ensure the element isn't out of the track bounds
            areaX = areaX < 0 ? 0 :
                (areaX + size) > TrackMap.Size ? TrackMap.Size - size :
                areaX;

            areaY = areaY < 0 ? 0 :
                (areaY + size) > TrackMap.Size ? TrackMap.Size - size :
                areaY;

            Rectangle area = new Rectangle(areaX, areaY, size, size);

            _area = area;

            int x = area.X + area.Width / Precision;
            int y = area.Y + area.Height / Precision;
            _target = new Point(x, y);
            _speed = 0;
        }

        private TrackAIElement() { }

        public bool IntersectsWith(Point point)
        {
            if (AreaShape == TrackAIElementShape.Rectangle)
            {
                return IntersectsWithRectangle(point);
            }

            return IntersectsWithTriangle(point);
        }

        private bool IntersectsWithRectangle(Point point)
        {
            return
                point.X >= _area.Left &&
                point.X < _area.Right &&
                point.Y >= _area.Top &&
                point.Y < _area.Bottom;
        }

        private bool IntersectsWithTriangle(Point point)
        {
            if (!IntersectsWithRectangle(point))
            {
                return false;
            }

            // Divide precision by 2
            point = new Point((point.X / Precision) * Precision, (point.Y / Precision) * Precision);
            int x = point.X - _area.X; // X coordinate relative to the triangle top-left corner
            int y = point.Y - _area.Y; // Y coordinate relative to the triangle top-left corner

            switch (AreaShape)
            {
                case TrackAIElementShape.TriangleTopLeft:
                    return x + y <= _area.Width - Precision;

                case TrackAIElementShape.TriangleTopRight:
                    return x >= y;

                case TrackAIElementShape.TriangleBottomRight:
                    return x + y >= _area.Width - Precision;

                case TrackAIElementShape.TriangleBottomLeft:
                    return x <= y;

                default:
                    throw new InvalidOperationException();
            }
        }

        public ResizeHandle GetResizeHandle(Point point)
        {
            if (AreaShape == TrackAIElementShape.Rectangle)
            {
                return GetResizeHandleRectangle(point);
            }

            return GetResizeHandleTriangle(point);
        }

        private ResizeHandle GetResizeHandleRectangle(Point point)
        {
            ResizeHandle resizeHandle;

            if (point.X > _area.Left &&
                point.X < _area.Right - 1 &&
                point.Y > _area.Top &&
                point.Y < _area.Bottom - 1)
            {
                resizeHandle = ResizeHandle.None;
            }
            else
            {
                if (point.X == _area.Left)
                {
                    if (point.Y == _area.Top)
                    {
                        resizeHandle = ResizeHandle.TopLeft;
                    }
                    else if (point.Y == _area.Bottom - 1)
                    {
                        resizeHandle = ResizeHandle.BottomLeft;
                    }
                    else
                    {
                        resizeHandle = ResizeHandle.Left;
                    }
                }
                else if (point.X == _area.Right - 1)
                {
                    if (point.Y == _area.Top)
                    {
                        resizeHandle = ResizeHandle.TopRight;
                    }
                    else if (point.Y == _area.Bottom - 1)
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
                    if (point.Y == _area.Top)
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

            switch (AreaShape)
            {
                case TrackAIElementShape.TriangleTopLeft:
                    #region
                    diagonal = (point.X - _area.X) + (point.Y - _area.Y);
                    if (diagonal >= _area.Width - Precision && diagonal <= _area.Width)
                    {
                        return ResizeHandle.BottomRight;
                    }

                    if (point.X == _area.Left)
                    {
                        return ResizeHandle.Left;
                    }

                    if (point.Y == _area.Top)
                    {
                        return ResizeHandle.Top;
                    }
                    #endregion
                    break;

                case TrackAIElementShape.TriangleTopRight:
                    #region
                    diagonal = (point.X - _area.X) - (point.Y - _area.Y);
                    if (diagonal >= -Precision && diagonal <= 0)
                    {
                        return ResizeHandle.BottomLeft;
                    }

                    if (point.X == _area.Right - 1)
                    {
                        return ResizeHandle.Right;
                    }

                    if (point.Y == _area.Top)
                    {
                        return ResizeHandle.Top;
                    }
                    #endregion
                    break;

                case TrackAIElementShape.TriangleBottomRight:
                    #region
                    diagonal = (point.X - _area.X) + (point.Y - _area.Y);
                    if (diagonal >= _area.Width - Precision && diagonal <= _area.Width)
                    {
                        return ResizeHandle.TopLeft;
                    }

                    if (point.X == _area.Right - 1)
                    {
                        return ResizeHandle.Right;
                    }

                    if (point.Y == _area.Bottom - 1)
                    {
                        return ResizeHandle.Bottom;
                    }
                    #endregion
                    break;

                case TrackAIElementShape.TriangleBottomLeft:
                    #region
                    diagonal = (point.X - _area.X) - (point.Y - _area.Y);
                    if (diagonal >= 0 && diagonal <= Precision)
                    {
                        return ResizeHandle.TopRight;
                    }

                    if (point.X == _area.Left)
                    {
                        return ResizeHandle.Left;
                    }

                    if (point.Y == _area.Bottom - 1)
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
            Point[] points = new Point[_area.Width + 3];

            int x;
            int y;
            int xStep;
            int yStep;
            Point rightAngle;

            switch (AreaShape)
            {
                case TrackAIElementShape.TriangleTopLeft:
                    x = _area.X;
                    y = _area.Y + _area.Height;
                    xStep = Precision;
                    yStep = -Precision;
                    rightAngle = _area.Location;
                    break;

                case TrackAIElementShape.TriangleTopRight:
                    x = _area.X + _area.Width;
                    y = _area.Y + _area.Height;
                    xStep = -Precision;
                    yStep = -Precision;
                    rightAngle = new Point(x, _area.Y);
                    break;

                case TrackAIElementShape.TriangleBottomRight:
                    x = _area.X + _area.Width;
                    y = _area.Y;
                    xStep = -Precision;
                    yStep = Precision;
                    rightAngle = new Point(x, _area.Y + _area.Height);
                    break;

                case TrackAIElementShape.TriangleBottomLeft:
                    x = _area.X;
                    y = _area.Y;
                    xStep = Precision;
                    yStep = Precision;
                    rightAngle = new Point(x, _area.Y + _area.Height);
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
            else if (x + _area.Width > TrackMap.Size)
            {
                x = TrackMap.Size - _area.Width;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y + _area.Height > TrackMap.Size)
            {
                y = TrackMap.Size - _area.Height;
            }

            int targetX = x - (_area.X - _target.X);
            int targetY = y - (_area.Y - _target.Y);

            if (_area.X != x || _area.Y != y)
            {
                _area.Location = new Point(x, y);
                OnPropertyChanged(PropertyNames.TrackAIElement.Location);

                MoveTargetTo(targetX, targetY);
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

            if (_target.X != x || _target.Y != y)
            {
                _target = new Point(x, y);
                OnPropertyChanged(PropertyNames.TrackAIElement.Target);
            }
        }

        public void Resize(ResizeHandle resizeHandle, int x, int y)
        {
            // Halve precision, so that areas are positioned following a 2-tile (16-px) step
            x = (x / Precision) * Precision;
            y = (y / Precision) * Precision;

            if (AreaShape == TrackAIElementShape.Rectangle)
            {
                ResizeRectangle(resizeHandle, x, y);
            }
            else
            {
                ResizeTriangle(resizeHandle, x, y);
            }

            OnPropertyChanged(PropertyNames.TrackAIElement.Area);
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
                    if (x >= _area.Right)
                    {
                        x = _area.Right - Precision;
                    }

                    if (y >= _area.Bottom)
                    {
                        y = _area.Bottom - Precision;
                    }

                    areaX = x;
                    areaY = y;
                    width = _area.Right - x;
                    height = _area.Bottom - y;
                    #endregion
                    break;

                case ResizeHandle.Top:
                    #region
                    if (y >= _area.Bottom)
                    {
                        y = _area.Bottom - Precision;
                    }

                    areaX = _area.X;
                    areaY = y;
                    width = _area.Width;
                    height = _area.Bottom - y;
                    #endregion
                    break;

                case ResizeHandle.TopRight:
                    #region
                    if (x < _area.Left)
                    {
                        x = _area.Left;
                    }

                    if (y >= _area.Bottom)
                    {
                        y = _area.Bottom - Precision;
                    }

                    areaX = _area.X;
                    areaY = y;
                    width = x - _area.Left + Precision;
                    height = _area.Bottom - y;
                    #endregion
                    break;

                case ResizeHandle.Right:
                    #region
                    if (x < _area.Left)
                    {
                        x = _area.Left;
                    }

                    areaX = _area.X;
                    areaY = _area.Y;
                    width = x - _area.Left + Precision;
                    height = _area.Height;
                    #endregion
                    break;

                case ResizeHandle.BottomRight:
                    #region
                    if (x < _area.Left)
                    {
                        x = _area.Left;
                    }

                    if (y < _area.Top)
                    {
                        y = _area.Top;
                    }

                    areaX = _area.X;
                    areaY = _area.Y;
                    width = x - _area.Left + Precision;
                    height = y - _area.Top + Precision;
                    #endregion
                    break;

                case ResizeHandle.Bottom:
                    #region
                    if (y < _area.Top)
                    {
                        y = _area.Top;
                    }

                    areaX = _area.X;
                    areaY = _area.Y;
                    width = _area.Width;
                    height = y - _area.Top + Precision;
                    #endregion
                    break;

                case ResizeHandle.BottomLeft:
                    #region
                    if (x >= _area.Right)
                    {
                        x = _area.Right - Precision;
                    }

                    if (y < _area.Top)
                    {
                        y = _area.Top;
                    }

                    areaX = x;
                    areaY = _area.Y;
                    width = _area.Right - x;
                    height = y - _area.Top + Precision;
                    #endregion
                    break;

                case ResizeHandle.Left:
                    #region
                    if (x >= _area.Right)
                    {
                        x = _area.Right - Precision;
                    }

                    areaX = x;
                    areaY = _area.Y;
                    width = _area.Right - x;
                    height = _area.Height;
                    #endregion
                    break;

                default:
                    throw new InvalidOperationException();
            }

            _area = new Rectangle(areaX, areaY, width, height);
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
                    length = (_area.Right - x) + (_area.Bottom - y);

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(length - _area.Right, length - _area.Bottom);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length

                    areaX = _area.Right - length;
                    areaY = _area.Bottom - length;
                    #endregion
                    break;

                case ResizeHandle.Top:
                    #region
                    length = _area.Bottom - y;

                    if (AreaShape == TrackAIElementShape.TriangleTopLeft)
                    {
                        areaX = _area.Left;

                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = _area.X + length - TrackMap.Size;
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaY = _area.Bottom - length;
                    }
                    else //if (AreaShape == Shape.TriangleTopRight)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = length - _area.Right;
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaX = _area.Right - length;
                        areaY = _area.Bottom - length;
                    }
                    #endregion
                    break;

                case ResizeHandle.TopRight:
                    #region
                    length = (x - _area.X) + (_area.Bottom - y);
                    areaX = _area.X;

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(areaX + length - TrackMap.Size, length - _area.Bottom);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length

                    areaY = _area.Bottom - length;
                    #endregion
                    break;

                case ResizeHandle.Right:
                    #region
                    length = x - _area.X + Precision;
                    areaX = _area.X;

                    if (AreaShape == TrackAIElementShape.TriangleTopRight)
                    {
                        areaY = _area.Y;

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
                    else //if (AreaShape == Shape.TriangleBottomRight)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(areaX + length - TrackMap.Size, length - _area.Bottom);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaY = _area.Bottom - length;
                    }
                    #endregion
                    break;

                case ResizeHandle.BottomRight:
                    #region
                    length = (x - _area.X) + (y - _area.Y);
                    areaX = _area.X;
                    areaY = _area.Y;

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
                    length = y - _area.Y + Precision;
                    areaY = _area.Y;

                    if (AreaShape == TrackAIElementShape.TriangleBottomRight)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(length - _area.Right, areaY + length - TrackMap.Size);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaX = _area.Right - length;
                    }
                    else //if (AreaShape == Shape.TriangleBottomLeft)
                    {
                        areaX = _area.X;

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
                    length = (_area.Right - x) + (y - _area.Y);
                    areaY = _area.Y;

                    #region Validate area length
                    if (length < Precision)
                    {
                        length = Precision;
                    }
                    else
                    {
                        int offBounds = Math.Max(length - _area.Right, areaY + length - TrackMap.Size);
                        if (offBounds > 0)
                        {
                            length -= offBounds;
                        }
                    }
                    #endregion Validate area length

                    areaX = _area.Right - length;
                    #endregion
                    break;

                case ResizeHandle.Left:
                    #region
                    length = _area.Right - x;

                    if (AreaShape == TrackAIElementShape.TriangleTopLeft)
                    {
                        areaY = _area.Y;

                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(length - _area.Right, areaY + length - TrackMap.Size);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length
                    }
                    else //if (AreaShape == Shape.TriangleBottomLeft)
                    {
                        #region Validate area length
                        if (length < Precision)
                        {
                            length = Precision;
                        }
                        else
                        {
                            int offBounds = Math.Max(length - _area.Right, length - _area.Bottom);
                            if (offBounds > 0)
                            {
                                length -= offBounds;
                            }
                        }
                        #endregion Validate area length

                        areaY = _area.Bottom - length;
                    }

                    areaX = _area.Right - length;
                    #endregion
                    break;

                default:
                    throw new InvalidOperationException();
            }

            _area = new Rectangle(areaX, areaY, length, length);
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        public void GetTargetBytes(byte[] data, ref int index)
        {
            data[index++] = (byte)_target.X;
            data[index++] = (byte)_target.Y;
            data[index++] = (byte)(_speed + (!_isIntersection ? 0x00 : 0x80));
        }

        /// <summary>
        /// Fills the passed byte array with data in the format the SMK ROM expects.
        /// </summary>
        /// <param name="data">The byte array to fill.</param>
        /// <param name="index">The array position where the data will be copied.</param>
        public void GetAreaBytes(byte[] data, ref int index)
        {
            data[index++] = (byte)AreaShape;

            if (AreaShape == TrackAIElementShape.Rectangle)
            {
                data[index++] = (byte)(_area.X / Precision);
                data[index++] = (byte)(_area.Y / Precision);
                data[index++] = (byte)(_area.Width / Precision);
                data[index++] = (byte)(_area.Height / Precision);
            }
            else
            {
                int size = _area.Width / Precision;

                switch (AreaShape)
                {
                    case TrackAIElementShape.TriangleTopLeft:
                        data[index++] = (byte)(_area.X / Precision);
                        data[index++] = (byte)(_area.Y / Precision);
                        break;

                    case TrackAIElementShape.TriangleTopRight:
                        data[index++] = (byte)(_area.X / Precision + size - 1);
                        data[index++] = (byte)(_area.Y / Precision);
                        break;

                    case TrackAIElementShape.TriangleBottomRight:
                        data[index++] = (byte)(_area.X / Precision + size - 1);
                        data[index++] = (byte)(_area.Y / Precision + size - 1);
                        break;

                    case TrackAIElementShape.TriangleBottomLeft:
                        data[index++] = (byte)(_area.X / Precision);
                        data[index++] = (byte)(_area.Y / Precision + size - 1);
                        break;
                }

                data[index++] = (byte)size;
            }
        }

        public TrackAIElement Clone()
        {
            return new TrackAIElement
            {
                _target = _target,
                _speed = _speed,
                _isIntersection = _isIntersection,
                _area = _area,
                _areaShape = _areaShape
            };
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
