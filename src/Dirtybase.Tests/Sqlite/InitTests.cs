using System;
using System.IO;
using Dirtybase.App;
using Dirtybase.App.Implementations.Sqlite;
using NUnit.Framework;
using System.Data.SQLite;
using SharpTestsEx;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    [Category(TestTypes.EndToEnd)]
    public class InitTests
    {
        private const string databaseFile = "Dirtybase.db";
        private const string connectionstring = "Data Source = " + databaseFile + ";Version=3;";
        private const string arguments = "init -db sqlite -cs " + connectionstring;
        private const string versionTableName = "DirtybaseVersions";

        [SetUp]
        public void SetUp()
        {
            MakeSqliteDatabase();
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect(); 
            GC.WaitForPendingFinalizers();
            DeleteSqliteDatabase();
        }

        [Test]
        public void InitOnSqliteShouldAddDirtyBaseVerionTable()
        {
            Program.Main(arguments.Split(' '));
            this.AssertDirtybaseTableExists();
        }

        [Test]
        public void InitOnExistingDirtyBaseSqliteDoNothing()
        {
            AddVersionTable();
            Program.Main(arguments.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            TearDown();
            Program.Main(arguments.Split(' '));
        }

        private void MakeSqliteDatabase()
        {
            DeleteSqliteDatabase();
            SQLiteConnection.CreateFile(databaseFile);
        }

        private static void DeleteSqliteDatabase()
        {
            if(File.Exists(databaseFile))
            {
                File.Delete(databaseFile);
            }
        }

        private void AddVersionTable()
        {
            using (var connection = new SQLiteConnection(connectionstring))
            {
                connection.Open();
                var command = connection.CreateCommand();
                try
                {
                    command.CommandText = "CREATE TABLE DirtybaseVersion(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)";
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
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