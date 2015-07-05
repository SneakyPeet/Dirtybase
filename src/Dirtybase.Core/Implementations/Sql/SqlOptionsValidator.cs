using System.Data.SqlClient;
using Dirtybase.Core.Options;
using Dirtybase.Core.Options.Validators;

namespace Dirtybase.Core.Implementations.Sql
{
    class SqlOptionsValidator : ConnectionStringValidator, IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return ValidateConnectionString<SqlConnectionStringBuilder>(options);
        }
    }
}