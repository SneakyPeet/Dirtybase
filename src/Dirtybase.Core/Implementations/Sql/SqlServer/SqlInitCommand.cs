using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sql.SqlServer
{
    class SqlInitCommand : SqlServerCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            var createVersionTableQuery = "CREATE SCHEMA Dirty CREATE TABLE " + VersionTableName + "(Version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateAppliedUtc datetime)";
            Init(options, versionComparer, notifier, createVersionTableQuery);
        }
    }
}