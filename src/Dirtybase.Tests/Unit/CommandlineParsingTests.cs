using System;
using Dirtybase.App;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    public class CommandlineParsingTests
    {
        private static readonly object[] positiveTestCases =
            {
                new object[] { "init -db sql", new DirtyOptions(DirtyCommand.Init, DatabaseType.Sql) },
                new object[] { "init -db sqlite", new DirtyOptions(DirtyCommand.Init, DatabaseType.Sqlite) },
                new object[] { "migrate -db sql", new DirtyOptions(DirtyCommand.Migrate, DatabaseType.Sql) },
                new object[] { "migrate -db sqlite", new DirtyOptions(DirtyCommand.Migrate, DatabaseType.Sqlite) },
                new object[] { "help", new DirtyOptions(DirtyCommand.Help, null) }
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
                new object[] { "migrate", typeof(ArgumentException), "Database Type Required. use 'help' option for help" },
                new object[] { "migrate -db", typeof(ArgumentException), "Database Type Required. use 'help' option for help" },
                new object[] { "migrate -db foo", typeof(ArgumentException), "foo is not a supported Database. use 'help' option for help" },
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
