using System;
using Dirtybase.App;
using Dirtybase.App.Commands;
using Dirtybase.App.Implementations.Help;
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
                new object[] { MakeOptions(DirtyCommand.Help), typeof(HelpCommand)},
            };

        [Test]
        [TestCaseSource("positiveTestCases")]
        public void GivenOptionsReturnCorrectCommandType(DirtyOptions options, Type expectedCommandType)
        {
            var command = this.commandFactory.Make(options);
            command.GetType().Should().Be.EqualTo(expectedCommandType);
        }

        private static DirtyOptions MakeOptions(DirtyCommand command)
        {
            return new DirtyOptions(command, null, null);
        }
    }
}
