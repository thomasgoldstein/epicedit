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
using NUnit.Framework;

namespace EpicEditTests.Rom
{
	[TestFixture]
	public class OffsetTest
	{
		[Test]
		public void TestOffset000000()
		{
			byte[] buffer = { 0x00, 0x00, 0x00 };
			Offset ofs = new Offset(buffer);

			Assert.AreEqual(ofs, 0x000000);
		}

		[Test]
		public void TestOffset0Fffff()
		{
			byte[] buffer = { 0xFF, 0xFF, 0xFF };
			Offset ofs = new Offset(buffer);

			Assert.AreEqual(ofs, 0x0FFFFF);
		}

		[Test]
		public void TestOffset063412()
		{
			byte[] buffer = { 0x12, 0x34, 0x56 };
			Offset ofs = new Offset(buffer);

			Assert.AreEqual(ofs, 0x063412);
		}

		[Test]
		public void TestOffset080400()
		{
			byte[] buffer = { 0x00, 0x04, 0xC8 };
			Offset ofs = new Offset(buffer);

			Assert.AreEqual(ofs, 0x080400);
		}

		[Test]
		public void TestOffset0FffffEncoded()
		{
			Offset ofs = new Offset(0x0FFFFF);
			byte[] buffer = { 0xFF, 0xFF, 0xCF };

			Assert.AreEqual(buffer, ofs.GetBytes());
		}

		[Test]
		public void TestOffset063412Encoded()
		{
			Offset ofs = new Offset(0x063412);
			byte[] buffer = { 0x12, 0x34, 0xC6 };

			Assert.AreEqual(buffer, ofs.GetBytes());
		}

		[Test]
		public void TestOffset080400Encoded()
		{
			Offset ofs = new Offset(0x080400);
			byte[] buffer = { 0x00, 0x04, 0xC8 };

			Assert.AreEqual(buffer, ofs.GetBytes());
		}
	}
}
