using System.Data.SQLite;
using Dirtybase.App.Options.Validators;

namespace Dirtybase.App.Implementations.Sqlite
{
    class SqliteOptionsValidator : ConnectionStringValidator, IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return ValidateConnectionString<SQLiteConnectionStringBuilder>(options);
        }
    }
}