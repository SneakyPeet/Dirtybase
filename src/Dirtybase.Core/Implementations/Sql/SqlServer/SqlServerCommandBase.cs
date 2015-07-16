using System;
using System.Data.SqlClient;
using Dirtybase.Core.Exceptions;

namespace Dirtybase.Core.Implementations.Sql.SqlServer
{
    internal class SqlServerCommandBase : SqlCommandBase<SqlConnection,SqlConnectionStringBuilder>
    {
        protected override string CheckIfExistQuery
        {
            get
            {
                return "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA = 'Dirty' AND TABLE_NAME ='" + this.versionTableName + "';";
            }
        }

        protected string schema = "Dirty";
        protected string versionTableName = "Versions";

        protected override string VersionTableName
        {
            get
            {
                return string.Format("{0}.{1}", this.schema, this.versionTableName);
            }
        }

        protected override void VerifyDatabaseExists(string connectionString)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                }
            }
            catch(Exception)
            {
                throw new DirtybaseException("Database Does Not Exist");
            }
        }
    }
}