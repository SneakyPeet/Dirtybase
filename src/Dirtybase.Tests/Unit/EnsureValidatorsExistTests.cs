using System;
using System.Reflection;
using Dirtybase.App;
using Dirtybase.App.Options.Validators;
using NUnit.Framework;
using SharpTestsEx;

namespace Dirtybase.Tests.Unit
{
    [TestFixture]
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
            foreach(T item in (T[])Enum.GetValues(typeof(T)))
            {
                var baseType = typeof(IOptionsValidator);
                var baseNameSpace = baseType.Namespace;
                var className = baseNameSpace + "." + item.ToString() + Constants.OptionsValidatorConvention;
                var assembly = Assembly.GetAssembly(typeof(IOptionsValidator));
                Type resultType = assembly.GetType(className);
                var errors = new Errors();
                if(resultType == null)
                {
                    errors.Add(item.ToString() + " OptionsValidator Not Implemented");
                }
                errors.ThrowIfErrors();
            }
        }
    }
}
