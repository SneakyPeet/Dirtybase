using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Sql
{
    class SqlMigrateCommand : IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            throw new System.NotImplementedException();
        }
    }
}