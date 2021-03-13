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

using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Tracks.Start;
using EpicEdit.Rom.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace EpicEdit.Rom.Tracks
{
    /// <summary>
    /// Represents the data found in a MAKE exported track file.
    /// </summary>
    internal class MakeTrack
    {
        /// <summary>
        /// Byte count per line.
        /// </summary>
        private const int LineLength = 32;

        private readonly Track _track;
        private readonly Game _game;

        public TrackMap Map
        {
            get => new TrackMap(MapBytes);
            set => MapBytes = value.GetBytes();
        }

        public Theme Theme
        {
            get => _game.Themes[SpRegion[1] >> 1];
            set => SpRegion = new byte[] { 0, _game.Themes.GetThemeId(value) };
        }

        public OverlayTiles OverlayTiles
        {
            get => new OverlayTiles(Gpex, _game.OverlayTileSizes, _game.OverlayTilePatterns);
            set => Gpex = value.GetBytes();
        }

        public GPStartPosition StartPosition
        {
            get
            {
                short x, y, w;
                if (BitConverter.IsLittleEndian)
                {
                    x = BitConverter.ToInt16(new byte[] { SpStx[1], SpStx[0] }, 0);
                    y = BitConverter.ToInt16(new byte[] { SpSty[1], SpSty[0] }, 0);
                    w = BitConverter.ToInt16(new byte[] { SpStw[1], SpStw[0] }, 0);
                }
                else
                {
                    x = BitConverter.ToInt16(SpStx, 0);
                    y = BitConverter.ToInt16(SpSty, 0);
                    w = BitConverter.ToInt16(SpStw, 0);
                }

                return new GPStartPosition(x, y, w);
            }
            set
            {
                SpStx = new byte[] { (byte)((value.X & 0xFF00) >> 8), (byte)(value.X & 0xFF) };
                SpSty = new byte[] { (byte)((value.Y & 0xFF00) >> 8), (byte)(value.Y & 0xFF) };
                SpStw = new byte[] { (byte)((value.SecondRowOffset & 0xFF00) >> 8), (byte)(value.SecondRowOffset & 0xFF) };
            }
        }

        public LapLine LapLine
        {
            get
            {
                byte[] data = new byte[]
                {
                    SpLsly[1],
                    SpLsly[0],
                    SpLspx[1],
                    SpLspy[1],
                    SpLspw[1],
                    SpLsph[1]
                };
                return new LapLine(data);
            }
            set
            {
                byte[] data = value.GetBytes();
                SpLsly = new byte[] { data[1], data[0] };
                SpLspx = new byte[] { 0, data[2] };
                SpLspy = new byte[] { 0, data[3] };
                SpLspw = new byte[] { 0, data[4] };
                SpLsph = new byte[] { 0, data[5] };
            }
        }

        public BattleStartPosition StartPositionP1
        {
            get => new BattleStartPosition(EEBattleStart1);
            set => EEBattleStart1 = value.GetBytes();
        }

        public BattleStartPosition StartPositionP2
        {
            get => new BattleStartPosition(EEBattleStart2);
            set => EEBattleStart2 = value.GetBytes();
        }

        public TrackObjects Objects
        {
            get
            {
                byte[] data = Utilities.ReadBlock(Obj, 0, TrackObjects.Size);
                byte[] propData =
                {
                    EEObjTileset[1],
                    EEObjInteract[1],
                    EEObjRoutine[1],
                    EEObjPalettes[0],
                    EEObjPalettes[1],
                    EEObjPalettes[2],
                    EEObjPalettes[3],
                    EEObjFlashing[1]
                };
                return new TrackObjects(data, AreaBorder, AI, propData, null);
            }
            set
            {
                int size = Obj.Length;
                byte[] data = value.GetBytes();
                Array.Resize<byte>(ref data, size);
                Obj = data;

                AreaBorder = value.Areas.GetBytes();

                EEObjTileset = new byte[] { 0, (byte)value.Tileset };
                EEObjInteract = new byte[] { 0, (byte)value.Interaction };
                EEObjRoutine = new byte[] { 0, (byte)value.Routine };
                EEObjPalettes = value.PaletteIndexes.GetBytes();
                EEObjFlashing = new byte[] { 0, value.Flashing ? (byte)1 : (byte)0 };
            }
        }

        public TrackAI AI
        {
            get
            {
                GetAIData(out byte[] targetData, out byte[] areaData);
                return new TrackAI(areaData, targetData, _track);
            }
            set => SetAIData(value);
        }

        public int ItemProbabilityIndex
        {
            get => EEItemProba[1] >> 1;
            set => EEItemProba = new byte[] { 0, (byte)(value << 1) };
        }

        private readonly Dictionary<string, byte[]> _fields;

        public byte[] this[string name]
        {
            get => _fields[name];
            set
            {
                if (!_fields.ContainsKey(name))
                {
                    _fields.Add(name, null);
                }

                _fields[name] = value;
            }
        }

        /// <summary>
        /// GP Start Position X.
        /// </summary>
        private byte[] SpStx
        {
            get => this["SP_STX"];
            set => this["SP_STX"] = value;
        }

        /// <summary>
        /// GP Start Position Y.
        /// </summary>
        private byte[] SpSty
        {
            get => this["SP_STY"];
            set => this["SP_STY"] = value;
        }

        /// <summary>
        /// GP Start Position Width (2nd Row Offset).
        /// </summary>
        private byte[] SpStw
        {
            get => this["SP_STW"];
            set => this["SP_STW"] = value;
        }

        /// <summary>
        /// Lap Line Area X.
        /// </summary>
        private byte[] SpLspx
        {
            get => this["SP_LSPX"];
            set => this["SP_LSPX"] = value;
        }

        /// <summary>
        /// Lap Line Area Y.
        /// </summary>
        private byte[] SpLspy
        {
            get => this["SP_LSPY"];
            set => this["SP_LSPY"] = value;
        }

        /// <summary>
        /// Lap Line Area Width.
        /// </summary>
        private byte[] SpLspw
        {
            get => this["SP_LSPW"];
            set => this["SP_LSPW"] = value;
        }

        /// <summary>
        /// Lap Line Area Height.
        /// </summary>
        private byte[] SpLsph
        {
            get => this["SP_LSPH"];
            set => this["SP_LSPH"] = value;
        }

        /// <summary>
        /// Lap Line Y.
        /// </summary>
        private byte[] SpLsly
        {
            get => this["SP_LSLY"];
            set => this["SP_LSLY"] = value;
        }

        /// <summary>
        /// Theme.
        /// </summary>
        private byte[] SpRegion
        {
            get => this["SP_REGION"];
            set => this["SP_REGION"] = value;
        }

        /// <summary>
        /// Battle Starting Position for Player 1.
        /// </summary>
        private byte[] EEBattleStart1
        {
            get => this["EE_BATTLESTART1"];
            set => this["EE_BATTLESTART1"] = value;
        }

        /// <summary>
        /// Battle Starting Position for Player 2.
        /// </summary>
        private byte[] EEBattleStart2
        {
            get => this["EE_BATTLESTART2"];
            set => this["EE_BATTLESTART2"] = value;
        }

        /// <summary>
        /// Object Tileset.
        /// </summary>
        private byte[] EEObjTileset
        {
            get => this["EE_OBJTILESET"];
            set => this["EE_OBJTILESET"] = value;
        }

        /// <summary>
        /// Object Interaction.
        /// </summary>
        private byte[] EEObjInteract
        {
            get => this["EE_OBJINTERACT"];
            set => this["EE_OBJINTERACT"] = value;
        }

        /// <summary>
        /// Object Routine.
        /// </summary>
        private byte[] EEObjRoutine
        {
            get => this["EE_OBJROUTINE"];
            set => this["EE_OBJROUTINE"] = value;
        }

        /// <summary>
        /// Object Palettes.
        /// </summary>
        private byte[] EEObjPalettes
        {
            get => this["EE_OBJPALETTES"];
            set => this["EE_OBJPALETTES"] = value;
        }

        /// <summary>
        /// Object Flashing.
        /// </summary>
        private byte[] EEObjFlashing
        {
            get => this["EE_OBJFLASHING"];
            set => this["EE_OBJFLASHING"] = value;
        }

        /// <summary>
        /// Item probability set index.
        /// </summary>
        private byte[] EEItemProba
        {
            get => this["EE_ITEMPROBA"];
            set => this["EE_ITEMPROBA"] = value;
        }

        // Object Behavior.
        // NOTE: Data ignored by Epic Edit, supported differently.
        // private byte[] SP_OPN;

        /// <summary>
        /// Tile Map.
        /// </summary>
        private byte[] MapBytes
        {
            get => this["MAP"];
            set => this["MAP"] = value;
        }

        // NOTE: Data ignored by Epic Edit, supported differently.
        // private byte[] MapMask;

        /// <summary>
        /// Overlay Tiles.
        /// </summary>
        private byte[] Gpex
        {
            get => this["GPEX"];
            set => this["GPEX"] = value;
        }

        /// <summary>
        /// AI.
        /// </summary>
        private byte[] Area
        {
            get => this["AREA"];
            set => this["AREA"] = value;
        }

        /// <summary>
        /// Objects.
        /// </summary>
        private byte[] Obj
        {
            get => this["OBJ"];
            set => this["OBJ"] = value;
        }

        /// <summary>
        /// Object View Areas.
        /// </summary>
        private byte[] AreaBorder
        {
            get => this["AREA_BORDER"];
            set => this["AREA_BORDER"] = value;
        }

        public MakeTrack(Track track, Game game)
        {
            _track = track;
            _game = game;
            _fields = new Dictionary<string, byte[]>();
            InitFields();
        }

        /// <summary>
        /// Initializes fields, and sets default values (in case the imported file or the loaded data is incomplete).
        /// </summary>
        private void InitFields()
        {
            SpStx = new byte[2];
            SpSty = new byte[2];
            SpStw = new byte[2];
            SpLspx = new byte[2];
            SpLspy = new byte[2];
            SpLspw = new byte[2];
            SpLsph = new byte[2];
            SpLsly = new byte[2];
            SpRegion = new byte[] { 0, 2 };

            EEBattleStart1 = new byte[] { 0x00, 0x02, 0x78, 0x02 };
            EEBattleStart2 = new byte[] { 0x00, 0x02, 0x88, 0x01 };
            EEObjTileset = new byte[2];
            EEObjInteract = new byte[2];
            EEObjRoutine = new byte[2];
            EEObjPalettes = new byte[4];
            EEObjFlashing = new byte[2];
            EEItemProba = new byte[2];

            MapBytes = new byte[TrackMap.SquareSize];

            byte[] gpex = new byte[OverlayTiles.Size];
            for (int i = 0; i < gpex.Length; i++)
            {
                gpex[i] = 0xFF;
            }
            Gpex = gpex;

            byte[] area = new byte[4064];
            for (int i = 0; i < area.Length; i++)
            {
                area[i] = 0xFF;
            }
            Area = area;

            Obj = new byte[64];

            byte[] areaBorder = new byte[TrackObjectAreas.Size];
            for (int i = 0; i < areaBorder.Length; i++)
            {
                areaBorder[i] = 0xFF;
            }
            AreaBorder = areaBorder;
        }

        /// <summary>
        /// Converts the MAKE AI data into the target and area data format that Epic Edit expects.
        /// </summary>
        private void GetAIData(out byte[] targetData, out byte[] areaData)
        {
            List<byte> targetDataList = new List<byte>();
            List<byte> areaDataList = new List<byte>();

            int count = Area.Length / LineLength;

            for (int x = 0; x < count && Area[x * LineLength] != 0xFF; x++)
            {
                // Reorder the target data bytes
                targetDataList.Add(Area[x * LineLength + 1]);
                targetDataList.Add(Area[x * LineLength + 2]);
                targetDataList.Add(Area[x * LineLength]);

                byte areaShape = Area[x * LineLength + 16];
                areaDataList.Add(areaShape);
                areaDataList.Add(Area[x * LineLength + 17]);
                areaDataList.Add(Area[x * LineLength + 18]);
                areaDataList.Add(Area[x * LineLength + 19]);
                if (areaShape == 0x00) // Rectangle, the fifth byte is not needed if the shape is not a rectangle
                {
                    areaDataList.Add(Area[x * LineLength + 20]);
                }
            }

            targetData = targetDataList.ToArray();
            areaData = areaDataList.ToArray();
        }

        /// <summary>
        /// Sets the AI data with the TrackAI object converted into the MAKE track format.
        /// </summary>
        private void SetAIData(TrackAI ai)
        {
            byte[] data = ai.GetBytes();
            int index = 0;

            for (int x = 0; x < ai.ElementCount; x++)
            {
                Area[x * LineLength] = data[data.Length - (ai.ElementCount - x) * 3 + 2];
                Area[x * LineLength + 1] = data[data.Length - (ai.ElementCount - x) * 3];
                Area[x * LineLength + 2] = data[data.Length - (ai.ElementCount - x) * 3 + 1];

                byte areaShape = data[index++];
                Area[x * LineLength + 16] = areaShape;
                Area[x * LineLength + 17] = data[index++];
                Area[x * LineLength + 18] = data[index++];
                Area[x * LineLength + 19] = data[index++];
                if (areaShape == 0x00) // Rectangle, the fifth byte is not needed if the shape is not a rectangle
                {
                    Area[x * LineLength + 20] = data[index++];
                }
            }
        }

        /// <summary>
        /// Loads the MAKE track file data.
        /// </summary>
        public void Load(string filePath)
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (TextReader reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Length == 0 || line[0] != '#')
                    {
                        continue;
                    }

                    int index = line.IndexOf(' ');
                    string fieldName = index == -1 ? line : line.Substring(0, index);
                    fieldName = fieldName.Substring(1); // Remove leading #

                    if (_fields.TryGetValue(fieldName, out byte[] data))
                    {
                        if (data.Length <= 4)
                        {
                            LoadLineData(data, line);
                        }
                        else
                        {
                            LoadBlockData(data, reader);
                        }
                    }
                }
            }
        }

        private static void LoadLineData(byte[] data, string line)
        {
            int space = line.IndexOf(' ');
            line = line.Substring(space).Trim();
            if (line.Length != data.Length * 2)
            {
                // Data length is higher or lower than expected
                throw new ArgumentException("Invalid data length. Import aborted.", nameof(data));
            }

            Utilities.LoadBytesFromHexString(data, line);
        }

        private static void LoadBlockData(byte[] data, TextReader reader)
        {
            int index = 0;
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line) && line[0] == '#')
            {
                byte[] lineBytes = Utilities.HexStringToBytes(line.Substring(1));
                int lineBytesLength = lineBytes.Length;

                if (index + lineBytesLength > data.Length)
                {
                    // Data length is higher than expected
                    throw new ArgumentException("Invalid data length. Import aborted.", nameof(data));
                }

                Buffer.BlockCopy(lineBytes, 0, data, index, lineBytesLength);
                line = reader.ReadLine();
                index += lineBytesLength;
            }

            if (index != data.Length)
            {
                // Data length is lower than expected
                throw new ArgumentException("Invalid data length. Import aborted.", nameof(data));
            }
        }

        /// <summary>
        /// Saves the data as a MAKE track file.
        /// </summary>
        public void Save(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("; Generated with " + Application.ProductName).AppendLine();

            foreach (KeyValuePair<string, byte[]> field in _fields)
            {
                if (field.Value.Length <= 4)
                {
                    sb.AppendLine("#" + field.Key + " " + Utilities.BytesToHexString(field.Value));
                }
                else
                {
                    sb.AppendLine();
                    sb.AppendLine("#" + field.Key);
                    AppendBlockData(sb, field.Value);
                }
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        private static void AppendBlockData(StringBuilder sb, byte[] data)
        {
            int lineLength = LineLength; // Byte count per line
            int lineCount = data.Length / lineLength;

            if (lineCount == 0)
            {
                lineCount = 1;
                lineLength = data.Length;
            }

            for (int y = 0; y < lineCount; y++)
            {
                byte[] lineBytes = Utilities.ReadBlock(data, y * lineLength, lineLength);
                sb.AppendLine("#" + Utilities.BytesToHexString(lineBytes));
            }
        }
    }
}