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
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;
using System;
using System.ComponentModel;
using System.IO;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents the common base between a <see cref="GPTrack"/> and a <see cref="BattleTrack"/>.
    /// </summary>
    internal abstract class Track : INotifyPropertyChanged
    {
        /// <summary>
        /// Total number of tracks (GP tracks + battle tracks).
        /// </summary>
        public const int Count = GPTrack.Count + BattleTrack.Count;

        /// <summary>
        /// Total number of track groups (GP and Battle).
        /// </summary>
        public const int GroupCount = GPTrack.GroupCount + BattleTrack.GroupCount;

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raised when a single color has been changed.
        /// </summary>
        public event EventHandler<EventArgs<int>> ColorChanged;

        /// <summary>
        /// Raised when multiple colors have been changed.
        /// </summary>
        public event EventHandler<EventArgs> ColorsChanged;

        /// <summary>
        /// Raised after the ColorChanged event has been raised and all related event handlers have been called,
        /// ensuring graphics that used this color have been updated at this point.
        /// </summary>
        public event EventHandler<EventArgs<int>> ColorGraphicsChanged;

        /// <summary>
        /// Raised after the ColorsChanged event has been raised and all related event handlers have been called,
        /// ensuring graphics that used these colors have been updated at this point.
        /// </summary>
        public event EventHandler<EventArgs> ColorsGraphicsChanged;

        public SuffixedTextItem SuffixedNameItem { get; }

        public string Name => SuffixedNameItem.Value;

        private Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set
            {
                if (_theme == value)
                {
                    return;
                }

                RemoveColorChangedEventHandlers();
                _theme = value;
                AddColorChangedEventHandlers();

                MarkAsModified(PropertyNames.Track.Theme);
            }
        }

        private readonly TrackMap _map;
        public TrackMap Map
        {
            get => _map;
            private set => _map.SetBytes(value.GetBytes());
        }

        private readonly OverlayTiles _overlayTiles;
        public OverlayTiles OverlayTiles
        {
            get => _overlayTiles;
            private set => _overlayTiles.SetBytes(value.GetBytes());
        }

        private readonly TrackAI _ai;
        public TrackAI AI
        {
            get => _ai;
            private set
            {
                byte[] data = value.GetBytes();
                byte[] areaData = new byte[data.Length - value.ElementCount * 3 - 1];
                byte[] targetData = new byte[data.Length - areaData.Length - 1];
                Buffer.BlockCopy(data, 0, areaData, 0, areaData.Length);
                Buffer.BlockCopy(data, areaData.Length + 1, targetData, 0, targetData.Length);
                _ai.SetBytes(areaData, targetData);
            }
        }

        public RoadTileset RoadTileset => Theme.RoadTileset;

        public bool Modified { get; private set; }

        protected Track(SuffixedTextItem nameItem, Theme theme,
                        byte[] map, byte[] overlayTilesData,
                        byte[] aiAreaData, byte[] aiTargetData,
                        OverlayTileSizes overlayTileSizes,
                        OverlayTilePatterns overlayTilePatterns)
        {
            _theme = theme;
            AddColorChangedEventHandlers();

            SuffixedNameItem = nameItem;
            SuffixedNameItem.PropertyChanged += SuffixedNameItem_PropertyChanged;

            _map = new TrackMap(map);
            Map.DataChanged += Map_DataChanged;

            _ai = new TrackAI(aiAreaData, aiTargetData, this);
            AI.PropertyChanged += AI_PropertyChanged;
            AI.ElementAdded += AI_PropertyChanged;
            AI.ElementRemoved += AI_PropertyChanged;
            AI.ElementsCleared += AI_PropertyChanged;

            _overlayTiles = new OverlayTiles(overlayTilesData, overlayTileSizes, overlayTilePatterns);
            OverlayTiles.DataChanged += OverlayTiles_DataChanged;
            OverlayTiles.ElementAdded += OverlayTiles_DataChanged;
            OverlayTiles.ElementRemoved += OverlayTiles_DataChanged;
            OverlayTiles.ElementsCleared += OverlayTiles_DataChanged;
        }

        private void AddColorChangedEventHandlers()
        {
            foreach (Palette palette in _theme.Palettes)
            {
                palette.ColorChanged += palette_ColorChanged;
                palette.ColorsChanged += palette_ColorsChanged;
                palette.ColorGraphicsChanged += palette_ColorGraphicsChanged;
                palette.ColorsGraphicsChanged += palette_ColorsGraphicsChanged;
            }
        }

        private void RemoveColorChangedEventHandlers()
        {
            foreach (Palette palette in _theme.Palettes)
            {
                palette.ColorChanged -= palette_ColorChanged;
                palette.ColorsChanged -= palette_ColorsChanged;
                palette.ColorGraphicsChanged -= palette_ColorGraphicsChanged;
                palette.ColorsGraphicsChanged -= palette_ColorsGraphicsChanged;
            }
        }

        private void palette_ColorChanged(object sender, EventArgs<int> e)
        {
            OnColorChanged(sender, e);
        }

        private void palette_ColorsChanged(object sender, EventArgs e)
        {
            OnColorsChanged(sender);
        }

        private void palette_ColorGraphicsChanged(object sender, EventArgs<int> e)
        {
            OnColorGraphicsChanged(sender, e);
        }

        private void palette_ColorsGraphicsChanged(object sender, EventArgs e)
        {
            OnColorsGraphicsChanged(sender);
        }

        private void OnColorChanged(object sender, EventArgs<int> e)
        {
            ColorChanged?.Invoke(sender, e);
        }

        private void OnColorsChanged(object sender)
        {
            ColorsChanged?.Invoke(sender, EventArgs.Empty);
        }

        private void OnColorGraphicsChanged(object sender, EventArgs<int> e)
        {
            ColorGraphicsChanged?.Invoke(sender, e);
        }

        private void OnColorsGraphicsChanged(object sender)
        {
            ColorsGraphicsChanged?.Invoke(sender, EventArgs.Empty);
        }

        private void SuffixedNameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(PropertyNames.Track.SuffixedNameItem);
        }

        private void Map_DataChanged(object sender, EventArgs e)
        {
            MarkAsModified(PropertyNames.Track.Map);
        }

        private void AI_PropertyChanged(object sender, EventArgs e)
        {
            MarkAsModified(PropertyNames.Track.AI);
        }

        private void OverlayTiles_DataChanged(object sender, EventArgs e)
        {
            MarkAsModified(PropertyNames.Track.OverlayTiles);
        }

        public void Import(string filePath, Game game)
        {
            if (filePath.EndsWith(".mkt", StringComparison.OrdinalIgnoreCase))
            {
                ImportMkt(filePath, game);
            }
            else
            {
                ImportSmkc(filePath, game);
            }
        }

        /// <summary>
        /// Imports an MKT (Track Designer) track.
        /// </summary>
        private void ImportMkt(string filePath, Game game)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
            {
                FileInfo info = new FileInfo(filePath);
                int fileLength = (int)info.Length;

                if (fileLength != TrackMap.SquareSize && fileLength != TrackMap.SquareSize + 1)
                {
                    throw new InvalidDataException($"\"{Path.GetFileName(filePath)}\" isn't a valid track file. Import aborted.");
                }

                byte[] mapData = new byte[TrackMap.SquareSize];
                reader.Read(mapData, 0, mapData.Length);

                Map = new TrackMap(mapData);

                if (fileLength == TrackMap.SquareSize + 1) // If a theme is defined
                {
                    byte themeId = (byte)(reader.ReadByte() >> 1);
                    Theme = game.Themes[themeId];
                }
            }
        }

        /// <summary>
        /// Imports an SMKC (MAKE) track.
        /// </summary>
        private void ImportSmkc(string filePath, Game game)
        {
            MakeTrack track = new MakeTrack(this, game);
            track.Load(filePath);
            LoadDataFrom(track);
        }

        /// <summary>
        /// Loads the regular track items from the MakeTrack object.
        /// </summary>
        protected virtual void LoadDataFrom(MakeTrack track)
        {
            Map = track.Map;
            Theme = track.Theme;
            OverlayTiles = track.OverlayTiles;
            AI = track.AI;
        }

        public void Export(string filePath, Game game)
        {
            if (filePath.EndsWith(".mkt", StringComparison.OrdinalIgnoreCase))
            {
                ExportMkt(filePath, game);
            }
            else
            {
                ExportSmkc(filePath, game);
            }
        }

        protected void MarkAsModified(string propertyName)
        {
            Modified = true;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ResetModifiedState()
        {
            if (!Modified)
            {
                return;
            }

            Modified = false;
            OnPropertyChanged(PropertyNames.Track.Modified);
        }

        /// <summary>
        /// Exports track as MKT (Track Designer).
        /// </summary>
        private void ExportMkt(string filePath, Game game)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(Map.GetBytes());

                byte themeId = game.Themes.GetThemeId(Theme);
                bw.Write(themeId);
            }
        }

        /// <summary>
        /// Exports track as SMKC (MAKE).
        /// </summary>
        private void ExportSmkc(string filePath, Game game)
        {
            MakeTrack track = new MakeTrack(this, game);
            LoadDataTo(track);
            track.Save(filePath);
        }

        /// <summary>
        /// Loads the regular track items from to MakeTrack object.
        /// </summary>
        protected virtual void LoadDataTo(MakeTrack track)
        {
            track.Map = Map;
            track.Theme = Theme;
            track.OverlayTiles = OverlayTiles;
            track.AI = AI;
        }
    }
}