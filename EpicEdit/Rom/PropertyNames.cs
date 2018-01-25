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

namespace EpicEdit.Rom
{
    /// <summary>
    /// Regroups all of the property names for classes which implement INotifyPropertyChanged.
    /// </summary>
    internal static class PropertyNames
    {
        internal static class Background
        {
            public const string Data = "Data"; // NOTE: Dummy property name
        }

        internal static class Game
        {
            public const string Data = "Data"; // NOTE: Dummy property name
            public const string Modified = nameof(Rom.Game.Modified);
        }

        internal static class GPStartPosition
        {
            public const string Location = nameof(Tracks.Start.GPStartPosition.Location);
            public const string SecondRowOffset = nameof(Tracks.Start.GPStartPosition.SecondRowOffset);
        }

        internal static class GPTrack
        {
            public const string ItemProbabilityIndex = nameof(Tracks.GPTrack.ItemProbabilityIndex);
            public const string LapLine = nameof(Tracks.GPTrack.LapLine);
            public const string Objects = nameof(Tracks.GPTrack.Objects);
            public const string StartPosition = nameof(Tracks.GPTrack.StartPosition);
        }

        internal static class ItemProbability
        {
            public const string DisplayedItems = nameof(Tracks.Items.ItemProbability.DisplayedItems);
            public const string FieldValue = "FieldValue"; // NOTE: Dummy property name
        }

        internal static class Palettes
        {
            public const string Palette = "Palette"; // NOTE: Dummy property name
        }

        internal static class RankPoints
        {
            public const string Value = "Value"; // NOTE: Dummy property name
        }

        internal static class RoadTile
        {
            public const string Genre = nameof(Tracks.Road.RoadTile.Genre);
        }

        internal static class SuffixedTextItem
        {
            public const string Suffix = nameof(Settings.SuffixedTextItem.Suffix);
            public const string TextItem = nameof(Settings.SuffixedTextItem.TextItem);
        }

        internal static class TextItem
        {
            public const string Value = nameof(Settings.TextItem.Value);
        }

        internal static class Tile
        {
            public const string Bitmap = nameof(Rom.Tile.Bitmap);
            public const string Graphics = nameof(Rom.Tile.Graphics);
            public const string Palette = nameof(Rom.Tile.Palette);
        }

        internal static class Track
        {
            public const string AI = nameof(Tracks.Track.AI);
            public const string Map = nameof(Tracks.Track.Map);
            public const string Modified = nameof(Tracks.Track.Modified);
            public const string OverlayTiles = nameof(Tracks.Track.OverlayTiles);
            public const string SuffixedNameItem = nameof(Tracks.Track.SuffixedNameItem);
            public const string Theme = nameof(Tracks.Track.Theme);
        }

        internal static class TrackAIElement
        {
            public const string Index = "Index"; // NOTE: Dummy property name
            public const string Location = nameof(Tracks.AI.TrackAIElement.Location);
            public const string Speed = nameof(Tracks.AI.TrackAIElement.Speed);
            public const string IsIntersection = nameof(Tracks.AI.TrackAIElement.IsIntersection);
            public const string Target = nameof(Tracks.AI.TrackAIElement.Target);
            public const string Zone = nameof(Tracks.AI.TrackAIElement.Zone);
            public const string ZoneShape = nameof(Tracks.AI.TrackAIElement.ZoneShape);
        }

        internal static class TrackObject
        {
            public const string Location = nameof(Tracks.Objects.TrackObject.Location);
        }

        internal static class TrackObjectMatchRace
        {
            public const string Direction = nameof(Tracks.Objects.TrackObjectMatchRace.Direction);
        }

        internal static class TrackObjectProperties
        {
            public const string Flashing = nameof(Tracks.Objects.TrackObjectProperties.Flashing);
            public const string Interaction = nameof(Tracks.Objects.TrackObjectProperties.Interaction);
            public const string Palette = nameof(Tracks.Objects.TrackObjectProperties.Palette);
            public const string PaletteIndexes = nameof(Tracks.Objects.TrackObjectProperties.PaletteIndexes);
            public const string Routine = nameof(Tracks.Objects.TrackObjectProperties.Routine);
            public const string Tileset = nameof(Tracks.Objects.TrackObjectProperties.Tileset);
        }

        internal static class TrackObjectZones
        {
            public const string FrontView = nameof(Tracks.Objects.TrackObjectZones.FrontView);
            public const string RearView = nameof(Tracks.Objects.TrackObjectZones.RearView);
        }
    }
}
