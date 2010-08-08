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
using System.ComponentModel;
using System.Drawing;

namespace EpicEdit.Rom.Tracks.AI
{
	public enum Shape
	{
		[Description("Rectangle")]
		Rectangle = 0,
		[Description("Triangle top left")]
		TriangleTopLeft = 2, // Top-left angle is the right angle
		[Description("Triangle top right")]
		TriangleTopRight = 4, // And so on
		[Description("Triangle bottom right")]
		TriangleBottomRight = 6,
		[Description("Triangle bottom left")]
		TriangleBottomLeft = 8
	}

	/// <summary>
	/// Represents an element of the <see cref="TrackAI"/>.
	/// </summary>
	public class TrackAIElement
	{
		public Shape ZoneShape { get; private set; }
		private Rectangle zone;
		public Rectangle Zone
		{
			get { return this.zone; }
		}
		private Point target;
		public Point Target
		{
			get { return this.target; }
			set { this.MoveTargetTo(value.X, value.Y); }
		}
		public byte Speed { get; set; }

		/// <summary>
		/// Initializes a TrackAIElement.
		/// </summary>
		/// <param name="zoneData">The byte array to get the zone data from.</param>
		/// <param name="zoneDataIndex">The index to use in the zone byte array.</param>
		/// <param name="targetData">The byte array to get the target data from.</param>
		/// <param name="targetDataIndex">The index to use in the target byte array.</param>
		public TrackAIElement(byte[] zoneData, ref int zoneDataIndex, byte[] targetData, ref int targetDataIndex)
		{
			this.ZoneShape = (Shape)zoneData[zoneDataIndex++];
			int zoneX = zoneData[zoneDataIndex++] * 2;
			int zoneY = zoneData[zoneDataIndex++] * 2;

			if (this.ZoneShape == Shape.Rectangle)
			{
				int zoneWidth = zoneData[zoneDataIndex++] * 2;
				int zoneHeight = zoneData[zoneDataIndex++] * 2;

				this.zone = new Rectangle(zoneX, zoneY, zoneWidth, zoneHeight);
			}
			else
			{
				int zoneSize = zoneData[zoneDataIndex++] * 2;

				// In the ROM, the X and Y values of a triangle
				// determine the location of its right angle.
				// We don't follow this logic, and change X and Y
				// to make them always determine the top left corner.
				if (this.ZoneShape == Shape.TriangleTopRight)
				{
					zoneX -= zoneSize - 2;
				}
				else if (this.ZoneShape == Shape.TriangleBottomRight)
				{
					zoneX -= zoneSize - 2;
					zoneY -= zoneSize - 2;
				}
				else if (this.ZoneShape == Shape.TriangleBottomLeft)
				{
					zoneY -= zoneSize - 2;
				}

				this.zone = new Rectangle(zoneX, zoneY, zoneSize, zoneSize);
			}

			this.target = new Point(targetData[targetDataIndex++], targetData[targetDataIndex++]);
			this.Speed = (byte)(targetData[targetDataIndex++] & 3);
			// In the original SMK ROM, Mario Circuit 2 has an AI element that has 0x80
			// tacked on its speed value. This is made to avoid a problem regarding object zones,
			// so that when doing the jump over the other road, the 4 pipes in front of you
			// still show up despite the fact you're over a different object zone at this point.
			// The extra 0x80 value is not actually related to the speed
			// (and will automatically be regenerated when resaving the AI), hence the bit mask.
		}

		public TrackAIElement(Point position)
		{
			int zoneX = ((position.X - 8) / 2) * 2;
			int zoneY = ((position.Y - 8) / 2) * 2;
			int size = 16;

			#region Ensure the element isn't out of the track bounds
			if (zoneX < 0)
			{
				zoneX = 0;
			}
			else if ((zoneX + size) > 128)
			{
				zoneX = 128 - size;
			}

			if (zoneY < 0)
			{
				zoneY = 0;
			}
			else if ((zoneY + size) > 128)
			{
				zoneY = 128 - size;
			}
			#endregion Ensure the element isn't out of the track bounds

			Rectangle zone = new Rectangle(zoneX, zoneY, size, size);

			this.zone = zone;

			int x = zone.X + zone.Width / 2;
			int y = zone.Y + zone.Height / 2;
			this.target = new Point(x, y);
			this.Speed = 0;
		}

