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

using EpicEdit.Rom.Tracks.Objects;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Tracks.Objects
{
    [TestFixture]
    internal class TrackObjectTests
    {
        [Test]
        public void TestGetBytes1()
        {
            byte[] dataBefore = { 0xA1, 0x05 };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes2()
        {
            byte[] dataBefore = { 0x14, 0x08 };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes3()
        {
            byte[] dataBefore = { 0x17, 0x03 };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes4()
        {
            byte[] dataBefore = { 0x9F, 0x07 };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes5()
        {
            byte[] dataBefore = { 0xBF, 0x27 };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes6()
        {
            byte[] dataBefore = { 0x12, 0x2A };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes7()
        {
            byte[] dataBefore = { 0x20, 0x2E };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes8()
        {
            byte[] dataBefore = { 0x9E, 0x2E };
            byte[] dataAfter = new byte[2];

            TrackObject trackObject = new TrackObject(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes9()
        {
            byte[] dataBefore = { 0xEF, 0x1E };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes10()
        {
            // NOTE: This tests vertical object direction.
            byte[] dataBefore = { 0xC4, 0x44 };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes11()
        {
            byte[] dataBefore = { 0x76, 0x1F };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes12()
        {
            // NOTE: This tests Match Race Banana objects.
            byte[] dataBefore = { 0x12, 0xAA };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes13()
        {
            // NOTE: This tests Match Race Banana objects.
            byte[] dataBefore = { 0x16, 0xAA };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes14()
        {
            // NOTE: This tests Match Race Banana objects.
            byte[] dataBefore = { 0x14, 0xAA };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }

        [Test]
        public void TestGetBytes15()
        {
            // NOTE: This tests Match Race Banana objects.
            byte[] dataBefore = { 0x18, 0xAA };
            byte[] dataAfter = new byte[2];

            TrackObjectMatchRace trackObject = new TrackObjectMatchRace(dataBefore, 0);
            trackObject.GetBytes(dataAfter, 0);

            Assert.AreEqual(dataBefore, dataAfter);
        }
    }
}
