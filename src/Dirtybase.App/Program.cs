using Dirtybase.App.Commands;
using Dirtybase.App.VersionComparison;

namespace Dirtybase.App
{
    class Program
    {
        public static void Main(string[] args)
        {
            var options = new DirtyOptions(args);
            var commandFactory = new CommandFactory();
            var command = commandFactory.Make(options);
            command.Execute(options, new VersionComparer());
        }
    }
}
