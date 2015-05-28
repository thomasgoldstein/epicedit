﻿#region GPL statement
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
using EpicEdit.Rom.Tracks.Overlay;
using NUnit.Framework;

namespace EpicEdit.Test.Rom.Tracks.Overlay
{
    [TestFixture]
    internal class OverlayTilesTest : TestBase
    {
        private Smk smk;

        public override void Init()
        {
            this.smk = new Smk();
        }

        private void TestGetBytes(byte[] data)
        {
            OverlayTiles overlayTiles = new OverlayTiles(data, this.smk.OverlayTileSizes, this.smk.OverlayTilePatterns);
            byte[] dataAfter = overlayTiles.GetBytes();

            Assert.AreEqual(data, dataAfter);
        }

        [Test]
        public void TestGetBytes1()
        {
            byte[] data =
            {
                0x00, 0x76, 0x0C, 0x00, 0x79, 0x0B, 0x00, 0xF5,
                0x0A, 0x00, 0x75, 0x0E, 0x00, 0x78, 0x0D, 0x00,
                0x7B, 0x0C, 0x00, 0xFB, 0x0E, 0x00, 0xF8, 0x0F,
                0x00, 0xF6, 0x10, 0x00, 0x7A, 0x10, 0xF0, 0xCB,
                0x07, 0xEC, 0x41, 0x09, 0x54, 0x85, 0x1D, 0x01,
                0x85, 0x1E, 0x54, 0x8D, 0x1D, 0x01, 0x8E, 0x1E,
                0xE0, 0x58, 0x3A, 0xE0, 0xDE, 0x3A, 0xE0, 0xE4,
                0x39, 0xE4, 0x75, 0x19, 0x74, 0x60, 0x2D, 0xEC,
                0x62, 0x2C, 0xE0, 0x88, 0x19, 0x74, 0x8E, 0x1A,
                0x00, 0x74, 0x0D, 0x74, 0x86, 0x1A, 0x00, 0xF7,
                0x09, 0xE4, 0x78, 0x14, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
            };

            this.TestGetBytes(data);
        }

        [Test]
        public void TestGetBytes2()
        {
            byte[] data =
            {
                0x02, 0xD6, 0x04, 0x02, 0xCB, 0x05, 0x02, 0x52,
                0x04, 0x02, 0x62, 0x08, 0x02, 0xD8, 0x06, 0x02,
                0x5A, 0x05, 0x02, 0x58, 0x03, 0x02, 0x5C, 0x04,
                0x02, 0xC5, 0x08, 0x02, 0x4F, 0x07, 0x74, 0x61,
                0x13, 0xF0, 0x07, 0x28, 0xF0, 0x0C, 0x2E, 0xE0,
                0x24, 0x35, 0xE0, 0xA2, 0x38, 0xE0, 0x29, 0x34,
                0xE0, 0x1C, 0x3A, 0x02, 0x5D, 0x07, 0x02, 0xBF,
                0x07, 0xDC, 0x90, 0x1E, 0xEC, 0x1C, 0x15, 0xEC,
                0x20, 0x15, 0x02, 0xDF, 0x05, 0x02, 0xE0, 0x09,
                0x02, 0xE4, 0x0A, 0xE0, 0x65, 0x18, 0x74, 0xE1,
                0x16, 0xE0, 0xEC, 0x1D, 0x74, 0x64, 0x15, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
            };

            this.TestGetBytes(data);
        }

        [Test]
        public void TestGetBytes3()
        {
            byte[] data =
            {
                0x03, 0x1A, 0x06, 0x03, 0x97, 0x04, 0x03, 0xE3,
                0x0B, 0x03, 0xE7, 0x0D, 0xEC, 0x71, 0x1B, 0xEC,
                0x77, 0x1B, 0xEC, 0xF0, 0x1D, 0xEC, 0xF6, 0x1D,
                0xEC, 0xF8, 0x1D, 0xF0, 0xDD, 0x28, 0xF0, 0xDD,
                0x29, 0xEC, 0x71, 0x20, 0xEC, 0x77, 0x20, 0xF0,
                0x9E, 0x34, 0xF0, 0x9E, 0x35, 0x54, 0x0D, 0x2A,
                0x08, 0x8E, 0x2F, 0x00, 0x8E, 0x28, 0x03, 0x9E,
                0x05, 0x03, 0x9D, 0x08, 0x03, 0xA0, 0x07, 0x03,
                0xA4, 0x05, 0x03, 0x95, 0x06, 0x03, 0x1C, 0x07,
                0xE0, 0x8A, 0x07, 0x03, 0xA3, 0x08, 0x74, 0x47,
                0x39, 0x74, 0x49, 0x3A, 0x74, 0x4B, 0x3B, 0x74,
                0x4D, 0x3C, 0xF0, 0x3E, 0x14, 0xF0, 0x3E, 0x15,
                0x03, 0x98, 0x07, 0x03, 0x21, 0x06, 0x03, 0x9C,
                0x04, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
            };

            this.TestGetBytes(data);
        }
    }
}
