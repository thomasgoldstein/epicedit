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
using System.ComponentModel;
using System.Globalization;

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Item of a <see cref="TextCollection"/>.
    /// </summary>
    internal class TextItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Region region;

        public TextItem(Region region, string value)
        {
            this.region = region;
            this.value = value;
        }

        private string value;
        public string Value
        {
            get { return this.value; }
            set
            {
                this.value = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }
        }

        public string FormattedValue
        {
            get
            {
                return this.region == Region.Jap ?
                       this.value :
                       CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.value.ToLowerInvariant());
            }
        }
    }
}
