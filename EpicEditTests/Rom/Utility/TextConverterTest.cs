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
using EpicEdit.Rom.Utility;
using NUnit.Framework;

namespace EpicEditTests.Rom.Utility
{
    [TestFixture]
    internal class TextConverterTest
    {
        private void TestName(byte[] data, string text, Region region)
        {
            this.TestName(data, text, region, null);
        }

        private void TestName(byte[] data, string text, Region region, byte? paletteIndex)
        {
            TextConverter.Instance.LoadCharacterSet(region);
            string actualText = TextConverter.Instance.DecodeText(data, paletteIndex != null);
            byte[] actualData = TextConverter.Instance.EncodeText(text, paletteIndex);
            Assert.AreEqual(text, actualText);
            Assert.AreEqual(data, actualData);
        }

        [Test]
        public void TestUSBattleCourse()
        {
            // NOTE: The original game uses 0x2C for spaces here (and some other times other characters),
            // but we just generate 0x2F every time (simpler, same result).
            byte[] data = new byte[]
            {
                0x0B, 0x0A, 0x1D, 0x1D, 0x15, 0x0E, 0x2F, 0x0C,
                0x18, 0x1E, 0x1B, 0x1C, 0x0E, 0x2F
            };

            this.TestName(data, "Battle Course ", Region.US);
        }

        [Test]
        public void TestUSBattleMode()
        {
            // NOTE: The original game leaves the palette index as 0 for spaces (0x2F),
            // but we don't (simpler, same result).
            byte[] data = new byte[]
            {
                0x0B, 0x0C, 0x0A, 0x0C, 0x1D, 0x0C, 0x1D, 0x0C,
                0x15, 0x0C, 0x0E, 0x0C, 0x2F, 0x0C, 0x16, 0x0C,
                0x18, 0x0C, 0x0D, 0x0C, 0x0E, 0x0C
            };

            this.TestName(data, "Battle Mode", Region.US, data[1]);
        }

        [Test]
        public void TestUSBowserCastle()
        {
            byte[] data = new byte[]
            {
                0x0B, 0x18, 0x20, 0x1C, 0x0E, 0x1B, 0x2F, 0x0C,
                0x0A, 0x1C, 0x1D, 0x15, 0x0E, 0x2F
            };

            this.TestName(data, "Bowser Castle ", Region.US);
        }

        [Test]
        public void TestUSChocoIsland()
        {
            byte[] data = new byte[]
            {
                0x0C, 0x11, 0x18, 0x0C, 0x18, 0x2F, 0x12, 0x1C,
                0x15, 0x0A, 0x17, 0x0D, 0x2F
            };

            this.TestName(data, "Choco Island ", Region.US);
        }

        [Test]
        public void TestUSDonutPlains()
        {
            byte[] data = new byte[]
            {
                0x0D, 0x18, 0x17, 0x1E, 0x1D, 0x2F, 0x19, 0x15,
                0x0A, 0x12, 0x17, 0x1C, 0x2F
            };

            this.TestName(data, "Donut Plains ", Region.US);
        }

        [Test]
        public void TestUSFlowerCup()
        {
            byte[] data = new byte[]
            {
                0x0F, 0x15, 0x18, 0x20, 0x0E, 0x1B, 0x2F, 0x0C,
                0x1E, 0x19
            };

            this.TestName(data, "Flower Cup", Region.US);
        }

        [Test]
        public void TestUSGhostValley()
        {
            byte[] data = new byte[]
            {
                0x10, 0x11, 0x18, 0x1C, 0x1D, 0x2F, 0x1F, 0x0A,
                0x15, 0x15, 0x0E, 0x22, 0x2F
            };

            this.TestName(data, "Ghost Valley ", Region.US);
        }

        [Test]
        public void TestUSKoopaBeach()
        {
            byte[] data = new byte[]
            {
                0x14, 0x18, 0x18, 0x19, 0x0A, 0x2F, 0x0B, 0x0E,
                0x0A, 0x0C, 0x11, 0x2F
            };

            this.TestName(data, "Koopa Beach ", Region.US);
        }

