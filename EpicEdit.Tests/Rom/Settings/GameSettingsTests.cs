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
using EpicEdit.Rom.Settings;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Settings
{
    [TestFixture]
    internal class GameSettingsTests
    {
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
            Game game = File.GetGame(Region.US);
            this.TestTexts(
                new[]
                {
                    "MARIOKART GP", "MATCH RACE", "BATTLE MODE"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                game.Settings.ModeNames);
        }

        [Test]
        public void TestUSGPCupSelectTexts()
        {
            Game game = File.GetGame(Region.US);
            byte[] romBuffer = File.ReadRom(Region.US);
            this.TestTexts(
                new[]
                {
                    "MUSHROOM CUP RACE", "FLOWER CUP RACE", "STAR CUP RACE", "SPECIAL CUP RACE"
                },
                File.ReadBlock(romBuffer, 0x4F85F, 8),
                File.ReadBlock(romBuffer, 0x4F867, 130),
                game.Settings.GPCupSelectTexts);
        }

        [Test]
        public void TestUSGPPodiumTexts()
        {
            Game game = File.GetGame(Region.US);
            byte[] romBuffer = File.ReadRom(Region.US);
            this.TestTexts(
                new[]
                {
                    "MUSHROOM", "FLOWER", "STAR", "SPECIAL", " CUP RACE\n"
                },
                File.ReadBlock(romBuffer, 0x5A0EE, 10),
                File.ReadBlock(romBuffer, 0x5A0F8, 80),
                game.Settings.GPPodiumCupTexts);
        }

        [Test]
        public void TestUSCourseSelectTexts()
        {
            Game game = File.GetGame(Region.US);
            byte[] romBuffer = File.ReadRom(Region.US);
            this.TestTexts(
                new[]
                {
                    "MUSHROOM CUP", "FLOWER CUP", "STAR CUP", "SPECIAL CUP",
                    "MARIO CIRCUIT ", "GHOST VALLEY ", "DONUT PLAINS ", "BOWSER CASTLE ",
                    "VANILLA LAKE ", "CHOCO ISLAND ", "KOOPA BEACH ", "BATTLE COURSE ", "RAINBOW ROAD "
                },
                File.ReadBlock(romBuffer, 0x1CA32, 26),
                File.ReadBlock(romBuffer, 0x1CA88, 173),
                game.Settings.CourseSelectTexts);
        }

        [Test]
        public void TestUSDriverNamesGPResults()
        {
            Game game = File.GetGame(Region.US);
            byte[] romBuffer = File.ReadRom(Region.US);
            this.TestTexts(
                new[]
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "DONKEY KONG JR", "KOOPA TROOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(romBuffer, 0x5C25B, 16),
                File.ReadBlock(romBuffer, 0x5C277, 134),
                game.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestUSDriverNamesGPPodium()
        {
            Game game = File.GetGame(Region.US);
            byte[] romBuffer = File.ReadRom(Region.US);
            this.TestTexts(
                new[]
                {
                    "MARIO", "LUIGI", "BOWSER", "THE PRINCESS",
                    "DK JR.", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(romBuffer, 0x5A148, 16),
                File.ReadBlock(romBuffer, 0x5A15C, 112),
                game.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestUSDriverNamesTimeTrial()
        {
            Game game = File.GetGame(Region.US);
            byte[] romBuffer = File.ReadRom(Region.US);
            this.TestTexts(
                new[]
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "D.K.JR", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(romBuffer, 0x1DDD3, 16),
                File.ReadBlock(romBuffer, 0x1DC91, 52),
                game.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestEuroModeNames()
        {
            Game game = File.GetGame(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MARIOKART GP", "MATCH RACE", "BATTLE MODE"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                game.Settings.ModeNames);
        }

        [Test]
        public void TestEuroGPCupSelectTexts()
        {
            Game game = File.GetGame(Region.Euro);
            byte[] romBuffer = File.ReadRom(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MUSHROOM CUP RACE", "FLOWER CUP RACE", "STAR CUP RACE", "SPECIAL CUP RACE"
                },
                File.ReadBlock(romBuffer, 0x4F778, 8),
                File.ReadBlock(romBuffer, 0x4F780, 130),
                game.Settings.GPCupSelectTexts);
        }

        [Test]
        public void TestEuroGPPodiumTexts()
        {
            Game game = File.GetGame(Region.Euro);
            byte[] romBuffer = File.ReadRom(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MUSHROOM", "FLOWER", "STAR", "SPECIAL", " CUP RACE\n"
                },
                File.ReadBlock(romBuffer, 0x5A0F8, 10),
                File.ReadBlock(romBuffer, 0x5A102, 80),
                game.Settings.GPPodiumCupTexts);
        }

        [Test]
        public void TestEuroCourseSelectTexts()
        {
            Game game = File.GetGame(Region.Euro);
            byte[] romBuffer = File.ReadRom(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MUSHROOM CUP", "FLOWER CUP", "STAR CUP", "SPECIAL CUP",
                    "MARIO CIRCUIT ", "GHOST VALLEY ", "DONUT PLAINS ", "BOWSER CASTLE ",
                    "VANILLA LAKE ", "CHOCO ISLAND ", "KOOPA BEACH ", "BATTLE COURSE ", "RAINBOW ROAD "
                },
                File.ReadBlock(romBuffer, 0x1C8CE, 26),
                File.ReadBlock(romBuffer, 0x1C924, 173),
                game.Settings.CourseSelectTexts);
        }

        [Test]
        public void TestEuroDriverNamesGPResults()
        {
            Game game = File.GetGame(Region.Euro);
            byte[] romBuffer = File.ReadRom(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "DONKEY KONG JR", "KOOPA TROOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(romBuffer, 0x5C263, 16),
                File.ReadBlock(romBuffer, 0x5C27F, 134),
                game.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestEuroDriverNamesGPPodium()
        {
            Game game = File.GetGame(Region.Euro);
            byte[] romBuffer = File.ReadRom(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MARIO", "LUIGI", "BOWSER", "THE PRINCESS",
                    "DK JR.", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(romBuffer, 0x5A152, 16),
                File.ReadBlock(romBuffer, 0x5A166, 112),
                game.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestEuroDriverNamesTimeTrial()
        {
            Game game = File.GetGame(Region.Euro);
            byte[] romBuffer = File.ReadRom(Region.Euro);
            this.TestTexts(
                new[]
                {
                    "MARIO", "LUIGI", "BOWSER", "PRINCESS",
                    "D.K.JR", "KOOPA", "TOAD", "YOSHI"
                },
                File.ReadBlock(romBuffer, 0x1DC81, 16),
                File.ReadBlock(romBuffer, 0x1DB3F, 52),
                game.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestJapModeNames()
        {
            Game game = File.GetGame(Region.Jap);
            this.TestTexts(
                new[]
                {
                    "マリオカートGP", "VSマッチレース", "バトルゲーム"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                game.Settings.ModeNames);
        }

        [Test]
        public void TestJapGPPodiumTexts()
        {
            Game game = File.GetGame(Region.Jap);
            byte[] romBuffer = File.ReadRom(Region.Jap);
            this.TestTexts(
                new[]
                {
                    " キノコ", " フラワー", " スター", " スペシャル", "カップレースー\n"
                },
                File.ReadBlock(romBuffer, 0x5A092, 10),
                File.ReadBlock(romBuffer, 0x5A09C, 68),
                game.Settings.GPPodiumCupTexts);
        }

        [Test]
        public void TestJapCourseSelectTexts()
        {
            Game game = File.GetGame(Region.Jap);
            byte[] romBuffer = File.ReadRom(Region.Jap);
            this.TestTexts(
                new[]
                {
                    "キノコカップ ", "フラワーカップ ", "スターカップ ", "スペシャルカップ ",
                    "マリオサーキット ", "おばけぬまコース ", "ドーナツへいやコース ", "クッパじょうコース ",
                    "バニラレイクコース ", "チョコレーとうコース ", "ノコノコビーチコース ", "バトルコース ", "レインボーロード"
                },
                File.ReadBlock(romBuffer, 0x1C9A3, 26),
                File.ReadBlock(romBuffer, 0x1CA19, 144),
                game.Settings.CourseSelectTexts);
        }

        [Test]
        public void TestJapDriverNamesGPResults()
        {
            Game game = File.GetGame(Region.Jap);
            byte[] romBuffer = File.ReadRom(Region.Jap);
            this.TestTexts(
                new[]
                {
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキーコングJR", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(romBuffer, 0x5C1EC, 32),
                File.ReadBlock(romBuffer, 0x5C216, 136),
                game.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestJapDriverNamesGPPodium()
        {
            Game game = File.GetGame(Region.Jap);
            byte[] romBuffer = File.ReadRom(Region.Jap);
            this.TestTexts(
                new[]
                {
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキーコングJR", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(romBuffer, 0x5A0E0, 16),
                File.ReadBlock(romBuffer, 0x5A0F0, 96),
                game.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestJapDriverNamesTimeTrial()
        {
            Game game = File.GetGame(Region.Jap);
            byte[] romBuffer = File.ReadRom(Region.Jap);
            this.TestTexts(
                new[]
                {
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキー", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(romBuffer, 0x1DDCA, 16),
                File.ReadBlock(romBuffer, 0x1DC75, 42),
                game.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestUnorderedIndexes()
        {
            // Testing TextCollection saving when text indexes were originally not in the right order
            // (can happen if the ROM has been edited manually).
            
            byte[] romBuffer1 = File.ReadRom(Region.US);
            byte[] romBuffer2 = romBuffer1.Clone() as byte[];

            // Switch the indexes for the first 2 names
            romBuffer2[0x1CA32] = romBuffer1[0x1CA34];
            romBuffer2[0x1CA33] = romBuffer1[0x1CA35];
            romBuffer2[0x1CA34] = romBuffer1[0x1CA32];
            romBuffer2[0x1CA35] = romBuffer1[0x1CA33];

            GameSettings settings = new GameSettings(romBuffer2, new Offsets(romBuffer2, Region.US), Region.US);

            // Switch the texts for the first 2 names
            string name1 = settings.CourseSelectTexts[0].Value;
            string name2 = settings.CourseSelectTexts[1].Value;

            // NOTE: Set the second name first because it's shorter, and won't cause us to hit the maximum character count
            settings.CourseSelectTexts[1].Value = name1;
            settings.CourseSelectTexts[0].Value = name2;

            // Resaving the data is supposed to sort the texts and the indexes
            settings.CourseSelectTexts.Save(romBuffer2);

            Assert.AreEqual(romBuffer1, romBuffer2);
        }

        [Test]
        public void TestSpaceBytes()
        {
            byte[] romBuffer = File.ReadRom(Region.US);

            romBuffer[0x1DC91 + 1] = 0x2C; // Replace Time Trial Mario's A with a thin space
            romBuffer[0x1DC91 + 3] = 0x2F; // Replace Time Trial Mario's I with a normal space
            GameSettings settings = new GameSettings(romBuffer, new Offsets(romBuffer, Region.US), Region.US);

            Assert.AreEqual("M R O", settings.DriverNamesTimeTrial[0].Value);
        }
    }
}
