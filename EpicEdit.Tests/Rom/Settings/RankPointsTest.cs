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

using EpicEdit.Rom.Settings;
using NUnit.Framework;

namespace EpicEdit.Tests.Rom.Settings
{
    [TestFixture]
    internal class RankPointsTest
    {
        [Test]
        public void TestGetBytes()
        {
            byte[] dataBefore =
            {
                0x09, 0x00, 0x06, 0x00, 0x03, 0x00, 0x01, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            };

            RankPoints rankPoints = new RankPoints(dataBefore);

            byte[] dataAfter = rankPoints.GetBytes();

            Assert.AreEqual(dataBefore, dataAfter);
        }
    }
}
