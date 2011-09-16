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
using System.ComponentModel;
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
        [Browsable(true)]
        public event EventHandler<EventArgs> ColorChanged;

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
        private RomColor selectedBasicColor = RomColor.From5BitRgb(31, 1, 0);

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

        // The selected color.
        private Color selectedColor;

        /// <summary>
        /// Used to prevent loops when certain clicks are performed in different UI controls.
        /// </summary>
        private bool performEvents = false;

        #endregion Private members

        /// <summary>
        /// Gets or sets the selected color.
        /// </summary>
        /// <returns>The color.</returns>
        public RomColor SelectedColor
        {
            get { return this.selectedColor; }
            set { this.SetColor(value); }
        }

        public ColorPicker()
        {
            this.InitializeComponent();

            this.basicColorsSize = this.basicColorsPictureBox.ClientSize;
            this.shadesSize = this.shadesPictureBox.ClientSize;

            // Initializing Bitmaps in order to avoid having to check if they're null before disposal
            this.basicColorsBitmap = this.shadesBitmap = this.shadesCache = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);

            this.InitBasicColorsBitmapCache();

            int x = 0;
            RomColor basicColor = this.basicColorsCache.GetPixel(x, 0);
            this.SetColor(x, basicColor, basicColor);
        }

        /// <summary>
        /// Sets the new color.
        /// </summary>
        /// <param name="x">X position in the basic colors.</param>
        private void SetColor(int x)
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
            this.selectedBasicColor = this.DrawBasicColorsBitmap(x);
            this.InvalidateBasicColorsSelection();
            this.basicColorsPictureBox.Refresh();

            this.InitShadesCache();
            RomColor shadeColor = this.DrawShadesBitmap(this.selectedShadeLocation.X, this.selectedShadeLocation.Y);
            this.shadesPictureBox.Refresh();

            this.SetColorSub(shadeColor);
        }

        /// <summary>
        /// Sets the new color.
        /// Automatically detects where the color exists in the shades and basic colors.
        /// </summary>
        /// <param name="color">The color.</param>
        private void SetColor(RomColor color)
        {
            color = color.To5Bit();
            RomColor basicColor = ColorPicker.FindBasicColor(color);
            int x = this.FindColorIndex(basicColor);
            this.SetColor(x, basicColor, color);
        }

        private void SetColor(int x, RomColor basicColor, RomColor color)
        {
            // TODO: The logic to find the location of the color in the basic and shades needs to be revamped, often colors are not found.
            // This is possibly due to the fact that not every shade is displayed. FindBasicColor may possibly be flawed as well.

            if (this.selectedBasicColor != basicColor)
            {
                this.InvalidateBasicColorsSelection();
                this.selectedBasicColor = basicColor;
                this.DrawBasicColorsBitmap(x);
                this.InvalidateBasicColorsSelection();

                this.InitShadesCache();
            }

            this.DrawShadesBitmap(color);
            this.shadesPictureBox.Refresh();

            this.SetColorSub(color);
        }

        private void SetColorSub(RomColor color)
        {
            this.selectedColor = color;

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
            RomColor color = (RomColor)this.basicColorsCache.GetPixel(x, 0);
            this.DrawBasicColorsBitmap(color, x);
            return color;
        }

        /// <summary>
        /// Draws the basic colors with the circle around a certain color.
        /// </summary>
        /// <param name="color">The color to select.</param>
        private void DrawBasicColorsBitmap(RomColor color)
        {
            int x = this.FindColorIndex(color);
            this.DrawBasicColorsBitmap(color, x);
        }

        /// <summary>
        /// Draws the basic colors with the circle around a certain color.
        /// </summary>
        /// <param name="color">The color to select.</param>
        /// <param name="x">The color location.</param>
        private void DrawBasicColorsBitmap(RomColor color, int x)
        {
            this.basicColorsBitmap.Dispose();
            this.basicColorsBitmap = this.basicColorsCache.Clone() as Bitmap;

            using (Graphics g = Graphics.FromImage(this.basicColorsBitmap))
            using (Pen pen = new Pen(color.Opposite()))
            {
                int y = this.basicColorsSize.Height / 2;
                g.DrawEllipse(pen, ColorPicker.GetSelectionBounds(x, y));
            }
        }

        /// <summary>
        /// Initializes the basicColorsCache member.
        /// </summary>
        private void InitBasicColorsBitmapCache()
        {
            int width = this.basicColorsSize.Width;
            int height = this.basicColorsSize.Height;

            using (Bitmap tempBitmap = new Bitmap(width, 1, PixelFormat.Format32bppPArgb))
            {
                FastBitmap fTempBitmap = new FastBitmap(tempBitmap);

                // Red to yellow
                for (byte index = 0; index <= 30; index++) // Skip the last one (31) because it is the same value as the first one of the next loop
                {
                    Color color = RomColor.From5BitRgb(31, index, 0);
                    fTempBitmap.SetPixel(index, 0, color);
                }

                // Yellow to green
                for (byte index = 31; index >= 1; index--) // Skip the last one (0) because it is the same value as the first one of the next loop
                {
                    Color color = RomColor.From5BitRgb(index, 31, 0);
                    fTempBitmap.SetPixel((31 - index) + 31, 0, color);
                }

                // Green to cyan
                for (byte index = 0; index <= 30; index++) // Skip the last one (31) because it is the same value as the first one of the next loop
                {
                    Color color = RomColor.From5BitRgb(0, 31, index);
                    fTempBitmap.SetPixel(index + 62, 0, color);
                }

                // Cyan to blue
                for (byte index = 31; index >= 1; index--) // Skip the last one (0) because it is the same value as the first one of the next loop
                {
                    Color color = RomColor.From5BitRgb(0, index, 31);
                    fTempBitmap.SetPixel((31 - index) + 93, 0, color);
                }

                // Blue to purple
                for (byte index = 0; index <= 30; index++) // Skip the last one (31) because it is the same value as the first one of the next loop
                {
                    Color color = RomColor.From5BitRgb(index, 0, 31);
                    fTempBitmap.SetPixel(index + 124, 0, color);
                }

                // Purple to red
                for (byte index = 31; index >= 1; index--) // Skip the last one (0) because it is the same value as the first one of the first loop
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
            RomColor selectedColor = (RomColor)this.shadesCache.GetPixel(x, y);

            this.shadesBitmap.Dispose();
            this.shadesBitmap = this.shadesCache.Clone() as Bitmap;

            using (Graphics g = Graphics.FromImage(this.shadesBitmap))
            using (Pen pen = new Pen(this.GetPenColor(x, y)))
            {
                g.DrawEllipse(pen, ColorPicker.GetSelectionBounds(x, y));
            }

            return selectedColor;
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

            FastBitmap fShadesBitmap = new FastBitmap(this.shadesBitmap);

            // The shades image is scaled by a factor of 2, so skip every other pixel
            for (int y = 0; y < this.shadesSize.Height; y += 2)
            {
                for (int x = 0; x < this.shadesSize.Width; x += 2)
                {
                    RomColor selectedColor = (RomColor)fShadesBitmap.GetPixel(x, y);
                    if (selectedColor == color)
                    {
                        fShadesBitmap.Release();
                        this.selectedShadeLocation = new Point(x, y);
                        using (Graphics g = Graphics.FromImage(this.shadesBitmap))
                        using (Pen pen = new Pen(this.GetPenColor(x, y)))
                        {
                            g.DrawEllipse(pen, ColorPicker.GetSelectionBounds(x, y));
                        }
                        return;
                    }
                }
            }

            fShadesBitmap.Release();
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
            if (y > 63)
            {
                // Optimize contrast
                y = 127 - y;
            }

            return ((RomColor)this.shadesCache.GetPixel(x, y)).Opposite();
        }

        /// <summary>
        /// Initializes the shadesCache member.
        /// </summary>
        private void InitShadesCache()
        {
            int width = this.shadesSize.Width;
            int height = this.shadesSize.Height;
            int size = width / 2; // Unscaled image size
            int halfSize = size / 2;
            int index, index2;

            // Generate the grays from black to white, these are at the bottom of the square, left to right
            RomColor[] grays = new RomColor[size];
            IEnumerator<RomColor> graysIte = RomColor.From5BitRgb(0, 0, 0).GetEnumerator(RomColor.From5BitRgb(31, 31, 31), size);
            index = 0;
            while (graysIte.MoveNext())
            {
                grays[index] = graysIte.Current.To5Bit();
                index++;
            }

            using (Bitmap tempBitmap = new Bitmap(size, size, PixelFormat.Format32bppPArgb))
            {
                FastBitmap fTempBitmap = new FastBitmap(tempBitmap);

                // Draw from black (top left) to our selected color (in the middle at the top)
                IEnumerator<RomColor> colorsIte = RomColor.From5BitRgb(0, 0, 0).GetEnumerator(this.selectedBasicColor, halfSize);
                index = 0;
                while (colorsIte.MoveNext())
                {
                    IEnumerator<RomColor> toGrayIte = colorsIte.Current.To5Bit().GetEnumerator(grays[index], size);
                    index2 = 0;
                    // Draw the vertical colors that goes from our shade (our color to black) to the gray variation at the bottom
                    while (toGrayIte.MoveNext())
                    {
                        RomColor color = toGrayIte.Current.To5Bit();
                        fTempBitmap.SetPixel(index, index2, color);
                        index2++;
                    }

                    index++;
                }

                // Draw from white (top right) to our selected color (in the middle at the top)
                colorsIte = this.selectedBasicColor.GetEnumerator(RomColor.From5BitRgb(31, 31, 31), halfSize);
                index = 0;
                while (colorsIte.MoveNext())
                {
                    IEnumerator<RomColor> toGrayIte = colorsIte.Current.To5Bit().GetEnumerator(grays[index + halfSize], size);
                    index2 = 0;
                    // Draw the vertical colors that goes from our shade (our color to white) to the gray variation at the bottom
                    while (toGrayIte.MoveNext())
                    {
                        RomColor color = toGrayIte.Current.To5Bit();
                        fTempBitmap.SetPixel(index + halfSize, index2, color);
                        index2++;
                    }

                    index++;
                }

                fTempBitmap.Release();

                this.shadesCache.Dispose();
                this.shadesCache = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
                using (Graphics g = Graphics.FromImage(this.shadesCache))
                {
                    g.PixelOffsetMode = PixelOffsetMode.Half;
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(tempBitmap, 0, 0, width, height);
                }
            }
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
            this.SetColor(x);

            this.ColorChanged(this, EventArgs.Empty);
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
            RomColor color = this.DrawShadesBitmap(x, y);
            this.InvalidateShadesSelection();
            this.shadesPictureBox.Refresh();

            this.SetColorSub(color);

            this.ColorChanged(this, EventArgs.Empty);
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
                RomColor color = RomColor.From5BitRgb((byte)redNumericUpDown.Value, (byte)greenNumericUpDown.Value, (byte)blueNumericUpDown.Value);
                this.SetColor(color);
            }
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

            color.Red = (byte)(color.Red * multiplier);
            color.Green = (byte)(color.Green * multiplier);
            color.Blue = (byte)(color.Blue * multiplier);

            return color.To5Bit();
        }

        private int FindColorIndex(RomColor color)
        {
            FastBitmap fBasicColors = new FastBitmap(this.basicColorsCache);
            for (int x = 0; x < this.basicColorsSize.Width; x++)
            {
                RomColor selectedColor = (RomColor)fBasicColors.GetPixel(x, 0);
                if (selectedColor == color)
                {
                    fBasicColors.Release();
                    return x;
                }
            }

            fBasicColors.Release();
            throw new ArgumentException("Invalid basic color.", "color");
        }

        #endregion Find colors
    }
}
