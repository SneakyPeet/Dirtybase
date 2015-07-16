using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sql.Sqlite
{
    internal class SqliteMigrateCommand : SqliteCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            this.Migrate(options, versionComparer);
        }
    }
}