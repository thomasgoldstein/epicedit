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
using EpicEdit.Rom.Compression;
using EpicEdit.Rom.Tracks;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Compression
{
    /// <summary>
    /// Non-regression tests for the compression rate of the OptimalCompressor.
    /// </summary>
    [TestFixture]
    internal class OptimalCompressorRateTest
    {
        private OptimalCompressor compressor;
        private byte[] romBuffer;
        private Game game;

        [SetUp]
        public void SetUp()
        {
            this.compressor = new OptimalCompressor();
            this.romBuffer = File.ReadRom(Region.US);
            this.game = File.GetGame(Region.US);
        }

        private void CheckCompression(int offset, int expectedSize)
        {
            int originalCompressedSize = Codec.GetCompressedLength(this.romBuffer, offset);
            byte[] decompressedData = Codec.Decompress(File.ReadBlock(this.romBuffer, offset, originalCompressedSize));
            byte[] buffer = this.compressor.Compress(decompressedData);
            int compressedSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedSize);
        }

        private void CheckCompressionFromDoubleCompressed(int offset, int expectedSize)
        {
            int originalCompressedSize = Codec.GetCompressedLength(this.romBuffer, offset);
            byte[] decompressedData = Codec.Decompress(Codec.Decompress(File.ReadBlock(this.romBuffer, offset, originalCompressedSize)));
            byte[] buffer = this.compressor.Compress(decompressedData);
            int compressedSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedSize);
        }

        private void CheckTrackCompression(int trackGroupId, int trackId, int expectedSize)
        {
            Track track = this.game.TrackGroups[trackGroupId][trackId];
            byte[] buffer = this.compressor.Compress(track.Map.GetBytes());
            int compressedMapSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedMapSize);
        }

        [Test]
        public void TestGhostPillarGraphics()
        {
            this.CheckCompression(0, 728);
        }

        [Test]
        public void TestMontyMoleGraphics()
        {
            this.CheckCompression(0x5D6, 710);
        }

        [Test]
        public void TestWinnerFlagGraphics()
        {
            this.CheckCompression(0xBB7, 657);
        }

        [Test]
        public void TestThwompGraphics()
        {
            this.CheckCompression(0x1070, 961);
        }

        [Test]
        public void TestLakituGraphics()
        {
            this.CheckCompression(0x10000, 2297);
        }

        [Test]
        public void TestPiranhaPlantGraphics()
        {
            this.CheckCompression(0x10AA5, 1232);
        }

        [Test]
        public void TestPipeGraphics()
        {
            this.CheckCompression(0x10F9B, 779);
        }

        [Test]
        public void TestChompGraphics()
        {
            this.CheckCompression(0x60000, 350);
        }

        [Test]
        public void TestPodiumGraphics()
        {
            this.CheckCompressionFromDoubleCompressed(0x737DA, 9277);
        }

        [Test]
        public void TestTrackMap1()
        {
            this.CheckTrackCompression(0, 0, 1596);
        }

        [Test]
        public void TestTrackMap2()
        {
            this.CheckTrackCompression(0, 1, 4095);
        }

        [Test]
        public void TestTrackMap3()
        {
            this.CheckTrackCompression(0, 2, 1040);
        }

        [Test]
        public void TestTrackMap4()
        {
            this.CheckTrackCompression(0, 3, 2397);
        }

        [Test]
        public void TestTrackMap5()
        {
            this.CheckTrackCompression(0, 4, 2056);
        }

        [Test]
        public void TestTrackMap6()
        {
            this.CheckTrackCompression(1, 0, 2587);
        }

        [Test]
        public void TestTrackMap7()
        {
            this.CheckTrackCompression(1, 1, 1196);
        }

        [Test]
        public void TestTrackMap8()
        {
            this.CheckTrackCompression(1, 2, 4296);
        }

        [Test]
        public void TestTrackMap9()
        {
            this.CheckTrackCompression(1, 3, 2820);
        }

        [Test]
        public void TestTrackMap10()
        {
            this.CheckTrackCompression(1, 4, 2623);
        }

        [Test]
        public void TestTrackMap11()
        {
            this.CheckTrackCompression(2, 0, 3143);
        }

        [Test]
        public void TestTrackMap12()
        {
            this.CheckTrackCompression(2, 1, 3370);
        }

        [Test]
        public void TestTrackMap13()
        {
            this.CheckTrackCompression(2, 2, 2845);
        }

        [Test]
        public void TestTrackMap14()
        {
            this.CheckTrackCompression(2, 3, 3092);
        }

        [Test]
        public void TestTrackMap15()
        {
            this.CheckTrackCompression(2, 4, 2524);
        }

        [Test]
        public void TestTrackMap16()
        {
            this.CheckTrackCompression(3, 0, 4289);
        }

        [Test]
        public void TestTrackMap17()
        {
            this.CheckTrackCompression(3, 1, 3913);
        }

        [Test]
        public void TestTrackMap18()
        {
            this.CheckTrackCompression(3, 2, 1280);
        }

        [Test]
        public void TestTrackMap19()
        {
            this.CheckTrackCompression(3, 3, 3030);
        }

        [Test]
        public void TestTrackMap20()
        {
            this.CheckTrackCompression(3, 4, 1003);
        }

        [Test]
        public void TestTrackMap21()
        {
            this.CheckTrackCompression(4, 0, 572);
        }

        [Test]
        public void TestTrackMap22()
        {
            this.CheckTrackCompression(4, 1, 744);
        }

        [Test]
        public void TestTrackMap23()
        {
            this.CheckTrackCompression(4, 2, 585);
        }

        [Test]
        public void TestTrackMap24()
        {
            this.CheckTrackCompression(4, 3, 1003);
        }
    }
}
