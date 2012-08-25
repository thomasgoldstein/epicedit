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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

using EpicEdit.Rom.Tracks;
using EpicEdit.Rom.Tracks.AI;
using EpicEdit.Rom.Tracks.Objects;
using EpicEdit.Rom.Tracks.Overlay;
using EpicEdit.Rom.Tracks.Road;
using EpicEdit.Rom.Tracks.Start;

namespace EpicEdit.Rom
{
    /// <summary>
    /// Represents the data found in a MAKE exported track file.
    /// </summary>
    internal class MakeTrack
    {
        private Track track;
        private Game game;

        public TrackMap Map
        {
            get { return new TrackMap(this.MAP); }
            set { this.MAP = value.GetBytes(); }
        }

        public Theme Theme
        {
            get { return this.game.Themes[this.SP_REGION[1] >> 1]; }
            set { this.SP_REGION = new byte[] { 0, this.game.Themes.GetThemeId(value) }; }
        }

        public OverlayTiles OverlayTiles
        {
            get
            {
                return new OverlayTiles(this.GPEX,
                                        this.game.OverlayTileSizes,
                                        this.game.OverlayTilePatterns);
            }
            set { this.GPEX = value.GetBytes(); }
        }

        public GPStartPosition StartPosition
        {
            get
            {
                short x, y, w;
                if (BitConverter.IsLittleEndian)
                {
                    x = BitConverter.ToInt16(new byte[] { this.SP_STX[1], this.SP_STX[0] }, 0);
                    y = BitConverter.ToInt16(new byte[] { this.SP_STY[1], this.SP_STY[0] }, 0);
                    w = BitConverter.ToInt16(new byte[] { this.SP_STW[1], this.SP_STW[0] }, 0);
                }
                else
                {
                    x = BitConverter.ToInt16(this.SP_STX, 0);
                    y = BitConverter.ToInt16(this.SP_STY, 0);
                    w = BitConverter.ToInt16(this.SP_STW, 0);
                }

                return new GPStartPosition(x, y, w);
            }
            set
            {
                this.SP_STX = new byte[] { (byte)((value.X & 0xFF00) >> 8), (byte)(value.X & 0xFF) };
                this.SP_STY = new byte[] { (byte)((value.Y & 0xFF00) >> 8), (byte)(value.Y & 0xFF) };
                this.SP_STW = new byte[] { (byte)((value.SecondRowOffset & 0xFF00) >> 8), (byte)(value.SecondRowOffset & 0xFF) };
            }
        }

        public LapLine LapLine
        {
            get
            {
                byte[] data = new byte[]
                {
                    this.SP_LSLY[1],
                    this.SP_LSLY[0],
                    this.SP_LSPX[1],
                    this.SP_LSPY[1],
                    this.SP_LSPW[1],
                    this.SP_LSPH[1]
                };
                return new LapLine(data);
            }
            set
            {
                byte[] data = value.GetBytes();
                this.SP_LSLY = new byte[] { data[1], data[0] };
                this.SP_LSPX = new byte[] { 0, data[2] };
                this.SP_LSPY = new byte[] { 0, data[3] };
                this.SP_LSPW = new byte[] { 0, data[4] };
                this.SP_LSPH = new byte[] { 0, data[5] };
            }
        }

        public BattleStartPosition StartPositionP1
        {
            get { return new BattleStartPosition(this.EE_BATTLESTART1); }
            set { this.EE_BATTLESTART1 = value.GetBytes(); }
        }

        public BattleStartPosition StartPositionP2
        {
            get { return new BattleStartPosition(this.EE_BATTLESTART2); }
            set { this.EE_BATTLESTART2 = value.GetBytes(); }
        }

        public TrackObjects Objects
        {
            get
            {
                byte[] data = Utilities.ReadBlock(this.OBJ, 0, TrackObjects.Size);
                return new TrackObjects(data);
            }
            set
            {
                int size = this.OBJ.Length;
                byte[] data = value.GetBytes();
                Array.Resize<byte>(ref data, size);
                this.OBJ = data;
            }
        }

        public TrackObjectZones ObjectZones
        {
            get { return new TrackObjectZones(this.AREA_BORDER, this.track as GPTrack); }
            set { this.AREA_BORDER = value.GetBytes(); }
        }

        public ObjectType ObjectTileset
        {
            get { return (ObjectType)this.EE_OBJTILESET[1]; }
            set { this.EE_OBJTILESET = new byte[] { 0, (byte)value }; }
        }

