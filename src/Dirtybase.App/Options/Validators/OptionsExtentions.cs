using System;

namespace Dirtybase.App.Options.Validators
{
    static class OptionsExtentions
    {
        public static Errors Validate(this DirtyOptions options, string validatorName)
        {
            var baseType = typeof(IOptionsValidator);
            var className = string.Format("{0}.{1}{2}", baseType.Namespace, validatorName, Constants.OptionsValidatorConvention);
            var validatorType = baseType.Assembly.GetType(className, true);
            var validator = (IOptionsValidator)Activator.CreateInstance(validatorType);
            var errors = validator.Errors(options);
            return errors;
        }

        public static string Name(this Enum input)
        {
            return input.ToString("f");
        }
    }
}