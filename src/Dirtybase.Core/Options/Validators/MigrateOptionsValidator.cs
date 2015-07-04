namespace Dirtybase.Core.Options.Validators
{
    class MigrateOptionsValidator : DatabaseOptionsValidator, IOptionsValidator 
    {
        public override Errors Errors(DirtyOptions options)
        {
            var errors = base.Errors(options);
            if(!options.Database.HasValue)
            {
                return errors;
            }
            if(string.IsNullOrWhiteSpace(options.ScriptFolder))
            {
                errors.Add(Constants.ScriptFolderNotSet);
            }
            return errors;
        }
    }
}