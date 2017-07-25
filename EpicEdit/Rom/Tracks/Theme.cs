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

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Tracks.Scenery;
using System;
using System.ComponentModel;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents the graphics set and music of a track.
    /// </summary>
    internal sealed class Theme : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Number of themes.
        /// </summary>
        public const int Count = 8;

        public event PropertyChangedEventHandler PropertyChanged;

        public TextItem NameItem { get; private set; }

        public string Name => this.NameItem.FormattedValue;

        public Palettes Palettes { get; private set; }
        public RoadTileset RoadTileset { get; private set; }
        public Background Background { get; private set; }

        public RomColor BackColor => this.Palettes.BackColor;

        public bool Modified
        {
            get
            {
                return
                    this.Palettes.Modified ||
                    this.RoadTileset.Modified ||
                    this.Background.Layout.Modified ||
                    this.Background.Tileset.Modified;
            }
        }

        public Theme(TextItem nameItem, Palettes palettes, RoadTileset roadTileset, Background background)
        {
            this.NameItem = nameItem;
            this.Palettes = palettes;
            this.Palettes.Theme = this;
            this.RoadTileset = roadTileset;
            this.Background = background;

            this.Palettes.PropertyChanged += this.OnPropertyChanged;
            this.RoadTileset.PropertyChanged += this.OnPropertyChanged;
            this.Background.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(sender, e);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void ResetModifiedState()
        {
            this.Palettes.ResetModifiedState();
            this.RoadTileset.ResetModifiedState();
            this.Background.Layout.ResetModifiedState();
            this.Background.Tileset.ResetModifiedState();
        }

        public void Dispose()
        {
            this.RoadTileset.Dispose();
            this.Background.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
