using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options.Validators;
using NUnit.Framework;
using System.Data.SQLite;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class InitTests : SqliteTestBase
    {
        private const string arguments = "init -db sqlite -cs " + connectionstring;
        
        [Test]
        public void InitOnSqliteShouldAddDirtyBaseVersionTable()
        {
            api.Do(arguments.Split(' '));
            AssertAgainstDatabase(DirtybaseVersionTableExists);
        }

        [Test]
        public void InitOnExistingDirtyBaseSqliteDoNothing()
        {
            this.CreateVersionTable();
            api.Do(arguments.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            TearDown();
            api.Do(arguments.Split(' '));
        }

        private Errors DirtybaseVersionTableExists(SQLiteConnection connection)
        {
            const string query = "SELECT name FROM sqlite_master WHERE name ='" + versionTableName + "';";
            using(var command = new SQLiteCommand(query, connection)) 
            {
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
}