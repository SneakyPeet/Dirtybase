using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dirtybase.App;
using Dirtybase.App.Commands;
using Dirtybase.App.Implementations.Help;
using Dirtybase.App.Options;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    [Category(TestTypes.Unit)]
    public class CommandFactoryTests
    {
        private CommandFactory commandFactory;

        [SetUp]
        public void SetUp()
        {
            commandFactory = new CommandFactory();
        }

        [Test]
        [TestCaseSource(typeof(CommandFactoryTestSource), "TestCases")]
        public void GivenOptionsReturnCorrectCommandType(DirtyOptions options, Type expectedCommandType)
        {
            var command = this.commandFactory.Make(options);
            command.GetType().Should().Be.EqualTo(expectedCommandType);
        }
    }

    public class CommandFactoryTestSource
    {
        public static IEnumerable TestCases
        {
            get
            {
                var testcases = new List<object>();
                var types = typeof(IDirtyCommand).Assembly.GetTypes();
                foreach (DirtyCommand command in (DirtyCommand[])Enum.GetValues(typeof(DirtyCommand)))
                {
                    if (command == DirtyCommand.Help)
                    {
                        testcases.Add(new object[] { MakeOptions(DirtyCommand.Help, null), typeof(HelpCommand) });
                    }
                    else
                    {
                        foreach (DatabaseType database in (DatabaseType[])Enum.GetValues(typeof(DatabaseType)))
                        {
                            var type = types.FirstOrDefault(t => t.Name == database.ToCommandConvetion(command));
                            if (type != null)
                            {
                                testcases.Add(new object[] { MakeOptions(command, database), type });
                            }
                        }
                    }
                }
                return testcases;
            }
        }

        private static DirtyOptions MakeOptions(DirtyCommand command, DatabaseType? databaseType)
        {
            return new DirtyOptions(command, databaseType, null, null);
        }
    }
}
