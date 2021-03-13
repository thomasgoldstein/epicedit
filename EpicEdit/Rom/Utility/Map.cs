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

using System.Collections.Generic;

namespace EpicEdit.Rom.Utility
{
    /// <summary>
    /// A two-way dictionary.
    /// </summary>
    internal class Map<T1, T2>
    {
        public Indexer<T1, T2> Forward { get; }
        public Indexer<T2, T1> Reverse { get; }

        private readonly Dictionary<T1, T2> _forward;
        private readonly Dictionary<T2, T1> _reverse;

        public Map()
        {
            _forward = new Dictionary<T1, T2>();
            _reverse = new Dictionary<T2, T1>();
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public void Remove(T1 t1)
        {
            _reverse.Remove(_forward[t1]);
            _forward.Remove(t1);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();
        }

        public class Indexer<T3, T4>
        {
            private readonly Dictionary<T3, T4> _dictionary;

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public T4 this[T3 index]
            {
                get => _dictionary[index];
                private set => _dictionary[index] = value;
            }

            public bool ContainsKey(T3 key)
            {
                return _dictionary.ContainsKey(key);
            }

            public bool TryGetValue(T3 key, out T4 value)
            {
                return _dictionary.TryGetValue(key, out value);
            }

            public T4[] GetValues()
            {
                var values = new T4[_dictionary.Values.Count];
                _dictionary.Values.CopyTo(values, 0);
                return values;
            }
        }
    }
}
