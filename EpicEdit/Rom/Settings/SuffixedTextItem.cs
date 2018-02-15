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

        public const int MaxSuffixCharacterCount = 23;

        private TextItem textItem;
        public TextItem TextItem
        {
            get => this.textItem;
            set
            {
                if (this.textItem == value)
                {
                    return;
                }

                if (this.textItem != null)
                {
                    this.textItem.PropertyChanged -= this.TextItem_PropertyChanged;
                }
                this.textItem = value;
                this.textItem.PropertyChanged += this.TextItem_PropertyChanged;

                this.OnPropertyChanged(PropertyNames.SuffixedTextItem.TextItem);
            }
        }

        public TextItem Suffix { get; }

        public string Value => this.TextItem.FormattedValue + this.Suffix?.FormattedValue;

        public SuffixedTextItem(TextItem nameItem, string nameSuffix, FreeTextCollection suffixCollection)
        {
            this.TextItem = nameItem;

            // NOTE: The "Battle Course" track group doesn't have a suffix because it doesn't actually exist in the game.
            // It's only created in the editor to have a logical group that contains the Battle Courses.
            if (suffixCollection != null)
            {
                this.Suffix = new TextItem(suffixCollection, nameSuffix);
                this.Suffix.PropertyChanged += this.Suffix_PropertyChanged;
                suffixCollection.Add(this.Suffix);
            }
        }

        private void TextItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(PropertyNames.SuffixedTextItem.TextItem);
        }

        private void Suffix_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged(PropertyNames.SuffixedTextItem.Suffix);
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
