using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sql
{
    internal abstract class SqlCommandBase<TSqlConnection, TSqlStringBuilder> 
        where TSqlConnection : DbConnection 
        where TSqlStringBuilder : DbConnectionStringBuilder
    {
        protected abstract string CheckIfExistQuery { get; }

        protected abstract string VersionTableName { get; }

        protected abstract void VerifyDatabaseExists(string connectionString);

        protected TSqlStringBuilder MakeConnectionString(string connectionString)
        {
            return Activator.CreateInstance(typeof(TSqlStringBuilder), new object[] { connectionString }) as TSqlStringBuilder;
        }

        protected TSqlConnection MakeConnection(string connectionString)
        {
            return Activator.CreateInstance(typeof(TSqlConnection), new object[] { connectionString }) as TSqlConnection;
        }

        public bool VersionTableExist(TSqlConnection connection)
        {
            connection.CreateCommand();
            
            using(var command = connection.CreateCommand())
            {
                command.CommandText = this.CheckIfExistQuery;
                using (var reader = command.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        protected void Init(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier, string createQuery)
        {
            this.VerifyDatabaseExists(options.ConnectionString);
            using (var connection = MakeConnection(options.ConnectionString))
            {
                connection.Open();
                try
                {
                    if (!this.VersionTableExist(connection))
                    {
                        CreateVersionTable(connection, createQuery);
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

        private static void CreateVersionTable(TSqlConnection connection, string createQuery)
        {
            var command = connection.CreateCommand();
            command.CommandText = createQuery;
            command.ExecuteNonQuery();
        }

        protected void Migrate(DirtyOptions options, IVersionComparer versionComparer)
        {
            this.VerifyDatabaseExists(options.ConnectionString);
            using (var connection = MakeConnection(options.ConnectionString))
            {
                connection.Open();
                try
                {
                    if (!this.VersionTableExist(connection))
                    {
                        throw new DirtybaseException(Constants.DatabaseNotInitialized);
                    }
                    var existingVersions = this.GetExistingVersions(connection);
                    var filesToApply = versionComparer.GetNewVersions(options, existingVersions.ToList());
                    this.Applyfiles(connection, filesToApply);
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
                connection.Close();
            }
        }

        private IEnumerable<DirtybaseVersion> GetExistingVersions(TSqlConnection connection)
        {
            var versionTableSelect = "SELECT * FROM " + VersionTableName;
            var versions = new List<DirtybaseVersion>();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = versionTableSelect;
                using (var datareader = command.ExecuteReader())
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

        private void Applyfiles(TSqlConnection connection, IEnumerable<DirtybaseVersion> filesToApply)
        {
            foreach (var file in filesToApply)
            {
                var fileInfo = new FileInfo(file.FilePath);
                string script = fileInfo.OpenText().ReadToEnd();
                ApplyScript(connection, script);
                this.InsertVersion(connection, file);
            }
        }

        private static void ApplyScript(TSqlConnection connection, string script)
        {
            using (var command = connection.CreateCommand())
            {
                var statements = SplitSqlStatements(script);
                var transaction = connection.BeginTransaction();
                command.Transaction = transaction;
                foreach (var statement in statements)
                {
                    command.CommandText = statement;
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private void InsertVersion(TSqlConnection connection, DirtybaseVersion file)
        {
            var insertQuery = string.Format("INSERT INTO {0} (Version, FileName, DateAppliedUtc) VALUES ('{1}', '{2}', '{3}')", this.VersionTableName, file.Version, file.FileName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            using (var command = connection.CreateCommand())
            {
                command.CommandText = insertQuery;
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