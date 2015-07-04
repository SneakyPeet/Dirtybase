using System;
using System.Linq;

namespace Dirtybase.Core.Options.Validators
{
    static class OptionsExtentions
    {
        public static Errors Validate(this DirtyOptions options, string validatorName)
        {
            var validator = ResolveValidator(validatorName);
            return validator.Errors(options);
        }

        private static IOptionsValidator ResolveValidator(string validatorName)
        {
            var assembly = typeof(IOptionsValidator).Assembly;
            var types = assembly.GetTypes();
            var validatorClassName = validatorName.ToOptionsValidatorConvetion();
            var validatorType = types.FirstOrDefault(t => t.Name == validatorClassName);
            var validator = (IOptionsValidator)Activator.CreateInstance(validatorType);
            return validator;
        }

        public static string Name(this Enum input)
        {
            return input.ToString("f");
        }
    }
}