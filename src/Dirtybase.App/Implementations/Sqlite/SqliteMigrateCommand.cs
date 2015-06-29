using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
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
                    Applyfiles(connection, filesToApply);
                }
                catch(Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }

        private void Applyfiles(SQLiteConnection connection, IEnumerable<DirtybaseVersion> filesToApply)
        {
            foreach(var file in filesToApply)
            {
                var fileInfo = new FileInfo(file.FilePath);
                string script = fileInfo.OpenText().ReadToEnd();
                var command = new SQLiteCommand(script, connection);
                command.ExecuteNonQuery();
                InsertVersion(connection, file);
            }
        }

        private void InsertVersion(SQLiteConnection connection, DirtybaseVersion file)
        {
            var insert = string.Format("INSERT INTO {0} (Version, FileName, DateAppliedUtc) VALUES ('{1}', '{2}', '{3}')", versionTableName, file.Version, file.FileName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            var command = new SQLiteCommand(insert, connection);
            command.ExecuteNonQuery();
        }
    }
}