        public ObjectType ObjectInteraction
        {
            get { return (ObjectType)this.EE_OBJINTERACT[1]; }
            set { this.EE_OBJINTERACT = new byte[] { 0, (byte)value }; }
        }

        public ObjectType ObjectRoutine
        {
            get { return (ObjectType)this.EE_OBJROUTINE[1]; }
            set { this.EE_OBJROUTINE = new byte[] { 0, (byte)value }; }
        }

        public byte[] ObjectPaletteIndexes
        {
            get { return this.EE_OBJPALETTES; }
            set { this.EE_OBJPALETTES = value; }
        }

        public bool ObjectFlashing
        {
            get { return this.EE_OBJFLASHING[1] != 0; }
            set { this.EE_OBJFLASHING[1] = value ? (byte)1 : (byte)0; }
        }

        public TrackAI AI
        {
            get
            {
                byte[] targetData, zoneData;
                this.GetAIData(out targetData, out zoneData);
                return new TrackAI(zoneData, targetData, this.track);
            }
            set { this.SetAIData(value); }
        }

        public int ItemProbabilityIndex
        {
            get { return this.EE_ITEMPROBA[1] >> 1; }
            set { this.EE_ITEMPROBA = new byte[] { 0, (byte)(value << 1) }; }
        }

        private Dictionary<string, byte[]> fields;

        public byte[] this[string name]
        {
            get { return this.fields[name]; }
            set
            {
                if (!this.fields.ContainsKey(name))
                {
                    this.fields.Add(name, null);
                }

                this.fields[name] = value;
            }
        }

        /// <summary>
        /// GP Start Position X.
        /// </summary>
        private byte[] SP_STX
        {
            get { return this["SP_STX"]; }
            set { this["SP_STX"] = value; }
        }

        /// <summary>
        /// GP Start Position Y.
        /// </summary>
        private byte[] SP_STY
        {
            get { return this["SP_STY"]; }
            set { this["SP_STY"] = value; }
        }

        /// <summary>
        /// GP Start Position Width (2nd Row Offset).
        /// </summary>
        private byte[] SP_STW
        {
            get { return this["SP_STW"]; }
            set { this["SP_STW"] = value; }
        }

        /// <summary>
        /// Lap Line Area X.
        /// </summary>
        private byte[] SP_LSPX
        {
            get { return this["SP_LSPX"]; }
            set { this["SP_LSPX"] = value; }
        }

        /// <summary>
        /// Lap Line Area Y.
        /// </summary>
        private byte[] SP_LSPY
        {
            get { return this["SP_LSPY"]; }
            set { this["SP_LSPY"] = value; }
        }

        /// <summary>
        /// Lap Line Area Width.
        /// </summary>
        private byte[] SP_LSPW
        {
            get { return this["SP_LSPW"]; }
            set { this["SP_LSPW"] = value; }
        }

        /// <summary>
        /// Lap Line Area Height.
        /// </summary>
        private byte[] SP_LSPH
        {
            get { return this["SP_LSPH"]; }
            set { this["SP_LSPH"] = value; }
        }

        /// <summary>
        /// Lap Line Y.
        /// </summary>
        private byte[] SP_LSLY
        {
            get { return this["SP_LSLY"]; }
            set { this["SP_LSLY"] = value; }
        }

        /// <summary>
        /// Theme.
        /// </summary>
        private byte[] SP_REGION
        {
            get { return this["SP_REGION"]; }
            set { this["SP_REGION"] = value; }
        }

        /// <summary>
        /// Battle Starting Position for Player 1.
        /// </summary>
        private byte[] EE_BATTLESTART1
        {
            get { return this["EE_BATTLESTART1"]; }
            set { this["EE_BATTLESTART1"] = value; }
        }

        /// <summary>
        /// Battle Starting Position for Player 2.
        /// </summary>
        private byte[] EE_BATTLESTART2
        {
            get { return this["EE_BATTLESTART2"]; }
            set { this["EE_BATTLESTART2"] = value; }
        }

        /// <summary>
        /// Object Tileset.
        /// </summary>
        private byte[] EE_OBJTILESET
        {
            get { return this["EE_OBJTILESET"]; }
            set { this["EE_OBJTILESET"] = value; }
        }

