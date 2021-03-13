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
using System.Collections.Generic;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Lightweight text collection that makes it possible to keep track of the total number of available characters.
    /// </summary>
    internal class FreeTextCollection : ITextCollection
    {
        public TextConverter Converter { get; }

        public Region Region => Converter.Region;

        public int TotalCharacterCount
        {
            get
            {
                var total = 0;

                foreach (var item in _items)
                {
                    total += Converter.EncodeText(item.Value, null).Length;
                }

                return total;
            }
        }

        public int MaxCharacterCount { get; }

        private readonly List<TextItem> _items;

        public FreeTextCollection(TextConverter converter, int maxCharacterCount)
        {
            Converter = converter;
            MaxCharacterCount = maxCharacterCount;
            _items = new List<TextItem>();
        }

        public void Add(TextItem item)
        {
            _items.Add(item);
        }
    }
}
