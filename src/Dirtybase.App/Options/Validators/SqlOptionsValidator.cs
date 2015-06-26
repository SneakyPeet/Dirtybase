using System.Data.SqlClient;

namespace Dirtybase.App.Options.Validators
{
    class SqlOptionsValidator : ConnectionStringValidator, IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return ValidateConnectionString<SqlConnectionStringBuilder>(options);
        }
    }
}