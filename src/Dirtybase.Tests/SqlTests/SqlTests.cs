using System.Data.SqlClient;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlTests
{
    [TestFixture]
    [Category(TestTypes.LongRunning)]
    public class SqlServerTests : DbTestBase<SqlConnection, SqlException>
    {
        protected override string ConnectionString { get { return "Server=.; Database=DirtyTest; Trusted_Connection=True;"; } }
        protected override string VersionTableName { get { return "DirtybaseVersions"; } }
        protected override string CreateVerisonTableQuery { get { return "CREATE TABLE " + this.VersionTableName + "(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)"; } }//todo
        protected override string SelectFromVersionTableQuery { get { return "SELECT name FROM sqlite_master WHERE name ='" + this.VersionTableName + "';"; } }//todo

        protected override string InitArguments { get { return "init -db sql -cs " + this.ConnectionString; } }
        protected override string MigrateArgs { get { return "migrate -db sql -cs " + this.ConnectionString + " -vf " + scriptFolder; } }

        protected override string AssertTableQuery(string tableName)
        {
            return string.Format("SELECT count(name) FROM sqlite_master where name = '{0}';", tableName); //todo
        }

        //[SetUp]
        //public override void SetUp()
        //{
        //    base.SetUp();
        //}

        //[TearDown]
        //public override void TearDown()
        //{
        //    base.TearDown();
        //}
    }
}