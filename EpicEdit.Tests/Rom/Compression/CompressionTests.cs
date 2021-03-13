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
    [TestFixture]
    internal class CompressionTests
    {
        private byte[] _romBuffer;
        private Game _game;

        [SetUp]
        public void SetUp()
        {
            _romBuffer = File.ReadRom(Region.US);
            _game = File.GetGame(Region.US);
        }

        private void CheckCompression(int offset, int expectedSize)
        {
            CheckCompression(offset, expectedSize, new FastCompressor());
            CheckCompression(offset, expectedSize, new OptimalCompressor());
        }

        private void CheckDoubleCompression(int offset, int expectedSize)
        {
            CheckDoubleCompression(offset, expectedSize, new FastCompressor());
            CheckDoubleCompression(offset, expectedSize, new OptimalCompressor());
        }

        private void CheckCompression(int offset, int expectedSize, ICompressor compressor)
        {
            var bufferA = Codec.Decompress(File.ReadBlock(_romBuffer, offset, expectedSize));
            var bufferB = Codec.Decompress(compressor.Compress(bufferA));

            Assert.AreEqual(expectedSize, bufferA.Length);
            Assert.AreEqual(bufferA, bufferB, "(Compressor: " + compressor.GetType().Name + ")");
        }

        private void CheckDoubleCompression(int offset, int expectedSize, ICompressor compressor)
        {
            var bufferA = Codec.Decompress(Codec.Decompress(File.ReadBlock(_romBuffer, offset, expectedSize)));
            var bufferB = Codec.Decompress(Codec.Decompress(compressor.Compress(compressor.Compress(bufferA))));

            Assert.AreEqual(expectedSize, bufferA.Length);
            Assert.AreEqual(bufferA, bufferB, "(Compressor: " + compressor.GetType().Name + ")");
        }

        private void CheckTrackCompression(int trackGroupId, int trackId)
        {
            CheckTrackCompression(trackGroupId, trackId, new FastCompressor());
            CheckTrackCompression(trackGroupId, trackId, new OptimalCompressor());
        }

        private void CheckTrackCompression(int trackGroupId, int trackId, ICompressor compressor)
        {
            var track = _game.TrackGroups[trackGroupId][trackId];

            var bufferA = track.Map.GetBytes();

            // Test simple compression
            var bufferC1 = compressor.Compress(bufferA);
            var bufferB = Codec.Decompress(bufferC1, 0, 16384);
            Assert.AreEqual(bufferA, bufferB);

            // Test double compression
            var bufferC2 = compressor.Compress(bufferC1);
            bufferB = Codec.Decompress(Codec.Decompress(bufferC2), 0, 16384);
            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestGhostPillarGraphics()
        {
            CheckCompression(0, 0x800);
        }

        [Test]
        public void TestMontyMoleGraphics()
        {
            CheckCompression(0x5D6, 0x720);
        }

        [Test]
        public void TestWinnerFlagGraphics()
        {
            CheckCompression(0xBB7, 0x700);
        }

        [Test]
        public void TestThwompGraphics()
        {
            CheckCompression(0x1070, 0x720);
        }

        [Test]
        public void TestLakituGraphics()
        {
            CheckCompression(0x10000, 0x1380);
        }

        [Test]
        public void TestPiranhaPlantGraphics()
        {
            CheckCompression(0x10AA5, 0x720);
        }

        [Test]
        public void TestPipeGraphics()
        {
            CheckCompression(0x10F9B, 0x720);
        }

        [Test]
        public void TestChompGraphics()
        {
            CheckCompression(0x60000, 0x280);
        }

        [Test]
        public void TestPodiumGraphics()
        {
            CheckDoubleCompression(0x737DA, 0x4A00);
        }

        [Test]
        public void TestTrackMap1()
        {
            CheckTrackCompression(0, 0);
        }

        [Test]
        public void TestTrackMap2()
        {
            CheckTrackCompression(0, 1);
        }

        [Test]
        public void TestTrackMap3()
        {
            CheckTrackCompression(0, 2);
        }

        [Test]
        public void TestTrackMap4()
        {
            CheckTrackCompression(0, 3);
        }

        [Test]
        public void TestTrackMap5()
        {
            CheckTrackCompression(0, 4);
        }

        [Test]
        public void TestTrackMap6()
        {
            CheckTrackCompression(1, 0);
        }

        [Test]
        public void TestTrackMap7()
        {
            CheckTrackCompression(1, 1);
        }

        [Test]
        public void TestTrackMap8()
        {
            CheckTrackCompression(1, 2);
        }

        [Test]
        public void TestTrackMap9()
        {
            CheckTrackCompression(1, 3);
        }

        [Test]
        public void TestTrackMap10()
        {
            CheckTrackCompression(1, 4);
        }

        [Test]
        public void TestTrackMap11()
        {
            CheckTrackCompression(2, 0);
        }

        [Test]
        public void TestTrackMap12()
        {
            CheckTrackCompression(2, 1);
        }

        [Test]
        public void TestTrackMap13()
        {
            CheckTrackCompression(2, 2);
        }

        [Test]
        public void TestTrackMap14()
        {
            CheckTrackCompression(2, 3);
        }

        [Test]
        public void TestTrackMap15()
        {
            CheckTrackCompression(2, 4);
        }

        [Test]
        public void TestTrackMap16()
        {
            CheckTrackCompression(3, 0);
        }

        [Test]
        public void TestTrackMap17()
        {
            CheckTrackCompression(3, 1);
        }

        [Test]
        public void TestTrackMap18()
        {
            CheckTrackCompression(3, 2);
        }

        [Test]
        public void TestTrackMap19()
        {
            CheckTrackCompression(3, 3);
        }

        [Test]
        public void TestTrackMap20()
        {
            CheckTrackCompression(3, 4);
        }

        [Test]
        public void TestTrackMap21()
        {
            CheckTrackCompression(4, 0);
        }

        [Test]
        public void TestTrackMap22()
        {
            CheckTrackCompression(4, 1);
        }

        [Test]
        public void TestTrackMap23()
        {
            CheckTrackCompression(4, 2);
        }

        [Test]
        public void TestTrackMap24()
        {
            CheckTrackCompression(4, 3);
        }
    }
}
