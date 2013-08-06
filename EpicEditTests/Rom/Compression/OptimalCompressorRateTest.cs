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

namespace EpicEditTests.Rom.Compression
{
    /// <summary>
    /// Non-regression tests for the compression rate of the OptimalCompressor.
    /// </summary>
    [TestFixture]
    internal class OptimalCompressorRateTest
    {
        private OptimalCompressor compressor = new OptimalCompressor();
        private Game smkGame;
        private Game eeGame;

        public OptimalCompressorRateTest()
        {
            this.smkGame = File.GetGame(Region.US);
            this.eeGame = new Game(File.RelativePath + "epicr.smc");
        }

        public void CheckTrackCompression(Game game, int trackGroupId, int trackId, int expectedSize)
        {
            Track track = game.TrackGroups[trackGroupId][trackId];
            byte[] buffer = compressor.Compress(track.Map.GetBytes(), false);
            int compressedTrackSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedTrackSize);
        }

        [Test]
        public void TestTrackSmk1()
        {
            this.CheckTrackCompression(this.smkGame, 0, 0, 1596);
        }

        [Test]
        public void TestTrackSmk2()
        {
            this.CheckTrackCompression(this.smkGame, 0, 1, 4095);
        }

        [Test]
        public void TestTrackSmk3()
        {
            this.CheckTrackCompression(this.smkGame, 0, 2, 1040);
        }

        [Test]
        public void TestTrackSmk4()
        {
            this.CheckTrackCompression(this.smkGame, 0, 3, 2397);
        }

        [Test]
        public void TestTrackSmk5()
        {
            this.CheckTrackCompression(this.smkGame, 0, 4, 2056);
        }

        [Test]
        public void TestTrackSmk6()
        {
            this.CheckTrackCompression(this.smkGame, 1, 0, 2587);
        }

        [Test]
        public void TestTrackSmk7()
        {
            this.CheckTrackCompression(this.smkGame, 1, 1, 1196);
        }

        [Test]
        public void TestTrackSmk8()
        {
            this.CheckTrackCompression(this.smkGame, 1, 2, 4296);
        }

        [Test]
        public void TestTrackSmk9()
        {
            this.CheckTrackCompression(this.smkGame, 1, 3, 2820);
        }

        [Test]
        public void TestTrackSmk10()
        {
            this.CheckTrackCompression(this.smkGame, 1, 4, 2623);
        }

        [Test]
        public void TestTrackSmk11()
        {
            this.CheckTrackCompression(this.smkGame, 2, 0, 3143);
        }

        [Test]
        public void TestTrackSmk12()
        {
            this.CheckTrackCompression(this.smkGame, 2, 1, 3370);
        }

        [Test]
        public void TestTrackSmk13()
        {
            this.CheckTrackCompression(this.smkGame, 2, 2, 2845);
        }

        [Test]
        public void TestTrackSmk14()
        {
            this.CheckTrackCompression(this.smkGame, 2, 3, 3092);
        }

        [Test]
        public void TestTrackSmk15()
        {
            this.CheckTrackCompression(this.smkGame, 2, 4, 2524);
        }

        [Test]
        public void TestTrackSmk16()
        {
            this.CheckTrackCompression(this.smkGame, 3, 0, 4289);
        }

        [Test]
        public void TestTrackSmk17()
        {
            this.CheckTrackCompression(this.smkGame, 3, 1, 3913);
        }

        [Test]
        public void TestTrackSmk18()
        {
            this.CheckTrackCompression(this.smkGame, 3, 2, 1280);
        }

        [Test]
        public void TestTrackSmk19()
        {
            this.CheckTrackCompression(this.smkGame, 3, 3, 3030);
        }

        [Test]
        public void TestTrackSmk20()
        {
            this.CheckTrackCompression(this.smkGame, 3, 4, 1003);
        }

