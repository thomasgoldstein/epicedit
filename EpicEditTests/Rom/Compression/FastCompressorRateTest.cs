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
    /// Non-regression tests for the compression rate of the FastCompressor.
    /// </summary>
    [TestFixture]
    internal class FastCompressorRateTest
    {
        private FastCompressor compressor = new FastCompressor();
        private Game smkGame;
        private Game eeGame;

        public FastCompressorRateTest()
        {
            this.smkGame = new Game(File.RelativePath + "smk.smc");
            this.eeGame = new Game(File.RelativePath + "epicr.smc");
        }

        public void CheckTrackCompression(Game game, int trackGroupId, int trackId, int expectedSize)
        {
            Track track = game.GetTrackGroups()[trackGroupId].GetTracks()[trackId];
            byte[] buffer = compressor.Compress(track.Map.GetBytes(), false);
            int compressedTrackSize = buffer.Length;

            Assert.AreEqual(expectedSize, compressedTrackSize);
        }

        [Test]
        public void TestTrackSmk1()
        {
            this.CheckTrackCompression(this.smkGame, 0, 0, 1705);
        }

        [Test]
        public void TestTrackSmk2()
        {
            this.CheckTrackCompression(this.smkGame, 0, 1, 4415);
        }

        [Test]
        public void TestTrackSmk3()
        {
            this.CheckTrackCompression(this.smkGame, 0, 2, 1097);
        }

        [Test]
        public void TestTrackSmk4()
        {
            this.CheckTrackCompression(this.smkGame, 0, 3, 2565);
        }

        [Test]
        public void TestTrackSmk5()
        {
            this.CheckTrackCompression(this.smkGame, 0, 4, 2167);
        }

        [Test]
        public void TestTrackSmk6()
        {
            this.CheckTrackCompression(this.smkGame, 1, 0, 2787);
        }

        [Test]
        public void TestTrackSmk7()
        {
            this.CheckTrackCompression(this.smkGame, 1, 1, 1260);
        }

        [Test]
        public void TestTrackSmk8()
        {
            this.CheckTrackCompression(this.smkGame, 1, 2, 4612);
        }

        [Test]
        public void TestTrackSmk9()
        {
            this.CheckTrackCompression(this.smkGame, 1, 3, 3009);
        }

        [Test]
        public void TestTrackSmk10()
        {
            this.CheckTrackCompression(this.smkGame, 1, 4, 2787);
        }

        [Test]
        public void TestTrackSmk11()
        {
            this.CheckTrackCompression(this.smkGame, 2, 0, 3359);
        }

        [Test]
        public void TestTrackSmk12()
        {
            this.CheckTrackCompression(this.smkGame, 2, 1, 3602);
        }

        [Test]
        public void TestTrackSmk13()
        {
            this.CheckTrackCompression(this.smkGame, 2, 2, 3066);
        }

        [Test]
        public void TestTrackSmk14()
        {
            this.CheckTrackCompression(this.smkGame, 2, 3, 3294);
        }

        [Test]
        public void TestTrackSmk15()
        {
            this.CheckTrackCompression(this.smkGame, 2, 4, 2657);
        }

        [Test]
        public void TestTrackSmk16()
        {
            this.CheckTrackCompression(this.smkGame, 3, 0, 4567);
        }

        [Test]
        public void TestTrackSmk17()
        {
            this.CheckTrackCompression(this.smkGame, 3, 1, 4162);
        }

        [Test]
        public void TestTrackSmk18()
        {
            this.CheckTrackCompression(this.smkGame, 3, 2, 1389);
        }

        [Test]
        public void TestTrackSmk19()
        {
            this.CheckTrackCompression(this.smkGame, 3, 3, 3231);
        }

        [Test]
        public void TestTrackSmk20()
        {
            this.CheckTrackCompression(this.smkGame, 3, 4, 1094);
        }

        [Test]
        public void TestTrackSmk21()
        {
            this.CheckTrackCompression(this.smkGame, 4, 0, 659);
        }

        [Test]
        public void TestTrackSmk22()
        {
            this.CheckTrackCompression(this.smkGame, 4, 1, 829);
        }

        [Test]
        public void TestTrackSmk23()
        {
            this.CheckTrackCompression(this.smkGame, 4, 2, 662);
        }

        [Test]
        public void TestTrackSmk24()
        {
            this.CheckTrackCompression(this.smkGame, 4, 3, 1104);
        }

        [Test]
        public void TestTrackEE1()
        {
            this.CheckTrackCompression(this.eeGame, 0, 0, 3169);
        }

        [Test]
        public void TestTrackEE2()
        {
            this.CheckTrackCompression(this.eeGame, 0, 1, 1571);
        }

        [Test]
        public void TestTrackEE3()
        {
            this.CheckTrackCompression(this.eeGame, 0, 2, 3558);
        }

        [Test]
        public void TestTrackEE4()
        {
            this.CheckTrackCompression(this.eeGame, 0, 3, 1353);
        }

        [Test]
        public void TestTrackEE5()
        {
            this.CheckTrackCompression(this.eeGame, 0, 4, 3911);
        }

        [Test]
        public void TestTrackEE6()
        {
            this.CheckTrackCompression(this.eeGame, 1, 0, 4026);
        }

        [Test]
        public void TestTrackEE7()
        {
            this.CheckTrackCompression(this.eeGame, 1, 1, 3098);
        }

        [Test]
        public void TestTrackEE8()
        {
            this.CheckTrackCompression(this.eeGame, 1, 2, 2277);
        }

        [Test]
        public void TestTrackEE9()
        {
            this.CheckTrackCompression(this.eeGame, 1, 3, 1726);
        }

        [Test]
        public void TestTrackEE10()
        {
            this.CheckTrackCompression(this.eeGame, 1, 4, 1826);
        }

        [Test]
        public void TestTrackEE11()
        {
            this.CheckTrackCompression(this.eeGame, 2, 0, 2022);
        }

        [Test]
        public void TestTrackEE12()
        {
            this.CheckTrackCompression(this.eeGame, 2, 1, 1821);
        }

        [Test]
        public void TestTrackEE13()
        {
            this.CheckTrackCompression(this.eeGame, 2, 2, 1487);
        }

        [Test]
        public void TestTrackEE14()
        {
            this.CheckTrackCompression(this.eeGame, 2, 3, 3176);
        }

        [Test]
        public void TestTrackEE15()
        {
            this.CheckTrackCompression(this.eeGame, 2, 4, 1621);
        }

        [Test]
        public void TestTrackEE16()
        {
            this.CheckTrackCompression(this.eeGame, 3, 0, 946);
        }

        [Test]
        public void TestTrackEE17()
        {
            this.CheckTrackCompression(this.eeGame, 3, 1, 3957);
        }

        [Test]
        public void TestTrackEE18()
        {
            this.CheckTrackCompression(this.eeGame, 3, 2, 564);
        }

        [Test]
        public void TestTrackEE19()
        {
            this.CheckTrackCompression(this.eeGame, 3, 3, 752);
        }

        [Test]
        public void TestTrackEE20()
        {
            this.CheckTrackCompression(this.eeGame, 3, 4, 2642);
        }

        [Test]
        public void TestTrackEE21()
        {
            this.CheckTrackCompression(this.eeGame, 4, 0, 1002);
        }

        [Test]
        public void TestTrackEE22()
        {
            this.CheckTrackCompression(this.eeGame, 4, 1, 907);
        }

        [Test]
        public void TestTrackEE23()
        {
            this.CheckTrackCompression(this.eeGame, 4, 2, 1086);
        }

        [Test]
        public void TestTrackEE24()
        {
            this.CheckTrackCompression(this.eeGame, 4, 3, 315);
        }
    }
}
