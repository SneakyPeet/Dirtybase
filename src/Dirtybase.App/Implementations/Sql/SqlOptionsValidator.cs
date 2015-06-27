using System.Data.SqlClient;
using Dirtybase.App.Options.Validators;

namespace Dirtybase.App.Implementations.Sql
{
    class SqlOptionsValidator : ConnectionStringValidator, IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return ValidateConnectionString<SqlConnectionStringBuilder>(options);
        }
    }
}