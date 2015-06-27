using System;
using System.Data.SQLite;
using System.IO;
using Dirtybase.App.Commands;

namespace Dirtybase.App.Implementations.Sqlite
{
    class SqliteInitCommand : IDirtyCommand
    {
        private const string versionTableName = "DirtybaseVersions";
        private const string checkIfExistQuery = "SELECT name FROM sqlite_master WHERE name ='" + versionTableName + "';";
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

        private void VerifyDatabaseExists(string connectionString)
        {
            var builder = new SQLiteConnectionStringBuilder(connectionString);
            if (!File.Exists(builder.DataSource))
            {
                throw new DirtybaseException("Database Does Not Exist");
            }
        }

        private bool VersionTableExist(SQLiteConnection connection)
        {
            var command = new SQLiteCommand(checkIfExistQuery, connection);
            using (var reader = command.ExecuteReader())
            {
                return reader.HasRows;
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