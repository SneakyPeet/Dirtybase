using System;
using System.Data.SQLite;
using Dirtybase.App.Commands;

namespace Dirtybase.App.Implementations.Sqlite
{
    class SqliteInitCommand : IDirtyCommand
    {
        public void Execute(DirtyOptions options)
        {
            using (var connection = new SQLiteConnection(options.ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                try
                {
                    command.CommandText = "CREATE TABLE DirtybaseVersion(version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateApplied datetime)";
                    command.ExecuteNonQuery();
                }
                catch (Exception)
                {
                    connection.Close();
                    throw;
                }
            }
        }
    }
}