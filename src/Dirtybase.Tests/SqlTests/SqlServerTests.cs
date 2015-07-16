using System;
using System.Data.SqlClient;
using Dirtybase.Core.Exceptions;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlTests
{
    [TestFixture]
    [Category(TestTypes.LongRunning)]
    public class SqlServerTests : DbTestBase<SqlConnection, SqlException>
    {
        private const string schema = "Dirty";
        private const string versionTable = "Versions";
        protected override string ConnectionString { get { return "Server=.; Database=DirtyTest; Trusted_Connection=True;"; } }
        protected override string VersionTableName { get { return schema + "." + versionTable; } }
        protected override string CreateVerisonTableQuery { get { return "Create schema " + schema + " CREATE TABLE " + this.VersionTableName + "(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)"; } }
        protected override string SelectFromVersionTableQuery { get { return "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + schema + "' AND TABLE_NAME ='" + versionTable + "';"; } }
        
        protected override string InitArguments { get { return "init -db sql -cs " + this.ConnectionString; } }
        protected override string MigrateArgs { get { return "migrate -db sql -cs " + this.ConnectionString + " -vf " + scriptFolder; } }

        protected override string AssertTableQuery(string tableName)
        {
            return string.Format("SELECT count(TABLE_NAME) FROM INFORMATION_SCHEMA.TABLES where TABLE_NAME = '{0}';", tableName);
        }
        
        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public override void IfDatabaseDoesNotExistOnInitThrowException()
        {
            this.TearDown();
            const string args = "init -db sql -cs Server=.; Database=DirtyFoo; Trusted_Connection=True;";
            this.api.Do(args.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public override void IfDatabaseDoesNotExistOnMigrateThrowException()
        {
            this.TearDown();
            const string args = "init -db sql -cs Server=.; Database=DirtyFoo; Trusted_Connection=True;";
            this.api.Do(args.Split(' '));
        }

        [TearDown]
        public override void TearDown()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            using (var connection = this.MakeConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    CleanupQuery(command, "DROP TABLE " + VersionTableName);
                    CleanupQuery(command, "DROP TABLE dbo.Team");
                    CleanupQuery(command, "DROP TABLE dbo.Employee");
                    CleanupQuery(command, "drop SCHEMA Dirty");
                }
                connection.Close();
            }
            base.TearDown();
        }

        private static void CleanupQuery(SqlCommand command, string query)
        {
            try
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
            catch(Exception)
            {
                //cleanup query not valid
            }
        }
    }
}