using System;
using System.Linq;
using Dirtybase.Core.Implementations.Help;
using Dirtybase.Core.Options;

namespace Dirtybase.Core.Commands
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
            var commandName = options.Database.Value.ToCommandConvetion(options.Command);
            var commandType = types.FirstOrDefault(t => t.Name == commandName);
            return (IDirtyCommand)Activator.CreateInstance(commandType);
        }
    }
}