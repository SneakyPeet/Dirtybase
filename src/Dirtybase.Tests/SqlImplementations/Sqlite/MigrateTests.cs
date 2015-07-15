using System.Data.SQLite;
using System.IO;
using Dirtybase.Core.Exceptions;
using NUnit.Framework;

namespace Dirtybase.Tests.SqlImplementations.Sqlite
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class MigrateTests : SqliteTestBase
    {
        private string InitArgs { get { return "init -db sqlite -cs " + this.ConnectionString; } }
        private string MigrateArgs {get {return "migrate -db sqlite -cs " + this.ConnectionString + " -vf " + scriptFolder;}}
        
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
    }
}