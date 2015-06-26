using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dirtybase.App.Options.Validators
{
    class SqlOptionsValidator : IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            var errors = new Errors();
            if(string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                errors.Add(Constants.InvalidConnectionString);
            }
            errors.AddRange(CheckConnectionString<SqlConnectionStringBuilder>(options.ConnectionString));
            return errors;
        }

        private static IEnumerable<string> CheckConnectionString<TBuilder>(string conncetionString) where TBuilder : DbConnectionStringBuilder
        {
            try
            {
                Activator.CreateInstance(typeof(TBuilder), conncetionString);
            }
            catch(Exception)
            {
                return new Errors{Constants.InvalidConnectionString};
            }
            return new Errors();
        }
    }
}