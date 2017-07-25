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
        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }

        private Dictionary<T1, T2> forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> reverse = new Dictionary<T2, T1>();

        public Map()
        {
            this.Forward = new Indexer<T1, T2>(this.forward);
            this.Reverse = new Indexer<T2, T1>(this.reverse);
        }

        public void Add(T1 t1, T2 t2)
        {
            this.forward.Add(t1, t2);
            this.reverse.Add(t2, t1);
        }

        public void Remove(T1 t1)
        {
            this.reverse.Remove(this.forward[t1]);
            this.forward.Remove(t1);
        }

        public void Clear()
        {
            this.forward.Clear();
            this.reverse.Clear();
        }

        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> dictionary;

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                this.dictionary = dictionary;
            }

            public T4 this[T3 index]
            {
                get => this.dictionary[index];
                private set => this.dictionary[index] = value;
            }

            public bool ContainsKey(T3 key)
            {
                return this.dictionary.ContainsKey(key);
            }

            public bool TryGetValue(T3 key, out T4 value)
            {
                return this.dictionary.TryGetValue(key, out value);
            }

            public T4[] GetValues()
            {
                T4[] values = new T4[this.dictionary.Values.Count];
                this.dictionary.Values.CopyTo(values, 0);
                return values;
            }
        }
    }
}
