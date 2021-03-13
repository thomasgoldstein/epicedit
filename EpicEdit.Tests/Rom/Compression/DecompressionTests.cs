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
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Compression
{
    [TestFixture]
    internal class DecompressionTests
    {
        private byte[] _romBuffer;

        [SetUp]
        public void SetUp()
        {
            _romBuffer = File.ReadRom(Region.US);
        }

        [Test]
        public void InvalidCompressedDataTest()
        {
            // Checks that we don't crash if the compressed data is incorrect.
            // Here, we have a command 5 that references an address that is out of the buffer range.
            var bufferA = File.ReadFile("cmd5test.smc");
            var bufferB = Codec.Decompress(File.ReadFile("cmd5testc.smc"), 0);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void EpicRacersBattleTrack4Test()
        {
            // Checks that command 3 loops properly after 0xFF
            var bufferA = File.ReadFile("er_track24c.smc");
            var bufferB = Codec.Decompress(File.ReadFile("er_track24cc.smc"));

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_1()
        {
            var bufferA = File.ReadFile("track1c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x68D2C);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_2()
        {
            var bufferA = File.ReadFile("track2c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x7A181);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_3()
        {
            var bufferA = File.ReadFile("track3c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6F5BF);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_4()
        {
            var bufferA = File.ReadFile("track4c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x7B825);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_5()
        {
            var bufferA = File.ReadFile("track5c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x50C45);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_6()
        {
            var bufferA = File.ReadFile("track6c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x77251);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_7()
        {
            var bufferA = File.ReadFile("track7c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x7E821);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_8()
        {
            var bufferA = File.ReadFile("track8c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6A823);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_9()
        {
            var bufferA = File.ReadFile("track9c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x7C527);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_10()
        {
            var bufferA = File.ReadFile("track10c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x68000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_11()
        {
            var bufferA = File.ReadFile("track11c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6E1AE);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_12()
        {
            var bufferA = File.ReadFile("track12c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6CFC4);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_13()
        {
            var bufferA = File.ReadFile("track13c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x3103C);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_14()
        {
            var bufferA = File.ReadFile("track14c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x7D5FF);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_15()
        {
            var bufferA = File.ReadFile("track15c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6952B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_16()
        {
            var bufferA = File.ReadFile("track16c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x78A6B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_17()
        {
            var bufferA = File.ReadFile("track17c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x3D551);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_18()
        {
            var bufferA = File.ReadFile("track18c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6A245);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_19()
        {
            var bufferA = File.ReadFile("track19c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6BF68);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_20()
        {
            var bufferA = File.ReadFile("track20c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x3EAD1);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_21()
        {
            var bufferA = File.ReadFile("track21c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x6F338);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_22()
        {
            var bufferA = File.ReadFile("track22c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x21765);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_23()
        {
            var bufferA = File.ReadFile("track23c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x214EE);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_24()
        {
            var bufferA = File.ReadFile("track24c.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x7ED3B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetGhostValley()
        {
            var bufferA = File.ReadFile("rtilesetgv.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x60189);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetMarioCircuit()
        {
            var bufferA = File.ReadFile("rtilesetmc.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x481C9);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetDonutPlains()
        {
            var bufferA = File.ReadFile("rtilesetdp.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x50000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetChocoIsland()
        {
            var bufferA = File.ReadFile("rtilesetci.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x78000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetVanillaLake()
        {
            var bufferA = File.ReadFile("rtilesetvl.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x14EE);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetKoopaBeach()
        {
            var bufferA = File.ReadFile("rtilesetkb.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x51636);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetBowserCastle()
        {
            var bufferA = File.ReadFile("rtilesetbc.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x48F6A);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetRainbowRoad()
        {
            var bufferA = File.ReadFile("rtilesetrr.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x41EBB);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetGhostValley()
        {
            var bufferA = File.ReadFile("btilesetgv.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x20148);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetMarioCircuit()
        {
            var bufferA = File.ReadFile("btilesetmc.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x20000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetDonutPlains()
        {
            var bufferA = File.ReadFile("btilesetdp.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x20356);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetChocoIsland()
        {
            var bufferA = File.ReadFile("btilesetci.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x206B5);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetVanillaLake()
        {
            var bufferA = File.ReadFile("btilesetvl.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x205EB);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetKoopaBeach()
        {
            var bufferA = File.ReadFile("btilesetkb.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x208A2);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetBrowserCastle()
        {
            var bufferA = File.ReadFile("btilesetbc.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x20523);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetRainbowRoad()
        {
            var bufferA = File.ReadFile("btilesetrr.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x1499);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteMarioCircuit()
        {
            var bufferA = File.ReadFile("cpalettemc.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x4117F);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteGhostValley()
        {
            var bufferA = File.ReadFile("cpalettegv.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x41313);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteDonutPlains()
        {
            var bufferA = File.ReadFile("cpalettedp.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x414C4);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteBowserCastle()
        {
            var bufferA = File.ReadFile("cpalettebc.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x41675);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteVanillaLake()
        {
            var bufferA = File.ReadFile("cpalettevl.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x4182F);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteChocoIsland()
        {
            var bufferA = File.ReadFile("cpaletteci.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x419C0);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteKoopaBeach()
        {
            var bufferA = File.ReadFile("cpalettekb.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x41B5B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteRainbowRoad()
        {
            var bufferA = File.ReadFile("cpaletterr.smc");
            var bufferB = Codec.Decompress(_romBuffer, 0x41D0B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_1()
        {
            var bufferA = File.ReadFile("track1.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track1c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_2()
        {
            var bufferA = File.ReadFile("track2.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track2c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_3()
        {
            var bufferA = File.ReadFile("track3.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track3c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_4()
        {
            var bufferA = File.ReadFile("track4.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track4c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_5()
        {
            var bufferA = File.ReadFile("track5.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track5c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_6()
        {
            var bufferA = File.ReadFile("track6.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track6c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_7()
        {
            var bufferA = File.ReadFile("track7.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track7c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_8()
        {
            var bufferA = File.ReadFile("track8.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track8c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_9()
        {
            var bufferA = File.ReadFile("track9.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track9c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_10()
        {
            var bufferA = File.ReadFile("track10.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track10c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_11()
        {
            var bufferA = File.ReadFile("track11.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track11c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_12()
        {
            var bufferA = File.ReadFile("track12.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track12c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_13()
        {
            var bufferA = File.ReadFile("track13.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track13c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_14()
        {
            var bufferA = File.ReadFile("track14.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track14c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_15()
        {
            var bufferA = File.ReadFile("track15.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track15c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_16()
        {
            var bufferA = File.ReadFile("track16.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track16c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_17()
        {
            var bufferA = File.ReadFile("track17.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track17c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_18()
        {
            var bufferA = File.ReadFile("track18.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track18c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_19()
        {
            var bufferA = File.ReadFile("track19.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track19c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_20()
        {
            var bufferA = File.ReadFile("track20.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track20c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_21()
        {
            var bufferA = File.ReadFile("track21.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track21c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_22()
        {
            var bufferA = File.ReadFile("track22.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track22c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_23()
        {
            var bufferA = File.ReadFile("track23.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track23c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_24()
        {
            var bufferA = File.ReadFile("track24.smc");
            var bufferB = Codec.Decompress(File.ReadFile("track24c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }
    }
}
