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
	/// Collection of all the overlay tile patterns in the game.
	/// </summary>
	public class OverlayTilePatterns : ICollection<OverlayTilePattern>
	{
		private OverlayTilePattern[] patterns;

		public OverlayTilePattern this[int index]
		{
			get { return this.patterns[index]; }
		}

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
			this.LoadPatterns(romBuffer, offsets, sizes);
		}

		private void LoadPatterns(byte[] romBuffer, Offsets offsets, OverlayTileSizes sizes)
		{
			int patternCount = OverlayTilePatterns.LoadPatternCount();

			this.patterns = new OverlayTilePattern[patternCount];

			// Get the addresses where the individual pattern data is
			int[] dataAddresses = LoadPatternDataAddresses(romBuffer, offsets);
			// Get the data lengths of all the patterns
			int[] dataLengths = LoadPatternDataLengths(offsets, dataAddresses);
			// Get the widths and heights of all the patterns
			OverlayTileSize[] overlayTilesizes = LoadPatternSizes(sizes);

			for (int i = 0; i < patternCount; i++)
			{
				byte[] data = Utilities.ReadBlock(romBuffer, dataAddresses[i], dataLengths[i]);
				this.patterns[i] = new OverlayTilePattern(data, overlayTilesizes[i]);
			}
		}

		private static int LoadPatternCount()
		{
			// TODO: Get this from the ROM somehow
			return 56;
		}

		private int[] LoadPatternDataAddresses(byte[] romBuffer, Offsets offsets)
		{
			int[] addresses = new int[this.Count];
			byte[][] data = Utilities.ReadBlockGroup(romBuffer, offsets[Address.TrackOverlayPatternAddresses], 2, this.Count);
			for (int x = 0; x < data.Length; x++)
			{
				addresses[x] = (data[x][1] << 8) + data[x][0] + 0x40000;
			}
			return addresses;
		}

		private int[] LoadPatternDataLengths(Offsets offsets, int[] dataAddresses)
		{
			// TODO: This data must be somewhere in the ROM, but for now, let's just do a substraction between the addresses, they are ordered in the rom anyways.
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
					// A bit of a hack, the overlay tile sizes follow the pattern data in the rom
					diff = offsets[Address.TrackOverlaySizes] - dataAddresses[i];
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
			int[] sizeIndexes = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 1, 1, 1, 1 };
			OverlayTileSize[] sizeArray = new OverlayTileSize[this.Count];
			for(int i = 0; i < this.Count; i++)
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

			throw new MissingMemberException("Pattern not found.");
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

		public int Count
		{
			get { return patterns.Length; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

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
			foreach (OverlayTilePattern pattern in patterns)
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
			Array.Copy(patterns, 0, array, arrayIndex, this.Count);
		}

		public bool Remove(OverlayTilePattern item)
		{
			throw new NotImplementedException();
		}
		
		#endregion ICollection
	}
}
