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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.Items;
using EpicEdit.UI.Tools;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace EpicEdit.UI.SettingEdition
{
    internal sealed class ItemIconPanel : TilePanel
    {
        private Theme _theme;

        [Browsable(false), DefaultValue(typeof(Theme), "")]
        public Theme Theme
        {
            get => _theme;
            set
            {
                _theme = value;
                UpdateImage();
            }
        }

        public ItemIconPanel()
        {
            if (DesignMode)
            {
                // Avoid exceptions in design mode
                _image = new Bitmap(1, 1, PixelFormat.Format32bppPArgb);
                _currentImage = _image;
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
        private Image _currentImage;

        private Image _image;
        private Image _disabledImage;

        private Image Image
        {
            //get => this.image;
            set
            {
                if (_image != null)
                {
                    _image.Dispose();
                }

                _image = value;

                if (_disabledImage != null)
                {
                    _disabledImage.Dispose();
                    _disabledImage = null;
                }

                SetCurrentImage();
            }
        }

        private bool _looksEnabled = true;

        /// <summary>
        /// Gets or sets value that specifies whether the control looks enabled or not.
        /// This enables mouse events (color picking) even if the control looks disabled.
        /// </summary>
        [Browsable(true), Category("Appearance"), DefaultValue(typeof(bool), "True")]
        public bool LooksEnabled
        {
            //get => this.looksEnabled;
            set
            {
                _looksEnabled = value;
                SetCurrentImage();
            }
        }

        public ItemType ItemType { get; set; }

        private void CreateImage()
        {
            var palettes = Theme.Palettes;
            Image = Context.Game.ItemIconGraphics.GetImage(ItemType, palettes);
        }

        public void UpdateImage()
        {
            CreateImage();
            Refresh();
        }

        private void SetCurrentImage()
        {
            if (_image == null)
            {
                return;
            }

            if (_looksEnabled)
            {
                _currentImage = _image;
            }
            else
            {
                if (_disabledImage == null)
                {
                    _disabledImage = new Bitmap(16, 16, PixelFormat.Format32bppPArgb);
                    using (var g = Graphics.FromImage(_disabledImage))
                    using (var image = ToolStripRenderer.CreateDisabledImage(_image))
                    {
                        g.Clear(SystemColors.Control);
                        g.DrawImage(image, 0, 0);
                    }
                }
                _currentImage = _disabledImage;
            }

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_currentImage, 0, 0);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            x /= Tile.Size;
            y /= Tile.Size;

            return Context.Game.ItemIconGraphics.GetTile(ItemType, x, y);
        }

        protected override void Dispose(bool disposing)
        {
            if (_image != null)
            {
                _image.Dispose();
            }

            if (_disabledImage != null)
            {
                _disabledImage.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
