using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options.Validators;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlImplementations.Sqlite
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class MigrateTests : SqliteTestBase
    {
        private const string scriptFolder = "testfolder";
        private string InitArgs { get { return "init -db sqlite -cs " + this.ConnectionString; } }
        private string MigrateArgs {get {return "migrate -db sqlite -cs " + this.ConnectionString + " -vf " + scriptFolder;}}
        private const string v1 = "v1_CreateTeamTable.sql";
        private const string v2 = "v2_CreateEmployeeTable.sql";
        private const string v3 = "v3_DeleteTeamTable.sql";
        private const string badfilename = "v_BadName.sql";
        private const string v22 = "v22_CreateTeamTable.sql";
        private const string v113 = "v113_DeleteTeamTable.sql";
        private const string v115 = "v1.1.5_CreateTeamTable.sql";
        private const string v1115 = "v1.1.15_DeleteTeamTable.sql";
        private const string vGo = "vgo_TestGoStatements.sql";
        private const string v1InvalidGo = "v1Invalidgo_TestGoStatements.sql";
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            this.CreateScriptsFolder();
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
                var info = Directory.GetDirectories(scriptFolder);
                foreach(var directory in info)
                {
                    Directory.Delete(directory, true);
                }
                Directory.Delete(scriptFolder, true);
            }
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public void IfDatabaseDoesNotExistThrowException()
        {
            this.TearDown();
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Dirtybase Not Initialized. Run init Command")]
        public void IfVersionTableDoesNotExistThrowException()
        {
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void NewVersionShouldUpdateDatabaseAndVersionTable()
        {
            this.CopyFileToScriptFolder(v1);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersion1);
        }
        
        [Test]
        public void MultipleNewVersionShouldUpdateDatabaseAndVersionTableInOrder()
        {
            this.CopyFileToScriptFolder(v1);
            this.CopyFileToScriptFolder(v2);
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersion3);
        }

        [Test]
        [ExpectedException(typeof(VersionFileNameFormatException), ExpectedMessage = "v_BadName.sql does not conform to the file naming convention")]
        public void FileWithBadVersionNameShouldThrowException()
        {
            this.CopyFileToScriptFolder(badfilename);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void NewVersionsOverExistingVersionShouldApplyNewVersionsOnly()
        {
            //given
            this.CopyFileToScriptFolder(v1);
            this.CopyFileToScriptFolder(v2);
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArgs.Split(' '));
            this.ApplyVersion1();
            this.AssertAgainstDatabase(this.DatabaseAtVersion1);
            //when - then
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersion3);
        }

        [Test]
        public void AllFilesInSubFoldersShouldBeApplied()
        {
            this.CopyFileToScriptFolder(v1);
            this.CopyFileToScriptFolder(v2,scriptFolder + "\\fooFolder");
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersion3);
        }

        [Test]
        [ExpectedException(typeof(VersionFileMissingException), ExpectedMessage = "The version file 'v1_CreateTeamTable.sql' cannot be found in script directory. Inconsistent versioning")]
        public void VersionRowInDbAndNotInFolderShouldThrowException()
        {
            //given
            this.CopyFileToScriptFolder(v2);
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArgs.Split(' '));
            this.ApplyVersion1();
            this.AssertAgainstDatabase(this.DatabaseAtVersion1);
            //when - then
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void FilesShouldBeAppliedInOrder()
        {
            //Will Fail If Not Applied In Order
            this.CopyFileToScriptFolder(v22);
            this.CopyFileToScriptFolder(v113);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void FunnyVersionNamedFilesShouldBeAppliedInOrder()
        {
            //Will Fail If Not Applied In Order
            this.CopyFileToScriptFolder(v115);
            this.CopyFileToScriptFolder(v1115);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void FilesWithGoSeperatorShouldRun()
        {
            this.CopyFileToScriptFolder(vGo);
            this.api.Do(this.InitArgs.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersionGo);
        }

        [Test]
        public void InvalidGoSeperatedFileShouldNotApplyAnyStatements()
        {
            this.CopyFileToScriptFolder(v1InvalidGo);
            this.api.Do(this.InitArgs.Split(' '));
            try
            {
                this.api.Do(this.MigrateArgs.Split(' '));
            }
            catch(SQLiteException e)
            {
                Assert.AreEqual("SQL logic error or missing database\r\nno such table: Teamfd", e.Message, "Not Expected Exception");
            }
            
            this.AssertAgainstDatabase(this.DatabaseNotAtVersionGo);
        }

        [Test]
        public void OnErrorShouldNotApplySubsequintFiles()
        {
            this.CopyFileToScriptFolder(v1InvalidGo);
            this.CopyFileToScriptFolder(v22);
            this.api.Do(this.InitArgs.Split(' '));
            try
            {
                this.api.Do(this.MigrateArgs.Split(' '));
            }
            catch (SQLiteException e)
            {
                Assert.AreEqual("SQL logic error or missing database\r\nno such table: Teamfd", e.Message, "Not Expected Exception");
            }
            this.AssertAgainstDatabase(this.DatabaseNotAtVersionGo);
        }

        private void ApplyVersion1()
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                try
                {
                    var query = "CREATE TABLE Team ( TeamId INT PRIMARY KEY, name nvarchar(20));" +
                                string.Format("INSERT INTO {0} (Version, FileName, DateAppliedUtc) VALUES ('{1}', '{2}', '{3}')", VersionTableName, "1", v1, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    using(var command = new SQLiteCommand(query, connection)) 
                    {
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

        private Errors DatabaseAtVersion1(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(true, connection, "Team"));
            errors.AddRange(this.HasV1Row(connection));
            return errors;
        }

        private Errors DatabaseAtVersion3(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(false, connection, "Team"));
            errors.AddRange(this.AssertTable(true, connection, "Employee"));
            errors.AddRange(this.HasV3Rows(connection));
            return errors;
        }

        private Errors DatabaseAtVersionGo(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(false, connection, "Team"));
            errors.AddRange(this.AssertTable(true, connection, "Employee"));
            errors.AddRange(this.HasVersionRow(connection, "go", vGo));
            return errors;
        }

        private Errors DatabaseNotAtVersionGo(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(false, connection, "Team"));
            errors.AddRange(this.AssertTable(false, connection, "Employee"));
            errors.AddRange(this.DoesNotHaveVersionRow(connection, "1InvalidGo", v1InvalidGo));
            return errors;
        }

        private IEnumerable<string> HasV3Rows(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.HasV1Row(connection));
            errors.AddRange(this.HasV2Row(connection));
            errors.AddRange(this.HasV3Row(connection));
            return errors;
        }

        private IEnumerable<string> AssertTable(bool exists, SQLiteConnection connection, string tableName)
        {
            string query = string.Format("SELECT count(name) FROM sqlite_master where name = '{0}';", tableName);
            int rowCount;
            using(var command = new SQLiteCommand(query, connection)) 
            {
                rowCount = Convert.ToInt32(command.ExecuteScalar());
            }

            var expectedRowCount = exists ? 1 : 0;
            var text = exists ? "Created" : "Deleted";
            if(rowCount == expectedRowCount)
            {
                return new Errors();
            }
            return new Errors{string.Format("{0} Table Should Be {1}", tableName, text)};
        }

        private IEnumerable<string> HasV1Row(SQLiteConnection connection)
        {
            return this.HasVersionRow(connection, "1", v1);
        }

        private IEnumerable<string> HasV2Row(SQLiteConnection connection)
        {
            return this.HasVersionRow(connection, "2", v2);
        }

        private IEnumerable<string> HasV3Row(SQLiteConnection connection)
        {
            return this.HasVersionRow(connection, "3", v3);
        }

        private Errors HasVersionRow(SQLiteConnection connection, string version, string fileName)
        {
            var query = string.Format("SELECT count(Version) FROM {0} where Version = '{1}' AND FileName = '{2}';", VersionTableName, version, fileName);
            int rowcount;
            using(var command = new SQLiteCommand(query, connection)) 
            {
                rowcount = Convert.ToInt32(command.ExecuteScalar());
            }
            if (rowcount == 1)
            {
                return new Errors();
            }
            return new Errors { string.Format("Version {0} Should Be Added To Version Table", version) };
        }

        private Errors DoesNotHaveVersionRow(SQLiteConnection connection, string version, string fileName)
        {
            var query = string.Format("SELECT count(Version) FROM {0} where Version = '{1}' AND FileName = '{2}';", VersionTableName, version, fileName);
            int rowcount;
            using (var command = new SQLiteCommand(query, connection))
            {
                rowcount = Convert.ToInt32(command.ExecuteScalar());
            }
            if (rowcount == 0)
            {
                return new Errors();
            }
            return new Errors { string.Format("Version {0} Should Not Be Added To Version Table", version) };
        }

        private void CopyFileToScriptFolder(string fileName)
        {
            this.CopyFileToScriptFolder(fileName, scriptFolder);
        }

        private void CopyFileToScriptFolder(string fileName, string folderpath)
        {
            string source = Path.Combine("TestScripts", fileName);
            if(!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            string destination = Path.Combine(folderpath, fileName);
            File.Copy(source, destination, true);
        }
    }
}