        [Test]
        public void TestTrackSmk21()
        {
            this.CheckTrackCompression(this.smkGame, 4, 0, 572);
        }

        [Test]
        public void TestTrackSmk22()
        {
            this.CheckTrackCompression(this.smkGame, 4, 1, 744);
        }

        [Test]
        public void TestTrackSmk23()
        {
            this.CheckTrackCompression(this.smkGame, 4, 2, 585);
        }

        [Test]
        public void TestTrackSmk24()
        {
            this.CheckTrackCompression(this.smkGame, 4, 3, 1003);
        }

        [Test]
        public void TestTrackEE1()
        {
            this.CheckTrackCompression(this.eeGame, 0, 0, 2950);
        }

        [Test]
        public void TestTrackEE2()
        {
            this.CheckTrackCompression(this.eeGame, 0, 1, 1446);
        }

        [Test]
        public void TestTrackEE3()
        {
            this.CheckTrackCompression(this.eeGame, 0, 2, 3376);
        }

        [Test]
        public void TestTrackEE4()
        {
            this.CheckTrackCompression(this.eeGame, 0, 3, 1241);
        }

        [Test]
        public void TestTrackEE5()
        {
            this.CheckTrackCompression(this.eeGame, 0, 4, 3643);
        }

        [Test]
        public void TestTrackEE6()
        {
            this.CheckTrackCompression(this.eeGame, 1, 0, 3806);
        }

        [Test]
        public void TestTrackEE7()
        {
            this.CheckTrackCompression(this.eeGame, 1, 1, 2919);
        }

        [Test]
        public void TestTrackEE8()
        {
            this.CheckTrackCompression(this.eeGame, 1, 2, 2129);
        }

        [Test]
        public void TestTrackEE9()
        {
            this.CheckTrackCompression(this.eeGame, 1, 3, 1557);
        }

        [Test]
        public void TestTrackEE10()
        {
            this.CheckTrackCompression(this.eeGame, 1, 4, 1699);
        }

        [Test]
        public void TestTrackEE11()
        {
            this.CheckTrackCompression(this.eeGame, 2, 0, 1888);
        }

        [Test]
        public void TestTrackEE12()
        {
            this.CheckTrackCompression(this.eeGame, 2, 1, 1663);
        }

        [Test]
        public void TestTrackEE13()
        {
            this.CheckTrackCompression(this.eeGame, 2, 2, 1360);
        }

        [Test]
        public void TestTrackEE14()
        {
            this.CheckTrackCompression(this.eeGame, 2, 3, 3006);
        }

        [Test]
        public void TestTrackEE15()
        {
            this.CheckTrackCompression(this.eeGame, 2, 4, 1477);
        }

        [Test]
        public void TestTrackEE16()
        {
            this.CheckTrackCompression(this.eeGame, 3, 0, 869);
        }

        [Test]
        public void TestTrackEE17()
        {
            this.CheckTrackCompression(this.eeGame, 3, 1, 3703);
        }

        [Test]
        public void TestTrackEE18()
        {
            this.CheckTrackCompression(this.eeGame, 3, 2, 513);
        }

        [Test]
        public void TestTrackEE19()
        {
            this.CheckTrackCompression(this.eeGame, 3, 3, 688);
        }

        [Test]
        public void TestTrackEE20()
        {
            this.CheckTrackCompression(this.eeGame, 3, 4, 2451);
        }

        [Test]
        public void TestTrackEE21()
        {
            this.CheckTrackCompression(this.eeGame, 4, 0, 904);
        }

        [Test]
        public void TestTrackEE22()
        {
            this.CheckTrackCompression(this.eeGame, 4, 1, 847);
        }

        [Test]
        public void TestTrackEE23()
        {
            this.CheckTrackCompression(this.eeGame, 4, 2, 1016);
        }

        [Test]
        public void TestTrackEE24()
        {
            this.CheckTrackCompression(this.eeGame, 4, 3, 298);
        }
    }
}
