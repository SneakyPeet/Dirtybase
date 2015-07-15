using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options.Validators;
using NUnit.Framework;
using System.Data.SQLite;

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

        private Errors DirtybaseVersionTableExists(SQLiteConnection connection)
        {
            string query = "SELECT name FROM sqlite_master WHERE name ='" + VersionTableName + "';";
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