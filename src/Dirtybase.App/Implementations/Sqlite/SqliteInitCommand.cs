using System;
using System.Data.SQLite;
using Dirtybase.App.Commands;

namespace Dirtybase.App.Implementations.Sqlite
{
    class SqliteInitCommand : SqliteCommandBase, IDirtyCommand
    {
        private const string createVersionTableQuery = "CREATE TABLE " + versionTableName + "(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)";
        
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