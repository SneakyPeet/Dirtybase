using Dirtybase.App.Commands;
using Dirtybase.App.VersionComparison;

namespace Dirtybase.App.Implementations.Sql
{
    class SqlMigrateCommand : IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer)
        {
            throw new System.NotImplementedException();
        }
    }
}