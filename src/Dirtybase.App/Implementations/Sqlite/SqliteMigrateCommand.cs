using Dirtybase.App.Commands;

namespace Dirtybase.App.Implementations.Sqlite
{
    class SqliteMigrateCommand : SqliteCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options)
        {
            VerifyDatabaseExists(options.ConnectionString);
        }
    }
}