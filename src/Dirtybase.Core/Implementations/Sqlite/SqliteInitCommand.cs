using System;
using System.Data.SQLite;
using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sqlite
{
    class SqliteInitCommand : SqliteCommandBase, IDirtyCommand
    {
        private const string createVersionTableQuery = "CREATE TABLE " + versionTableName + "(Version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateAppliedUtc datetime)";

        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            this.VerifyDatabaseExists(options.ConnectionString);
            using (var connection = new SQLiteConnection(options.ConnectionString))
            {
                connection.Open();
                try
                {
                    if (!this.VersionTableExist(connection))
                    {
                        CreateVersionTable(connection);
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

       private static void CreateVersionTable(SQLiteConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = createVersionTableQuery;
            command.ExecuteNonQuery();
        }
    }
}