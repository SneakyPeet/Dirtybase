using System.Data.SQLite;
using System.IO;
using Dirtybase.App.Exceptions;

namespace Dirtybase.App.Implementations.Sqlite
{
    internal class SqliteCommandBase
    {
        protected const string versionTableName = "DirtybaseVersions";
        protected const string checkIfExistQuery = "SELECT name FROM sqlite_master WHERE name ='" + versionTableName + "';";

        protected void VerifyDatabaseExists(string connectionString)
        {
            var builder = new SQLiteConnectionStringBuilder(connectionString);
            if (!File.Exists(builder.DataSource))
            {
                throw new DirtybaseException("Database Does Not Exist");
            }
        }

        protected bool VersionTableExist(SQLiteConnection connection)
        {
            using(var command = new SQLiteCommand(checkIfExistQuery, connection)) 
            {
                using (var reader = command.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }
    }
}