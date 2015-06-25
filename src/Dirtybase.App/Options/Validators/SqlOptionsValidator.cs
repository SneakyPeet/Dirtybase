namespace Dirtybase.App.Options.Validators
{
    class SqlOptionsValidator : IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return new Errors();
        }
    }
}