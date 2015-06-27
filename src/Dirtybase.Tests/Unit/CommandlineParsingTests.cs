using System;
using Dirtybase.App;
using Dirtybase.App.Options;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class CommandlineParsingTests
    {
        private const string validSqlConnectionString =
            "Server=myServerName\\myInstanceName;Database=myDataBase;User Id=myUsername;Password=myPassword;";
        private const string validSqliteConnectionString = "Data Source=c:\\mydb.db;Version=3;";

        private static readonly object[] positiveTestCases =
            {
                new object[] { "init -db sql -cs " + validSqlConnectionString, new DirtyOptions(DirtyCommand.Init, DatabaseType.Sql, validSqlConnectionString) },
                new object[] { "init -db sqlite -cs " + validSqliteConnectionString, new DirtyOptions(DirtyCommand.Init, DatabaseType.Sqlite, validSqliteConnectionString) },
                new object[] { "migrate -db sql -cs " + validSqlConnectionString, new DirtyOptions(DirtyCommand.Migrate, DatabaseType.Sql, validSqlConnectionString) },
                new object[] { "migrate -db sqlite -cs " + validSqliteConnectionString, new DirtyOptions(DirtyCommand.Migrate, DatabaseType.Sqlite, validSqliteConnectionString) },
                new object[] { "help", new DirtyOptions(DirtyCommand.Help, null, null) }
            };

        [Test]
        [TestCaseSource("positiveTestCases")]
        public void GivenPositiveTestCaseOptionsShouldMatch(string input, DirtyOptions expectedOptions)
        {
            var args = input.Split(' ');
            var options = new DirtyOptions(args);
            options.Should().Be.EqualTo(expectedOptions);
        }

        private static readonly object[] negativeTestCases =
            {
                new object[] { "", typeof(ArgumentException), "use 'help' option for help" },
                new object[] { "foo", typeof(ArgumentException), "foo is not an option. use 'help' option for help" },
                new object[] { "init", typeof(ArgumentException), "Database Type Required. use 'help' option for help" },
                new object[] { "init -db", typeof(ArgumentException), "Database Type Required. use 'help' option for help" },
                new object[] { "init -db foo", typeof(ArgumentException), "foo is not a supported Database. use 'help' option for help" },
                new object[] { "init -db sql", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "init -db sql -cs", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "init -db sql -cs foo", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "init -db sqlite", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "init -db sqlite -cs", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "init -db sqlite -cs foo", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "migrate", typeof(ArgumentException), "Database Type Required. use 'help' option for help" },
                new object[] { "migrate -db", typeof(ArgumentException), "Database Type Required. use 'help' option for help" },
                new object[] { "migrate -db foo", typeof(ArgumentException), "foo is not a supported Database. use 'help' option for help" },
                new object[] { "migrate -db sql", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "migrate -db sql -cs", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "migrate -db sql -cs foo", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "migrate -db sqlite", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "migrate -db sqlite -cs", typeof(ArgumentException), "Invalid Connection String" },
                new object[] { "migrate -db sqlite -cs foo", typeof(ArgumentException), "Invalid Connection String" }
            };

        [Test]
        [TestCaseSource("negativeTestCases")]
        public void GivenNegativeTestCaseThrowException(string input, Type expectedException, string message)
        {
            try
            {
                var args= input.Split(' ');
                new DirtyOptions(args);
                throw new Exception("exception not thrown");
            }
            catch(Exception e)
            {
                if(e.GetType() == expectedException)
                {
                    e.Message.Should().Be.EqualTo(message);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
