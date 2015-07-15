using System;
using System.Data.SQLite;
using System.IO;
using Dirtybase.Tests.SqlImplementations.Helpers;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlImplementations.Sqlite
{
    public abstract class SqliteTestBase : DbTestBase<SQLiteConnection>
    {
        protected const string databaseFile = "Dirtybase.db";
        protected override string ConnectionString { get { return "Data Source = " + databaseFile + ";Version=3;"; } }
        protected override string VersionTableName { get { return "DirtybaseVersions"; } }
        protected override string CreateVerisonTableQuery { get { return "CREATE TABLE " + VersionTableName + "(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)"; } }
        protected override string SelectFromVersionTableQuery { get { return "SELECT name FROM sqlite_master WHERE name ='" + VersionTableName + "';"; } }

        protected override string AssertTableQuery(string tableName)
        {
            return string.Format("SELECT count(name) FROM sqlite_master where name = '{0}';", tableName);
        }

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
    }
}