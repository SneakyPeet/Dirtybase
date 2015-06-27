using System;
using System.Linq;
using System.Reflection;
using Dirtybase.App;
using Dirtybase.App.Commands;
using Dirtybase.App.Options;
using Dirtybase.App.Options.Validators;
using NUnit.Framework;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class EnsureCommandsExistTests
    {
        [Test]
        public void EnsureCommandValidatorsAreImplemented()
        {
            var errors = new Errors();
            foreach (DirtyCommand item in (DirtyCommand[])Enum.GetValues(typeof(DirtyCommand)))
            {
                if (item == DirtyCommand.Help)
                {
                    continue;
                }
                errors.AddRange(CheckIfCommandsExistForAllDatabaseTypes(item));
            }
            if (errors.Any())
            {
                Assert.Fail(errors.Message);
            }
        }

        private Errors CheckIfCommandsExistForAllDatabaseTypes(DirtyCommand command)
        {
            var errors = new Errors();
            var dirtyCommandInterfaceType = typeof(IDirtyCommand);
            var assembly = Assembly.GetAssembly(dirtyCommandInterfaceType);
            var types = assembly.GetTypes().ToList();
            foreach (DatabaseType item in (DatabaseType[])Enum.GetValues(typeof(DatabaseType)))
            {
                var className = string.Format("{0}{1}{2}", item.ToString(), command.ToString(), Constants.CommandConvention);
                var type = types.FirstOrDefault(t => t.Name == className);
                if (type == null)
                {
                    errors.Add(string.Format("{0}{1} Not Implemented", item.ToString(), Constants.CommandConvention));
                }
                else if (!dirtyCommandInterfaceType.IsAssignableFrom(type))
                {
                    errors.Add(string.Format("{0} Does Not Implemented {1}", type.Name, dirtyCommandInterfaceType.Name));
                }
            }
            return errors;
        }
    }
}
