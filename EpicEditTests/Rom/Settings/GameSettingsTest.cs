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

namespace EpicEditTests.Rom.Settings
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

        private void TestTexts(string[] expectedTexts, byte[] expectedBytes, TextCollection textColl)
        {
            Assert.AreEqual(expectedTexts.Length, textColl.Count);

            for (int i = 0; i < expectedTexts.Length; i++)
            {
                Assert.AreEqual(expectedTexts[i], textColl[i]);
            }

            if (expectedBytes != null)
            {
                Assert.AreEqual(expectedBytes, textColl.GetBytes());
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
                this.gameJ.Settings.ModeNames);
        }

        [Test]
        public void TestJapDriverNamesGPResults()
        {
            // TODO: Make this test pass.
            // This test fails because we do not load the ten-ten and maru characters.
            // Unlike for other Japanese texts where these characters are located right after
            // the character they should be connected to, here, they're defined in a separate place,
            // away from the main text.
            // e.g: the value for Luigi's name (4 characters) is:
            //     88 20 61 20 9C 20 6B 20 FF FF,
            // followed by the ten-ten data:
            //     10 21 10 21 10 21 2E 20 FF FF,
            // which specifies nothing should be added to the first 3 characters, but a ten-ten (2E)
            // should be added to the 4th.
            this.TestTexts(
                new string[]
                {
                    // Texts without ten-ten and maru:
                    //"マリオ", "ルイーシ", "クッハ", "ヒーチ",
                    //"トンキーコンクJR", "ノコノコ", "キノヒオ", "ヨッシー"
                    "マリオ", "ルイージ", "クッパ", "ピーチ",
                    "ドンキーコングJR", "ノコノコ", "キノピオ", "ヨッシー"
                },
                File.ReadBlock(this.romBufferJ, 0x5C216, 134),
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
                File.ReadBlock(this.romBufferJ, 0x1DC75, 42),
                this.gameJ.Settings.DriverNamesTimeTrial);
        }
    }
}
