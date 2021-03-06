﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dirtybase.Core;
using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options.Validators;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlTests
{
    public abstract class DbTestBase<TDbConnection, TDbException> 
        where TDbConnection : DbConnection 
        where TDbException: DbException
    {
        protected abstract string ConnectionString { get; }
        protected abstract string VersionTableName { get; }
        protected abstract string CreateVerisonTableQuery { get; }
        protected abstract string SelectFromVersionTableQuery { get; }
        protected abstract string InitArguments { get; }
        protected abstract string MigrateArgs { get; }
        protected abstract string AssertTableQuery(string tableName);

        protected const string scriptFolder = "testfolder";
        protected const string v1 = "v1_CreateTeamTable.sql";
        protected const string v2 = "v2_CreateEmployeeTable.sql";
        protected const string v3 = "v3_DeleteTeamTable.sql";
        protected const string badfilename = "v_BadName.sql";
        protected const string v22 = "v22_CreateTeamTable.sql";
        protected const string v113 = "v113_DeleteTeamTable.sql";
        protected const string v115 = "v1.1.5_CreateTeamTable.sql";
        protected const string v1115 = "v1.1.15_DeleteTeamTable.sql";
        protected const string vGo = "vgo_TestGoStatements.sql";
        protected const string v1InvalidGo = "v1Invalidgo_TestGoStatements.sql";

        protected DirtybaseApi api = new DirtybaseApi(new TestNotifier());

        [SetUp]
        public virtual void SetUp()
        {
            this.CreateScriptsFolder();
        }

        [SetUp]
        public virtual void TearDown()
        {
            DeleteScriptsFolder();
        }

        #region Init Tests

        [Test]
        public void InitShouldAddDirtyBaseVersionTable()
        {
            this.api.Do(this.InitArguments.Split(' '));
            this.AssertAgainstDatabase(this.DirtybaseVersionTableExists);
        }

        [Test]
        public void InitOnExistingDirtyBaseDoNothing()
        {
            this.CreateVersionTable();
            this.api.Do(this.InitArguments.Split(' '));
        }

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public virtual void IfDatabaseDoesNotExistOnInitThrowException()
        {
            this.TearDown();
            this.api.Do(this.InitArguments.Split(' '));
        }

        #endregion

        #region Migration Tests

        [Test]
        [ExpectedException(typeof(DirtybaseException), ExpectedMessage = "Database Does Not Exist")]
        public virtual void IfDatabaseDoesNotExistOnMigrateThrowException()
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
            this.api.Do(this.InitArguments.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersion1);
        }

        [Test]
        public void MultipleNewVersionShouldUpdateDatabaseAndVersionTableInOrder()
        {
            this.CopyFileToScriptFolder(v1);
            this.CopyFileToScriptFolder(v2);
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArguments.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersion3);
        }

        [Test]
        [ExpectedException(typeof(VersionFileNameFormatException), ExpectedMessage = "v_BadName.sql does not conform to the file naming convention")]
        public void FileWithBadVersionNameShouldThrowException()
        {
            this.CopyFileToScriptFolder(badfilename);
            this.api.Do(this.InitArguments.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void NewVersionsOverExistingVersionShouldApplyNewVersionsOnly()
        {
            //given
            this.CopyFileToScriptFolder(v1);
            this.CopyFileToScriptFolder(v2);
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArguments.Split(' '));
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
            this.CopyFileToScriptFolder(v2, scriptFolder + "\\fooFolder");
            this.CopyFileToScriptFolder(v3);
            this.api.Do(this.InitArguments.Split(' '));
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
            this.api.Do(this.InitArguments.Split(' '));
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
            this.api.Do(this.InitArguments.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void FunnyVersionNamedFilesShouldBeAppliedInOrder()
        {
            //Will Fail If Not Applied In Order
            this.CopyFileToScriptFolder(v115);
            this.CopyFileToScriptFolder(v1115);
            this.api.Do(this.InitArguments.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
        }

        [Test]
        public void FilesWithGoSeperatorShouldRun()
        {
            this.CopyFileToScriptFolder(vGo);
            this.api.Do(this.InitArguments.Split(' '));
            this.api.Do(this.MigrateArgs.Split(' '));
            this.AssertAgainstDatabase(this.DatabaseAtVersionGo);
        }

        [Test]
        public void InvalidGoSeperatedFileShouldNotApplyAnyStatements()
        {
            this.CopyFileToScriptFolder(v1InvalidGo);
            this.api.Do(this.InitArguments.Split(' '));
            try
            {
                this.api.Do(this.MigrateArgs.Split(' '));
            }
            catch (SQLiteException e)
            {
                Assert.AreEqual("SQL logic error or missing database\r\nno such table: Teamfd", e.Message, "Not Expected Exception");
            }
            catch (SqlException e)
            {
                Assert.AreEqual("Cannot drop the table 'Teamfd', because it does not exist or you do not have permission.", e.Message, "Not Expected Exception");
            }
            
            this.AssertAgainstDatabase(this.DatabaseNotAtVersionGo);
        }

        [Test]
        public void OnErrorShouldNotApplySubsequintFiles()
        {
            this.CopyFileToScriptFolder(v1InvalidGo);
            this.CopyFileToScriptFolder(v22);
            this.api.Do(this.InitArguments.Split(' '));
            try
            {
                this.api.Do(this.MigrateArgs.Split(' '));
            }
            catch (SQLiteException e)
            {
                Assert.AreEqual("SQL logic error or missing database\r\nno such table: Teamfd", e.Message, "Not Expected Exception");
            }
            catch (SqlException e)
            {
                Assert.AreEqual("Cannot drop the table 'Teamfd', because it does not exist or you do not have permission.", e.Message, "Not Expected Exception");
            }
            this.AssertAgainstDatabase(this.DatabaseNotAtVersionGo);
        }

        #endregion 

        protected void CreateVersionTable()
        {
            this.RunCommands(new[] { this.CreateVerisonTableQuery });
        }

        protected void RunCommands(IEnumerable<string> queries)
        {
            
            using (var connection = this.MakeConnection())
            {
                connection.Open();
                using(var command = connection.CreateCommand()) 
                {
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
                }
                connection.Close();
            }
        }

        protected void AssertAgainstDatabase(Func<TDbConnection, Errors> validator)
        {
            using (var connection = this.MakeConnection())
            {
                connection.Open();
                try
                {
                    var errors = validator(connection);
                    if(errors.Any())
                    {
                        Assert.Fail(errors.Message);
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

        protected TDbConnection MakeConnection()
        {
            return Activator.CreateInstance(typeof(TDbConnection), new object[] { this.ConnectionString }) as TDbConnection;
        }
        
        protected Errors DirtybaseVersionTableExists(TDbConnection connection)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = this.SelectFromVersionTableQuery;
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        return new Errors();
                    }
                    return new Errors { "Dirtybase Version Table Does Not Exist" };
                }
            }
        }

        protected void ApplyVersion1()
        {
            using (var connection = this.MakeConnection())
            {
                connection.Open();
                try
                {
                    var query = "CREATE TABLE Team ( TeamId INT PRIMARY KEY, name nvarchar(20));" +
                                String.Format("INSERT INTO {0} (Version, FileName, DateAppliedUtc) VALUES ('{1}', '{2}', '{3}')", this.VersionTableName, "1", v1, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    using (var command = connection.CreateCommand())
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

        protected Errors DatabaseAtVersion1(TDbConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(true, connection, "Team"));
            errors.AddRange(this.HasV1Row(connection));
            return errors;
        }

        protected Errors DatabaseAtVersion3(TDbConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(false, connection, "Team"));
            errors.AddRange(this.AssertTable(true, connection, "Employee"));
            errors.AddRange(this.HasV3Rows(connection));
            return errors;
        }

        protected Errors DatabaseAtVersionGo(TDbConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(false, connection, "Team"));
            errors.AddRange(this.AssertTable(true, connection, "Employee"));
            errors.AddRange(this.HasVersionRow(connection, "go", vGo));
            return errors;
        }

        protected Errors DatabaseNotAtVersionGo(TDbConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.AssertTable(false, connection, "Team"));
            errors.AddRange(this.AssertTable(false, connection, "Employee"));
            errors.AddRange(this.DoesNotHaveVersionRow(connection, "1InvalidGo", v1InvalidGo));
            return errors;
        }

        protected IEnumerable<string> HasV3Rows(TDbConnection connection)
        {
            var errors = new Errors();
            errors.AddRange(this.HasV1Row(connection));
            errors.AddRange(this.HasV2Row(connection));
            errors.AddRange(this.HasV3Row(connection));
            return errors;
        }

        protected IEnumerable<string> HasV1Row(TDbConnection connection)
        {
            return this.HasVersionRow(connection, "1", v1);
        }

        protected IEnumerable<string> HasV2Row(TDbConnection connection)
        {
            return this.HasVersionRow(connection, "2", v2);
        }

        protected IEnumerable<string> HasV3Row(TDbConnection connection)
        {
            return this.HasVersionRow(connection, "3", v3);
        }

        protected IEnumerable<string> AssertTable(bool exists, TDbConnection connection, string tableName)
        {
            int rowCount;
            using (var command = connection.CreateCommand())
            {
                command.CommandText = this.AssertTableQuery(tableName);
                rowCount = Convert.ToInt32(command.ExecuteScalar());
            }

            var expectedRowCount = exists ? 1 : 0;
            var text = exists ? "Created" : "Deleted";
            if (rowCount == expectedRowCount)
            {
                return new Errors();
            }
            return new Errors { String.Format("{0} Table Should Be {1}", tableName, text) };
        }

        protected Errors HasVersionRow(TDbConnection connection, string version, string fileName)
        {
            var rowcount = this.GetVersionTableRowCount(connection, version, fileName);
            if (rowcount == 1)
            {
                return new Errors();
            }
            return new Errors { String.Format("Version {0} Should Be Added To Version Table", version) };
        }

        protected Errors DoesNotHaveVersionRow(TDbConnection connection, string version, string fileName)
        {
            var rowcount = this.GetVersionTableRowCount(connection, version, fileName);
            if (rowcount == 0)
            {
                return new Errors();
            }
            return new Errors { String.Format("Version {0} Should Not Be Added To Version Table", version) };
        }

        private int GetVersionTableRowCount(TDbConnection connection, string version, string fileName)
        {
            var query = String.Format("SELECT count(Version) FROM {0} where Version = '{1}' AND FileName = '{2}';", this.VersionTableName, version, fileName);
            int rowcount;
            using(var command = connection.CreateCommand())
            {
                command.CommandText = query;
                rowcount = Convert.ToInt32(command.ExecuteScalar());
            }
            return rowcount;
        }

        protected void CreateScriptsFolder()
        {
            DeleteScriptsFolder();
            Directory.CreateDirectory(scriptFolder);
        }

        protected static void DeleteScriptsFolder()
        {
            if (Directory.Exists(scriptFolder))
            {
                var info = Directory.GetDirectories(scriptFolder);
                foreach (var directory in info)
                {
                    Directory.Delete(directory, true);
                }
                Directory.Delete(scriptFolder, true);
            }
        }

        protected void CopyFileToScriptFolder(string fileName)
        {
            this.CopyFileToScriptFolder(fileName, scriptFolder);
        }

        protected void CopyFileToScriptFolder(string fileName, string folderpath)
        {
            string source = Path.Combine("TestScripts", fileName);
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            string destination = Path.Combine(folderpath, fileName);
            File.Copy(source, destination, true);
        }
    }
}