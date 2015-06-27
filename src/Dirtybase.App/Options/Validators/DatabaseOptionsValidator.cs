namespace Dirtybase.App.Options.Validators
{
    internal class DatabaseOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            if (!options.Database.HasValue)
            {
                return new Errors { Constants.DatabaseTypeRequired };
            }
            return options.Validate(options.Database.Value.Name());
        }
    }
}