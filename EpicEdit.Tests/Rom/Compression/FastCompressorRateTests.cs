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
    /// Non-regression tests for the compression rate of the FastCompressor.
    /// </summary>
    [TestFixture]
    internal class FastCompressorRateTests
    {
        private FastCompressor _compressor;
        private byte[] _romBuffer;
        private Game _game;

        [SetUp]
        public void SetUp()
        {
            _compressor = new FastCompressor();
            _romBuffer = File.ReadRom(Region.US);
            _game = File.GetGame(Region.US);
        }

        private void CheckCompression(int offset, int expectedSize)
        {
            var originalCompressedSize = Codec.GetCompressedLength(_romBuffer, offset);
            var decompressedData = Codec.Decompress(File.ReadBlock(_romBuffer, offset, originalCompressedSize));
            var buffer = _compressor.Compress(decompressedData);
            var compressedSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedSize);
        }

        private void CheckCompressionFromDoubleCompressed(int offset, int expectedSize)
        {
            var originalCompressedSize = Codec.GetCompressedLength(_romBuffer, offset);
            var decompressedData = Codec.Decompress(Codec.Decompress(File.ReadBlock(_romBuffer, offset, originalCompressedSize)));
            var buffer = _compressor.Compress(decompressedData);
            var compressedSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedSize);
        }

        private void CheckTrackCompression(int trackGroupId, int trackId, int expectedSize)
        {
            var track = _game.TrackGroups[trackGroupId][trackId];
            var buffer = _compressor.Compress(track.Map.GetBytes());
            var compressedMapSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedMapSize);
        }

        [Test]
        public void TestGhostPillarGraphics()
        {
            CheckCompression(0, 733);
        }

        [Test]
        public void TestMontyMoleGraphics()
        {
            CheckCompression(0x5D6, 715);
        }

        [Test]
        public void TestWinnerFlagGraphics()
        {
            CheckCompression(0xBB7, 662);
        }

        [Test]
        public void TestThwompGraphics()
        {
            CheckCompression(0x1070, 965);
        }

        [Test]
        public void TestLakituGraphics()
        {
            CheckCompression(0x10000, 2319);
        }

        [Test]
        public void TestPiranhaPlantGraphics()
        {
            CheckCompression(0x10AA5, 1238);
        }

        [Test]
        public void TestPipeGraphics()
        {
            CheckCompression(0x10F9B, 782);
        }

        [Test]
        public void TestChompGraphics()
        {
            CheckCompression(0x60000, 350);
        }

        [Test]
        public void TestPodiumGraphics()
        {
            CheckCompressionFromDoubleCompressed(0x737DA, 9422);
        }

        [Test]
        public void TestTrackMap1()
        {
            CheckTrackCompression(0, 0, 1705);
        }

        [Test]
        public void TestTrackMap2()
        {
            CheckTrackCompression(0, 1, 4415);
        }

        [Test]
        public void TestTrackMap3()
        {
            CheckTrackCompression(0, 2, 1097);
        }

        [Test]
        public void TestTrackMap4()
        {
            CheckTrackCompression(0, 3, 2565);
        }

        [Test]
        public void TestTrackMap5()
        {
            CheckTrackCompression(0, 4, 2167);
        }

        [Test]
        public void TestTrackMap6()
        {
            CheckTrackCompression(1, 0, 2787);
        }

        [Test]
        public void TestTrackMap7()
        {
            CheckTrackCompression(1, 1, 1260);
        }

        [Test]
        public void TestTrackMap8()
        {
            CheckTrackCompression(1, 2, 4612);
        }

        [Test]
        public void TestTrackMap9()
        {
            CheckTrackCompression(1, 3, 3009);
        }

        [Test]
        public void TestTrackMap10()
        {
            CheckTrackCompression(1, 4, 2787);
        }

        [Test]
        public void TestTrackMap11()
        {
            CheckTrackCompression(2, 0, 3359);
        }

        [Test]
        public void TestTrackMap12()
        {
            CheckTrackCompression(2, 1, 3602);
        }

        [Test]
        public void TestTrackMap13()
        {
            CheckTrackCompression(2, 2, 3066);
        }

        [Test]
        public void TestTrackMap14()
        {
            CheckTrackCompression(2, 3, 3294);
        }

        [Test]
        public void TestTrackMap15()
        {
            CheckTrackCompression(2, 4, 2657);
        }

        [Test]
        public void TestTrackMap16()
        {
            CheckTrackCompression(3, 0, 4567);
        }

        [Test]
        public void TestTrackMap17()
        {
            CheckTrackCompression(3, 1, 4162);
        }

        [Test]
        public void TestTrackMap18()
        {
            CheckTrackCompression(3, 2, 1389);
        }

        [Test]
        public void TestTrackMap19()
        {
            CheckTrackCompression(3, 3, 3231);
        }

        [Test]
        public void TestTrackMap20()
        {
            CheckTrackCompression(3, 4, 1094);
        }

        [Test]
        public void TestTrackMap21()
        {
            CheckTrackCompression(4, 0, 659);
        }

        [Test]
        public void TestTrackMap22()
        {
            CheckTrackCompression(4, 1, 829);
        }

        [Test]
        public void TestTrackMap23()
        {
            CheckTrackCompression(4, 2, 662);
        }

        [Test]
        public void TestTrackMap24()
        {
            CheckTrackCompression(4, 3, 1104);
        }
    }
}
