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
using EpicEdit.Rom.Utility;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Settings
{
    [TestFixture]
    internal class GameSettingsTest
    {
        private Game game;
        private Game gameJ;
        private byte[] romBuffer;
        private byte[] romBufferJ;

        public GameSettingsTest()
        {
            this.game = File.GetGame(Region.US);
            this.gameJ = File.GetGame(Region.Jap);
            this.romBuffer = File.ReadRom(Region.US);
            this.romBufferJ = File.ReadRom(Region.Jap);
        }

        private void TestTexts(string[] expectedTexts, byte[] expectedIndexes, byte[] expectedBytes, TextCollection textColl)
        {
            Assert.AreEqual(expectedTexts.Length, textColl.Count);

            for (int i = 0; i < expectedTexts.Length; i++)
            {
                Assert.AreEqual(expectedTexts[i], textColl[i]);
            }

            if (expectedBytes != null)
            {
                byte[] indexes;
                byte[] textBytes = textColl.GetBytes(out indexes);
                Assert.AreEqual(expectedIndexes, indexes);
                Assert.AreEqual(expectedBytes, textBytes);
            }
        }

        [Test]
        public void TestUSCupAndThemeNames()
        {
            this.TestTexts(
                new string[]
                {
                    "Mushroom Cup", "Flower Cup", "Star Cup", "Special Cup",
                    "Mario Circuit ", "Ghost Valley ", "Donut Plains ", "Bowser Castle ",
                    "Vanilla Lake ", "Choco Island ", "Koopa Beach ", "Battle Course ", "Rainbow Road "
                },
                File.ReadBlock(this.romBuffer, 0x1CA32, 26),
                File.ReadBlock(this.romBuffer, 0x1CA88, 173),
                this.game.Settings.CupAndThemeNames);
        }

        [Test]
        public void TestUSModeNames()
        {
            this.TestTexts(
                new string[]
                {
                    "Mariokart Gp", "Match Race", "Battle Mode"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                this.game.Settings.ModeNames);
        }

        [Test]
        public void TestUSDriverNamesGPResults()
        {
            this.TestTexts(
                new string[]
                {
                    "Mario", "Luigi", "Bowser", "Princess",
                    "Donkey Kong Jr", "Koopa Troopa", "Toad", "Yoshi"
                },
                File.ReadBlock(this.romBuffer, 0x5C25B, 16),
                File.ReadBlock(this.romBuffer, 0x5C277, 134),
                this.game.Settings.DriverNamesGPResults);
        }

        [Test]
        public void TestUSDriverNamesGPPodium()
        {
            this.TestTexts(
                new string[]
                {
                    "Mario", "Luigi", "Bowser", "The Princess",
                    "Dk Jr.", "Koopa", "Toad", "Yoshi"
                },
                File.ReadBlock(this.romBuffer, 0x5A148, 16),
                File.ReadBlock(this.romBuffer, 0x5A15C, 112),
                this.game.Settings.DriverNamesGPPodium);
        }

        [Test]
        public void TestUSDriverNamesTimeTrial()
        {
            this.TestTexts(
                new string[]
                {
                    "Mario", "Luigi", "Bowser", "Princess",
                    "D.K.Jr", "Koopa", "Toad", "Yoshi"
                },
                File.ReadBlock(this.romBuffer, 0x1DDD3, 16),
                File.ReadBlock(this.romBuffer, 0x1DC91, 52),
                this.game.Settings.DriverNamesTimeTrial);
        }

        [Test]
        public void TestJapCupAndThemeNames()
        {
            this.TestTexts(
                new string[]
                {
                    "キノコカップ ", "フラワーカップ ", "スターカップ ", "スペシャルカップ ",
                    "マリオサーキット ", "おばけぬまコース ", "ドーナツへいやコース ", "クッパじょうコース ",
                    "バニラレイクコース ", "チョコレーとうコース ", "ノコノコビーチコース ", "バトルコース ", "レインボーロード"
                },
                File.ReadBlock(this.romBufferJ, 0x1C9A3, 26),
                File.ReadBlock(this.romBufferJ, 0x1CA19, 144),
                this.gameJ.Settings.CupAndThemeNames);
        }

        [Test]
        public void TestJapModeNames()
        {
            this.TestTexts(
                new string[]
                {
                    "マリオカートGP", "VSマッチレース", "バトルゲーム"
                },
                null, // Not testing expected bytes, data resaving not supported
                null, // Not testing expected bytes, data resaving not supported
                this.gameJ.Settings.ModeNames);
        }

        [Test]
        public void TestJapDriverNamesGPResults()
        {
            this.TestTexts(
                new string[]
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
                new string[]
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
                new string[]
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
            byte[] romBuffer = this.romBuffer.Clone() as byte[];

            // Switch the indexes for the first 2 names
            romBuffer[0x1CA32] = this.romBuffer[0x1CA34];
            romBuffer[0x1CA33] = this.romBuffer[0x1CA35];
            romBuffer[0x1CA34] = this.romBuffer[0x1CA32];
            romBuffer[0x1CA35] = this.romBuffer[0x1CA33];

            GameSettings settings = new GameSettings(romBuffer, new Offsets(romBuffer, Region.US), Region.US);

            // Switch the texts for the first 2 names
            string name1 = settings.CupAndThemeNames[0];
            string name2 = settings.CupAndThemeNames[1];
            settings.CupAndThemeNames[0] = name2;
            settings.CupAndThemeNames[1] = name1;

            // Resaving the data is supposed to sort the texts and the indexes
            settings.CupAndThemeNames.Save(romBuffer);

            Assert.AreEqual(this.romBuffer, romBuffer);
        }
    }
}
