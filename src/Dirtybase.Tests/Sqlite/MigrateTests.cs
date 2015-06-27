using System;
using System.Data.SQLite;
using System.IO;
using Dirtybase.App;
using Dirtybase.App.Implementations.Sqlite;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Sqlite
{
    [TestFixture]
    [Category(TestTypes.EndToEnd)]
    public class MigrateTests : SqliteTestBase
    {
        private const string scriptFolder = "testfolder";
        private const string initArgs = "init -db sqlite -cs " + connectionstring;
        private const string migrateArgs = "migrate -db sqlite -cs " + connectionstring + " -sf " + scriptFolder;
        private const string v1 = "v1_CreateTeamTable.sql";

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            CreateScriptsFolder();
        }

        [SetUp]
        public override void TearDown()
        {
            base.TearDown();
            DeleteScriptsFolder();
        }

        private void CreateScriptsFolder()
        {
            DeleteScriptsFolder();
            Directory.CreateDirectory(scriptFolder);
        }

        private static void DeleteScriptsFolder()
        {
            if(Directory.Exists(scriptFolder))
            {
                Directory.Delete(scriptFolder, true);
            }
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            TearDown();
            Program.Main(migrateArgs.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Dirtybase Not Initialized. Run init Command")]
        public void IfVersionTableDoesNotExistThrowException()
        {
            Program.Main(migrateArgs.Split(' '));
        }

        [Test]
        public void NewVersionShouldUpdateDatabaseAndVersionTable()
        {
            CopyFileToScriptFolder(v1);
            Program.Main(initArgs.Split(' '));
            Program.Main(migrateArgs.Split(' '));
            AssertAgainstDatabase(HasUpdatedTeamTable);
            AssertAgainstDatabase(HasV1Row);
        }

        private bool HasUpdatedTeamTable(SQLiteConnection connection)
        {
            const string query = "SELECT name FROM sqlite_master where name = 'Team';";
            var command = new SQLiteCommand(query, connection);
            using (var reader = command.ExecuteReader())
            {
                Assert.IsTrue(reader.HasRows, "Team table should be updated");
            }
            return true;
        }

        private bool HasV1Row(SQLiteConnection connection)
        {
            const string query = "SELECT count(Version) FROM " + versionTableName + " where Version = 'v1' AND FileName = '" + v1 + "';";
            var command = new SQLiteCommand(query, connection);
            var rowcount = Convert.ToInt32(command.ExecuteScalar());
            Assert.IsTrue(rowcount == 1, "Version Should Be Added To Version Table");
            return true;
        }

        private void CopyFileToScriptFolder(string fileName)
        {
            string source = Path.Combine("TestScripts", fileName);
            string destination = Path.Combine(scriptFolder, fileName);
            File.Copy(source, destination, true);
        }
    }
}