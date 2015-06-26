using System.Data.SQLite;

namespace Dirtybase.App.Options.Validators
{
    class SqliteOptionsValidator : ConnectionStringValidator, IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return ValidateConnectionString<SQLiteConnectionStringBuilder>(options);
        }
    }
}