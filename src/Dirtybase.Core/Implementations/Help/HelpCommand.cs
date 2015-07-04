using Dirtybase.Core.Commands;
using Dirtybase.Core.Options;
using Dirtybase.Core.VersionComparison;

namespace Dirtybase.Core.Implementations.Help
{
    internal class HelpCommand : IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer, INotifier notifier)
        {
            notifier.SendInfo("Help Can Be Found At https://github.com/SneakyPeet/Dirtybase");
        }
    }
}