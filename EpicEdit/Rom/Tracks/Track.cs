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
using System.IO;

using EpicEdit.Rom.Settings;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Utility;

namespace EpicEdit.Rom.Tracks
{
    internal enum ResizeHandle
    {
        None,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left
    }

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
        public event EventHandler<EventArgs<int>> ColorChanged;
        public event EventHandler<EventArgs> ColorsChanged;

        private readonly SuffixedTextItem suffixedNameItem;
        public SuffixedTextItem SuffixedNameItem
        {
            get { return this.suffixedNameItem; }
        }

        public string Name
        {
            get { return this.SuffixedNameItem.Value; }
        }

        private Theme theme;
        public Theme Theme
        {
            get { return this.theme; }
            set
            {
                if (this.theme == value)
                {
                    return;
                }

                this.RemoveColorChangedEventHandlers();
                this.theme = value;
                this.AddColorChangedEventHandlers();

                this.MarkAsModified("Theme");
            }
        }

        private readonly TrackMap map;
        public TrackMap Map
        {
            get { return this.map; }
            private set { this.map.SetBytes(value.GetBytes()); }
        }

        private readonly OverlayTiles overlayTiles;
        public OverlayTiles OverlayTiles
        {
            get { return this.overlayTiles; }
            private set { this.overlayTiles.SetBytes(value.GetBytes()); }
        }

        private readonly TrackAI ai;
        public TrackAI AI
        {
            get { return this.ai; }
            private set
            {
                byte[] data = value.GetBytes();
                byte[] zoneData = new byte[data.Length - value.ElementCount * 3];
                byte[] targetData = new byte[data.Length - zoneData.Length];
                Buffer.BlockCopy(data, 0, zoneData, 0, zoneData.Length);
                Buffer.BlockCopy(data, zoneData.Length, targetData, 0, targetData.Length);
                this.ai.SetBytes(zoneData, targetData);
            }
        }

        public RoadTileset RoadTileset
        {
            get { return this.Theme.RoadTileset; }
        }

        public bool Modified { get; private set; }

        protected Track(SuffixedTextItem nameItem, Theme theme,
                        byte[] map, byte[] overlayTilesData,
                        byte[] aiZoneData, byte[] aiTargetData,
                        OverlayTileSizes overlayTileSizes,
                        OverlayTilePatterns overlayTilePatterns)
        {
            this.theme = theme;
            this.AddColorChangedEventHandlers();

            this.suffixedNameItem = nameItem;
            this.SuffixedNameItem.PropertyChanged += this.SuffixedNameItem_PropertyChanged;

            this.map = new TrackMap(map);
            this.Map.DataChanged += this.Map_DataChanged;

            this.ai = new TrackAI(aiZoneData, aiTargetData, this);
            this.AI.DataChanged += this.AI_DataChanged;
            this.AI.ElementAdded += this.AI_DataChanged;
            this.AI.ElementRemoved += this.AI_DataChanged;
            this.AI.ElementsCleared += this.AI_DataChanged;

            this.overlayTiles = new OverlayTiles(overlayTilesData, overlayTileSizes, overlayTilePatterns);
            this.OverlayTiles.DataChanged += this.OverlayTiles_DataChanged;
            this.OverlayTiles.ElementAdded += this.OverlayTiles_DataChanged;
            this.OverlayTiles.ElementRemoved += this.OverlayTiles_DataChanged;
            this.OverlayTiles.ElementsCleared += this.OverlayTiles_DataChanged;
        }

        private void AddColorChangedEventHandlers()
        {
            foreach (Palette palette in this.theme.Palettes)
            {
                palette.ColorChanged += this.palette_ColorChanged;
                palette.ColorsChanged += this.palette_ColorsChanged;
            }
        }

        private void RemoveColorChangedEventHandlers()
        {
            foreach (Palette palette in this.theme.Palettes)
            {
                palette.ColorChanged -= this.palette_ColorChanged;
                palette.ColorsChanged -= this.palette_ColorsChanged;
            }
        }

