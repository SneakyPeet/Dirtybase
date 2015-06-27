using System;
using System.Linq;
using Dirtybase.App.Implementations.Help;

namespace Dirtybase.App.Commands
{
    public class CommandFactory
    {
        public IDirtyCommand Make(DirtyOptions options)
        {
            if(options.Command == DirtyCommand.Help)
            {
                return new HelpCommand();
            }
            var types = this.GetType().Assembly.GetTypes();
            var commandName = string.Format("{0}{1}{2}", options.Database, options.Command, Constants.CommandConvention);
            var commandType = types.FirstOrDefault(t => t.Name == commandName);
            return (IDirtyCommand)Activator.CreateInstance(commandType);
        }
    }
}