        /// <summary>
        /// Object Interaction.
        /// </summary>
        private byte[] EE_OBJINTERACT
        {
            get { return this["EE_OBJINTERACT"]; }
            set { this["EE_OBJINTERACT"] = value; }
        }

        /// <summary>
        /// Object Routine.
        /// </summary>
        private byte[] EE_OBJROUTINE
        {
            get { return this["EE_OBJROUTINE"]; }
            set { this["EE_OBJROUTINE"] = value; }
        }

        /// <summary>
        /// Object Palettes.
        /// </summary>
        private byte[] EE_OBJPALETTES
        {
            get { return this["EE_OBJPALETTES"]; }
            set { this["EE_OBJPALETTES"] = value; }
        }

        /// <summary>
        /// Object Flashing.
        /// </summary>
        private byte[] EE_OBJFLASHING
        {
            get { return this["EE_OBJFLASHING"]; }
            set { this["EE_OBJFLASHING"] = value; }
        }

        /// <summary>
        /// Item probability set index.
        /// </summary>
        private byte[] EE_ITEMPROBA
        {
            get { return this["EE_ITEMPROBA"]; }
            set { this["EE_ITEMPROBA"] = value; }
        }

        // Object Behavior.
        // NOTE: Data ignored by Epic Edit, supported differently.
        // private byte[] SP_OPN;

        /// <summary>
        /// Tile Map.
        /// </summary>
        private byte[] MAP
        {
            get { return this["MAP"]; }
            set { this["MAP"] = value; }
        }

        // NOTE: Data ignored by Epic Edit, supported differently.
        // private byte[] MAPMASK;

        /// <summary>
        /// Overlay Tiles.
        /// </summary>
        private byte[] GPEX
        {
            get { return this["GPEX"]; }
            set { this["GPEX"] = value; }
        }

        /// <summary>
        /// AI.
        /// </summary>
        private byte[] AREA
        {
            get { return this["AREA"]; }
            set { this["AREA"] = value; }
        }

        /// <summary>
        /// Objects.
        /// </summary>
        private byte[] OBJ
        {
            get { return this["OBJ"]; }
            set { this["OBJ"] = value; }
        }

        /// <summary>
        /// Object View Zones.
        /// </summary>
        private byte[] AREA_BORDER
        {
            get { return this["AREA_BORDER"]; }
            set { this["AREA_BORDER"] = value; }
        }

        public MakeTrack(Track track, Game game)
        {
            this.track = track;
            this.game = game;
            this.InitFields();
        }

        /// <summary>
        /// Initializes fields, and sets default values (in case the imported file or the loaded data is incomplete).
        /// </summary>
        private void InitFields()
        {
            this.fields = new Dictionary<string, byte[]>();

            this.SP_STX = new byte[2];
            this.SP_STY = new byte[2];
            this.SP_STW = new byte[2];
            this.SP_LSPX = new byte[2];
            this.SP_LSPY = new byte[2];
            this.SP_LSPW = new byte[2];
            this.SP_LSPH = new byte[2];
            this.SP_LSLY = new byte[2];
            this.SP_REGION = new byte[] { 0, 2 };

            this.EE_BATTLESTART1 = new byte[] { 0x00, 0x02, 0x78, 0x02 };
            this.EE_BATTLESTART2 = new byte[] { 0x00, 0x02, 0x88, 0x01 };
            this.EE_OBJTILESET = new byte[2];
            this.EE_OBJINTERACT = new byte[2];
            this.EE_OBJROUTINE = new byte[2];
            this.EE_OBJPALETTES = new byte[4];
            this.EE_OBJFLASHING = new byte[2];
            this.EE_ITEMPROBA = new byte[2];

            this.MAP = new byte[TrackMap.SquareSize];

            byte[] gpex = new byte[OverlayTiles.Size];
            for (int i = 0; i < gpex.Length; i++)
            {
                gpex[i] = 0xFF;
            }
            this.GPEX = gpex;

            byte[] area = new byte[4064];
            for (int i = 0; i < area.Length; i++)
            {
                area[i] = 0xFF;
            }
            this.AREA = area;

            this.OBJ = new byte[64];

            byte[] areaBorder = new byte[TrackObjectZones.Size];
            for (int i = 0; i < areaBorder.Length; i++)
            {
                areaBorder[i] = 0xFF;
            }
            this.AREA_BORDER = areaBorder;
        }

