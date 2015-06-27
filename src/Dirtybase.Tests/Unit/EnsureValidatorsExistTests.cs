using System;
using System.Linq;
using System.Reflection;
using Dirtybase.App;
using Dirtybase.App.Options;
using Dirtybase.App.Options.Validators;
using NUnit.Framework;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class EnsureValidatorsExistTests
    {
        [Test]
        public void EnsureCommandValidatorsAreImplemented()
        {
            AssertValidatorsExistForEnum<DirtyCommand>();
        }

        [Test]
        public void EnsureDatabaseValidatorsAreImplemented()
        {
            AssertValidatorsExistForEnum<DatabaseType>();
        }

        private static void AssertValidatorsExistForEnum<T>()
        {
            var errors = new Errors();
            var optionsValidatorInterfaceType = typeof(IOptionsValidator);
            var assembly = Assembly.GetAssembly(optionsValidatorInterfaceType);
            var types = assembly.GetTypes().ToList();
            foreach (T item in (T[])Enum.GetValues(typeof(T)))
            {
                var className = item.ToString() + Constants.OptionsValidatorConvention;
                var validatorType = types.FirstOrDefault(t => t.Name == className);
                if (validatorType == null)
                {
                    errors.Add(string.Format("{0} Not Implemented", item.ToString().ToOptionsValidatorConvetion()));
                }
                else if (!optionsValidatorInterfaceType.IsAssignableFrom(validatorType))
                {
                    errors.Add(string.Format("{0} Does Not Implemented {1}", validatorType.Name, optionsValidatorInterfaceType.Name));
                }
            }
            if (errors.Any())
            {
                Assert.Fail(errors.Message);
            }
        }
    }
}