		public bool IntersectsWith(Point point)
		{
			if (this.ZoneShape == Shape.Rectangle)
			{
				return this.IntersectsWithRectangle(point);
			}

			return this.IntersectsWithTriangle(point);
		}

		private bool IntersectsWithRectangle(Point point)
		{
			return
				point.X >= this.zone.Left &&
				point.X < this.zone.Right &&
				point.Y >= this.zone.Top &&
				point.Y < this.zone.Bottom;
		}

		private bool IntersectsWithTriangle(Point point)
		{
			if (!this.IntersectsWithRectangle(point))
			{
				return false;
			}

			// Divide precision by 2
			point = new Point((point.X / 2) * 2, (point.Y / 2) * 2);
			int x = point.X - this.zone.X; // X coordinate relative to the triangle top-left corner
			int y = point.Y - this.zone.Y; // Y coordinate relative to the triangle top-left corner

			switch (this.ZoneShape)
			{
				case Shape.TriangleTopLeft:
					return x + y <= this.zone.Width - 2;

				case Shape.TriangleTopRight:
					return x >= y;

				case Shape.TriangleBottomRight:
					return x + y >= this.zone.Width - 2;

				case Shape.TriangleBottomLeft:
					return x <= y;

				default:
					throw new InvalidOperationException();
			}
		}

		public ResizeHandle GetResizeHandle(Point point)
		{
			if (this.ZoneShape == Shape.Rectangle)
			{
				return this.GetResizeHandleRectangle(point);
			}

			return this.GetResizeHandleTriangle(point);
		}

