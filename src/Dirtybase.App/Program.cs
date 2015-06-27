using Dirtybase.App.Commands;

namespace Dirtybase.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new DirtyOptions(args);
            var commandFactory = new CommandFactory();
            var command = commandFactory.Make(options);
            command.Execute(options);
        }
    }
}
