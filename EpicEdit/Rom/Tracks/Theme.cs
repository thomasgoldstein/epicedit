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

        public TextItem NameItem { get; }

        public string Name => NameItem.FormattedValue;

        public Palettes Palettes { get; }

        public RoadTileset RoadTileset { get; }

        public Background Background { get; }

        public RomColor BackColor => Palettes.BackColor;

        public bool Modified
        {
            get
            {
                return
                    Palettes.Modified ||
                    RoadTileset.Modified ||
                    Background.Layout.Modified ||
                    Background.Tileset.Modified;
            }
        }

        public Theme(TextItem nameItem, Palettes palettes, RoadTileset roadTileset, Background background)
        {
            NameItem = nameItem;
            Palettes = palettes;
            Palettes.Theme = this;
            RoadTileset = roadTileset;
            Background = background;

            Palettes.PropertyChanged += OnPropertyChanged;
            RoadTileset.PropertyChanged += OnPropertyChanged;
            Background.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public override string ToString()
        {
            return Name;
        }

        public void ResetModifiedState()
        {
            Palettes.ResetModifiedState();
            RoadTileset.ResetModifiedState();
            Background.Layout.ResetModifiedState();
            Background.Tileset.ResetModifiedState();
        }

        public void Dispose()
        {
            RoadTileset.Dispose();
            Background.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
