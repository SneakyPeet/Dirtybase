using System;
using System.Linq;
using System.Reflection;
using Dirtybase.App;
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
            var assembly = Assembly.GetAssembly(typeof(IOptionsValidator));
            var types = assembly.GetTypes().ToList();
            foreach (T item in (T[])Enum.GetValues(typeof(T)))
            {
                var className = item.ToString() + Constants.OptionsValidatorConvention;
                if (types.All(t => t.Name != className))
                {
                    errors.Add(item.ToString() + "OptionsValidator Not Implemented");
                }
            }
            errors.ThrowIfErrors();
        }

        //private static void AssertValidatorsExistForEnum<T>()
        //{
        //    foreach(T item in (T[])Enum.GetValues(typeof(T)))
        //    {
        //        var baseType = typeof(IOptionsValidator);
        //        var baseNameSpace = baseType.Namespace;
        //        var className = baseNameSpace + "." + item.ToString() + Constants.OptionsValidatorConvention;
        //        var assembly = Assembly.GetAssembly(typeof(IOptionsValidator));
        //        Type resultType = assembly.GetType(className);
        //        var errors = new Errors();
        //        if(resultType == null)
        //        {
        //            errors.Add(item.ToString() + " OptionsValidator Not Implemented");
        //        }
        //        errors.ThrowIfErrors();
        //    }
        //}
    }
}
