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

        private readonly OverlayTilePattern[] _patterns;

        public OverlayTilePattern this[int index] => _patterns[index];

        public bool Modified
        {
            get
            {
                foreach (var pattern in _patterns)
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
            _patterns = new OverlayTilePattern[PatternCount];
            LoadPatterns(romBuffer, offsets, sizes);
        }

        private void LoadPatterns(byte[] romBuffer, Offsets offsets, OverlayTileSizes sizes)
        {
            // Get the addresses where the individual pattern data is
            var addressOffset = offsets[Offset.TrackOverlayPatterns];
            var dataAddresses = LoadPatternDataAddresses(romBuffer, addressOffset);

            // Get the data lengths of all the patterns
            var sizeOffset = offsets[Offset.TrackOverlaySizes];
            var dataLengths = LoadPatternDataLengths(dataAddresses, sizeOffset);

            // Get the widths and heights of all the patterns
            var overlayTilesizes = LoadPatternSizes(sizes);

            for (var i = 0; i < PatternCount; i++)
            {
                var data = Utilities.ReadBlock(romBuffer, dataAddresses[i], dataLengths[i]);
                _patterns[i] = new OverlayTilePattern(data, overlayTilesizes[i]);
            }
        }

        private int[] LoadPatternDataAddresses(byte[] romBuffer, int offset)
        {
            var addresses = new int[Count];
            var data = Utilities.ReadBlockGroup(romBuffer, offset, 2, Count);
            for (var i = 0; i < data.Length; i++)
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
            var lengths = new int[Count];
            for (var i = 0; i < dataAddresses.Length; i++)
            {
                var diff = 0;
                for (var j = i + 1; j < dataAddresses.Length; j++)
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
            // Maybe the size of each pattern should be saved in the Epic Edit-dedicated ROM range when we start supporting updating the patterns.
            var sizeIndexes = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1 };
            var sizeArray = new OverlayTileSize[Count];
            for (var i = 0; i < Count; i++)
            {
                sizeArray[i] = sizes[sizeIndexes[i]];
            }
            return sizeArray;
        }

        public int IndexOf(OverlayTilePattern pattern)
        {
            for (var i = 0; i < _patterns.Length; i++)
            {
                if (_patterns[i] == pattern)
                {
                    return i;
                }
            }

            throw new ArgumentException("Pattern not found.", nameof(pattern));
        }

        #region ICollection

        public IEnumerator<OverlayTilePattern> GetEnumerator()
        {
            foreach (var pattern in _patterns)
            {
                yield return pattern;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _patterns.GetEnumerator();
        }

        public int Count => _patterns.Length;

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
            foreach (var pattern in _patterns)
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
            Array.Copy(_patterns, 0, array, arrayIndex, Count);
        }

        public bool Remove(OverlayTilePattern item)
        {
            throw new NotImplementedException();
        }

        #endregion ICollection
    }
}
