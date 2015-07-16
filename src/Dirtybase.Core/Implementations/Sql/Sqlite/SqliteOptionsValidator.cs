using System.Data.SQLite;
using Dirtybase.Core.Options;
using Dirtybase.Core.Options.Validators;

namespace Dirtybase.Core.Implementations.Sql.Sqlite
{
    class SqliteOptionsValidator : ConnectionStringValidator, IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return ValidateConnectionString<SQLiteConnectionStringBuilder>(options);
        }
    }
}