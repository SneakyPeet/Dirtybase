using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sql.Sqlite
{
    class SqliteInitCommand : SqliteCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            var createVersionTableQuery = "CREATE TABLE " + VersionTableName + "(Version nvarchar(20) PRIMARY KEY, FileName nvarchar(256), DateAppliedUtc datetime)";
            Init(options, versionComparer, notifier, createVersionTableQuery);
        }
    }
}