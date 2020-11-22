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

using EpicEdit.Rom;
using EpicEdit.Rom.Tracks.AI;
using NUnit.Framework;
using System;

namespace EpicEdit.Tests.Rom.Tracks.AI
{
    [TestFixture]
    internal class TrackAITests
    {
        private void TestGetBytes(byte[] areaData, byte[] targetData)
        {
            byte[] dataBefore = new byte[areaData.Length + 1 + targetData.Length];
            Array.Copy(areaData, dataBefore, areaData.Length);
            dataBefore[areaData.Length] = 0xFF; // Area data ends with 0xFF
            Array.Copy(targetData, 0, dataBefore, areaData.Length + 1, targetData.Length);

            TrackAI trackAI = new TrackAI(areaData, targetData, null);

            byte[] dataAfter = trackAI.GetBytes();

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytesMarioCircuit1()
        {
            byte[] romBuffer = File.ReadRom(Region.US);
            byte[] areaData = File.ReadBlock(romBuffer, 0x60ECB, 142);
            byte[] targetData = File.ReadBlock(romBuffer, 0x60F5A, 90);
            this.TestGetBytes(areaData, targetData);
        }

        [Test]
        public void TestGetBytesMarioCircuit2()
        {
            // NOTE: Mario Circuit 2 has an intersection, good to test
            byte[] romBuffer = File.ReadRom(Region.US);
            byte[] areaData = File.ReadBlock(romBuffer, 0x617D9, 169);
            byte[] targetData = File.ReadBlock(romBuffer, 0x61883, 105);
            this.TestGetBytes(areaData, targetData);
        }
    }
}
