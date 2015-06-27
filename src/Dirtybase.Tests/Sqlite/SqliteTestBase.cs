using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using NUnit.Framework;

namespace Dirtybase.Tests.Sqlite
{
    public abstract class SqliteTestBase
    {
        protected const string databaseFile = "Dirtybase.db";
        protected const string connectionstring = "Data Source = " + databaseFile + ";Version=3;";
        protected const string versionTableName = "DirtybaseVersions";
        protected const string createVerisonTableQuery =
            "CREATE TABLE " + versionTableName + "(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)";

        [SetUp]
        public virtual void SetUp()
        {
            this.MakeSqliteDatabase();
        }

        [TearDown]
        public virtual void TearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            DeleteSqliteDatabase();
        }

        private void MakeSqliteDatabase()
        {
            DeleteSqliteDatabase();
            SQLiteConnection.CreateFile(databaseFile);
        }

        private static void DeleteSqliteDatabase()
        {
            if (File.Exists(databaseFile))
            {
                File.Delete(databaseFile);
            }
        }

        protected void CreateVersionTable()
        {
            RunCommands(new[] { createVerisonTableQuery });
        }

        protected void RunCommands(IEnumerable<string> queries)
        {
            using (var connection = new SQLiteConnection(connectionstring))
            {
                connection.Open();
                var command = connection.CreateCommand();
                try
                {
                    foreach(var query in queries)
                    {
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }

        protected void AssertAgainstDatabase(Func<SQLiteConnection, bool> assert)
        {
            using (var connection = new SQLiteConnection(connectionstring))
            {
                connection.Open();
                try
                {
                    assert(connection);
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }
    }
}