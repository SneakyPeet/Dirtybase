using Dirtybase.App;
using Dirtybase.App.Exceptions;
using Dirtybase.App.Implementations.Sqlite;
using Dirtybase.App.Options.Validators;
using NUnit.Framework;
using System.Data.SQLite;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    [Category(TestTypes.EndToEnd)]
    public class InitTests : SqliteTestBase
    {
        private const string arguments = "init -db sqlite -cs " + connectionstring;

        [Test]
        public void InitOnSqliteShouldAddDirtyBaseVersionTable()
        {
            Program.Main(arguments.Split(' '));
            AssertAgainstDatabase(DirtybaseVersionTableExists);
        }

        [Test]
        public void InitOnExistingDirtyBaseSqliteDoNothing()
        {
            this.CreateVersionTable();
            Program.Main(arguments.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            TearDown();
            Program.Main(arguments.Split(' '));
        }

        private Errors DirtybaseVersionTableExists(SQLiteConnection connection)
        {
            const string query = "SELECT name FROM sqlite_master WHERE name ='" + versionTableName + "';";
            var command = new SQLiteCommand(query, connection);
            using (var reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    return new Errors();
                }
                return new Errors{"Dirtybase Version Table Does Not Exist"};
            }
        }
    }
}