using Dirtybase.App;
using Dirtybase.App.Implementations.Sqlite;
using NUnit.Framework;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    [Category(TestTypes.EndToEnd)]
    public class MigrateTests : SqliteTestBase
    {
        private const string scriptFolder = "testfolder";
        private const string arguments = "migrate -db sqlite -cs " + connectionstring + " -sf " + scriptFolder;

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            TearDown();
            Program.Main(arguments.Split(' '));
        }
    }
}