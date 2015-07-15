using Dirtybase.Core.Exceptions;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlImplementations.Sqlite
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class InitTests : SqliteTestBase
    {
        private string Arguments { get { return "init -db sqlite -cs " + this.ConnectionString; } }
        
        [Test]
        public void InitShouldAddDirtyBaseVersionTable()
        {
            this.api.Do(this.Arguments.Split(' '));
            this.AssertAgainstDatabase(this.DirtybaseVersionTableExists);
        }

        [Test]
        public void InitOnExistingDirtyBaseSqliteDoNothing()
        {
            this.CreateVersionTable();
            this.api.Do(this.Arguments.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            this.TearDown();
            this.api.Do(this.Arguments.Split(' '));
        }
    }
}