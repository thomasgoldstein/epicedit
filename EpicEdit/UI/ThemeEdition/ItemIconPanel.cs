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
using System.Drawing.Imaging;
using System.Windows.Forms;

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.UI.Tools;

namespace EpicEdit.UI.ThemeEdition
{
    internal sealed class ItemIconPanel : TilePanel
    {
        private Theme theme;

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get { return this.theme; }
            set
            {
                this.theme = value;
                this.UpdateImage();
            }
        }

        public ItemIconPanel()
        {
            if (this.DesignMode)
            {
                // Avoid exceptions in design mode
                this.image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
                this.currentImage = this.image;
            }
        }

        private new bool DesignMode
        {
            get
            {
                if (base.DesignMode)
                {
                    return true;
                }

                return LicenseManager.UsageMode == LicenseUsageMode.Designtime;
            }
        }

        /// <summary>
        /// Reference to the current image (enabled or disabled).
        /// </summary>
        private Image currentImage;

        private Image image;
        private Image disabledImage;

        private Image Image
        {
            //get { return this.image; }
            set
            {
                if (this.image != null)
                {
                    this.image.Dispose();
                }

                this.image = value;

                if (this.disabledImage != null)
                {
                    this.disabledImage.Dispose();
                    this.disabledImage = null;
                }

                this.SetCurrentImage();
            }
        }

        private bool looksEnabled = true;

        /// <summary>
        /// Gets or sets value that specifies whether the control looks enabled or not.
        /// This enables mouse events (color picking) even if the control looks disabled.
        /// </summary>
        [Browsable(true), DefaultValue(typeof(bool), "true")]
        public bool LooksEnabled
        {
            //get { return this.looksEnabled; }
            set
            {
                this.looksEnabled = value;
                this.SetCurrentImage();
            }
        }

        private ItemType itemType;
        public ItemType ItemType
        {
            get { return this.itemType; } // Needed for the IDE designer
            set { this.itemType = value; }
        }

        private void CreateImage()
        {
            Palettes palettes = this.Theme.Palettes;
            this.Image = Context.Game.ItemIconGraphics.GetImage(this.itemType, palettes);
        }

        public void UpdateImage()
        {
            this.CreateImage();
            this.Refresh();
        }

        private void SetCurrentImage()
        {
            if (this.image == null)
            {
                return;
            }

            if (this.looksEnabled)
            {
                this.currentImage = this.image;
            }
            else
            {
                if (this.disabledImage == null)
                {
                    this.disabledImage = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
                    using (Graphics g = Graphics.FromImage(this.disabledImage))
                    using (Image image = ToolStripRenderer.CreateDisabledImage(this.image))
                    {
                        g.Clear(SystemColors.Control);
                        g.DrawImage(image, 0, 0);
                    }
                }
                this.currentImage = this.disabledImage;
            }

            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.currentImage, 0, 0);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            return Context.Game.ItemIconGraphics.GetTile(this.itemType, x, y);
        }

        protected override void Dispose(bool disposing)
        {
            if (this.image != null)
            {
                this.image.Dispose();
            }

            if (this.disabledImage != null)
            {
                this.disabledImage.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
