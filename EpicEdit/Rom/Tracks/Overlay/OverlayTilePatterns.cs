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

using EpicEdit.Rom.Utility;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EpicEdit.Rom.Tracks.Overlay
{
    /// <summary>
    /// Collection of all the overlay tile patterns in the game.
    /// </summary>
    internal class OverlayTilePatterns : ICollection<OverlayTilePattern>
    {
        private const int PatternCount = 56;

        private readonly OverlayTilePattern[] patterns;

        public OverlayTilePattern this[int index] => this.patterns[index];

        public bool Modified
        {
            get
            {
                foreach (OverlayTilePattern pattern in this.patterns)
                {
                    if (pattern.Modified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public OverlayTilePatterns(byte[] romBuffer, Offsets offsets, OverlayTileSizes sizes)
        {
            this.patterns = new OverlayTilePattern[PatternCount];
            this.LoadPatterns(romBuffer, offsets, sizes);
        }

        private void LoadPatterns(byte[] romBuffer, Offsets offsets, OverlayTileSizes sizes)
        {
            // Get the addresses where the individual pattern data is
            int addressOffset = offsets[Offset.TrackOverlayPatterns];
            int[] dataAddresses = this.LoadPatternDataAddresses(romBuffer, addressOffset);

            // Get the data lengths of all the patterns
            int sizeOffset = offsets[Offset.TrackOverlaySizes];
            int[] dataLengths = this.LoadPatternDataLengths(dataAddresses, sizeOffset);

            // Get the widths and heights of all the patterns
            OverlayTileSize[] overlayTilesizes = this.LoadPatternSizes(sizes);

            for (int i = 0; i < PatternCount; i++)
            {
                byte[] data = Utilities.ReadBlock(romBuffer, dataAddresses[i], dataLengths[i]);
                this.patterns[i] = new OverlayTilePattern(data, overlayTilesizes[i]);
            }
        }

        private int[] LoadPatternDataAddresses(byte[] romBuffer, int offset)
        {
            int[] addresses = new int[this.Count];
            byte[][] data = Utilities.ReadBlockGroup(romBuffer, offset, 2, this.Count);
            for (int i = 0; i < data.Length; i++)
            {
                addresses[i] = (data[i][1] << 8) + data[i][0] + 0x40000;
            }
            return addresses;
        }

        private int[] LoadPatternDataLengths(int[] dataAddresses, int offset)
        {
            // TODO: This data must be somewhere in the ROM, but for now, let's just do a substraction between the addresses, they are ordered in the ROM anyways.
            // This also assumes the entire data area is used, which is why this code should be changed eventually when we start modifying these patterns.
            // From the documentation the games loads up 32 bytes into VRAM starting at one of these data addresses when the overlay is required.
            // This means that quite possibly there is no data in the ROM about the lengths of these items, the overlay tile map of the track tells 
            // the engine how many bytes to use.
            int[] lengths = new int[this.Count];
            for (int i = 0; i < dataAddresses.Length; i++)
            {
                int diff = 0;
                for (int j = i + 1; j < dataAddresses.Length; j++)
                {
                    if (dataAddresses[j] > dataAddresses[i])
                    {
                        diff = dataAddresses[j] - dataAddresses[i];
                        break;
                    }
                }
                if (diff == 0)
                {
                    // A bit of a hack, the overlay tile sizes follow the pattern data in the ROM
                    diff = offset - dataAddresses[i];
                }
                lengths[i] = diff;
            }

            return lengths;
        }

        private OverlayTileSize[] LoadPatternSizes(OverlayTileSizes sizes)
        {
            // TODO: Get this from the ROM, if even possible.
            // It is unlikely that this information is available anywhere in the ROM since
            // each overlay tile indicates which size to use when displayed at runtime.
            // Maybe the size of each pattern should be saved in the epic zone when we start supporting updating the patterns.
            byte[] sizeIndexes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1 };
            OverlayTileSize[] sizeArray = new OverlayTileSize[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                sizeArray[i] = sizes[sizeIndexes[i]];
            }
            return sizeArray;
        }

        public int IndexOf(OverlayTilePattern pattern)
        {
            for (int i = 0; i < this.patterns.Length; i++)
            {
                if (this.patterns[i] == pattern)
                {
                    return i;
                }
            }

            throw new ArgumentException("Pattern not found.", nameof(pattern));
        }

        #region ICollection

        public IEnumerator<OverlayTilePattern> GetEnumerator()
        {
            foreach (OverlayTilePattern pattern in this.patterns)
            {
                yield return pattern;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.patterns.GetEnumerator();
        }

        public int Count => this.patterns.Length;

        public bool IsReadOnly => true;

        public void Add(OverlayTilePattern item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(OverlayTilePattern item)
        {
            foreach (OverlayTilePattern pattern in this.patterns)
            {
                if (pattern.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(OverlayTilePattern[] array, int arrayIndex)
        {
            Array.Copy(this.patterns, 0, array, arrayIndex, this.Count);
        }

        public bool Remove(OverlayTilePattern item)
        {
            throw new NotImplementedException();
        }

        #endregion ICollection
    }
}
