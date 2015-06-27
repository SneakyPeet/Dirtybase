using System;
using Dirtybase.App;
using Dirtybase.App.Implementations.Sqlite;
using NUnit.Framework;
using System.Data.SQLite;
using SharpTestsEx;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    [Category(TestTypes.EndToEnd)]
    public class InitTests : SqliteTestBase
    {
        private const string arguments = "init -db sqlite -cs " + connectionstring;

        [Test]
        public void InitOnSqliteShouldAddDirtyBaseVerionTable()
        {
            Program.Main(arguments.Split(' '));
            this.AssertDirtybaseTableExists();
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

        private void AssertDirtybaseTableExists()
        {
            bool hasTable;
            using(var connection = new SQLiteConnection(connectionstring))
            {
                connection.Open();
                const string query = "SELECT name FROM sqlite_master WHERE name ='"+ versionTableName + "';";
                var command = new SQLiteCommand(query, connection);
                try
                {
                    using (var reader = command.ExecuteReader())
                    {
                        hasTable = reader.HasRows;
                    }
                }
                catch(Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
            hasTable.Should().Be.True();
        }
    }
}