		private ResizeHandle GetResizeHandleRectangle(Point point)
		{
			ResizeHandle resizeHandle;

			if (point.X > this.zone.Left &&
				point.X < this.zone.Right - 1 &&
				point.Y > this.zone.Top &&
				point.Y < this.zone.Bottom - 1)
			{
				resizeHandle = ResizeHandle.None;
			}
			else
			{
				if (point.X == this.zone.Left)
				{
					if (point.Y == this.zone.Top)
					{
						resizeHandle = ResizeHandle.TopLeft;
					}
					else if (point.Y == this.zone.Bottom - 1)
					{
						resizeHandle = ResizeHandle.BottomLeft;
					}
					else
					{
						resizeHandle = ResizeHandle.Left;
					}
				}
				else if (point.X == this.zone.Right - 1)
				{
					if (point.Y == this.zone.Top)
					{
						resizeHandle = ResizeHandle.TopRight;
					}
					else if (point.Y == this.zone.Bottom - 1)
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
					if (point.Y == this.zone.Top)
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

			switch (this.ZoneShape)
			{
				case Shape.TriangleTopLeft:
					#region
					diagonal = (point.X - this.zone.X) + (point.Y - this.zone.Y);
					if (diagonal >= this.zone.Width - 2 && diagonal <= this.zone.Width)
					{
						return ResizeHandle.BottomRight;
					}

					if (point.X == this.zone.Left)
					{
						return ResizeHandle.Left;
					}

					if (point.Y == this.zone.Top)
					{
						return ResizeHandle.Top;
					}
					#endregion
					break;

				case Shape.TriangleTopRight:
					#region
					diagonal = (point.X - this.zone.X) - (point.Y - this.zone.Y);
					if (diagonal >= -2 && diagonal <= 0)
					{
						return ResizeHandle.BottomLeft;
					}

					if (point.X == this.zone.Right - 1)
					{
						return ResizeHandle.Right;
					}

					if (point.Y == this.zone.Top)
					{
						return ResizeHandle.Top;
					}
					#endregion
					break;

				case Shape.TriangleBottomRight:
					#region
					diagonal = (point.X - this.zone.X) + (point.Y - this.zone.Y);
					if (diagonal >= this.zone.Width - 2 && diagonal <= this.zone.Width)
					{
						return ResizeHandle.TopLeft;
					}

					if (point.X == this.zone.Right - 1)
					{
						return ResizeHandle.Right;
					}

					if (point.Y == this.zone.Bottom - 1)
					{
						return ResizeHandle.Bottom;
					}
					#endregion
					break;

				case Shape.TriangleBottomLeft:
					#region
					diagonal = (point.X - this.zone.X) - (point.Y - this.zone.Y);
					if (diagonal >= 0 && diagonal <= 2)
					{
						return ResizeHandle.TopRight;
					}

					if (point.X == this.zone.Left)
					{
						return ResizeHandle.Left;
					}

					if (point.Y == this.zone.Bottom - 1)
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
			Point[] points = new Point[this.zone.Width + 3];

			int x;
			int y;
			int xStep;
			int yStep;
			Point rightAngle;

			switch (this.ZoneShape)
			{
				case Shape.TriangleTopLeft:
					x = this.zone.X;
					y = this.zone.Y + this.zone.Height;
					xStep = 2;
					yStep = -2;
					rightAngle = this.zone.Location;
					break;

				case Shape.TriangleTopRight:
					x = this.zone.X + this.zone.Width;
					y = this.zone.Y + this.zone.Height;
					xStep = -2;
					yStep = -2;
					rightAngle = new Point(x, this.zone.Y);
					break;

				case Shape.TriangleBottomRight:
					x = this.zone.X + this.zone.Width;
					y = this.zone.Y;
					xStep = -2;
					yStep = 2;
					rightAngle = new Point(x, this.zone.Y + this.zone.Height);
					break;

				case Shape.TriangleBottomLeft:
					x = this.zone.X;
					y = this.zone.Y;
					xStep = 2;
					yStep = 2;
					rightAngle = new Point(x, this.zone.Y + this.zone.Height);
					break;

				default:
					throw new InvalidOperationException();
			}

			int i = 0;
			bool even = true;
			while (i < points.Length - 2)
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

		public void MoveTo(int x, int y)
		{
			// Halve precision, so that zones are positioned following a 2-tile (16-px) step
			x = (x / 2) * 2;
			y = (y / 2) * 2;

			if (x < 0)
			{
				x = 0;
			}
			else if (x + this.zone.Width > 128)
			{
				x = 128 - this.zone.Width;
			}

			if (y < 0)
			{
				y = 0;
			}
			else if (y + this.zone.Height > 128)
			{
				y = 128 - this.zone.Height;
			}

			int targetX = x - (this.zone.X - this.target.X);
			int targetY = y - (this.zone.Y - this.target.Y);

			this.zone.Location = new Point(x, y);
			this.MoveTargetTo(targetX, targetY);
		}

		private void MoveTargetTo(int x, int y)
		{
			if (x < 0)
			{
				x = 0;
			}
			else if (x > 127)
			{
				x = 127;
			}

			if (y < 0)
			{
				y = 0;
			}
			else if (y > 127)
			{
				y = 127;
			}

			this.target = new Point(x, y);
		}

		public void Resize(ResizeHandle resizeHandle, int x, int y)
		{
			// Halve precision, so that zones are positioned following a 2-tile (16-px) step
			x = (x / 2) * 2;
			y = (y / 2) * 2;

			if (this.ZoneShape == Shape.Rectangle)
			{
				this.ResizeRectangle(resizeHandle, x, y);
			}
			else
			{
				this.ResizeTriangle(resizeHandle, x, y);
			}
		}

		private void ResizeRectangle(ResizeHandle resizeHandle, int x, int y)
		{
			int zoneX;
			int zoneY;
			int width;
			int height;

			switch (resizeHandle)
			{
				case ResizeHandle.TopLeft:
					#region
					if (x >= this.zone.Right)
					{
						x = this.zone.Right - 2;
					}

					if (y >= this.zone.Bottom)
					{
						y = this.zone.Bottom - 2;
					}

					zoneX = x;
					zoneY = y;
					width = this.zone.Right - x;
					height = this.zone.Bottom - y;
					#endregion
					break;

				case ResizeHandle.Top:
					#region
					if (y >= this.zone.Bottom)
					{
						y = this.zone.Bottom - 2;
					}

					zoneX = this.zone.X;
					zoneY = y;
					width = this.zone.Width;
					height = this.zone.Bottom - y;
					#endregion
					break;

				case ResizeHandle.TopRight:
					#region
					if (x < this.zone.Left)
					{
						x = this.zone.Left;
					}

					if (y >= this.zone.Bottom)
					{
						y = this.zone.Bottom - 2;
					}

					zoneX = this.zone.X;
					zoneY = y;
					width = x - this.zone.Left + 2;
					height = this.zone.Bottom - y;
					#endregion
					break;

				case ResizeHandle.Right:
					#region
					if (x < this.zone.Left)
					{
						x = this.zone.Left;
					}

					zoneX = this.zone.X;
					zoneY = this.zone.Y;
					width = x - this.zone.Left + 2;
					height = this.zone.Height;
					#endregion
					break;

				case ResizeHandle.BottomRight:
					#region
					if (x < this.zone.Left)
					{
						x = this.zone.Left;
					}

					if (y < this.zone.Top)
					{
						y = this.zone.Top;
					}

					zoneX = this.zone.X;
					zoneY = this.zone.Y;
					width = x - this.zone.Left + 2;
					height = y - this.zone.Top + 2;
					#endregion
					break;

				case ResizeHandle.Bottom:
					#region
					if (y < this.zone.Top)
					{
						y = this.zone.Top;
					}

					zoneX = this.zone.X;
					zoneY = this.zone.Y;
					width = this.zone.Width;
					height = y - this.zone.Top + 2;
					#endregion
					break;

				case ResizeHandle.BottomLeft:
					#region
					if (x >= this.zone.Right)
					{
						x = this.zone.Right - 2;
					}

					if (y < this.zone.Top)
					{
						y = this.zone.Top;
					}

					zoneX = x;
					zoneY = this.zone.Y;
					width = this.zone.Right - x;
					height = y - this.zone.Top + 2;
					#endregion
					break;

				case ResizeHandle.Left:
					#region
					if (x >= this.zone.Right)
					{
						x = this.zone.Right - 2;
					}

					zoneX = x;
					zoneY = this.zone.Y;
					width = this.zone.Right - x;
					height = this.zone.Height;
					#endregion
					break;

				default:
					throw new InvalidOperationException();
			}

			this.zone = new Rectangle(zoneX, zoneY, width, height);
		}

		private void ResizeTriangle(ResizeHandle resizeHandle, int x, int y)
		{
			int zoneX;
			int zoneY;
			int length;

			switch (resizeHandle)
			{
				case ResizeHandle.TopLeft:
					#region
					length = (this.zone.Right - x) + (this.zone.Bottom - y);

					#region Validate zone length
					if (length < 2)
					{
						length = 2;
					}
					else
					{
						int offbounds = Math.Max(length - this.zone.Right, length - this.zone.Bottom);
						if (offbounds > 0)
						{
							length -= offbounds;
						}
					}
					#endregion Validate zone length

					zoneX = this.zone.Right - length;
					zoneY = this.zone.Bottom - length;
					#endregion
					break;

				case ResizeHandle.Top:
					#region
					length = this.zone.Bottom - y;

					if (this.ZoneShape == Shape.TriangleTopLeft)
					{
						zoneX = this.zone.Left;

						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = this.zone.X + length - 128;
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion Validate zone length

						zoneY = this.zone.Bottom - length;
					}
					else //if (this.ZoneShape == Shape.TriangleTopRight)
					{
						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = length - this.zone.Right;
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion Validate zone length

						zoneX = this.zone.Right - length;
						zoneY = this.zone.Bottom - length;
					}
					#endregion
					break;

				case ResizeHandle.TopRight:
					#region
					length = (x - this.zone.X) + (this.zone.Bottom - y);
					zoneX = this.zone.X;

					#region Validate zone length
					if (length < 2)
					{
						length = 2;
					}
					else
					{
						int offbounds = Math.Max(zoneX + length - 128, length - this.zone.Bottom);
						if (offbounds > 0)
						{
							length -= offbounds;
						}
					}
					#endregion Validate zone length

					zoneY = this.zone.Bottom - length;
					#endregion
					break;

				case ResizeHandle.Right:
					#region
					length = x - this.zone.X + 2;
					zoneX = this.zone.X;

					if (this.ZoneShape == Shape.TriangleTopRight)
					{
						zoneY = this.zone.Y;

						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = Math.Max(zoneX + length - 128, zoneY + length - 128);
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion Validate zone length
					}
					else //if (this.ZoneShape == Shape.TriangleBottomRight)
					{
						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = Math.Max(zoneX + length - 128, length - this.zone.Bottom);
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion Validate zone length

						zoneY = this.zone.Bottom - length;
					}
					#endregion
					break;

				case ResizeHandle.BottomRight:
					#region
					length = (x - this.zone.X) + (y - this.zone.Y);
					zoneX = this.zone.X;
					zoneY = this.zone.Y;

					#region Validate zone length
					if (length < 2)
					{
						length = 2;
					}
					else
					{
						int offbounds = Math.Max(zoneX + length - 128, zoneY + length - 128);
						if (offbounds > 0)
						{
							length -= offbounds;
						}
					}
					#endregion Validate zone length
					#endregion
					break;

				case ResizeHandle.Bottom:
					#region
					length = y - this.zone.Y + 2;
					zoneY = this.zone.Y;

					if (this.ZoneShape == Shape.TriangleBottomRight)
					{
						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = Math.Max(length - this.zone.Right, zoneY + length - 128);
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion

						zoneX = this.zone.Right - length;
					}
					else //if (this.ZoneShape == Shape.TriangleBottomLeft)
					{
						zoneX = this.zone.X;

						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = Math.Max(zoneX + length - 128, zoneY + length - 128);
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion
					}
					#endregion
					break;

				case ResizeHandle.BottomLeft:
					#region
					length = (this.zone.Right - x) + (y - this.zone.Y);
					zoneY = this.zone.Y;

					#region Validate zone length
					if (length < 2)
					{
						length = 2;
					}
					else
					{
						int offbounds = Math.Max(length - this.zone.Right, zoneY + length - 128);
						if (offbounds > 0)
						{
							length -= offbounds;
						}
					}
					#endregion Validate zone length

					zoneX = this.zone.Right - length;
					#endregion
					break;

				case ResizeHandle.Left:
					#region
					length = this.zone.Right - x;

					if (this.ZoneShape == Shape.TriangleTopLeft)
					{
						zoneY = this.zone.Y;

						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = Math.Max(length - this.zone.Right, zoneY + length - 128);
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion Validate zone length
					}
					else //if (this.ZoneShape == Shape.TriangleBottomLeft)
					{
						#region Validate zone length
						if (length < 2)
						{
							length = 2;
						}
						else
						{
							int offbounds = Math.Max(length - this.zone.Right, length - this.zone.Bottom);
							if (offbounds > 0)
							{
								length -= offbounds;
							}
						}
						#endregion Validate zone length

						zoneY = this.zone.Bottom - length;
					}

					zoneX = this.zone.Right - length;
					#endregion
					break;

				default:
					throw new InvalidOperationException();
			}

			this.zone = new Rectangle(zoneX, zoneY, length, length);
		}

		public void ChangeShape(Shape newShape)
		{
			if (this.ZoneShape == Shape.Rectangle &&
				newShape != Shape.Rectangle)
			{
				if (this.zone.Width > this.zone.Height)
				{
					this.zone.Width = this.zone.Height;
				}
				else if (this.zone.Height > this.zone.Width)
				{
					this.zone.Height = this.zone.Width;
				}
			}

			this.ZoneShape = newShape;
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
			data[index++] = this.Speed;
		}

		/// <summary>
		/// Fills the passed byte array with data in the format the SMK ROM expects.
		/// </summary>
		/// <param name="data">The byte array to fill.</param>
		/// <param name="index">The array position where the data will be copied.</param>
		public void GetZoneBytes(byte[] data, ref int index)
		{
			data[index++] = (byte)this.ZoneShape;

			if (this.ZoneShape == Shape.Rectangle)
			{
				data[index++] = (byte)(this.zone.X / 2);
				data[index++] = (byte)(this.zone.Y / 2);
				data[index++] = (byte)(this.zone.Width / 2);
				data[index++] = (byte)(this.zone.Height / 2);
			}
			else
			{
				int size = this.zone.Width / 2;

				switch (this.ZoneShape)
				{
					case Shape.TriangleTopLeft:
						data[index++] = (byte)(this.zone.X / 2);
						data[index++] = (byte)(this.zone.Y / 2);
						break;

					case Shape.TriangleTopRight:
						data[index++] = (byte)(this.zone.X / 2 + size - 1);
						data[index++] = (byte)(this.zone.Y / 2);
						break;

					case Shape.TriangleBottomRight:
						data[index++] = (byte)(this.zone.X / 2 + size - 1);
						data[index++] = (byte)(this.zone.Y / 2 + size - 1);
						break;

					case Shape.TriangleBottomLeft:
						data[index++] = (byte)(this.zone.X / 2);
						data[index++] = (byte)(this.zone.Y / 2 + size - 1);
						break;
				}

				data[index++] = (byte)size;
			}
		}
	}
}
