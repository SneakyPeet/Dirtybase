using System;
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
            throw new ArgumentException("Command Does Not Exist");
        }
    }
}