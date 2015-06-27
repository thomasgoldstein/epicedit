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

namespace EpicEdit.Rom
{
    /// <summary>
    /// Regroups all of the property names for classes which implement INotifyPropertyChanged.
    /// </summary>
    internal static class PropertyNames
    {
        internal static class GPStartPosition
        {
            public const string Location = "Location";
            public const string SecondRowOffset = "SecondRowOffset";
        }

        internal static class GPTrack
        {
            public const string ItemProbabilityIndex = "ItemProbabilityIndex";
            public const string LapLine = "LapLine";
            public const string Objects = "Objects";
            public const string StartPosition = "StartPosition";
        }

        internal static class RoadTile
        {
            public const string Genre = "Genre";
        }

        internal static class SuffixedTextItem
        {
            public const string Suffix = "Suffix";
            public const string TextItem = "TextItem";
        }

        internal static class TextItem
        {
            public const string Value = "Value";
        }

        internal static class Tile
        {
            public const string Bitmap = "Bitmap";
            public const string Graphics = "Graphics";
            public const string Palette = "Palette";
        }

        internal static class Track
        {
            public const string AI = "AI";
            public const string Map = "Map";
            public const string Modified = "Modified";
            public const string OverlayTiles = "OverlayTiles";
            public const string SuffixedNameItem = "SuffixedNameItem";
            public const string Theme = "Theme";
        }

        internal static class TrackAIElement
        {
            public const string Index = "Index";
            public const string Location = "Location";
            public const string Speed = "Speed";
            public const string Target = "Target";
            public const string Zone = "Zone";
            public const string ZoneShape = "ZoneShape";
        }

        internal static class TrackObject
        {
            public const string Location = "Location";
        }

        internal static class TrackObjectMatchRace
        {
            public const string Direction = "Direction";
        }

        internal static class TrackObjectProperties
        {
            public const string Flashing = "Flashing";
            public const string Interaction = "Interaction";
            public const string Palette = "Palette";
            public const string PaletteIndexes = "PaletteIndexes";
            public const string Routine = "Routine";
            public const string Tileset = "Tileset";
        }

        internal static class TrackObjectZones
        {
            public const string FrontView = "FrontView";
            public const string RearView = "RearView";
        }
    }
}
