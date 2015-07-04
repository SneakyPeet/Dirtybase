using System;
using System.Collections.Generic;
using System.Linq;
using Dirtybase.Core.Exceptions;
using Dirtybase.Core.Options.Validators;

namespace Dirtybase.Core.Options
{
    public class DirtyOptions
    {
        public DirtyCommand Command { get; private set; }
        public DatabaseType? Database { get; private set; }
        public string ConnectionString { get; private set; }
        public string ScriptFolder { get; private set; }

        public DirtyOptions(DirtyCommand command, DatabaseType? dbType, string connectionString, string scriptFolder)
        {
            this.Command = command;
            this.Database = dbType;
            this.ConnectionString = connectionString;
            this.ScriptFolder = scriptFolder;
        }

        public DirtyOptions(string[] input)
        {
            var args = ResplitArguments(input);
            this.ParseArguments(args);
            this.ValidateOptions();
        }

        private static string[] ResplitArguments(string[] input)
        {
            var argsAsString = String.Join(" ", input);
            var sepperators = new[] { " -" };
            var args = argsAsString.Split(sepperators, StringSplitOptions.None);
            if(args == null || args.Length == 0)
            {
                throw new DirtybaseException(Constants.HelpMessage);
            }
            return args;
        }

        private void ParseArguments(string[] args)
        {
            var option = args[0];
            switch(option)
            {
                case "init": 
                    this.ParseCommand(args,DirtyCommand.Init);
                    break;
                case "migrate":
                    this.ParseCommand(args, DirtyCommand.Migrate);
                    break;
                case "help":
                    this.ParseCommand(args, DirtyCommand.Help);
                    break;
                case "":
                    throw new DirtybaseException(Constants.HelpMessage);
                default:
                    throw new DirtybaseException(option + Constants.NotAnOption);
            }
        }

        private void ParseCommand(string[] args, DirtyCommand command)
        {
            this.Command = command;
            this.ParseNextOption(args);
        }

        private void ParseNextOption(IEnumerable<string> args)
        {
            this.ParseOptions(args.Skip(1).ToArray());
        }

        private void ParseOptions(string[] args)
        {
            if (args.Length == 0)
            {
                return;
            }
            var option = args[0].Substring(0, 2);
            switch(option)
            {
                case "db":
                    this.SetDatabase(args);
                    break;
                case "cs":
                    this.SetConnectionString(args);
                    break;
                case "vf":
                    this.SetScriptFolder(args);
                    break;
                case "":
                    throw new DirtybaseException(Constants.HelpMessage);
                default:
                    throw new DirtybaseException(option + Constants.NotAnOption);
            }
            this.ParseNextOption(args);
        }

        private void SetDatabase(string[] args)
        {
            var db = GetSuppliedOption(args);
            switch (db)
            {
                case "sql":
                    this.Database = DatabaseType.Sql;
                    break;
                case "sqlite":
                    this.Database = DatabaseType.Sqlite;
                    break;
                case "":
                    throw new DirtybaseException(Constants.DatabaseTypeRequired);
                default:
                    throw new DirtybaseException(db + Constants.DatabaseNotSupported);
            }
        }

        private void SetConnectionString(string[] args)
        {
            this.ConnectionString = GetSuppliedOption(args);
        }

        private void SetScriptFolder(string[] args)
        {
            this.ScriptFolder = GetSuppliedOption(args);
        }

        private static string GetSuppliedOption(string[] args)
        {
            var option = args[0];
            if(option.Length > 2)
            {
                return args[0].Substring(3);
            }
            return string.Empty;
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
            return this.Command == other.Command
                && this.Database == other.Database
                && this.ConnectionString == other.ConnectionString
                && this.ScriptFolder == other.ScriptFolder;
        }

        public override int GetHashCode()
        {
            return (int)this.Command
                + (this.Database.HasValue ? (int)this.Database.Value : 0)
                + this.ConnectionString.GetHashCode()
                + this.ScriptFolder.GetHashCode();
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