        [Test]
        public void TestUSMarioCircuit()
        {
            byte[] data = new byte[]
            {
                0x16, 0x0A, 0x1B, 0x12, 0x18, 0x2F, 0x0C, 0x12,
                0x1B, 0x0C, 0x1E, 0x12, 0x1D, 0x2F
            };

            this.TestName(data, "Mario Circuit ", Region.US);
        }

        [Test]
        public void TestUSMariokartGp()
        {
            byte[] data = new byte[]
            {
                0x16, 0x08, 0x0A, 0x08, 0x1B, 0x08, 0x12, 0x08,
                0x18, 0x08, 0x14, 0x08, 0x0A, 0x08, 0x1B, 0x08,
                0x1D, 0x08, 0x2F, 0x08, 0x10, 0x08, 0x19, 0x08
            };

            this.TestName(data, "Mariokart Gp", Region.US, data[1]);
        }

        [Test]
        public void TestUSMatchRace()
        {
            byte[] data = new byte[]
            {
                0x16, 0x0A, 0x0A, 0x0A, 0x1D, 0x0A, 0x0C, 0x0A,
                0x11, 0x0A, 0x2F, 0x0A, 0x1B, 0x0A, 0x0A, 0x0A,
                0x0C, 0x0A, 0x0E, 0x0A
            };

            this.TestName(data, "Match Race", Region.US, data[1]);
        }

        [Test]
        public void TestUSMushroomCup()
        {
            byte[] data = new byte[]
            {
                0x16, 0x1E, 0x1C, 0x11, 0x1B, 0x18, 0x18, 0x16,
                0x2F, 0x0C, 0x1E, 0x19
            };

            this.TestName(data, "Mushroom Cup", Region.US);
        }

        [Test]
        public void TestUSRainbowRoad()
        {
            byte[] data = new byte[]
            {
                0x1B, 0x0A, 0x12, 0x17, 0x0B, 0x18, 0x20, 0x2F,
                0x1B, 0x18, 0x0A, 0x0D, 0x2F
            };

            this.TestName(data, "Rainbow Road ", Region.US);
        }

        [Test]
        public void TestUSSpecialCup()
        {
            byte[] data = new byte[]
            {
                0x1C, 0x19, 0x0E, 0x0C, 0x12, 0x0A, 0x15, 0x2F,
                0x0C, 0x1E, 0x19
            };

            this.TestName(data, "Special Cup", Region.US);
        }

        [Test]
        public void TestUSStarCup()
        {
            byte[] data = new byte[]
            {
                0x1C, 0x1D, 0x0A, 0x1B, 0x2F, 0x0C, 0x1E, 0x19
            };

            this.TestName(data, "Star Cup", Region.US);
        }

        [Test]
        public void TestUSVanillaLake()
        {
            byte[] data = new byte[]
            {
                0x1F, 0x0A, 0x17, 0x12, 0x15, 0x15, 0x0A, 0x2F,
                0x15, 0x0A, 0x14, 0x0E, 0x2F
            };

            this.TestName(data, "Vanilla Lake ", Region.US);
        }

        [Test]
        public void TestJapBattleCourse()
        {
            byte[] data = new byte[]
            {
                0x79, 0x2E, 0x73, 0x88, 0x69, 0x9C, 0x6C, 0x2C
            };

            this.TestName(data, "バトルコース ", Region.Jap);
        }

        [Test]
        public void TestJapBattleMode()
        {
            byte[] data = new byte[]
            {
                0x79, 0x0C, 0x2E, 0x0C, 0x73, 0x0C, 0x88, 0x0C,
                0x68, 0x0C, 0x2E, 0x0C, 0x9C, 0x0C, 0x80, 0x0C
            };

            this.TestName(data, "バトルゲーム", Region.Jap, data[1]);
        }

        [Test]
        public void TestJapBowserCastle()
        {
            byte[] data = new byte[]
            {
                0x67, 0x97, 0x79, 0x2F, 0x3B, 0x2E, 0x94, 0x32,
                0x69, 0x9C, 0x6C, 0x2C
            };

            this.TestName(data, "クッパじょうコース ", Region.Jap);
        }

        [Test]
        public void TestJapChocoIsland()
        {
            byte[] data = new byte[]
            {
                0x70, 0x9A, 0x69, 0x89, 0x9C, 0x43, 0x32, 0x69,
                0x9C, 0x6C, 0x2C
            };

            this.TestName(data, "チョコレーとうコース ", Region.Jap);
        }

