using System;
using System.Data.SQLite;
using System.IO;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlImplementations
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class SqliteTests : DbTestBase<SQLiteConnection, SQLiteException>
    {
        protected const string databaseFile = "Dirtybase.db";
        protected override string ConnectionString { get { return "Data Source = " + databaseFile + ";Version=3;"; } }
        protected override string VersionTableName { get { return "DirtybaseVersions"; } }
        protected override string CreateVerisonTableQuery { get { return "CREATE TABLE " + this.VersionTableName + "(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)"; } }
        protected override string SelectFromVersionTableQuery { get { return "SELECT name FROM sqlite_master WHERE name ='" + this.VersionTableName + "';"; } }

        protected override string InitArguments { get { return "init -db sqlite -cs " + this.ConnectionString; } }
        protected override string MigrateArgs { get { return "migrate -db sqlite -cs " + this.ConnectionString + " -vf " + scriptFolder; } }

        protected override string AssertTableQuery(string tableName)
        {
            return string.Format("SELECT count(name) FROM sqlite_master where name = '{0}';", tableName);
        }

        [SetUp]
        public override void SetUp()
        {
            this.MakeSqliteDatabase();
            base.SetUp();
        }

        [TearDown]
        public override void TearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            DeleteSqliteDatabase();
            base.TearDown();
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