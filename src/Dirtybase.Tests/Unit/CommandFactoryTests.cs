using System;
using Dirtybase.App;
using Dirtybase.App.Commands;
using Dirtybase.App.Implementations.Help;
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
                new object[] { MakeOptions(DirtyCommand.Init, DatabaseType.Sqlite), typeof(HelpCommand)},
                new object[] { MakeOptions(DirtyCommand.Init, DatabaseType.Sql), typeof(HelpCommand)}
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
