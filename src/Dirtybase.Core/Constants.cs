namespace Dirtybase.Core
{
    public class Constants
    {
        public const string HelpMessage = "use 'help' option for help";
        public const string NotAnOption = " is not an option. " + HelpMessage;
        public const string DatabaseTypeRequired = "Database Type Required. " + HelpMessage;
        public const string DatabaseNotSupported = " is not a supported Database. " + HelpMessage;
        public const string InvalidConnectionString = "Invalid Connection String";
        public const string ScriptFolderNotSet = "Script Folder Not Set";
        public const string DatabaseNotInitialized = "Dirtybase Not Initialized. Run init Command";
        
        public const string OptionsValidatorConvention = "OptionsValidator";
        public static string CommandConvention = "Command";
    }
}