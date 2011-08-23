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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.UI.Gfx;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    public partial class ColorPicker : UserControl
    {
        #region Private members

        /// <summary>
        /// The image of the top bar, goes from red to green to blue and back to red.
        /// </summary>
        private Bitmap basicColorsBitmap;

        /// <summary>
        /// The cache for basicColorsBitmap.
        /// </summary>
        private Bitmap basicColorsCache;

        /// <summary>
        /// Cached size for basicColorsBitmap / basicColorsCache.
        /// </summary>
        private Size basicColorsSize;

        /// <summary>
        /// The selected color from basicColorsBitmap.
        /// </summary>
        private RomColor selectedBasicColor = RomColor.From5BitRgb(31, 0, 0);

        /// <summary>
        /// The image where a selected color goes to black, white and gray.
        /// </summary>
        private Bitmap shadesBitmap;

        /// <summary>
        /// The cache for the shadesBitmap.
        /// </summary>
        private Bitmap shadesCache;

        /// <summary>
        /// Cached size for shadesBitmap / shadesCache.
        /// </summary>
        private Size shadesSize;

        /// <summary>
        /// The highlighted color from shadesBitmap.
        /// </summary>
        private Point selectedShadeLocation;

        /// <summary>
        /// Unmodified color used as input, usually comes from the palette.
        /// </summary>
        private RomColor oldColor;

        /// <summary>
        /// The new color, not yet set in the palette.
        /// </summary>
        private RomColor newColor;

        /// <summary>
        /// Used to prevent loops when certain clicks are performed in different UI controls.
        /// </summary>
        private bool performEvents = false;

        #endregion Private members

        public ColorPicker() : this(new RomColor()) { }

        public ColorPicker(RomColor color)
        {
            this.InitializeComponent();

            UITools.FixToolTip(this.oldColorPictureBox, this.oldColorToolTip);
            UITools.FixToolTip(this.newColorPictureBox, this.newColorToolTip);

            this.basicColorsSize = this.basicColorsPictureBox.ClientSize;
            this.shadesSize = this.shadesPictureBox.ClientSize;

            // Initializing Bitmaps in order to avoid having to check if they're null before disposal
            this.basicColorsBitmap = this.shadesBitmap = this.shadesCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);

            this.InitBasicColorsBitmapCache();

            // Convert the input color to an SNES (5-bit) color
            color = color.To5Bit();
            this.UpdateOldColor(color);

            this.selectedBasicColor = this.DrawBasicColorsBitmap(0);
            this.InitShadesCache();
            RomColor shadeColor = this.DrawShadesBitmap(63, 0);
            this.UpdateNewColor(shadeColor);

            this.performEvents = false;

            this.redNumericUpDown.Value = shadeColor.Red5Bit;
            this.greenNumericUpDown.Value = shadeColor.Green5Bit;
            this.blueNumericUpDown.Value = shadeColor.Blue5Bit;

            this.performEvents = true;
        }

        /// <summary>
        /// Sets the color displayed as the old color.
        /// </summary>
        /// <param name="color">The color.</param>
        public void SetColor(RomColor color)
        {
            this.SetNewColor(color);

            this.UpdateOldColor(color);
            this.oldColorPictureBox.Invalidate();
        }

        /// <summary>
        /// Gets the user selected color (new color).
        /// </summary>
        /// <returns>The color.</returns>
        public RomColor SelectedColor
        {
            get
            {
                return this.newColor;
            }
        }

        /// <summary>
        /// Automatically detects where the color exists in the shades and basic colors.
        /// </summary>
        /// <param name="color">The color.</param>
        private void SetNewColor(RomColor color)
        {
            // TODO: The logic to find the location of the color in the basic and shades needs to be revamped, often colors are not found.
            // This is possibly due to the fact that not every shade is displayed. FindBasicColor may possibly be flawed as well.
            color = color.To5Bit();

            this.InvalidateBasicColorsSelection();
            this.selectedBasicColor = this.DrawBasicColorsBitmap(ColorPicker.FindBasicColor(color));
            this.InvalidateBasicColorsSelection();

            this.InitShadesCache();
            this.DrawShadesBitmap(color);
            this.shadesPictureBox.Invalidate();

            this.UpdateNewColor(color);
            this.newColorPictureBox.Invalidate();

            this.performEvents = false;

            this.redNumericUpDown.Value = color.Red5Bit;
            this.greenNumericUpDown.Value = color.Green5Bit;
            this.blueNumericUpDown.Value = color.Blue5Bit;

            this.performEvents = true;
        }

        /// <summary>
        /// Gives a new tool tip object to a control.
        /// </summary>
        private static void SetToolTip(ToolTip toolTip, Control control, RomColor color)
        {
            toolTip.RemoveAll();
            toolTip.SetToolTip(control, color.ToString());
        }

        #region Bitmap drawing

        /// <summary>
        /// Returns the bounds of the passed selection coordinates.
        /// </summary>
        /// <param name="x">X value.</param>
        /// <param name="y">Y value.</param>
        /// <returns>The bounds of the passed selection coordinates.</returns>
        private static Rectangle GetSelectionBounds(int x, int y)
        {
            return new Rectangle(x - 3, y - 3, 6, 6);
        }

        private void InvalidateBasicColorsSelection()
        {
            Rectangle rec = ColorPicker.GetSelectionBounds(this.FindColorIndex(this.selectedBasicColor), this.basicColorsSize.Height / 2);
            rec.Inflate(1, 1);
            this.basicColorsPictureBox.Invalidate(rec);
        }

        private void InvalidateShadesSelection()
        {
            Rectangle rec = ColorPicker.GetSelectionBounds(this.selectedShadeLocation.X, this.selectedShadeLocation.Y);
            rec.Inflate(1, 1);
            this.shadesPictureBox.Invalidate(rec);
        }

        /// <summary>
        /// Draws the basic colors with the circle around a certain x position.
        /// </summary>
        /// <param name="x">The position to select.</param>
        /// <returns>The color at x.</returns>
        private RomColor DrawBasicColorsBitmap(int x)
        {
            RomColor selectedColor = (RomColor)this.basicColorsCache.GetPixel(x, 0);

            this.basicColorsBitmap.Dispose();
            this.basicColorsBitmap = this.basicColorsCache.Clone() as Bitmap;

            using (Graphics g = Graphics.FromImage(this.basicColorsBitmap))
            using (Pen pen = new Pen(selectedColor.Opposite()))
            {
                int y = this.basicColorsSize.Height / 2;
                g.DrawEllipse(pen, ColorPicker.GetSelectionBounds(x, y));
            }

            return selectedColor;
        }

        /// <summary>
        /// Draws the basic colors with the circle around a certain color.
        /// </summary>
        /// <param name="color">The color to select.</param>
        /// <returns>The selected color.</returns>
        private RomColor DrawBasicColorsBitmap(RomColor color)
        {
            this.basicColorsBitmap.Dispose();
            this.basicColorsBitmap = this.basicColorsCache.Clone() as Bitmap;

            int x = this.FindColorIndex(color);
            if (x == -1)
            {
                return new RomColor();
            }

            using (Graphics g = Graphics.FromImage(this.basicColorsBitmap))
            using (Pen pen = new Pen(color.Opposite()))
            {
                g.DrawEllipse(pen, x - 3, 4, 6, 6);
            }

            return color;
        }

        /// <summary>
        /// Initializes the basicColorsCache member.
        /// </summary>
        private void InitBasicColorsBitmapCache()
        {
            int width = this.basicColorsSize.Width;
            int height = this.basicColorsSize.Height;
            Bitmap tempBitmap = new Bitmap(width, 1, PixelFormat.Format32bppPArgb);
            FastBitmap fTempBitmap = new FastBitmap(tempBitmap);

            // Red to yellow
            for (int index = 0; index <= 30; index++) // Skip the last one (31) because it is the same value as the first one of the next loop
            {
                Color color = RomColor.From5BitRgb(31, index, 0);
                fTempBitmap.SetPixel(index, 0, color);
            }

            // Yellow to green
            for (int index = 31; index >= 1; index--) // Skip the last one (0) because it is the same value as the first one of the next loop
            {
                Color color = RomColor.From5BitRgb(index, 31, 0);
                fTempBitmap.SetPixel(31 - index + 31, 0, color);
            }

            // Green to cyan
            for (int index = 0; index <= 30; index++) // Skip the last one (31) because it is the same value as the first one of the next loop
            {
                Color color = RomColor.From5BitRgb(0, 31, index);
                fTempBitmap.SetPixel(index + 62, 0, color);
            }

            // Cyan to blue
            for (int index = 31; index >= 1; index--) // Skip the last one (0) because it is the same value as the first one of the next loop
            {
                Color color = RomColor.From5BitRgb(0, index, 31);
                fTempBitmap.SetPixel((31 - index) + 93, 0, color);
            }

            // Blue to purple
            for (int index = 0; index <= 30; index++) // Skip the last one (31) because it is the same value as the first one of the next loop
            {
                Color color = RomColor.From5BitRgb(index, 0, 31);
                fTempBitmap.SetPixel(index + 124, 0, color);
            }

            // Purple to red
            for (int index = 31; index >= 1; index--) // Skip the last one (0) because it is the same value as the first one of the first loop
            {
                Color color = RomColor.From5BitRgb(31, 0, index);
                fTempBitmap.SetPixel((31 - index) + 155, 0, color);
            }

            fTempBitmap.Release();

            this.basicColorsCache = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(this.basicColorsCache))
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(tempBitmap, 0, 0, width, height);
            }
        }

        /// <summary>
        /// Draws the shades of a color with the circle around a certain x, y position.
        /// </summary>
        /// <param name="x">The x coordinate to select.</param>
        /// <param name="y">The y coordinate to select.</param>
        /// <returns>The color at x, y.</returns>
        private RomColor DrawShadesBitmap(int x, int y)
        {
            this.selectedShadeLocation = new Point(x, y);
            RomColor selectedShadeColor = (RomColor)this.shadesCache.GetPixel(x, y);

            this.shadesBitmap.Dispose();
            this.shadesBitmap = this.shadesCache.Clone() as Bitmap;

            using (Graphics g = Graphics.FromImage(this.shadesBitmap))
            using (Pen pen = new Pen(this.GetPenColor(x, y)))
            {
                g.DrawEllipse(pen, ColorPicker.GetSelectionBounds(x, y));
            }

            return selectedShadeColor;
        }

        /// <summary>
        /// Draws the shades of a color with the circle around a certain color.
        /// This is used to find a color that user might have entered manually.
        /// It does not always draw the ellipse as not all colors are necessarily displayed.
        /// </summary>
        /// <param name="color">The color to draw a circle around.</param>
        private void DrawShadesBitmap(RomColor color)
        {
            this.shadesBitmap.Dispose();
            this.shadesBitmap = this.shadesCache.Clone() as Bitmap;

            for (int x = 0; x < this.shadesSize.Width; x++)
            {
                for (int y = 0; y < this.shadesSize.Height; y++)
                {
                    RomColor selectedShadeColor = (RomColor)this.shadesBitmap.GetPixel(x, y);
                    if (selectedShadeColor == color)
                    {
                        this.selectedShadeLocation = new Point(x, y);
                        using (Graphics g = Graphics.FromImage(this.shadesBitmap))
                        using (Pen pen = new Pen(GetPenColor(x, y)))
                        {
                            g.DrawEllipse(pen, ColorPicker.GetSelectionBounds(x, y));
                        }
                        return;
                    }
                }
            }

            this.selectedShadeLocation = new Point(63, 0);
        }

        /// <summary>
        /// Gets the color at a specific location.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The color.</returns>
        private RomColor GetPenColor(int x, int y)
        {
            RomColor color;
            if (y > 63)
            {
                color = ((RomColor)this.shadesBitmap.GetPixel(x, 127 - y)).Opposite();
            }
            else
            {
                color = ((RomColor)this.shadesBitmap.GetPixel(x, y)).Opposite();
            }
            return color;
        }

        /// <summary>
        /// Initializes the shadesCache member.
        /// </summary>
        private void InitShadesCache()
        {
            this.shadesCache.Dispose();
            this.shadesCache = new Bitmap(128, 128, PixelFormat.Format32bppPArgb);
            int index, index2;

            using (Graphics g = Graphics.FromImage(this.shadesCache))
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                #region Gray colors

                // Generate the grays from black to white, these are at the bottom of the square, left to right
                RomColor[] grays = new RomColor[64];
                IEnumerator<RomColor> graysIte = RomColor.From5BitRgb(0, 0, 0).GetEnumerator(RomColor.From5BitRgb(31, 31, 31), 64);
                index = 0;
                while (graysIte.MoveNext())
                {
                    grays[index] = graysIte.Current.To5Bit();
                    index++;
                }

                #endregion Gray colors

                // Draw from black (top left) to our selected color (in the middle at the top)
                IEnumerator<RomColor> colorsIte = RomColor.From5BitRgb(0, 0, 0).GetEnumerator(this.selectedBasicColor, 32);
                index = 0;
                while (colorsIte.MoveNext())
                {
                    IEnumerator<RomColor> toGrayIte = colorsIte.Current.To5Bit().GetEnumerator(grays[index], 64);
                    index2 = 0;
                    // Draw the vertical colors that goes from our shade (our color to black) to the gray variation at the bottom
                    while (toGrayIte.MoveNext())
                    {
                        brush.Color = toGrayIte.Current.To5Bit();
                        g.FillRectangle(brush, (int)(index * 2), (int)(index2 * 2), 2, 2);
                        index2++;
                    }

                    index++;
                }

                // Draw from white (top right) to our selected color (in the middle at the top)
                colorsIte = this.selectedBasicColor.GetEnumerator(RomColor.From5BitRgb(31, 31, 31), 32);
                index = 0;
                while (colorsIte.MoveNext())
                {
                    IEnumerator<RomColor> toGrayIte = colorsIte.Current.To5Bit().GetEnumerator(grays[index + 32], 64);
                    index2 = 0;
                    // Draw the vertical colors that goes from our shade (our color to white) to the gray variation at the bottom
                    while (toGrayIte.MoveNext())
                    {
                        brush.Color = toGrayIte.Current.To5Bit();
                        g.FillRectangle(brush, (int)((index + 32) * 2), (int)(index2 * 2), 2, 2);
                        index2++;
                    }

                    index++;
                }
            }
        }

        /// <summary>
        /// Updates the old color.
        /// </summary>
        /// <param name="color">The color.</param>
        private void UpdateOldColor(RomColor color)
        {
            this.oldColor = color;
            this.oldColorPictureBox.BackColor = color;
            ColorPicker.SetToolTip(this.oldColorToolTip, this.oldColorPictureBox, color);
        }

        /// <summary>
        /// Updates the new color.
        /// </summary>
        /// <param name="color">The color.</param>
        private void UpdateNewColor(RomColor color)
        {
            this.newColor = color;
            this.newColorPictureBox.BackColor = color;
            ColorPicker.SetToolTip(this.newColorToolTip, this.newColorPictureBox, color);
        }

        #endregion Bitmap drawing

        #region Events handlers

        /// <summary>
        /// Catches when the user moves in the basic colors while clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicColorsPictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.BasicColorsClicked(e.X);
            }
        }

        /// <summary>
        /// Catches a user click in the basic colors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicColorsPictureBoxClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs)
            {
                MouseEventArgs me = e as MouseEventArgs;
                this.BasicColorsClicked(me.X);
            }
        }

        /// <summary>
        /// Redraws shades and new color based on the location of the click in the basic colors.
        /// </summary>
        /// <param name="x">Location of the click.</param>
        private void BasicColorsClicked(int x)
        {
            // Make sure we are not out of bounds
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= this.basicColorsSize.Width)
            {
                x = this.basicColorsSize.Width - 1;
            }

            this.InvalidateBasicColorsSelection();
            // Redraw all pieces, basic colors, shades and new color
            this.selectedBasicColor = this.DrawBasicColorsBitmap(x);
            this.InvalidateBasicColorsSelection();
            this.basicColorsPictureBox.Update();

            this.InitShadesCache();
            RomColor shadeColor = this.DrawShadesBitmap(this.selectedShadeLocation.X, this.selectedShadeLocation.Y);
            this.shadesPictureBox.Refresh();

            this.performEvents = false;

            this.redNumericUpDown.Value = shadeColor.Red5Bit;
            this.greenNumericUpDown.Value = shadeColor.Green5Bit;
            this.blueNumericUpDown.Value = shadeColor.Blue5Bit;

            this.performEvents = true;

            this.UpdateNewColor(shadeColor);
            this.newColorPictureBox.Refresh();
        }

        /// <summary>
        /// Catches when the user moves in the shades while clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShadesPictureBoxMouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                this.ShadesClicked(e.X, e.Y);
            }
        }

        /// <summary>
        /// Catches a user click in the shades.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShadesPictureBoxClick(object sender, EventArgs e)
        {
            if (e is MouseEventArgs)
            {
                MouseEventArgs me = e as MouseEventArgs;
                this.ShadesClicked(me.X, me.Y);
            }
        }

        private void ShadesClicked(int x, int y)
        {
            // Make sure we are not out of bounds
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= this.shadesBitmap.Width)
            {
                x = this.shadesBitmap.Width - 1;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y >= this.shadesBitmap.Height)
            {
                y = this.shadesBitmap.Height - 1;
            }

            this.InvalidateShadesSelection();
            // Redraw shades and new color
            RomColor shadeColor = this.DrawShadesBitmap(x, y);
            this.InvalidateShadesSelection();
            this.shadesPictureBox.Update();

            this.performEvents = false;

            this.redNumericUpDown.Value = shadeColor.Red5Bit;
            this.greenNumericUpDown.Value = shadeColor.Green5Bit;
            this.blueNumericUpDown.Value = shadeColor.Blue5Bit;

            this.performEvents = true;

            this.UpdateNewColor(shadeColor);
            this.newColorPictureBox.Refresh();
        }

        /// <summary>
        /// Catches the user changing numbers for the color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RgbValueChanged(object sender, EventArgs e)
        {
            if (this.performEvents)
            {
                RomColor color = RomColor.From5BitRgb((int)redNumericUpDown.Value, (int)greenNumericUpDown.Value, (int)blueNumericUpDown.Value);
                this.SetNewColor(color);
            }
        }

        /// <summary>
        /// Redraws the old color.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OldColorPictureBoxClick(object sender, EventArgs e)
        {
            this.SetNewColor(this.oldColor);
        }

        #region Paint

        private void BasicColorsPictureBoxPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.basicColorsBitmap, 0, 0);
        }

        private void ShadesPictureBoxPaint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.shadesBitmap, 0, 0);
        }

        #endregion Paint

        #endregion Events handlers

        #region Find colors

        /// <summary>
        /// Finds the basic color of any RomColor.
        /// A basic color always has one of the three components equal to 255 (8 bit).
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static RomColor FindBasicColor(RomColor color)
        {
            byte max = Math.Max(color.Red, color.Green);
            max = Math.Max(max, color.Blue);
            if (max == 0) // If the color passed is black, then this color will be found in any shade, pass the default red.
            {
                return Color.Red;
            }

            byte min = Math.Min(color.Red, color.Green);
            min = Math.Min(min, color.Blue);
            if (min == 255) // If the color passed is white, then this color will be found in any shade, pass the default red.
            {
                return Color.Red;
            }

            // Remove the smallest component as the basic colors only ever contain two of the components
            // This is done in the reverse order so that it makes more sense to the user when playing with numbers on the dialog
            if (color.Blue == min)
            {
                color.Blue = 0;
            }
            else if (color.Green == min)
            {
                color.Green = 0;
            }
            else if (color.Red == min)
            {
                color.Red = 0;
            }

            float multiplier = 255f / (float)max;

            float red = color.Red * multiplier;
            float green = color.Green * multiplier;
            float blue = color.Blue * multiplier;

            return RomColor.From8BitRgb((byte)red, (byte)green, (byte)blue).To5Bit();
        }

        private int FindColorIndex(RomColor color)
        {
            for (int x = 0; x < this.basicColorsSize.Width; x++)
            {
                RomColor selectedColor = (RomColor)this.basicColorsCache.GetPixel(x, 0);
                if (selectedColor == color)
                {
                    return x;
                }
            }

            return -1;
        }

        #endregion Find colors
    }
}
