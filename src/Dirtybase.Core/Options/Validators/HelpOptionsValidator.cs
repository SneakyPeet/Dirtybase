namespace Dirtybase.Core.Options.Validators
{
    class HelpOptionsValidator : IOptionsValidator
    {
        public Errors Errors(DirtyOptions options)
        {
            return new Errors();
        }
    }
}