        [Test]
        public void TestJapDonutPlains()
        {
            byte[] data = new byte[]
            {
                0x73, 0x2E, 0x9C, 0x74, 0x71, 0x4C, 0x31, 0x53,
                0x69, 0x9C, 0x6C, 0x2C
            };

            this.TestName(data, "ドーナツへいやコース ", Region.Jap);
        }

        [Test]
        public void TestJapFlowerCup()
        {
            byte[] data = new byte[]
            {
                0x7B, 0x86, 0x8B, 0x9C, 0x65, 0x97, 0x7B, 0x2F,
                0x2C
            };

            this.TestName(data, "フラワーカップ ", Region.Jap);
        }

        [Test]
        public void TestJapGhostValley()
        {
            byte[] data = new byte[]
            {
                0x34, 0x49, 0x2E, 0x38, 0x46, 0x4E, 0x69, 0x9C,
                0x6C, 0x2C
            };

            this.TestName(data, "おばけぬまコース ", Region.Jap);
        }

        [Test]
        public void TestJapKoopaBeach()
        {
            byte[] data = new byte[]
            {
                0x78, 0x69, 0x78, 0x69, 0x7A, 0x2E, 0x9C, 0x70,
                0x69, 0x9C, 0x6C, 0x2C
            };

            this.TestName(data, "ノコノコビーチコース ", Region.Jap);
        }

        [Test]
        public void TestJapMarioCircuit()
        {
            byte[] data = new byte[]
            {
                0x7E, 0x87, 0x64, 0x6A, 0x9C, 0x66, 0x97, 0x73,
                0x2C
            };

            this.TestName(data, "マリオサーキット ", Region.Jap);
        }

        [Test]
        public void TestJapMariokartGp()
        {
            byte[] data = new byte[]
            {
                0x7E, 0x08, 0x87, 0x08, 0x64, 0x08, 0x65, 0x08,
                0x9C, 0x08, 0x73, 0x08, 0x20, 0x08, 0x21, 0x08
            };

            this.TestName(data, "マリオカートGP", Region.Jap, data[1]);
        }

        [Test]
        public void TestJapMatchRace()
        {
            byte[] data = new byte[]
            {
                0x22, 0x0A, 0x23, 0x0A, 0x7E, 0x0A, 0x97, 0x0A,
                0x70, 0x0A, 0x89, 0x0A, 0x9C, 0x0A, 0x6C, 0x0A
            };

            this.TestName(data, "VSマッチレース", Region.Jap, data[1]);
        }

        [Test]
        public void TestJapMushroomCup()
        {
            byte[] data = new byte[]
            {
                0x66, 0x78, 0x69, 0x65, 0x97, 0x7B, 0x2F, 0x2C
            };

            this.TestName(data, "キノコカップ ", Region.Jap);
        }

        [Test]
        public void TestJapRainbowRoad()
        {
            byte[] data = new byte[]
            {
                0x89, 0x61, 0x8D, 0x7D, 0x2E, 0x9C, 0x8A, 0x9C,
                0x73, 0x2E
            };

            this.TestName(data, "レインボーロード", Region.Jap);
        }

        [Test]
        public void TestJapSpecialCup()
        {
            byte[] data = new byte[]
            {
                0x6C, 0x7C, 0x2F, 0x6B, 0x98, 0x88, 0x65, 0x97,
                0x7B, 0x2F, 0x2C
            };

            this.TestName(data, "スペシャルカップ ", Region.Jap);
        }

        [Test]
        public void TestJapStarCup()
        {
            byte[] data = new byte[]
            {
                0x6C, 0x6F, 0x9C, 0x65, 0x97, 0x7B, 0x2F, 0x2C
            };

            this.TestName(data, "スターカップ ", Region.Jap);
        }

        [Test]
        public void TestJapVanillaLake()
        {
            byte[] data = new byte[]
            {
                0x79, 0x2E, 0x75, 0x86, 0x89, 0x61, 0x67, 0x69,
                0x9C, 0x6C, 0x2C
            };

            this.TestName(data, "バニラレイクコース ", Region.Jap);
        }
    }
}
