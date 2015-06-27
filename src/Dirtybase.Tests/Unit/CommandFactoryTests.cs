using System;
using Dirtybase.App;
using Dirtybase.App.Commands;
using Dirtybase.App.Implementations.Help;
using Dirtybase.App.Implementations.Sql;
using Dirtybase.App.Implementations.Sqlite;
using Dirtybase.App.Options;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class CommandFactoryTests
    {

        private CommandFactory commandFactory;

        [SetUp]
        public void SetUp()
        {
            commandFactory = new CommandFactory();
        }

        private static readonly object[] positiveTestCases =
            {
                new object[] { MakeOptions(DirtyCommand.Help, null), typeof(HelpCommand)},
                new object[] { MakeOptions(DirtyCommand.Init, DatabaseType.Sqlite), typeof(SqliteInitCommand)},
                new object[] { MakeOptions(DirtyCommand.Migrate, DatabaseType.Sqlite), typeof(SqliteMigrateCommand)},
                new object[] { MakeOptions(DirtyCommand.Init, DatabaseType.Sql), typeof(SqlInitCommand)},
                new object[] { MakeOptions(DirtyCommand.Migrate, DatabaseType.Sql), typeof(SqlMigrateCommand)}
            };

        [Test]
        [TestCaseSource("positiveTestCases")]
        public void GivenOptionsReturnCorrectCommandType(DirtyOptions options, Type expectedCommandType)
        {
            var command = this.commandFactory.Make(options);
            command.GetType().Should().Be.EqualTo(expectedCommandType);
        }

        private static DirtyOptions MakeOptions(DirtyCommand command, DatabaseType? databaseType)
        {
            return new DirtyOptions(command, databaseType, null);
        }
    }
}
