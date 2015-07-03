using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dirtybase.App.Commands;
using Dirtybase.App.Exceptions;
using Dirtybase.App.VersionComparison;

namespace Dirtybase.App.Implementations.Sqlite
{
    internal class SqliteMigrateCommand : SqliteCommandBase, IDirtyCommand
    {
        const string versionTableSelect = "SELECT * FROM " + versionTableName;

        public void Execute(DirtyOptions options, IVersionComparer versionComparer)
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
                    var filesToApply = versionComparer.GetNewVersions(options, existingVersions.ToList());
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
                ApplyScript(connection, script);
                InsertVersion(connection, file);
            }
        }

        private static void ApplyScript(SQLiteConnection connection, string script)
        {
            using(var command = new SQLiteCommand(connection))
            {
                var statements = SplitSqlStatements(script);
                var transaction = connection.BeginTransaction();
                foreach(var statement in statements)
                {
                    command.CommandText = statement;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
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

        //from http://stackoverflow.com/questions/18596876/go-statements-blowing-up-sql-execution-in-net/18597052#18597052
        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                    sqlScript,
                    @"^\s*GO\s* ($ | \-\- .*$)",
                    RegexOptions.Multiline |
                    RegexOptions.IgnorePatternWhitespace |
                    RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'));
        }
    }
}