        /// <summary>
        /// Converts the MAKE AI data into the target and zone data format that Epic Edit expects.
        /// </summary>
        private void GetAIData(out byte[] targetData, out byte[] zoneData)
        {
            int lineLength = 32; // Byte count per line

            List<byte> targetDataList = new List<byte>();
            List<byte> zoneDataList = new List<byte>();

            int count = this.AREA.Length / lineLength;

            for (int x = 0; x < count; x++)
            {
                if (this.AREA[x * lineLength] != 0xFF)
                {
                    // Reorder the target data bytes
                    targetDataList.Add(this.AREA[x * lineLength + 1]);
                    targetDataList.Add(this.AREA[x * lineLength + 2]);
                    targetDataList.Add(this.AREA[x * lineLength]);

                    byte zoneShape = this.AREA[x * lineLength + 16];
                    zoneDataList.Add(zoneShape);
                    zoneDataList.Add(this.AREA[x * lineLength + 17]);
                    zoneDataList.Add(this.AREA[x * lineLength + 18]);
                    zoneDataList.Add(this.AREA[x * lineLength + 19]);
                    if (zoneShape == 0x00) // Rectangle, the fifth byte is not needed if the shape is not a rectangle
                    {
                        zoneDataList.Add(this.AREA[x * lineLength + 20]);
                    }
                }
            }

            targetData = targetDataList.ToArray();
            zoneData = zoneDataList.ToArray();
        }

        /// <summary>
        /// Sets the AI data with the TrackAI object converted into the MAKE track format.
        /// </summary>
        private void SetAIData(TrackAI ai)
        {
            int lineLength = 32; // Byte count per line
            byte[] data = ai.GetBytes();
            int index = 0;

            for (int x = 0; x < ai.ElementCount; x++)
            {
                this.AREA[x * lineLength] = data[data.Length - (ai.ElementCount - x) * 3 + 2];
                this.AREA[x * lineLength + 1] = data[data.Length - (ai.ElementCount - x) * 3];
                this.AREA[x * lineLength + 2] = data[data.Length - (ai.ElementCount - x) * 3 + 1];

                byte zoneShape = data[index++];
                this.AREA[x * lineLength + 16] = zoneShape;
                this.AREA[x * lineLength + 17] = data[index++];
                this.AREA[x * lineLength + 18] = data[index++];
                this.AREA[x * lineLength + 19] = data[index++];
                if (zoneShape == 0x00) // Rectangle, the fifth byte is not needed if the shape is not a rectangle
                {
                    this.AREA[x * lineLength + 20] = data[index++];
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
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line.Length != 0 && line[0] == '#')
                    {
                        int index = line.IndexOf(' ');
                        string fieldName = index == -1 ? line : line.Substring(0, index);
                        fieldName = fieldName.Substring(1); // Remove leading #

                        if (this.fields.ContainsKey(fieldName))
                        {
                            byte[] data = this.fields[fieldName];

                            if (data.Length <= 4)
                            {
                                MakeTrack.LoadLineData(data, line);
                            }
                            else
                            {
                                MakeTrack.LoadBlockData(data, reader);
                            }
                        }
                    }

                    line = reader.ReadLine();
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
                throw new ArgumentOutOfRangeException("data", "Invalid data length. Import aborted.");
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
                    throw new ArgumentOutOfRangeException("data", "Invalid data length. Import aborted.");
                }

                Buffer.BlockCopy(lineBytes, 0, data, index, lineBytesLength);
                line = reader.ReadLine();
                index += lineBytesLength;
            }

            if (index != data.Length)
            {
                // Data length is lower than expected
                throw new ArgumentOutOfRangeException("data", "Invalid data length. Import aborted.");
            }
        }

        /// <summary>
        /// Saves the data as a MAKE track file.
        /// </summary>
        public void Save(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("; Generated with " + Application.ProductName).AppendLine();

            foreach (KeyValuePair<string, byte[]> field in this.fields)
            {
                if (field.Value.Length <= 4)
                {
                    sb.AppendLine("#" + field.Key + " " + Utilities.BytesToHexString(field.Value));
                }
                else
                {
                    sb.AppendLine();
                    sb.AppendLine("#" + field.Key);
                    MakeTrack.AppendBlockData(sb, field.Value);
                }
            }

            File.WriteAllText(filePath, sb.ToString());
        }

        private static void AppendBlockData(StringBuilder sb, byte[] data)
        {
            int lineLength = 32; // Byte count per line
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