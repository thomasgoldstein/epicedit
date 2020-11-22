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

using EpicEdit.Rom.Utility;
using System;
using System.Collections.Generic;

namespace EpicEdit.Rom
{
    /// <summary>
    /// Buffer which contains all the changes that need to be saved back into the ROM.
    /// </summary>
    internal class SaveBuffer
    {
        private byte[] romBuffer;
        private readonly Queue<byte[]> savedData;
        private Range range;
        private readonly Region region;

        /// <summary>
        /// Gets the current index value.
        /// </summary>
        public int Index { get; private set; }

        public SaveBuffer(byte[] romBuffer)
        {
            this.romBuffer = romBuffer;
            this.savedData = new Queue<byte[]>();
            this.region = Game.GetRegion(romBuffer);

            const int RangeStart = RomSize.Size512;
            int rangeEnd = Math.Min(this.romBuffer.Length, RomSize.Size1024);
            this.range = new Range(RangeStart, rangeEnd);
            this.Index = this.range.Start;
        }

        /// <summary>
        /// Adds the data to the buffer.
        /// </summary>
        /// <param name="data">The data.</param>
        public void Add(byte[] data)
        {
            this.savedData.Enqueue(data);
            this.Index += data.Length;
        }

        /// <summary>
        /// Adds the data to the buffer, and updates the relevant offset to point to the relocated data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="offsetIndex">The address of the 3-byte offset to the data in the ROM.</param>
        public void Add(byte[] data, int offsetIndex)
        {
            byte[] offset = Utilities.OffsetToBytes(this.Index);
            Buffer.BlockCopy(offset, 0, this.romBuffer, offsetIndex, offset.Length);
            this.Add(data);
        }

        /// <summary>
        /// Adds the compressed data to the buffer, and updates the relevant offset to point to the relocated data.
        /// </summary>
        /// <param name="data">The compressed data.</param>
        /// <param name="offsetIndex">The address of the 3-byte offset to the data in the ROM.</param>
        public void AddCompressed(byte[] data, int offsetIndex)
        {
            if (this.region == Region.US)
            {
                // The US ROM doesn't support compressed data that spans from an (n)xxxx offset
                // to an (n+1)xxxx one. Ie: the leading byte must be the same from start to end,
                // or the data cannot be decompressed properly.
                // Add blank bytes when necessary to avoid issues.
                int start = this.Index;
                int end = start + data.Length;
                if ((start & 0xF0000) < (end & 0xF0000))
                {
                    int blankSize = 0x10000 - (start & 0xFFFF);
                    this.Add(new byte[blankSize]);
                }
            }

            this.Add(data, offsetIndex);
        }

        public bool Includes(int offset)
        {
            return this.range.Includes(offset);
        }

        public byte[] GetRomBuffer()
        {
            // We return the romBuffer rather than simply calling the
            // SaveToRomBuffer void method, because the romBuffer reference
            // may have changed if the ResizeRomBuffer method was called.
            this.SaveToRomBuffer();
            return this.romBuffer;
        }

        private void SaveToRomBuffer()
        {
            this.CheckDataSize();

            // Save data to buffer
            int index = this.range.Start;
            foreach (byte[] dataBlock in savedData)
            {
                Buffer.BlockCopy(dataBlock, 0, this.romBuffer, index, dataBlock.Length);
                index += dataBlock.Length;
            }

            // Wipe out the rest of the range
            for (int i = index; i < this.range.End; i++)
            {
                this.romBuffer[i] = 0xFF;
            }
        }

        private void CheckDataSize()
        {
            // Compute total size of all the saved data to make sure it fits
            int savedDataSize = 0;
            foreach (byte[] dataBlock in this.savedData)
            {
                savedDataSize += dataBlock.Length;
            }

            // Check if all the saved data fits in the range
            if (savedDataSize > this.range.Length)
            {
                if (savedDataSize <= RomSize.Size512)
                {
                    if (this.range.Length == 0 && // If the ROM is 512 KiB (ie: the original SMK ROM size)
                        savedDataSize > RomSize.Size256) // And if the data that needs to be saved is over 256 Kib
                    {
                        this.ExpandRomBuffer(RomSize.Size512);
                    }
                    else
                    {
                        // The ROM size is 512 or 768 KiB
                        // and can be expanded by 256 KiB to make all the data fit
                        this.ExpandRomBuffer(RomSize.Size256);
                    }

                    this.range.End = this.romBuffer.Length;
                }
                else
                {
                    // The data doesn't fit and we can't expand the ROM for more free space
                    throw new InvalidOperationException("It's not possible to fit more data in this ROM.");
                }
            }
        }

        /// <summary>
        /// Expands the ROM buffer by the given value.
        /// </summary>
        /// <param name="expandValue">Number of bytes added to the buffer.</param>
        private void ExpandRomBuffer(int expandValue)
        {
            this.ResizeRomBuffer(this.romBuffer.Length + expandValue);
        }

        /// <summary>
        /// Resize the ROM buffer to the given size.
        /// </summary>
        /// <param name="size">New ROM buffer length.</param>
        private void ResizeRomBuffer(int size)
        {
            if (size > RomSize.Size8192)
            {
                throw new ArgumentOutOfRangeException(nameof(size), "The ROM can't be expanded because the maximum size has been reached.");
            }

            byte[] resizedRomBuffer = new byte[size];
            Buffer.BlockCopy(this.romBuffer, 0, resizedRomBuffer, 0, this.romBuffer.Length);

            this.romBuffer = resizedRomBuffer;
        }
    }
}
