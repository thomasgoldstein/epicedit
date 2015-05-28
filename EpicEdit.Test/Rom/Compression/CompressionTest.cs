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
using EpicEdit.Rom;
using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Compression
{
    [TestFixture]
    internal class CompressionTest : TestBase
    {
        private byte[] romBuffer;
        private Game game;

        public override void Init()
        {
            this.romBuffer = File.ReadRom(Region.US);
            this.game = File.GetGame(Region.US);
        }

        private void TestGraphics(int offset, int expectedSize)
        {
            this.TestGraphics(offset, expectedSize, false);
            this.TestGraphics(offset, expectedSize, true);
        }

        private void TestGraphics(int offset, int expectedSize, bool optimal)
        {
            Codec.Optimal = optimal;

            byte[] bufferA = Codec.Decompress(File.ReadBlock(this.romBuffer, offset, expectedSize));
            byte[] bufferB = Codec.Decompress(Codec.Compress(bufferA));

            Assert.AreEqual(expectedSize, Codec.GetLength(this.romBuffer, offset));
            Assert.AreEqual(bufferA, bufferB, "(Optimal: " + optimal + ")");
        }

        [Test]
        public void TestGfxGhostPillar()
        {
            this.TestGraphics(0, 0x334);
        }

        [Test]
        public void TestGfxMountyMole()
        {
            this.TestGraphics(0x5D6, 0x32D);
        }

        [Test]
        public void TestGfxWinnerFlag()
        {
            this.TestGraphics(0xBB7, 0x2DF);
        }

        [Test]
        public void TestGfxThwomp()
        {
            this.TestGraphics(0x1070, 0x429);
        }

        [Test]
        public void TestGfxLakitu()
        {
            this.TestGraphics(0x10000, 0xAA5);
        }

        [Test]
        public void TestGfxPiranhaPlant()
        {
            this.TestGraphics(0x10AA5, 0x4F6);
        }

        [Test]
        public void TestGfxPipe()
        {
            this.TestGraphics(0x10F9B, 0x35D);
        }

        [Test]
        public void TestGfxChomp()
        {
            this.TestGraphics(0x60000, 0x189);
        }

        private void CheckTrackCompression(int trackGroupId, int trackId)
        {
            Track track = this.game.TrackGroups[trackGroupId][trackId];

            byte[] bufferA = track.Map.GetBytes();

            // Test simple compression
            byte[] bufferC1 = Codec.Compress(bufferA);
            byte[] bufferB = Codec.Decompress(bufferC1, 0, 16384);
            Assert.AreEqual(bufferA, bufferB);

            // Test double compression
            byte[] bufferC2 = Codec.Compress(bufferC1);
            bufferB = Codec.Decompress(Codec.Decompress(bufferC2), 0, 16384);
            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrackSmk1()
        {
            this.CheckTrackCompression(0, 0);
        }

        [Test]
        public void TestTrackSmk2()
        {
            this.CheckTrackCompression(0, 1);
        }

        [Test]
        public void TestTrackSmk3()
        {
            this.CheckTrackCompression(0, 2);
        }

        [Test]
        public void TestTrackSmk4()
        {
            this.CheckTrackCompression(0, 3);
        }

        [Test]
        public void TestTrackSmk5()
        {
            this.CheckTrackCompression(0, 4);
        }

        [Test]
        public void TestTrackSmk6()
        {
            this.CheckTrackCompression(1, 0);
        }

        [Test]
        public void TestTrackSmk7()
        {
            this.CheckTrackCompression(1, 1);
        }

        [Test]
        public void TestTrackSmk8()
        {
            this.CheckTrackCompression(1, 2);
        }

        [Test]
        public void TestTrackSmk9()
        {
            this.CheckTrackCompression(1, 3);
        }

        [Test]
        public void TestTrackSmk10()
        {
            this.CheckTrackCompression(1, 4);
        }

        [Test]
        public void TestTrackSmk11()
        {
            this.CheckTrackCompression(2, 0);
        }

        [Test]
        public void TestTrackSmk12()
        {
            this.CheckTrackCompression(2, 1);
        }

        [Test]
        public void TestTrackSmk13()
        {
            this.CheckTrackCompression(2, 2);
        }

        [Test]
        public void TestTrackSmk14()
        {
            this.CheckTrackCompression(2, 3);
        }

        [Test]
        public void TestTrackSmk15()
        {
            this.CheckTrackCompression(2, 4);
        }

        [Test]
        public void TestTrackSmk16()
        {
            this.CheckTrackCompression(3, 0);
        }

        [Test]
        public void TestTrackSmk17()
        {
            this.CheckTrackCompression(3, 1);
        }

        [Test]
        public void TestTrackSmk18()
        {
            this.CheckTrackCompression(3, 2);
        }

        [Test]
        public void TestTrackSmk19()
        {
            this.CheckTrackCompression(3, 3);
        }

        [Test]
        public void TestTrackSmk20()
        {
            this.CheckTrackCompression(3, 4);
        }

        [Test]
        public void TestTrackSmk21()
        {
            this.CheckTrackCompression(4, 0);
        }

        [Test]
        public void TestTrackSmk22()
        {
            this.CheckTrackCompression(4, 1);
        }

        [Test]
        public void TestTrackSmk23()
        {
            this.CheckTrackCompression(4, 2);
        }

        [Test]
        public void TestTrackSmk24()
        {
            this.CheckTrackCompression(4, 3);
        }
    }
}
