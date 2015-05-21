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

namespace EpicEdit.Rom.Settings
{
    /// <summary>
    /// Represents a text item followed by a suffix.
    /// Example: "MARIO CIRCUIT 1". Text item = "MARIO CIRCUIT ", suffix = "1".
    /// </summary>
    internal class SuffixedTextItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private TextItem textItem;
        public TextItem TextItem
        {
            get { return this.textItem; }
            set
            {
                if (this.textItem != null)
                {
                    this.textItem.PropertyChanged -= this.TextItem_PropertyChanged;
                }
                this.textItem = value;
                this.textItem.PropertyChanged += this.TextItem_PropertyChanged;
            }
        }

        private string suffix;
        public string Suffix
        {
            get { return this.suffix; }
            set
            {
                if (this.suffix == value)
                {
                    return;
                }

                this.suffix = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Suffix"));
                }
            }
        }

        public string Value
        {
            get { return this.TextItem.FormattedValue + this.Suffix; }
        }

        public SuffixedTextItem(TextItem nameItem, string nameSuffix)
        {
            this.TextItem = nameItem;
            this.suffix = nameSuffix;
        }

        private void TextItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs("TextItem"));
            }
        }
    }
}
