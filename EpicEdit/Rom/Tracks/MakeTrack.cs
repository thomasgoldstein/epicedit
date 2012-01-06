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
                byte[] data = new byte[44];
                Buffer.BlockCopy(this.OBJ, 0, data, 0, data.Length);
                return new TrackObjects(data);
            }
            set
            {
                int size = this.OBJ.Length;
                this.OBJ = value.GetBytes();
                Array.Resize<byte>(ref this.OBJ, size);
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

        /// <summary>
        /// GP Start Position X.
        /// </summary>
        private byte[] SP_STX;

        /// <summary>
        /// GP Start Position Y.
        /// </summary>
        private byte[] SP_STY;

        /// <summary>
        /// GP Start Position Width (2nd Row Offset).
        /// </summary>
        private byte[] SP_STW;

        /// <summary>
        /// Lap Line Area X.
        /// </summary>
        private byte[] SP_LSPX;

        /// <summary>
        /// Lap Line Area Y.
        /// </summary>
        private byte[] SP_LSPY;

        /// <summary>
        /// Lap Line Area Width.
        /// </summary>
        private byte[] SP_LSPW;

        /// <summary>
        /// Lap Line Area Height.
        /// </summary>
        private byte[] SP_LSPH;

        /// <summary>
        /// Lap Line Y.
        /// </summary>
        private byte[] SP_LSLY;

        /// <summary>
        /// Theme.
        /// </summary>
        private byte[] SP_REGION;

        /*/// <summary>
        /// Object Behavior.
        /// </summary>
        private byte[] SP_OPN;*/

        /// <summary>
        /// Tile Map.
        /// </summary>
        private byte[] MAP;

        //private byte[] MAPMASK;

        /// <summary>
        /// Overlay Tiles.
        /// </summary>
        private byte[] GPEX;

        /// <summary>
        /// AI.
        /// </summary>
        private byte[] AREA;

        /// <summary>
        /// Objects.
        /// </summary>
        private byte[] OBJ;

        /// <summary>
        /// Object View Zones.
        /// </summary>
        private byte[] AREA_BORDER;

        /// <summary>
        /// Battle Starting Position for Player 1.
        /// </summary>
        private byte[] EE_BATTLESTART1;

        /// <summary>
        /// Battle Starting Position for Player 2.
        /// </summary>
        private byte[] EE_BATTLESTART2;

        /// <summary>
        /// Object Tileset.
        /// </summary>
        private byte[] EE_OBJTILESET;

        /// <summary>
        /// Object Interaction.
        /// </summary>
        private byte[] EE_OBJINTERACT;

        /// <summary>
        /// Object Routine.
        /// </summary>
        private byte[] EE_OBJROUTINE;

        /// <summary>
        /// Object Palettes.
        /// </summary>
        private byte[] EE_OBJPALETTES;

        /// <summary>
        /// Object Flashing.
        /// </summary>
        private byte[] EE_OBJFLASHING;

        public MakeTrack(Track track, Game game)
        {
            this.track = track;
            this.game = game;
            this.InitFields();
        }

        /// <summary>
        /// Set fields to default values (in case the imported file or the loaded data is incomplete).
        /// </summary>
        private void InitFields()
        {
            this.SP_STX = new byte[2];
            this.SP_STY = new byte[2];
            this.SP_STW = new byte[2];
            this.SP_LSPX = new byte[2];
            this.SP_LSPY = new byte[2];
            this.SP_LSPW = new byte[2];
            this.SP_LSPH = new byte[2];
            this.SP_LSLY = new byte[2];
            this.SP_REGION = new byte[] { 0, 2 };

            this.MAP = new byte[TrackMap.SquareSize];

            this.GPEX = new byte[OverlayTiles.Size];
            for (int i = 0; i < this.GPEX.Length; i++)
            {
                this.GPEX[i] = 0xFF;
            }

            this.AREA = new byte[4064];
            for (int i = 0; i < this.AREA.Length; i++)
            {
                this.AREA[i] = 0xFF;
            }

            this.OBJ = new byte[64];

            this.AREA_BORDER = new byte[10];
            for (int i = 0; i < this.AREA_BORDER.Length; i++)
            {
                this.AREA_BORDER[i] = 0xFF;
            }

            this.EE_BATTLESTART1 = new byte[] { 0x00, 0x02, 0x78, 0x02 };
            this.EE_BATTLESTART2 = new byte[] { 0x00, 0x02, 0x88, 0x01 };

            this.EE_OBJTILESET = new byte[2];
            this.EE_OBJINTERACT = new byte[2];
            this.EE_OBJROUTINE = new byte[2];
            this.EE_OBJPALETTES = new byte[4];
            this.EE_OBJFLASHING = new byte[2];
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
                    if (line.StartsWith("#SP_STX ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_STX, line);
                    }
                    else if (line.StartsWith("#SP_STY ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_STY, line);
                    }
                    else if (line.StartsWith("#SP_STW ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_STW, line);
                    }
                    else if (line.StartsWith("#SP_LSPX ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_LSPX, line);
                    }
                    else if (line.StartsWith("#SP_LSPY ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_LSPY, line);
                    }
                    else if (line.StartsWith("#SP_LSPW ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_LSPW, line);
                    }
                    else if (line.StartsWith("#SP_LSPH ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_LSPH, line);
                    }
                    else if (line.StartsWith("#SP_LSLY ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_LSLY, line);
                    }
                    else if (line.StartsWith("#SP_REGION ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.SP_REGION, line);
                    }
                    /*else if (line.StartsWith("#SP_OPN ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadData(this.SP_OPN, line);
                    }*/
                    else if (line.Equals("#MAP", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadBlockData(this.MAP, reader);
                    }
                    /*else if (line.Equals("#MAPMASK", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadBlockData(this.MAPMASK, reader);
                    }*/
                    else if (line.Equals("#GPEX", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadBlockData(this.GPEX, reader);
                    }
                    else if (line.Equals("#AREA", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadBlockData(this.AREA, reader);
                    }
                    else if (line.Equals("#OBJ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadBlockData(this.OBJ, reader);
                    }
                    else if (line.Equals("#AREA_BORDER", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadBlockData(this.AREA_BORDER, reader);
                    }
                    else if (line.StartsWith("#EE_BATTLESTART1 ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_BATTLESTART1, line);
                    }
                    else if (line.StartsWith("#EE_BATTLESTART2 ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_BATTLESTART2, line);
                    }
                    else if (line.StartsWith("#EE_OBJTILESET ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_OBJTILESET, line);
                    }
                    else if (line.StartsWith("#EE_OBJINTERACT ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_OBJINTERACT, line);
                    }
                    else if (line.StartsWith("#EE_OBJROUTINE ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_OBJROUTINE, line);
                    }
                    else if (line.StartsWith("#EE_OBJPALETTES ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_OBJPALETTES, line);
                    }
                    else if (line.StartsWith("#EE_OBJFLASHING ", StringComparison.Ordinal))
                    {
                        MakeTrack.LoadLineData(this.EE_OBJFLASHING, line);
                    }

                    line = reader.ReadLine();
                }
            }
        }

        private static void LoadLineData(byte[] field, string line)
        {
            int space = line.IndexOf(' ');
            line = line.Substring(space).Trim();
            if (line.Length != field.Length * 2)
            {
                // Data length is higher or lower than expected
                throw new InvalidDataException("Invalid data length. Import aborted.");
            }

            Utilities.LoadBytesFromHexString(field, line);
        }

        private static void LoadBlockData(byte[] field, TextReader reader)
        {
            int index = 0;
            string line = reader.ReadLine();
            while (!string.IsNullOrEmpty(line) && line[0] == '#')
            {
                byte[] lineBytes = Utilities.HexStringToBytes(line.Substring(1));
                int lineBytesLength = lineBytes.Length;

                if (index + lineBytesLength > field.Length)
                {
                    // Data length is higher than expected
                    throw new InvalidDataException("Invalid data length. Import aborted.");
                }

                Buffer.BlockCopy(lineBytes, 0, field, index, lineBytesLength);
                line = reader.ReadLine();
                index += lineBytesLength;
            }

            if (index != field.Length)
            {
                // Data length is lower than expected
                throw new InvalidDataException("Invalid data length. Import aborted.");
            }
        }

        /// <summary>
        /// Saves the data as a MAKE track file.
        /// </summary>
        public void Save(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("; Generated with " + Application.ProductName).AppendLine();

            sb.AppendLine("#SP_STX " + Utilities.BytesToHexString(this.SP_STX));
            sb.AppendLine("#SP_STY " + Utilities.BytesToHexString(this.SP_STY));
            sb.AppendLine("#SP_STW " + Utilities.BytesToHexString(this.SP_STW));
            sb.AppendLine("#SP_LSPX " + Utilities.BytesToHexString(this.SP_LSPX));
            sb.AppendLine("#SP_LSPY " + Utilities.BytesToHexString(this.SP_LSPY));
            sb.AppendLine("#SP_LSPW " + Utilities.BytesToHexString(this.SP_LSPW));
            sb.AppendLine("#SP_LSPH " + Utilities.BytesToHexString(this.SP_LSPH));
            sb.AppendLine("#SP_LSLY " + Utilities.BytesToHexString(this.SP_LSLY));
            sb.AppendLine("#SP_REGION " + Utilities.BytesToHexString(this.SP_REGION));
            // SP_OPN not supported, do not write SP_OPN data

            sb.AppendLine();

            sb.AppendLine("#MAP");
            MakeTrack.AppendBlockData(sb, this.MAP);

            sb.AppendLine();
            // MAP_MASK not supported, do not write MAP_MASK data

            sb.AppendLine("#GPEX");
            MakeTrack.AppendBlockData(sb, this.GPEX);

            sb.AppendLine();

            sb.AppendLine("#AREA");
            MakeTrack.AppendBlockData(sb, this.AREA);

            sb.AppendLine();

            sb.AppendLine("#OBJ");
            MakeTrack.AppendBlockData(sb, this.OBJ);

            sb.AppendLine();

            sb.AppendLine("#AREA_BORDER");
            sb.AppendLine("#" + Utilities.BytesToHexString(this.AREA_BORDER));

            sb.AppendLine();

            // Epic Edit only fields:
            sb.AppendLine("#EE_BATTLESTART1 " + Utilities.BytesToHexString(this.EE_BATTLESTART1));
            sb.AppendLine("#EE_BATTLESTART2 " + Utilities.BytesToHexString(this.EE_BATTLESTART2));

            sb.AppendLine();

            sb.AppendLine("#EE_OBJTILESET " + Utilities.BytesToHexString(this.EE_OBJTILESET));
            sb.AppendLine("#EE_OBJINTERACT " + Utilities.BytesToHexString(this.EE_OBJINTERACT));
            sb.AppendLine("#EE_OBJROUTINE " + Utilities.BytesToHexString(this.EE_OBJROUTINE));
            sb.AppendLine("#EE_OBJPALETTES " + Utilities.BytesToHexString(this.EE_OBJPALETTES));
            sb.AppendLine("#EE_OBJFLASHING " + Utilities.BytesToHexString(this.EE_OBJFLASHING));

            File.WriteAllText(filePath, sb.ToString());
        }

        private static void AppendBlockData(StringBuilder sb, byte[] data)
        {
            int lineLength = 32; // Byte count per line

            for (int y = 0; y < data.Length / lineLength; y++)
            {
                byte[] lineBytes = new byte[lineLength];
                Buffer.BlockCopy(data, y * lineLength, lineBytes, 0, lineLength);
                sb.AppendLine("#" + Utilities.BytesToHexString(lineBytes));
            }
        }
    }
}