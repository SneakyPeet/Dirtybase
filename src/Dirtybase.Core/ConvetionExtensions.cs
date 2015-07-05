using System;
using Dirtybase.Core.Options;

namespace Dirtybase.Core
{
    public static class ConvetionExtensions
    {
        public static string ToCommandConvetion(this DatabaseType datebaseType, DirtyCommand command)
        {
            return string.Format("{0}{1}{2}", datebaseType, command, Constants.CommandConvention);
        }

        public static string ToOptionsValidatorConvetion(this Enum input)
        {
            return input.ToString().ToOptionsValidatorConvetion();
        }

        public static string ToOptionsValidatorConvetion(this string input)
        {
            return string.Format("{0}{1}", input, Constants.OptionsValidatorConvention);
        }

        
    }
}