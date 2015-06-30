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
        const string versionTableSelect = "SELECT * FROM " + versionTableName;

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
                    var existingVersions = GetExistingVersions(connection);
                    var filesToApply = versionComparor.GetNewVersions(options, existingVersions);
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

        private IEnumerable<DirtybaseVersion> GetExistingVersions(SQLiteConnection connection)
        {
            var versions = new List<DirtybaseVersion>();
            using(var command = new SQLiteCommand(versionTableSelect, connection))
            {
                using(var datareader = command.ExecuteReader())
                {
                    if (datareader.HasRows)
                    {
                        while (datareader.Read())
                        {
                            versions.Add(new DirtybaseVersion(datareader.GetString(0), datareader.GetString(1)));
                        }
                    }
                }
            }
            return versions;
        }

        private void Applyfiles(SQLiteConnection connection, IEnumerable<DirtybaseVersion> filesToApply)
        {
            foreach(var file in filesToApply)
            {
                var fileInfo = new FileInfo(file.FilePath);
                string script = fileInfo.OpenText().ReadToEnd();
                using(var command = new SQLiteCommand(script, connection)) 
                {
                    command.ExecuteNonQuery();
                }
                InsertVersion(connection, file);
            }
        }

        private void InsertVersion(SQLiteConnection connection, DirtybaseVersion file)
        {
            var insert = string.Format("INSERT INTO {0} (Version, FileName, DateAppliedUtc) VALUES ('{1}', '{2}', '{3}')", versionTableName, file.Version, file.FileName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            using(var command = new SQLiteCommand(insert, connection)) 
            {
                command.ExecuteNonQuery();
            }
        }
    }
}