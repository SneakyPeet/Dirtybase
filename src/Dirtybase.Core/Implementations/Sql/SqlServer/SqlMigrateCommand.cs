using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sql.SqlServer
{
    class SqlMigrateCommand : SqlServerCommandBase, IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            Migrate(options, versionComparer);
        }
    }
}