        private void palette_ColorChanged(object sender, EventArgs<int> e)
        {
            this.OnColorChanged(sender, e.Value);
        }

        private void palette_ColorsChanged(object sender, EventArgs e)
        {
            this.OnColorsChanged(sender);
        }

        private void OnColorChanged(object sender, int value)
        {
            if (this.ColorChanged != null)
            {
                this.ColorChanged(sender, new EventArgs<int>(value));
            }
        }

        private void OnColorsChanged(object sender)
        {
            if (this.ColorsChanged != null)
            {
                this.ColorsChanged(sender, EventArgs.Empty);
            }
        }

        private void SuffixedNameItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChange("SuffixedNameItem");
        }

        private void Map_DataChanged(object sender, EventArgs e)
        {
            this.MarkAsModified("Map");
        }

        private void AI_DataChanged(object sender, EventArgs e)
        {
            this.MarkAsModified("AI");
        }

        private void OverlayTiles_DataChanged(object sender, EventArgs e)
        {
            this.MarkAsModified("OverlayTiles");
        }

        public void Import(string filePath, Game game)
        {
            if (filePath.EndsWith(".mkt", StringComparison.OrdinalIgnoreCase))
            {
                this.ImportMkt(filePath, game);
            }
            else
            {
                this.ImportSmkc(filePath, game);
            }

            this.MarkAsModified("Map");
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
                    throw new InvalidDataException("\"" + Path.GetFileName(filePath) + "\"" + Environment.NewLine +
                                                   "isn't a valid track file. Import aborted.");
                }

                byte[] mapData = new byte[TrackMap.SquareSize];
                reader.Read(mapData, 0, mapData.Length);

                this.Map = new TrackMap(mapData);

                if (fileLength == TrackMap.SquareSize + 1) // If a theme is defined
                {
                    byte themeId = (byte)(reader.ReadByte() >> 1);
                    this.Theme = game.Themes[themeId];
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
            this.LoadDataFrom(track);
        }

        /// <summary>
        /// Loads the regular track items from the MakeTrack object.
        /// </summary>
        protected virtual void LoadDataFrom(MakeTrack track)
        {
            this.Map = track.Map;
            this.Theme = track.Theme;
            this.OverlayTiles = track.OverlayTiles;
            this.AI = track.AI;
        }

        public void Export(string filePath, Game game)
        {
            if (filePath.EndsWith(".mkt", StringComparison.OrdinalIgnoreCase))
            {
                this.ExportMkt(filePath, game);
            }
            else
            {
                this.ExportSmkc(filePath, game);
            }
        }

        protected void MarkAsModified(string propertyName)
        {
            this.Modified = true;
            this.OnPropertyChange(propertyName);
        }

        private void OnPropertyChange(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ResetModifiedState()
        {
            if (!this.Modified)
            {
                return;
            }

            this.Modified = false;
            this.OnPropertyChange("Modified");
        }

        /// <summary>
        /// Exports track as MKT (Track Designer).
        /// </summary>
        private void ExportMkt(string filePath, Game game)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
            {
                bw.Write(this.Map.GetBytes());

                byte themeId = game.Themes.GetThemeId(this.Theme);
                bw.Write(themeId);
            }
        }

        /// <summary>
        /// Exports track as SMKC (MAKE).
        /// </summary>
        private void ExportSmkc(string filePath, Game game)
        {
            MakeTrack track = new MakeTrack(this, game);
            this.LoadDataTo(track);
            track.Save(filePath);
        }

        /// <summary>
        /// Loads the regular track items from to MakeTrack object.
        /// </summary>
        protected virtual void LoadDataTo(MakeTrack track)
        {
            track.Map = this.Map;
            track.Theme = this.Theme;
            track.OverlayTiles = this.OverlayTiles;
            track.AI = this.AI;
        }
    }
}