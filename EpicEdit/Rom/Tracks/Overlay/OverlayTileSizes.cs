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
using System.Collections;
using System.Collections.Generic;

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// Collection of all the overlay tile sizes in the game.
    /// </summary>
    internal class OverlayTileSizes : IEnumerable<OverlayTileSize>
    {
        private OverlayTileSize[] sizes;

        public OverlayTileSize this[int index]
        {
            get { return this.sizes[index]; }
        }

        public int Count
        {
            get { return this.sizes.Length; }
        }

        public bool Modified
        {
            get
            {
                foreach (OverlayTileSize size in this.sizes)
                {
                    if (size.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public OverlayTileSizes(byte[] romBuffer, int offset)
        {
            this.LoadSizes(romBuffer, offset);
        }

        private void LoadSizes(byte[] romBuffer, int offset)
        {
            // There are only ever 4 sizes in the game
            this.sizes = new OverlayTileSize[4];
            byte[][] data = Utilities.ReadBlockGroup(romBuffer, offset, 2, 4);
            for (int i = 0; i < data.Length; i++)
            {
                this.sizes[i] = new OverlayTileSize(data[i]);
            }
        }

        public void Save(byte[] romBuffer, int offset)
        {
            for (int i = 0; i < this.sizes.Length; i++)
            {
                this.sizes[i].Save(romBuffer, offset, i);
            }
        }

        public IEnumerator<OverlayTileSize> GetEnumerator()
        {
            foreach (OverlayTileSize size in this.sizes)
            {
                yield return size;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.sizes.GetEnumerator();
        }

        public int IndexOf(OverlayTileSize size)
        {
            for (int i = 0; i < this.sizes.Length; i++)
            {
                if (this.sizes[i] == size)
                {
                    return i;
                }
            }

            throw new MissingMemberException("Size not found.");
        }
    }
}
