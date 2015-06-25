namespace Dirtybase.App.Options.Validators
{
    class SqliteOptionsValidator : IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return new Errors();
        }
    }
}