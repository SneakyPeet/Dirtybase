﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Dirtybase.App;
using Dirtybase.App.Exceptions;
using Dirtybase.App.Implementations.Sqlite;
using Dirtybase.App.Options.Validators;
using NUnit.Framework;

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
        private const string v2 = "v2_CreateEmployeeTable.sql";
        private const string v3 = "v3_DeleteTeamTable.sql";
        private const string badfilename = "v_BadName.sql";

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
            AssertAgainstDatabase(DatabaseAtVersion1);
        }
        
        [Test]
        public void MultipleNewVersionShouldUpdateDatabaseAndVersionTableInOrder()
        {
            CopyFileToScriptFolder(v1);
            CopyFileToScriptFolder(v2);
            CopyFileToScriptFolder(v3);
            Program.Main(initArgs.Split(' '));
            Program.Main(migrateArgs.Split(' '));
            AssertAgainstDatabase(DatabaseAtVersion3);
        }

        [Test]
        [ExpectedException(typeof(VersionFileNameFormatException), ExpectedMessage = "v_BadName.sql does not conform to the file naming convention")]
        public void FileWithBadVersionNameShouldThrowException()
        {
            CopyFileToScriptFolder(badfilename);
            Program.Main(initArgs.Split(' '));
            Program.Main(migrateArgs.Split(' '));
        }

        [Test]
        public void NewVersionsOverExistingVersionShouldApplyNewVersionsOnly()
        {
            //given
            CopyFileToScriptFolder(v1);
            CopyFileToScriptFolder(v2);
            CopyFileToScriptFolder(v3);
            Program.Main(initArgs.Split(' '));
            ApplyVersion1();
            AssertAgainstDatabase(DatabaseAtVersion1);
            //when - then
            Program.Main(migrateArgs.Split(' '));
            AssertAgainstDatabase(DatabaseAtVersion3);
        }

        //[Test]
        //public void FilesShouldBeAppliedInOrder()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void AllFilesInSubFoldersShouldBeApplied()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void TestTransactions()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void FilesWithGoSeperatorShouldRun()
        //{
        //    throw new NotImplementedException();
        //}

        //[Test]
        //public void WarnIfFileInDBandNotInFolder()
        //{
        //    throw new NotImplementedException();
        //}

        private void ApplyVersion1()
        {
            using (var connection = new SQLiteConnection(connectionstring))
            {
                connection.Open();
                try
                {
                    var query = "CREATE TABLE Team ( TeamId INT PRIMARY KEY, name nvarchar(20));" +
                                string.Format("INSERT INTO {0} (Version, FileName, DateAppliedUtc) VALUES ('{1}', '{2}', '{3}')", versionTableName, "v1", v1, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
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
            errors.AddRange(AssertTable(true, connection, "Team"));
            errors.AddRange(HasV1Row(connection));
            return errors;
        }

        private Errors DatabaseAtVersion3(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(AssertTable(false, connection, "Team"));
            errors.AddRange(AssertTable(true, connection, "Employee"));
            errors.AddRange(HasV3Rows(connection));
            return errors;
        }

        private IEnumerable<string> HasV3Rows(SQLiteConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(HasV1Row(connection));
            errors.AddRange(HasV2Row(connection));
            errors.AddRange(HasV3Row(connection));
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
            return HasVersionRow(connection, "v1", v1);
        }

        private IEnumerable<string> HasV2Row(SQLiteConnection connection)
        {
            return HasVersionRow(connection, "v2", v2);
        }

        private IEnumerable<string> HasV3Row(SQLiteConnection connection)
        {
            return HasVersionRow(connection, "v3", v3);
        }

        private Errors HasVersionRow(SQLiteConnection connection, string version, string fileName)
        {
            var query = string.Format("SELECT count(Version) FROM {0} where Version = '{1}' AND FileName = '{2}';", versionTableName, version, fileName);
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

        private void CopyFileToScriptFolder(string fileName)
        {
            string source = Path.Combine("TestScripts", fileName);
            string destination = Path.Combine(scriptFolder, fileName);
            File.Copy(source, destination, true);
        }
    }
}