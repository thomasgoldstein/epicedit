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
    /// <summary>
    /// Non-regression tests for the compression rate of the FastCompressor.
    /// </summary>
    [TestFixture]
    internal class FastCompressorRateTest
    {
        private readonly FastCompressor compressor;
        private readonly Game game;

        public FastCompressorRateTest()
        {
            this.compressor = new FastCompressor();
            this.game = File.GetGame(Region.US);
        }

        public void CheckTrackCompression(int trackGroupId, int trackId, int expectedSize)
        {
            Track track = this.game.TrackGroups[trackGroupId][trackId];
            byte[] buffer = compressor.Compress(track.Map.GetBytes(), false);
            int compressedMapSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedMapSize);
        }

        [Test]
        public void TestTrackSmk1()
        {
            this.CheckTrackCompression(0, 0, 1705);
        }

        [Test]
        public void TestTrackSmk2()
        {
            this.CheckTrackCompression(0, 1, 4415);
        }

        [Test]
        public void TestTrackSmk3()
        {
            this.CheckTrackCompression(0, 2, 1097);
        }

        [Test]
        public void TestTrackSmk4()
        {
            this.CheckTrackCompression(0, 3, 2565);
        }

        [Test]
        public void TestTrackSmk5()
        {
            this.CheckTrackCompression(0, 4, 2167);
        }

        [Test]
        public void TestTrackSmk6()
        {
            this.CheckTrackCompression(1, 0, 2787);
        }

        [Test]
        public void TestTrackSmk7()
        {
            this.CheckTrackCompression(1, 1, 1260);
        }

        [Test]
        public void TestTrackSmk8()
        {
            this.CheckTrackCompression(1, 2, 4612);
        }

        [Test]
        public void TestTrackSmk9()
        {
            this.CheckTrackCompression(1, 3, 3009);
        }

        [Test]
        public void TestTrackSmk10()
        {
            this.CheckTrackCompression(1, 4, 2787);
        }

        [Test]
        public void TestTrackSmk11()
        {
            this.CheckTrackCompression(2, 0, 3359);
        }

        [Test]
        public void TestTrackSmk12()
        {
            this.CheckTrackCompression(2, 1, 3602);
        }

        [Test]
        public void TestTrackSmk13()
        {
            this.CheckTrackCompression(2, 2, 3066);
        }

        [Test]
        public void TestTrackSmk14()
        {
            this.CheckTrackCompression(2, 3, 3294);
        }

        [Test]
        public void TestTrackSmk15()
        {
            this.CheckTrackCompression(2, 4, 2657);
        }

        [Test]
        public void TestTrackSmk16()
        {
            this.CheckTrackCompression(3, 0, 4567);
        }

        [Test]
        public void TestTrackSmk17()
        {
            this.CheckTrackCompression(3, 1, 4162);
        }

        [Test]
        public void TestTrackSmk18()
        {
            this.CheckTrackCompression(3, 2, 1389);
        }

        [Test]
        public void TestTrackSmk19()
        {
            this.CheckTrackCompression(3, 3, 3231);
        }

        [Test]
        public void TestTrackSmk20()
        {
            this.CheckTrackCompression(3, 4, 1094);
        }

        [Test]
        public void TestTrackSmk21()
        {
            this.CheckTrackCompression(4, 0, 659);
        }

        [Test]
        public void TestTrackSmk22()
        {
            this.CheckTrackCompression(4, 1, 829);
        }

        [Test]
        public void TestTrackSmk23()
        {
            this.CheckTrackCompression(4, 2, 662);
        }

        [Test]
        public void TestTrackSmk24()
        {
            this.CheckTrackCompression(4, 3, 1104);
        }
    }
}
