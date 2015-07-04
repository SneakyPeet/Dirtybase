using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Commands
{
    public interface IDirtyCommand
    {
        void Execute(DirtyOptions options, IVersionComparer versionComparer);
    }
}