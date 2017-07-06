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
using EpicEdit.Rom.Settings;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Settings
{
    [TestFixture]
    internal class GameSettingsTest : TestBase
    {
        private Game gameJ;
        private Game gameU;
        private Game gameE;
        private byte[] romBufferJ;
        private byte[] romBufferU;
        private byte[] romBufferE;

        public override void Init()
        {
            this.gameJ = File.GetGame(Region.Jap);
            this.gameU = File.GetGame(Region.US);
            this.gameE = File.GetGame(Region.Euro);
            this.romBufferJ = File.ReadRom(Region.Jap);
            this.romBufferU = File.ReadRom(Region.US);
            this.romBufferE = File.ReadRom(Region.Euro);
        }

        private void TestTexts(string[] expectedTexts, byte[] expectedIndexes, byte[] expectedBytes, TextCollection textColl)
        {
            Assert.AreEqual(expectedTexts.Length, textColl.Count);

            for (int i = 0; i < expectedTexts.Length; i++)
            {
                Assert.AreEqual(expectedTexts[i], textColl[i].Value);
            }

            if (expectedBytes != null)
            {
                byte[] textBytes = textColl.GetBytes(out byte[] indexes);
                Assert.AreEqual(expectedIndexes, indexes);
                Assert.AreEqual(expectedBytes, textBytes);

                // The original data is filled with text with no extra room,
                // so the total character count should be equal to the max.
                Assert.AreEqual(textColl.MaxCharacterCount, textColl.TotalCharacterCount);
            }
        }

        [Test]
        public void TestUSModeNames()
        {
            this.TestTexts(
                new []
                {
                    "MARIOKART GP", "MATCH RACE", "BATTLE MODE"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                this.gameU.Settings.ModeNames);
        }

        [Test]
        public void TestUSGPCupSelectTexts()
        {
            this.TestTexts(
                new []
                {
                    "MUSHROOM CUP RACE", "FLOWER CUP RACE", "STAR CUP RACE", "SPECIAL CUP RACE"
                },
                File.ReadBlock(this.romBufferU, 0x4F85F, 8),
                File.ReadBlock(this.romBufferU, 0x4F867, 130),
                this.gameU.Settings.GPCupSelectTexts);
        }

        [Test]
        public void TestUSGPPodiumTexts()
        {
            this.TestTexts(
                new []
                {
                    "MUSHROOM", "FLOWER", "STAR", "SPECIAL", " CUP RACE\n"
                },
                File.ReadBlock(this.romBufferU, 0x5A0EE, 10),
                File.ReadBlock(this.romBufferU, 0x5A0F8, 80),
                this.gameU.Settings.GPPodiumCupTexts);
        }

        [Test]
        public void TestUSCupAndThemeTexts()
        {
            this.TestTexts(
                new []
                {
                    "MUSHROOM CUP", "FLOWER CUP", "STAR CUP", "SPECIAL CUP",
                    "MARIO CIRCUIT ", "GHOST VALLEY ", "DONUT PLAINS ", "BOWSER CASTLE ",
                    "VANILLA LAKE ", "CHOCO ISLAND ", "KOOPA BEACH ", "BATTLE COURSE ", "RAINBOW ROAD "
                },
                File.ReadBlock(this.romBufferU, 0x1CA32, 26),
                File.ReadBlock(this.romBufferU, 0x1CA88, 173),
                this.gameU.Settings.CupAndThemeTexts);
        }

        [Test]
        public void TestUSDriverNamesGPResults()
        {
            this.TestTexts(
                new []
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "DONKEY KONG JR", "KOOPA TROOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(this.romBufferU, 0x5C25B, 16),
                File.ReadBlock(this.romBufferU, 0x5C277, 134),
                this.gameU.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestUSDriverNamesGPPodium()
        {
            this.TestTexts(
                new []
                {
                    "MARIO", "LUIGI", "BOWSER", "THE PRINCESS",
                    "DK JR.", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(this.romBufferU, 0x5A148, 16),
                File.ReadBlock(this.romBufferU, 0x5A15C, 112),
                this.gameU.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestUSDriverNamesTimeTrial()
        {
            this.TestTexts(
                new []
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "D.K.JR", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(this.romBufferU, 0x1DDD3, 16),
                File.ReadBlock(this.romBufferU, 0x1DC91, 52),
                this.gameU.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestEuroModeNames()
        {
            this.TestTexts(
                new []
                {
                    "MARIOKART GP", "MATCH RACE", "BATTLE MODE"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                this.gameE.Settings.ModeNames);
        }

        [Test]
        public void TestEuroGPCupSelectTexts()
        {
            this.TestTexts(
                new []
                {
                    "MUSHROOM CUP RACE", "FLOWER CUP RACE", "STAR CUP RACE", "SPECIAL CUP RACE"
                },
                File.ReadBlock(this.romBufferE, 0x4F778, 8),
                File.ReadBlock(this.romBufferE, 0x4F780, 130),
                this.gameE.Settings.GPCupSelectTexts);
        }

        [Test]
        public void TestEuroGPPodiumTexts()
        {
            this.TestTexts(
                new []
                {
                    "MUSHROOM", "FLOWER", "STAR", "SPECIAL", " CUP RACE\n"
                },
                File.ReadBlock(this.romBufferE, 0x5A0F8, 10),
                File.ReadBlock(this.romBufferE, 0x5A102, 80),
                this.gameE.Settings.GPPodiumCupTexts);
        }

        [Test]
        public void TestEuroCupAndThemeTexts()
        {
            this.TestTexts(
                new []
                {
                    "MUSHROOM CUP", "FLOWER CUP", "STAR CUP", "SPECIAL CUP",
                    "MARIO CIRCUIT ", "GHOST VALLEY ", "DONUT PLAINS ", "BOWSER CASTLE ",
                    "VANILLA LAKE ", "CHOCO ISLAND ", "KOOPA BEACH ", "BATTLE COURSE ", "RAINBOW ROAD "
                },
                File.ReadBlock(this.romBufferE, 0x1C8CE, 26),
                File.ReadBlock(this.romBufferE, 0x1C924, 173),
                this.gameE.Settings.CupAndThemeTexts);
        }

        [Test]
        public void TestEuroDriverNamesGPResults()
        {
            this.TestTexts(
                new []
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "DONKEY KONG JR", "KOOPA TROOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(this.romBufferE, 0x5C263, 16),
                File.ReadBlock(this.romBufferE, 0x5C27F, 134),
                this.gameE.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestEuroDriverNamesGPPodium()
        {
            this.TestTexts(
                new []
                {
                    "MARIO", "LUIGI", "BOWSER", "THE PRINCESS",
                    "DK JR.", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(this.romBufferE, 0x5A152, 16),
                File.ReadBlock(this.romBufferE, 0x5A166, 112),
                this.gameE.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestEuroDriverNamesTimeTrial()
        {
            this.TestTexts(
                new []
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "D.K.JR", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(this.romBufferE, 0x1DC81, 16),
                File.ReadBlock(this.romBufferE, 0x1DB3F, 52),
                this.gameE.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestJapModeNames()
        {
            this.TestTexts(
                new []
                {
                    "マリオカートGP", "VSマッチレース", "バトルゲーム"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                this.gameJ.Settings.ModeNames);
        }

        [Test]
        public void TestJapGPPodiumTexts()
        {
            this.TestTexts(
                new []
                {
                    " キノコ", " フラワー", " スター", " スペシャル", "カップレースー\n"
                },
                File.ReadBlock(this.romBufferJ, 0x5A092, 10),
                File.ReadBlock(this.romBufferJ, 0x5A09C, 68),
                this.gameJ.Settings.GPPodiumCupTexts);
        }

        [Test]
        public void TestJapCupAndThemeTexts()
        {
            this.TestTexts(
                new []
                {
                    "キノコカップ ", "フラワーカップ ", "スターカップ ", "スペシャルカップ ",
                    "マリオサーキット ", "おばけぬまコース ", "ドーナツへいやコース ", "クッパじょうコース ",
                    "バニラレイクコース ", "チョコレーとうコース ", "ノコノコビーチコース ", "バトルコース ", "レインボーロード"
                },
                File.ReadBlock(this.romBufferJ, 0x1C9A3, 26),
                File.ReadBlock(this.romBufferJ, 0x1CA19, 144),
                this.gameJ.Settings.CupAndThemeTexts);
        }

        [Test]
        public void TestJapDriverNamesGPResults()
        {
            this.TestTexts(
                new []
                {
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキーコングJR", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(this.romBufferJ, 0x5C1EC, 32),
                File.ReadBlock(this.romBufferJ, 0x5C216, 136),
                this.gameJ.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestJapDriverNamesGPPodium()
        {
            this.TestTexts(
                new []
                {
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキーコングJR", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(this.romBufferJ, 0x5A0E0, 16),
                File.ReadBlock(this.romBufferJ, 0x5A0F0, 96),
                this.gameJ.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestJapDriverNamesTimeTrial()
        {
            this.TestTexts(
                new []
                {
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキー", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(this.romBufferJ, 0x1DDCA, 16),
                File.ReadBlock(this.romBufferJ, 0x1DC75, 42),
                this.gameJ.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestUnorderedIndexes()
        {
            // Testing TextCollection saving when text indexes were originally not in the right order
            // (can happen if the ROM has been edited manually).

            // Working on a clone to avoid changing the original buffer
            byte[] romBuffer = this.romBufferU.Clone() as byte[];

            // Switch the indexes for the first 2 names
            romBuffer[0x1CA32] = this.romBufferU[0x1CA34];
            romBuffer[0x1CA33] = this.romBufferU[0x1CA35];
            romBuffer[0x1CA34] = this.romBufferU[0x1CA32];
            romBuffer[0x1CA35] = this.romBufferU[0x1CA33];

            GameSettings settings = new GameSettings(romBuffer, new Offsets(romBuffer, Region.US), Region.US);

            // Switch the texts for the first 2 names
            string name1 = settings.CupAndThemeTexts[0].Value;
            string name2 = settings.CupAndThemeTexts[1].Value;

            // NOTE: Set the second name first because it's shorter, and won't cause us to hit the maximum character count
            settings.CupAndThemeTexts[1].Value = name1;
            settings.CupAndThemeTexts[0].Value = name2;

            // Resaving the data is supposed to sort the texts and the indexes
            settings.CupAndThemeTexts.Save(romBuffer);

            Assert.AreEqual(this.romBufferU, romBuffer);
        }

        [Test]
        public void TestSpaceBytes()
        {
            // Working on a clone to avoid changing the original buffer
            byte[] romBuffer = this.romBufferU.Clone() as byte[];

            romBuffer[0x1DC91 + 1] = 0x2C; // Replace Time Trial Mario's A with a thin space
            romBuffer[0x1DC91 + 3] = 0x2F; // Replace Time Trial Mario's I with a normal space
            GameSettings settings = new GameSettings(romBuffer, new Offsets(romBuffer, Region.US), Region.US);

            Assert.AreEqual("M R O", settings.DriverNamesTimeTrial[0].Value);
        }
    }
}
