using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Dirtybase.Core.Options.Validators
{
    internal abstract class ConnectionStringValidator
    {
        public static Errors ValidateConnectionString<TBuilder>(DirtyOptions options) where TBuilder : DbConnectionStringBuilder
        {
            var errors = new Errors();
            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                errors.Add(Constants.InvalidConnectionString);
            }
            errors.AddRange(CheckConnectionString<TBuilder>(options.ConnectionString));
            return errors;
        }

        protected static IEnumerable<string> CheckConnectionString<TBuilder>(string conncetionString) where TBuilder : DbConnectionStringBuilder
        {
            try
            {
                Activator.CreateInstance(typeof(TBuilder), conncetionString);
            }
            catch (Exception)
            {
                return new Errors { Constants.InvalidConnectionString };
            }
            return new Errors();
        }
    }
}