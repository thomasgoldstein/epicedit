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
    public static class CharacterSetJap
    {
        public static char[] Get()
        {
            char nch = '\0'; // Unicode null character
            char ten = '\u3099'; // Ten-ten
            char mar = '\u309A'; // Maru
            return new char[]
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', // 00-0F
                nch, nch, nch, 'J', nch, nch, nch, nch, nch, nch, nch, 'R', nch, nch, nch, nch, // 10-1F
                'G', 'P', 'V', 'S', '?', '.', ',', '!', '\'','"', nch, nch, nch, nch, ten, mar, // 20-2F
                'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く', 'け', 'こ', 'さ', 'し', 'す', 'せ', nch, 'た', // 30-3F
                nch, 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね', 'の', 'は', nch, nch, 'へ', nch, 'ま', 'み', // 40-4F
                ' ', nch, 'も', 'や', nch, nch, 'ら', nch, 'る', 'れ', 'ろ', 'わ', 'を', 'ん', '(', '+', // 50-5F
                'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク', 'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', 'タ', // 60-6F
                'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ', 'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', 'ミ', // 70-7F
                'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ', 'ル', 'レ', 'ロ', 'ワ', 'ェ', 'ン', '。', 'ヽ', // 80-8F
                nch, 'っ', nch, nch, 'ょ', nch, nch, 'ッ', 'ャ', 'ァ', 'ョ', nch, 'ー', nch, nch, nch, // 90-9F
            };
        }
    }
}
