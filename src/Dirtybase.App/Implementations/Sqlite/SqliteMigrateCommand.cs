using System;
using System.Data.SQLite;
using Dirtybase.App.Commands;

namespace Dirtybase.App.Implementations.Sqlite
{
    class SqliteMigrateCommand : SqliteCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options)
        {
            VerifyDatabaseExists(options.ConnectionString);
            using (var connection = new SQLiteConnection(options.ConnectionString))
            {
                connection.Open();
                try
                {
                    if (!VersionTableExist(connection))
                    {
                        throw new DirtybaseException(Constants.DatabaseNotInitialized);
                    }
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }
    }
}