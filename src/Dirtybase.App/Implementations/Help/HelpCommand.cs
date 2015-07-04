using System;
using Dirtybase.App.Commands;
using Dirtybase.App.VersionComparison;

namespace Dirtybase.App.Implementations.Help
{
    internal class HelpCommand : IDirtyCommand
    {
        public void Execute(DirtyOptions options, IVersionComparer versionComparer)
        {
            Console.WriteLine("Help Can Be Found At https://github.com/SneakyPeet/Dirtybase");
        }
    }
}