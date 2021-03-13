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

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// A <see cref="TextCollection"/> featuring a hack for Japanese GP podium cup texts.
    /// </summary>
    internal class GPPodiumCupTextCollection : TextCollection
    {
        public GPPodiumCupTextCollection(byte[] romBuffer, int indexOffset, int count, int totalSize, bool hasPaletteData,
                                         bool fixedLength, bool japAltMode, bool tallCharacters, byte shiftValue, byte[] keys, char[] values)
            : base(romBuffer, indexOffset, count, totalSize, hasPaletteData, fixedLength, japAltMode, tallCharacters, shiftValue, keys, values)
        {

        }

        protected override byte[] GetBytes(out byte[] indexes, out int length)
        {
            byte[] data = base.GetBytes(out indexes, out length);

            if (Region == Region.Jap && data[63] == 0x0D)
            {
                // HACK: Preserve the original color palette of the ending "-" character (palette 0x09 instead of 0x0D).
                // To support this without any hacks, we'd probably need to drop the current
                // "game text to string" logic and let users edit each character as tiles. Not worth it.
                data[63] = 0x09;
            }

            return data;
        }
    }
}
