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
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.UI.Tools;
using System.ComponentModel;
using System.Drawing;

namespace EpicEdit.UI.TrackEdition
{
    internal sealed class TrackPanel : TilePanel
    {
        [Browsable(false), DefaultValue(typeof(Track), "")]
        public Track Track { get; set; }

        [Browsable(false), DefaultValue(typeof(EditionMode), "Tileset")]
        public EditionMode EditionMode { get; set; }

        [Browsable(false), DefaultValue(typeof(int), "0")]
        public int ScrollPositionX { get; set; }

        [Browsable(false), DefaultValue(typeof(int), "0")]
        public int ScrollPositionY { get; set; }

        protected override void GetColorAt(int x, int y, out Palette palette, out int colorIndex)
        {
            if (EditionMode == EditionMode.Objects)
            {
                if (Track is GPTrack gpTrack && gpTrack.Objects.Routine != TrackObjectType.Pillar)
                {
                    // Shift the X value, to account for the fact that track objects,
                    // which are rendered on 2x2 tiles (16x16 pixels) are spread across 3 tiles
                    // horizontally, as they are centered (4px + 8px + 4px).
                    // So, shifting the X value makes it easier for us, as that lets us treat
                    // them as being on 2 tiles rather than 3.
                    var halfTile = (int)((Tile.Size / 2) * Zoom);
                    x += halfTile;
                }
            }

            base.GetColorAt(x, y, out palette, out colorIndex);
        }

        protected override Tile GetTileAt(int x, int y)
        {
            // Convert from pixel precision to tile precision
            var tileX = x / Tile.Size;
            var tileY = y / Tile.Size;

            tileX += ScrollPositionX;
            tileY += ScrollPositionY;

            if (tileX > TrackMap.Size || tileY > TrackMap.Limit)
            {
                // Allow tileX value to be over the TrackMap.Limit for
                // EditionMode.Objects + GP track + object type different from pillar,
                // due to the fact we shifted the X value in the GetColorAt method override.
                return null;
            }

            if (EditionMode == EditionMode.Objects)
            {
                if (Track is GPTrack gpTrack && gpTrack.Objects.Routine != TrackObjectType.Pillar)
                {
                    return GetObjectTile(gpTrack, x, y, tileX, tileY);
                }
            }

            if (tileX == TrackMap.Size)
            {
                // Not EditionMode.Objects + GP track + object type different from pillar,
                // so check that tileX is not over TrackMap.Limit (ie: equal to TrackMap.Size)
                return null;
            }

            if (EditionMode == EditionMode.Overlay)
            {
                var tile = GetOverlayTile(Track, tileX, tileY);

                if (tile != null)
                {
                    return tile;
                }
            }

            var index = Track.Map[tileX, tileY];

            return Track.RoadTileset[index];
        }

        private static Tile GetObjectTile(GPTrack track, int x, int y, int tileX, int tileY)
        {
            var objects = track.Objects;

            foreach (var obj in objects)
            {
                // Since objects are rendered on 2x2 tiles,
                // add or substract 1 to account for this.
                if ((tileX == obj.X || tileX == obj.X + 1) &&
                    (tileY == obj.Y - 1 || tileY == obj.Y))
                {
                    var relativeX = tileX - obj.X;
                    var relativeY = tileY - obj.Y + 1;

                    var tile = Context.Game.ObjectGraphics.GetTile(track, obj, relativeX, relativeY);

                    var pixelX = x % Tile.Size;
                    var pixelY = y % Tile.Size;

                    if (tile.GetColorIndexAt(pixelX, pixelY) != 0)
                    {
                        // If the hovered pixel is not transparent, return the hovered tile
                        return tile;
                    }
                }
            }

            return null;
        }

        private static Tile GetOverlayTile(Track track, int tileX, int tileY)
        {
            var tileset = track.RoadTileset;
            var location = new Point(tileX, tileY);

            var overlay = track.OverlayTiles;
            for (var i = overlay.Count - 1; i >= 0; i--)
            {
                var overlayTile = overlay[i];
                if (overlayTile.IntersectsWith(location))
                {
                    var relativeX = tileX - overlayTile.X;
                    var relativeY = tileY - overlayTile.Y;
                    var tileId = overlayTile.Pattern[relativeX, relativeY];

                    if (tileId != OverlayTile.None)
                    {
                        return tileset[tileId];
                    }
                }
            }

            return null;
        }
    }
}
