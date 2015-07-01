using Dirtybase.App.VersionComparison;

namespace Dirtybase.App.Commands
{
    public interface IDirtyCommand
    {
        void Execute(DirtyOptions options, IVersionComparer versionComparer);
    }
}