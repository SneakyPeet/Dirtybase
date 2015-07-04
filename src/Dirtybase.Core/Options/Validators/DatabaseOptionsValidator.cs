namespace Dirtybase.Core.Options.Validators
{
    internal class DatabaseOptionsValidator
    {
        public virtual Errors Errors(DirtyOptions options)
        {
            if (!options.Database.HasValue)
            {
                return new Errors { Constants.DatabaseTypeRequired };
            }
            return options.Validate(options.Database.Value.Name());
        }
    }
}