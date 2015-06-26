using System;
using System.Collections.Generic;
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
            errors.AddRange(CheckConnectionString(options.ConnectionString));
            return errors;
        }

        private static IEnumerable<string> CheckConnectionString(string conncetionString)
        {
            try
            {
                new SqlConnectionStringBuilder(conncetionString);
            }
            catch(Exception)
            {
                return new Errors{Constants.InvalidConnectionString};
            }
            return new Errors();
        }
    }
}