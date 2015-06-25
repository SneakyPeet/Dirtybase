using System;
using System.Collections.Generic;
using System.Linq;
using Dirtybase.App.Options.Validators;

namespace Dirtybase.App
{
    public class DirtyOptions
    {
        public DirtyCommand Command { get; private set; }
        public DatabaseType? Database { get; private set; }

        public DirtyOptions(DirtyCommand command, DatabaseType? dbType)
        {
            this.Command = command;
            this.Database = dbType;
        }

        public DirtyOptions(string[] args)
        {
            if(args == null || args.Length == 0)
            {
                throw new ArgumentException(Constants.HelpMessage);
            }
            this.ParseArguments(args);
            this.ValidateOptions();
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
                case "-db":
                    this.SetDatabase(args);
                    break;
                case "":
                    throw new ArgumentException(Constants.HelpMessage);
                default:
                    throw new ArgumentException(option + Constants.NotAnOption);
            }
        }

        private void SetDatabase(string[] args)
        {
            if(args.Length < 2)
            {
                throw new ArgumentException(Constants.DatabaseTypeRequired);
            }
            var db = args[1];
            switch(db)
            {
                case "sql":
                    this.Database = DatabaseType.Sql;
                    break;
                case "sqlite":
                    this.Database = DatabaseType.Sqlite;
                    break;
                default:
                    throw new ArgumentException(db + Constants.DatabaseNotSupported);
            }
            ParseArguments(args.Skip(2).ToArray());
        }

        private void SetCommand(IEnumerable<string> args, DirtyCommand command)
        {
            this.Command = command;
            this.ParseArguments(args.Skip(1).ToArray());
        }

        private void ValidateOptions()
        {
            this.Validate(this.Command.Name()).ThrowIfErrors();
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