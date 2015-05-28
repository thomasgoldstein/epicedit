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
using NUnit.Framework;

namespace EpicEdit.Test
{
    internal abstract class TestBase
    {
        private Exception testFixtureSetupException;

        [TestFixtureSetUp]
        protected void SetUpTestFixture()
        {
            try
            {
                this.Init();
            }
            catch (Exception ex)
            {
                this.testFixtureSetupException = ex;
            }
        }

        public abstract void Init();

        [SetUp]
        public void LogTestFixtureExceptions()
        {
            // Workaround for NUnit limitation if there are any exceptions during TestFixtureSetUp,
            // as it only logs "TestFixtureSetUp failed in ClassName" otherwise.
            if (this.testFixtureSetupException != null)
            {
                Assert.Fail(this.testFixtureSetupException.Message);
            }
        }
    }
}
