using System;
using System.IO;
using Dirtybase.App;
using NUnit.Framework;
using System.Data.SQLite;
using SharpTestsEx;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    public class InitTests
    {
        private const string databaseFile = "Dirtybase.db";
        private const string connectionstring = "Data Source = " + databaseFile + ";Version=3;";
        private const string arguments = "init -db sqlite -cs " + connectionstring;
        private const string versionTableName = "dirtybase.Version";

        [Test]
        [Category(TestTypes.EndToEnd)]
        public void InitOnSqlDatabaseShouldAddDirtyBaseVerionTable()
        {
            MakeSqliteDatabase();
            Program.Main(arguments.Split(' '));
            AssertDirtybaseTableIsMade();
        }

        private void MakeSqliteDatabase()
        {
            if (File.Exists(databaseFile))
            {
                File.Delete(databaseFile);
            }
            SQLiteConnection.CreateFile(databaseFile);
        }

        private void AssertDirtybaseTableIsMade()
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
            }
            hasTable.Should().Be.True();
        }
    }
}