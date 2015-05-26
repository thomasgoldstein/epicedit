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
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Compression
{
    [TestFixture]
    internal class DecompressionTest
    {
        private readonly byte[] romBuffer;

        public DecompressionTest()
        {
            this.romBuffer = File.ReadRom(Region.US);
        }

        [Test]
        public void InvalidCompressedDataTest()
        {
            // Checks that we don't crash if the compressed data is incorrect.
            // Here, we have a command 5 that references an address that is out of the buffer range.
            byte[] bufferA = File.ReadFile("randomtest.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("randomtestc.smc"), 0);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void EpicRacerTest() // Checks that function 3 loops properly after 0xFF
        {
            byte[] bufferA = File.ReadFile("epicr_track24.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("epicr.smc"), 0x8E5A2);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_1()
        {
            byte[] bufferA = File.ReadFile("track1c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x68D2C);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_2()
        {
            byte[] bufferA = File.ReadFile("track2c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x7A181);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_3()
        {
            byte[] bufferA = File.ReadFile("track3c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6F5BF);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_4()
        {
            byte[] bufferA = File.ReadFile("track4c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x7B825);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_5()
        {
            byte[] bufferA = File.ReadFile("track5c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x50C45);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_6()
        {
            byte[] bufferA = File.ReadFile("track6c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x77251);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_7()
        {
            byte[] bufferA = File.ReadFile("track7c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x7E821);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_8()
        {
            byte[] bufferA = File.ReadFile("track8c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6A823);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_9()
        {
            byte[] bufferA = File.ReadFile("track9c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x7C527);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_10()
        {
            byte[] bufferA = File.ReadFile("track10c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x68000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_11()
        {
            byte[] bufferA = File.ReadFile("track11c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6E1AE);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_12()
        {
            byte[] bufferA = File.ReadFile("track12c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6CFC4);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_13()
        {
            byte[] bufferA = File.ReadFile("track13c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x3103C);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_14()
        {
            byte[] bufferA = File.ReadFile("track14c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x7D5FF);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_15()
        {
            byte[] bufferA = File.ReadFile("track15c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6952B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_16()
        {
            byte[] bufferA = File.ReadFile("track16c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x78A6B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_17()
        {
            byte[] bufferA = File.ReadFile("track17c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x3D551);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_18()
        {
            byte[] bufferA = File.ReadFile("track18c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6A245);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_19()
        {
            byte[] bufferA = File.ReadFile("track19c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6BF68);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_20()
        {
            byte[] bufferA = File.ReadFile("track20c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x3EAD1);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_21()
        {
            byte[] bufferA = File.ReadFile("track21c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x6F338);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_22()
        {
            byte[] bufferA = File.ReadFile("track22c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x21765);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_23()
        {
            byte[] bufferA = File.ReadFile("track23c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x214EE);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack1st_24()
        {
            byte[] bufferA = File.ReadFile("track24c.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x7ED3B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetGhostValley()
        {
            byte[] bufferA = File.ReadFile("rtilesetgv.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x60189);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetMarioCircuit()
        {
            byte[] bufferA = File.ReadFile("rtilesetmc.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x481C9);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetDonutPlains()
        {
            byte[] bufferA = File.ReadFile("rtilesetdp.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x50000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetChocoIsland()
        {
            byte[] bufferA = File.ReadFile("rtilesetci.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x78000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetVanillaLake()
        {
            byte[] bufferA = File.ReadFile("rtilesetvl.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x14EE);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetKoopaBeach()
        {
            byte[] bufferA = File.ReadFile("rtilesetkb.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x51636);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetBowserCastle()
        {
            byte[] bufferA = File.ReadFile("rtilesetbc.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x48F6A);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestRoadTilesetRainbowRoad()
        {
            byte[] bufferA = File.ReadFile("rtilesetrr.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x41EBB);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetGhostValley()
        {
            byte[] bufferA = File.ReadFile("btilesetgv.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x20148);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetMarioCircuit()
        {
            byte[] bufferA = File.ReadFile("btilesetmc.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x20000);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetDonutPlains()
        {
            byte[] bufferA = File.ReadFile("btilesetdp.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x20356);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetChocoIsland()
        {
            byte[] bufferA = File.ReadFile("btilesetci.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x206B5);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetVanillaLake()
        {
            byte[] bufferA = File.ReadFile("btilesetvl.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x205EB);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetKoopaBeach()
        {
            byte[] bufferA = File.ReadFile("btilesetkb.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x208A2);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetBrowserCastle()
        {
            byte[] bufferA = File.ReadFile("btilesetbc.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x20523);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestBackgroundTilesetRainbowRoad()
        {
            byte[] bufferA = File.ReadFile("btilesetrr.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x1499);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteMarioCircuit()
        {
            byte[] bufferA = File.ReadFile("cpalettemc.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x4117F);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteGhostValley()
        {
            byte[] bufferA = File.ReadFile("cpalettegv.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x41313);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteDonutPlains()
        {
            byte[] bufferA = File.ReadFile("cpalettedp.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x414C4);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteBowserCastle()
        {
            byte[] bufferA = File.ReadFile("cpalettebc.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x41675);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteVanillaLake()
        {
            byte[] bufferA = File.ReadFile("cpalettevl.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x4182F);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteChocoIsland()
        {
            byte[] bufferA = File.ReadFile("cpaletteci.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x419C0);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteKoopaBeach()
        {
            byte[] bufferA = File.ReadFile("cpalettekb.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x41B5B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestColorPaletteRainbowRoad()
        {
            byte[] bufferA = File.ReadFile("cpaletterr.smc");
            byte[] bufferB = Codec.Decompress(this.romBuffer, 0x41D0B);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_1()
        {
            byte[] bufferA = File.ReadFile("track1.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track1c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_2()
        {
            byte[] bufferA = File.ReadFile("track2.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track2c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_3()
        {
            byte[] bufferA = File.ReadFile("track3.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track3c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_4()
        {
            byte[] bufferA = File.ReadFile("track4.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track4c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_5()
        {
            byte[] bufferA = File.ReadFile("track5.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track5c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_6()
        {
            byte[] bufferA = File.ReadFile("track6.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track6c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_7()
        {
            byte[] bufferA = File.ReadFile("track7.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track7c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_8()
        {
            byte[] bufferA = File.ReadFile("track8.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track8c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_9()
        {
            byte[] bufferA = File.ReadFile("track9.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track9c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_10()
        {
            byte[] bufferA = File.ReadFile("track10.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track10c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_11()
        {
            byte[] bufferA = File.ReadFile("track11.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track11c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_12()
        {
            byte[] bufferA = File.ReadFile("track12.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track12c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_13()
        {
            byte[] bufferA = File.ReadFile("track13.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track13c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_14()
        {
            byte[] bufferA = File.ReadFile("track14.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track14c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_15()
        {
            byte[] bufferA = File.ReadFile("track15.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track15c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_16()
        {
            byte[] bufferA = File.ReadFile("track16.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track16c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_17()
        {
            byte[] bufferA = File.ReadFile("track17.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track17c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_18()
        {
            byte[] bufferA = File.ReadFile("track18.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track18c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_19()
        {
            byte[] bufferA = File.ReadFile("track19.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track19c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_20()
        {
            byte[] bufferA = File.ReadFile("track20.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track20c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_21()
        {
            byte[] bufferA = File.ReadFile("track21.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track21c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_22()
        {
            byte[] bufferA = File.ReadFile("track22.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track22c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_23()
        {
            byte[] bufferA = File.ReadFile("track23.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track23c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }

        [Test]
        public void TestTrack2nd_24()
        {
            byte[] bufferA = File.ReadFile("track24.smc");
            byte[] bufferB = Codec.Decompress(File.ReadFile("track24c.smc"), 0, 16384);

            Assert.AreEqual(bufferA, bufferB);
        }
    }
}
