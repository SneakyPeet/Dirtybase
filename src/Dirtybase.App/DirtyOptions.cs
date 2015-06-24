using System;
using System.Collections.Generic;
using System.Linq;

namespace Dirtybase.App
{
    public class DirtyOptions
    {
        public DirtyCommand Command { get; private set; }

        public DirtyOptions(DirtyCommand command)
        {
            this.Command = command;
        }

        public DirtyOptions(string[] args)
        {
            if(args == null || args.Length == 0)
            {
                throw new ArgumentException("use 'help' option for help");
            }
            this.ParseArguments(args);
        }

        private void ParseArguments(string[] args)
        {
            if(args.Length == 0)
            {
                return;
            }
            var option = args[0];
            switch(option)
            {
                case "init": 
                    this.SetCommand(args,DirtyCommand.Init);
                    break;
                case "migrate":
                    this.SetCommand(args, DirtyCommand.Migrate);
                    break;
                case "help":
                    this.SetCommand(args, DirtyCommand.Help);
                    break;
                case "":
                    throw new ArgumentException("use 'help' option for help");
                default:
                    throw new ArgumentException(option + " is not an option. use 'help' option for help");
            }
        }

        private void SetCommand(IEnumerable<string> args, DirtyCommand command)
        {
            this.Command = command;
            this.ParseArguments(args.Skip(1).ToArray());
        }

        public override bool Equals(object obj)
        {
            if(obj is DirtyOptions)
            {
                return this.Equals(obj as DirtyOptions);
            }
            return false;
        }

        protected bool Equals(DirtyOptions other)
        {
            return this.Command == other.Command;
        }

        public override int GetHashCode()
        {
            return (int)this.Command;
        }

        public static bool operator ==(DirtyOptions left, DirtyOptions right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DirtyOptions left, DirtyOptions right)
        {
            return !Equals(left, right);
        }
    }
}