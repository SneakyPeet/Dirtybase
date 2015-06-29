using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dirtybase.App.Commands;
using Dirtybase.App.Exceptions;
using Dirtybase.App.VersionComparison;

namespace Dirtybase.App.Implementations.Sqlite
{
    internal class SqliteMigrateCommand : SqliteCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparor versionComparor)
        {
            VerifyDatabaseExists(options.ConnectionString);
            using(var connection = new SQLiteConnection(options.ConnectionString))
            {
                connection.Open();
                try
                {
                    if(!VersionTableExist(connection))
                    {
                        throw new DirtybaseException(Constants.DatabaseNotInitialized);
                    }
                    //todo get list of existing versions
                    var versionsToApply = new List<DirtybaseVersion>();
                    var filesToApply = versionComparor.GetNewVersions(options, versionsToApply);
                }
                catch(Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }
    }
}