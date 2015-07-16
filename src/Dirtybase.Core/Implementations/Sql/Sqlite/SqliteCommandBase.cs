using System.Data.SQLite;
using System.IO;
using Dirtybase.Core.Exceptions;

namespace Dirtybase.Core.Implementations.Sql.Sqlite
{
    internal class SqliteCommandBase : SqlCommandBase<SQLiteConnection,SQLiteConnectionStringBuilder>
    {
        protected override string CheckIfExistQuery
        {
            get
            {
                return "SELECT name FROM sqlite_master WHERE name ='" + this.VersionTableName + "';";
            }
        }

        protected override string VersionTableName
        {
            get
            {
                return "DirtybaseVersions";
            }
        }

        protected override void VerifyDatabaseExists(string connectionString)
        {
            var builder = this.MakeConnectionString(connectionString);
            if (!File.Exists(builder["data source"] as string))
            {
                throw new DirtybaseException("Database Does Not Exist");
            }
        }
    }
}