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
    internal class OptimalCompressorRateTests
    {
        private OptimalCompressor _compressor;
        private byte[] _romBuffer;
        private Game _game;

        [SetUp]
        public void SetUp()
        {
            _compressor = new OptimalCompressor();
            _romBuffer = File.ReadRom(Region.US);
            _game = File.GetGame(Region.US);
        }

        private void CheckCompression(int offset, int expectedSize)
        {
            int originalCompressedSize = Codec.GetCompressedLength(_romBuffer, offset);
            byte[] decompressedData = Codec.Decompress(File.ReadBlock(_romBuffer, offset, originalCompressedSize));
            byte[] buffer = _compressor.Compress(decompressedData);
            int compressedSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedSize);
        }

        private void CheckCompressionFromDoubleCompressed(int offset, int expectedSize)
        {
            int originalCompressedSize = Codec.GetCompressedLength(_romBuffer, offset);
            byte[] decompressedData = Codec.Decompress(Codec.Decompress(File.ReadBlock(_romBuffer, offset, originalCompressedSize)));
            byte[] buffer = _compressor.Compress(decompressedData);
            int compressedSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedSize);
        }

        private void CheckTrackCompression(int trackGroupId, int trackId, int expectedSize)
        {
            Track track = _game.TrackGroups[trackGroupId][trackId];
            byte[] buffer = _compressor.Compress(track.Map.GetBytes());
            int compressedMapSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedMapSize);
        }

        [Test]
        public void TestGhostPillarGraphics()
        {
            CheckCompression(0, 728);
        }

        [Test]
        public void TestMontyMoleGraphics()
        {
            CheckCompression(0x5D6, 710);
        }

        [Test]
        public void TestWinnerFlagGraphics()
        {
            CheckCompression(0xBB7, 657);
        }

        [Test]
        public void TestThwompGraphics()
        {
            CheckCompression(0x1070, 961);
        }

        [Test]
        public void TestLakituGraphics()
        {
            CheckCompression(0x10000, 2297);
        }

        [Test]
        public void TestPiranhaPlantGraphics()
        {
            CheckCompression(0x10AA5, 1232);
        }

        [Test]
        public void TestPipeGraphics()
        {
            CheckCompression(0x10F9B, 779);
        }

        [Test]
        public void TestChompGraphics()
        {
            CheckCompression(0x60000, 350);
        }

        [Test]
        public void TestPodiumGraphics()
        {
            CheckCompressionFromDoubleCompressed(0x737DA, 9277);
        }

        [Test]
        public void TestTrackMap1()
        {
            CheckTrackCompression(0, 0, 1596);
        }

        [Test]
        public void TestTrackMap2()
        {
            CheckTrackCompression(0, 1, 4095);
        }

        [Test]
        public void TestTrackMap3()
        {
            CheckTrackCompression(0, 2, 1040);
        }

        [Test]
        public void TestTrackMap4()
        {
            CheckTrackCompression(0, 3, 2397);
        }

        [Test]
        public void TestTrackMap5()
        {
            CheckTrackCompression(0, 4, 2056);
        }

        [Test]
        public void TestTrackMap6()
        {
            CheckTrackCompression(1, 0, 2587);
        }

        [Test]
        public void TestTrackMap7()
        {
            CheckTrackCompression(1, 1, 1196);
        }

        [Test]
        public void TestTrackMap8()
        {
            CheckTrackCompression(1, 2, 4296);
        }

        [Test]
        public void TestTrackMap9()
        {
            CheckTrackCompression(1, 3, 2820);
        }

        [Test]
        public void TestTrackMap10()
        {
            CheckTrackCompression(1, 4, 2623);
        }

        [Test]
        public void TestTrackMap11()
        {
            CheckTrackCompression(2, 0, 3143);
        }

        [Test]
        public void TestTrackMap12()
        {
            CheckTrackCompression(2, 1, 3370);
        }

        [Test]
        public void TestTrackMap13()
        {
            CheckTrackCompression(2, 2, 2845);
        }

        [Test]
        public void TestTrackMap14()
        {
            CheckTrackCompression(2, 3, 3092);
        }

        [Test]
        public void TestTrackMap15()
        {
            CheckTrackCompression(2, 4, 2524);
        }

        [Test]
        public void TestTrackMap16()
        {
            CheckTrackCompression(3, 0, 4289);
        }

        [Test]
        public void TestTrackMap17()
        {
            CheckTrackCompression(3, 1, 3913);
        }

        [Test]
        public void TestTrackMap18()
        {
            CheckTrackCompression(3, 2, 1280);
        }

        [Test]
        public void TestTrackMap19()
        {
            CheckTrackCompression(3, 3, 3030);
        }

        [Test]
        public void TestTrackMap20()
        {
            CheckTrackCompression(3, 4, 1003);
        }

        [Test]
        public void TestTrackMap21()
        {
            CheckTrackCompression(4, 0, 572);
        }

        [Test]
        public void TestTrackMap22()
        {
            CheckTrackCompression(4, 1, 744);
        }

        [Test]
        public void TestTrackMap23()
        {
            CheckTrackCompression(4, 2, 585);
        }

        [Test]
        public void TestTrackMap24()
        {
            CheckTrackCompression(4, 3, 1003);
        }
    }
}
