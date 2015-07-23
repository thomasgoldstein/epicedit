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

namespace EpicEdit.Rom.Utility
{
    internal static class CharacterSet
    {
        /// <summary>
        /// A null / undefined character.
        /// </summary>
        private const char Nch = char.MinValue;

        public static char[] Get(Region region, bool tallCharacters)
        {
            return region == Region.Jap ?
                CharacterSet.GetJapaneseSet() :
                !tallCharacters ?
                CharacterSet.GetEnglishSet() :
                CharacterSet.GetLargeEnglishSet();
        }

        private static char[] GetEnglishSet()
        {
            return new []
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', // 00-0F
                'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', // 10-1F
                'W', 'X', 'Y', 'Z', '?', '.', ',', '!', '\'','"', Nch, Nch, Nch, Nch, ':', ' ', // 20-2F
            };
        }

        private static char[] GetLargeEnglishSet()
        {
            return new []
            {
                'A', Nch, 'B', Nch, 'C', Nch, 'D', Nch, 'E', Nch, 'F', Nch, 'G', Nch, 'H', Nch, // 80-8F
                'I', Nch, 'J', Nch, 'K', Nch, 'L', Nch, 'M', Nch, 'N', Nch, 'O', Nch, 'P', Nch, // 90-9F
                'Q', Nch, 'R', Nch, 'S', Nch, 'T', Nch, 'U', Nch, 'V', Nch, 'W', Nch, 'X', Nch, // A0-AF
                'Y', Nch, 'Z', Nch, '?', Nch, '.', ',', '!', Nch, Nch, Nch, Nch, Nch, Nch, Nch, // B0-BF
                Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, // C0-CF
                Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, Nch, // D0-DF
                Nch, Nch, Nch, Nch, Nch, ' ', // E0-E5
            };
        }

        private static char[] GetJapaneseSet()
        {
            const char Ten = '\u3099'; // Ten-ten
            const char Mar = '\u309A'; // Maru
            return new []
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', // 00-0F
                Nch, Nch, Nch, 'J', Nch, Nch, Nch, Nch, Nch, Nch, Nch, 'R', Nch, Nch, Nch, Nch, // 10-1F
                'G', 'P', 'V', 'S', '?', '.', ',', '!', '\'','"', Nch, Nch, Nch, Nch, Ten, Mar, // 20-2F
                'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', Nch, 'た', // 30-3F
                Nch, 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', Nch, Nch, 'へ', Nch, 'ま', 'み', // 40-4F
                ' ', Nch, 'も', 'や', Nch, Nch, 'ら', Nch, 'る', 'れ', 'ろ', 'わ', 'を', 'ん', '(', '+', // 50-5F
                'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', 'タ', // 60-6F
                'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', 'ミ', // 70-7F
                'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ェ', 'ン', '。', 'ヽ', // 80-8F
                Nch, 'っ', Nch, Nch, 'ょ', Nch, Nch, 'ッ', 'ャ', 'ァ', 'ョ', Nch, 'ー', Nch, Nch, Nch, // 90-9F
            };
        }
    }
}
